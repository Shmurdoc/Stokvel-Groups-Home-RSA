using Stokvel_Groups_Home_RSA.Data;
using Stokvel_Groups_Home_RSA.Interface.IRepo.UserArea;
using Stokvel_Groups_Home_RSA.Models;

namespace Stokvel_Groups_Home_RSA.Repositories.UserArea
{
    public class GroupsRepository : Repository<Group>, IGroupsRepository
    {
        private readonly ApplicationDbContext _context;
        public GroupsRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(Group group)
        {
            _context.Groups.Update(group);
        }

        public bool GroupExists(string VerifyKey)
        {
            return _context.Groups.Any(x => x.VerifyKey == VerifyKey);
        }

    }
}
