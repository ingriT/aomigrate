using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace AO_SP_Export
{
    internal class Exporter
    {
        private const string ConnectionStringOld = "Server=.;Integrated Security=true;Database=Nieuwsoverzicht";

        internal static string GetEzineTitle(int ezineId)
        {
            using (var connection = new SqlConnection(ConnectionStringOld))
            {
                string sql = $@"SELECT i.Title FROM vwItems i WHERE i.ItemId = @ezineId";

                return connection.ExecuteScalar<string>(sql, new { ezineId = ezineId });
            }
        }

        internal static List<EzineItem> GetItems(int ezineId, int numOfItems)
        {
            var output = new List<EzineItem>();

            using (var connection = new SqlConnection(ConnectionStringOld))
            {
                string sql = $@"
SELECT TOP {numOfItems} 
    i.ItemId, 
    i.Title, 
    i.[Description], 
    itd.[Data], 
    i.CreatedDate, 
    i.ModifiedDate, 
    u.Email AS Author, 
    images.[Data] AS ImageData, 
    images.[FileName] AS ImageFileName
FROM vwItems i
	INNER JOIN [types] t on i.typeid = t.TypeID
	INNER JOIN vwItemRelations ir ON ir.IDTo = i.ItemID
	INNER JOIN vwItems iParent ON iParent.ItemId = ir.IDFrom
	INNER JOIN vwUsers u ON u.UserId = i.CreatedUserID
	LEFT JOIN vwImages images ON i.ImageGUID = images.ImageGUID
	LEFT JOIN vwItemTextData itd ON itd.ItemId = i.ItemID
WHERE t.Code = 'EZINE_ITEM' AND iParent.ItemId = @ezineId
ORDER BY CreatedDate DESC";

                var items = connection.Query(sql, new { ezineId = ezineId }).ToList();

                foreach (var item in items)
                {
                    var attachments = GetAttachments(connection, item.ItemId);
                    var tagValue = GetTagValues(connection, item.ItemId);

                    var content = item.Description;
                    if (!string.IsNullOrEmpty(item.Data))
                    {
                        content = content + item.Data;
                    }

                    var ezineItem = new EzineItem(item.ItemId, item.Title, content, item.Description, item.Data, item.Author, item.ImageData, item.ImageFileName, tagValue,
                        item.CreatedDate, item.ModifiedDate, attachments);

                    output.Add(ezineItem);
                }
            }

            return output;
        }

        private static string GetTagValues(SqlConnection connection, int itemId)
        {
            var tagValue = string.Empty;

            string sql = $@"
SELECT tag.[Value] AS TagValue
FROM vwItemTags it
INNER JOIN vwTags tag ON tag.TagId = it.TagId
WHERE it.ItemId = @itemId
ORDER BY it.CreatedDate DESC";

            var tags = connection.Query(sql, new { itemId }).ToList();

            foreach (var tag in tags)
            {
                if (!string.IsNullOrEmpty(tagValue))
                {
                    tagValue += ", ";
                }

                tagValue += tag.TagValue;
            }

            return tagValue;
        }

        private static List<ItemAttachment> GetAttachments(SqlConnection connection, int itemId)
        {
            var attachments = new List<ItemAttachment>();

            string sql = $@"
SELECT i.ItemId, i.Title, ibd.[FileName], ibd.[Data], ft.MimeType, ft.Extension
FROM vwItemRelations ir 
	INNER JOIN vwItems i on i.ItemId = ir.IDTo
	INNER JOIN vwItemBinaryData ibd on ibd.ItemID = i.ItemID
	INNER JOIN FileTypes ft on ft.FileTypeID = ibd.FileTypeID
	INNER JOIN [Types] t ON t.TypeId = ir.TypeID
WHERE t.Code = 'REL_DOCUMENT' AND
	ir.IdFrom = @itemId";

            var itemAttachments = connection.Query(sql, new { itemId }).ToList();

            foreach (var itemAttachment in itemAttachments)
            {
                var attachment = new ItemAttachment(itemAttachment.ItemId, itemAttachment.Title, itemAttachment.Data, itemAttachment.FileName, itemAttachment.MimeType);

                attachments.Add(attachment);
            }

            return attachments;
        }
    }
}