﻿using Eveneum;
using System.Threading.Tasks;

namespace Domain.Repositories.Interfaces
{
    public interface IStreamRepository<T> where T : AggregateRoot, new()
    {
        Task<StreamHeaderResponse> GetHeaderAsync(string streamId);
        Task<T?> GetAsync(string streamId);
        Task SaveAsync(T entity);
    }
}