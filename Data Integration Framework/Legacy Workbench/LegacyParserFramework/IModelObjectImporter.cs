using System;
using System.Collections.Generic;
using System.Text;
using Infosys.Lif.LegacyWorkbench.Entities;


namespace Infosys.Lif.LegacyWorkbench.Framework
{
    public interface IModelObjectImporter
    {
        //GenericCollection<Entity> RetrieveModelObjectDetails();
        event ModelObjectsRetrievedEventHandler ModelObjectsRetrieved;

        Entities.Entity RetrieveModelObjectDetails(string fileName);
    }

    public delegate void ModelObjectsRetrievedEventHandler(object sender, ModelObjectsRetrievedEventArgs e);

    public class ModelObjectsRetrievedEventArgs : EventArgs
    {
        GenericCollection<Entity> retrievedModelObjects = new GenericCollection<Entity>();

        public GenericCollection<Entity> RetrievedModelObjects
        {
            get { return retrievedModelObjects; }
            set { retrievedModelObjects = value; }
        }
    }
}
