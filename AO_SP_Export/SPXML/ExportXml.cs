using System;

namespace AO_SP_Export
{
    internal class ExportXml
    {
        internal static void Run(int ezineId, string directory, int numOfItems, int increment)
        {
            // Get some items from the database
            var ezineItemsForExport = Exporter.GetItems(ezineId, numOfItems);

            // Convert them to manifest.xml
            var manifest = XmlConverter.GetManifestXml(ezineItemsForExport);
            manifest.Save(directory + "Manifest.xml");

            // Create a SystemData.xml to with that manifest.xml
            var systemData = XmlConverter.GetSystemData(ezineItemsForExport.Count);
            systemData.Save(directory + "SystemData.xml");

            // Create ExportSettings.xml
            var exportSettings = XmlConverter.GetExportSettings(increment);
            exportSettings.Save(directory + "ExportSettings.xml");

            // Create Requirements.xml
            var requirements = XmlConverter.GetRequirements();
            requirements.Save(directory + "Requirements.xml");
        }
    }
}