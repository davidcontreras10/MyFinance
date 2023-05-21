using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace EFDataAccess.Models
{
    public partial class TransferTrxDef
    {
        public Guid TransferTrxDefId { get; set; }
        public int ToAccountId { get; set; }

        public virtual Account ToAccount { get; set; }
        public virtual AutomaticTask TransferTrxDefNavigation { get; set; }
    }
}
