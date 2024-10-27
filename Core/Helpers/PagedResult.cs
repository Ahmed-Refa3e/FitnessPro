namespace Core.Helpers;

public class PagedResult<T>(List<T> data, int count, int pageNumber, int pageSize)
{
    public int PageNumber { get; set; } = pageNumber;
    public int PageSize { get; set; } = pageSize;
    public int TotalPages { get; set; } = (int)Math.Ceiling(count / (double)pageSize);
    public int TotalRecords { get; set; } = count;
    public List<T> Data { get; set; } = data;
}

