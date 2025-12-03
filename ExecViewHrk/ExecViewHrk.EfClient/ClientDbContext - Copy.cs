namespace ExecViewHrk.EfClient
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class ClientDbContext : DbContext
    {
        public ClientDbContext()
            : base("name=ClientDbContex")
        {
            this.Configuration.LazyLoadingEnabled = false;
        }

        public ClientDbContext(string connString) : base(connString) 
        {
            this.Configuration.LazyLoadingEnabled = false;
        }

        public virtual DbSet<ddlActualMaritalStatus> ddlActualMaritalStatuses { get; set; }
        public virtual DbSet<DdlGender> DdlGenders { get; set; }
        public virtual DbSet<ddlPrefix> ddlPrefixes { get; set; }
        public virtual DbSet<ddlSuffix> ddlSuffixes { get; set; }
        public virtual DbSet<Person> Persons { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ddlActualMaritalStatus>()
                .Property(e => e.description)
                .IsUnicode(false);

            modelBuilder.Entity<ddlActualMaritalStatus>()
                .Property(e => e.code)
                .IsUnicode(false);

            modelBuilder.Entity<ddlActualMaritalStatus>()
                .HasMany(e => e.Persons)
                .WithOptional(e => e.ddlActualMaritalStatus)
                .HasForeignKey(e => e.ActualMaritalStatusId);

            modelBuilder.Entity<DdlGender>()
                .Property(e => e.GenderCode)
                .IsUnicode(false);

            modelBuilder.Entity<DdlGender>()
                .Property(e => e.GenderDescription)
                .IsUnicode(false);

            modelBuilder.Entity<ddlPrefix>()
                .Property(e => e.Description)
                .IsUnicode(false);

            modelBuilder.Entity<ddlPrefix>()
                .Property(e => e.Code)
                .IsUnicode(false);

            modelBuilder.Entity<ddlSuffix>()
                .Property(e => e.Description)
                .IsUnicode(false);

            modelBuilder.Entity<ddlSuffix>()
                .Property(e => e.Code)
                .IsUnicode(false);

            modelBuilder.Entity<Person>()
                .Property(e => e.SSN)
                .IsUnicode(false);

            modelBuilder.Entity<Person>()
                .Property(e => e.Lastname)
                .IsUnicode(false);

            modelBuilder.Entity<Person>()
                .Property(e => e.Firstname)
                .IsUnicode(false);

            modelBuilder.Entity<Person>()
                .Property(e => e.MiddleName)
                .IsUnicode(false);

            modelBuilder.Entity<Person>()
                .Property(e => e.PreferredName)
                .IsUnicode(false);

            modelBuilder.Entity<Person>()
                .Property(e => e.eMail)
                .IsUnicode(false);

            modelBuilder.Entity<Person>()
                .Property(e => e.AlternateEMail)
                .IsUnicode(false);

            modelBuilder.Entity<Person>()
                .Property(e => e.MaidenName)
                .IsUnicode(false);

            modelBuilder.Entity<Person>()
                .Property(e => e.EnteredBy)
                .IsUnicode(false);
        }
    }
}
