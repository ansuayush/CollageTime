namespace ExecViewHrk.EfClient
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("SelfOnboardingBankAccounts")]
    public partial class SelfOnboardingBankAccount
    {
        [Key]
        public int BankAccountId { get; set; }

        public int HireId { get; set; }

        public int? AccountTypeId { get; set; }

        [StringLength(150)]
        public string BankName { get; set; }

        [StringLength(50)]
        public string RoutingNumber { get; set; }

        [StringLength(50)]
        public string AccountNumber { get; set; }

        public bool IsPrimary { get; set; }

        public virtual SelfOnboardingHire SelfOnboardingHire { get; set; }
    }
}
