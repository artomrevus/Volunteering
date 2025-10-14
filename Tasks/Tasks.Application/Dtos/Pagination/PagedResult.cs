namespace Tasks.Application.Dtos.Pagination;

public class PagedResult<T>(
    IEnumerable<T> items,
    int count,
    int currentPage,
    int pageSize)
{
    public IEnumerable<T> Items { get; set; } = items;
    
    public int CurrentPage { get; set; } = currentPage;
    
    public int PageSize { get; set; } = pageSize;
    
    public int TotalPages { get; set; } = (int)Math.Ceiling(count / (double)pageSize);
    
    public int TotalCount { get; set; } = count;
    
    public bool HasPrevious => CurrentPage > 1;
    
    public bool HasNext => CurrentPage < TotalPages;
}