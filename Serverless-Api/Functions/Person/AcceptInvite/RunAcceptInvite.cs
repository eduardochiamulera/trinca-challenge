using Domain.Events;
using Domain.Entities;
using Domain.Repositories;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Domain;
using Domain.Enumerations;

namespace Serverless_Api
{
    public partial class RunAcceptInvite
	{
		private readonly Person _user;
		private readonly IPersonRepository _repository;
		private readonly IBbqRepository _bbqRepository;

		public RunAcceptInvite(IPersonRepository repository, Person user, IBbqRepository bbqRepository)
		{
			_user = user;
			_repository = repository;
			_bbqRepository = bbqRepository;
		}

		[Function(nameof(RunAcceptInvite))]
		public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "put", Route = "person/invites/{inviteId}/accept")] HttpRequestData req, string inviteId)
		{
			var bbq = await _bbqRepository.GetAsync(inviteId);

			if (bbq.Status == BbqStatus.ItsNotGonnaHappen)
			{
				return await req.CreateResponse(System.Net.HttpStatusCode.OK, "Bbq Its Not Gonna Happen.");
			}

			var answer = await req.Body<InviteAnswer>();

			var person = await _repository.GetAsync(_user.Id);

			if (person.Invites.Any(x => x.Id == inviteId && x.Status == InviteStatus.Accepted))
			{
				return await req.CreateResponse(System.Net.HttpStatusCode.OK, "Invite already accepted.");
			}

			var @event = new InviteWasAccepted { InviteId = inviteId, IsVeg = answer.IsVeg, PersonId = person.Id };

			person.Apply(@event);

			await _repository.SaveAsync(person);

			bbq.Apply(@event);
			await _bbqRepository.SaveAsync(bbq);
			//implementar efeito do aceite do convite no churrasco
			//quando tiver 7 pessoas ele está confirmado
			if (bbq.BbqConfirmation >= Constants.NumeroConfirmacoesAlteraStatusBbq && bbq.Status != BbqStatus.Confirmed)
			{
				bbq = await _bbqRepository.GetAsync(inviteId);
				bbq.Apply(new BbqStatusUpdatedAutomatic(BbqStatus.Confirmed));
				await _bbqRepository.SaveAsync(bbq);
			}
			

			return await req.CreateResponse(System.Net.HttpStatusCode.OK, person.TakeSnapshot());
		}
	}
}
