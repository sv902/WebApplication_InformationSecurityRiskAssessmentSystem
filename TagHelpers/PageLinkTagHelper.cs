using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.Razor.TagHelpers;
using WebApplication_InformationSecurityRiskAssessmentSystem.ViewModels;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace WebApplication_InformationSecurityRiskAssessmentSystem.TagHelpers
{
    public class PageLinkTagHelper : TagHelper
    {
        private IUrlHelperFactory? _urlHelperFactory;

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext? ViewContext { get; set; }

        public PageViewModel? PageModel {  get; set; }
        public string? PageAction { get; set; }

        public PageLinkTagHelper(IUrlHelperFactory urlHelperFactory)
        {
            _urlHelperFactory = urlHelperFactory;           
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            IUrlHelper urlHelper = _urlHelperFactory!.GetUrlHelper(ViewContext!);
            output.TagName = "div";

            // 1 -> <ul>
            TagBuilder tag = new TagBuilder("ul");
            tag.AddCssClass("pagination");

            // 2 -> посилання на попердню сторінку
            if (PageModel!.HasPreviousPage)
            {
                TagBuilder prevItem = CreateTag(PageModel.PageNumber - 1, urlHelper);
                tag.InnerHtml.AppendHtml(prevItem);
            }

            // 3 -> посилання на поточну сторінку
            TagBuilder currItem = CreateTag(PageModel.PageNumber, urlHelper);
            tag.InnerHtml.AppendHtml(currItem);

            // 4 -> посилання на наступну сторінку
            if (PageModel!.HasNextPage)
            {
                TagBuilder nextItem = CreateTag(PageModel.PageNumber + 1, urlHelper);
                tag.InnerHtml.AppendHtml(nextItem);
            }

            // 5 -> вставка списку
            output.Content.AppendHtml(tag);
        }

        private  TagBuilder CreateTag(int pageNumber, IUrlHelper urlHelper)
        {
            TagBuilder item = new TagBuilder("li");
            TagBuilder link = new TagBuilder("a");

            // 1
            if (pageNumber == PageModel?.PageNumber)
            {
                item.AddCssClass("active");
            }
            else
            {
                link.Attributes["href"] = urlHelper.Action(PageAction, new { pageNumber = pageNumber });
            }

            // 2
            item.AddCssClass("page-item");
            link.AddCssClass("page-link");
            link.InnerHtml.Append(pageNumber.ToString());
            item.InnerHtml.AppendHtml(link);

            // 3
            return item;
        }
    }
}
