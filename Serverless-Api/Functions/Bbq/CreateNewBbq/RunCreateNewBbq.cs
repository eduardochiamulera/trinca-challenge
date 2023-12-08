using CrossCutting.Requests;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Services.Services.Interfaces;
using System.Net;

namespace Serverless_Api
{
	public partial class RunCreateNewBbq
    {
		private readonly IBbqService _bbqService;

		public RunCreateNewBbq(IBbqService bbqService)
		{
			_bbqService = bbqService;
		}

		[Function(nameof(RunCreateNewBbq))]
		public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "churras")] HttpRequestData req)
		{
			var input = await req.Body<NewBbqRequest>();

			var result = await _bbqService.CreateNewBbq(input);

			if (!result.IsSuccess)
			{
				return await req.CreateResponse(result.Error.Code, result);
			}

			return await req.CreateResponse(HttpStatusCode.Created, result);
		}
	}
}
