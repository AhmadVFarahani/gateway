namespace Gateway.Domain.Entities;

public class Scope:BaseEntity
{
    public string Name { get; set; } = string.Empty; // مثل payment:write
    public string DisplayName { get; set; } = string.Empty; // مثل "ثبت پرداخت"
    public string Description { get; set; } = string.Empty; // توضیحات برای مدیریت
    public bool IsActive { get; set; } = true;
}