using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Infosys.Lif.LegacyWorkbench.Framework;
using Infosys.Solutions.CodeGeneration.Framework;

namespace Infosys.Lif.LegacyWorkbench.CodeProviders
{
    public class TINSCodeGenerator : ICodeGenerator
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

        Entities.Project projectToBeGeneratedFor = null; 
        public Infosys.Lif.LegacyWorkbench.Entities.Project ProjectToBeGeneratedFor
        {
            get
            {
                return projectToBeGeneratedFor;
            }
        }        
        
        public TINSCodeGenerator()
        {
            templatesDirectory = System.Windows.Forms.Application.StartupPath + @"\Templates\";

            string workingDirectory = System.Environment.GetEnvironmentVariable("windir");
            DotNetFrameworkDirectory = workingDirectory + @"\Microsoft.NET\Framework\v2.0.50727\";            
        }

        string DotNetFrameworkDirectory;

        string templatesDirectory;
        

        public void Initialize(Entities.Project codeProviderProject, string xsdObjGenPath)
        {
            CodeGenBase.InitializeBase(codeProviderProject, xsdObjGenPath);
        }

        public void GenerateModelObjectDataEntities()
        {
            CodeGenBase.GenerateModelObjectDataEntitiesBase_tins();
        }

        public void GenerateContractDataEntities()
        {
            CodeGeneratorBase.IsTins = true;
            CodeGenBase.GenerateContractDataEntitiesBase();
        }

        public void GenerateModelObjectSerializers()
        {
            CodeGenBase.GenerateModelObjectSerializersBase_tins();            
        }

        public void GenerateContractSerializers()
        {
            CodeGenBase.GenerateContractSerializersBase_tins();
        }

        public void CleanUp()
        {
            CodeGenBase.CleanUpBase();
        }
        
    }
}