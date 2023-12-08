using CrossCutting.Requests;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Services.Services.Interfaces;
using System.Net;

namespace Serverless_Api
{
	public partial class RunModerateBbq
	{
		private readonly IBbqService _bbqService;

		public RunModerateBbq(IBbqService bbqService)
		{
			_bbqService = bbqService;
		}

		[Function(nameof(RunModerateBbq))]
		public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "put", Route = "churras/{id}/moderar")] HttpRequestData req, string id)
		{
			var moderationRequest = await req.Body<ModerateBbqRequest>();

			var response = await _bbqService.ModerateBbq(id, moderationRequest);

			if (!response.IsSuccess)
			{
				return await req.CreateResponse(response.Error.Code, response);
			}


			return await req.CreateResponse(HttpStatusCode.OK, response);
		}
	}
}
