<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
    <xsl:output method="html" encoding="UTf-8"/>

<xsl:template match="/root/footer">
    <footer>
        <xsl:apply-templates select="footerContent"/>
        <xsl:apply-templates select="footerBottom"/>
    </footer>
</xsl:template>

<xsl:template match="footerContent">
    <div class="footer-content">
        <xsl:apply-templates select="footerSection"/>
    </div>
</xsl:template>

<xsl:template match="footerSection">
    <div class="footer-section">
        <xsl:attribute name="class">
            <xsl:value-of select="@class"/>
        </xsl:attribute>
        <xsl:apply-templates select="title"/>
        <xsl:apply-templates select="text"/>
        <xsl:apply-templates select="links"/>
    </div>
</xsl:template>

<xsl:template match="title">
    <h2><xsl:value-of select="."/></h2>
</xsl:template>

<xsl:template match="text">
    <p><xsl:value-of select="."/></p>
</xsl:template>

<xsl:template match="links">
    <nav>
        <ul>
            <xsl:apply-templates select="link"/>
        </ul>
    </nav>
</xsl:template>

<xsl:template match="link">
    <li><a>
        <xsl:attribute name="href">
            <xsl:value-of select="@href"/>
        </xsl:attribute>
        <xsl:value-of select="."/>
    </a></li>
</xsl:template>

<xsl:template match="footerBottom">
    <div class="footer-bottom">
        <xsl:apply-templates select="copyright"/>
        <xsl:apply-templates select="author"/>
    </div>
</xsl:template>

<xsl:template match="copyright">
    <p><xsl:value-of select="."/></p>
</xsl:template>

<xsl:template match="author">
    <p><xsl:value-of select="."/></p>
</xsl:template>

</xsl:stylesheet>
