using System;
using System.Collections.Generic;
using System.Text;

// The code generator requires this.
using Infosys.Solutions.CodeGeneration.Framework;

// The Legacy Parser specific namespace.
using ParserGen = Infosys.Lif.LegacyParser;

/****************************************************************
 * This file is a part of the Legacy Parser utility.
 * This is utilized to provide content to templates for rendering the csproj/vbproj file 
 * and the sln file.
 * This is specific to VS 2005. 
 * Copyright (c) 2003 - 2005 Infosys Technologies Ltd. All Rights Reserved.
 ***************************************************************/
namespace Infosys.Lif.LegacyParser.ContentProviders
{
    /// <summary>
    /// The class which provides content to the template of the csproj file and sln file.
    /// </summary>
    public class Vs2005Project : ContentProvider
    {

        /// <summary>
        /// The file being generated may be of 3 types.
        /// </summary>
        public enum ProjectType
        {
            /// <summary>
            /// The file being generated is the csproj file for the DataEntity files.
            /// </summary>
            DataEntityProject,
            /// <summary>
            /// The file being generated is the csproj file for the Serializer files.
            /// </summary>
            SerializerProject,

            ServiceComponentProject,

            UnitTestCaseProject,

            /// <summary>
            /// The file being generated is the sln file.
            /// </summary>
            SolutionType
        }
        /// <summary>
        /// The project entity for which this is being generated. 
        /// This will be required to render the root namespace, assembly name etc.
        /// </summary>
        ParserGen.Project projectToBeGeneratedFor;

        /// <summary>
        /// decides whether this is being used for data entity or serializers or sln files.
        /// </summary>
        ProjectType projectTypeBeingGenerated = ProjectType.DataEntityProject;


        /// <summary>
        /// Teh constructor for this entity. 
        /// </summary>
        /// <param name="projectItem">the Project entity for which this is being generated. 
        /// Will be used to render the root namespace, the assembly name etc.</param>
        /// <param name="projectType">The type of project for which this file is being generated.</param>
        public Vs2005Project(ParserGen.Project projectItem, ProjectType projectType)
        {
            //New Guid for each project.
            _dataEntityGuid = System.Guid.NewGuid();
            _serializerGuid = System.Guid.NewGuid();
            _serviceComponentGuid = System.Guid.NewGuid();
            //////_unitTestCasesGuid = System.Guid.NewGuid();
            _solutionGuid = System.Guid.NewGuid();


            projectToBeGeneratedFor = projectItem;
            projectTypeBeingGenerated = projectType;
        }

        /// <summary>
        /// This holds the Guid for the data entity project.
        /// </summary>
        Guid _dataEntityGuid;

        /// <summary>
        /// This holds the Guid for the serializers project.
        /// </summary>
        Guid _serializerGuid;

        Guid _serviceComponentGuid;
        //////Guid _unitTestCasesGuid;

        /// <summary>
        /// This holds the Guid for the solution file.
        /// </summary>
        Guid _solutionGuid;

        /// <summary>
        /// This can be used to retrieve the Guid for the Data Entity project.
        /// </summary>
        [PlaceHolder("DataEntityGuid")]
        string DataEntityGuid
        {
            get
            {
                return _dataEntityGuid.ToString().ToUpperInvariant();
            }
        }

        /// <summary>
        /// This can be used to retrieve the Guid for the solution.
        /// </summary>
        [PlaceHolder("SolutionGuid")]
        string SolutionGuid
        {
            get
            {
                return _solutionGuid.ToString().ToUpperInvariant();
            }
        }

        /// <summary>
        /// This can be used to retrieve the Guid for the Serializers project.
        /// </summary>
        [PlaceHolder("ServiceComponentGuid")]
        string ServiceComponentGuid
        {
            get
            {
                return _serviceComponentGuid.ToString().ToUpperInvariant();
            }
        }


        /// <summary>
        /// This can be used to retrieve the Guid for the Serializers project.
        /// </summary>
        [PlaceHolder("SerializerGuid")]
        string SerializerGuid
        {
            get
            {
                return _serializerGuid.ToString().ToUpperInvariant();
            }
        }

