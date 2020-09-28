using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using static AO_SP_Export.Program;

namespace AO_SP_Export
{
    internal class Exporter
    {
        private const string ConnectionStringOld = "Server=.;Integrated Security=true;Database=Nieuwsoverzicht";

        internal static string GetEzineTitle(Ezine ezine)
        {
            using (var connection = new SqlConnection(ConnectionStringOld))
            {
                string sql = $@"SELECT i.Title FROM vwItems i WHERE i.ItemId = @ezineId";

                return connection.ExecuteScalar<string>(sql, new { ezineId = (int)ezine });
            }
        }

        internal static List<EzineItem> GetItems(Ezine ezine, DateTime fromDate, int numOfItems = 0)
        {
            var output = new List<EzineItem>();
            var topSql = numOfItems > 0 ? $" TOP {numOfItems} " : "";

            using (var connection = new SqlConnection(ConnectionStringOld))
            {
                string sql = $@"
SELECT {topSql}
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
WHERE t.Code = 'EZINE_ITEM' AND iParent.ItemId = @ezineId AND i.CreatedDate >= @fromDate
ORDER BY i.CreatedDate DESC";

                var items = connection.Query(sql, new { ezineId = (int)ezine, fromDate = fromDate }).ToList();

                foreach (var item in items)
                {
                    List<string> tagValues = GetTagValues(connection, item.ItemId);

                    var newTags = ReplaceTagsAndCheckIfImport(ezine, tagValues);

                    if (!string.IsNullOrEmpty(newTags))
                    {
                        var attachments = GetAttachments(connection, item.ItemId);
                        var content = item.Description;
                        if (!string.IsNullOrEmpty(item.Data))
                        {
                            content = content + item.Data;
                        }

                        var ezineItem = new EzineItem(item.ItemId, item.Title, content, item.Description, item.Data, item.Author, item.ImageData, item.ImageFileName, newTags,
                            item.CreatedDate, item.ModifiedDate, attachments);

                        output.Add(ezineItem);
                    }
                }
            }

            return output;
        }

        private static string ReplaceTagsAndCheckIfImport(Ezine ezine, List<string> tagValues)
        {
            var newTags = new List<string>();

            var tagsToRemove = GetTagsToRemove(ezine);
            var tagsToReplace = GetTagsToReplace(ezine);

            foreach (var tagValue in tagValues)
            {
                if (tagsToRemove.Contains(tagValue, StringComparer.InvariantCultureIgnoreCase))
                {
                    continue;
                }

                if (tagsToReplace.Keys.Contains(tagValue))
                {
                    var newTag = tagsToReplace[tagValue];

                    if (!newTags.Contains(newTag, StringComparer.InvariantCulture))
                    {
                        newTags.Add(newTag);
                        continue;
                    }
                }

                newTags.Add(tagValue);
            }

            var output = string.Empty;

            foreach (var tag in newTags)
            {
                if (!string.IsNullOrEmpty(output))
                {
                    output += ", ";
                }

                output += tag;
            }

            return output;
        }

        private static Dictionary<string, string> GetTagsToReplace(Ezine ezine)
        {
            var tagsToReplace = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);

