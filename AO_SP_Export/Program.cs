using System;

namespace AO_SP_Export
{
    class Program
    {
        static void Main(string[] args)
        {
            var xmlExport = false;

            var ezineId = 209; // Corporate Know-How
            const int numOfItems = 50; //Int32.MaxValue;

            if (xmlExport)
            {
                const int increment = 8;

                var directory = $@"C:\Users\ingri\OneDrive - iLLUMiON\A&O MIGRATIE\{increment} {DateTime.Now.ToString("yyyyMMdd_HHmmss")}\\";

                if (!System.IO.Directory.Exists(directory))
                {
                    System.IO.Directory.CreateDirectory(directory);
                }

                ExportXml.Run(ezineId, directory, numOfItems, increment);
            }
            else
            {
                DBImporter.Run(ezineId, numOfItems);

                ezineId = 239; // Employment
                DBImporter.Run(ezineId, numOfItems);

                ezineId = 169; // Litigation
                DBImporter.Run(ezineId, numOfItems);

                ezineId = 201; // Tax alert
                DBImporter.Run(ezineId, numOfItems);

                ezineId = 272; // Global Tax
                DBImporter.Run(ezineId, numOfItems);

                ezineId = 205; // Bibliotheek
                DBImporter.Run(ezineId, numOfItems);

                ezineId = 164; // Ondernemingsraad
                DBImporter.Run(ezineId, numOfItems);

            }
        }
    }
}
