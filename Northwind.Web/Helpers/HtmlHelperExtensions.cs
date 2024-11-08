using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Northwind.Web.Helpers
{
    public static class HtmlHelperExtensions
    {
        public static IHtmlContent NorthwindImageLink(this IHtmlHelper htmlHelper, int imageId, string linkText)
        {
            var url = $"/Image/{imageId}";
            var tagBuilder = new TagBuilder("a");
            tagBuilder.Attributes["href"] = url;
            tagBuilder.InnerHtml.Append(linkText);

            return tagBuilder;
        }
    }
}