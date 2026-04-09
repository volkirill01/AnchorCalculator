using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using UI.AnchorCalculator.ViewModels;

namespace UI.AnchorCalculator.TagHelpers;

public class PageLinkTagHelper : TagHelper
{
	private IUrlHelperFactory m_UrlHelperFactory;

	public PageLinkTagHelper(IUrlHelperFactory urlHelperFactory)
	{
		m_UrlHelperFactory = urlHelperFactory;
	}

	[ViewContext]
	[HtmlAttributeNotBound]
	public ViewContext ViewContext { get; set; }
	public PageViewModelAnchors PageModel { get; set; }
	public string PageAction { get; set; }

	[HtmlAttributeName(DictionaryAttributePrefix = "page-url-")]
	public Dictionary<string, object> PageUrlValues { get; set; } = new Dictionary<string, object>();

	public override void Process(TagHelperContext context, TagHelperOutput output)
	{
		IUrlHelper urlHelper = m_UrlHelperFactory.GetUrlHelper(ViewContext);
		output.TagName = "div";

		// List of links wrapped in ul
		TagBuilder tag = new TagBuilder("ul");
		tag.AddCssClass("pagination");

		// Creating 3 links, current, previous, next
		TagBuilder currentItem = CreateTag(PageModel.PageNumber, urlHelper);

		// Creating a link for the previous page if it exists
		if (PageModel.HasPreviousPage)
		{
			TagBuilder prevItem = CreateTag(PageModel.PageNumber - 1, urlHelper);
			tag.InnerHtml.AppendHtml(prevItem);
		}
		tag.InnerHtml.AppendHtml(currentItem);

		// Creating a link for the next page if it exists
		if (PageModel.HasNextPage)
		{
			TagBuilder nextItem = CreateTag(PageModel.PageNumber + 1, urlHelper);
			tag.InnerHtml.AppendHtml(nextItem);
		}

		output.Content.AppendHtml(tag);
	}

	private TagBuilder CreateTag(int pageNumber, IUrlHelper urlHelper)
	{
		TagBuilder item = new TagBuilder("li");
		TagBuilder link = new TagBuilder("a");
		if (pageNumber == this.PageModel.PageNumber)
			item.AddCssClass("chosen");
		else
		{
			PageUrlValues["page"] = pageNumber;
			link.Attributes["href"] = urlHelper.Action(PageAction, PageUrlValues);
		}

		item.AddCssClass("page-item");
		link.AddCssClass("page-link");
		link.InnerHtml.Append(pageNumber.ToString());
		item.InnerHtml.AppendHtml(link);
		return item;
	}
}
