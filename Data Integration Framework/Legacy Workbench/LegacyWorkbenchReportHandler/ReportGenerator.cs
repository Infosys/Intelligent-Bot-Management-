using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Configuration;
using System.Reflection;

using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Serialization;
using EnvDTE;
using System.Data;
using System.Diagnostics;

namespace Infosys.Lif.LegacyWorkbench.ReportManager
{
    public class ReportGenerator
    {
        const string configurations = "CONFIGURATIONS";
        const string dataEntities = "DATAENTITIES";
        const string hostAccess = "HOSTACCESS";
        const string serializers = "SERIALIZERS";
        const string WCF = "WCF";
        const string WebService = "WEB SERVICE";
        const string bin = "BIN";
        const string obj = "OBJ";
        const string properties = "PROPERTIES";

        const string code = "Code";
        const string xml = "XML";
        const string xsd = "XSD";

        const string nameOfXMLFile = "GenerationReportXml";
       
        private static Hashtable htRecordTime = new Hashtable();
        
        private static long startTime;
        private Hashtable htProjectNames = new Hashtable();
        const string empty = "";

        //made static so that values in hashtable are retained when control goes back to legacy parser after initialization.
        static Hashtable htCodeGroup = new Hashtable();
        static Hashtable htCapersJonesConversionFactors = new Hashtable();
        static Hashtable htCustomConversionFactors = new Hashtable();

        //Parameterized constructor- required in order to pass the hashtables carrying code group,CapersJonesConversionFactors,        
        //CustomConversionFactors from LegacyParser.
        public ReportGenerator(Hashtable CodeGroup, Hashtable CapersJonesConversionFactors, Hashtable CustomConversionFactors)
        {
            htCodeGroup = CodeGroup;
            htCapersJonesConversionFactors = CapersJonesConversionFactors;
            htCustomConversionFactors = CustomConversionFactors;
            
        }

        //Default constructor
        public ReportGenerator()
        {
        }

        /// <summary>
        /// Stores the file name and start time as key-value pairs in a hashtable.Called when a file is sent for creation.
        /// </summary>
        /// <param name="filename">The absolute filename of the file being generated.</param>
        public static void StartTimeCapture(string filename)
        {
            startTime = DateTime.Now.Ticks;
            htRecordTime.Add(filename, startTime);
        }

        /// <summary>
        /// Replaces the start time with the time taken for generation in the hashtable.Called after a file has been created.
        /// </summary>
        /// <param name="filename">The absolute filename of the file being generated.</param>
        public static void RecordTime(string filename)
        {
            long stopTimeCapture = DateTime.Now.Ticks - startTime;
            TimeSpan timeDiff = new TimeSpan(stopTimeCapture);

            object changeValueForKey = null;
            foreach (DictionaryEntry de in htRecordTime)
            {
                if (de.Key.ToString().ToUpperInvariant() == filename.ToUpperInvariant())
                {
                    changeValueForKey = de.Key;
                    break;
                }
            }
            //updating the value to the total time taken for creation(in milli seconds).
            htRecordTime[changeValueForKey] = timeDiff.TotalMilliseconds;
            
        }

        /// <summary>
        /// Clearing all static variables.
        /// </summary>
        public static void CleanUpStaticVariables()
        {
            htRecordTime.Clear();
            startTime = 0;
            htCodeGroup.Clear();
            htCapersJonesConversionFactors.Clear();
            htCustomConversionFactors.Clear();
        }

        /*************************************************************************************************/

