using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Northwind.Web.Helpers
{
    [HtmlTargetElement("a", Attributes = NorthwindIdAttributeName)]
    public class NorthwindImageLinkTagHelper : TagHelper
    {
        private const string NorthwindIdAttributeName = "northwind-id";

        [HtmlAttributeName(NorthwindIdAttributeName)]
        public int NorthwindId { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var url = $"/Image/{NorthwindId}";

            output.Attributes.SetAttribute("href", url);

            if (output.Content.IsEmptyOrWhiteSpace)
            {
                output.Content.SetContent("View Image");
            }
        }
    }
}