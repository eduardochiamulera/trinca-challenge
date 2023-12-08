using Domain.Entities;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Services.Services.Interfaces;
using System.Net;

namespace Serverless_Api
{
	public partial class RunGetShoppingList
	{
		private readonly Person _user;
		private readonly IBbqService _bbqService;

		public RunGetShoppingList(Person user, IBbqService bbqService)
		{
			_user = user;
			_bbqService = bbqService;
		}

		[Function(nameof(RunGetShoppingList))]
		public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "churras/{churrasId}/shoppingList")] HttpRequestData req, string churrasId)
		{
			var response = await _bbqService.GetShoppingList(churrasId, _user.Id);

			if (!response.IsSuccess)
			{
				return await req.CreateResponse(response.Error.Code, response);
			}

			return await req.CreateResponse(HttpStatusCode.OK, response);
		}
	}
}
