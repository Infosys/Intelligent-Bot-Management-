using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;


namespace Infosys.Lif.LegacyWorkbench.CodeProviders
{
    internal class XsdObjectGeneratorWrapper
    {
        
        const string CSharpNameSpaceContract = "";
        const string CSharpNameSpaceModelObject = "";


        const string XsdNameSpaceModelObject = "http://infosys/entities/host/modelobjects/common";
        const string XsdNameSpaceContract = "http://infosys/entities/host/contracts/common";

        private string _xsdObjGenPath;

        internal static System.Collections.Hashtable ModelObjectNamespaceMapping = new System.Collections.Hashtable();

        public XsdObjectGeneratorWrapper(string xsdObjGenPath)
        {
            _xsdObjGenPath = xsdObjGenPath;
        }
        string xsdObjPath
        {
            get
            {
                return _xsdObjGenPath;
            }
        }

        string arguments = "  \"{0}\\{1}.xsd\" /n:{2} /f:\"{3}\\{1}.cs\"";

        public bool CallXsdObjGen(string xsdFilePath, string outputLocation, string fileName, string strNamespace)
        {
            System.Collections.ArrayList filesToBeDeleted = new System.Collections.ArrayList();

            ShowSummary("Creating Directory " + outputLocation);
            System.IO.Directory.CreateDirectory(outputLocation);

            System.Diagnostics.ProcessStartInfo procStartInfo = new System.Diagnostics.ProcessStartInfo(xsdObjPath);

            procStartInfo.RedirectStandardInput = true;
            procStartInfo.RedirectStandardOutput = true;
            procStartInfo.WorkingDirectory = outputLocation;
            procStartInfo.CreateNoWindow = true;
            procStartInfo.UseShellExecute = false;

            string[] strParameters = new string[4];
            strParameters[0] = xsdFilePath;
            strParameters[1] = fileName;
            strParameters[2] = strNamespace;
            strParameters[3] = outputLocation;

            arguments = string.Format(arguments, strParameters);

            procStartInfo.Arguments = arguments;


            System.IO.StreamWriter streamWriter;
            System.IO.StreamReader streamReader;

            ShowSummary("Executing " + procStartInfo.FileName + " " + procStartInfo.Arguments);
            ShowSummary("=======================================");
            System.Diagnostics.Process xsdGenProcess = System.Diagnostics.Process.Start(procStartInfo);



            streamWriter = xsdGenProcess.StandardInput;
            streamReader = xsdGenProcess.StandardOutput;

            //textBox1.Text += streamReader.ReadToEnd();
            ////SearchForString(streamReader, "Imported namespaces were found");
            ////SearchForString(streamReader, "conflict with types and element names from the schemas");

            bool isCompleted = false;
            while (!isCompleted)
            {
                string readString = RetrieveString(streamReader);

                ShowSummary(readString);

                if (readString.Contains("Please enter a CLR namespace name for this namespace: "))
                {
                    readString = RetrieveSchemaNamespace(readString);
                    ShowSummary("Trying to parse the namespace for" + readString);

                    //readString = ParseSchemaNamspace(readString);

                    readString = RetrieveCSharpNamespace(readString);
                    ShowSummary("Writing namespace " + readString);
                    WriteCsharpNameSpace(streamWriter, readString);
                }
                if (streamReader.EndOfStream)
                {
                    isCompleted = true;
                }

                if (readString.Contains("Writing file"))
                {
                    string nameOfFileWritten = readString.Substring(" Writing file ".Length, readString.Length - " Writing file ".Length);
                    if (!nameOfFileWritten.StartsWith(outputLocation + "\\" + fileName + ".cs"))
                    {
                        filesToBeDeleted.Add(nameOfFileWritten);
                    }
                    else
                    {
                        DeleteFirst2Lines(outputLocation + "\\" + fileName + ".cs");
                    }
                }
            }
            ShowSummary("=======================================");
            ShowSummary("XSD Obj Gen returned " + xsdGenProcess.ExitCode);
            foreach (string fileToBeDeleted in filesToBeDeleted)
            {
                ShowSummary("Deleted file " + outputLocation + "\\" + fileToBeDeleted);
                System.IO.File.Delete(outputLocation + "\\" + fileToBeDeleted);
            }
            return true;
        }

