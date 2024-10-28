using Stokvel_Groups_Home_RSA.Interface.Infrastructure;
using Stokvel_Groups_Home_RSA.Models;

namespace Stokvel_Groups_Home_RSA.Interface.IRepo.UserArea
{
    public interface IGroupsRepository : IRepository<Group>
    {
        void Update(Group group);
        bool GroupExists(string VerifyKey);
    }
}
