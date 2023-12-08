using CrossCutting;
using CrossCutting.Models;
using CrossCutting.Requests;
using Domain;
using Domain.Entities;
using Domain.Events;
using Domain.Repositories.Interfaces;
using Services.Services.Interfaces;
using System;

namespace Services.Services
{
	public class BbqService : IBbqService
	{
		private readonly IBbqRepository _bbqRepository;
		private readonly IPersonRepository _personRepository;
		private readonly SnapshotStore _snapshots;

		public BbqService(IBbqRepository bbqRepository, IPersonRepository personRepository, SnapshotStore snapshots)
		{
			_bbqRepository = bbqRepository;
			_personRepository = personRepository;
			_snapshots = snapshots;
		}

		public async Task<ServiceResult<BbqResponse>> CreateNewBbq(NewBbqRequest input)
		{
			if (input == null)
			{
				return ServiceResult<BbqResponse>.Failure(Constants.InputRequired);
			}

			if (input.Date <= DateTime.UtcNow)
			{
				return ServiceResult<BbqResponse>.Failure(Constants.BadRequest("Date for the barbecue is invalid."));
			}

			var churras = new Bbq();

			churras.Apply(new ThereIsSomeoneElseInTheMood(Guid.NewGuid(), input.Date, input.Reason));

			var lookups = await _snapshots.AsQueryable<Lookups>("Lookups").SingleOrDefaultAsync();

			await _bbqRepository.SaveAsync(churras);

			foreach (var personId in lookups.ModeratorIds)
			{
				var person = await _personRepository.GetAsync(personId);
				person.Apply(new PersonHasBeenInvitedToBbq(churras.Id, churras.Date, churras.Reason));
				await _personRepository.SaveAsync(person);
			}

			return ServiceResult<BbqResponse>.Success(churras.TakeSnapshot());
		}

		public async Task<ServiceResult<ShoppingListResponse>> GetShoppingList(string churrasId, string personId)
		{
			var person = await _personRepository.GetAsync(personId);

			if (person == null)
			{
				return ServiceResult<ShoppingListResponse>.Failure(Constants.NotFound(nameof(person)));
			}

			if (!person.IsCoOwner)
			{
				return ServiceResult<ShoppingListResponse>.Failure(new Error(403, "Person is not authorized"));
			}

			var bbq = await _bbqRepository.GetAsync(churrasId);

			if (bbq == null)
			{
				return ServiceResult<ShoppingListResponse>.Failure(Constants.NotFound(nameof(bbq)));
			}

			return ServiceResult<ShoppingListResponse>.Success(bbq.ShoppingList.TakeSnapshot());
		}

		public async Task<ServiceResult<BbqResponse>> ModerateBbq(string churrasId, ModerateBbqRequest input)
		{
			var bbq = await _bbqRepository.GetAsync(churrasId);

			if (bbq == null)
			{
				return ServiceResult<BbqResponse>.Failure(Constants.NotFound(nameof(bbq)));
			}

			bbq.Apply(new BbqStatusUpdated(input.GonnaHappen, input.TrincaWillPay));

			await _bbqRepository.SaveAsync(bbq);

			var lookups = await _snapshots.AsQueryable<Lookups>("Lookups").SingleOrDefaultAsync();

			if (input.GonnaHappen)
			{
				foreach (var personId in lookups.PeopleIds)
				{
					var person = await _personRepository.GetAsync(personId);
					if (!person.IsCoOwner)
					{
						var @event = new PersonHasBeenInvitedToBbq(bbq.Id, bbq.Date, bbq.Reason);
						person.Apply(@event);
						await _personRepository.SaveAsync(person);
					}
				}
			}
			else
			{
				foreach (var personId in lookups.PeopleIds)
				{
					var person = await _personRepository.GetAsync(personId);
					var @event = new InviteWasDeclined { InviteId = bbq.Id, PersonId = person.Id };
					person.Apply(@event);
					await _personRepository.SaveAsync(person);
				}
			}

			return ServiceResult<BbqResponse>.Success(bbq.TakeSnapshot());
		}
	}
}
