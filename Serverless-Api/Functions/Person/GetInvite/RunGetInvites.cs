using Domain.Entities;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Services.Services.Interfaces;
using System.Net;

namespace Serverless_Api
{
    public partial class RunGetInvites
    {
        private readonly Person _user;
		private readonly IPersonService _personService;

		public RunGetInvites(Person user, IPersonService personService)
        {
            _user = user;
			_personService = personService;
        }

        [Function(nameof(RunGetInvites))]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "person/invites")] HttpRequestData req)
        {
            var response = await _personService.GetInvites(_user.Id);

			if (!response.IsSuccess)
			{
				return await req.CreateResponse(response.Error.Code, response);
			}

			return await req.CreateResponse(HttpStatusCode.OK, response);
		}
    }
}
