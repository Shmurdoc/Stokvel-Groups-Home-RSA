//using Moq;
//using Stokvel_Groups_Home_RSA.Models.GroupedTables;
//using Stokvel_Groups_Home_RSA.Models;
//using Xunit;
//using Microsoft.AspNetCore.Mvc.ViewFeatures;
//using Microsoft.EntityFrameworkCore;
//using Stokvel_Groups_Home_RSA.Controllers;
//using Stokvel_Groups_Home_RSA.Interface.IRepo;
//using Stokvel_Groups_Home_RSA.Interface.IServices.IAccountProfileServices;
//using Stokvel_Groups_Home_RSA.Interface.IServices.IAccountRequestService;
//using Stokvel_Groups_Home_RSA.Interface.IServices.IDepositRequestService;
//using Stokvel_Groups_Home_RSA.Interface.IServices.IGroupServices;
//using Stokvel_Groups_Home_RSA.Interface.IServices.IWalletRepositoryService;
//using Stokvel_Groups_Home_RSA.Services.DepositRequestService.DepoChildClass;
//using Stokvel_Groups_Home_RSA.Services.PreDepositRequestService.PreDepositInfo;
//using Stokvel_Groups_Home_RSA.Services.WalletRequestService.Wallet;
//using System.Linq.Expressions;
//using System.Security.Principal;

//namespace Stokvel_Groups_Home_RSA.UnitTesting.PenaltyFee_BackGround
//{
//    public class PenaltyFeeCheckTest
//    {

//        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
//        private readonly Mock<IAccountProfileRequestServices> _mockAccountProfileRequestServices;
//        private readonly Mock<IGroupRequestServices> _mockGroupRequestServices;
//        private readonly Mock<IDepositService> _mockDepositService;
//        private readonly IDepositService _mockDepositServiceTest;
//        private readonly WalletInfo _walletInfo;
//        private readonly PreDepositInfo _mockPreDepositInfo;
//        private readonly Mock<ITempDataDictionary> _mockTempData;
//        private readonly DepositsController _controller;
//        private readonly Mock<IAccountRequestServices> _mockAccountRequestServices;
//        private readonly Mock<DbSet<PreDeposit>> _mockDbSet;
//        private readonly Mock<IDepositSet> _mockDepositSet;
//        private readonly Mock<IDepositRequestServices> _mockIDepositRequestServices;
//        private readonly WalletDepositRequest _mockWithdrawRequest;
//        private readonly Mock<IWalletRequestServices> _mockWithdrawRequestServices;


//       public PenaltyFeeCheckTest()
//        {
//            _mockUnitOfWork = new Mock<IUnitOfWork>();
//            _mockAccountProfileRequestServices = new Mock<IAccountProfileRequestServices>();
//            _mockGroupRequestServices = new Mock<IGroupRequestServices>();
//            _walletInfo = new WalletInfo(_mockUnitOfWork.Object);
//            _mockPreDepositInfo = new PreDepositInfo(_mockUnitOfWork.Object, _mockGroupRequestServices.Object);
//            _mockDepositService = new Mock<IDepositService>();
//            _mockTempData = new Mock<ITempDataDictionary>();
//            _mockAccountRequestServices = new Mock<IAccountRequestServices>();
//            _mockDbSet = new Mock<DbSet<PreDeposit>>();
//            _mockDepositSet = new Mock<IDepositSet>();
//            _mockIDepositRequestServices = new Mock<IDepositRequestServices>();
//            _mockWithdrawRequestServices = new Mock<IWalletRequestServices>();
//            _mockWithdrawRequest = new WalletDepositRequest(_mockUnitOfWork.Object, _mockDepositSet.Object, _mockWithdrawRequestServices.Object, _mockGroupRequestServices.Object);
//        }

//        [Fact]
//        public async Task ApplyPenaltiesAsync_ShouldApplyPenaltiesToOverdueMembers()
//        {
//            // Arrange
//            var groupIdList = new List<int> { 1, 2 };
//            var accountList = new List<Account>
//    {
//        new Account { AccountId = 1, GroupId = 1 },
//        new Account { AccountId = 2, GroupId = 1 },
//        new Account { AccountId = 3, GroupId = 2 }
//    };

