using PagedList;
using Stokvel_Groups_Home_RSA.Interface.IRepo;
using Stokvel_Groups_Home_RSA.Interface.IServices.IGroupServices;
using Stokvel_Groups_Home_RSA.Models;

namespace Stokvel_Groups_Home_RSA.Services.GroupServices
{
    public class GroupRequestServices : IGroupRequestServices
    {

        private readonly IUnitOfWork _unitOfWork;

        public GroupRequestServices(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }




        public async Task<decimal> CalculateAmountTarget(int groupId)
        {
            var group = await _unitOfWork.GroupsRepository.GetByIdAsync(groupId);

            if (group == null )
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

        public async Task<PagedList.IPagedList<Group>> FilterAccountUsers(string? sortOrder, string? currentFilter, string? searchString, int? page)
        {
            var groups = from g in await _unitOfWork.GroupsRepository.GetAllAsync()
                         select g;
            if (!String.IsNullOrEmpty(searchString))
            {
                groups = groups.Where(g => g.GroupName.Contains(searchString)
                                       || g.VerifyKey.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "name_desc":
                    groups = groups.OrderByDescending(g => g.TypeAccount);
                    break;
                case "Date":
                    groups = groups.OrderBy(g => g.TypeAccount);
                    break;
                case "date_desc":
                    groups = groups.OrderByDescending(g => g.GroupDate);
                    break;
                default:
                    groups = groups.OrderBy(g => g.GroupDate);
                    break;
            }


            int pageSize = 3;
            int pageNumber = (page ?? 1);
            PagedList.IPagedList<Group> resultList = groups.ToPagedList(pageNumber, pageSize);
            return resultList;
        }

    }
}
