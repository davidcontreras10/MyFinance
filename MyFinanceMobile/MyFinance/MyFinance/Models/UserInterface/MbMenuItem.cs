using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFinance.Models.UserInterface
{
    public class MbMenuItem
    {
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public IEnumerable<MbMenuItem> SubMenus { get; set; } = new MbMenuItem[] { };
    }
}
