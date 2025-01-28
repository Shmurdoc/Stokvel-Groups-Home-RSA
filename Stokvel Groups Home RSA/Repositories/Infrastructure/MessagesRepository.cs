using Stokvel_Groups_Home_RSA.Data;
using Stokvel_Groups_Home_RSA.Interface.Messages;
using Stokvel_Groups_Home_RSA.Models;


namespace Stokvel_Groups_Home_RSA.Repositories;

public class MessagesRepository : Repository<Message>, IMessageRepository
{

		private readonly ApplicationDbContext _context;

    public MessagesRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public void Update(Message? message)
    {
        _context.Messages.Update(message);
    }

}
