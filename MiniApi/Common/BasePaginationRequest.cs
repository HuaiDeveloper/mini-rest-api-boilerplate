using System.ComponentModel.DataAnnotations;

namespace MiniApi.Common;

public class BasePaginationRequest
{
    [Range(1, Int32.MaxValue)]
    public int Page { get; set; }
    
    [Range(1, 50)]
    public int Size { get; set; }
}