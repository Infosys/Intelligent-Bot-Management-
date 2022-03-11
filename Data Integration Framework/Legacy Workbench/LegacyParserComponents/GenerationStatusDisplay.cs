using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Infosys.Lif.LegacyWorkbench
{
    public partial class GenerationStatusDisplay : Form
    {
        public GenerationStatusDisplay()
        {
            InitializeComponent();
        }

        public void AddGenerationItem(string itemGroupType, Entities.GenericCollection<string> itemsToBeAdded)
        {
            CompletionStatus completionDisplayer = new CompletionStatus();
            completionDisplayer.Text = itemGroupType;
            panel1.Controls.Add(completionDisplayer);
        }

    }
}