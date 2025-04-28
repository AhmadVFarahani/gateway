using Gateway.Application.Base;
using Gateway.Domain.Entities;

namespace Gateway.Application.ApiKeys;

public class ApiKeyDto:BaseDto
{
    public string Key { get; set; } = string.Empty;
    public long UserId { get; set; }
    public bool IsActive { get; set; }
    public DateTime? ExpirationDate { get; set; }
}
