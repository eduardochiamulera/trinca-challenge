using System;
using Domain.Entities;
using Domain.Repositories.Interfaces;

namespace Domain.Repositories
{
    internal class PersonRepository : StreamRepository<Person>, IPersonRepository
    {
        public PersonRepository(IEventStore<Person> eventStore) : base(eventStore) { }
    }
}