//            var deposits = new List<Deposit>
//    {
//        new Deposit { AccountId = 1, Amount = 500 },
//        new Deposit { AccountId = 2, Amount = 200 },
//        new Deposit { AccountId = 3, Amount = 400 }
//    };

//            var currentMonthAccounts = new List<AccountInvoice>
//    {
//        new AccountInvoice { AccountId = 1, Invoices = new List<Invoice> { new Invoice { TotalAmount = 500 } } },
//        new AccountInvoice { AccountId = 2, Invoices = new List<Invoice> { new Invoice { TotalAmount = 200 } } },
//        new AccountInvoice { AccountId = 3, Invoices = new List<Invoice> { new Invoice { TotalAmount = 400 } } }
//    };

//            var invoicesForGroup = new List<Invoice>
//    {
//        new Invoice { TotalAmount = 500 },
//        new Invoice { TotalAmount = 200 }
//    };

//            var outstandingAmount = 700;
//            var groupTargetAmount = 1000; // Example group target amount

//            // Mocking dependencies



//            var mockDbSetPreDeposit = new Mock<DbSet<Group>>();
//            mockDbSetPreDeposit.As<IQueryable<Group>>().Setup(m => m.Provider).Returns(new List<Group> { depositToAccount.PreDeposit }.AsQueryable().Provider);
//            mockDbSetPreDeposit.As<IQueryable<Group>>().Setup(m => m.Expression).Returns(new List<Group> { depositToAccount.PreDeposit }.AsQueryable().Expression);
//            mockDbSetPreDeposit.As<IQueryable<Group>>().Setup(m => m.ElementType).Returns(new List<Group> { depositToAccount.PreDeposit }.AsQueryable().ElementType);
//            mockDbSetPreDeposit.As<IQueryable<Group>>().Setup(m => m.GetEnumerator()).Returns(new List<Group> { depositToAccount.PreDeposit }.AsQueryable().GetEnumerator());

//            _mockUnitOfWork.Setup(u => u.PreDepositRepository.GetList()).Returns(mockDbSetPreDeposit.Object);
//            _mockDepositService.Setup(p => p.WalletDepositRequestAsync(deposit, description, accountId, userId, dropdownValue)).Returns(Task.CompletedTask);


//            _mockUnitOfWork.Setup(u => u.AccountsRepository.GetAllAsync(It.IsAny<Expression<Func<Account, bool>>>(), It.IsAny<string>())).ReturnsAsync(new List<Account> { account });
//            _mockGroupRequestServices.Setup(p => p.CalculateAmountTarget(It.IsAny<int>())).ReturnsAsync(groupTargetAmount);
//            _mockWithdrawRequestServices.Setup(p => p.)



//            _mockUnitOfWork.Setup(u => u.GroupsRepository.GetList()).ReturnsAsync(groupIdList);
//            _mockUnitOfWork.Setup(u => u.AccountsRepository.GetAllAsync()).ReturnsAsync(accountList);
//            _mockGroupRequestServices.Setup(s => s.CalculateAmountTarget(It.IsAny<int>())).ReturnsAsync(groupTargetAmount);
//            _mockWithdrawServices.Setup(w => w.PenaltiesAsync(It.IsAny<List<Account>>(), It.IsAny<decimal>())).Returns(Task.CompletedTask);

//            _mockService.Setup(s => s.GetDeposits(It.IsAny<int>())).ReturnsAsync(deposits);
//            _mockService.Setup(s => s.GetCurrentMonthAccounts(It.IsAny<List<Deposit>>())).ReturnsAsync(currentMonthAccounts);
//            _mockService.Setup(s => s.GetInvoicesForGroup(It.IsAny<List<Deposit>>(), It.IsAny<List<AccountInvoice>>())).ReturnsAsync(invoicesForGroup);
//            _mockService.Setup(s => s.GetOutstandingAmount(It.IsAny<List<Invoice>>(), It.IsAny<int>())).ReturnsAsync(outstandingAmount);

//            // Act
//            await _yourClassUnderTest.ApplyPenaltiesAsync();

//            // Assert
//            _mockWithdrawServices.Verify(w => w.PenaltiesAsync(It.Is<List<Account>>(a => a.Count == 1 && a[0].AccountId == 2), It.Is<decimal>(gt => gt == groupTargetAmount)), Times.Once);
//        }

//    }
//}
