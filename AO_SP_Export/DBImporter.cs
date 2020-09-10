using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

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
    ContentInnerText VARCHAR(MAX),
    AuthorEmail VARCHAR(MAX),
    ImageFileName VARCHAR(MAX),
    TagValue VARCHAR(MAX),
    CreatedOn DATETIME,
    ModifiedOn DATETIME
)";

                connection.Execute(sql);

                foreach (var item in ezineItems)
                {
                    sql = $@"
INSERT INTO {tableName} (Title, Content, ContentInnerText, AuthorEmail, ImageFileName, TagValue, CreatedOn, ModifiedOn)
VALUES (@title, @content, @contentInnerText, @authorEmail, @imageFileName, @tagValue, @createdOn, @modifiedOn)";

                    connection.Execute(sql, new
                    {
                        title = item.Title,
                        content = item.Content,
                        contentInnerText = item.ContentInnerText,
                        authorEmail = item.AuthorEmail,
                        imageFileName = item.ImageFileName,
                        tagValue = item.TagValue,
                        createdOn = item.CreatedOn,
                        modifiedOn = item.ModifiedOn
                    });
                }
            }

            return output;
        }
    }
}