        /// <summary>
        /// Generate report in xml format.Called from the legacy workbench after all the files have been created.
        /// </summary>
        /// <param name="outputPath">The absolute path of the generated solution.</param>
        /// <param name="selectedServiceType">The Service Type chosen for which the solution will be generated.</param>
        public static void GenerateXMLForReport(string outputPath, string solnFileName, string selectedServiceType)
        {
            ReportGenerator report = new ReportGenerator();
            Infosys.Lif.LegacyWorkbench.ReportManager.FileCollection fileCollectionTopHierarchy = new FileCollection();

            DirectoryInfo directories = new DirectoryInfo(outputPath);
            FileInfo[] filesInTopHierarchy = directories.GetFiles();

            //Files in the top hierarchy are added 
            foreach (FileInfo fiTemp in filesInTopHierarchy)
            {
                Infosys.Lif.LegacyWorkbench.ReportManager.File file1 = new File();
                file1 = report.AddFileDetails(directories, fiTemp);
                if (file1 != null)
                {
                    //Files in top hierarchial level are not associated with any group.
                    file1.Group = empty;
                    fileCollectionTopHierarchy.Add(file1);
                }
            }         
            
            Infosys.Lif.LegacyWorkbench.ReportManager.LegacyWorkbenchReport reportForXML = new Infosys.Lif.LegacyWorkbench.ReportManager.LegacyWorkbenchReport();
            Infosys.Lif.LegacyWorkbench.ReportManager.Configurations config = new Configurations();
            Infosys.Lif.LegacyWorkbench.ReportManager.DataEntities dataEntity = new DataEntities();
            Infosys.Lif.LegacyWorkbench.ReportManager.Serializers serializer = new Serializers();
            Infosys.Lif.LegacyWorkbench.ReportManager.ServiceInterface serviceInterface = new ServiceInterface();
            int totalNoOfFiles = 0;
            
            //Scanning through all the directories present at the location where the generated solution has been saved.
            foreach (DirectoryInfo dir in directories.GetDirectories())
            {
                string nameOfFolder = dir.Name;
                switch (nameOfFolder.ToUpperInvariant())
                {
                    case configurations:
                        //Extracting the details for the files present in the configuration folder for service interface-HA/WF
                        config = report.CreateConfigStructureForHostAccessAndWF(dir);
                        break;
                    case dataEntities:
                        //Extracting the details for the files present in the data entities folder
                        dataEntity = report.CreateDataEntityStructure(dir);
                        break;
                    case hostAccess:
                        if (selectedServiceType.ToUpperInvariant() == WCF || selectedServiceType.ToUpperInvariant() == WebService)
                        {
                            //Extracting the details for the files present in the configuration folder for service interface-WebService/WCF
                            config = report.CreateConfigStructureForWebServiceAndWCF(dir);
                        }
                        serviceInterface = report.CreateServiceInterfaceStruct(dir);
                        break;
                    case serializers:
                        //Extracting the details for the files present in the serializers folder
                        serializer = report.CreateSerializerStructure(dir);
                        break;
                    default:
                        break;
                }
            }

            reportForXML.FileCollection = fileCollectionTopHierarchy;
            reportForXML.Configurations = config;
            reportForXML.DataEntities = dataEntity;
            reportForXML.Serializers = serializer;
            reportForXML.ServiceInterface = serviceInterface;
            totalNoOfFiles = Convert.ToInt32(config.NumberOfFiles) + Convert.ToInt32(dataEntity.NumberOfFiles) + Convert.ToInt32(serviceInterface.NumberOfFiles) + Convert.ToInt32(serializer.NumberOfFiles) + Convert.ToInt32(fileCollectionTopHierarchy.Count);
            reportForXML.TotalNoOfFiles = totalNoOfFiles.ToString();
            
            string currentTime = System.DateTime.Now.ToLongDateString();

            
            System.IO.Stream stream = new System.IO.FileStream(outputPath + "\\" +  solnFileName + "Report" + currentTime+ ".xml", System.IO.FileMode.Create);
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Infosys.Lif.LegacyWorkbench.ReportManager.LegacyWorkbenchReport));
            //Generate XML
            xmlSerializer.Serialize(stream, reportForXML);
            stream.Close();
        }
        
        /***********************************************************************************************************/
        /// <summary>
        /// Create structure for configuration folder for service type Host Access or WF Activity.
        /// </summary>
        /// <param name="dir">The directory corresponding to the Configurations folder.</param>
        /// <returns>An instance of Infosys.Lif.LegacyWorkbench.ReportManager.Configurations which contains all the attributes 
        /// and details of files in the Configurations folder.</returns>
        private Infosys.Lif.LegacyWorkbench.ReportManager.Configurations CreateConfigStructureForHostAccessAndWF(DirectoryInfo dir)
        {
            Infosys.Lif.LegacyWorkbench.ReportManager.Configurations config = new Infosys.Lif.LegacyWorkbench.ReportManager.Configurations();
            config.FileCollection = CreateFileCollection(dir);
            config.NumberOfFiles = config.FileCollection.Count.ToString();
            return config;
        }

        /***********************************************************************************************************/
        /// <summary>
        /// Create structure for configuration folder for service type WCF and Web Service.
        /// </summary>
        /// <param name="dir">The directory corresponding to the Configurations folder.</param>
        /// <returns>An instance of Infosys.Lif.LegacyWorkbench.ReportManager.Configurations which contains all the attributes 
        /// and details of files in the Configurations folder.</returns>
        private Infosys.Lif.LegacyWorkbench.ReportManager.Configurations CreateConfigStructureForWebServiceAndWCF(DirectoryInfo dir)
        {
            Infosys.Lif.LegacyWorkbench.ReportManager.Configurations config = new Infosys.Lif.LegacyWorkbench.ReportManager.Configurations();

            DirectoryInfo[] diLevel1 = dir.GetDirectories();//WebService/WCFHost,project.datacontract,project.servicecontract,project.serviceimplementation
            foreach (DirectoryInfo diTempLevel1 in diLevel1)
            {
                Infosys.Lif.LegacyWorkbench.ReportManager.FileCollection fileCollection = new FileCollection();
                if (diTempLevel1.Name == "WebService" || diTempLevel1.Name == "WCFHost")
                {
                    DirectoryInfo[] diLevel2 = diTempLevel1.GetDirectories();
                    foreach (DirectoryInfo diTempLevel2 in diLevel2)
                    {
                        if (diTempLevel2.Name == "Configurations")
                        {
                            FileInfo[] filesInConfig = diTempLevel2.GetFiles();
                            foreach (FileInfo fiTemp in filesInConfig)
                            {
                                Infosys.Lif.LegacyWorkbench.ReportManager.File file1 = new File();
                                file1 = AddFileDetails(diTempLevel1, fiTemp);
                                if (file1 != null)
                                {
                                    file1.Group = "";
                                    fileCollection.Add(file1);
                                }
                            }
                            config.FileCollection = fileCollection;
                            config.NumberOfFiles = config.FileCollection.Count.ToString();
                        }
                    }
                }
            }
            return config;
        }

        /***********************************************************************************************************/
        /// <summary>
        /// Create structure for dataEntities folder.
        /// </summary>
        /// <param name="dir">The directory corresponding to the Data Entities folder.</param>
        /// <returns>An instance of Infosys.Lif.LegacyWorkbench.ReportManager.DataEntities which contains all the attributes 
        /// and details of files in the DataEntities folder.</returns>
        private Infosys.Lif.LegacyWorkbench.ReportManager.DataEntities CreateDataEntityStructure(DirectoryInfo dir)//dir=data entities
        {
            Infosys.Lif.LegacyWorkbench.ReportManager.DataEntities dataEntities = new Infosys.Lif.LegacyWorkbench.ReportManager.DataEntities();
            Infosys.Lif.LegacyWorkbench.ReportManager.FileCollection fileCollection = new Infosys.Lif.LegacyWorkbench.ReportManager.FileCollection();
            
            int noOfFilesInContracts = 0;
            int noOfFilesInModelObjects = 0;
            int noOfFilesInDataEntities = 0;
            DirectoryInfo[] di = dir.GetDirectories();//contracts, model objects
            
            //scanning through all the directories in DataEntities.
            foreach (DirectoryInfo diTemp in di)
            {
                if (diTemp.Name=="Contracts")
                {
                    //Create file collection for contracts in DataEntities.
                    dataEntities.Contracts.FileCollection = CreateStructureForContractsAndModelObjsDE(diTemp);
                    noOfFilesInContracts = dataEntities.Contracts.FileCollection.Count;
                }
                else if (diTemp.Name == "ModelObjects")
                {
                    //Create file collection for model objects in DataEntities.
                    dataEntities.ModelObjects.FileCollection = CreateStructureForContractsAndModelObjsDE(diTemp);
                    noOfFilesInModelObjects = dataEntities.ModelObjects.FileCollection.Count;
                }
            }

            dataEntities.Contracts.NumberOfFiles = noOfFilesInContracts.ToString();
            dataEntities.ModelObjects.NumberOfFiles = noOfFilesInModelObjects.ToString();
            noOfFilesInDataEntities = noOfFilesInContracts + noOfFilesInModelObjects;
            dataEntities.NumberOfFiles = noOfFilesInDataEntities.ToString();
            return dataEntities;
        }

        /***********************************************************************************************************/
        /// <summary>
        /// Create structure for Contracts and Model Objects in Data Entites.
        /// </summary>
        /// <param name="dir">The directory corresponding to the Contracts/Model Objects folder in the Data Entities folder.</param>
        /// <returns>An instance of Infosys.Lif.LegacyWorkbench.ReportManager.FileCollection which contains all the attributes 
        /// and details of files in the Contracts/Model Objects folder inside DataEntities folder.</returns>
        private Infosys.Lif.LegacyWorkbench.ReportManager.FileCollection CreateStructureForContractsAndModelObjsDE(DirectoryInfo dir)//dir= Contracts
        {
            Infosys.Lif.LegacyWorkbench.ReportManager.FileCollection fileCollection = new FileCollection();

            DirectoryInfo[] diLevel1 = dir.GetDirectories();//di=code,xsd....say code
            foreach (DirectoryInfo diTempLevel1 in diLevel1)
            {
                //chk if files exist and create a filecollection for the same.
                FileInfo[] filesLevel1 = diTempLevel1.GetFiles();
                foreach (FileInfo fiTempLevel1 in filesLevel1)
                {
                    Infosys.Lif.LegacyWorkbench.ReportManager.File file1 = new File();
                    file1 = AddFileDetails(diTempLevel1, fiTempLevel1);
                    if (file1 != null)
                    {
                        file1.Group = empty;
                        fileCollection.Add(file1);
                    }
                    
                }

                DirectoryInfo[] diLevel2 = diTempLevel1.GetDirectories();//obj,gpnm1,gpnm2....

                foreach (DirectoryInfo diTempLevel2 in diLevel2)//obj,gpnm1,gpnm2....
                {
                    if (diTempLevel2.Name.ToUpperInvariant() != obj)
                    {
                        FileInfo[] filesLevel2 = diTempLevel2.GetFiles();
                        foreach (FileInfo fiTempLevel2 in filesLevel2)
                        {
                            Infosys.Lif.LegacyWorkbench.ReportManager.File file1 = new File();
                            file1 = AddFileDetails(diTempLevel2, fiTempLevel2);
                            if (file1 != null)
                            {
                                file1.Group = diTempLevel2.Name;
                                fileCollection.Add(file1);
                            }
                            
                        }
                    }
                }
            }
            return fileCollection;
        }
        /***********************************************************************************************************/
        /// <summary>
        /// Create structure for service interface folder.
        /// </summary>
        /// <param name="dir">The directory corresponding to the Service Interface folder.</param>
        /// <returns>An instance of Infosys.Lif.LegacyWorkbench.ReportManager.ServiceInterface which contains all the attributes 
        /// and details of files in the ServiceInterface folder.</returns>
        private Infosys.Lif.LegacyWorkbench.ReportManager.ServiceInterface CreateServiceInterfaceStruct(DirectoryInfo dir)//dir=host access
        {
            Infosys.Lif.LegacyWorkbench.ReportManager.ServiceInterface serviceInterface = new ServiceInterface();
            Infosys.Lif.LegacyWorkbench.ReportManager.FileCollection fileCollection = new FileCollection();
            int noOfFilesInServiceInterface = 0;
          
            //1. add files present in Host Access folder.
            FileInfo[] filesInHostAccess = dir.GetFiles();
            foreach (FileInfo fiTemp in filesInHostAccess)
            {
                Infosys.Lif.LegacyWorkbench.ReportManager.File file1 = new File();
                file1 = AddFileDetails(dir, fiTemp);
                if (file1 != null)
                {
                    file1.Group = empty;
                    fileCollection.Add(file1);
                }
                
            }

            //2. Scanning thru directoriess in Host Access.
            DirectoryInfo[] dirLevel1 = dir.GetDirectories();//project.datacontract,project.servicecontract,project.serviceimplementation,WCFHost....DataContractAssembly,ServiceContractAssembly,ServiceImplementationAssembly,WebService

            foreach (DirectoryInfo dirTempLevel1 in dirLevel1)
            {
                //3. If files exist, add them to the file collection
                FileInfo[] files = dirTempLevel1.GetFiles();
                foreach (FileInfo fiTemp in files)
                {
                    Infosys.Lif.LegacyWorkbench.ReportManager.File file1 = new File();
                    file1 = AddFileDetails(dirTempLevel1, fiTemp);
                    if (file1 != null)
                    {
                        file1.Group = empty;
                        fileCollection.Add(file1);
                    }
                }
                
                //4. Scanning thru subdirectories in the directories present in Host Access.
                DirectoryInfo[] dirLevel2 = dirTempLevel1.GetDirectories();//bin,obj,properties,gpnm1,gpnm2....
                foreach (DirectoryInfo dirTempLevel2 in dirLevel2)
                {
                    if (dirTempLevel2.Name.ToUpperInvariant() != obj && dirTempLevel2.Name.ToUpperInvariant() != bin && dirTempLevel2.Name.ToUpperInvariant()!= configurations)
                    {
                        FileInfo[] filesLevel2 = dirTempLevel2.GetFiles();
                        foreach (FileInfo fiTempLevel2 in filesLevel2)
                        {
                            Infosys.Lif.LegacyWorkbench.ReportManager.File file2 = new File();
                            file2 = AddFileDetails(dirTempLevel2, fiTempLevel2);
                            if (file2 != null)
                            {
                                if (dirTempLevel2.Name.ToUpperInvariant() == properties)
                                {
                                    file2.Group = empty;
                                }
                                else
                                {
                                    file2.Group = dirTempLevel2.Name;
                                }
                                fileCollection.Add(file2);
                            }
                        }
                    }
                }
            }
            
            serviceInterface.FileCollection = fileCollection;
            noOfFilesInServiceInterface = serviceInterface.FileCollection.Count;
            serviceInterface.NumberOfFiles = noOfFilesInServiceInterface.ToString();
            return serviceInterface;
        }
        /***********************************************************************************************************/
        /// <summary>
        /// Create structure for Serializers folder.
        /// </summary>
        /// <param name="dir">The directory corresponding to the Serializers folder</param>
        /// <returns>An instance of Infosys.Lif.LegacyWorkbench.ReportManager.Serializers which contains all the attributes 
        /// and details of files in the Serializers folder.</returns>
        private Infosys.Lif.LegacyWorkbench.ReportManager.Serializers CreateSerializerStructure(DirectoryInfo dir)//dir=Serializers
        {
            Infosys.Lif.LegacyWorkbench.ReportManager.Serializers serializers = new Serializers();
            Infosys.Lif.LegacyWorkbench.ReportManager.FileCollection fileCollection = new Infosys.Lif.LegacyWorkbench.ReportManager.FileCollection();

            int noOfFilesInContracts = 0;
            int noOfFilesInModelObjects = 0;
            int noOfFilesInSerializers = 0;
            DirectoryInfo[] di = dir.GetDirectories();//contracts, model objects

            foreach (DirectoryInfo diTemp in di)
            {
                if (diTemp.Name == "Contracts")
                {
                    serializers.Contracts.FileCollection = CreateStructForContractsAndModelObjsSerializers(diTemp);
                    noOfFilesInContracts = serializers.Contracts.FileCollection.Count;
                }
                else if (diTemp.Name == "ModelObjects")
                {
                    serializers.ModelObjects.FileCollection = CreateStructForContractsAndModelObjsSerializers(diTemp);
                    noOfFilesInModelObjects = serializers.ModelObjects.FileCollection.Count;
                }
            }

            serializers.Contracts.NumberOfFiles = noOfFilesInContracts.ToString();
            serializers.ModelObjects.NumberOfFiles = noOfFilesInModelObjects.ToString();
            noOfFilesInSerializers = noOfFilesInContracts + noOfFilesInModelObjects;
            serializers.NumberOfFiles = noOfFilesInSerializers.ToString();

            return serializers;
        }
        /***********************************************************************************************************/
        /// <summary>
        /// Create structure for Contracts and Model Objects in Serializers.
        /// </summary>
        /// <param name="dir">The directory corresponding to the Contracts/Model Objects folder in the Serializers folder.</param>
        /// <returns>An instance of Infosys.Lif.LegacyWorkbench.ReportManager.FileCollection which contains all the attributes 
        /// and details of files in the Contracts/Model Objects folder inside Serializers folder.</returns>
        private Infosys.Lif.LegacyWorkbench.ReportManager.FileCollection CreateStructForContractsAndModelObjsSerializers(DirectoryInfo dir)//dir= Contracts
        {
            Infosys.Lif.LegacyWorkbench.ReportManager.FileCollection fileCollection = new FileCollection();

            DirectoryInfo[] di = dir.GetDirectories();//obj,gpnm1,gpnm2....

            FileInfo[] filesLevel1 = dir.GetFiles();
            foreach (FileInfo fiTemp in filesLevel1)
            {
                Infosys.Lif.LegacyWorkbench.ReportManager.File file1 = new File();
                file1 = AddFileDetails(dir, fiTemp);
                if (file1 != null)
                {
                    file1.Group = "";
                    fileCollection.Add(file1);
                }
            }

            foreach (DirectoryInfo diTemp in di)//obj,gpnm1,gpnm2....
            {
                if (diTemp.Name != "obj")
                {
                    FileInfo[] files = diTemp.GetFiles();
                    foreach (FileInfo fiTemp in files)
                    {
                        Infosys.Lif.LegacyWorkbench.ReportManager.File file1 = new File();
                        file1 = AddFileDetails(diTemp, fiTemp);
                        if (file1 != null)
                        {
                            file1.Group = diTemp.Name;
                            fileCollection.Add(file1);
                        }
                    }
                }
            }
            return fileCollection;
        }
        /***********************************************************************************************************/
        /// <summary>
        /// Add file details.
        /// </summary>
        /// <param name="dir">The directory containing the file whose details are computed here.</param>
        /// <param name="fiTemp">File whose attributes are to be computed.</param>
        /// <returns>An instance of Infosys.Lif.LegacyWorkbench.ReportManager.File which contains all the attributes 
        ///  of the file passed in the parameter.</returns>
        private Infosys.Lif.LegacyWorkbench.ReportManager.File AddFileDetails(DirectoryInfo dir, FileInfo fiTemp)
        {
            Infosys.Lif.LegacyWorkbench.ReportManager.File file = new File();
            file.Name = fiTemp.Name;

            //Extracting time taken from the hashtable.
            foreach (DictionaryEntry de in htRecordTime)
            {
                if (de.Key.ToString().ToUpperInvariant() == fiTemp.FullName.ToUpperInvariant())
                {
                    file.TimeTakenForGeneration = de.Value.ToString();
                }
            }

            //Extracting CapersJonesConversionFactor from the hashtable.
            foreach (DictionaryEntry de in htCapersJonesConversionFactors)
            {
                if (de.Key.ToString().ToUpperInvariant() == fiTemp.Extension.ToUpperInvariant())
                {
                    file.CapersJonesConversionFactor = Convert.ToDecimal(de.Value);
                }
            }

            //Extracting CustomConversionFactor from the hashtable.
            foreach (DictionaryEntry de in htCustomConversionFactors)
            {
                if (de.Key.ToString().ToUpperInvariant() == fiTemp.Extension.ToUpperInvariant())
                {
                    file.CustomConversionFactor = Convert.ToDecimal(de.Value);
                }
            }

            if (dir.GetDirectories().Length==0)
            {
                file.Group = empty;
            }

            //Check if the file is the solution file, then compute the project attribute for all the files.
            if (fiTemp.Extension == ".sln")
            {
                ComputeProjectAttribute(fiTemp);
            }

            file.Location = fiTemp.DirectoryName;
            
            string extensionsCodeGroup = "";
            string extensionsXMLGroup = "";
            string extensionsXSDGroup = "";
            string fileStatisticsGroup = "";

            foreach (DictionaryEntry de in htCodeGroup)
            {
                if (de.Key.ToString() == "ExtensionsCodeGroup")
                {
                    extensionsCodeGroup = de.Value.ToString();
                }
                else if (de.Key.ToString() == "ExtensionsXMLGroup")
                {
                    extensionsXMLGroup = de.Value.ToString();
                }
                else if (de.Key.ToString() == "ExtensionsXSDGroup")
                {
                    extensionsXSDGroup = de.Value.ToString();
                }
                else if (de.Key.ToString() == "FileStatisticsGroup")
                {
                    fileStatisticsGroup = de.Value.ToString();
                }
            }
            
            bool fileExists = false;

            Array extnsCodeGp = extensionsCodeGroup.Split(';');
            Array extnsXMLGp = extensionsXMLGroup.Split(';');
            Array extnsXSDGp = extensionsXSDGroup.Split(';');
            Array extnsFileStatisticsGp = fileStatisticsGroup.Split(';');

            //Computing the file type on the basis of the extension of the file.
            foreach (string extension in extnsCodeGp)//extensions in this group currently are: csproj,svc,cs,sln,asmx
            {
                if (fiTemp.Extension == extension)
                {
                    file.Type = code;
                    fileExists = true;
                    break;
                }
            }
            foreach (string extension in extnsXMLGp)//extensions in this group currently are: config
            {
                if (fiTemp.Extension == extension)
                {
                    file.Type = xml;
                    fileExists = true;
                }
            }
            foreach (string extension in extnsXSDGp)//extensions in this group currently are: xsd
            {
                if (fiTemp.Extension == extension)
                {
                    file.Type = xsd;
                    fileExists = true;
                }
            }    
            if ( !fileExists)
            {
                file = null;
            }
            else 
            {
                foreach (DictionaryEntry de in htProjectNames)
                {
                    if (de.Key.ToString() == fiTemp.FullName.ToUpperInvariant() || de.Key.ToString() == fiTemp.DirectoryName.ToUpperInvariant())
                    {
                        file.Project = de.Value.ToString();
                    }
                }
                
                foreach (string extension in extnsFileStatisticsGp)
                {
                    if (fiTemp.Extension == extension)
                    {
                        ComputeFileStatistics(file, fiTemp);//parameters--file:ServiceWorkbench file, fitemp : file whose details are to be computed.
                    }
                }
            }
            return file;
        }

        /***********************************************************************************************************/
        /// <summary>
        /// Compute the project attribute.
        /// </summary>
        /// <param name="file">File whose project attribute is to be computed</param>
        private void ComputeProjectAttribute(FileInfo file)
        {
            System.Diagnostics.Process p = new System.Diagnostics.Process();
            p.StartInfo.RedirectStandardOutput = false;
            p.StartInfo.Arguments = file.FullName;
            p.StartInfo.FileName = file.FullName;
            p.StartInfo.UseShellExecute = true;
            p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            p.StartInfo.CreateNoWindow = true;

            p.Start();

            EnvDTE.DTE dte = null;

            try
            {
                //Get the Dte object .looping done to allow the Process of loading the solution completely
                while (dte == null)
                {
                    dte = Infosys.Lif.LegacyWorkbench.ReportManager.VSEnvironmentDteHandler.GetIDEInstance(file.FullName);
                }
            }
            catch (Exception ex)
            {
                p.Close();
            }

            if (dte == null)
            {
                p.Close();
            }

            System.Threading.Thread.Sleep(800);
            EnvDTE.Solution sln = (EnvDTE.Solution)dte.Solution;
            EnvDTE.Projects projectsInSolution;
            EnvDTE.Project proj = null;
            //string projectName = "";
            string fileName = "";
            string fullPathForFolderItem = "";
            string relativePath = "";
            string fullPathForFileItem = "";
            int projectsInSolutionCount = 1;
            projectsInSolution = sln.Projects;
            //Make sure there is at least one project in the open solution
            //if (projectsInSolution.Count > 0)
            {
                while (projectsInSolutionCount <= projectsInSolution.Count)
                {
                    proj = (EnvDTE.Project)(projectsInSolution.Item(projectsInSolutionCount));
                    relativePath = proj.UniqueName;

                    //Give solution file as the project attribute for all csproj files.
                    if (proj.Kind != "{E24C65DC-7377-472b-9ABA-BC803B73C61A}")//website project
                    {
                        htProjectNames.Add(proj.FullName.ToUpperInvariant(), Path.GetFileName(sln.FileName));
                    }
                   
                    int fileCount = 0;
                    int projectItemsInProjectLevel1Count = 1;
                    //Scanning through all the Project items for each project in the solution.
                    while (projectItemsInProjectLevel1Count <= proj.ProjectItems.Count)
                    {
                        int projProperties = 1;
                        //Scanning through all the properties of all the items in the project to extract the full path of the file..
                        while (projProperties <= proj.Properties.Count)
                        {
                            if (proj.ProjectItems.Item(projectItemsInProjectLevel1Count).Properties.Item(projProperties).Name == "FullPath")
                            {
                                fullPathForFileItem = proj.ProjectItems.Item(projectItemsInProjectLevel1Count).Properties.Item(projProperties).Value.ToString();
                                break;
                            }
                            projProperties++;
                        }

                        //Checking if there are any more project items down in the hierarchy and applying separate logic to compute the full path.
                        if (proj.ProjectItems.Item(projectItemsInProjectLevel1Count).ProjectItems != null && proj.ProjectItems.Item(projectItemsInProjectLevel1Count).ProjectItems.Count!=0)
                        {
                            fileCount = proj.ProjectItems.Item(projectItemsInProjectLevel1Count).ProjectItems.Count;
                            int projectItemsInProjectLevel2Count = 1;
                            while (projectItemsInProjectLevel2Count <= fileCount && proj.ProjectItems.Item(projectItemsInProjectLevel1Count).Name.ToUpperInvariant() != bin)
                            {
                                fileName = proj.ProjectItems.Item(projectItemsInProjectLevel1Count).ProjectItems.Item(projectItemsInProjectLevel2Count).Name;

                                int counter = 1;
                                while (counter <= proj.ProjectItems.Item(projectItemsInProjectLevel1Count).ProjectItems.Item(projectItemsInProjectLevel2Count).Properties.Count)
                                {
                                    if (proj.ProjectItems.Item(projectItemsInProjectLevel1Count).ProjectItems.Item(projectItemsInProjectLevel2Count).Properties.Item(counter).Name == "FullPath")
                                    {
                                        fullPathForFolderItem = proj.ProjectItems.Item(projectItemsInProjectLevel1Count).ProjectItems.Item(projectItemsInProjectLevel2Count).Properties.Item(counter).Value.ToString();
                                        break;
                                    }
                                    counter++;
                                }
                                projectItemsInProjectLevel2Count++;
                                htProjectNames.Add(fullPathForFolderItem.ToUpperInvariant() , Path.GetFileName(relativePath));
                            }
                        }
                        else
                        {
                            //If there are no further project items, then the full path is stored as it is into the hashtable.
                            htProjectNames.Add(fullPathForFileItem.ToUpperInvariant(), Path.GetFileName(relativePath));
                        }
                        projectItemsInProjectLevel1Count++;
                        }
                        projectsInSolutionCount++;
                 }
            }
            //close the dte object.
            dte.Quit();
        }

        /***********************************************************************************************************/
        /// <summary>
        /// Compute the Statistics of the file.
        /// </summary>
        /// <param name="file">An instance of the Infosys.Lif.LegacyWorkbench.ReportManager.File to which all the file 
        /// statistics will be mapped on. </param>
        /// <param name="filetemp">File whose statistics are to be computed</param>
        private void ComputeFileStatistics(File file,FileInfo filetemp)
        {
            int blankLineCount = 0;
            int commentLineCount = 0;
            int locCount = 0;
            int totalLineCount = 0;
            int count = 0;
           
            string[] lines = System.IO.File.ReadAllLines(filetemp.FullName);
            foreach (string line in lines)
            {
                if (line.Trim().Length == 0)
                {
                    blankLineCount++;
                }
                else if (line.Trim().StartsWith("/") || (line.Trim().StartsWith("<!--")) )
                {
                    if (line.Trim().StartsWith("/*") && !(line.Trim().EndsWith("*/")))
                    {
                        //If a multiline comment has started then here only it is counted as a comment.
                        commentLineCount++;
                        count = 1;
                        continue;
                    }
                    
                    if (line.Trim().StartsWith("<!--") && !(line.Trim().EndsWith("-->")))
                    {
                        //If a multiline comment has started then here only it is counted as a comment.
                        commentLineCount++;
                        count = 1;
                        continue;
                    }
                    commentLineCount++;
                }

                //If a multiline comment has started in some previous line and it ends in this line, then value of count is changed to 0.
                else if (count == 1)
                {
                    if ((line.Trim().EndsWith("*/")) || (line.Trim().EndsWith("-->")))
                    {
                        count = 0;
                    }
                }
                else
                {
                    locCount++;
                }
            }
            totalLineCount = blankLineCount + commentLineCount + locCount;
            file.LinesOfCode = locCount.ToString();
            file.CommentsCount = commentLineCount.ToString();
            file.BlankLinesCount = blankLineCount.ToString();
        }

        /***********************************************************************************************************/
        /// <summary>
        /// Create file collection.
        /// </summary>
        /// <param name="dir">The directory containing the collection of files.</param>
        /// <returns>An instance of Infosys.Lif.LegacyWorkbench.ReportManager.FileCollection containing all 
        /// the files in the directory with all the attributes and file statistics added.</returns>
        private Infosys.Lif.LegacyWorkbench.ReportManager.FileCollection CreateFileCollection(DirectoryInfo dir)
        {

            Infosys.Lif.LegacyWorkbench.ReportManager.FileCollection fileCollection = new Infosys.Lif.LegacyWorkbench.ReportManager.FileCollection();
            FileInfo[] fi = dir.GetFiles();
            foreach (FileInfo fiTemp in fi)
            {
                Infosys.Lif.LegacyWorkbench.ReportManager.File file = new File();
                file = AddFileDetails(dir, fiTemp);

                fileCollection.Add(file);
            }

            return fileCollection;
        }
    }
}


