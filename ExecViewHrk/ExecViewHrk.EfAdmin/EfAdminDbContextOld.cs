namespace ExecViewHrk.EfAdmin
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class EfAdminDbContext : DbContext
    {
        public EfAdminDbContext()
            : base("name=EvHrkDbContext")
        {
            this.Configuration.LazyLoadingEnabled = false;
        }

        public virtual DbSet<C__MigrationHistory> C__MigrationHistory { get; set; }
        public virtual DbSet<AspNetRole> AspNetRoles { get; set; }
        public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }
        public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }
        public virtual DbSet<AspNetUser> AspNetUsers { get; set; }
        public virtual DbSet<Client> Clients { get; set; }
        public virtual DbSet<ErrorLog> ErrorLogs { get; set; }
        public virtual DbSet<MessageLog> MessageLogs { get; set; }

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
                .HasMany(e => e.Clients)
                .WithMany(e => e.AspNetUsers)
                .Map(m => m.ToTable("UserClients").MapLeftKey("UserId").MapRightKey("ClientId"));

            modelBuilder.Entity<Client>()
                .Property(e => e.ClientnName)
                .IsUnicode(false);

            modelBuilder.Entity<Client>()
                .Property(e => e.ConnectString)
                .IsUnicode(false);

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
        }
    }
}
