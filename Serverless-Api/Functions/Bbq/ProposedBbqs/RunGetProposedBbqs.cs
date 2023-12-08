using Domain.Entities;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Services.Services.Interfaces;
using System.Net;

namespace Serverless_Api
{
    public partial class RunGetProposedBbqs
    {
        private readonly Person _user;
        private readonly IPersonService _personService;

		public RunGetProposedBbqs(Person user, IPersonService personService)
		{
			_user = user;
			_personService = personService;
		}


        [Function(nameof(RunGetProposedBbqs))]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "churras")] HttpRequestData req)
        {

            var response = await _personService.GetProposedBbq(_user.Id);

            if (!response.IsSuccess)
            {
				return await req.CreateResponse(response.Error.Code, response);
			}

            return await req.CreateResponse(HttpStatusCode.OK, response);
        }
    }
}