        /// <summary>
        /// This retrieves the Root name space (and assembly name for the 
        /// Parsers project.
        /// </summary>
        [PlaceHolder("SerializerRootNamespace")]
        string SerializerRootNamespace
        {
            get
            {
                return projectToBeGeneratedFor.SerializerRootNamespace;
            }
        }


        [PlaceHolder("ServiceComponentRootNameSpace")]
        string ServiceComponentRootNameSpace
        {
            get
            {
                return projectToBeGeneratedFor.ServiceComponentRootNameSpace;
            }
        }

        [PlaceHolder("UnitTestCasesRootNameSpace")]
        string UnitTestCasesRootNameSpace
        {
            get
            {
                return projectToBeGeneratedFor.UnitTestCasesRootNameSpace;
            }
        }

        //////[PlaceHolder("UnitTestCasesGuid")]
        //////string UnitTestCasesGuid
        //////{
        //////    get
        //////    {
        //////        return _unitTestCasesGuid.ToString().ToUpperInvariant();
        //////    }
        //////}


        /// <summary>
        /// This retrieves the Root namespace and assembly name for the Data Entity project.
        /// </summary>
        [PlaceHolder("DataEntityRootNamespace")]
        string DataEntityRootNamespace
        {
            get
            {
                return projectToBeGeneratedFor.DataEntityRootNamespace;
            }
        }

        /// <summary>
        /// This is used to fill up the FileIncludes template in the template file.
        /// This will return a string including all the files which have to 
        /// be included in the project.
        /// </summary>
        [PlaceHolder("FileIncludes")]
        string FileIncludes
        {
            get
            {
                // If the file being generated is a sln file, no file needs to be included. 
                // So ignore by return ing empty string.
                if (projectTypeBeingGenerated == ProjectType.SolutionType)
                {
                    return string.Empty;
                }

                // The string which has to replace the FileIncludes placeholder.
                string strBuilt = string.Empty;

                // Loop the modules and build up the string for including all the files 
                // for this project.
                for (int moduleLooper = 0;
                    moduleLooper < projectToBeGeneratedFor.Modules.Count;
                    moduleLooper++)
                {
                    LegacyParserModule currModule = projectToBeGeneratedFor.Modules[moduleLooper];

                    if (projectTypeBeingGenerated == ProjectType.ServiceComponentProject)
                    {
                        FileItem fileItem = new FileItem();
                        fileItem.ContentTemplate =
                            ContentTemplate.RepeatingTemplate(FileItem.ContentProvidedFor);
                        fileItem.FilePath =
                            currModule.Name + "\\ServiceComponent.cs";
                        strBuilt += fileItem.GenerateContent();
                    }
                    else if (projectTypeBeingGenerated == ProjectType.UnitTestCaseProject)
                    {
                        FileItem fileItem = new FileItem();
                        fileItem.ContentTemplate =
                            ContentTemplate.RepeatingTemplate(FileItem.ContentProvidedFor);
                        fileItem.FilePath =
                            currModule.Name + "\\TestCases.cs";
                        strBuilt += fileItem.GenerateContent();
                    }
                    else
                    {
                        // Loop through all the entities included in this project. 
                        for (int entityLooper = 0; entityLooper < currModule.Entities.Count;
                            entityLooper++)
                        {
                            Entity currEntity = currModule.Entities[entityLooper];

                            FileItem fileItem = new FileItem();
                            fileItem.ContentTemplate =
                                ContentTemplate.RepeatingTemplate(FileItem.ContentProvidedFor);


                            // The project type Solution has already been handled intiially.
                            switch (projectTypeBeingGenerated)
                            {
                                case ProjectType.DataEntityProject:
                                    fileItem.FilePath =
                                        currModule.DataEntityNamespace + "\\" + currEntity.DataEntityClassName + ".cs";
                                    break;
                                case ProjectType.SerializerProject:
                                    fileItem.FilePath = currModule.SerializerNamespace + "\\" + currEntity.SerializerClassName + ".cs";
                                    break;
                            }
                            strBuilt += fileItem.GenerateContent();
                        }
                    }
                }
                return strBuilt;
            }
        }

        bool _includeServiceComponentsInSolution = false;

