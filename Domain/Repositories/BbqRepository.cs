using System;
using Domain.Entities;
using Domain.Repositories.Interfaces;

namespace Domain.Repositories
{
    internal class BbqRepository : StreamRepository<Bbq>, IBbqRepository
    {
        public BbqRepository(IEventStore<Bbq> eventStore) : base(eventStore) { }
    }
}
