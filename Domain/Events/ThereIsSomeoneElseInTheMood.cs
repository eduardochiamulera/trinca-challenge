using System;

namespace Domain.Events
{
    public class ThereIsSomeoneElseInTheMood : IEvent
    {
        public ThereIsSomeoneElseInTheMood(Guid id, DateTime date, string reason)
        {
            Id = id;
            Date = date;
            Reason = reason;
        }

        public Guid Id { get; set; }
        public string Reason { get; set; }
        public DateTime Date { get; set; }
    }
}
