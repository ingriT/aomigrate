using System;

namespace AO_SP_Export
{
    class Program
    {
        static void Main(string[] args)
        {
            const int increment = 7;

            var ezineId = 209; // Corporate Know-How
            var directory = $@"C:\Users\ingri\OneDrive - iLLUMiON\A&O MIGRATIE\{increment} {DateTime.Now.ToString("yyyyMMdd_HHmmss")}\\";

            if (!System.IO.Directory.Exists(directory))
            {
                System.IO.Directory.CreateDirectory(directory);
            }

            ExportXml.Run(ezineId, directory, increment);
        }
    }
}
