using PagedList;
using Stokvel_Groups_Home_RSA.Interface.IRepo;
using Stokvel_Groups_Home_RSA.Interface.IServices.IGroupServices;
using Stokvel_Groups_Home_RSA.Models;

namespace Stokvel_Groups_Home_RSA.Services.GroupServices
{
    public class GroupRequestServices : IGroupRequestServices
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IGroupServices _groupServices;

        public GroupRequestServices(IGroupServices groupServices, IUnitOfWork unitOfWork)
        {
            _groupServices = groupServices;
            _unitOfWork = unitOfWork;
        }




        public async Task<decimal> CalculateAmountTarget(int groupId)
        {
            var group = await _unitOfWork.GroupsRepository.GetByIdAsync(groupId);
            if (group == null || group.TotalGroupMembers <= 1)
            {
                throw new InvalidOperationException("Invalid group or insufficient group members.");
            }

            var memberPreDepoTarget = group.AccountTarget / (group.TotalGroupMembers - 1);
            return memberPreDepoTarget;
        }


        public bool CheckIfGroupExists(string verifyKey)
        {
            return _unitOfWork.GroupsRepository.GroupExists(verifyKey);
        }


        public Task<IPagedList<Group>> FilterAccountUsers(string? sortOrder, string? currentFilter, string? searchString, int? page) => _groupServices.FilterAccountUsers(sortOrder, currentFilter, searchString, page);
    }
}
