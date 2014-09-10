<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">
	
	<xsl:output method="xml" indent="yes"/>

	<xsl:template match="PART">
		<xsl:apply-templates select="MODULE">
			<xsl:with-param name="part" select="name"/>
		</xsl:apply-templates>
	</xsl:template>
	
	<xsl:template match="MODULE[name='KolonyConverter']">
		<xsl:param name="part"/>
		<recipe>
			<name>
				<xsl:value-of select="converterName"/>
			</name>
			<part>
				<xsl:value-of select="$part"/>
			</part>
			<input>
				<xsl:value-of select="inputResources"/>
			</input>
			<output>
				<xsl:value-of select="outputResources"/>
			</output>
		</recipe>
	</xsl:template>

	<xsl:template match="MODULE"></xsl:template>

</xsl:stylesheet>
