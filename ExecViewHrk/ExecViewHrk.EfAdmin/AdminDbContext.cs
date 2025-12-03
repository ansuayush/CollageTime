namespace ExecViewHrk.EfAdmin
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class AdminDbContext : DbContext
    {
        public AdminDbContext()
            : base("name=AdminDbContext")
        {
            this.Configuration.LazyLoadingEnabled = false;
        }

        public virtual DbSet<C__MigrationHistory> C__MigrationHistory { get; set; }
        public virtual DbSet<AspNetRole> AspNetRoles { get; set; }
        public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }
        public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }
        public virtual DbSet<AspNetUser> AspNetUsers { get; set; }
        public virtual DbSet<Employer> Employers { get; set; }
        public virtual DbSet<ErrorLog> ErrorLogs { get; set; }
        public virtual DbSet<ExternalUserClient> ExternalUserClients { get; set; }
        public virtual DbSet<MessageLog> MessageLogs { get; set; }
        public virtual DbSet<UserCompany> UserCompanies { get; set; }
        public virtual DbSet<ClientConfiguration> ClientConfigurations { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AspNetRole>()
                .HasMany(e => e.AspNetUsers)
                .WithMany(e => e.AspNetRoles)
                .Map(m => m.ToTable("AspNetUserRoles").MapLeftKey("RoleId").MapRightKey("UserId"));

            modelBuilder.Entity<AspNetUser>()
                .HasMany(e => e.AspNetUserClaims)
                .WithRequired(e => e.AspNetUser)
                .HasForeignKey(e => e.UserId);

            modelBuilder.Entity<AspNetUser>()
                .HasMany(e => e.AspNetUserLogins)
                .WithRequired(e => e.AspNetUser)
                .HasForeignKey(e => e.UserId);

            modelBuilder.Entity<AspNetUser>()
                .HasMany(e => e.ExternalUserClients)
                .WithRequired(e => e.AspNetUser)
                .HasForeignKey(e => e.UserId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<AspNetUser>()
                .HasMany(e => e.UserCompanies)
                .WithRequired(e => e.AspNetUser)
                .HasForeignKey(e => e.UserId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<AspNetUser>()
                .HasMany(e => e.Employers)
                .WithMany(e => e.AspNetUsers1)
                .Map(m => m.ToTable("UserClients").MapLeftKey("UserId").MapRightKey("EmployerClientId"));

            modelBuilder.Entity<Employer>()
                .Property(e => e.EmployerName)
                .IsUnicode(false);

            modelBuilder.Entity<Employer>()
                .Property(e => e.DatabaseName)
                .IsUnicode(false);

            modelBuilder.Entity<Employer>()
                .Property(e => e.CreatedBy)
                .IsUnicode(false);

            modelBuilder.Entity<Employer>()
                .Property(e => e.ModifiedBy)
                .IsUnicode(false);

            modelBuilder.Entity<Employer>()
                .HasMany(e => e.AspNetUsers)
                .WithRequired(e => e.Employer)
                .HasForeignKey(e => e.EmployerId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Employer>()
               .HasMany(e => e.ClientConfigurations)
               .WithRequired(e => e.Employer)
               .WillCascadeOnDelete(false);

            modelBuilder.Entity<Employer>()
                .HasMany(e => e.ExternalUserClients)
                .WithRequired(e => e.Employer)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Employer>()
                .HasMany(e => e.UserCompanies)
                .WithRequired(e => e.Employer)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ErrorLog>()
                .Property(e => e.UserName)
                .IsUnicode(false);

            modelBuilder.Entity<ErrorLog>()
                .Property(e => e.ExceptionMessage)
                .IsUnicode(false);

            modelBuilder.Entity<ErrorLog>()
                .Property(e => e.ExceptionStackTrace)
                .IsUnicode(false);

            modelBuilder.Entity<ErrorLog>()
                .Property(e => e.ExceptionTargetSite)
                .IsUnicode(false);

            modelBuilder.Entity<ErrorLog>()
                .Property(e => e.ExceptionSource)
                .IsUnicode(false);

            modelBuilder.Entity<ErrorLog>()
                .Property(e => e.RequestUrl)
                .IsUnicode(false);

            modelBuilder.Entity<ErrorLog>()
                .Property(e => e.RequestUrlReferrer)
                .IsUnicode(false);

            modelBuilder.Entity<ErrorLog>()
                .Property(e => e.RequestUserAgent)
                .IsUnicode(false);

            modelBuilder.Entity<ErrorLog>()
                .Property(e => e.RequestPhysicalPath)
                .IsUnicode(false);

            modelBuilder.Entity<ErrorLog>()
                .Property(e => e.ServerVariablesRemoteAddress)
                .IsUnicode(false);

            modelBuilder.Entity<ErrorLog>()
                .Property(e => e.Browser)
                .IsUnicode(false);

            modelBuilder.Entity<MessageLog>()
                .Property(e => e.Category)
                .IsUnicode(false);

            modelBuilder.Entity<MessageLog>()
                .Property(e => e.Message)
                .IsUnicode(false);

            modelBuilder.Entity<MessageLog>()
                .Property(e => e.Source)
                .IsUnicode(false);

            modelBuilder.Entity<ClientConfiguration>()
                .Property(e => e.ConfigurationName)
                .IsUnicode(false);
        }
    }
}
