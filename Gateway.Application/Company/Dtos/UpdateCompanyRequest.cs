﻿namespace Gateway.Application.Company.Dtos;

public class UpdateCompanyRequest
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}
