using System;

namespace MyFinance.MyFinanceModel
{
    public class BaseCustomToken
    {
        public DateTime DateTimeIssued { get; set; }
        public TimeSpan ExpireTime { get; set; }

        public bool HasExpired()
        {
            return DateTime.Now > DateTimeIssued.Add(ExpireTime);
        }
    }

    public class UserToken : BaseCustomToken
    {
        public string UserId { get; set; }
    }
}
