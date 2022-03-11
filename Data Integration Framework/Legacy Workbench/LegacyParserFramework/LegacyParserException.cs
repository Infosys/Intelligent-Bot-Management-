using System;
using System.Collections.Generic;
using System.Text;

namespace Infosys.Lif.LegacyWorkbench.Framework
{
    public class LegacyParserException : Exception
    {
        ErrorReasonCode errorReason;

        public ErrorReasonCode ErrorReason
        {
            get { return errorReason; }
            set { errorReason = value; }
        }

        Entities.GenericCollection<object> placeHolder
            = new Entities.GenericCollection<object>();

        public Entities.GenericCollection<object> PlaceHolder
        {
            get { return placeHolder; }
            set { placeHolder = value; }
        }


        public override string Message
        {
            get
            {
                string message;
                switch(errorReason)
                {
                    case ErrorReasonCode.IncorrectDataItemDefinition:
                        message = "Error Occurred while importing Data Definition from {0}. No data Items found";
                        break;
                    case ErrorReasonCode.DataDefinitionNotFound:
                        message = "Error Occurred while importing Data Definition from {0}. File Not found.";
                        break;

                    case ErrorReasonCode.ProgramIdNotFound:
                        message = "Program id not defined in file {0}";
                        break;
                    default:
                        message = "Unexpected error Occured";
                        break;
                }

                object[] objParams= new object[placeHolder.Count];
                placeHolder.CopyTo(objParams);
                return string.Format(message, objParams);
            }
        }

        public enum ErrorReasonCode
        {
            IncorrectDataItemDefinition,
            DataDefinitionNotFound,
            ErrorsDuringModelObjectRetrieval,
            ProgramIdNotFound,
            IncorrectRetrieverType
        }
    }
}
