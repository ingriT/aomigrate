using Dapper;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using static AO_SP_Export.Program;

namespace AO_SP_Export
{
    internal class DBImporter
    {
        private const string ConnectionStringNew = "Server=.;Integrated Security=true;Database=NieuwsoverzichtLite";

        internal static void Run(Ezine ezine, string tableName, DateTime fromDate, bool addEzineTitleToItemTitle = false, bool createCsvOverview = false)
        {
            var itemsRemoved = new List<EzineItem>();
            var titleSupplement = string.Empty;
            var ezineTitle = Exporter.GetEzineTitle(ezine);

            if (addEzineTitleToItemTitle)
            {
                titleSupplement = $" - {ezineTitle}";
            }

            // Get some items from the database
            var ezineItemsForExport = Exporter.GetItems(ezine, fromDate, titleSupplement, out itemsRemoved);

            if (createCsvOverview)
            {
                var fileName = $"C:\\temp\\ezines\\overzichten\\Overzicht_{tableName}.csv";
                var fileContent = new List<string>();

                if (!File.Exists(fileName))
                {
                    var header = "Ezine;InExport;ItemId;ItemTitle;CreatedOn;ModifiedOn;Tags;Url";
                    fileContent.Add(header);
                }

                foreach (var item in ezineItemsForExport)
                {
                    var fileLine = $"{ezineTitle};1;{item.Id};{item.Title.Replace(";", "")};{item.CreatedOn};{item.ModifiedOn};{item.TagValue.Replace(";", "")};http://amsmartsite.amsterdam.allenovery.com/ItemShow.aspx?itemId={item.Id}";
                    fileContent.Add(fileLine);
                }

                foreach (var item in itemsRemoved)
                {
                    var fileLine = $"{ezineTitle};0;{item.Id};{item.Title.Replace(";", "")};{item.CreatedOn};{item.ModifiedOn};;http://amsmartsite.amsterdam.allenovery.com/ItemShow.aspx?itemId={item.Id}";
                    fileContent.Add(fileLine);
                }

                if (File.Exists(fileName))
                {
                    File.AppendAllLines(fileName, fileContent);
                }
                else
                {
                    File.WriteAllLines(fileName, fileContent);
                }
            }
            else
            {
                SaveItems(tableName, ezineItemsForExport);
            }
        }

        internal static List<EzineItem> SaveItems(string tableName, List<EzineItem> ezineItems)
        {
            var output = new List<EzineItem>();

            using (var connection = new SqlConnection(ConnectionStringNew))
            {
                var sql = $@"
IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dbo' 
                 AND  TABLE_NAME = '{tableName}'))
BEGIN

CREATE TABLE {tableName} (
    Id INT IDENTITY (1,1),
    Title VARCHAR(MAX),
    Content VARCHAR(MAX),
    AuthorEmail VARCHAR(MAX),
    [Image] VARCHAR(MAX),
    TagValue VARCHAR(MAX),
    CreatedOn DATETIME,
    ModifiedOn DATETIME
)

END
";

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

                        using (var image = Image.Load(item.ImageData))
                        {
                            var orgWidth = image.Width;
                            var orgHeight = image.Height;

                            var newWidth = 124;

                            if (orgWidth != newWidth)
                            {
                                var newHeight = (orgWidth / newWidth) * orgHeight;

                                if (orgWidth > newWidth)
                                {
                                    newHeight = (newWidth / orgWidth) * orgHeight;
                                }

                                image.Mutate(x => x.Resize(newWidth, newHeight));

                                image.Save(imagePath + item.ImageFileNameUrl);
                            }
                            else
                            {
                                using (var fs = new FileStream(imagePath + item.ImageFileNameUrl, FileMode.Create, FileAccess.Write))
                                {
                                    fs.Write(item.ImageData, 0, item.ImageData.Length);
                                }
                            }
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