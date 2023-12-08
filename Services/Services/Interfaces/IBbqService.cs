using CrossCutting.Models;
using CrossCutting.Requests;

namespace Services.Services.Interfaces
{
	public interface IBbqService
	{
		Task<ServiceResult<BbqResponse>> CreateNewBbq(NewBbqRequest input);
		Task<ServiceResult<ShoppingListResponse>> GetShoppingList(string churrasId, string personId);
		Task<ServiceResult<BbqResponse>> ModerateBbq(string churrasId, ModerateBbqRequest input);
	}
}
