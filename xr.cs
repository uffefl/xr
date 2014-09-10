using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Xsl;

namespace xr
{

	static class Extensions
	{
		public static T Pop<T>(this IList<T> self)
		{
			var t = self[0];
			self.RemoveAt(0);
			return t;
		}
		public static void Push<T>(this IList<T> self, T t)
		{
			self.Insert(0, t);
		}
		public static void Push<T>(this IList<T> self, IEnumerable<T> ts)
		{
			var e = ts.GetEnumerator();
			for (var i=0; e.MoveNext(); i++)
			{
				self.Insert(i, e.Current);
			}
		}
		public static bool Empty<T>(this IList<T> self)
		{
			return self.Count == 0;
		}
		public static void Append<T>(this IList<T> self, T t)
		{
			self.Insert(self.Count, t);
		}
		public static void Append<T>(this IList<T> self, IEnumerable<T> ts)
		{
			var e = ts.GetEnumerator();
			for (var i = self.Count; e.MoveNext(); i++)
			{
				self.Insert(i, e.Current);
			}
		}
		public static IEnumerable<string> Lines(this StreamReader tr)
		{
			var lines = new List<string>();
			while (!tr.EndOfStream)
			{
				lines.Append(tr.ReadLine());
			}
			return lines;
		}
		public static IEnumerable<string> Lines(this FileInfo f)
		{
			using (var tr = f.OpenText()) return tr.Lines();
		}
		public static IEnumerable<string> Lines(this IEnumerable<FileInfo> fs)
		{
			return fs.SelectMany(f => f.Lines());
		}
		public static string All(this FileInfo f)
		{
			using (var tr = f.OpenText()) return tr.ReadToEnd();
		}
	}

	class Program
	{

		static void Syntax()
		{
			Console.Error.WriteLine("rer [-f|-s [path-with]<pattern> [-args <read-args-from-file>] [-re <regex> <replace>] [-o <output-file>]");
			Console.Error.WriteLine();
			Console.Error.WriteLine("  -args  reads more command line arguments from a file, one argument per line");
			Console.Error.WriteLine("  -f     read files matching a given pattern into the buffer, no recursive directory traversal");
			Console.Error.WriteLine("  -s     same but with recursive directory traversal");
			Console.Error.WriteLine("  -re    perform a regular expression replace operation on the buffer");
			Console.Error.WriteLine("  -o     writes the buffer to a file and clears the buffer (use - for stdout)");
		}

		class RegexReplace
		{
			public Regex Regex { get; set; }
			public string Replace { get; set; }
		}

		class Arg
		{
			public string arg;
			public DirectoryInfo cwd;
			public string option { get { return arg.StartsWith("-") || arg.StartsWith("/") ? arg.Substring(1).ToLowerInvariant() : null; } }
		}

		static void Main(string[] raws)
		{
			var cwd = new DirectoryInfo(".");
			var args = raws.Select(arg => new Arg { arg = arg, cwd = cwd }).ToList();
			var text = "";
			while (!args.Empty())
			{
				var arg = args.Pop();
				switch (arg.option)
				{
					// include arguments from external file
					case "args":
					{
						var filename = args.Pop().arg;
						var path = Path.Combine(arg.cwd.FullName, filename);
						var file = new FileInfo(path);
						using (var tr = file.OpenText())
						{
							args.Push(tr.Lines().Select(line => new Arg { arg = line, cwd = file.Directory }));
						}
						break;
					}
					// include files matching a given pattern
					case "f": // current directory only
					case "s": // recursive
					{
						var pathAndPattern = args.Pop().arg;
						var path = Path.Combine(arg.cwd.FullName, pathAndPattern);
						var dir = new DirectoryInfo(Path.GetDirectoryName(path));
						var pattern = Path.GetFileName(path);
						text = dir.EnumerateFiles(pattern, arg.option == "f" ? SearchOption.TopDirectoryOnly : SearchOption.AllDirectories).Aggregate(text, (current, file) => current + file.All());
						break;
					}
					// perform a regular expression replace operation
					case "re":
					{
						var regex = new Regex(args.Pop().arg);
						var replace = args.Pop().arg.Replace("\\n","\n").Replace("\\t","\t").Replace("\\\\","\\");
						text = regex.Replace(text, replace);
						break;
					}
					// keep performing a regular expression replace operation until the buffer stops changing
					case "rei":
					{
						var regex = new Regex(args.Pop().arg);
						var replace = args.Pop().arg.Replace("\\n", "\n").Replace("\\t", "\t").Replace("\\\\", "\\");
						var was = text;
						for (var i = 0; i < 1000; i++)
						{
							text = regex.Replace(text, replace);
							if (was == text) break;
							was = text;
						}
						break;
					}
					// run the buffer through an xsl transform
					case "xsl":
					{
						var filename = Path.Combine(arg.cwd.FullName, args.Pop().arg);
						var xsl = new XslCompiledTransform();
						xsl.Load(filename);
						var xr = XmlReader.Create(new StringReader(text));
						var sb = new StringBuilder();
						var xw = new StringWriter(sb);
						xsl.Transform(xr, null, xw);
						text = sb.ToString();
						break;
					}
					// output text buffer
					case "o":
					{
						var file = args.Pop().arg;
						if (file == "-")
						{
							Console.Out.Write(text);
						}
						else
						{
							using (var tw = new FileInfo(Path.Combine(arg.cwd.FullName, file)).CreateText())
							{
								tw.Write(text);
								tw.Flush();
								tw.Close();
							}
						}
						text = "";
						break;
					}
					default:
					{
						Syntax();
						return;
					}
				}
			}

			if (text != "")
			{
				Console.Out.Write(text);
			}

		}

	}


}
