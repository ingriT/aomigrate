namespace AO_SP_Export
{
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Text;

    using Dapper;

    public static class PdfExport
    {
        private const string ConnectionStringOld = "Server=.;Integrated Security=true;Database=Nieuwsoverzicht";

        public static void Run()
        {
            var items = GetAllItems();
            var htmlContent = new StringBuilder();

            var contentFolder = items.First().Parent;

            foreach (var item in items)
            {
                if (item.Parent != contentFolder)
                {
                    CreatePdf(htmlContent, contentFolder);
                    htmlContent.Clear();
                    contentFolder = item.Parent;
                }

                htmlContent.Append($"<h1>{item.Title}</h1>");
                htmlContent.Append($"<p><strong>{item.BreadCrumb}</strong></p>");
                htmlContent.Append($"<p><em>Aangemaakt: {item.CreatedOn.ToString("dd-MM-yyyy HH:mm")}");
                if (!string.IsNullOrEmpty(item.AuthorEmailCreated))
                {
                    htmlContent.Append($" ({item.AuthorEmailCreated})");
                }

                if (item.ModifiedOn.HasValue)
                {
                    htmlContent.Append($"<br/>Gewijzigd: {item.ModifiedOn.Value.ToString("dd-MM-yyyy HH:mm")}");
                    if (!string.IsNullOrEmpty(item.AuthorEmailModified))
                    {
                        htmlContent.Append($" ({item.AuthorEmailModified})");
                    }
                }

                htmlContent.Append("</em></p>");

                htmlContent.Append(item.Description);
                htmlContent.Append(item.Content);
                htmlContent.Append("<div style=\"page-break-after:always\"></div>");
            }

            CreatePdf(htmlContent, contentFolder);
        }

        internal static void CreatePdf(StringBuilder content, string parentTitle)
        {
            (new NReco.PdfGenerator.HtmlToPdfConverter()).GeneratePdf(content.ToString(),
                $"<h1>Allen & Overy Nieuwsoverzicht alle content</h1><p>{parentTitle}<p/>",
                $"c:\\temp\\ezines\\Content {parentTitle}.pdf");
        }

        internal static List<StaticItem> GetAllItems()
        {
            var output = new List<StaticItem>();

            using (var connection = new SqlConnection(ConnectionStringOld))
            {
                string sql = $@"
SELECT
    i.ItemId, 
    i.Title, 
    i.[Description], 
    itd.Data, 
    i.CreatedDate, 
    i.ModifiedDate, 
    u.Email As AuthorEmailCreated, 
    u2.Email As AuthorEmailModified,
    iParent.Title AS Parent1, 
    iParent2.Title AS Parent2, 
    iParent3.Title AS Parent3, 
    iParent4.Title AS Parent4

FROM vwItems i
	INNER JOIN [types] t ON t.TypeID = i.TypeID
	INNER JOIN vwItemRelations ir ON ir.idto = i.ItemID
	INNER JOIN vwItems iParent ON iParent.itemid = ir.IDFrom
	INNER JOIN [types] tParent ON tparent.TypeID = iParent.TypeID

	LEFT JOIN vwItemTextData itd ON itd.itemid = i.ItemID

	LEFT JOIN users u ON u.userid = i.CreatedUserId
	LEFT JOIN users u2 ON u2.userid = i.ModifiedUserID

	LEFT JOIN vwItemRelations ir2 ON ir2.idto = iparent.ItemID
	LEFT JOIN vwItems iParent2 ON iParent2.itemid = ir2.IDFrom
	LEFT JOIN [types] tParent2 ON tparent2.TypeID = iParent2.TypeID and tparent2.Code = 'CONTAINER'

	LEFT JOIN vwItemRelations ir3 ON ir3.idto = iparent2.ItemID
	LEFT JOIN vwItems iParent3 ON iParent3.itemid = ir3.IDFrom
	LEFT JOIN [types] tParent3 ON tparent3.TypeID = iParent3.TypeID and tparent3.Code = 'CONTAINER'

	LEFT JOIN vwItemRelations ir4 ON ir4.idto = iparent3.ItemID
	LEFT JOIN vwItems iParent4 ON iParent4.itemid = ir4.IDFrom
	LEFT JOIN [types] tParent4 ON tparent4.TypeID = iParent4.TypeID and tparent4.Code = 'CONTAINER'

WHERE t.Code = 'item' AND tParent.Code = 'CONTAINER'
ORDER BY COALESCE(iParent4.ItemId, COALESCE(iParent3.ItemId, COALESCE(iParent2.ItemId, iParent.ItemId)))";

                var items = connection.Query(sql);

                foreach (var item in items)
                {
                    var breadCrumb = string.Empty;
                    var parent = string.Empty;
                    if (!string.IsNullOrEmpty(item.Parent4))
                    {
                        parent = item.Parent4;
                        breadCrumb = item.Parent4;
                    }
                    if (!string.IsNullOrEmpty(item.Parent3))
                    {
                        if (!string.IsNullOrEmpty(breadCrumb))
                        {
                            breadCrumb += " / ";
                        }
                        else
                        {
                            parent = item.Parent3;
                        }
                        breadCrumb += item.Parent3;
                    }
                    if (!string.IsNullOrEmpty(item.Parent2))
                    {
                        if (!string.IsNullOrEmpty(breadCrumb))
                        {
                            breadCrumb += " / ";
                        }
                        else
                        {
                            parent = item.Parent2;
                        }
                        breadCrumb += item.Parent2;
                    }
                    if (!string.IsNullOrEmpty(item.Parent1))
                    {
                        if (!string.IsNullOrEmpty(breadCrumb))
                        {
                            breadCrumb += " / ";
                        }
                        else
                        {
                            parent = item.Parent1;
                        }
                        breadCrumb += item.Parent1;
                    }

                    var staticItem = new StaticItem(item.ItemId, parent, item.Title, breadCrumb, item.Description?.Trim(), item.Data?.Trim(), item.AuthorEmailCreated,
                        item.AuthorEmailModified, item.CreatedDate, item.ModifiedDate);

                    output.Add(staticItem);
                }
            }

            return output;
        }
    }
}
