//using Microsoft.AspNetCore.Mvc.ViewFeatures;
//using Microsoft.EntityFrameworkCore;
//using Moq;
//using Stokvel_Groups_Home_RSA.Controllers;
//using Stokvel_Groups_Home_RSA.Interface.IRepo;
//using Stokvel_Groups_Home_RSA.Interface.IServices.IAccountProfileServices;
//using Stokvel_Groups_Home_RSA.Interface.IServices.IAccountRequestService;
//using Stokvel_Groups_Home_RSA.Interface.IServices.IDepositRequestService;
//using Stokvel_Groups_Home_RSA.Interface.IServices.IGroupServices;
//using Stokvel_Groups_Home_RSA.Interface.IServices.IPreDepositRequestService;
//using Stokvel_Groups_Home_RSA.Interface.IServices.IWalletRepositoryService;
//using Stokvel_Groups_Home_RSA.Interface.IServices.IWithdrawServices;
//using Stokvel_Groups_Home_RSA.Models;
//using Stokvel_Groups_Home_RSA.Models.GroupedTables;
//using Stokvel_Groups_Home_RSA.Services.DepositRequestService.DepoChildClass;
//using Stokvel_Groups_Home_RSA.Services.DepositRequestService.DepositService;
//using Stokvel_Groups_Home_RSA.Services.DepositRequestService.DepositSet;
//using Stokvel_Groups_Home_RSA.Services.PreDepositRequestService.PreDepositInfo;
//using Stokvel_Groups_Home_RSA.Services.WalletRequestService.Wallet;
//using System.Linq.Expressions;
//using System.Threading.Tasks;
//using Xunit;

//namespace Stokvel_Groups_Home_RSA.UnitTesting.DepositTests.Sub_Test;

//public class DepositSubMethods
//{
//    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
//    private readonly Mock<IAccountProfileRequestServices> _mockAccountProfileRequestServices;
//    private readonly Mock<IGroupRequestServices> _mockGroupRequestServices;
//    private readonly Mock<IDepositService> _mockDepositService;
//    private readonly IDepositService _mockDepositServiceTest;
//    private readonly WalletInfo _walletInfo;
//    private readonly PreDepositInfo _mockPreDepositInfo;
//    private readonly Mock<ITempDataDictionary> _mockTempData;
//    private readonly DepositsController _controller;
//    private readonly Mock<IAccountRequestServices> _mockAccountRequestServices;
//    private readonly Mock<DbSet<PreDeposit>> _mockDbSet;
//    private readonly Mock<IDepositSet> _mockDepositSet;
//    private readonly Mock<IDepositRequestServices> _mockIDepositRequestServices;
//    private readonly WalletDepositRequest _mockWithdrawRequest;
//    private readonly Mock<IWalletRequestServices> _mockWithdrawRequestServices;

//    public DepositSubMethods()
//    {
//        _mockUnitOfWork = new Mock<IUnitOfWork>();
//        _mockAccountProfileRequestServices = new Mock<IAccountProfileRequestServices>();
//        _mockGroupRequestServices = new Mock<IGroupRequestServices>();
//        _walletInfo = new WalletInfo(_mockUnitOfWork.Object);
//        _mockPreDepositInfo = new PreDepositInfo(_mockUnitOfWork.Object, _mockGroupRequestServices.Object);
//        _mockDepositService = new Mock<IDepositService>();
//        _mockTempData = new Mock<ITempDataDictionary>();
//        _mockAccountRequestServices = new Mock<IAccountRequestServices>();
//        _mockDbSet = new Mock<DbSet<PreDeposit>>();
//        _mockDepositSet = new Mock<IDepositSet>();
//        _mockIDepositRequestServices = new Mock<IDepositRequestServices>();
//        _mockWithdrawRequestServices = new Mock<IWalletRequestServices>();
//        _mockWithdrawRequest = new WalletDepositRequest(_mockUnitOfWork.Object, _mockDepositSet.Object, _mockWithdrawRequestServices.Object, _mockGroupRequestServices.Object);

//        // Create the controller, passing the mocked dependencies
//        _mockDepositServiceTest = new DepositService(
//            _mockDepositSet.Object,
//            _mockUnitOfWork.Object,
//             _mockPreDepositInfo,
//             _walletInfo,
//            _mockAccountProfileRequestServices.Object,
//            _mockGroupRequestServices.Object
//        )
//        { };
//    }

