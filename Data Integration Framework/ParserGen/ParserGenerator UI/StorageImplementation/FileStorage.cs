using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Windows.Forms;

using Infosys.Lif.LegacyParser.Interfaces;

/****************************************************************
 * This file is a part of the Legacy Parser utility.
 * This file holds the File Storage Implementor.
 * Copyright (c) 2003 - 2005 Infosys Technologies Ltd. All Rights Reserved.
 ***************************************************************/


namespace Infosys.Lif.LegacyParser.UI.StorageImplementations
{
    #region FileStorage definition
    /// <summary>
    /// This is an implementation of the IStorageImplementation. 
    /// It takes input as a Project file and persists into a file by serializing it.
    /// And vice versa.
    /// </summary>
    internal class FileStorage : IStorageImplementation
    {
        /// <summary>
        /// The serializer used to store/retrieve the object of Project type.
        /// </summary>
        XmlSerializer serializer = new XmlSerializer(typeof(Project));

        #region IStorageImplementation Members

        /// <summary>
        /// The method called to persist an object of Project type.
        /// </summary>
        /// <param name="projToBeStored">
        /// The project object which has to be stored.
        /// </param>
        public void Store(Project projectToBeStored)
        {
            // Popup a Save file dialog to input the location where the file should 
            // be stored.
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = "Xml File (*.xml) | *.xml|All Files(*.*)|*.*";
            saveDialog.DefaultExt = "xml";

            DialogResult result = saveDialog.ShowDialog();
            // If Cancel was not pressed
            if (result != DialogResult.Cancel)
            {
                // Open the file stream, and serialzie the Project object passed as input.
                FileStream fs = new FileStream(saveDialog.FileName, FileMode.Create);
                serializer.Serialize(fs, projectToBeStored);
            }

        }

        /// <summary>
        /// Used to retrieve a Project persisted in a file.
        /// </summary>
        /// <returns>
        /// A project object after unpersisiting from a medium.
        /// </returns>
        public Project Retrieve()
        {
            Project project = null;
            // Popup an Open file dialog.
            OpenFileDialog openDialog = new OpenFileDialog();
            openDialog.Multiselect = false;
            openDialog.Filter = "Xml File (*.xml) | *.xml|All Files(*.*)|*.*";
            // If Cancel is not clicked de-serialize the object from the file
            // and return.
            if (openDialog.ShowDialog() != DialogResult.Cancel)
            {
                System.IO.FileStream fs
                    = new System.IO.FileStream(openDialog.FileName,
                    System.IO.FileMode.Open, FileAccess.Read);
                project = (Project)serializer.Deserialize(fs);
            }
            return project;
        }

        #endregion
    }
    #endregion
}
