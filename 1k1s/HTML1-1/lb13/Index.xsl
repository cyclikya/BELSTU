<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
<xsl:template match="/">
<html>
    <style>
        table {
            width: 100%;
            border-collapse: collapse;
        }
        th, td {
            border: 1px solid black;
            padding: 8px;
            text-align: left;
        }
        th {
            background-color: red;
            color: white;
        }
    </style>
    <body>
        <table>
            <tr>
                <th><xsl:value-of select="fruits/head/glName"/></th>
                <th><xsl:value-of select="fruits/head/glColor"/></th>
                <th><xsl:value-of select="fruits/head/glTaste"/></th>
            </tr>
            <xsl:for-each select="fruits/fruit">
            <xsl:sort select="name"/>
                <tr>
                    <td><xsl:value-of select="name"/></td>
                    <td><xsl:value-of select="color"/></td>
                    <td><xsl:value-of select="taste"/></td>
                </tr>
            </xsl:for-each>
        </table>
    </body>
</html>
</xsl:template>
</xsl:stylesheet>