//    [Fact]
//    public async Task ProcessDeposit_WithExcess_ShouldDepositToWallet()
//    {
//        // Arrange
//        var description = "test description";
//        decimal excess = 10;
//        int memberDescription = 0;
//        decimal memberDepoTarget = 1333;
//        string dropdownValue = "PayNow";

//        var deposit = new Deposit { DepositAmount = 343 };
//        var depositPreDepo = new Deposit { DepositId = 1, DepositAmount = 1000, DepositDate = DateTime.Now, DepositReference = "PreDeposit" };
//        var invoice = new Invoice { InvoiceId = 1, TotalAmount = 1000, InvoiceDate = DateTime.Now, Description = "PreDeposit" };

//        var accountId = 1;
//        var groupId = 1;
//        var userId = "user1";

//        string startDateString = "2024-11-01 00:00:00.0000000";
//        string endDateString = "2024-11-25 00:00:00.0000000";

//        var group = new Group
//        {
//            GroupId = groupId,
//            GroupName = "Test Group",
//            VerifyKey = "1234",
//            TotalGroupMembers = 4,
//            GroupDate = DateTime.Parse(startDateString),
//            GroupStatus = true,
//            AccountTarget = 4000
//        };

//        var wallet = new Wallet { WalletId = 1, Amount = 10, Id = userId };
//        var applicationUser = new ApplicationUser
//        {
//            Id = userId,
//            UserName = "Test User",
//            Email = "test@example.com",
//            Wallets = wallet
//        };

//        var account = new Account
//        {
//            AccountId = accountId,
//            Id = "XXXXXXXX",
//            GroupVerifyKey = "1234",
//            AccountQueue = 1,
//            AccountQueueStart = DateTime.Parse(startDateString),
//            AccountQueueEnd = DateTime.Parse(endDateString),
//            GroupId = groupId,
//            Group = group,
//            Accepted = true,
//            ApplicationUser = applicationUser,
//            Invoices = new List<Invoice> { new Invoice { InvoiceId = 1, AccountId = accountId, InvoiceDate = DateTime.Now, Description = "PreDeposit", TotalAmount = 1000, Deposit = new Deposit { DepositId = 1, DepositAmount = 1000, DepositReference = "PreDeposit", DepositDate = DateTime.Now } } }
//        };

//        var preDeposit = new PreDeposit
//        {
//            AccountId = accountId,
//            Amount = 1000,
//            PreDepositId = 1,
//            PreDepositDate = DateTime.Now,
//            Account = account
//        };

//        var depositToAccount = new DepositToAccount
//        {
//            Account = account,
//            PreDeposit = preDeposit,
//            Deposit = new List<Deposit?> { depositPreDepo },
//            Invoice = new List<Invoice?> { invoice }
//        };

//        var applicationUserWallet = new ApplicationUserWallet
//        {
//            Account = account,
//            ApplicationUser = applicationUser,
//            Wallet = wallet
//        };

//        var profile = new AccountProfile { StatusRank = MemberStatuses.Bronze };
//        var applicationUserAccountProfile = new ApplicationUserAccountProfile
//        {
//            AccountProfile = profile,
//            ApplicationUser = applicationUser
//        };

//        var mockDbSetPreDeposit = new Mock<DbSet<PreDeposit>>();
//        mockDbSetPreDeposit.As<IQueryable<PreDeposit>>().Setup(m => m.Provider).Returns(new List<PreDeposit> { depositToAccount.PreDeposit }.AsQueryable().Provider);
//        mockDbSetPreDeposit.As<IQueryable<PreDeposit>>().Setup(m => m.Expression).Returns(new List<PreDeposit> { depositToAccount.PreDeposit }.AsQueryable().Expression);
//        mockDbSetPreDeposit.As<IQueryable<PreDeposit>>().Setup(m => m.ElementType).Returns(new List<PreDeposit> { depositToAccount.PreDeposit }.AsQueryable().ElementType);
//        mockDbSetPreDeposit.As<IQueryable<PreDeposit>>().Setup(m => m.GetEnumerator()).Returns(new List<PreDeposit> { depositToAccount.PreDeposit }.AsQueryable().GetEnumerator());

//        _mockUnitOfWork.Setup(u => u.PreDepositRepository.GetList()).Returns(mockDbSetPreDeposit.Object);
//        _mockDepositService.Setup(p => p.WalletDepositRequestAsync(deposit, description, accountId, userId, dropdownValue)).Returns(Task.CompletedTask);
//        _mockUnitOfWork.Setup(u => u.AccountsRepository.GetAllAsync(It.IsAny<Expression<Func<Account, bool>>>(), It.IsAny<string>())).ReturnsAsync(new List<Account> { account });

