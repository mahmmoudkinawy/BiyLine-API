namespace BiyLineApi.Helpers;
public class PaginationParams
{
    private const int _maxPageSize = 30;
    private int _pageNumber = 1;
    private int _pageSize = 5;

    public int PageNumber
    {
        get => _pageNumber;
        set => _pageNumber = value < 1 ? 1 : value;
    }

    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value < 1 ? 1 : (value > _maxPageSize ? _maxPageSize : value);
    }
}
