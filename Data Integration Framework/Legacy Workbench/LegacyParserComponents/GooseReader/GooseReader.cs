using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Infosys.Lif.LegacyWorkbench
{
    internal class GooseReader : Framework.IContractRetriever
    {
        public Entities.Contract RetrieveContractDetails(string filePath)
        {
            StreamReader sr = new StreamReader(filePath);

            Entities.Contract contractBeingBuilt = new Entities.Contract();


            contractBeingBuilt.ContractName = FindContractName(sr);

            SearchForStartOfInput(sr);
            Entities.GenericCollection<Entities.ModelObject> inputModelObjects = ExtractModelObjects(sr);
            contractBeingBuilt.InputModelObjects = inputModelObjects;

            if (!sr.EndOfStream)
            {
                // Indicates that the file has not ended. 
                Entities.GenericCollection<Entities.ModelObject> outputModelObjects = ExtractModelObjects(sr);
                contractBeingBuilt.OutputModelObjects = outputModelObjects;
            }

            return contractBeingBuilt;
        }

        private string FindContractName(StreamReader sr)
        {
            string str = sr.ReadLine();

            while (!str.Trim().ToLowerInvariant().StartsWith("contract name"))
            {
                str = sr.ReadLine();
            }
            str = sr.ReadLine();
            str = sr.ReadLine();


            char[] delimiters = new char[1];
            delimiters[0] = ' ';
            string[] strArr = str.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
            if (strArr.Length > 2)
            {
                return strArr[0];
            }
            else
            {
                return string.Empty;
            }

        }
        private void SearchForStartOfInput(StreamReader sr)
        {
            string str = sr.ReadLine();
            while (!str.Trim().ToLowerInvariant().StartsWith("input components"))
            {
                str = sr.ReadLine();
            }
            str = sr.ReadLine();
        }







        private Entities.GenericCollection<Entities.ModelObject> ExtractModelObjects(StreamReader sr)
        {

            Entities.GenericCollection<Entities.ModelObject> topModelObjects = new Entities.GenericCollection<Entities.ModelObject>();
            Entities.ModelObject topModelObject = null;

            string readString = ExtractString(sr, string.Empty);
            while (!CheckForOutput(readString) && !CheckForEndOfFile(sr, readString))
            {


                char[] delimiters = new char[1];
                delimiters[0] = '\t';
                Entities.ModelObject currModelObject = topModelObject;


                string[] splitStrings = readString.Split(delimiters, StringSplitOptions.None);

                // First will have level 0
                int levelNumber = splitStrings.Length - 2;


                if (levelNumber == 0)
                {
                    topModelObject = new Entities.ModelObject();
                    topModelObjects.Add(topModelObject);
                    currModelObject = topModelObject;
                }

                for (int levelIncrementer = 0;
                        levelIncrementer < levelNumber; levelIncrementer++)
                {
                    if (levelIncrementer == levelNumber - 1)
                    {
                        currModelObject.ModelObjects.Add(new Entities.ModelObject());
                    }
                    Entities.GenericCollection<Entities.ModelObject> ModelObjectCollection = currModelObject.ModelObjects;
                    currModelObject = ModelObjectCollection[ModelObjectCollection.Count - 1];
                }


                char[] delimiters2 = new char[1];
                delimiters2[0] = ' ';
                string[] splitStrings2 = splitStrings[splitStrings.Length - 1].Split(delimiters2, StringSplitOptions.RemoveEmptyEntries);

                currModelObject.Name = splitStrings2[splitStrings2.Length - 2];
                currModelObject.HostName = splitStrings2[splitStrings2.Length - 2];


                string[] delimiters3 = new string[1];
                delimiters3[0] = "to";
                string[] splitStrings3 = splitStrings2[splitStrings2.Length - 1].ToLowerInvariant().Split(delimiters3, StringSplitOptions.RemoveEmptyEntries);
                currModelObject.MinCount = System.Convert.ToInt32(splitStrings3[0]);


                if (splitStrings3[1].ToUpperInvariant() != "M")
                {
                    currModelObject.MaxCount = Convert.ToInt32(splitStrings3[1]);
                }
                else
                {
                    currModelObject.MaxCount = -1;
                }

                if (sr.EndOfStream)
                {
                    break;
                }
                readString = ExtractString(sr, readString);

            }
            return topModelObjects;

        }

        private bool CheckForEndOfFile(StreamReader sr, string readString)
        {
            if (readString.Length > 0 || !sr.EndOfStream)
            {
                return false;
            }
            return true;
        }


        private static string ExtractString(StreamReader sr, string readString)
        {
            try
            {
                readString = sr.ReadLine();
                if (!sr.EndOfStream)
                {
                    while (readString.Length == 0)
                    {
                        readString = sr.ReadLine();
                    }
                }
            }
            catch (Exception exc)
            {
                System.Windows.Forms.MessageBox.Show(exc.ToString());
            }
            return readString;
        }
        private bool CheckForOutput(string strRead)
        {
            return (strRead.ToLowerInvariant().Equals("output components"));
        }





    }
}
