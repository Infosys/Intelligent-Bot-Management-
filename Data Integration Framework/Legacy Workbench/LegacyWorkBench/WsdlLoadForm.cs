using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Web.Services.Description;
using System.Net;

namespace Infosys.Lif.LegacyWorkbench
{
    public partial class WsdlLoadForm : Form
    {
        public WsdlLoadForm()
        {
            InitializeComponent();
        }

        ServiceDescription wsdlDetails;

        public ServiceDescription WsdlDetails
        {
            get { return wsdlDetails; }
            set { wsdlDetails = value; }
        }              
                
        private void btnOk_Click(object sender, EventArgs e)
        {
            //if no URL or file path is entered
            if (txtFilePath.Text.Trim().Equals(string.Empty) && txtUrl.Text.Trim().Equals(string.Empty))
            {
                MessageBox.Show("Please provide the URL or the file path", "ERROR", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                this.DialogResult = DialogResult.None;
            }
            else
            {
                if (radUrl.Checked == true)
                {
                    //load WSDL from URL
                    string wsdlUrl = txtUrl.Text;
                    if (!wsdlUrl.Trim().EndsWith("?WSDL"))
                    {
                        wsdlUrl = wsdlUrl + "?WSDL";                        
                    }

                    try
                    {
                        HttpWebRequest req = (HttpWebRequest)WebRequest.Create(wsdlUrl);

                        req.Method = "GET";
                        HttpWebResponse result = (HttpWebResponse)req.GetResponse();
                        System.IO.Stream wsdlStream = result.GetResponseStream();
                        wsdlDetails = ServiceDescription.Read(wsdlStream);
                    }                        
                    catch(Exception ex)
                    {
                        MessageBox.Show("Either the URL is wrong or the service is down, Please check.", "ERROR", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        this.DialogResult = DialogResult.None;
                        return;
                    }
                }
                else
                {
                    //load WSDL from file
                    try
                    {
                        wsdlDetails = ServiceDescription.Read(txtFilePath.Text);
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show("Either the file path is wrong or the file is not supported, Please check.", "ERROR", MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        this.DialogResult = DialogResult.None;
                        return;
                    }
                }                
            }
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            openWsdlDialog.Filter = "WSDL Files|*.wsdl|All Files|*.*";
            if (openWsdlDialog.ShowDialog() == DialogResult.OK)
            {
                txtFilePath.Text = openWsdlDialog.FileName;
            }            
        }

        private void radUrl_CheckedChanged(object sender, EventArgs e)
        {
            if (radUrl.Checked == true)
            {
                txtUrl.Enabled = true;
                txtFilePath.Enabled = false;
                btnBrowse.Enabled = false;
            }
            else
            {
                txtUrl.Enabled = false;
                txtFilePath.Enabled = true;
                btnBrowse.Enabled = true;
            }
        }

        private void radFilePath_CheckedChanged(object sender, EventArgs e)
        {
            if (radFilePath.Checked == true)
            {
                txtUrl.Enabled = false;
                txtFilePath.Enabled = true;
                btnBrowse.Enabled = true;
            }
            else
            {
                txtUrl.Enabled = true;
                txtFilePath.Enabled = false;
                btnBrowse.Enabled = false;
            }
        }

        private void WsdlLoadForm_Load(object sender, EventArgs e)
        {
            //set default values of the controls on the form 
            radUrl.Checked = true;
            txtFilePath.Enabled = false;
            btnBrowse.Enabled = false;
        }        
    }
}