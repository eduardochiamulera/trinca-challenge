using System;

namespace CrossCutting.Requests
{
	public class NewBbqRequest
	{
		public NewBbqRequest(DateTime date, string reason)
		{
			Date = date;
			Reason = reason;
		}

		public DateTime Date { get; set; }

		public string Reason { get; set; }
	}
}
