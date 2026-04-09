namespace UI.AnchorCalculator.ViewModels;

public class PageViewModelAnchors
{
	public int PageNumber { get; private set; }
	public int TotalPages { get; private set; }
	public int PageSize { get; private set; }

	public PageViewModelAnchors(int count, int pageNumber, int pageSize)
	{
		PageNumber = pageNumber;
		TotalPages = (int)Math.Ceiling(count / (double)pageSize);
		PageSize = pageSize;
	}

	public bool HasPreviousPage => PageNumber > 1;
	public bool HasNextPage => PageNumber < TotalPages;
}
