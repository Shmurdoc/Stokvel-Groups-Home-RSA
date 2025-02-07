//using Moq;
//using Xunit;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.ViewFeatures;
//using Stokvel_Groups_Home_RSA.Interface.IRepo;
//using Stokvel_Groups_Home_RSA.Interface.IServices.IAccountProfileServices;
//using Stokvel_Groups_Home_RSA.Interface.IServices.IDepositRequestService;
//using Stokvel_Groups_Home_RSA.Interface.IServices.IGroupServices;
//using Stokvel_Groups_Home_RSA.Models;
//using Stokvel_Groups_Home_RSA.Controllers;
//using Stokvel_Groups_Home_RSA.Interface.IServices.IAccountRequestService;
//using Stokvel_Groups_Home_RSA.Interface.IServices.IWalletRepositoryService;
//using Stokvel_Groups_Home_RSA.Interface.IServices.IPreDepositRequestService;
//using System.Collections.Generic;
//using System.Linq;
//using System.Linq.Expressions;
//using System.Threading.Tasks;
//using Stokvel_Groups_Home_RSA.Models.GroupedTables;
//using Stokvel_Groups_Home_RSA.Services.WalletRequestService.Wallet;
//using Stokvel_Groups_Home_RSA.Services.PreDepositRequestService.PreDepositInfo;
//using Microsoft.EntityFrameworkCore;
//using System.Security.Principal;

//namespace Stokvel_Groups_Home_RSA.UnitTesting.DepositTests
//{
//    public class DepositControllerTests
//    {
//        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
//        private readonly Mock<IAccountProfileRequestServices> _mockAccountProfileRequestServices;
//        private readonly Mock<IGroupRequestServices> _mockGroupRequestServices;
//        private readonly Mock<IDepositService> _mockDepositService;
//        private readonly WalletInfo _walletInfo;
//        private readonly PreDepositInfo _mockPreDepositInfo;
//        private readonly Mock<ITempDataDictionary> _mockTempData;
//        private readonly DepositsController _controller;
//        private readonly Mock<IAccountRequestServices> _mockAccountRequestServices;
//        private readonly Mock<DbSet<PreDeposit>> _mockDbSet;


//        public DepositControllerTests()
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

//            // Create the controller, passing the mocked dependencies
//            _controller = new DepositsController(
//                _mockUnitOfWork.Object,
//                _mockDepositService.Object,
//                _mockAccountRequestServices.Object,
//                _walletInfo,
//                _mockPreDepositInfo,
//                _mockGroupRequestServices.Object,
//                _mockAccountProfileRequestServices.Object
//            )
//            {
//                TempData = _mockTempData.Object
//            };
//        }

//        //[Fact]
//        //public async Task Create_ShouldReturnView_WhenModelStateIsInvalid()
//        //{
//        //    // Arrange: Make ModelState invalid
//        //    _controller.ModelState.AddModelError("DepositAmount", "Invalid Deposit");
//        //    int accountId = 1;
//        //    int groupId = 1;
//        //    string startDateString = "2024-11-01 00:00:00.0000000";
//        //    string endDateString = "2024-11-25 00:00:00.0000000";
//        //    var account = new Account
//        //    {
//        //        AccountId = accountId,
//        //        Id = "XXXXXXXX",
//        //        GroupVerifyKey = "1234",
//        //        AccountQueue = 1,
//        //        AccountQueueStart = DateTime.Parse(startDateString),
//        //        AccountQueueEnd = DateTime.Parse(endDateString),
//        //        GroupId = groupId,
//        //        Accepted = true,
//        //        Invoices = new List<Invoice>
//        //    {
//        //        new Invoice {
//        //            InvoiceId = 1,
//        //            AccountId = accountId,
//        //            InvoiceDate = DateTime.Now,
//        //            Description = "PreDeposit",
//        //            TotalAmount = 1000,
//        //            Deposit = new Deposit { DepositId = 1, DepositAmount = 1000, DepositReference = "PreDeposit", DepositDate = DateTime.Now }
//        //        }
//        //    }
//        //    };
//        //    var deposit = new Deposit(); // Provide necessary Deposit data

//        //    var preDeposit = new PreDeposit { AccountId = accountId, Account = account };
//        //    var memberListPaid = new List<PreDeposit> { preDeposit }.AsQueryable();
            

//        //    // Act
//        //    var result = await _controller.Create(1, 1, deposit, "PayNow");

//        //    // Assert
//        //    var viewResult = Assert.IsType<ViewResult>(result);
//        //    Assert.Equal(deposit, viewResult.Model);
//        //}

//        [Fact]
//        public async Task Create_ShouldRedirectToIndex_WhenTargetMet()
//        {
//            // Arrange
//            var deposit = new Deposit { DepositAmount = 100 };
//            var depositPreDepo = new Deposit { DepositId = 1, DepositAmount = 1000, DepositDate = DateTime.Now, DepositReference = "PreDeposit" };
//            var invoice = new Invoice { InvoiceId = 1, TotalAmount = 1000, InvoiceDate = DateTime.Now, Description = "PreDeposit" };
//            var accountId = 1;
//            var groupId = 1;
//            var userId = "user1";
//            string startDateString = "2024-11-01 00:00:00.0000000";
//            string endDateString = "2024-11-25 00:00:00.0000000";

