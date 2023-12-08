using CrossCutting.Models;
using Domain.Enumerations;
using Domain.Events;
using System;

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

		internal void When(ThereIsSomeoneElseInTheMood @event)
		{
			Id = @event.Id.ToString();
			Date = @event.Date;
			Reason = @event.Reason;
			Status = BbqStatus.New;
		}

		internal void When(BbqStatusUpdatedAutomatic @event)
		{
			Status = @event.Status;
		}

		internal void When(BbqStatusUpdated @event)
		{
			if(@event.Status == BbqStatus.ItsNotGonnaHappen)
			{
				ShoppingList = new ShoppingList();
				BbqConfirmation= 0;
			}

			Status = @event.Status;
			IsTrincasPaying = @event.TrincaWillPay;
		}

		internal void When(InviteWasDeclined @event)
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

		internal void When(InviteWasAccepted @event)
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


		public BbqResponse TakeSnapshot()
		{
            return new BbqResponse
			{
                Id = Id,
                Date = Date,
                IsTrincasPaying = IsTrincasPaying,
                Status = Status.ToString()
            };
		}
	}
}
