using Domain.Enumerations;

namespace Domain.Events
{
    public class BbqStatusUpdatedAutomatic : IEvent
    {
        public BbqStatusUpdatedAutomatic(BbqStatus status)
        {
            Status = status;
        }

        public BbqStatus Status { get; }
    }
}
