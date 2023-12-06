using CrossCutting;
using Domain.Entities;
using Domain.Repositories;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Net;
using static Serverless_Api.Functions.Bbq.GetShoppingList.RunGetShoppingList;

namespace Serverless_Api
{
	public partial class RunGetShoppingList
	{
		private readonly Person _user;
		private readonly SnapshotStore _snapshots;
		private readonly IPersonRepository _personRepository;
		private readonly IBbqRepository _bbqRepository;
		public RunGetShoppingList(IBbqRepository bbqRepository, IPersonRepository personsRepository, SnapshotStore snapshots, Person user)
		{
			_user = user;
			_snapshots = snapshots;
			_personRepository = personsRepository;
			_bbqRepository = bbqRepository;
		}

		[Function(nameof(RunGetShoppingList))]
		public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "churras/{churrasId}/shoppingList")] HttpRequestData req, string churrasId)
		{
			var person = await _personRepository.GetAsync(_user.Id);

			if(person == null)
			{
				return await req.CreateResponse(HttpStatusCode.NotFound, "Person not found.");
			}

			if (!person.IsCoOwner)
			{
				return await req.CreateResponse(HttpStatusCode.Forbidden, null);
			}

			var bbq = await _bbqRepository.GetAsync(churrasId);

			if(bbq == null)
			{
				return await req.CreateResponse(HttpStatusCode.NotFound, "Bbq not found.");
			}

			var shoppingListSnapshot = bbq.ShoppingList.TakeSnapshot();

			return await req.CreateResponse(HttpStatusCode.OK, shoppingListSnapshot);
		}
	}
}
