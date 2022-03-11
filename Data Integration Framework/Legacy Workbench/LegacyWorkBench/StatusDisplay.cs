using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Infosys.Lif.LegacyWorkbench
{
    public partial class StatusDisplay : Form
    {
        internal ProgressBar statusProgressBar;
        internal Label statusProgressLabel;

        public StatusDisplay()
        {
            InitializeComponent();
            statusProgressBar = progressBar;
            statusProgressLabel = lblStatus;
        }
    }
}