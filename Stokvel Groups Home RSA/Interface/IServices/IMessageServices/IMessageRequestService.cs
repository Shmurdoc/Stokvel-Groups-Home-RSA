using Stokvel_Groups_Home_RSA.Models;

namespace Stokvel_Groups_Home_RSA.Interface.IServices.IMessageServices
{
	public interface IMessageRequestService
	{

		Task<List<Message>> UserMessages(string id, string managerId, int groupId);
		Task<List<Message>> MemberMessages(string id, string managerId, int groupId);
		string Status(int accountId);
	}
}
