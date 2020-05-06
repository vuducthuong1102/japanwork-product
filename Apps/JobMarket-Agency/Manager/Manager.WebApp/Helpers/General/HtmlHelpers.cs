using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Linq.Expressions;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Manager.WebApp.Resources;
using System.Text.RegularExpressions;

namespace Manager.WebApp.Helpers
{
    public static class HtmlHelpers
    {

        #region Extensions

        /// <summary>
        /// https://gist.github.com/scottcate/4469809
        /// 
        /// Usage:
        /// @Html.DescriptionFor(m => m.PropertyName)
        /// 
        /// supply cssclass name, and override span with div tag
        /// @Html.DescriptionFor(m => m.PropertyName, "desc", "div")
        /// 
        /// using the named param
        /// @Html.DescriptionFor(m => m.PropertyName, tagName: "div")
        /// </summary>
        public static MvcHtmlString DescriptionFor<TModel, TValue>(this HtmlHelper<TModel> self,
                                                                Expression<Func<TModel, TValue>> expression,
                                                                string cssClassName = "", string tagName = "span")
        {
            var metadata = ModelMetadata.FromLambdaExpression(expression, self.ViewData);
            var description = metadata.Description;

            if (!string.IsNullOrEmpty(description))
            {
                var tag = new TagBuilder(tagName) { InnerHtml = description };
                if (!string.IsNullOrEmpty(cssClassName))
                    tag.AddCssClass(cssClassName);

                return new MvcHtmlString(tag.ToString());
            }

            return MvcHtmlString.Empty;
        }



        public static MvcHtmlString HelpButtonFor<TModel, TValue>(this HtmlHelper<TModel> self,
                                                                Expression<Func<TModel, TValue>> expression,
                                                                string cssClassName = "", string tagName = "span", string place = "", string dataSkin = "")
        {

            /*<span class="help-button" data-original-title="Hello Tooltip!" data-rel="tooltip" data-placement="top">?</span>*/

            var metadata = ModelMetadata.FromLambdaExpression(expression, self.ViewData);
            var description = metadata.Description;

            if (!string.IsNullOrEmpty(description))
            {
                //var tag = new TagBuilder(tagName) { InnerHtml = "?" };
                var tag = new TagBuilder(tagName) { InnerHtml = "" };
                cssClassName = "help-button " + cssClassName;
                tag.AddCssClass(cssClassName);

                //tag.MergeAttribute("data-rel", "tooltip");
                tag.MergeAttribute("data-toggle", "m-tooltip");
                tag.MergeAttribute("data-width", "auto");
                tag.MergeAttribute("data-original-title", description);
                tag.MergeAttribute("data-skin", dataSkin);
                tag.MergeAttribute("data-placement", place);

                //tag.MergeAttribute("data-placement", "right");
                //tag.MergeAttribute("data-original-title", description);

                return new MvcHtmlString(tag.ToString());
            }

            return MvcHtmlString.Empty;
        }





        /// <summary>
        /// http://weblogs.asp.net/gunnarpeipman/asp-net-mvc-how-to-show-asterisk-after-required-field-label
        /// http://cru.caes.ucdavis.edu/blog/posts/its-magic/extend-mvc-labelfor-method
        /// http://stackoverflow.com/questions/5940506/how-can-i-modify-labelfor-to-display-an-asterisk-on-required-fields        
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
        public static MvcHtmlString LabelForRequired<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, string labelText = "")
        {

            return LabelHelper(html,

                ModelMetadata.FromLambdaExpression(expression, html.ViewData),

                ExpressionHelper.GetExpressionText(expression), labelText);

        }

        /*
         required:after { content:" *"; color: #f00; } 
         */
        private static MvcHtmlString LabelHelper(HtmlHelper html,
            ModelMetadata metadata, string htmlFieldName, string labelText)
        {
            if (string.IsNullOrEmpty(labelText))
            {
                labelText = metadata.DisplayName ?? metadata.PropertyName ?? htmlFieldName.Split('.').Last();
            }

            if (string.IsNullOrEmpty(labelText))
            {
                return MvcHtmlString.Empty;
            }

            bool isRequired = false;

            if (metadata.ContainerType != null)
            {
                isRequired = metadata.ContainerType.GetProperty(metadata.PropertyName)
                                .GetCustomAttributes(typeof(RequiredAttribute), false)
                                .Length == 1;
            }

            TagBuilder tag = new TagBuilder("label");
            tag.Attributes.Add(
                "for",
                TagBuilder.CreateSanitizedId(
                    html.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(htmlFieldName)
                )
            );

            if (isRequired)
                tag.Attributes.Add("class", "label-required");

            tag.SetInnerText(labelText);
            var output = tag.ToString(TagRenderMode.Normal);

            if (isRequired)
            {
                var asteriskTag = new TagBuilder("span");
                asteriskTag.Attributes.Add("class", "required");
                asteriskTag.SetInnerText("*");
                output += asteriskTag.ToString(TagRenderMode.Normal);
            }

            return MvcHtmlString.Create(output);
        }







