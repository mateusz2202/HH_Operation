﻿namespace OperationAPI.Interfaces;

public interface ICacheService
{
    Task<T?> GetAsync<T>(string key);
    Task<bool> SetAsync<T>(string key, T value, DateTimeOffset expirationTime);
    Task<bool> RemoveAsync(string key);
}
