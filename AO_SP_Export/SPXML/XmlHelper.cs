using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace AO_SP_Export
{
    public static class XmlHelper
    {
        public static XmlDocument CreateDefaultDocument()
        {
            var document = new XmlDocument();

            XmlDeclaration xmlDeclaration = document.CreateXmlDeclaration("1.0", "UTF-8", null);
            document.AppendChild(xmlDeclaration);

            return document;
        }
    }
}
