using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace SuperBot.ConsoleApp
{
    [RunInstaller(true)]
    public partial class MyProjectInstaller : System.Configuration.Install.Installer
    {
        private ServiceInstaller serviceInstaller1;
        private ServiceProcessInstaller processInstaller;

        public MyProjectInstaller()
        {
            // Instantiate installers for process and services.
            processInstaller = new ServiceProcessInstaller();
            serviceInstaller1 = new ServiceInstaller();

            // The services run under the system account.
            processInstaller.Account = ServiceAccount.LocalSystem;

            // The services are started manually.
            serviceInstaller1.StartType = ServiceStartMode.Manual;

            // ServiceName must equal those on ServiceBase derived classes.
            serviceInstaller1.Description = "Superbot service";
            serviceInstaller1.DisplayName = "Superbot service";
            serviceInstaller1.ServiceName = "SuperBotforHealthCheck";

            // Add installers to collection. Order is not important.
            Installers.Add(serviceInstaller1);
            Installers.Add(processInstaller);
        }
    }
}
