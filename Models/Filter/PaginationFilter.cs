namespace mypaperwork.Models.Filter;

public class PaginationFilter
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public string? SortBy { get; set; }
    public string? FilterValue { get; set; }
    public bool? IsSortDescending { get; set; }
    public PaginationFilter()
    {
        this.PageNumber = 1;
        this.PageSize = 50;
    }
    public PaginationFilter(int pageNumber, int pageSize)
    {
        this.PageNumber = pageNumber < 1 ? 1 : pageNumber;
        this.PageSize = pageSize > 10 ? 10 : pageSize;
    }
}