using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Infosys.Lif.LegacyWorkbench
{
    public partial class CompletionStatus : UserControl
    {
        public CompletionStatus()
        {
            InitializeComponent();
        }

        public override string Text
        {
            get
            {
                return lblHeader.Text;
            }
            set
            {
                lblHeader.Text = value;
            }
        }


        System.Collections.Hashtable hashTable = new System.Collections.Hashtable();

        public void DisplayItems(string itemHeading, Entities.GenericCollection<string> itemsToBeAdded)
        {
            lblHeader.Text = itemHeading;
            for (int looper = 0; looper < itemsToBeAdded.Count; looper++)
            {
                string strItemName = itemsToBeAdded[looper];
                ItemStatusDisplay itemDisplayer = new ItemStatusDisplay();
                itemDisplayer.Text = strItemName;
                itemDisplayer.Location = new Point(0, looper + looper * itemDisplayer.Size.Height);
                hashTable.Add(strItemName, itemDisplayer);
                panel1.Controls.Add(itemDisplayer);
            }
        }
    }
}
