using CrossCutting;
using Domain.Entities;
using Domain.Events;
using Domain.Repositories.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Net;

namespace Serverless_Api
{
    public partial class RunModerateBbq
	{
		private readonly SnapshotStore _snapshots;
		private readonly IPersonRepository _persons;
		private readonly IBbqRepository _repository;

		public RunModerateBbq(IBbqRepository repository, SnapshotStore snapshots, IPersonRepository persons)
		{
			_persons = persons;
			_snapshots = snapshots;
			_repository = repository;
		}

		[Function(nameof(RunModerateBbq))]
		public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "put", Route = "churras/{id}/moderar")] HttpRequestData req, string id)
		{
			var moderationRequest = await req.Body<ModerateBbqRequest>();

			var bbq = await _repository.GetAsync(id);

			if (bbq == null)
			{
				return await req.CreateResponse(HttpStatusCode.NotFound, "Bbq not found.");
			}

			bbq.Apply(new BbqStatusUpdated(moderationRequest.GonnaHappen, moderationRequest.TrincaWillPay));
			await _repository.SaveAsync(bbq);

			var lookups = await _snapshots.AsQueryable<Lookups>("Lookups").SingleOrDefaultAsync();

			if (moderationRequest.GonnaHappen)
			{
				foreach (var personId in lookups.PeopleIds)
				{
					var person = await _persons.GetAsync(personId);
					if (!person.IsCoOwner)
					{
						var @event = new PersonHasBeenInvitedToBbq(bbq.Id, bbq.Date, bbq.Reason);
						person.Apply(@event);
						await _persons.SaveAsync(person);
					}
				}
			}
			else
			{
				foreach (var personId in lookups.PeopleIds)
				{
					var person = await _persons.GetAsync(personId);
					var @event = new InviteWasDeclined { InviteId = bbq.Id, PersonId = person.Id } ;
					person.Apply(@event);
					await _persons.SaveAsync(person);
				}
			}


			return await req.CreateResponse(HttpStatusCode.OK, bbq.TakeSnapshot());
		}
	}
}