        public bool IncludeServiceComponentsInSolution
        {
            get
            {
                return _includeServiceComponentsInSolution;
            }
            set
            {
                _includeServiceComponentsInSolution = value;
            }
        }
        [PlaceHolder("ServiceComponentProject")]
        public string ServiceComponentProject
        {
            get
            {
                if (_includeServiceComponentsInSolution)
                {
                    ServiceComponentProjectData projGenerator = new ServiceComponentProjectData();
                    projGenerator.ContentTemplate = ContentTemplate.RepeatingTemplate("ServiceComponentProjectData");
                    projGenerator.ServiceComponentGuid = ServiceComponentGuid;
                    projGenerator.ServiceComponentRootNameSpace = ServiceComponentRootNameSpace;
                    projGenerator.SolutionGuid = SolutionGuid;
                    ////////projGenerator.UnitTestCasesGuid = UnitTestCasesGuid;
                    projGenerator.UnitTestCasesRootNameSpace = UnitTestCasesRootNameSpace;
                    return projGenerator.GenerateContent();
                }
                return string.Empty;
            }
        }
        [PlaceHolder("ServiceComponentProjectSection")]
        public string ServiceComponentProjectSection
        {
            get
            {
                if (_includeServiceComponentsInSolution)
                {
                    ServiceComponentProjectData projGenerator = new ServiceComponentProjectData();
                    projGenerator.ContentTemplate = ContentTemplate.RepeatingTemplate("ServiceComponentProjectSectionData");
                    projGenerator.ServiceComponentGuid = ServiceComponentGuid;
                    projGenerator.ServiceComponentRootNameSpace = ServiceComponentRootNameSpace;
                    projGenerator.SolutionGuid = SolutionGuid;
                    //////////projGenerator.UnitTestCasesGuid = UnitTestCasesGuid;
                    projGenerator.UnitTestCasesRootNameSpace = UnitTestCasesRootNameSpace;
                    return projGenerator.GenerateContent();
                }
                return string.Empty;
            }
        }


        class ServiceComponentProjectData : ContentProvider
        {
            string _solutionGuid;
            [PlaceHolder("SolutionGuid")]
            public string SolutionGuid
            {
                get
                {
                    return _solutionGuid;
                }
                set
                {
                    _solutionGuid = value;
                }
            }
            string _serviceComponentGuid;
            [PlaceHolder("ServiceComponentGuid")]
            public string ServiceComponentGuid
            {
                get
                {
                    return _serviceComponentGuid;
                }
                set
                {
                    _serviceComponentGuid = value;
                }
            }
            string _serviceComponentRootNameSpace;
            [PlaceHolder("ServiceComponentRootNameSpace")]
            public string ServiceComponentRootNameSpace
            {
                get
                {
                    return _serviceComponentRootNameSpace;
                }
                set
                {
                    _serviceComponentRootNameSpace = value;
                }
            }
            //////string _unitTestCasesGuid;
            //////[PlaceHolder("UnitTestCasesGuid")]
            //////public string UnitTestCasesGuid
            //////{
            //////    get
            //////    {
            //////        return _unitTestCasesGuid;
            //////    }
            //////    set
            //////    {
            //////        _unitTestCasesGuid = value;
            //////    }
            //////}
            string _unitTestCasesRootNameSpace;
            [PlaceHolder("UnitTestCasesRootNameSpace")]
            public string UnitTestCasesRootNameSpace
            {
                get
                {
                    return _unitTestCasesRootNameSpace;
                }
                set
                {
                    _unitTestCasesRootNameSpace = value;
                }
            }

        }

        /// <summary>
        /// This class enables us to include all files belonging to the current project.
        /// This provides content for EACH file which has to be included.
        /// </summary>
        internal class FileItem : ContentProvider
        {
            /// <summary>
            /// The repeating template for which this class will provide content for.
            /// </summary>
            static internal string ContentProvidedFor = "FileInclude";

            /// <summary>
            /// The path to the file.
            /// </summary>
            string _filePath = string.Empty;

            /// <summary>
            /// This provides content for the path to the file which 
            /// has to be included in the project.
            /// </summary>
            [PlaceHolder("FileName")]
            internal string FilePath
            {
                get
                {
                    return _filePath;
                }
                set
                {
                    _filePath = value;
                }
            }

        }
    }
}
