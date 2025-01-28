using Stokvel_Groups_Home_RSA.Models;

namespace Stokvel_Groups_Home_RSA.Interface.IServices.IMessageServices
{
	public interface IMessageServices
	{

        Task<List<Message>> GetUserMessagesAsync(string userId, string managerId, int groupId);

        Task<List<Message>> GetMemberMessagesAsync(int groupId);


    }
}
