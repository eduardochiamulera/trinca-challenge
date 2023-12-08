using CrossCutting.Models;
using CrossCutting.Requests;
using Domain;
using Domain.Enumerations;
using Domain.Events;
using Domain.Repositories.Interfaces;
using Services.Services.Interfaces;

namespace Services.Services
{
	internal class PersonService : IPersonService
	{
		private readonly IBbqRepository _bbqRepository;
		private readonly IPersonRepository _personRepository;

		public PersonService(IBbqRepository bbqRepository, IPersonRepository personRepository)
		{
			_bbqRepository = bbqRepository;
			_personRepository = personRepository;
		}

		public async Task<ServiceResult<PersonResponse>> AcceptInvite(string personId, string inviteId, InviteAnswerRequest input)
		{
			if(input == null)
			{
				return ServiceResult<PersonResponse>.Failure(Constants.InputRequired);
			}

			var bbq = await _bbqRepository.GetAsync(inviteId);

			if(bbq == null)
			{
				return ServiceResult<PersonResponse>.Failure(Constants.NotFound("Bbq not found."));
			}

			if (bbq.Status == BbqStatus.ItsNotGonnaHappen)
			{
				return ServiceResult<PersonResponse>.Success(null, "Bbq Its Not Gonna Happen.");
			}

			var person = await _personRepository.GetAsync(personId);

			if (person == null)
			{
				return ServiceResult<PersonResponse>.Failure(Constants.NotFound("Person not found."));
			}

			if (person.Invites.Any(x => x.Id == inviteId && x.Status == InviteStatus.Accepted))
			{
				return ServiceResult<PersonResponse>.Success(null, "Invite already accepted.");
			}

			var @event = new InviteWasAccepted { InviteId = inviteId, IsVeg = input.IsVeg, PersonId = person.Id };

			person.Apply(@event);

			await _personRepository.SaveAsync(person);

			bbq.Apply(@event);
			await _bbqRepository.SaveAsync(bbq);
			
			if (bbq.BbqConfirmation >= Constants.NumeroConfirmacoesAlteraStatusBbq && bbq.Status != BbqStatus.Confirmed)
			{
				bbq = await _bbqRepository.GetAsync(inviteId);
				bbq.Apply(new BbqStatusUpdatedAutomatic(BbqStatus.Confirmed));
				await _bbqRepository.SaveAsync(bbq);
			}

			return ServiceResult<PersonResponse>.Success(person.TakeSnapshot());
		}

		public async Task<ServiceResult<PersonResponse>> DeclineInvite(string personId, string inviteId, InviteAnswerRequest input)
		{
			if (input == null)
			{
				return ServiceResult<PersonResponse>.Failure(Constants.InputRequired);
			}

			var bbq = await _bbqRepository.GetAsync(inviteId);

			if (bbq == null)
			{
				return ServiceResult<PersonResponse>.Failure(Constants.NotFound(nameof(bbq)));
			}

			if (bbq.Status == BbqStatus.ItsNotGonnaHappen)
			{
				return ServiceResult<PersonResponse>.Success(null, "Bbq Its Not Gonna Happen.");
			}

			var person = await _personRepository.GetAsync(personId);

			if (person == null)
			{
				return ServiceResult<PersonResponse>.Failure(Constants.NotFound(nameof(person)));
			}

			if (person.Invites.Any(x => x.Id == inviteId && x.Status == InviteStatus.Declined))
			{
				return ServiceResult<PersonResponse>.Success(null, "Invite already accepted.");
			}

			var @event = new InviteWasDeclined { InviteId = inviteId, IsVeg = input.IsVeg, PersonId = person.Id };

			person.Apply(@event);

			await _personRepository.SaveAsync(person);

			bbq.Apply(@event);
			await _bbqRepository.SaveAsync(bbq);

			if (bbq.BbqConfirmation < Constants.NumeroConfirmacoesAlteraStatusBbq && bbq.Status != BbqStatus.PendingConfirmations)
			{
				bbq = await _bbqRepository.GetAsync(inviteId);
				bbq.Apply(new BbqStatusUpdatedAutomatic(BbqStatus.PendingConfirmations));
				await _bbqRepository.SaveAsync(bbq);
			}

			return ServiceResult<PersonResponse>.Success(person.TakeSnapshot());
		}

		public async Task<ServiceResult<PersonResponse>> GetInvites(string personId)
		{
			var person = await _personRepository.GetAsync(personId);

			if (person == null)
			{
				return ServiceResult<PersonResponse>.Failure(Constants.NotFound(nameof(person)));
			}

			return ServiceResult<PersonResponse>.Success(person.TakeSnapshot());
		}

		public async Task<ServiceResult<IEnumerable<BbqResponse>>> GetProposedBbq(string personId)
		{
			var snapshots = new List<BbqResponse>();
			var person = await _personRepository.GetAsync(personId);

			if (person == null)
			{
				return ServiceResult<IEnumerable<BbqResponse>>.Failure(Constants.NotFound(nameof(person)));
			}

			foreach (var bbqId in person.Invites.Where(i => i.Date > DateTime.Now).Select(o => o.Id))
			{
				var bbq = await _bbqRepository.GetAsync(bbqId);

				if (bbq.Status != BbqStatus.ItsNotGonnaHappen)
				{
					snapshots.Add(bbq.TakeSnapshot());
				}
			}

			return ServiceResult<IEnumerable<BbqResponse>>.Success(snapshots);
		}
	}
}
