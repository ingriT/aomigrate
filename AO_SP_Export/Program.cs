using System;

namespace AO_SP_Export
{
    class Program
    {
        static void Main(string[] args)
        {
            var createPdf = true;

            if (createPdf)
            {
                PdfExport.Run();
            }
            else
            {
                var fromStart = DateTime.Now.AddYears(-20);
                var lastYear = DateTime.Now.AddMonths(-14);

                DBImporter.Run(Ezine.AllenOveryVakpublicaties, "R_L", fromStart);
                DBImporter.Run(Ezine.AmsterdamOfficeNews, "BD_M", lastYear, true);
                DBImporter.Run(Ezine.AmsterdamWallArt, "Amsterdam_Wall_Art", lastYear);
                DBImporter.Run(Ezine.AmsterdamseNieuwsoverzicht, "BD_M", lastYear, true);
                DBImporter.Run(Ezine.ApollohouseVandaag, "BD_M", lastYear, true);
                DBImporter.Run(Ezine.Bibliotheek, "R_L", fromStart);
                DBImporter.Run(Ezine.CorporateKnowHowAlert, "NL_Corporate", fromStart);
                DBImporter.Run(Ezine.EmploymentOnline, "NL_Employment", fromStart);
                DBImporter.Run(Ezine.HRBerichten, "HR", fromStart);
                DBImporter.Run(Ezine.LearningAndDevelopmentOnline, "HR_L_D", fromStart);
                DBImporter.Run(Ezine.LitigationOnline, "NL_Litigation", fromStart);
                DBImporter.Run(Ezine.MediaAndExposure, "BD_M", lastYear, true);
                DBImporter.Run(Ezine.MTMededelingen, "MT", fromStart);
                DBImporter.Run(Ezine.Ondernemingsraad, "Ondernemingsraad", fromStart);
                DBImporter.Run(Ezine.SponsoringEnCSR, "BD_M", lastYear, true);
                DBImporter.Run(Ezine.TaxAlert, "NL_Tax", fromStart);
            }
        }

        public enum Ezine
        {
            AllenOveryVakpublicaties = 296,
            AmsterdamOfficeNews = 206,
            AmsterdamWallArt = 48794,
            AmsterdamseNieuwsoverzicht = 289,
            ApollohouseVandaag = 208,
            Bibliotheek = 205,
            CorporateKnowHowAlert = 209,
            EmploymentOnline = 239,
            LitigationOnline = 169,
            HRBerichten = 198,
            LearningAndDevelopmentOnline = 259,
            MediaAndExposure = 268,
            MTMededelingen = 207,
            Ondernemingsraad = 164,
            SponsoringEnCSR = 237,
            TaxAlert = 201,
        }
    }
}
