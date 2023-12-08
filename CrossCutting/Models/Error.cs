using System.Net;

namespace CrossCutting.Models
{
	public sealed class Error
	{
		public Error(int code, string message)
		{
			Code = (HttpStatusCode)code;
			Message = message;
		}

		public HttpStatusCode Code { get; }
		public string Message { get; set; }
	}
}
