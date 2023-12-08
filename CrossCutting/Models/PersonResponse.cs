using System.Collections.Generic;

namespace CrossCutting.Models
{
	public class PersonResponse
	{
		public string Id { get; set; }

		public string Name { get; set; }

		public bool IsCoOwner { get; set; }

		public IEnumerable<InviteResponse> Invites { get; set; }
	}
}
