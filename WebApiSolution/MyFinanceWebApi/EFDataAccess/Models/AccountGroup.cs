using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace EFDataAccess.Models
{
    public partial class AccountGroup
    {
        public AccountGroup()
        {
            Account = new HashSet<Account>();
        }

        public int AccountGroupId { get; set; }
        public string AccountGroupName { get; set; }
        public string DisplayValue { get; set; }
        public int? AccountGroupPosition { get; set; }
        public bool? DisplayDefault { get; set; }
        public Guid? UserId { get; set; }

        public virtual AppUser User { get; set; }
        public virtual ICollection<Account> Account { get; set; }
    }
}
