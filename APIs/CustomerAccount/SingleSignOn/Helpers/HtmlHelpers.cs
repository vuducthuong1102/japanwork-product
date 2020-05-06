using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;

namespace SingleSignOn.Helpers
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

        public static MvcHtmlString BootstrapPager(this HtmlHelper helper, int currentPageIndex, Func<int, string> action, int totalItems, int pageSize = 10, int numberOfLinks = 5)
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
            ul.AddCssClass("pagination pull-right no-margin");
            ul.InnerHtml += AddLink(1, action, currentPageIndex == 1, "disabled", "<<", "First Page");
            ul.InnerHtml += AddLink(currentPageIndex - 1, action, !hasPreviousPage, "disabled", "<", "Previous Page");
            for (int i = firstPageNumber; i <= lastPageNumber; i++)
            {
                ul.InnerHtml += AddLink(i, action, i == currentPageIndex, "active", i.ToString(), i.ToString());
            }
            ul.InnerHtml += AddLink(currentPageIndex + 1, action, !hasNextPage, "disabled", ">", "Next Page");
            ul.InnerHtml += AddLink(totalPages, action, currentPageIndex == totalPages, "disabled", ">>", "Last Page");
            return MvcHtmlString.Create(ul.ToString());
        }

        private static TagBuilder AddLink(int index, Func<int, string> action, bool condition, string classToAdd, string linkText, string tooltip)
        {
            var li = new TagBuilder("li");
            li.MergeAttribute("title", tooltip);
            if (condition)
            {
                li.AddCssClass(classToAdd);
            }
            var a = new TagBuilder("a");
            a.MergeAttribute("href", !condition ? action(index) : "javascript:");
            a.SetInnerText(linkText);
            li.InnerHtml = a.ToString();
            return li;
        }


        #endregion

    }
}