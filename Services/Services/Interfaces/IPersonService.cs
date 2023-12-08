using CrossCutting.Models;
using CrossCutting.Requests;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Services.Interfaces
{
	public interface IPersonService
	{
		Task<ServiceResult<IEnumerable<BbqResponse>>> GetProposedBbq(string personId);

		Task<ServiceResult<PersonResponse>> AcceptInvite(string personId, string inviteId, InviteAnswerRequest input);
		Task<ServiceResult<PersonResponse>> DeclineInvite(string personId, string inviteId, InviteAnswerRequest input);
		Task<ServiceResult<PersonResponse>> GetInvites(string personId);
	}
}
