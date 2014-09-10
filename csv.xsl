<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">
	
	<!-- generic CSV converter -->
	
	<xsl:output method="text"/>

	<xsl:variable name="sep" select="';'"/>
	<xsl:variable name="lf" select="'&#10;'"/>

	<xsl:key name="column" match="/*/*/*" use="name()"/>
	
	<xsl:template match="/">
		<xsl:for-each select="/*/*/*[generate-id() = generate-id(key('column',name())[1])]">
			<xsl:value-of select="name()"/>
			<xsl:if test="position()!=last()">
				<xsl:value-of select="$sep"/>
			</xsl:if>
		</xsl:for-each>
		<xsl:value-of select="$lf"/>
		<xsl:apply-templates select="/*/*">
			<xsl:sort select="name"/>
		</xsl:apply-templates>
	</xsl:template>


	<xsl:template match="//RESOURCE_DEFINITION">
		<xsl:variable name="in" select="."/>
		<xsl:for-each select="/*/*/*[generate-id() = generate-id(key('column',name())[1])]">
			<xsl:variable name="col" select="name()"/>
			<xsl:value-of select="$in/*[name()=$col]"/>
			<xsl:if test="position()!=last()">
				<xsl:value-of select="$sep"/>
			</xsl:if>
		</xsl:for-each>
		<xsl:value-of select="$lf"/>
	</xsl:template>
	
</xsl:stylesheet>