        #endregion

      
        public static IEnumerable<SelectListItem> TimeZoneList(string currentTimeZoneId)
        {
            var timeZoneList = TimeZoneInfo
            .GetSystemTimeZones()
            .Select(t => new SelectListItem
            {
                Text = t.DisplayName,
                Value = t.Id,
                Selected = !string.IsNullOrEmpty(currentTimeZoneId) && t.Id == currentTimeZoneId
            });

            return timeZoneList;
        }

        public static MvcHtmlString AntiForgeryTokenForAjaxPost(this HtmlHelper helper)
        {
            var antiForgeryInputTag = helper.AntiForgeryToken().ToString();
            // Above gets the following: <input name="__RequestVerificationToken" type="hidden" value="PnQE7R0MIBBAzC7SqtVvwrJpGbRvPgzWHo5dSyoSaZoabRjf9pCyzjujYBU_qKDJmwIOiPRDwBV1TNVdXFVgzAvN9_l2yt9-nf4Owif0qIDz7WRAmydVPIm6_pmJAI--wvvFQO7g0VvoFArFtAR2v6Ch1wmXCZ89v0-lNOGZLZc1" />
            var removedStart = antiForgeryInputTag.Replace(@"<input name=""__RequestVerificationToken"" type=""hidden"" value=""", "");
            var tokenValue = removedStart.Replace(@""" />", "");
            if (antiForgeryInputTag == removedStart || removedStart == tokenValue)
                throw new InvalidOperationException("Oops! The Html.AntiForgeryToken() method seems to return something I did not expect.");
            return new MvcHtmlString(string.Format(@"{0}:""{1}""", "__RequestVerificationToken", tokenValue));
        }

        #region BootstrapPager

        //http://weblogs.asp.net/imranbaloch/a-simple-bootstrap-pager-html-helper

        public static MvcHtmlString BootstrapPager(this HtmlHelper helper, int currentPageIndex, Func<int, string> action, int totalItems, int pageSize = 10, int numberOfLinks = 5, string onclickEnvent = "")
        {
            if (totalItems <= 0)
            {
                return MvcHtmlString.Empty;
            }
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            var lastPageNumber = (int)Math.Ceiling((double)currentPageIndex / numberOfLinks) * numberOfLinks;
            var firstPageNumber = lastPageNumber - (numberOfLinks - 1);
            var hasPreviousPage = currentPageIndex > 1;
            var hasNextPage = currentPageIndex < totalPages;
            if (lastPageNumber > totalPages)
            {
                lastPageNumber = totalPages;
            }
            var ul = new TagBuilder("ul");            
            //ul.AddCssClass("pagination");
            ul.AddCssClass("m-datatable__pager-nav");
            ul.InnerHtml += AddLink(1, action, currentPageIndex == 1, "m-datatable__pager-link m-datatable__pager-link--first m-datatable__pager-link--disabled", "<<", ManagerResource.LB_FIRST_PAGE,"<i class='la la-angle-double-left'></i>", onclickEnvent);
            ul.InnerHtml += AddLink(currentPageIndex - 1, action, !hasPreviousPage, "m-datatable__pager-link m-datatable__pager-link--first m-datatable__pager-link--disabled", "<", ManagerResource.LB_PREVIOUS_PAGE, "<i class='la la-angle-left'></i>", onclickEnvent);
            for (int i = firstPageNumber; i <= lastPageNumber; i++)
            {
                ul.InnerHtml += AddLink(i, action, i == currentPageIndex, "m-datatable__pager-link m-datatable__pager-link-number m-datatable__pager-link--active", i.ToString(), i.ToString(), onclickEnvent);
            }
            ul.InnerHtml += AddLink(currentPageIndex + 1, action, !hasNextPage, "m-datatable__pager-link m-datatable__pager-link--first m-datatable__pager-link--disabled", ">", ManagerResource.LB_NEXT_PAGE, "<i class='la la-angle-right'></i>", onclickEnvent);
            ul.InnerHtml += AddLink(totalPages, action, currentPageIndex == totalPages, "m-datatable__pager-link m-datatable__pager-link--first m-datatable__pager-link--disabled", ">>", ManagerResource.LB_LAST_PAGE, "<i class='la la-angle-double-right'></i>", onclickEnvent);
            return MvcHtmlString.Create(ul.ToString());
        }

        private static TagBuilder AddLink(int index, Func<int, string> action, bool condition, string classToAdd, string linkText, string tooltip, string onclickEvent = "")
        {
            var li = new TagBuilder("li");
            li.MergeAttribute("title", tooltip);                   
            
            var a = new TagBuilder("a");
            if (condition)
            {
                a.AddCssClass(classToAdd);
            }
            else
            {
                if (!string.IsNullOrEmpty(onclickEvent))
                    li.MergeAttribute("onclick", onclickEvent + "(" + index + ")");

                a.AddCssClass("m-datatable__pager-link m-datatable__pager-link-number");
            }
            a.MergeAttribute("href", !condition ? action(index) : "javascript:");
            a.SetInnerText(linkText);
            li.InnerHtml = a.ToString();
            return li;
        }

        private static TagBuilder AddLink(int index, Func<int, string> action, bool condition, string classToAdd, string linkText, string tooltip, string linkIconHtml = "", string onclickEvent = "")
        {
            //var li = new TagBuilder("li");
            //li.MergeAttribute("title", tooltip);

            //var a = new TagBuilder("a");
            //a.AddCssClass("pager-control");

            //a.MergeAttribute("href", !condition ? action(index) : "javascript:");
            //a.MergeAttribute("data-page", index.ToString());
            //if (!string.IsNullOrEmpty(linkIconHtml))
            //    a.InnerHtml = linkIconHtml;
            //else
            //    a.SetInnerText(linkText);

            //if (condition)
            //{
            //    li.AddCssClass(classToAdd);
            //}

            //li.InnerHtml = a.ToString();
            //return li;

            var li = new TagBuilder("li");
            li.MergeAttribute("title", tooltip);
           
            var a = new TagBuilder("a");
            if (condition)
            {
                a.AddCssClass(classToAdd);                
            }
            else
            {
                if (!string.IsNullOrEmpty(onclickEvent))
                    li.MergeAttribute("onclick", onclickEvent + "(" + index + ")");
                a.AddCssClass("m-datatable__pager-link m-datatable__pager-link-number");
            }
            a.MergeAttribute("href", !condition ? action(index) : "javascript:");

            if (!string.IsNullOrEmpty(linkIconHtml))
                a.InnerHtml = linkIconHtml;
            else
                a.SetInnerText(linkText);

            li.InnerHtml = a.ToString();
            return li;
        }
        #endregion


        #region BootstrapPager

        //http://weblogs.asp.net/imranbaloch/a-simple-bootstrap-pager-html-helper

        public static MvcHtmlString BootstrapPagerFrontEnd(this HtmlHelper helper, int currentPageIndex, Func<int, string> action, int totalItems, int pageSize = 10, int numberOfLinks = 5, string onclickEnvent = "")
        {
            if (totalItems <= 0)
            {
                return MvcHtmlString.Empty;
            }
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            var lastPageNumber = (int)Math.Ceiling((double)currentPageIndex / numberOfLinks) * numberOfLinks;
            var firstPageNumber = lastPageNumber - (numberOfLinks - 1);
            var hasPreviousPage = currentPageIndex > 1;
            var hasNextPage = currentPageIndex < totalPages;
            if (lastPageNumber > totalPages)
            {
                lastPageNumber = totalPages;
            }
            var ul = new TagBuilder("ul");
            ul.AddCssClass("pagination nobottommargin");
            ul.InnerHtml += AddLinkFrontEnd(1, action, currentPageIndex == 1, "disabled", "<<", ManagerResource.LB_FIRST_PAGE, "", onclickEnvent);
            ul.InnerHtml += AddLinkFrontEnd(currentPageIndex - 1, action, !hasPreviousPage, "disabled", "<", ManagerResource.LB_PREVIOUS_PAGE, "", onclickEnvent);
            for (int i = firstPageNumber; i <= lastPageNumber; i++)
            {
                ul.InnerHtml += AddLinkFrontEnd(i, action, i == currentPageIndex, "active", i.ToString(), i.ToString(), onclickEnvent);
            }
            ul.InnerHtml += AddLinkFrontEnd(currentPageIndex + 1, action, !hasNextPage, "disabled", ">", ManagerResource.LB_NEXT_PAGE, "", onclickEnvent);
            ul.InnerHtml += AddLinkFrontEnd(totalPages, action, currentPageIndex == totalPages, "disabled", ">>", ManagerResource.LB_LAST_PAGE, "", onclickEnvent);
            return MvcHtmlString.Create(ul.ToString());
        }

        private static TagBuilder AddLinkFrontEnd(int index, Func<int, string> action, bool condition, string classToAdd, string linkText, string tooltip, string onclickEvent = "")
        {
            var li = new TagBuilder("li");
            li.MergeAttribute("title", tooltip);
            li.AddCssClass("page-item");
            var a = new TagBuilder("a");
            if (condition)
            {
                //a.AddCssClass(classToAdd);
                li.AddCssClass(classToAdd);
            }
            else
            {
                if (!string.IsNullOrEmpty(onclickEvent))
                    li.MergeAttribute("onclick", onclickEvent + "(" + index + ")");
            }

            a.AddCssClass("page-link");
            a.MergeAttribute("href", !condition ? action(index) : "javascript:");
            a.SetInnerText(linkText);
            li.InnerHtml = a.ToString();
            return li;
        }

        private static TagBuilder AddLinkFrontEnd(int index, Func<int, string> action, bool condition, string classToAdd, string linkText, string tooltip, string linkIconHtml = "", string onclickEvent = "")
        {
            var li = new TagBuilder("li");
            li.MergeAttribute("title", tooltip);
            li.AddCssClass("page-item");
            var a = new TagBuilder("a");
            if (condition)
            {
                //a.AddCssClass(classToAdd);
                li.AddCssClass(classToAdd);
            }
            else
            {
                if (!string.IsNullOrEmpty(onclickEvent))
                    li.MergeAttribute("onclick", onclickEvent + "(" + index + ")");               
            }
            a.AddCssClass("page-link");
            a.MergeAttribute("href", !condition ? action(index) : "javascript:");

            if (!string.IsNullOrEmpty(linkIconHtml))
                a.InnerHtml = linkIconHtml;
            else
                a.SetInnerText(linkText);

            li.InnerHtml = a.ToString();
            return li;
        }
        #endregion

        public static string RemoveScriptTags(string htmlText)
        {
            if (string.IsNullOrEmpty(htmlText))
                return htmlText;

            var cleanStr = htmlText;

            cleanStr = Regex.Replace(cleanStr, "<script[\\d\\D]*?>[\\d\\D]*?</script>", "", RegexOptions.Singleline | RegexOptions.IgnoreCase);

            return cleanStr;
        }

        public static string ShowHtmlTextByLimit(string content, int maxLength = 120)
        {
            if (!string.IsNullOrEmpty(content))
            {
                content = HtmlRemoval.StripTagsCharArray(content);
                if (content.Length > maxLength)
                {
                    content = content.Substring(0, maxLength) + "...";
                }
            }

            return content;
        }
    }

    public static class HtmlRemoval
    {
        /// <summary>
        /// Remove HTML from string with Regex.
        /// </summary>
        public static string StripTagsRegex(string source)
        {
            return Regex.Replace(source, "<.*?>", string.Empty);
        }

        /// <summary>
        /// Compiled regular expression for performance.
        /// </summary>
        static Regex _htmlRegex = new Regex("<.*?>", RegexOptions.Compiled);

        /// <summary>
        /// Remove HTML from string with compiled Regex.
        /// </summary>
        public static string StripTagsRegexCompiled(string source)
        {
            return _htmlRegex.Replace(source, string.Empty);
        }

        /// <summary>
        /// Remove HTML tags from string using char array.
        /// </summary>
        public static string StripTagsCharArray(string source)
        {
            char[] array = new char[source.Length];
            int arrayIndex = 0;
            bool inside = false;

            for (int i = 0; i < source.Length; i++)
            {
                char let = source[i];
                if (let == '<')
                {
                    inside = true;
                    continue;
                }
                if (let == '>')
                {
                    inside = false;
                    continue;
                }
                if (!inside)
                {
                    array[arrayIndex] = let;
                    arrayIndex++;
                }
            }
            return new string(array, 0, arrayIndex);
        }

        public static bool IsHtmlFragment(string value)
        {
            return Regex.IsMatch(value, @"</?(p|div)>");
        }

        /// <summary>
        /// Remove tags from a html string
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string RemoveTags(string value)
        {
            if (value != null)
            {
                value = CleanHtmlComments(value);
                value = CleanHtmlBehaviour(value);
                value = Regex.Replace(value, @"</[^>]+?>", " ");
                value = Regex.Replace(value, @"<[^>]+?>", "");
                value = value.Trim();
            }
            return value;
        }

        /// <summary>
        /// Clean script and styles html tags and content
        /// </summary>
        /// <returns></returns>
        public static string CleanHtmlBehaviour(string value)
        {
            value = Regex.Replace(value, "(<style.+?</style>)|(<script.+?</script>)", "", RegexOptions.IgnoreCase | RegexOptions.Singleline);

            return value;
        }

        /// <summary>
        /// Replace the html commens (also html ifs of msword).
        /// </summary>
        public static string CleanHtmlComments(string value)
        {
            //Remove disallowed html tags.
            value = Regex.Replace(value, "<!--.+?-->", "", RegexOptions.IgnoreCase | RegexOptions.Singleline);

            return value;
        }

        /// <summary>
        /// Adds rel=nofollow to html anchors
        /// </summary>
        public static string HtmlLinkAddNoFollow(string value)
        {
            return Regex.Replace(value, "<a[^>]+href=\"?'?(?!#[\\w-]+)([^'\">]+)\"?'?[^>]*>(.*?)</a>", "<a href=\"$1\" rel=\"nofollow\" target=\"_blank\">$2</a>", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        }

        public static string StripHTML(string source)
        {
            try
            {
                string result;

                // Remove HTML Development formatting
                // Replace line breaks with space
                // because browsers inserts space
                result = source.Replace("\r", " ");
                // Replace line breaks with space
                // because browsers inserts space
                result = result.Replace("\n", " ");
                // Remove step-formatting
                result = result.Replace("\t", string.Empty);
                // Remove repeating spaces because browsers ignore them
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"( )+", " ");

                // Remove the header (prepare first by clearing attributes)
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<( )*head([^>])*>", "<head>",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"(<( )*(/)( )*head( )*>)", "</head>",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         "(<head>).*(</head>)", string.Empty,
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                // remove all scripts (prepare first by clearing attributes)
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<( )*script([^>])*>", "<script>",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"(<( )*(/)( )*script( )*>)", "</script>",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                //result = System.Text.RegularExpressions.Regex.Replace(result,
                //         @"(<script>)([^(<script>\.</script>)])*(</script>)",
                //         string.Empty,
                //         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"(<script>).*(</script>)", string.Empty,
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                // remove all styles (prepare first by clearing attributes)
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<( )*style([^>])*>", "<style>",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"(<( )*(/)( )*style( )*>)", "</style>",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         "(<style>).*(</style>)", string.Empty,
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                // insert tabs in spaces of <td> tags
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<( )*td([^>])*>", "\t",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                // insert line breaks in places of <BR> and <LI> tags
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<( )*br( )*>", "\r",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<( )*li( )*>", "\r",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                // insert line paragraphs (double line breaks) in place
                // if <P>, <DIV> and <TR> tags
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<( )*div([^>])*>", "\r\r",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<( )*tr([^>])*>", "\r\r",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<( )*p([^>])*>", "\r\r",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                // Remove remaining tags like <a>, links, images,
                // comments etc - anything that's enclosed inside < >
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<[^>]*>", string.Empty,
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                // replace special characters:
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @" ", " ",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&bull;", " * ",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&lsaquo;", "<",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&rsaquo;", ">",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&trade;", "(tm)",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&frasl;", "/",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&lt;", "<",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&gt;", ">",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&copy;", "(c)",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&reg;", "(r)",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                // Remove all others. More can be added, see
                // http://hotwired.lycos.com/webmonkey/reference/special_characters/
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&(.{2,6});", string.Empty,
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                // for testing
                //System.Text.RegularExpressions.Regex.Replace(result,
                //       this.txtRegex.Text,string.Empty,
                //       System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                // make line breaking consistent
                result = result.Replace("\n", "\r");

                // Remove extra line breaks and tabs:
                // replace over 2 breaks with 2 and over 4 tabs with 4.
                // Prepare first to remove any whitespaces in between
                // the escaped characters and remove redundant tabs in between line breaks
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         "(\r)( )+(\r)", "\r\r",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         "(\t)( )+(\t)", "\t\t",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         "(\t)( )+(\r)", "\t\r",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         "(\r)( )+(\t)", "\r\t",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                // Remove redundant tabs
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         "(\r)(\t)+(\r)", "\r\r",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                // Remove multiple tabs following a line break with just one tab
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         "(\r)(\t)+", "\r\t",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                // Initial replacement target string for line breaks
                string breaks = "\r\r\r";
                // Initial replacement target string for tabs
                string tabs = "\t\t\t\t\t";
                for (int index = 0; index < result.Length; index++)
                {
                    result = result.Replace(breaks, "\r\r");
                    result = result.Replace(tabs, "\t\t\t\t");
                    breaks = breaks + "\r";
                    tabs = tabs + "\t";
                }

                // That's it.
                return result;
            }
            catch
            {              
                return source;
            }
        }
    }
}