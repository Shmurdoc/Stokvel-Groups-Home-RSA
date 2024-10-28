using Stokvel_Groups_Home_RSA.Models;

namespace Stokvel_Groups_Home_RSA.Interface.IRepo.Finance
{
    public interface IPaymentLogsRepository
    {


        void Update(DepositLog? paymentLog);



    }
}
