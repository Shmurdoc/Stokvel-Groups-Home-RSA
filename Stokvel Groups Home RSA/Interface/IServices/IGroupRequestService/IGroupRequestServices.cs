

using Stokvel_Groups_Home_RSA.Models;

namespace Stokvel_Groups_Home_RSA.Interface.IServices.IGroupServices
{
    public interface IGroupRequestServices
    {
        bool CheckIfGroupExists(string verifyKey);
        Task<decimal> CalculateAmountTarget(int groupId);
        Task<PagedList.IPagedList<Group>> FilterAccountUsers(string? sortOrder, string? currentFilter, string? searchString, int? page);
    }
}
