namespace Serverless_Api.Functions.Bbq.GetShoppingList
{
	public partial class RunGetShoppingList
	{
		public class GetShoppingListRequest
		{
			public GetShoppingListRequest(string bbqId)
			{
				BbqId = bbqId;
			}

			public string BbqId { get; }
		}
	}
}
