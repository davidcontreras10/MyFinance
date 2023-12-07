﻿using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace EFDataAccess.Models
{
    public partial class ResourceRequiredAccess
    {
        public int ResourceActionId { get; set; }
        public int ApplicationResourceId { get; set; }
        public int ResourceAccessLevelId { get; set; }
        public int? ApplicationModuleId { get; set; }

        public virtual ApplicationModule ApplicationModule { get; set; }
        public virtual ApplicationResource ApplicationResource { get; set; }
        public virtual ResourceAccessLevel ResourceAccessLevel { get; set; }
        public virtual ResourceAction ResourceAction { get; set; }
    }
}