//            var account = new Account
//            {
//                AccountId = accountId,
//                Id = "XXXXXXXX",
//                GroupVerifyKey = "1234",
//                AccountQueue = 1,
//                AccountQueueStart = DateTime.Parse(startDateString),
//                AccountQueueEnd = DateTime.Parse(endDateString),
//                GroupId = groupId,
//                Accepted = true,
//                Invoices = new List<Invoice>
//            {
//                new Invoice {
//                    InvoiceId = 1,
//                    AccountId = accountId,
//                    InvoiceDate = DateTime.Now,
//                    Description = "PreDeposit",
//                    TotalAmount = 1000,
//                    Deposit = new Deposit { DepositId = 1, DepositAmount = 1000, DepositReference = "PreDeposit", DepositDate = DateTime.Now }
//                }
//            }
//            };
//            var preDeposit = new PreDeposit { AccountId = accountId, Amount = 1000, PreDepositId = 1, PreDepositDate = DateTime.Now, Account = account };
//            var depositToAccount = new DepositToAccount
//            {
//                Account = account,
//                PreDeposit = preDeposit,
//                Deposit = new List<Deposit?> { depositPreDepo },
//                Invoice = new List<Invoice?> { invoice }
//            };



//            // Mocking required methods
//            _mockTempData.Setup(t => t["accountId"]).Returns(accountId);
//            _mockTempData.Setup(t => t["groupId"]).Returns(groupId);

//            _mockAccountProfileRequestServices.Setup(a => a.AccountProfileInfoAsync(It.IsAny<string>())).ReturnsAsync(
//                new Models.GroupedTables.ApplicationUserAccountProfile { AccountProfile = new AccountProfile { StatusRank = MemberStatuses.Bronze } }
//            );
//            _mockUnitOfWork.Setup(u => u.GroupsRepository.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(new Group { Active = true, AccountTarget = 4000, VerifyKey = "1234" });
//            _mockUnitOfWork.Setup(u => u.AccountsRepository.GetAllAsync(It.IsAny<Expression<Func<Account, bool>>>(), It.IsAny<string>())).ReturnsAsync(new List<Account> { new Account { AccountId = accountId } });

//            _mockGroupRequestServices.Setup(g => g.CalculateAmountTarget(It.IsAny<int>())).ReturnsAsync(200);
//            _mockDepositService.Setup(d => d.DepositRequestAsync(It.IsAny<Deposit>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);

//            var mockDbSet = new Mock<DbSet<PreDeposit>>();
//            mockDbSet.As<IQueryable<PreDeposit>>().Setup(m => m.Provider).Returns(new List<PreDeposit> { depositToAccount.PreDeposit }.AsQueryable().Provider);
//            mockDbSet.As<IQueryable<PreDeposit>>().Setup(m => m.Expression).Returns(new List<PreDeposit> { depositToAccount.PreDeposit }.AsQueryable().Expression);
//            mockDbSet.As<IQueryable<PreDeposit>>().Setup(m => m.ElementType).Returns(new List<PreDeposit> { depositToAccount.PreDeposit }.AsQueryable().ElementType);
//            mockDbSet.As<IQueryable<PreDeposit>>().Setup(m => m.GetEnumerator()).Returns(new List<PreDeposit> { depositToAccount.PreDeposit }.AsQueryable().GetEnumerator());

//            _mockUnitOfWork.Setup(u => u.PreDepositRepository.GetList()).Returns(mockDbSet.Object);
//            _mockUnitOfWork.Setup(u => u.PreDepositRepository.GetAllAsync(It.IsAny<Expression<Func<PreDeposit, bool>>>(), It.IsAny<string>())).ReturnsAsync(new List<PreDeposit>{preDeposit});

//            // Act
//            var result = await _controller.Create(accountId, groupId, deposit, "PayNow");

//            // Assert
//            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
//            Assert.Equal("Deposits", redirectResult.ControllerName);

//        }

//        //[Fact]
//        //public async Task Create_ShouldReturnView_WhenTargetNotMet()
//        //{
//        //    // Arrange
//        //    var deposit = new Deposit { DepositAmount = 50 };
//        //    var accountId = 1;
//        //    var groupId = 1;
//        //    var userId = "user1";

//        //    // Mocking required methods
//        //    _mockTempData.Setup(t => t["accountId"]).Returns(accountId);
//        //    _mockTempData.Setup(t => t["groupId"]).Returns(groupId);
//        //    _mockAccountProfileRequestServices.Setup(a => a.AccountProfileInfoAsync(It.IsAny<string>())).ReturnsAsync(
//        //        new Models.GroupedTables.ApplicationUserAccountProfile { AccountProfile = new AccountProfile { StatusRank = MemberStatuses.Bronze } }
//        //    );
//        //    _mockUnitOfWork.Setup(u => u.GroupsRepository.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(new Group { Active = true, AccountTarget = 4000 });
//        //    _mockGroupRequestServices.Setup(g => g.CalculateAmountTarget(It.IsAny<int>())).ReturnsAsync(200);

//        //    // Mocking the account list for the group
//        //    var accountList = new List<Account>
//        //    {
//        //        new Account { AccountId = 1, GroupId = 1 },
//        //        new Account { AccountId = 2, GroupId = 1 }
//        //    };

//        //    // Set up the mock to return the list of accounts
//        //    _mockUnitOfWork.Setup(u => u.AccountsRepository.GetAllAsync(It.IsAny<Expression<Func<Account, bool>>>(), It.IsAny<string>()))
//        //        .ReturnsAsync(accountList);

//        //    // Act
//        //    var result = await _controller.Create(accountId, groupId, deposit, "PayNow");

//        //    // Assert
//        //    var viewResult = Assert.IsType<ViewResult>(result);
//        //    Assert.Equal(deposit, viewResult.Model);
//        //}


//    }
//}
