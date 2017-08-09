using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Threading.Tasks;

namespace SamSyncAgent
{
    [RunInstaller(true)]
    public partial class SamSyncServiceInstaller : System.Configuration.Install.Installer
    {
        public SamSyncServiceInstaller()
        {
            InitializeComponent();
        }
    }
}
