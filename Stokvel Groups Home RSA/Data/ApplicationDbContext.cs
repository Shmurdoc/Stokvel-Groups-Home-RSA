using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Stokvel_Groups_Home_RSA.Models;

namespace Stokvel_Groups_Home_RSA.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        protected ApplicationDbContext(DbContextOptions contextOptions)
        : base(contextOptions)
        {
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Message>()
                .HasOne<AppUser>(a => a.Sender)
                .WithMany(d => d.Messages)
                .HasForeignKey(d => d.UserID);

            /*//AdminAccountUser to Account One to Many
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Account>()
                .HasOne<AdminAccountUser>(au => au.AdminAccountUser)
                .WithMany(a => a.AdminAccounts)
                .HasForeignKey(a => a.Id)
                .OnDelete(DeleteBehavior.Cascade);*/

            //ApplicationUser to Account One to Many
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Account>()
                .HasOne<ApplicationUser>(au => au.ApplicationUser)
                .WithMany(a => a.UserAccounts)
                .HasForeignKey(a => a.Id)
                .OnDelete(DeleteBehavior.Cascade);

            //Account & PreDeposit one to one
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Account>()
            .HasOne(a => a.PreDeposit)
            .WithOne(x => x.Account)
            .HasForeignKey<PreDeposit>(a => a.AccountId);

            //AccountProfile & ApplicationUser one to one
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ApplicationUser>()
            .HasOne(a => a.AccountProfiles)
            .WithOne(x => x.ApplicationUser)
            .HasForeignKey<AccountProfile>(a => a.Id);



            //Account to Invoice One to Many
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Invoice>()
                .HasOne<Account>(a => a.Account)
                .WithMany(a => a.Invoices)
                .HasForeignKey(a => a.AccountId)
                .OnDelete(DeleteBehavior.Cascade);

            //Invoice to Deposit One to Many
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Invoice>()
                .HasOne<Deposit>(a => a.Deposit)
                .WithMany(a => a.Invoices)
                .HasForeignKey(a => a.DepositId)
                .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<PreDeposit>()
                .Property(p => p.Amount)
                .HasColumnType("decimal(18,4)");



            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Wallet>()
                .Property(a => a.Amount)
                .HasColumnType("decimal(18,4)");



            //Accounts to Groups one to Many
            modelBuilder.Entity<Account>()
                .HasOne<Group>(g => g.Group)
                .WithMany(a => a.Accounts)
                .HasForeignKey(a => a.GroupId);



            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<PenaltyFee>()
                .Property(pa => pa.PenaltyAmount)
                .HasColumnType("decimal(18,4)");

            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Invoice>()
                .Property(ta => ta.TotalAmount)
                .HasColumnType("decimal(18,4)");

            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Deposit>()
                .Property(d => d.DepositAmount)
                .HasColumnType("decimal(18,4)");

            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<AccountProfile>()
                .Property(tp => tp.TotalAmountDeposited)
                .HasColumnType("decimal(18,4)");

            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<AccountProfile>()
                .Property(tp => tp.TotalPenaltyFee)
                .HasColumnType("decimal(18,4)");

            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ApplicationUser>()
                .Property(p => p.Loans)
                .HasColumnType("decimal(18,4)");

            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ApplicationUser>()
                .Property(p => p.RentPayment)
                .HasColumnType("decimal(18,4)");

            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<WithdrawDetails>()
                .Property(p => p.CreditAmount)
                .HasColumnType("decimal(18,4)");

            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<PenaltyFee>()
                .Property(p => p.PenaltyAmount)
                .HasColumnType("decimal(18,4)");

        }

        
        public virtual DbSet<ApplicationUser>? ApplicationUsers { get; set; }
        public virtual DbSet<AccountProfile>? AccountProfiles { get; set; }
        /*public virtual DbSet<AccountUserPersonal>? AccountUserPersonals { get; set; }*/

        public virtual DbSet<Group>? Groups { get; set; }
        public virtual DbSet<Account>? Accounts { get; set; }
        public virtual DbSet<Calendar>? Calendar { get; set; }
        public virtual DbSet<Invoice>? Invoices { get; set; }
        //public virtual DbSet<MemberInvoice>? MemberInvoices { get; set; }
        public virtual DbSet<WithdrawDetails>? WithdrawDetails { get; set; }
        //public virtual DbSet<PenaltyFee>? PenaltyFees { get; set; }
        public virtual DbSet<Deposit>? Deposits { get; set; }
        public virtual DbSet<PreDeposit>? PreDeposits { get; set; }
        //public virtual DbSet<DepositLog>? DepositLog { get; set; }
        //public virtual DbSet<BankDetails>? BankDetails { get; set; }
        //public virtual DbSet<Wallet>? Wallets { get; set; }
        //public virtual DbSet<ApplicationUser>? ApplicationUser { get; set; }
        public virtual DbSet<Message>? Messages { get; set; }
        //public virtual DbSet<DepositLog>? DepositLog { get; set; }
        //public virtual DbSet<BankDetails>? BankDetails { get; set; }
        //public virtual DbSet<Wallet>? Wallets { get; set; }
        //public virtual DbSet<ApplicationUser>? ApplicationUser { get; set; }
        public DbSet<Stokvel_Groups_Home_RSA.Models.PenaltyFee>? PenaltyFee { get; set; }

    }


}
