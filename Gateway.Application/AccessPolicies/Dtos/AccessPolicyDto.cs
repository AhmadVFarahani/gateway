using Gateway.Application.Base;

namespace Gateway.Application.AccessPolicies.Dtos;

public class AccessPolicyDto:BaseDto
{
    public long? UserId { get; set; }
    public long? ApiKeyId { get; set; }
    public long ScopeId { get; set; }
    public bool IsActive { get; set; }
}
