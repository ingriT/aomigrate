using System;

namespace AO_SP_Export
{
    class Program
    {
        static void Main(string[] args)
        {
            var xmlExport = false;

            var ezineId = 209; // Corporate Know-How
            const int numOfItems = Int32.MaxValue;
            var fromDate = DateTime.Now.AddYears(-20);

            if (xmlExport)
            {
                const int increment = 8;

                var directory = $@"C:\Users\ingri\OneDrive - iLLUMiON\A&O MIGRATIE\{increment} {DateTime.Now.ToString("yyyyMMdd_HHmmss")}\\";

                if (!System.IO.Directory.Exists(directory))
                {
                    System.IO.Directory.CreateDirectory(directory);
                }
                
                ExportXml.Run(ezineId, directory, fromDate, increment, numOfItems);
            }
            else
            {
                DBImporter.Run(ezineId, fromDate, numOfItems);

                ezineId = 239; // Employment
                DBImporter.Run(ezineId, fromDate, numOfItems);

                ezineId = 169; // Litigation
                DBImporter.Run(ezineId, fromDate, numOfItems);

                ezineId = 201; // Tax alert
                DBImporter.Run(ezineId, fromDate, numOfItems);

//                ezineId = 272; // Global Tax
//                DBImporter.Run(ezineId, numOfItems);

                ezineId = 205; // Bibliotheek
                DBImporter.Run(ezineId, fromDate, numOfItems);

                ezineId = 164; // Ondernemingsraad
                DBImporter.Run(ezineId, fromDate, numOfItems);

            }
        }
    }
}
