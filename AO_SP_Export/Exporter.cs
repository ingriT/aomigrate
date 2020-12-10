using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
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

        internal static List<EzineItem> GetItems(Ezine ezine, DateTime fromDate, string titleSupplement, out List<EzineItem> itemsRemoved)
        {
            var output = new List<EzineItem>();
            itemsRemoved = new List<EzineItem>();

            using (var connection = new SqlConnection(ConnectionStringOld))
            {
                string sql = $@"
SELECT
    i.ItemId, 
    i.Title, 
    i.[Description], 
    itd.[Data], 
    i.CreatedDate, 
    i.ModifiedDate, 
    u.Email AS Author, 
    images.[Data] AS ImageData, 
    images.[FileName] AS ImageFileName,
    iep.DateTimeValue AS PublishDate
FROM vwItems i
	INNER JOIN [types] t on i.typeid = t.TypeID
	INNER JOIN vwItemRelations ir ON ir.IDTo = i.ItemID
	INNER JOIN vwItems iParent ON iParent.ItemId = ir.IDFrom
	LEFT JOIN Users u ON u.UserId = i.CreatedUserID
	LEFT JOIN vwImages images ON i.ImageGUID = images.ImageGUID
	LEFT JOIN vwItemTextData itd ON itd.ItemId = i.ItemID
    LEFT JOIN vwItemExtraProperties iep ON iep.ItemId = i.ItemId AND iep.Code = 'PUBLISHDATE'
WHERE t.Code = 'EZINE_ITEM' AND iParent.ItemId = @ezineId AND i.CreatedDate >= @fromDate
ORDER BY COALESCE(iep.DateTimeValue, i.CreatedDate) DESC";

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

                        var ezineItem = new EzineItem(item.ItemId, item.Title + titleSupplement, content, item.Description, item.Data, item.Author, item.ImageData, item.ImageFileName, newTags,
                            item.CreatedDate, item.ModifiedDate, item.PublishDate, attachments);

                        output.Add(ezineItem);
                    }
                    else
                    {
                        var content = item.Description;
                        if (!string.IsNullOrEmpty(item.Data))
                        {
                            content = content + item.Data;
                        }

                        var ezineItem = new EzineItem(item.ItemId, item.Title + titleSupplement, content, item.Description, item.Data, item.Author, item.ImageData, item.ImageFileName, newTags,
                            item.CreatedDate, item.ModifiedDate, item.PubishDate, new List<ItemAttachment>());

                        itemsRemoved.Add(ezineItem);
                    }
                }
            }

            return output;
        }

        private static string ReplaceTagsAndCheckIfImport(Ezine ezine, List<string> tagValues)
        {
            var newTags = new List<string>();
            var doNotImportItem = false;

            if (ezine == Ezine.Bibliotheek)
            {
                newTags.Add("BibliotheekArchief");
            }
            else if (ezine == Ezine.AllenOveryVakpublicaties)
            {
                newTags.Add("Vakpublicatie");
            }
            else
            {
                var tagsToRemove = GetTagsToRemove(ezine);
                var tagsToReplace = GetTagsToReplace(ezine);

                foreach (var tagValue in tagValues)
                {
                    if (tagsToRemove.Contains(tagValue, StringComparer.InvariantCultureIgnoreCase))
                    {
                        doNotImportItem = true;
                        continue;
                    }

                    if (tagsToReplace.Keys.Contains(tagValue))
                    {
                        var newTag = tagsToReplace[tagValue].Trim();

                        if (!newTags.Contains(newTag, StringComparer.InvariantCulture))
                        {
                            newTags.Add(newTag);
                            continue;
                        }
                    }

                    if (!newTags.Contains(tagValue, StringComparer.InvariantCulture))
                    {
                        newTags.Add(tagValue);
                    }
                }
            }

            if (!doNotImportItem && newTags.Count == 0)
            {
                if (ezine == Ezine.HRBerichten)
                {
                    newTags.Add("Berichten van HR");
                }
                else if (ezine == Ezine.MTMededelingen)
                {
                    newTags.Add("MT mededelingen");
                }
                else if (ezine == Ezine.Ondernemingsraad)
                {
                    newTags.Add("OR bericht");
                }
                else
                {
                    newTags.Add("NoCategory");
                }
            }

            var output = string.Empty;

            if (ezine == Ezine.TaxAlert)
            {
                /* 
                 * Uitgaande van het feit dat de volgorde van de tags zijn ontstaan door de volgorde van toekenning ervan door de editor en we dus niet in de 
                 * problemen komen als we de 1e categorie kiezen om toe te kennen en dan vervolgens hetzelfde item weer tegenkomen met een andere 1e categorie het volgende:
                 * Wat vind je van de volgende regels:
                 * - Als een item meerder categorieën heeft dan krijgt het item de categorie toegekend dat als  eerste genoemd wordt. Dit voorkomt lange rijen
                 *   categorie-regels met meerdere categorieën zoals nu is te zien in de Tax export.
                 * - Bovengenoemde regel geldt NIET als er in 1 van de categorieën een VT: W-Belastingplan [JAARTAL] voorkomt. In dat geval valt het item onder het
                 *   genoemde belastingplan.
                */
                if (newTags.Count > 1)
                {
                    foreach (var newTag in newTags)
                    {
                        if (newTag.ToLower().Contains("w-belastingplan"))
                        {
                            newTags.Clear();
                            newTags.Add(newTag);
                            break;
                        }
                    }

                    var firstTag = newTags.First();
                    newTags.Clear();
                    newTags.Add(firstTag);
                }
            }
            else if (ezine == Ezine.CorporateKnowHowAlert || ezine == Ezine.LitigationOnline)
            {
                /* 
                 * All items with multiple categories will get only the category which has been selected first.
                */
                if (newTags.Count > 1)
                {
                    var firstTag = newTags.First();
                    newTags.Clear();
                    newTags.Add(firstTag);
                }
            }

            foreach (var tag in newTags)
            {
                if (!string.IsNullOrEmpty(output))
                {
                    output += ", ";
                }

                output += tag;
            }

            if (output.Contains("Presentaties intern") && output.Contains(","))
            {
                output = "Presentaties intern";
            }
            else if (output.StartsWith("Wet- en regelgeving update, Signaleringen Ontwikkelingen"))
            {
                output = "Wet- en regelgeving update";
            }

            return output;
        }

        private static Dictionary<string, string> GetTagsToReplace(Ezine ezine)
        {
            var tagsToReplace = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);

            if (ezine == Ezine.LitigationOnline)
            {
                tagsToReplace.Add("Aanwinsten", "Aanwinsten literatuur PG");
                tagsToReplace.Add("Class actions", "Class actions");
                tagsToReplace.Add("E-alerts Litigation", "E-Alerts PG Litigation");
                tagsToReplace.Add("Engelse taal", "Engelse taal");
                tagsToReplace.Add("Griffierecht", "Griffierechten en kosten");
                tagsToReplace.Add("Jurisprudentiebundels", "Jurisprudentiebundels");
                tagsToReplace.Add("Know how", "Presentaties intern");
                tagsToReplace.Add("Know how berichten", "Presentaties intern");
                tagsToReplace.Add("Nieuws algemeen", "PG-nieuws");
                tagsToReplace.Add("Presentaties Arbitration Group Amsterdam (AGA) ", "Presentaties AGA");
                tagsToReplace.Add("Presentaties Arbitration Group Amsterdam (AGA)", "Presentaties AGA");
                tagsToReplace.Add("Presentaties extern", "Presentatie extern");
                tagsToReplace.Add("Presentaties Know How Bijeenkomsten", "Presentaties intern");
                tagsToReplace.Add("Presentaties Litigation Academy", "Presentaties intern");
                tagsToReplace.Add("Publicaties Litigation", "Publicaties Litigation");
                tagsToReplace.Add("Seminar Uitspraak Gemist", "Uitspraak gemist seminars");
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
                tagsToReplace.Add("Financiële instellingen", "Financial Regulatory");
                tagsToReplace.Add("Financiële markten", "Financial Regulatory");
                tagsToReplace.Add("Wft wetgeving", "Financial Regulatory");
                tagsToReplace.Add("Hoge raad", "Jurisprudentie");
                tagsToReplace.Add("Actuele uitspraken", "Jurisprudentie");
                tagsToReplace.Add("Know How Nieuws", "Know How Nieuws");
                tagsToReplace.Add("SFP's", "SFP");
                tagsToReplace.Add(HttpUtility.HtmlEncode("SFP's"), "SFP");
                tagsToReplace.Add("AFM", "Wet- en Regelgeving");
                tagsToReplace.Add("AFM berichten", "Wet- en Regelgeving");
                tagsToReplace.Add("AIFM Richtlijn", "Wet- en Regelgeving");
                tagsToReplace.Add("Beloning", "Wet- en Regelgeving");
                tagsToReplace.Add("Europees nieuws", "Wet- en Regelgeving");
                tagsToReplace.Add("ESMA", "Wet- en Regelgeving");
                tagsToReplace.Add("Legislation", "Wet- en Regelgeving");
                tagsToReplace.Add("Wetsvoorstellen", "Wet- en Regelgeving");
                tagsToReplace.Add("Wet- en Regelgeving Update", "Wet- en Regelgeving");
            }
            else if (ezine == Ezine.TaxAlert)
            {
                tagsToReplace.Add("Agenda literatuuroverleg", "Agenda literatuuroverleg");
                tagsToReplace.Add("Arrestenoverzicht", "Arresten hoge raad");
                tagsToReplace.Add("Belastingdienst", "Belastingdienst en aangiften");
                tagsToReplace.Add("Belastingdienst en aangiften", "Belastingdienst en aangiften");
                tagsToReplace.Add("Beconberichten", "Belastingdienst en aangiften");
                tagsToReplace.Add("Kantoor", "Belastingdienst en aangiften");
                tagsToReplace.Add("GT Denmark", "Belastingdienst en aangiften");
                tagsToReplace.Add("GT EU Deveopments", "Belastingdienst en aangiften");
                tagsToReplace.Add("GT Germany", "Belastingdienst en aangiften");
                tagsToReplace.Add("Conferenties / symposia  / seminars", "Conferenties symposia en seminars");
                tagsToReplace.Add("Conferenties/ symposia  /seminars", "Conferenties symposia en seminars");
                tagsToReplace.Add("Conferenties/symposia/seminars", "Conferenties symposia en seminars");
                tagsToReplace.Add("Conferenties/ symposia/seminars", "Conferenties symposia en seminars");
                tagsToReplace.Add("Opleidingen", "Conferenties symposia en seminars");
                tagsToReplace.Add("Presentaties", "Conferenties symposia en seminars");
                tagsToReplace.Add("Seminars", "Conferenties symposia en seminars");
                tagsToReplace.Add("E-Alerts", "eAlerts");
                tagsToReplace.Add("e-Alert", "eAlerts");
                tagsToReplace.Add("Fiscaal overleg andere kantoren", "Fiscaal overleg andere kantoren");
                tagsToReplace.Add("Global layer friends", "Global layer friends");
                tagsToReplace.Add("IBFD", "IBFD _ Nieuwe artikelen");
                tagsToReplace.Add("Know How berichten", "Know how berichten");
                tagsToReplace.Add("Know how nieuws", "Know how berichten");
                tagsToReplace.Add("Know how overleg", "Know how berichten");
                tagsToReplace.Add("Know-How overleg", "Know how berichten");
                tagsToReplace.Add("Kantoor algemeen", "Know how berichten");
                tagsToReplace.Add("Tips & tricks", "Know how berichten");
                tagsToReplace.Add("Tips &amp; tricks", "Know how berichten");
                tagsToReplace.Add("Loyens & Loeff Tax alerts", "Know how berichten");
                tagsToReplace.Add("Loyens &amp; Loeff Tax alerts", "Know how berichten");
                tagsToReplace.Add("Hoge Raad", "Know how berichten");
                tagsToReplace.Add("Landen Nieuws", "Landen Nieuws");
                tagsToReplace.Add("Newsletters", "Landen Nieuws");
                tagsToReplace.Add("Landen: Australia", "Landen Australia");
                tagsToReplace.Add("Landen: Belgie", "Landen Belgie");
                tagsToReplace.Add("Landen: Benelux", "Landen Benelux");
                tagsToReplace.Add("Landen: Caraiben", "Landen Caraiben");
                tagsToReplace.Add("Landen: China", "Landen China");
                tagsToReplace.Add("Landen: Curacao", "Landen Curacao");
                tagsToReplace.Add("Landen: Denemarken", "Landen Denemarken");
                tagsToReplace.Add("Landen: Duitsland", "Landen Duitsland");
                tagsToReplace.Add("Landen: EU", "Landen EU");
                tagsToReplace.Add("Europees nieuws", "Landen EU");
                tagsToReplace.Add("Hvj EU", "Landen EU");
                tagsToReplace.Add("State Aid", "Landen EU");
                tagsToReplace.Add("Landen: Frankrijk", "Landen Frankrijk");
                tagsToReplace.Add("Landen: Griekenland", "Landen Griekenland");
                tagsToReplace.Add("Landen: Ierland", "Landen Ierland");
                tagsToReplace.Add("Landen: Luxemburg", "Landen Luxemburg");
                tagsToReplace.Add("Landen: Oostenrijk", "Landen Oostenrijk");
                tagsToReplace.Add("Landen: Portugal", "Landen Portugal");
                tagsToReplace.Add("Landen: Spanje", "Landen Spanje");
                tagsToReplace.Add("Landen: UK", "Landen UK");
                tagsToReplace.Add("Landen: US", "Landen US");
                tagsToReplace.Add("Landen: Zwitserland", "Landen Zwitserland");
                tagsToReplace.Add("Nieuw boeken", "Nieuwe boeken");
                tagsToReplace.Add("Nieuwetijdschriften", "Nieuwetijdschriften");
                tagsToReplace.Add("Vakbladen", "Nieuwetijdschriften");
                tagsToReplace.Add("Vakliteratuur", "Nieuwetijdschriften");
                tagsToReplace.Add("NOB", "NOB");
                tagsToReplace.Add("SFP's", "SFPs");
                tagsToReplace.Add("Vaktechniek", "Vaktechniek");
                tagsToReplace.Add("VT: Aansprakelijkheid", "VT Aansprakelijkheid");
                tagsToReplace.Add("VT: Arbeidsrecht", "VT Arbeidsrecht");
                tagsToReplace.Add("VT: Bankenbelasting", "VT Bankenbelasting");
                tagsToReplace.Add("VT: Belastingplan", "VT Belastingplan");
                tagsToReplace.Add("VT: Beneficial ownership", "VT Beneficial ownership");
                tagsToReplace.Add("VT: BEPS", "VT BEPS");
                tagsToReplace.Add("VT: Brexit", "VT Brexit");
                tagsToReplace.Add("VT: BTW", "VT BTW");
                tagsToReplace.Add("VT: BvR overdrachtbelasting", "VT BvR overdrachtbelasting");
                tagsToReplace.Add("VT: Diversen", "VT Diversen");
                tagsToReplace.Add("VT: Dividendbelasting", "VT Dividendbelasting");
                tagsToReplace.Add("VT: Estate planning", "VT Estate planning");
                tagsToReplace.Add("VT: EU", "VT EU");
                tagsToReplace.Add("VT: FACTA", "VT FACTA");
                tagsToReplace.Add("VT: Formeel", "VT Formeel");
                tagsToReplace.Add("VT: Formeel Fraus legis", "VT Fraus legis");
                tagsToReplace.Add("VT: FTT", "VT FTT");
                tagsToReplace.Add("VT: Fusies en overnames", "VT Fusies en overnames");
                tagsToReplace.Add("VT: Inkomstenbelasting", "VT Inkomstenbelasting");
                tagsToReplace.Add("VT: Internationaal", "VT Internationaal");
                tagsToReplace.Add("VT: LB Excessieve beloningen", "VT LB Excessieve beloningen");
                tagsToReplace.Add("VT: Loonbelasting", "VT Loonbelasting");
                tagsToReplace.Add("VT: M&A", "VT M&A");
                tagsToReplace.Add("VT: M&amp;A", "VT M&A");
                tagsToReplace.Add("VT: Mandatory disclosure", "VT Mandatory disclosure");
                tagsToReplace.Add("VT: MLI", "VT MLI");
                tagsToReplace.Add("VT: OESO", "VT OESO");
                tagsToReplace.Add("VT: Ondernemingsrecht", "VT Ondernemingsrecht");
                tagsToReplace.Add("VT: Opinies", "VT Opinies");
                tagsToReplace.Add("VT: Real estate", "VT Real estate");
                tagsToReplace.Add("VT: Rulings", "VT Rulings");
                tagsToReplace.Add("VT: Sociale zekerheid", "VT Sociale zekerheid");
                tagsToReplace.Add("VT: Tax Treaties", "VT Tax Treaties");
                tagsToReplace.Add("VT: Tier1", "VT Tier1");
                tagsToReplace.Add("VT: Transfer pricing", "VT Transfer Pricing");
                tagsToReplace.Add("VT: Verschoningsrecht", "VT Verschoningsrecht");
                tagsToReplace.Add("VT: VPB", "VT VPB");
                tagsToReplace.Add("VT: VPB (Niet) transparantie", "VT VPB (Niet) transparantie");
                tagsToReplace.Add("VT: VPB 30% regeling", "VT VPB 30% regeling");
                tagsToReplace.Add("VT: Vpb Beleggingsinstellingen", "VT VPB Beleggingsinstellingen");
                tagsToReplace.Add("VT: VPB Deelnemingsvrijstelling", "VT VPB Deelnemingsvrijstelling");
                tagsToReplace.Add("VT: VPB FGR", "VT VPB FGR");
                tagsToReplace.Add("VT: VPB Fiscale eenheid", "VT VPB Fiscal" +
                    "e eenheid");
                tagsToReplace.Add("VT: Vpb RE Investment Trusts (REITs)", "VT VPB RE Investment Trusts (REITs)");
                tagsToReplace.Add("VT: VPB Rente aftrek", "VT VPB Rente aftrek");
                tagsToReplace.Add("VT: Wetsvoorstellen", "VT Wetsvoorstellen");
                tagsToReplace.Add("VTO", "VTO");
                tagsToReplace.Add("Wetsvoorstellen", "Wetsvoorstellen");
                tagsToReplace.Add("Wetten.nl", "Wetten_nl");
                tagsToReplace.Add("Wft wetgeving", "WWFT");
            }

            return tagsToReplace;
        }

        private static List<string> GetTagsToRemove(Ezine ezine)
        {
            var tagsToRemove = new List<string>();

            if (ezine == Ezine.LitigationOnline)
            {
                tagsToRemove.AddRange(new List<string> { "A&amp;O News", "A&O News", "Financial publications", "Mededeling", "Nieuw in KH systeem en Lit. Online",
                    "Nieuw op de Know-How site", "Nieuwe producten", "Nieuws algemeen", "Nieuwsberichten" });
            }
            else if (ezine == Ezine.CorporateKnowHowAlert)
            {
                tagsToRemove.AddRange(new List<string> { "CESR" });
            }
            else if (ezine == Ezine.TaxAlert)
            {
                tagsToRemove.AddRange(new List<string> { "Alle categorieen", "Belastingzaken", "Bericht", "Bibliotheek", "Brochures A&O", "English pointer",
                    "GT Journals", "Literatuur", "Newsletter fiscoloog", "Newsletter Int. Fiscale Actualiteit", "Nieuwe boeken", "Protocol AAFD", "Rechtsorde",
                    "Scripties", "Tijdschriften", "Brochures A&amp;O", });
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
                tagValues.Add(HttpUtility.HtmlDecode(tag.TagValue.Trim()));
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