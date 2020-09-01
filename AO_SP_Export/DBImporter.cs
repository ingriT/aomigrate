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
CREATE TABLE {tableName} (
    UniqueId UNIQUEIDENTIFIER,
    ItemId INT,
    Title VARCHAR(MAX),
    Content VARCHAR(MAX),
    [Description] VARCHAR(MAX),
    [Data] VARCHAR(MAX),
    AuthorEmail VARCHAR(MAX),
    ImageData VARBINARY(MAX),
    ImageFileName VARCHAR(MAX),
    TagValue VARCHAR(MAX),
    CreatedOn DATETIME,
    ModifiedOn DATETIME
)";

                connection.Execute(sql);

                foreach (var item in ezineItems)
                {
                    sql = $@"
INSERT INTO {tableName} (UniqueId, ItemId, Title, Content, [Description], [Data], AuthorEmail, ImageData, ImageFileName, TagValue, CreatedOn, ModifiedOn)
VALUES (@itemGuid, @itemId, @title, @content, @description, @data, @authorEmail, @imageData, @imageFileName, @tagValue, @createdOn, @modifiedOn)";

                    connection.Execute(sql, new
                    {
                        itemGuid = item.GuidId,
                        itemId = item.Id,
                        title = item.Title,
                        content = item.Content,
                        description = item.Description,
                        data = item.Data,
                        authorEmail = item.AuthorEmail,
                        imageData = item.ImageData,
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