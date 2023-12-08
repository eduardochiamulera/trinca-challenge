using CrossCutting.Models;
using Domain.Enumerations;

namespace Domain.Entities
{
	public class ShoppingList
	{
		public decimal Vegetables { get; private set; }
		public decimal Meat { get; private set; }

		internal void UpdateBbqListShop(decimal vegetables, decimal meat, ShoppingListCalculationType type)
		{
			if(type == ShoppingListCalculationType.Add)
			{
				Vegetables += vegetables;
				Meat += meat;
			}
			else
			{
				Vegetables -= vegetables;
				Meat -= meat;
			}
		}

		public ShoppingListResponse TakeSnapshot()
		{
			return new ShoppingListResponse
			{
				Vegetables = Vegetables,
				Meat = Meat
			};
		}
	}
}
