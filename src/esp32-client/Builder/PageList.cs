namespace esp32_client.Builder;

public class PagedListModel<T> where T : class
{
    public List<T> Data { get; set; } = new List<T>();
    public bool HasNextPage { get; set; }
    public bool HasPreviousPage { get; set; }
    public int TotalCount { get; set; }
    public int TotalPage { get; set; }
}

public static class PagedList
{
    public static PagedListModel<T> ToPagedListModel<T>(this List<T> data, int pageIndex = 0, int pageSize = int.MaxValue) where T : class
    {
        var response = new PagedListModel<T>();

        if (pageSize == 0) return response;

        int dataCount = data.Count;

        response.TotalCount = dataCount;
        response.TotalPage = dataCount / pageSize + (dataCount % pageSize > 0 ? 1 : 0);
        response.HasPreviousPage = pageIndex > 0;
        response.HasNextPage = pageIndex < response.TotalPage - 1;
        response.Data = data.Skip(pageIndex * pageSize).Take(pageSize).ToList();

        return response;
    }
}
