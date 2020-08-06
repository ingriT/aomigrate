using System;

namespace AO_SP_Export
{
    internal class ExportXml
    {
        internal static void Run(int ezineId, string fileName)
        {
            // Get some items from the database
            var ezineItemsForExport = Exporter.GetItems(ezineId);

            // Convert them to Xml
            var xmlDocument = XmlConverter.GetManifestXml(ezineItemsForExport);

            xmlDocument.Save(fileName);
        }
    }
}