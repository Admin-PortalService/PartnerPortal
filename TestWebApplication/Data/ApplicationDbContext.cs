using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TestWebApplication.Models;
using TestWebApplication.Models.ViewModels;
namespace TestWebApplication.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            Guid roleGuid = new Guid("363432dc-f55d-43bd-9f8c-8f764817319e"); //Guid.NewGuid();
            builder.Entity<Roles>().HasData(
                new Roles()
                {
                    RoleID = roleGuid,
                    RoleDesc = "SuperAdmin",
                    RoleType = "SUPERADMIN",
                    CreatedOn = new DateTime(2023, 6, 1, 0, 0, 0, 0, DateTimeKind.Local).AddTicks(0),
                    CreatedBy = "System",
                    ModifiedOn = new DateTime(2023, 6, 1, 0, 0, 0, 0, DateTimeKind.Local).AddTicks(0),
                    ModifiedBy = "System",
                    IsActive = true,
                    IsInternal = true,
                },
                new Roles()
                {
                    RoleID = new Guid("15391037-debe-4dab-b8b8-4cd14e1efe3b"),
                    RoleDesc = "Administrator",
                    RoleType = "ADMINISTRATOR",
                    CreatedOn = new DateTime(2023, 6, 1, 0, 0, 0, 0, DateTimeKind.Local).AddTicks(0),
                    CreatedBy = "System",
                    ModifiedOn = new DateTime(2023, 6, 1, 0, 0, 0, 0, DateTimeKind.Local).AddTicks(0),
                    ModifiedBy = "System",
                    IsActive = true,
                    IsInternal = true,
                }
                );



            Guid userGuid = new Guid("c44b011b-a25a-436b-bd87-ba600c942df3");

            var appUser = new IdentityUser()
            {
                Id = userGuid + string.Empty,
                UserName = "superadmin@gmail.com",
                NormalizedUserName = "SUPERADMIN@GMAIL.COM",
                Email = "superadmin@gmail.com",
                NormalizedEmail = "SUPERADMIN@GMAIL.COM",
                EmailConfirmed = true,
                PhoneNumberConfirmed = false,
                TwoFactorEnabled = false,
                LockoutEnabled = true,
                AccessFailedCount = 0,
                PasswordHash = "AQAAAAEAACcQAAAAEKou4krBVEylwJ8+x1rw/Rfiq+YL/w2kE4NJ6+8WI0+gMhmDh2Vosu0wLVZ53NzZew==",
                SecurityStamp = "b1804040-602f-4d2b-8794-269fdacf7874",
                ConcurrencyStamp = "561ad666-24fe-4ba1-9db6-e3ee4c03dc43"
            };



            //PasswordHasher<IdentityUser> ph = new PasswordHasher<IdentityUser>();
            //appUser.PasswordHash = ph.HashPassword(appUser, "P@ssw0rd1");



            builder.Entity<IdentityUser>().HasData(appUser);



            builder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string>()
            {
                RoleId = roleGuid + string.Empty,
                UserId = userGuid + string.Empty,
            });



            base.OnModelCreating(builder);
        }
        public DbSet <Projects> Project
            { get; set; }
        public DbSet<Incident> Incident
        { get; set; }
        public DbSet<Attachment> Attachment
        { get; set; }
        public DbSet<Module> Module
        { get; set; }

        public DbSet<Roles> Roles
        { get; set; }

        public DbSet<UserAccess> UserAccess
        { get; set; }
        public DbSet<IssueType> IssueType
        { get; set; }

        public DbSet<Customer> Customer
        { get; set; }

        public DbSet<Comment> Comment
        { get; set; }

        public DbSet<Users> Users
        { get; set; }

        public DbSet<RolePermission> RolePermission
        { get; set; }

        public DbSet<Assign> Assign
        { get; set; }

        public DbSet<Solution> Solution
        { get; set; }

        public DbSet<Severity> Severity
        { get; set; }
        
        public DbSet<AssignLog> AssignLog
        { get; set; }

        public DbSet<MaintenanceLog> MaintenanceLog
        { get; set; }

        public DbSet<Reseller> Reseller
        { get; set; }

        public DbSet<ImplementationAssist> ImplementationAssist
        { get; set; }

        public DbSet<Sale> Sale
        { get; set; }

        public DbSet<ProductVersion> ProductVersion
        { get; set; }

        public DbSet<Campus> Campus
        { get; set; }

        public DbSet<Price> Price
        { get; set; }

        public DbSet<UserProfile> UserProfile
        { get; set; }
        
        public DbSet<Statement> Statement
        { get; set; }
        public DbSet<Item> Item
        { get; set; }
        public DbSet<item_statement> item_statement
        { get; set; }
        public DbSet<Quotation> Quotation
        { get; set; }
        public DbSet<ItemQuotation> ItemQuotation
        { get; set; }
        public DbSet<MeetingNote> MeetingNote
        { get; set; }
        public DbSet<InternalNote> InternalNote
        {
            get; set;
        }
        public DbSet<Contract> Contract
        {
            get; set;
        }
        public DbSet<User_Customer> User_Customer
        { 
            get; set; 
        }
        public DbSet<SLAConfig> SLAConfig
        {
            get; set;
        }
        public DbSet<IncidentHistory> IncidentHistory
        {
            get; set;
        }
    }

}