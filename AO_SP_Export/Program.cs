using System;

namespace AO_SP_Export
{
    class Program
    {
        static void Main(string[] args)
        {
            var xmlExport = false;

            const int numOfItems = Int32.MaxValue;
            var fromDate = DateTime.Now.AddMonths(-20);

            if (xmlExport)
            {
                const int increment = 8;

                var directory = $@"C:\Users\ingri\OneDrive - iLLUMiON\A&O MIGRATIE\{increment} {DateTime.Now.ToString("yyyyMMdd_HHmmss")}\\";

                if (!System.IO.Directory.Exists(directory))
                {
                    System.IO.Directory.CreateDirectory(directory);
                }
                
                ExportXml.Run(Ezine.CorporateKnowHowAlert, directory, fromDate, increment, numOfItems);
            }
            else
            {
                DBImporter.Run(Ezine.CorporateKnowHowAlert, fromDate, numOfItems);

                DBImporter.Run(Ezine.Employment, fromDate, numOfItems);

                DBImporter.Run(Ezine.Litigation, fromDate, numOfItems);

                DBImporter.Run(Ezine.TaxAlert, fromDate, numOfItems);

                DBImporter.Run(Ezine.Bibliotheek, fromDate, numOfItems);

                DBImporter.Run(Ezine.Ondernemingsraad, fromDate, numOfItems);
            }
        }

        public enum Ezine
        {
            CorporateKnowHowAlert = 209,
            Employment = 239,
            Litigation = 169,
            TaxAlert = 201,
            Bibliotheek = 205,
            Ondernemingsraad = 164
        }
    }
}
