using Stokvel_Groups_Home_RSA.Models;
using Stokvel_Groups_Home_RSA.Interface.Infrastructure;

namespace Stokvel_Groups_Home_RSA.Interface.Messages;

public interface IMessageRepository : IRepository<Message>
{
    void Update(Message? message);

}