        private void DeleteFirst2Lines(string p)
        {
            return;
            System.IO.FileStream fs = new System.IO.FileStream(p, System.IO.FileMode.Open);
            System.IO.StreamReader sr = new System.IO.StreamReader(fs);
            sr.ReadLine();
            sr.ReadLine();
            string fileContent = sr.ReadToEnd();

            fs.Close();

            fs = new System.IO.FileStream(p, System.IO.FileMode.Open);
            System.IO.StreamWriter sw = new System.IO.StreamWriter(fs);

            // Ignore the first 2 lines.
            sw.Write(fileContent);
            fs.Close();



        }
        private void ShowSummary(string p)
        {
            Framework.Helper.ShowSummary(p);

        }
        private void WriteCsharpNameSpace(System.IO.StreamWriter streamWriter, string readString)
        {
            streamWriter.WriteLine(readString);
        }
        private string RetrieveCSharpNamespace(string readString)
        {
            if (readString == XsdNameSpaceModelObject)
            {
                return CSharpNameSpaceModelObject;
            }
            if (readString == XsdNameSpaceContract)
            {
                return CSharpNameSpaceContract;
            }

            string keyForEntity = string.Empty;
            foreach (string strKey in Mappings.XsdNameSpaceMappings.Keys)
            {
                if (Mappings.XsdNameSpaceMappings[strKey].ToString().Trim() == readString)
                {
                    keyForEntity = strKey;
                }
            }
            return Mappings.CSharpNameSpaceMappings[keyForEntity].ToString();



            //////char[] splitChars = new char[1];
            //////splitChars[0] = ':';
            //////string[] splitStrings = readString.Split(splitChars);
            //////// Can be ModelObject/Contract
            //////string entityType = splitStrings[0];
            //////string moduleName = splitStrings[1];
            //////string entityName = splitStrings[2];

            //////return nameSpace + "." + entityType + "." + moduleName + "." + entityName;
        }
        //////private string ParseSchemaNamspace(string readString)
        //////{
        //////    char[] splitChars = new char[1];
        //////    splitChars[0] = '/';
        //////    string[] splitStrings = readString.Split(splitChars, StringSplitOptions.None);
        //////    readString = splitStrings[splitStrings.Length - 1];
        //////    return readString;

        //////    //////char[] splitChars = new char[1];
        //////    //////splitChars[0] = '/';
        //////    //////string[] splitStrings = readString.Split(splitChars, StringSplitOptions.None);
        //////    //////readString = splitStrings[splitStrings.Length - 3] + ":" + splitStrings[splitStrings.Length - 2] + ":" + splitStrings[splitStrings.Length - 1];
        //////    //////return readString;
        //////}
        private string RetrieveString(System.IO.StreamReader streamReader)
        {
            string readString = string.Empty;
            int charCode = streamReader.Peek();


            ///////////while (!readString.Contains("Please enter a CLR namespace name for this namespace: ") && charCode!=-1)
            while (charCode != -1)
            {
                charCode = streamReader.Read();
                readString += char.ConvertFromUtf32(charCode);
                if (charCode == '\n')
                    break;
                charCode = streamReader.Peek();
            }
            return readString;
        }
        private string RetrieveSchemaNamespace(string readString)
        {
            int indexOfEnd = readString.IndexOf(". Please enter a CLR namespace name for this namespace: ");
            int lengthOfStartTag = "Xsd namespace = ".Length;
            readString = readString.Substring(lengthOfStartTag, indexOfEnd - lengthOfStartTag);
            return readString;

        }

        //////private string SearchForString(System.IO.StreamReader streamReader, string searchString)
        //////{
        //////    string readString = string.Empty;
        //////    while (!readString.Contains(searchString))
        //////    {
        //////        readString = streamReader.ReadLine();
        //////        textBox1.Text += readString;
        //////    }
        //////    return readString;
        //////}

    }
}
