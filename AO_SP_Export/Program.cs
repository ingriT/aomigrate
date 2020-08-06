using System;

namespace AO_SP_Export
{
    class Program
    {
        static void Main(string[] args)
        {
            var ezineId = 209; // Corporate Know-How
            var fileName = @"C:\Users\ingri\OneDrive - iLLUMiON\A&O MIGRATIE\" + DateTime.Now.ToString("yyyymmdd_HHMMss") + "_Manifest.xml";

            ExportXml.Run(ezineId, fileName);
        }
    }
}
