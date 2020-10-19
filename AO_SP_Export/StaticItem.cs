namespace AO_SP_Export
{
    using System;
    using System.Web;

    internal class StaticItem
    {
        public int ItemId;
        public string Parent;
        public string Title;
        public string BreadCrumb;
        public string Description;
        public string Content;
        public string AuthorEmailCreated;
        public string AuthorEmailModified;
        public DateTime CreatedOn;
        public DateTime? ModifiedOn;

        public StaticItem(int itemId, string parent, string title, string breadCrumb, string description, string content, string authorEmailCreated, string authorEmailModified, DateTime createdOn, DateTime? modifiedOn)
        {
            this.Parent = parent;
            this.ItemId = itemId;
            this.Title = title;
            this.BreadCrumb = breadCrumb;

            if (!string.IsNullOrEmpty(content))
            {
                if (content.StartsWith("<table class=\"documentHeader\" border=\"0\">") ||
                    content.StartsWith("<table border=\"0\" class=\"documentHeader\">"))
                {
                    content = content.Substring(content.IndexOf("</table>") + "</table>".Length);
                }

                var startIndexStyle = content.ToLower().IndexOf("<style");
                if (startIndexStyle > 0)
                {
                    var endIndexStyle = content.ToLower().IndexOf("</style>");
                    if (endIndexStyle > 0)
                    {
                        var styleString = content.Substring(startIndexStyle, endIndexStyle + "</style>".Length - startIndexStyle);

                        content = content.Replace(styleString, string.Empty);
                    }
                }
                var startIndexScript = content.ToLower().IndexOf("<script");
                if (startIndexScript > 0)
                {
                    var endIndexScript = content.ToLower().IndexOf("</script>");
                    if (endIndexScript > 0)
                    {
                        var scriptString = content.Substring(startIndexScript, endIndexScript + "</script>".Length - startIndexScript);

                        content = content.Replace(scriptString, string.Empty);
                    }
                }

                content = Clean(content);
            }

            if (!string.IsNullOrEmpty(description))
            {
                description = Clean(description);
            }

            this.Description = description;
            this.Content = content;
            this.AuthorEmailCreated = authorEmailCreated;
            this.AuthorEmailModified = authorEmailModified;
            this.CreatedOn = createdOn;
            this.ModifiedOn = modifiedOn;
        }

        private string Clean(string input)
        {
            input = input.Replace("  ", " ");
            input = input.Replace(" ", " ");
            input = input.Replace("<a name=\"", "<a n=\"");
            input = input.Replace("é", HttpUtility.HtmlEncode("é"));
            input = input.Replace("ë", HttpUtility.HtmlEncode("ë"));
            input = input.Replace("è", HttpUtility.HtmlEncode("è"));
            input = input.Replace("ê", HttpUtility.HtmlEncode("ê"));
            input = input.Replace("ẽ", HttpUtility.HtmlEncode("ẽ"));
            input = input.Replace("á", HttpUtility.HtmlEncode("á"));
            input = input.Replace("à", HttpUtility.HtmlEncode("à"));
            input = input.Replace("ä", HttpUtility.HtmlEncode("ä"));
            input = input.Replace("â", HttpUtility.HtmlEncode("â"));
            input = input.Replace("í", HttpUtility.HtmlEncode("í"));
            input = input.Replace("ì", HttpUtility.HtmlEncode("ì"));
            input = input.Replace("ï", HttpUtility.HtmlEncode("ï"));
            input = input.Replace("ó", HttpUtility.HtmlEncode("ó"));
            input = input.Replace("ò", HttpUtility.HtmlEncode("ò"));
            input = input.Replace("ö", HttpUtility.HtmlEncode("ö"));
            input = input.Replace("ú", HttpUtility.HtmlEncode("ú"));
            input = input.Replace("ù", HttpUtility.HtmlEncode("ù"));
            input = input.Replace("ü", HttpUtility.HtmlEncode("ü"));
            input = input.Replace("€", "&euro;");
            input = input.Replace("…", "...");

            return input;
        }
    }
}
