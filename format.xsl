<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">
	
	<xsl:output method="xml" indent="yes"/>

	<xsl:template match="*">
		<xsl:element name="{name()}">
			<xsl:apply-templates select="*|text()|@*"/>
		</xsl:element>
	</xsl:template>

	<xsl:template match="text()">
		<xsl:value-of select="."/>
	</xsl:template>

</xsl:stylesheet>
