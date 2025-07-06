using Gateway.Application.Base;

namespace Gateway.Application.AccessPolicies.Dtos;

public class AccessPolicyDto:BaseDto
{
    public long? UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public long? ApiKeyId { get; set; }
    public string? ApiKey  { get; set; } = string.Empty;
    public long ScopeId { get; set; }
    public string ScopeName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}
