using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace AO_SP_Export
{
    internal class Exporter
    {
        internal static List<EzineItem> GetItems(int ezineId)
        {
            var output = new List<EzineItem>();

            using (var connection = new SqlConnection("Server=.;Integrated Security=true;Database=Nieuwsoverzicht"))
            {
                string sql = $@"SELECT TOP 10 i.ItemId, i.Title, i.[Description], itd.[Data], i.CreatedDate, i.ModifiedDate, u.Email, images.[Data], images.[FileName]
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

                foreach(var item in items)
                {
                    var content = item.Description;
                    if (!string.IsNullOrEmpty(item.Data))
                    {
                        content = content + item.Data;
                    }

                    var ezineItem = new EzineItem(item.ItemId, item.Title, content, item.CreatedDate, item.ModifiedDate);

                    output.Add(ezineItem);
                }
            }

            return output;
        }
    }
}