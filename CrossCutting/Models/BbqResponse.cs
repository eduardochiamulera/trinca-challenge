using System;

namespace CrossCutting.Models
{
	public class BbqResponse
	{
		public string Id { get; set; }
		public DateTime Date { get; set; }
		public bool IsTrincasPaying { get; set; }
		public string Status { get; set; }

	}
}
