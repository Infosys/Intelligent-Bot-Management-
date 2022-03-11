using System;
using System.Collections.Generic;
using System.Text;

using System.IO;

namespace Infosys.Lif.LegacyWorkbench
{
    public class SmallTalkConverter:Framework.IModelObjectRetriever
    {
        #region Declare Constants
        public const string goosecodeforobject = "goose code for object";
        public const string shortidentifier = "shortidentifier";
        public const string symbolA = "^'";
        public const string symbolB = "'";
        public const string clauseGetDefinition = "clauseGetDefinition";
        public const string symbolC = "^#(";
        public const string symbolD = "))";
        public const string symbolE = ")";
        public const string symbolF = "(";
        public const string asA = "as";
        public const string StringA = "String";
        public const string dateymd = "dateymd";
        public const string booleanA = "boolean";
        public const string alpha = "alpha";
        public const string integerA = "integer";
        public const string floatA = "float";
        public const string timestamp = "timestamp";
        #endregion
        private Entities.Entity Translate(string fileName)
        {
            
            Entities.Entity entityBeingBuilt = new Entities.Entity();
            entityBeingBuilt.EntityName = null;
            StreamReader sr2;
            try
            {
                sr2 = new StreamReader(fileName);
            }
            catch (FileNotFoundException filenotFoundException)
            {
                Framework.LegacyParserException lpException
                    = new Framework.LegacyParserException();
                lpException.ErrorReason
                    = Framework.LegacyParserException.ErrorReasonCode.DataDefinitionNotFound;
                lpException.PlaceHolder.Add(fileName);
                throw lpException;
            }
            ////////string ProgramId;


            while (!sr2.EndOfStream)
            {

                string Current_Line = sr2.ReadLine();
                if (Current_Line.Trim().ToLowerInvariant().Contains(goosecodeforobject) && entityBeingBuilt.EntityName == null)
                {
                    char[] delimeters = new char[1];
                    delimeters[0] = ' ';
                    entityBeingBuilt.EntityName = Current_Line.Split(delimeters, StringSplitOptions.RemoveEmptyEntries)[4];
                }
                else if (Current_Line.Trim().ToLowerInvariant().Equals(shortidentifier))
                {
                    while (!sr2.EndOfStream)
                    {
                        Current_Line = sr2.ReadLine();
                        if (Current_Line.Trim().Contains(symbolA))
                        {
                            int end = Current_Line.LastIndexOf(symbolB);
                            int start = Current_Line.IndexOf(symbolB);
                            entityBeingBuilt.ProgramId = Current_Line.Substring(start + 1, (end - start - 1));
                            break;
                        }
                    }
                }
                else if (Current_Line.Trim().Equals(clauseGetDefinition))
                {
                    while (!sr2.EndOfStream)
                    {
                        Current_Line = sr2.ReadLine();
                        if (Current_Line.Trim().Contains(symbolC))
                        {
                            bool flag = false;
                            while (!sr2.EndOfStream)
                            {
                                Current_Line = sr2.ReadLine();
                                if (Current_Line.Trim().Contains(symbolD))
                                {
                                    flag = true;
                                }

                                int end = Current_Line.IndexOf(symbolE);
                                int start = Current_Line.LastIndexOf(symbolF);
                                string dataItemString;
                                if (Current_Line.Length >= (end) && end > -1)
                                {
                                    dataItemString
                                        = Current_Line.Substring(start + 1, (end - start - 1));
                                }
                                else
                                {
                                    Framework.LegacyParserException lpException
                                        = new Framework.LegacyParserException();
                                    lpException.ErrorReason
                                        = Framework.LegacyParserException.ErrorReasonCode.IncorrectDataItemDefinition;
                                    lpException.PlaceHolder.Add(fileName);
                                    throw lpException;
                                }


                                entityBeingBuilt.DataItems.Add(BuildDataItem(dataItemString));

                                if (flag == true)
                                {
                                    break;
                                }
                            }
                            break;
                        }
                    }
                }
            }
            sr2.Close();
            return entityBeingBuilt;
        }

        private Entities.DataItem BuildDataItem(string dataItemString)
        {
            Entities.DataItem dataItem = new Entities.DataItem();
            char[] delimiters = new char[2];
            delimiters[0] = ' ';
            delimiters[1] = '\t';
            dataItemString = dataItemString.Trim();
            string[] splitStrings
                = dataItemString.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
            dataItem.ItemName = splitStrings[0];
            PopulateDataItem(splitStrings[1], dataItem);
            return dataItem;
        }

        private void PopulateDataItem(string dataTypeString,
            Entities.DataItem dataItemToBePopulated)
        {
            int lengthOfMiddle
                = dataTypeString.Length - asA.Length - StringA.Length;
            dataTypeString = dataTypeString.Substring(2, lengthOfMiddle);
            dataTypeString = dataTypeString.ToLowerInvariant();
            switch (dataTypeString)
            {
                case dateymd:
                    dataItemToBePopulated.ItemType = Entities.DataItem.DataItemType.Date;
                    dataItemToBePopulated.Length = 10;
                    break;
                case booleanA:
                    dataItemToBePopulated.ItemType = Entities.DataItem.DataItemType.Boolean;
                    dataItemToBePopulated.Length = 1;
                    break;
                default:
                    if (dataTypeString.ToLowerInvariant().StartsWith(alpha))
                    {
                        dataItemToBePopulated.ItemType
                            = Entities.DataItem.DataItemType.String;
                        dataTypeString
                            = dataTypeString.Substring(alpha.Length);
                        dataItemToBePopulated.Length
                            = Convert.ToInt32(dataTypeString);
                    }
                    else if (dataTypeString.ToLowerInvariant().StartsWith(integerA))
                    {
                        //dataItemToBePopulated.ItemType
                        //    = Entities.DataItem.DataItemType.SignedIntegerType;
                        dataTypeString
                            = dataTypeString.Substring(integerA.Length);
                        dataItemToBePopulated.Length
                            = Convert.ToInt32(dataTypeString);
                    }
                    else if (dataTypeString.ToLowerInvariant().StartsWith(floatA))
                    {
                        dataItemToBePopulated.ItemType
                            = Entities.DataItem.DataItemType.Float;
                        dataTypeString
                            = dataTypeString.Substring(floatA.Length);
                        ////dataItemToBePopulated.Length
                        ////    = Convert.ToInt32(dataTypeString);
                    }
                    else if (dataTypeString.ToLowerInvariant().StartsWith(timestamp))
                    {
                        dataItemToBePopulated.ItemType
                            = Entities.DataItem.DataItemType.String;
                        dataTypeString
                            = dataTypeString.Substring(timestamp.Length);
                        ////dataItemToBePopulated.Length
                        ////    = Convert.ToInt32(dataTypeString);
                    }
                    break;
            }
        }

        #region IModelObjectRetriever Members

        public Infosys.Lif.LegacyWorkbench.Entities.Entity RetrieveModelObjectDetails(string filePath)
        {
            return Translate(filePath);
        }

        #endregion
    }
}
