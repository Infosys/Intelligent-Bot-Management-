using System;
using System.Collections.Generic;
using System.Text;

using System.IO;

namespace Infosys.Lif.LegacyWorkbench
{
    internal class CsvReader
    {

        static int unNamedCsv = 0;

        public Entities.Contract RetrieveContractDetails(string filePath)
        {
            Entities.Contract contractBeingBuilt = new Entities.Contract();

            contractBeingBuilt.ContractName = "CsvFile" + unNamedCsv.ToString();
            unNamedCsv++;
            contractBeingBuilt.InputModelObjects = new Entities.GenericCollection<Entities.ModelObject>();
            contractBeingBuilt.InputModelObjects.Add(BuildModelObjects(filePath));
            //Entities.Contract contractBeingBuilt = new Infosys.Lif.LegacyWorkbench.Entities.Contract();

            return contractBeingBuilt;
        }



        private Entities.ModelObject BuildModelObjects(string filePath)
        {
            StreamReader sr = new StreamReader(filePath);
            Entities.ModelObject topModelObject;
            try
            {
                Entities.ModelObject ModelObject = new Entities.ModelObject();

                topModelObject = ModelObject;

                // This will work in the following manner:
                // When it finds the comma, indicating that the level number is increasing
                // it will increase the level number
                // as well as increment the ModelObject pointer.




                String[] split_min, split_max;

                String First_Line, Current_Line;
                char delimeter1, delimeter2;

                int i, level;

                First_Line = sr.ReadLine();
                delimeter1 = ':';
                delimeter2 = ',';

                i = 0;
                level = 0;

                while (!sr.EndOfStream)
                {

                    Current_Line = sr.ReadLine();

                    if (Current_Line != null && Current_Line.Contains(":"))
                    {
                        split_min = Current_Line.Split(delimeter1);
                        split_max = split_min[1].Split(delimeter2);
                        i = 1;
                        level = 1;

                        // Loop thru the split strings and
                        // increase level number if necessary
                        while (i < split_max.Length)
                        {
                            if (split_max[i].TrimEnd() == "")
                            {
                                level++;
                            }
                            else
                            {
                                break;
                            }
                            i++;
                        }

                        ModelObject = topModelObject;

                        for (int levelIncrementer = 1; levelIncrementer < level; levelIncrementer++)
                        {
                            if (levelIncrementer == level - 1)
                            {
                                ModelObject.ModelObjects.Add(new Entities.ModelObject());
                            }
                            Entities.GenericCollection<Entities.ModelObject> ModelObjectCollection = ModelObject.ModelObjects;
                            ModelObject = ModelObjectCollection[ModelObjectCollection.Count - 1];
                        }

                        ModelObject.Name = split_max[i];
                        ModelObject.MinCount = System.Convert.ToInt32(split_min[0]);
                        ModelObject.HostName = split_max[i];

                        if (split_max[0].ToUpperInvariant() != "M")
                        {
                            ModelObject.MaxCount = System.Convert.ToInt32(split_max[0]);
                        }
                        else
                        {
                            ModelObject.MaxCount = -1;
                        }
                    }
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                sr.Close();
                sr = null;
            }
            return topModelObject;
        }
    }
}
