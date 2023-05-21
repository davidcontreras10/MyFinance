using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace EFDataAccess.Models
{
    public partial class Spend
    {
        public Spend()
        {
            SpendOnPeriod = new HashSet<SpendOnPeriod>();
        }

        public int SpendId { get; set; }
        public double? OriginalAmount { get; set; }
        public DateTime? SpendDate { get; set; }
        public int? SpendTypeId { get; set; }
        public string Description { get; set; }
        public int? AmountCurrencyId { get; set; }
        public int AmountTypeId { get; set; }
        public double? Numerator { get; set; }
        public double? Denominator { get; set; }
        public bool IsPending { get; set; }
        public DateTime? SetPaymentDate { get; set; }

        public virtual Currency AmountCurrency { get; set; }
        public virtual AmountType AmountType { get; set; }
        public virtual SpendType SpendType { get; set; }
        public virtual LoanRecord LoanRecord { get; set; }
        public virtual ICollection<SpendOnPeriod> SpendOnPeriod { get; set; }
    }
}
