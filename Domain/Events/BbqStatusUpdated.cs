using Domain.Entities;

namespace Domain.Events
{
    public class BbqStatusUpdated : IEvent
    {
        public BbqStatusUpdated(bool gonnaHappen, bool trincaWillPay)
        {
            GonnaHappen = gonnaHappen;
            TrincaWillPay = trincaWillPay;
        }

        public bool GonnaHappen { get; }
        public bool TrincaWillPay { get; }

        public BbqStatus Status
        {
            get
            {
                return GonnaHappen ? BbqStatus.PendingConfirmations : BbqStatus.ItsNotGonnaHappen;

			}
        }
    }
}