            if (ezine == Ezine.Litigation)
            {
                tagsToReplace.Add("Aanwinsten", "Aanwinsten literatuur");
                tagsToReplace.Add("Class actions", "Class actions");
                tagsToReplace.Add("E-alerts Litigation", "E-alerts Litigation");
                tagsToReplace.Add("Engelse taal", "Engelse taal");
                tagsToReplace.Add("Griffierecht", "Griffierechten en kosten");
                tagsToReplace.Add("Jurisprudentiebundels", "Jurisprudentiebundels");
                tagsToReplace.Add("Know how", "Know how");
                tagsToReplace.Add("Know how berichten", "Know how");
                tagsToReplace.Add("Nieuws algemeen", "PG-nieuws");
                tagsToReplace.Add("Presentaties Arbitration Group Amsterdam (AGA) ", "Presentaties AGA");
                tagsToReplace.Add("Presentaties extern", "Presentatie extern");
                tagsToReplace.Add("Presentaties Know How Bijeenkomsten", "Presentaties intern");
                tagsToReplace.Add("Presentaties Litigation Academy", "Presentaties intern");
                tagsToReplace.Add("Publicaties Litigation", "Publicaties Litigation");
                tagsToReplace.Add("Seminar Uitspraak Gemist", "Seminar Uitspraak Gemist");
                tagsToReplace.Add("Signalering", "Signaleringen Ontwikkelingen");
                tagsToReplace.Add("Wet- en regelgeving update", "Wet- en regelgeving update");
                tagsToReplace.Add("Wetsvoorstellen", "Wet- en regelgeving update");
                tagsToReplace.Add("Update Wet- en regelgeving update", "Wet- en regelgeving update");
            }
            else if (ezine == Ezine.CorporateKnowHowAlert)
            {
                tagsToReplace.Add("Eumedion", "Corporate Governance");
                tagsToReplace.Add("FD", "Corporate Signalering");
                tagsToReplace.Add("Nieuwsberichten", "Corporate Signalering");
                tagsToReplace.Add("e-Alert", "e-Alert / eReview");
                tagsToReplace.Add("e-Review", "e-Alert / eReview");
                tagsToReplace.Add("Financiele instellingen", "Financial Regulatory");
                tagsToReplace.Add("Financiele markten", "Financial Regulatory");
                tagsToReplace.Add("Wft wetgeving", "Financial Regulatory");
                tagsToReplace.Add("Hoge raad", "Jurisprudentie");
                tagsToReplace.Add("Actuele uitspraken", "Jurisprudentie");
                tagsToReplace.Add("Know How Nieuws", "Know How Nieuws");
                tagsToReplace.Add("SFP's", "SFP's");
                tagsToReplace.Add("AFM", "Wet- en Regelgeving Update");
                tagsToReplace.Add("AFM berichten", "Wet- en Regelgeving Update");
                tagsToReplace.Add("AIFM Richtlijn", "Wet- en Regelgeving Update");
                tagsToReplace.Add("Beloning", "Wet- en Regelgeving Update");
                tagsToReplace.Add("Europees nieuws", "Wet- en Regelgeving Update");
                tagsToReplace.Add("ESMA", "Wet- en Regelgeving Update");
                tagsToReplace.Add("Legislation", "Wet- en Regelgeving Update");
                tagsToReplace.Add("Wetsvoorstellen", "Wet- en Regelgeving Update");
            }

            return tagsToReplace;
        }

        private static List<string> GetTagsToRemove(Ezine ezine)
        {
            var tagsToRemove = new List<string>();

            if (ezine == Ezine.Litigation)
            {
                tagsToRemove.AddRange(new List<string> { "A&O News", "Financial publications", "Mededeling", "Nieuw in KH systeem en Lit. Online",
                    "Nieuw op de Know-How site", "Nieuwe producten", "Nieuws algemeen", "Nieuwsberichten" });
            }
            else if (ezine == Ezine.CorporateKnowHowAlert)
            {
                tagsToRemove.AddRange(new List<string> { "CESR" });
            }

            return tagsToRemove;
        }

        private static List<string> GetTagValues(SqlConnection connection, int itemId)
        {
            List<string> tagValues = new List<string>();

            string sql = $@"
SELECT tag.[Value] AS TagValue
FROM vwItemTags it
INNER JOIN vwTags tag ON tag.TagId = it.TagId
WHERE it.ItemId = @itemId
ORDER BY it.CreatedDate DESC";

            var tags = connection.Query(sql, new { itemId }).ToList();

            foreach (var tag in tags)
            {
                tagValues.Add(tag.TagValue);
            }

            return tagValues;
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