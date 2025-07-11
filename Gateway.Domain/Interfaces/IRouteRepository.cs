﻿using Gateway.Domain.Entities;

namespace Gateway.Domain.Interfaces;

public interface IRouteRepository
{
    Task<Route?> GetByIdAsync(long id);
    Task<IEnumerable<Route>> GetAllAsync();
    Task<IEnumerable<Route>> GetByServiceId(long serviceId);
    Task AddAsync(Route route);
    Task UpdateAsync(Route route);
    Task DeleteAsync(Route route);
    Task<Route?> GetByPathAsync(string path);
}
