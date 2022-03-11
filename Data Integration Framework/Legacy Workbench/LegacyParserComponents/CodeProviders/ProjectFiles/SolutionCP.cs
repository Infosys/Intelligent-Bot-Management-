using System;
using System.Collections.Generic;
using System.Text;

using Infosys.Solutions.CodeGeneration.Framework;

namespace Infosys.Lif.LegacyWorkbench.CodeProviders.ProjectFiles
{
    internal class SolutionCP : ContentProvider
    {
        Guid solutionGuid;

        [PlaceHolder("SolutionGuid")]
        public string SolutionGuid
        {
            get { return solutionGuid.ToString(); }
        }
        Guid modelObjectDataEntityGuid;

        [PlaceHolder("ModelObjectDataEntityGuid")]
        public string ModelObjectDataEntityGuid
        {
            get { return modelObjectDataEntityGuid.ToString(); }
        }
        string modelObjectDataEntityRootNameSpace;

        [PlaceHolder("ModelObjectDataEntityRootNamespace")]
        public string ModelObjectDataEntityRootNameSpace
        {
            get { return modelObjectDataEntityRootNameSpace; }
        }
        Guid modelObjectSerializerGuid;
        [PlaceHolder("ModelObjectSerializerGuid")]
        public string ModelObjectSerializerGuid
        {
            get { return modelObjectSerializerGuid.ToString(); }
        }
        string modelObjectSerializerRootNameSpace;
        [PlaceHolder("ModelObjectSerializerRootNamespace")]
        public string ModelObjectSerializerRootNameSpace
        {
            get { return modelObjectSerializerRootNameSpace; }
        }
        Guid contractDataEntityGuid;
        [PlaceHolder("ContractDataEntityGuid")]
        public string ContractDataEntityGuid
        {
            get { return contractDataEntityGuid.ToString(); }
        }
        string contractDataEntityRootNameSpace;
        [PlaceHolder("ContractDataEntityRootNamespace")]
        public string ContractDataEntityRootNameSpace
        {
            get { return contractDataEntityRootNameSpace; }
        }
        Guid contractSerializerGuid;
        [PlaceHolder("ContractSerializerGuid")]
        public string ContractSerializerGuid
        {
            get { return contractSerializerGuid.ToString(); }
        }


        //////string hostAccessRootNameSpace;
        //////[PlaceHolder("HostAccessRootNamespace")]
        //////public string HostAccessRootNameSpace
        //////{
        //////    get { return hostAccessRootNameSpace; }
        //////}


        string contractSerializerRootNameSpace;
        [PlaceHolder("ContractSerializerRootNamespace")]
        public string ContractSerializerRootNameSpace
        {
            get { return contractSerializerRootNameSpace; }
        }


        //////Guid hostAccessGuid;
        //////[PlaceHolder("HostAccessGuid")]
        //////string HostAccessGuid
        //////{
        //////    get
        //////    { return hostAccessGuid.ToString(); }
        //////}



        string[] hostAccessRootNameSpaces;
        Guid[] hostAccessGuids;
        [PlaceHolder("HostAccessFilled")]
        string HostAccessFilled
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                Template template
                    = ContentTemplate.RepeatingTemplate("HostAccessTemplate");

                foreach (Guid hostAccessGuid in hostAccessGuids)
                {
                    HostAccessTemplateCP codeGenerator
                        = new HostAccessTemplateCP(hostAccessGuid);
                    codeGenerator.ContentTemplate = template;
                    sb.Append(codeGenerator.GenerateContent());

                }
                return sb.ToString();
            }
        }
        [PlaceHolder("HardCodedStringEmpty")]
        string HardCodedStringEmpty
        {
            get
            {
                return string.Empty;
            }
        }

        [PlaceHolder("ProjectFilled")]
        string ProjectFilled
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                Template template
                    = ContentTemplate.RepeatingTemplate("ProjectTemplate");

                for (int counter = 0; counter < hostAccessGuids.Length; counter++)
                {
                    ProjectTemplateCP codeGenerator
                        = new ProjectTemplateCP(solutionGuid, hostAccessRootNameSpaces[counter], 
                        hostAccessGuids[counter],contractDataEntityGuid);
                    codeGenerator.ContentTemplate = template;
                    sb.Append(codeGenerator.GenerateContent());

                }
                return sb.ToString();
            }
        }

        internal SolutionCP(Guid _solutionGuid,
            Guid _ModelObjectDataEntityGuid, string _ModelObjectDataEntityRootNameSpace,
            Guid _ModelObjectSerializerGuid, string _ModelObjectSerializerRootNameSpace,
                Guid _contractDataEntityGuid, string _contractDataEntityRootNameSpace,
            Guid _contractSerializerGuid, string _contractSerializerRootNameSpace,
            Guid[] _hostAccessGuid, string[] _hostAccessRootNS)
        {

            solutionGuid = _solutionGuid;
            modelObjectDataEntityGuid = _ModelObjectDataEntityGuid;
            modelObjectDataEntityRootNameSpace = _ModelObjectDataEntityRootNameSpace;
            modelObjectSerializerGuid = _ModelObjectSerializerGuid;
            modelObjectSerializerRootNameSpace = _ModelObjectSerializerRootNameSpace;
            contractDataEntityGuid = _contractDataEntityGuid;
            contractDataEntityRootNameSpace = _contractDataEntityRootNameSpace;
            contractSerializerGuid = _contractSerializerGuid;
            contractSerializerRootNameSpace = _contractSerializerRootNameSpace;
            hostAccessGuids = _hostAccessGuid;
            hostAccessRootNameSpaces = _hostAccessRootNS;
        }
    }

    internal class HostAccessTemplateCP : ContentProvider
    {
        Guid projectGuid;
        internal HostAccessTemplateCP(Guid hostAccessProjectGuid)
        {
            projectGuid = hostAccessProjectGuid;
        }

        [PlaceHolder("HostAccessGuid")]
        string HostAccessGuid
        {
            get
            {
                return projectGuid.ToString();
            }
        }
    }


    internal class ProjectTemplateCP : ContentProvider
    {
        Guid _SolutionGuid, _HostAccessGuid, _ContractDataEntityGuid;
        string _HostAccessRootNamespace;
        internal ProjectTemplateCP(Guid solutionGuid,
            string hostAccessRootNamespace, Guid hostAccessGuid, Guid contractDataEntityGuid)
        {
            _SolutionGuid = solutionGuid;
            _HostAccessGuid = hostAccessGuid;
            _HostAccessRootNamespace = hostAccessRootNamespace;
            _ContractDataEntityGuid = contractDataEntityGuid;
        }
        [PlaceHolder("SolutionGuid")]
        string SolutionGuid
        {
            get
            {
                return _SolutionGuid.ToString();
            }
        }
        [PlaceHolder("HostAccessRootNamespace")]
        string HostAccessRootNamespace
        {
            get
            {
                return _HostAccessRootNamespace;
            }
        }
        [PlaceHolder("HostAccessGuid")]
        string HostAccessGuid
        {
            get
            {
                return _HostAccessGuid.ToString();
            }
        }
        [PlaceHolder("ContractDataEntityGuid")]
        string ContractDataEntityGuid
        {
            get
            {
                return _ContractDataEntityGuid.ToString();
            }
        }
    }
}
