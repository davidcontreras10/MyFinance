using System.ComponentModel;
using System.Configuration.Install;

namespace CurrencyUpdateWinService
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
        }
    }
}
