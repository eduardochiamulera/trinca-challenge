using CrossCutting.Requests;
using Domain.Entities;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Services.Services.Interfaces;
using System.Net;

namespace Serverless_Api
{
	public partial class RunAcceptInvite
	{
		private readonly Person _user;
		private readonly IPersonService _personService;

		public RunAcceptInvite(Person user, IPersonService personService)
		{
			_user = user;
			_personService = personService;
		}


		[Function(nameof(RunAcceptInvite))]
		public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "put", Route = "person/invites/{inviteId}/accept")] HttpRequestData req, string inviteId)
		{
			var answer = await req.Body<InviteAnswerRequest>();

			var response = await _personService.AcceptInvite(_user.Id, inviteId, answer);

			if (!response.IsSuccess)
			{
				return await req.CreateResponse(response.Error.Code, response);
			}

			return await req.CreateResponse(HttpStatusCode.OK, response);

		}
	}
}
