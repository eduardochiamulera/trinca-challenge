using CrossCutting.Requests;
using Domain.Entities;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Services.Services.Interfaces;
using System.Net;

namespace Serverless_Api
{
	public partial class RunDeclineInvite
    {
        private readonly Person _user;
		private readonly IPersonService _personService;

		public RunDeclineInvite(Person user, IPersonService personService)
		{
			_user = user;
			_personService = personService;
		}


		[Function(nameof(RunDeclineInvite))]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "put", Route = "person/invites/{inviteId}/decline")] HttpRequestData req, string inviteId)
        {
			var answer = await req.Body<InviteAnswerRequest>();

			var response = await _personService.DeclineInvite(_user.Id, inviteId, answer);

			if (!response.IsSuccess)
			{
				return await req.CreateResponse(response.Error.Code, response);
			}

			return await req.CreateResponse(HttpStatusCode.OK, response);
		}
    }
}
