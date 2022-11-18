using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyFinanceWebApp.Models
{
    public enum ScheduleTaskRequestType
    {
        Unknown = 0,
        View = 1,
        New = 2
    }

    public class ScheduledTasksViewModel : BaseModel
    {
        public ScheduleTaskRequestType RequestType { get; set; }
    }
}