using HtmlAgilityPack;
using System;
using System.Collections.Generic;

namespace AO_SP_Export
{
    internal class ItemAttachment
    {
        internal readonly int Id;
        internal readonly string Title;
        internal readonly byte[] FileData;
        internal readonly string FileName;
        internal readonly string MimeType;

        internal readonly string FileNameUrl;

        internal ItemAttachment(int id, string title, byte[] fileData, string fileName, string mimeType)
        {
            fileName = fileName.Replace("\\", string.Empty);
            fileName = fileName.Replace("/", string.Empty);
            fileName = fileName.Replace(":", string.Empty);
            fileName = fileName.Replace("*", string.Empty);
            fileName = fileName.Replace("?", string.Empty);
            fileName = fileName.Replace("\"", string.Empty);
            fileName = fileName.Replace("<", string.Empty);
            fileName = fileName.Replace(">", string.Empty);
            fileName = fileName.Replace("|", string.Empty);
            fileName = fileName.Replace("#", string.Empty);
            fileName = fileName.Replace("{", string.Empty);
            fileName = fileName.Replace("}", string.Empty);
            fileName = fileName.Replace("%", string.Empty);
            fileName = fileName.Replace("~", string.Empty);
            fileName = fileName.Replace("&", string.Empty);
            fileName = fileName.Replace("–", string.Empty);

            this.Id = id;
            this.Title = title;
            this.FileData = fileData;
            this.FileName = fileName;
            this.MimeType = mimeType;

            this.FileNameUrl = Guid.NewGuid().ToString().Substring(0, 8) + "_" + fileName;
        }
    }

    internal class EzineItem
    {
        internal readonly int Id;
        internal readonly string Title;
        internal readonly string Content;
        internal readonly string ContentInnerText;

        internal readonly Guid GuidId;
        internal readonly Guid DocId;
        internal readonly string Version;
        internal readonly int Order;
        internal readonly string ContentTypeId;
        internal readonly string Author;
        internal readonly string ModiiedBy;

        internal readonly string ParentId;
        internal readonly string ParentWebId;
        internal readonly string ParentWebUrl;
        internal readonly string ParentFolderId;
        internal readonly string ParentListId;

        internal readonly string Url;
        internal readonly string FileUrl;
        internal readonly string Name;
        internal readonly string DirName;

        internal readonly DateTime CreatedOn;
        internal readonly DateTime? ModifiedOn;

        internal readonly string Description;
        internal readonly string Data;
        internal readonly string AuthorEmail;

        internal readonly byte[] ImageData;
        internal readonly string ImageFileName;
        internal readonly string TagValue;

        internal readonly string ImageFileNameUrl;

        internal readonly List<ItemAttachment> Attachments;

        internal EzineItem(int id, string title, string content, string description, string data, string authorEmail, byte[] imageData, string imageFileName, string tagValue,
            DateTime createdOn, DateTime? modifiedOn, List<ItemAttachment> attachments)
        {
            this.Description = description;
            this.Data = data;
            this.AuthorEmail = authorEmail;
            this.ImageData = imageData;
            this.ImageFileName = imageFileName;

            if (!string.IsNullOrEmpty(imageFileName))
            {
                this.ImageFileNameUrl = Guid.NewGuid().ToString().Substring(0, 8) + ImageFileName;
            }
            else
            {
                this.ImageFileNameUrl = "";
            }

            this.TagValue = tagValue;

            this.Attachments = attachments;

            content = RemoveOldHyperlinks(content);
            content = AddAttachments(content, attachments);

            this.Id = id;
            this.Title = title + " - " + tagValue;
            this.Content = content;
            this.ContentInnerText = StripHtml(content);
            this.CreatedOn = createdOn;
            this.ModifiedOn = modifiedOn;

            this.Author = "13590";
            this.ModiiedBy = Author;

            this.GuidId = Guid.NewGuid();
            this.DocId = Guid.NewGuid();
            this.Order = 2000;
            this.Version = "1.0";
            this.ContentTypeId = "0x010100C568DB52D9D0A14D9B2FDCC96666E9F2007948130EC3DB064584E219954237AF3900B37ECDCFE86B4247B74261B966D494E20051EC7EA0A6BA4547A4E4F5A0B5A4D3E100B4992F7AE6064C919F74AD3FF72AC75200ADCAC66E6555B542A94D314B2C86FFDB";

            // are these for corporate know how??
            this.ParentId = "94bbd111-79fa-4872-998d-ac37834198ab";
            this.ParentWebId = "0f48c312-58c7-4982-9071-8cb0dc277589";
            this.ParentFolderId = "1f5d10a5-d6ef-4e74-a864-37988029fd5e";
            this.ParentListId = ParentId;
            this.ParentWebUrl = "/Locations/Europe/Netherlands/NL Corporate";

            var filteredTitle = Title.Replace(" ", "-").Replace("&", string.Empty).Replace(";", string.Empty).Replace(":", string.Empty).Replace(",", string.Empty).Replace(".", string.Empty)
                .Replace("?", string.Empty).Replace("!", string.Empty).Replace("@", string.Empty).Replace("#", string.Empty).Replace("%", string.Empty).Replace("<", string.Empty)
                .Replace(">", string.Empty).Replace("'", string.Empty).Replace("`", string.Empty).Replace("\"", string.Empty).Replace("\\", string.Empty).Replace("/", string.Empty);

            this.Name = filteredTitle + "-" + this.Id + "_v1.aspx";
            this.FileUrl = "/Pages/" + Name;
            this.Url = ParentWebUrl + FileUrl;

            this.DirName = "Locations/Europe/Netherlands/NL Corporate/Pages";
        }

        private string RemoveOldHyperlinks(string content)
        {
            while (content.Contains("href=\"http://amsmartsite"))
            {
                var startIndex = content.IndexOf("href=\"http://amsmartsite");
                var endIndex = content.IndexOf("\"", startIndex + "href=\"".Length);

                var hyperlinkContent = content.Substring(startIndex, endIndex + 1 - startIndex);

                content = content.Replace(hyperlinkContent, "title=\"Deze hyperlink is door de migratie naar SharePoint niet meer beschikbaar.\"");
            }

            return content;
        }

        private string AddAttachments(string content, List<ItemAttachment> attachments)
        {
            if (attachments.Count > 0)
            {
                content += "<p><strong>Bijlagen:</strong></p>";
                content += "<ul>";

                foreach (var attachment in attachments)
                {
                    content += "<li>";
                    content += $"<a target=\"_blank\" href=\"http://global.intranet.allenovery.com/locations/europe/netherlands/archief/{attachment.FileNameUrl}\">";
                    content += $"{attachment.Title}";
                    content += "</a>";
                    content += "</li>";
                }

                content += "</ul>";
            }

            return content;
        }

        private string StripHtml(string content)
        {
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(content);
            return htmlDoc.DocumentNode.InnerText;
        }
    }
}