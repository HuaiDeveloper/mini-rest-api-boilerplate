namespace MiniApi.Common;

public class BasePaginationResponse<T> : BaseResponse<T>
{
    public int TotalCount { get; set; } = default!;
}