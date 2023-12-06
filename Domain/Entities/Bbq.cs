using Domain.Events;
using System;
using System.Dynamic;

namespace Domain.Entities
{
	public class Bbq : AggregateRoot
	{
		public string Reason { get; set; }
		public BbqStatus Status { get; set; }
		public DateTime Date { get; set; }
		public bool IsTrincasPaying { get; set; }
		public int BbqConfirmation { get; private set; }
		public ShoppingList ShoppingList { get; private set; }
		public Bbq()
		{
			ShoppingList = new ShoppingList();
		}

		public void When(ThereIsSomeoneElseInTheMood @event)
		{
			Id = @event.Id.ToString();
			Date = @event.Date;
			Reason = @event.Reason;
			Status = BbqStatus.New;
		}

		public void When(BbqStatusUpdatedAutomatic @event)
		{
			Status = @event.Status;
		}

		public void When(BbqStatusUpdated @event)
		{
			Status = @event.Status;
			IsTrincasPaying = @event.TrincaWillPay;
		}

		public void When(InviteWasDeclined @event)
		{
			if (BbqConfirmation == default)
			{
				return;
			}

			BbqConfirmation -= 1;
			if (@event.IsVeg)
			{
				ShoppingList.UpdateBbqListShop((Constants.QuantidadeVegetaisVegetarianosKilos * -1), 0, ShoppingListCalculationType.Decrement);
			}
			else
			{
				ShoppingList.UpdateBbqListShop(Constants.QuantidadeVegetaisKilos, Constants.QuantidadeCarneKilos, ShoppingListCalculationType.Decrement);
			}
		}

		public void When(InviteWasAccepted @event)
		{
			BbqConfirmation += 1;
			if (@event.IsVeg)
			{
				ShoppingList.UpdateBbqListShop(Constants.QuantidadeVegetaisVegetarianosKilos, 0, ShoppingListCalculationType.Add);
			}
			else
			{
				ShoppingList.UpdateBbqListShop(Constants.QuantidadeVegetaisKilos, Constants.QuantidadeCarneKilos, ShoppingListCalculationType.Add);
			}
		}


		public object TakeSnapshot()
		{
            return new
            {
                Id,
                Date,
                IsTrincasPaying,
                Status = Status.ToString()
            };
		}
	}
}
