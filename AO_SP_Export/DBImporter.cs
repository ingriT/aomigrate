using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;

namespace AO_SP_Export
{
    internal class DBImporter
    {
        private const string ConnectionStringNew = "Server=.;Integrated Security=true;Database=NieuwsoverzichtLite";

        internal static void Run(int ezineId, int numOfItems)
        {
            // Get some items from the database
            var ezineItemsForExport = Exporter.GetItems(ezineId, numOfItems);

            var ezineTitle = Exporter.GetEzineTitle(ezineId);
            var tableName = ezineTitle.Replace("_", "").Replace("-", "").Replace(" ", "").Replace(",", "").Replace(".", "").ToLower();

            SaveItems(tableName, ezineItemsForExport);
        }

        internal static List<EzineItem> SaveItems(string tableName, List<EzineItem> ezineItems)
        {
            var output = new List<EzineItem>();

            using (var connection = new SqlConnection(ConnectionStringNew))
            {
                var sql = $@"DROP TABLE {tableName})";
                try
                {
                    connection.Execute(sql);
                }
                catch (Exception ex)
                {
                    // do nothing
                }

                sql = $@"
IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dbo' 
                 AND  TABLE_NAME = '{tableName}'))
BEGIN
    DROP TABLE {tableName}
END

CREATE TABLE {tableName} (
    Id INT IDENTITY (1,1),
    Title VARCHAR(MAX),
    Content VARCHAR(MAX),
    AuthorEmail VARCHAR(MAX),
    [Image] VARCHAR(MAX),
    TagValue VARCHAR(MAX),
    CreatedOn DATETIME,
    ModifiedOn DATETIME
)";

                connection.Execute(sql);

                foreach (var item in ezineItems)
                {
                    var imageFileName = "";

                    if (!string.IsNullOrEmpty(item.ImageFileName))
                    {
                        imageFileName = $"<img src=\"http://global.intranet.allenovery.com/locations/europe/netherlands/publishingimages/{item.ImageFileNameUrl}\" />";
                    }

                    sql = $@"
INSERT INTO {tableName} (Title, Content, AuthorEmail, [Image], TagValue, CreatedOn, ModifiedOn)
VALUES (@title, @content, @authorEmail, @imageFileName, @tagValue, @createdOn, @modifiedOn)";

                    connection.Execute(sql, new
                    {
                        title = item.Title,
                        content = item.Content,
                        authorEmail = item.AuthorEmail,
                        imageFileName = imageFileName,
                        tagValue = item.TagValue,
                        createdOn = item.CreatedOn,
                        modifiedOn = item.ModifiedOn
                    });
                }

                var filePath = @"C:\temp\ezines\archief\";
                var imagePath = @"C:\temp\ezines\images\";

                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }
                if (!Directory.Exists(imagePath))
                {
                    Directory.CreateDirectory(imagePath);
                }

                foreach (var item in ezineItems)
                {
                    if (!string.IsNullOrEmpty(item.ImageFileNameUrl))
                    {
                        if (File.Exists(imagePath + item.ImageFileNameUrl))
                        {
                            throw new Exception($"{item.ImageFileNameUrl} file exists!");
                        }

                        using (var fs = new FileStream(imagePath + item.ImageFileNameUrl, FileMode.Create, FileAccess.Write))
                        {
                            fs.Write(item.ImageData, 0, item.ImageData.Length);
                        }
                    }

                    foreach (var attachment in item.Attachments)
                    {
                        if (File.Exists(filePath + attachment.FileNameUrl))
                        {
                            throw new Exception($"{attachment.FileNameUrl} file exists!");
                        }

                        using (var fs = new FileStream(filePath + attachment.FileNameUrl, FileMode.Create, FileAccess.Write))
                        {
                            fs.Write(attachment.FileData, 0, attachment.FileData.Length);
                        }
                    }
                }
            }

            return output;
        }
    }
}