using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace EFDataAccess.Models
{
    public partial class SpInTrxDef
    {
        public Guid SpInTrxDefId { get; set; }
        public bool IsSpendTrx { get; set; }

        public virtual AutomaticTask SpInTrxDefNavigation { get; set; }
    }
}
