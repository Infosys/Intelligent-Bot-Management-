using System;
using System.Collections.Generic;
using System.Text;

using System.Xml.Serialization;

using System.Windows.Forms;

namespace Infosys.Lif.LegacyWorkbench
{
    public class FileStorage : Framework.IStorage
    {
        #region IStorage Members

        public bool Save(Entities.Project projectToBeSaved)
        {
            SaveFileDialog fileDialog = new SaveFileDialog();
            fileDialog.Filter = "XML Files|*.xml|All Files|*.*";
            fileDialog.DefaultExt = "xml";
            fileDialog.OverwritePrompt = true;

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                System.IO.Stream stream = new System.IO.FileStream(fileDialog.FileName, System.IO.FileMode.Create);

                XmlSerializer xmlSerializer = new XmlSerializer(typeof(Entities.Project));
                xmlSerializer.Serialize(stream, projectToBeSaved);
                stream.Close();
            }
            return true;

        }

        public Entities.Project Load()
         {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "XML Files|*.xml|All Files|*.*";
            //fileDialog.DefaultExt = "xml";
            Entities.Project projectLoaded = null;
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                System.IO.FileStream fileStream
                    = new System.IO.FileStream(fileDialog.FileName, System.IO.FileMode.Open);

                //Code added by Srajan J Lad
                //code to prevent uploading of files other than .xml
                if (!fileDialog.FileName.ToLowerInvariant().EndsWith(".xml"))
                {
                    MessageBox.Show("Upload only Xml files with .xml extension", "Legacy Workbench",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    projectLoaded = null;
                }
                else
                {
                    try
                    {
                        XmlSerializer xmlSerializer = new XmlSerializer(typeof(Entities.Project));
                        projectLoaded = (Entities.Project)xmlSerializer.Deserialize(fileStream);
                        fileStream.Close();
                    }
                    catch
                    {
                        MessageBox.Show("This is an invalid xml for Legacy Workbench", "Legacy Workbench",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return null;
                    }
                }
                //Code ends
            }
            return projectLoaded;
        }

        #endregion
    }
}
