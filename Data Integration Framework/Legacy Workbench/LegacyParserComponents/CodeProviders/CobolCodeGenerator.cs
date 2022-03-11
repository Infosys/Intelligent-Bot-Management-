using System;
using System.Collections.Generic;
using System.Text;
using Infosys.Lif.LegacyWorkbench.Framework;
using Infosys.Solutions.CodeGeneration.Framework;

namespace Infosys.Lif.LegacyWorkbench.CodeProviders
{
    class CobolCodeGenerator : ICodeGenerator
    {
        CodeProviders.CodeGeneratorBase CodeGenBase = new CodeGeneratorBase();

        bool isErrorOccurred = false;

        public bool IsErrorOccurred
        {
            get { return isErrorOccurred; }
            set { isErrorOccurred = value; }
        }

        string errorReason;

        public string ErrorReason
        {
            get { return errorReason; }
            set { errorReason = value; }
        }

        Entities.Project projectToBeGeneratedFor;
        public Infosys.Lif.LegacyWorkbench.Entities.Project ProjectToBeGeneratedFor
        {
            get
            {
                return projectToBeGeneratedFor;
            }
        }

        public CobolCodeGenerator()
        {
            
        }

        public void Initialize(Entities.Project codeProviderProject, string xsdObjGenPath)
        {
            CodeGenBase.InitializeBase(codeProviderProject, xsdObjGenPath);
            CheckError(CodeGenBase);
        }

        public void GenerateModelObjectDataEntities()
        {
            CodeGenBase.GenerateModelObjectDataEntitiesBase_cobol();
            CheckError(CodeGenBase);
        }

        public void GenerateContractDataEntities()
        {
            CodeGeneratorBase.IsTins = false;
            CodeGenBase.GenerateContractDataEntitiesBase();
            CheckError(CodeGenBase);
        }
        
        public void GenerateModelObjectSerializers()
        {
            CodeGenBase.GenerateModelObjectSerializersBase_cobol();
            CheckError(CodeGenBase);
        }


        public void GenerateContractSerializers()
        {
            CodeGenBase.GenerateContractSerializersBase_cobol();
            CheckError(CodeGenBase);
        }
        

        public void CleanUp()
        {
            CodeGenBase.CleanUpBase();
            CheckError(CodeGenBase);
        }

        private void CheckError(CodeGeneratorBase CodeGenBase)
        {
            if (CodeGenBase.IsErrorOccurred)
            {
                this.IsErrorOccurred = true;
                this.ErrorReason = CodeGenBase.ErrorReason;
            }
        }

    }
}