//        var mockDbSetAccount = new Mock<DbSet<Account>>();
//        mockDbSetAccount.As<IQueryable<Account>>().Setup(m => m.Provider).Returns(new List<Account> { account }.AsQueryable().Provider);
//        mockDbSetAccount.As<IQueryable<Account>>().Setup(m => m.Expression).Returns(new List<Account> { account }.AsQueryable().Expression);
//        mockDbSetAccount.As<IQueryable<Account>>().Setup(m => m.ElementType).Returns(new List<Account> { account }.AsQueryable().ElementType);
//        mockDbSetAccount.As<IQueryable<Account>>().Setup(m => m.GetEnumerator()).Returns(new List<Account> { account }.AsQueryable().GetEnumerator());

//        _mockUnitOfWork.Setup(u => u.AccountsRepository.GetList()).Returns(mockDbSetAccount.Object);
//        _mockUnitOfWork.Setup(u => u.AccountsRepository.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(account);
//        _mockUnitOfWork.Setup(u => u.GroupsRepository.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(group);
//        _mockUnitOfWork.Setup(u => u.PreDepositRepository.GetAllAsync(It.IsAny<Expression<Func<PreDeposit, bool>>>(), It.IsAny<string>())).ReturnsAsync(new List<PreDeposit> { new PreDeposit { PreDepositId = 1 } });

//        _mockUnitOfWork.Setup(x => x.PreDepositRepository.GetAllAsync(It.IsAny<Expression<Func<PreDeposit, bool>>>(), It.IsAny<string>())).ReturnsAsync(new List<PreDeposit> { preDeposit });
//        _mockAccountProfileRequestServices.Setup(x => x.AccountProfileInfoAsync(userId)).ReturnsAsync(applicationUserAccountProfile);
//        _mockUnitOfWork.Setup(u => u.WalletRepository.Update(wallet));
//        _mockWithdrawRequestServices.Setup(x => x.WalletGetAmountAsync(accountId)).ReturnsAsync(applicationUserWallet);
//        _mockGroupRequestServices.Setup(x => x.CalculateAmountTarget(groupId)).ReturnsAsync(memberDepoTarget);
//        _mockIDepositRequestServices.Setup(x => x.DepositAsync(deposit, description, accountId, userId, dropdownValue)).Returns(Task.CompletedTask);

//        // Act
//        await _mockDepositServiceTest.ProcessDeposit(deposit, description, accountId, userId, excess, memberDescription, memberDepoTarget, dropdownValue);

//        // Assert
//        _mockUnitOfWork.Verify(w => w.WalletRepository.Update(It.IsAny<Wallet>()), Times.Once);
//    }


//    //[Fact]
//    //public async Task ProcessDeposit_WithNoExcess_ShouldPreDepositRequest()
//    //{
//    //    // Arrange
//    //    var deposit = new Deposit();
//    //    var description = "test description";
//    //    int accountId = 1;
//    //    string userId = "user1";
//    //    decimal excess = 0;
//    //    int memberDescription = 0;
//    //    decimal memberDepoTarget = 100;
//    //    string dropdownValue = "test dropdown";

//    //    _unitOfWorkMock.Setup(u => u.PreDepositRepository.GetAllAsync(It.IsAny<Func<PreDeposit, bool>>()))
//    //        .ReturnsAsync(new List<PreDeposit>());

//    //    _accountProfileRequestServicesMock.Setup(a => a.AccountProfileInfoAsync(userId))
//    //        .ReturnsAsync(new ProfileStatus { AccountProfile = new AccountProfile { StatusRank = MemberStatuses.Active } });

//    //    _preDepositInfoMock.Setup(p => p.PreDepoMembersAsync(accountId))
//    //        .ReturnsAsync(new MemberExpectedDepoAmount { PreDeposit = new PreDeposit { Amount = 50 } });

//    //    // Act
//    //    await _yourService.ProcessDeposit(deposit, description, accountId, userId, excess, memberDescription, memberDepoTarget, dropdownValue);

//    //    // Assert
//    //    _depositServiceMock.Verify(d => d.PreDepositRequestAsync(deposit, "PreDeposit", accountId, userId, dropdownValue), Times.Once);
//    //}
//}
