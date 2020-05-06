using MySite.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace MySite.Helpers
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
                                                                string cssClassName = "", string tagName = "span")
        {

            /*<span class="help-button" data-original-title="Hello Tooltip!" data-rel="tooltip" data-placement="top">?</span>*/

            var metadata = ModelMetadata.FromLambdaExpression(expression, self.ViewData);
            var description = metadata.Description;

            if (!string.IsNullOrEmpty(description))
            {
                var tag = new TagBuilder(tagName) { InnerHtml = "?" };
                cssClassName = "help-button " + cssClassName;
                tag.AddCssClass(cssClassName);

                tag.MergeAttribute("data-rel", "tooltip");
                tag.MergeAttribute("data-placement", "right");
                tag.MergeAttribute("data-original-title", description);

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
            //ul.AddCssClass("m-datatable__pager-nav");
            //ul.InnerHtml += AddLink(1, action, currentPageIndex == 1, "m-datatable__pager-link m-datatable__pager-link--first m-datatable__pager-link--disabled", "<<", UserWebResource.LB_FIRST_PAGE, "<i class='la la-angle-double-left'></i>", onclickEnvent);
            ul.InnerHtml += AddLink(currentPageIndex - 1, action, !hasPreviousPage, "prev", "<", UserWebResource.LB_PREV_PAGE, "<i class='la la-angle-left'></i>", onclickEnvent);
            for (int i = firstPageNumber; i <= lastPageNumber; i++)
            {
                ul.InnerHtml += AddLink(i, action, i == currentPageIndex, "active", i.ToString(), i.ToString(), onclickEnvent);
            }
            ul.InnerHtml += AddLink(currentPageIndex + 1, action, !hasNextPage, "next", ">", UserWebResource.LB_NEXT_PAGE, "<i class='la la-angle-right'></i>", onclickEnvent);
            //ul.InnerHtml += AddLink(totalPages, action, currentPageIndex == totalPages, "m-datatable__pager-link m-datatable__pager-link--first m-datatable__pager-link--disabled", ">>", UserWebResource.LB_LAST_PAGE, "<i class='la la-angle-double-right'></i>", onclickEnvent);
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
                li.AddCssClass(classToAdd);
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

        public static string ShowHtmlTextByLimit(string content, int maxLength = 120)
        {
            if (!string.IsNullOrEmpty(content))
            {
                content = HtmlRemoval.StripTagsCharArray(content);
                if(content.Length > maxLength)
                {
                    content = content.Substring(0, maxLength) + "...";
                }
            }

            return content;
        }

        public static string CheckMaxLengthReturnClass(string content, int maxLength = 80)
        {
            var returnclass = "";
           
            if (!string.IsNullOrEmpty(content))
            {
                if (content.Length > maxLength)
                {
                    returnclass = "_10lg";
                }
            }

            return returnclass;
        }
        public static string CheckLikeCountReturnClass( int likecount)
        {
            var returnclass = "";
            if (likecount>0)
            {
                 returnclass = "_10lp";
            }

            return returnclass;
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
    }
}