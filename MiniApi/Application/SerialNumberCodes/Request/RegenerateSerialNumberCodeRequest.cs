using System.ComponentModel.DataAnnotations;

namespace MiniApi.Application.SerialNumberCodes.Request;

public class RegenerateSerialNumberCodeRequest
{
    [Range(1, 1000000)]
    public long TotalSerialNumberCodeCount { get; set; }
}