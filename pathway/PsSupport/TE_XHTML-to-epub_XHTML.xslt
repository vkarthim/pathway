<?xml version="1.0" encoding="UTF-8"?>
<!-- Transform XHTML file exported from TE into a simpler XHTML to be used in making a phone app. -->
<!-- ?xml version="1.0"? -->
<!-- From Larry W.'s TE_XHTML-to-Phone_XHTML.xslt -->
<!-- Modified October 6, 2010. -->

<xsl:transform version="1.0"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:xhtml="http://www.w3.org/1999/xhtml"
    xmlns="http://www.w3.org/1999/xhtml"
    xmlns:fn="http://www.w3.org/2005/xpath-functions"
    exclude-result-prefixes="xhtml">
    
	<xsl:output method="xml" version="1.0" encoding="UTF-8" doctype-public="-//W3C//DTD XHTML 1.1//EN" doctype-system="http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd" indent="yes"/>

	<xsl:strip-space elements="*"/>

	<!-- Use a key to speed up the processing of the verses for each chapter. -->
	<xsl:key name="verses-by-chapter" match="xhtml:span[@class='Verse_Number']"
			use="generate-id(preceding::xhtml:span[@class='Chapter_Number'][1])" />

	<!--Straight copy for these elements. -->
	<xsl:template match="xhtml:html | xhtml:head | xhtml:title | xhtml:link | xhtml:body | xhtml:a">
		<xsl:copy>
			<xsl:for-each select="@*">
				<xsl:copy/>
			</xsl:for-each>
			<xsl:apply-templates/>
		</xsl:copy>
	</xsl:template>

	<!-- Special processing for a couple divs -->
	<xsl:template match="xhtml:div">
		<xsl:copy>
			<xsl:for-each select="@*">
				<xsl:copy/>
			</xsl:for-each>
			<xsl:apply-templates/>
			<xsl:if test="@class = 'scrSection'">
				<!-- If there are any footnotes / endnotes, list them here at the end of each section -->
				<xsl:if test="count(descendant::xhtml:span[@class='Note_General_Paragraph']) > 0">
					<xsl:element name="ul">
						<xsl:attribute name="class"><xsl:text>footnotes</xsl:text></xsl:attribute>
						<!-- general notes - use the note title for the list bullet -->
						<xsl:for-each select="descendant::xhtml:span[@class='Note_General_Paragraph']">
							<xsl:element name="li">
								<xsl:attribute name="id"><xsl:text>FN_</xsl:text><xsl:value-of select="@id"/></xsl:attribute>
								<xsl:element name="a">
									<xsl:attribute name="href"><xsl:text>#</xsl:text><xsl:value-of select="@id"/></xsl:attribute>
									<xsl:text>[</xsl:text><xsl:value-of select="@title"/><xsl:text>]</xsl:text>
								</xsl:element>
								<xsl:text> </xsl:text>
								<xsl:value-of select="."/>
							</xsl:element>
						</xsl:for-each>
						<!-- cross-references - use the verse number for the list bullet -->
						<xsl:for-each select="descendant::xhtml:span[@class='Note_CrossHYPHENReference_Paragraph']">
							<xsl:element name="li">
								<xsl:attribute name="id"><xsl:text>FN_</xsl:text><xsl:value-of select="@id"/></xsl:attribute>
								<xsl:element name="a">
									<xsl:attribute name="href"><xsl:text>#</xsl:text><xsl:value-of select="@id"/></xsl:attribute>
									<xsl:value-of select="preceding::xhtml:span[@class='Chapter_Number'][1]"/><xsl:text>:</xsl:text>
									<xsl:value-of select="preceding::xhtml:span[@class='Verse_Number'][1]"/>
								</xsl:element>
								<xsl:text> </xsl:text>
								<xsl:value-of select="."/>
							</xsl:element>
						</xsl:for-each>
					</xsl:element>
				</xsl:if>
			</xsl:if>
		</xsl:copy>
	</xsl:template>

	<!-- image processing -->
	<xsl:template match="xhtml:img">
		<xsl:copy>
			<!-- The image munging in the preprocessor leaves some old image references - scrape them out here -->
			<xsl:attribute name="src"><xsl:value-of select="@src"/></xsl:attribute>
			<xsl:attribute name="alt"><xsl:value-of select="@src"/></xsl:attribute>
			<xsl:attribute name="longdesc"><xsl:value-of select="@src"/></xsl:attribute>
			<xsl:apply-templates/>
		</xsl:copy>
	</xsl:template>

	<!-- don't copy meta -->
	<xsl:template match="xhtml:meta" />
	
	<!-- Process the chapters. -->
	<xsl:template match="xhtml:span[@class='Chapter_Number']" mode="process">
	        	<!-- Process all text between here and the matching chapter end. -->
		<xsl:element name="div">
	            		<xsl:variable name="startGenerateID" select="generate-id()"/>
			<xsl:variable name="verses" select="key('verses-by-chapter',$startGenerateID)"/>
			<xsl:attribute name="class"><xsl:text>chapter</xsl:text></xsl:attribute>
			<xsl:attribute name="title"><xsl:value-of select="."/></xsl:attribute>
	            		<xsl:apply-templates select="$verses" mode="processVerses"/>
	        	</xsl:element>
	</xsl:template>
    
	<!-- span processing -->  
	<!-- .epub doesn't recognize the lang attribute for spans - remove them here. -->
	<xsl:template match="xhtml:span">
		<xsl:if test="count(child::*) > 0 or string-length(normalize-space(./text())) > 0">
			<xsl:copy>
				<xsl:if test="@lang != ''">
					<xsl:attribute name="lang"><xsl:value-of select="@lang"/></xsl:attribute>
				</xsl:if>
				<xsl:for-each select="@*[not(local-name() = 'lang' )]">
					<xsl:copy/>
				</xsl:for-each>
				<xsl:if test="@class='sense-variantrefs'">
					<xsl:text> (</xsl:text>
				</xsl:if>
				<xsl:if test="@class='semantic-domains'">
					<xsl:text> (</xsl:text>
				</xsl:if>
				<xsl:if test="@class='semantic-domain-name'">
					<xsl:text> - </xsl:text>
				</xsl:if>
				<!-- if this is a Chapter_Number span, add an ID with Book and Chapter -->
				<xsl:if test="@class = 'Chapter_Number'">
				<!--	<xsl:attribute name="id"><xsl:value-of select="../../../../xhtml:span[1]"/><xsl:value-of select="."/></xsl:attribute> -->
					<xsl:attribute name="id"><xsl:text>CH_</xsl:text><xsl:value-of select="."/></xsl:attribute>
				</xsl:if>
				<xsl:if test="@class='Note_General_Paragraph'">
					<xsl:element name="a">
						<xsl:attribute name="href"><xsl:text>#FN_</xsl:text><xsl:value-of select="@id"/></xsl:attribute>
						<xsl:text>[</xsl:text><xsl:value-of select="@title"/><xsl:text>]</xsl:text>
					</xsl:element>
				</xsl:if>
				<xsl:if test="@class='Note_CrossHYPHENReference_Paragraph'">
					<xsl:element name="a">
						<xsl:attribute name="href"><xsl:text>#FN_</xsl:text><xsl:value-of select="@id"/></xsl:attribute>
						<xsl:text>[*]</xsl:text>
					</xsl:element>
				</xsl:if>
				<!-- if this is a Verse_Number span, add an ID with Book, Chapter and Verse -->
				<!-- Note that the chapter is found by selecting "previous...[1]" - that selects the first item, counting backwards
				       (i.e. the previous node). Ugh. -->
				<xsl:if test="@class = 'Verse_Number'">
					<xsl:attribute name="id"><xsl:value-of select="../../../../xhtml:span[1]"/><xsl:text>_</xsl:text><xsl:value-of select="preceding::xhtml:span[@class='Chapter_Number'][1]"/><xsl:text>_</xsl:text><xsl:value-of select="."/></xsl:attribute>
				</xsl:if>
				<xsl:apply-templates/>
				<xsl:if test="@class='sense-variantrefs'">
					<xsl:text>) </xsl:text>
				</xsl:if>
				<xsl:if test="@class='semantic-domains'">
					<xsl:text>) </xsl:text>
				</xsl:if>
			</xsl:copy>
		</xsl:if>
	</xsl:template>
    
	<!-- Special handling of text. -->
	<xsl:template match="text()">
		<!-- Replace curly quotes with straight quotes. -->
		<xsl:value-of select="translate(.,'“”','&quot;&quot;')"/>
	</xsl:template>

	<!-- Default element and attribute templates. -->
	<xsl:template match="*">
		<xsl:comment>Warning :: The element "<xsl:value-of select="name()"/>", child of "<xsl:value-of select="name(..)"/>", has no matching template.</xsl:comment>
	</xsl:template>
	<!-- default attribute template -->
	<xsl:template match="@*">
		<xsl:comment>Warning :: The attribute "<xsl:value-of select="name()"/>" for element "<xsl:value-of select="name(..)"/>" has no matching template.</xsl:comment>
	</xsl:template>

</xsl:transform>