using System;
using System.Collections.Generic;
using System.Text;

namespace Infosys.Lif.LegacyWorkbench.Framework
{
    public interface ICodeGenerator
    {
        void Initialize(Entities.Project codeProviderProject, string xsdObjGenPath);
        Entities.Project ProjectToBeGeneratedFor
        {
            get;
        }
        void GenerateModelObjectDataEntities();
        void GenerateContractDataEntities();
        void GenerateModelObjectSerializers();
        void GenerateContractSerializers();
        void CleanUp();
        bool IsErrorOccurred
        {
            get;
            set;
        }
        string ErrorReason
        {
            get;
            set;
        }
    }
}
