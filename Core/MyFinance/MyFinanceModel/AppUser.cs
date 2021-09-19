using System;

namespace MyFinance.MyFinanceModel
{
    public class AppUser
    {
        #region Attributes 

        public string Username { set; get; }
        public string Name { set; get; }
        public Guid UserId { get; set; }
        public string PrimaryEmail { get; set; }

        #endregion
    }
}
