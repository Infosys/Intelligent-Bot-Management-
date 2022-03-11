using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Infosys.Lif.LegacyWorkbench
{
    public partial class ItemStatusDisplay : UserControl
    {
        public ItemStatusDisplay()
        {
            InitializeComponent();
        }

        public override string Text
        {
            get
            {
                return lblItemForGeneration.Text;
            }
            set
            {
                lblItemForGeneration.Text = value;
            }
        }
    }
}
