namespace UI.AnchorCalculator.Extensions;

public class PagingData
{
	public int Page { get; set; }
	public int Count { get; set; }
	public int PageSize { get; set; }

	public PagingData(int page, int count, int pageSize)
	{
		Page = page;
		Count = count;
		PageSize = pageSize;
	}
}
