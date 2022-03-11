using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

/****************************************************************
 * This file is a part of the Legacy Parser utility.
 * This is the COBOL specific implementation of the ILanguageConverter interface.
 * This implementation reads COBOL copy books and converts it to parser 
 * generator specific .NET type.
 * Copyright (c) 2003 - 2005 Infosys Technologies Ltd. All Rights Reserved.
 ***************************************************************/
namespace Infosys.Lif.LegacyParser.LanguageConverters
{
    /// <summary>
    /// Objects of this type will be used to convert the COBOL language 
    /// mainframe copy books to Legacy Parser specific .NET entity type.
    /// </summary>
    public class Cobol : Interfaces.ILanguageConverter
    {

        /// <summary>
        /// Used to maintain the current token position in the COBOL copy book file.
        /// </summary>
        private int currentIndex;

        /// <summary>
        /// The lines read from the input file are split according to the "."
        /// If there are more than one "." it is considered as multiple lines.
        /// </summary>
        private string[] allReadLines;

        /// <summary>
        /// The characters with which the strings should be trimmed. 
        /// These are unwanted characters at the beginning/end of the string.
        /// </summary>
        private char[] trimCharacters ={ ' ', '\t', '\0', '\r', '\n' };


        #region ILanguageConverter Members

        /// <summary>
        /// This is the method which will be used to translate a cobol file 
        /// into the legacy parser entities which the code generator understands.
        /// </summary>
        /// <param name="pathOfCobolCopyBook">This should be the path to the COBOL copy book.</param>
        /// <returns>Legacy parser specific .NET entity type.</returns>
        public Entity Translate(string pathOfCobolCopyBook)
        {
            lengthCalculator = 0;
            CobolFileParser fileParser = new CobolFileParser(pathOfCobolCopyBook);
            string linkageSection = fileParser.LinkageSection;
            string[] splitters ={ Environment.NewLine };
            string[] splitLinkageSection = linkageSection.Split(splitters, StringSplitOptions.RemoveEmptyEntries);
            Entity currEntity = new Entity();

            {
                currEntity.EntityName = fileParser.programId;
                currEntity.DataEntityClassName = ParseToCorrectCase(fileParser.programId);
                currEntity.SerializerClassName = ParseToCorrectCase(fileParser.programId);
                currEntity.ProgramId = fileParser.programId;

            }
            //currEntity.DataItems = new GenericCollection<DataItem>();

            currEntity.DataItems.Add(new DataItem());
            currEntity.DataItems[0].ItemType = DataItem.DataItemType.GroupItemType;
            currEntity.DataItems[0].ItemName = "LINKAGE SECTION";

            DeclarationHelper levelOnehelper = new DeclarationHelper();
            levelOnehelper.DeclarationLevel = 0;
            levelOnehelper.declarationItem = currEntity.DataItems[0];

            Stack<DeclarationHelper> hierarchicalQueue = new Stack<DeclarationHelper>();
            hierarchicalQueue.Push(levelOnehelper);

            for (int looper = 0; looper < splitLinkageSection.Length; looper++)
            {
                char[] declarationSplitter = { ' ' };
                string[] currDeclarationTokens = splitLinkageSection[looper].Split(declarationSplitter, StringSplitOptions.RemoveEmptyEntries);
                int currLevelOfDeclaration = 0;
                DeclarationHelper prevItem = hierarchicalQueue.Peek();
                if (Int32.TryParse(currDeclarationTokens[0], out currLevelOfDeclaration))
                {
                    if (prevItem.DeclarationLevel < currLevelOfDeclaration)
                    {
                        DataItem dataItem;

                        if (currDeclarationTokens.Length > 2 && currDeclarationTokens[2].ToLowerInvariant().Equals("pic"))
                        {
                            dataItem = BuildDataItem(currDeclarationTokens);
                        }
                        else
                        {
                            dataItem = new DataItem();
                        }


                        dataItem.NumberOfOccurences = RetrieveNumberOfOccurences(splitLinkageSection[looper], currDeclarationTokens, dataItem);
                        if (prevItem.declarationItem.Direction != DataItem.ParameterDirectionType.InputAndOutput)
                        {
                            dataItem.Direction = prevItem.declarationItem.Direction;
                        }

                        prevItem.declarationItem.GroupItems.Add(dataItem);
                        prevItem.declarationItem.ItemType = DataItem.DataItemType.GroupItemType;
                        //dataItem = BuildDataItem(currDeclarationTokens);
                        dataItem.ItemName = ParseToCorrectCase(currDeclarationTokens[1]);

                        DeclarationHelper helperObj = new DeclarationHelper();
                        helperObj.DeclarationLevel = currLevelOfDeclaration;
                        helperObj.declarationItem = dataItem;
                        hierarchicalQueue.Push(helperObj);
                    }
                    else
                    {
                        prevItem = hierarchicalQueue.Pop();
                        while (true)
                        {
                            prevItem = hierarchicalQueue.Peek();
                            if (prevItem.DeclarationLevel >= currLevelOfDeclaration)
                            {
                                prevItem = hierarchicalQueue.Pop();
                            }
                            else
                            {
                                break;
                            }
                        }
                        prevItem.declarationItem.ItemType = DataItem.DataItemType.GroupItemType;
                        DataItem dataItem;
                        if (currDeclarationTokens.Length > 2 && currDeclarationTokens[2].ToLowerInvariant().Equals("pic"))
                        {
                            dataItem = BuildDataItem(currDeclarationTokens);
                        }
                        else
                        {
                            dataItem = new DataItem();
                        }

                        if (splitLinkageSection[looper].ToLowerInvariant().IndexOf("redefines") > -1)
                        {
                            BuildRedefines(dataItem, prevItem.declarationItem, currDeclarationTokens);
                        }

                        dataItem.NumberOfOccurences = RetrieveNumberOfOccurences(splitLinkageSection[looper], currDeclarationTokens, dataItem);
                        dataItem.ItemName = ParseToCorrectCase(currDeclarationTokens[1]);
                        if (prevItem.declarationItem.Direction != DataItem.ParameterDirectionType.InputAndOutput)
                        {
                            dataItem.Direction = prevItem.declarationItem.Direction;
                        }
                        prevItem.declarationItem.GroupItems.Add(dataItem);
                        prevItem.declarationItem.ItemType = DataItem.DataItemType.GroupItemType;
                        

                        DeclarationHelper helperObj = new DeclarationHelper();
                        helperObj.DeclarationLevel = currLevelOfDeclaration;
                        helperObj.declarationItem = dataItem;
                        hierarchicalQueue.Push(helperObj);

                    }
                }
            }
            return currEntity;
            #region Old Code
            //// The legacy parser specific entity type.
            ////Entity translatedEntity = new Entity();

            ////// Open the file to be read as a stream
            ////StreamReader fileReader =
            ////    new StreamReader(pathOfCobolCopyBook);

            ////// will be set to true if the next key word will be the program id.
            ////// this will be decided by searching for the keyword (Program-id).
            ////bool isNextLineEntityName = false;

            ////// Used to locate the position of the data item in the copy book.
            ////int lengthCalculator = 0;


            ////int currentDepthOfDeclaration = 0;

            ////// Loop till end of the file.
            ////while (!fileReader.EndOfStream)
            ////{
            ////}
            ////////{
            ////////    // Call the private method to retrieve a token from the file.
            ////////    string readLine = ReadLine(fileReader);

            ////////    // If the previous keyword was program-id it means the next 
            ////////    // keyword is the program id.
            ////////    if (isNextLineEntityName)
            ////////    {
            ////////        translatedEntity.EntityName = readLine;
            ////////        translatedEntity.DataEntityClassName = ParseToCorrectCase(readLine);
            ////////        translatedEntity.SerializerClassName = ParseToCorrectCase(readLine);
            ////////        translatedEntity.ProgramId = readLine;
            ////////        isNextLineEntityName = false;
            ////////    }
            ////////    else if (readLine.StartsWith("PROGRAM-ID"))
            ////////    {
            ////////        isNextLineEntityName = true;
            ////////    }
            ////////    else if (!isCommented(readLine))
            ////////    {
            ////////        // The code has not been commented (should not be ignored).
            ////////        string[] tokens =
            ////////            readLine.Split(
            ////////            trimCharacters, StringSplitOptions.RemoveEmptyEntries);


            ////////        // Split tokens will be as indicated below.
            ////////        // Sample:
            ////////        //25
            ////////        //KICIN-Trial-1
            ////////        //PIC
            ////////        //9(9)
            ////////        tokens[0] = RectifyReadString(tokens[0]);


            ////////        // This is COBOL equivalent of an enumeration.
            ////////        if (tokens[0].Equals("88"))
            ////////        {

            ////////            // Eg: The below defines a Trial-Value variable which has only 
            ////////            // 2 options possible, '10' and '11'.
            ////////            //      01 Trial-Value pic x(2) 
            ////////            //          88  Value-Of-Ten        value '10'  <-- Language converter is currently here
            ////////            //          88  Value-Of-Eleven     value '11'  <-- or here.

            ////////            //Get the previously added data type and make it 
            ////////            //enum type
            ////////            DataItem itemToBeMadeEnum =
            ////////                translatedEntity.DataItems[translatedEntity.DataItems.Count - 1];


            ////////            itemToBeMadeEnum.ItemType = DataItem.DataItemType.EnumType;

            ////////            itemToBeMadeEnum.EnumName = itemToBeMadeEnum.ItemName;

            ////////            // Add the enum to the Entity collection. This will enable us to define 
            ////////            // the various enums in the data entity.
            ////////            GenericCollection<EnumType> enumCollection = translatedEntity.Enums;


            ////////            EnumType enumTypeToBeModified = null;
            ////////            for (int itemCounter = 0; itemCounter < enumCollection.Count;
            ////////                itemCounter++)
            ////////            {
            ////////                EnumType enumType = enumCollection[itemCounter];
            ////////                if (enumType.Name.Equals(itemToBeMadeEnum.EnumName))
            ////////                {
            ////////                    enumTypeToBeModified = enumType;
            ////////                    break;
            ////////                }
            ////////            }
            ////////            // The enum was not found so create the enum type.
            ////////            if (enumTypeToBeModified == null)
            ////////            {
            ////////                enumTypeToBeModified = new EnumType();
            ////////                enumTypeToBeModified.Name = itemToBeMadeEnum.EnumName;
            ////////                translatedEntity.Enums.Add(enumTypeToBeModified);
            ////////            }
            ////////            // Add the enum value just found in the copy book.
            ////////            EnumPropertyType enumProperty = new EnumPropertyType();


            ////////            tokens[1] = RectifyReadString(tokens[1]);
            ////////            tokens[3] = RectifyReadString(tokens[3]);


            ////////            enumProperty.Name = ParseToCorrectCase(tokens[1]);
            ////////            enumProperty.Value = ParseToCorrectCase(tokens[3]);


            ////////            enumTypeToBeModified.EnumProperties.Add(enumProperty);
            ////////        }
            ////////        else if (tokens.Length >= 4)
            ////////        {
            ////////            // A variable is being declared.
            ////////            // Eg: 01 Variable-Name Pic x(9)    Value   'Whatever'.

            ////////            // Rectify the token (variable name).
            ////////            tokens[2] = RectifyReadString(tokens[2]);

            ////////            if (tokens[2].ToLowerInvariant() == "pic")
            ////////            {
            ////////                DataItem oneItem = new DataItem();

            ////////                // Set the variable name 
            ////////                oneItem.ItemName = ParseToCorrectCase(tokens[1]);

            ////////                // Split the x(9) token.
            ////////                char[] splitters = { '(', ')' };
            ////////                string[] splitToken = tokens[3].Split(splitters,
            ////////                    StringSplitOptions.RemoveEmptyEntries);

            ////////                // Check whether the type is string type or integer type.
            ////////                // It cannot be determined whether the value is an enum 
            ////////                // type right now. (This is done on the parsng of the next line).
            ////////                if (splitToken[0].StartsWith("x",
            ////////                    StringComparison.InvariantCultureIgnoreCase))
            ////////                {
            ////////                    oneItem.ItemType = DataItem.DataItemType.StringType;
            ////////                }
            ////////                else
            ////////                {
            ////////                    if (splitToken[0].StartsWith("-",
            ////////                        StringComparison.InvariantCultureIgnoreCase))
            ////////                    {
            ////////                        oneItem.ItemType = DataItem.DataItemType.SignedIntegerType;
            ////////                    }
            ////////                    else
            ////////                    {
            ////////                        oneItem.ItemType = DataItem.DataItemType.UnsignedIntegerType;
            ////////                    }
            ////////                }


            ////////                //If only x is mentioned
            ////////                if (splitToken.Length > 1)
            ////////                {
            ////////                    //length
            ////////                    oneItem.Length = Convert.ToInt32(splitToken[1]);
            ////////                }
            ////////                else
            ////////                {
            ////////                    //length is the number of X mentioned.
            ////////                    oneItem.Length = splitToken[0].Length;
            ////////                }
            ////////                // The below logic is used to calculate the position of 
            ////////                // the current variable in relation to the whole copy book.
            ////////                oneItem.Position = lengthCalculator;
            ////////                lengthCalculator += oneItem.Length;



            ////////                // Add the currently built variable to the entity.
            ////////                translatedEntity.DataItems.Add(oneItem);
            ////////            }
            ////////        }
            ////////        else
            ////////        {
            ////////            int depthLevelOfGroupItem = 0;
            ////////            if (Int32.TryParse(tokens[0], out depthLevelOfGroupItem))
            ////////            {

            ////////            }
            ////////        }
            ////////    }
            ////////}
            //if (translatedEntity.DataItems.Count == 0)
            //{
            //    return null;
            //}
            //else
            //{
            //    if (translatedEntity.EntityName == null ||
            //        translatedEntity.EntityName.Length == 0)
            //    {
            //        translatedEntity.EntityName
            //        = ParseToCorrectCase(pathOfCobolCopyBook.Substring(
            //        pathOfCobolCopyBook.LastIndexOf('\\') + 1));
            //    }
            //    if (translatedEntity.ProgramId == null ||
            //        translatedEntity.ProgramId.Length == 0)
            //    {
            //        translatedEntity.ProgramId
            //        = pathOfCobolCopyBook.Substring(
            //        pathOfCobolCopyBook.LastIndexOf('\\') + 1);
            //    }
            //    return translatedEntity;
            //}
            #endregion
        }

        private void BuildRedefines(DataItem dataItem, DataItem parentItem, string[] currDeclarationTokens)
        {
            int looper = 0;
            for (; looper < currDeclarationTokens.Length; looper++)
            {
                if (currDeclarationTokens[looper].ToLowerInvariant().Equals("redefines"))
                {
                    break;
                }
            }
            looper++;
            foreach (DataItem item in parentItem.GroupItems)
            {

                string correctedString =ParseToCorrectCase(currDeclarationTokens[looper]);
                if (correctedString.Equals(item.ItemName))
                {
                    dataItem.Redefines = parentItem.GroupItems.IndexOf(item);
                    dataItem.Direction = DataItem.ParameterDirectionType.OutputType;


                    parentItem.GroupItems[dataItem.Redefines].Redefines = parentItem.GroupItems.Count;
                    parentItem.GroupItems[dataItem.Redefines].Direction = DataItem.ParameterDirectionType.InputType;

                }
            }
        }

        private int RetrieveNumberOfOccurences(string p, string[] currDeclarationTokens, DataItem dataItem)
        {
            int numberOfOccurences = 1;
            if (p.ToLowerInvariant().IndexOf("occurs") > 0)
            {
                bool isOccursFound = false;
                foreach (string strCurr in currDeclarationTokens)
                {
                    if (isOccursFound)
                    {
                        Int32.TryParse(strCurr, out numberOfOccurences);
                        isOccursFound = false;
                        break;
                    }
                    if (strCurr.ToLowerInvariant().Equals("occurs"))
                    {
                        isOccursFound = true;
                    }

                }
            }
            return numberOfOccurences;
        }
        int lengthCalculator = 0;
        private DataItem BuildDataItem(string[] currDeclarationTokens)
        {
            DataItem dataItem = new DataItem();
            dataItem.ItemName = ParseToCorrectCase(currDeclarationTokens[1]);
            // A variable is being declared.
            // Eg: 01 Variable-Name Pic x(9)    Value   'Whatever'.
            if (currDeclarationTokens[2].ToLowerInvariant() == "pic")
            {
                // Split the x(9) token.
                char[] splitters = { '(', ')' };
                string[] splitToken = currDeclarationTokens[3].Split(splitters,
                    StringSplitOptions.RemoveEmptyEntries);

                // Check whether the type is string type or integer type.
                // It cannot be determined whether the value is an enum 
                // type right now. (This is done on the parsng of the next line).
                if (splitToken[0].StartsWith("x",
                    StringComparison.InvariantCultureIgnoreCase))
                {
                    dataItem.ItemType = DataItem.DataItemType.StringType;
                }
                else
                {
                    if (splitToken[0].StartsWith("-",
                        StringComparison.InvariantCultureIgnoreCase))
                    {
                        dataItem.ItemType = DataItem.DataItemType.SignedIntegerType;
                    }
                    else
                    {
                        dataItem.ItemType = DataItem.DataItemType.UnsignedIntegerType;
                    }
                }
                //If only x is mentioned
                if (splitToken.Length > 1)
                {
                    //length
                    dataItem.Length = Convert.ToInt32(splitToken[1]);
                }
                else
                {
                    //length is the number of X mentioned.
                    dataItem.Length = splitToken[0].Length;
                }

                // The below logic is used to calculate the position of 
                // the current variable in relation to the whole copy book.
                dataItem.Position = lengthCalculator;
                lengthCalculator += dataItem.Length;
            }

            return dataItem;
        }

        internal class DeclarationHelper
        {
            internal int DeclarationLevel = 0;
            internal DataItem declarationItem;
        }

        /// <summary>
        /// Readline method is used to retrieve a line form the Stream reader. 
        /// This line should be further split according to the "." and has to be 
        /// considered as multiple lines.
        /// </summary>
        /// <param name="fileReader">The StreamReader which has been opened to read 
        /// from the COBOL copy book. This is the location from which the file will be read.</param>
        /// <returns>This method returns a line from the input COBOL copy book file.</returns>
        private string ReadLine(StreamReader fileReader)
        {
            // If a previously read line exists, read the next line from it and return.
            if (allReadLines != null)
            {
                // Increment the pointer used to retrieve the next line.
                currentIndex++;


                if (allReadLines.Length <= currentIndex)
                {
                    // Clear the value as the parsed input lines has been cleared.
                    currentIndex = 0;
                    allReadLines = null;
                }
                else
                {
                    // return the previously read lines after rectifying the string.
                    return RectifyReadString(allReadLines[currentIndex]);
                }
            }

            // Previously read lines are not present. So read another line from the input stream.
            string readLine = fileReader.ReadLine();

            // Clear all unwanted characters.
            readLine = readLine.Trim(trimCharacters);
            char[] endOfLine ={ '.' };
            allReadLines = readLine.Split(endOfLine, StringSplitOptions.RemoveEmptyEntries);

            // Are there multiple lines in the same input line.
            if (allReadLines.Length < 2)
            {
                readLine = RectifyReadString(readLine);
                allReadLines = null;
            }
            else
            {
                return RectifyReadString(allReadLines[currentIndex]);
            }

            return readLine;
        }

        /// <summary>
        /// This method will be utilized to correct the names of the variables.
        /// This is because the COBOL copy books contain various options.
        /// For example, we can use hyphen as part of the variable names.
        /// </summary>
        /// <param name="varibleNameWhichHasToBeCorrected">The COBOL copy book name of the variable.</param>
        /// <returns>The case-corrected variable name as required by .NET entities.</returns>
        protected static string ParseToCorrectCase(string variableName)
        {
            if (variableName == null)
            {
                return string.Empty;
            }
            // All the hyphens should be replaced by _.
            // We achieve this by splitting the input string to tokens using hyphen as the 
            // split cahracter. and append these cokens using the _ as seperator.
            char[] splitChars = { '-' };

            string[] splitStrings =
                variableName.Split(splitChars);


            string newString = string.Empty;
            StringBuilder strBuilder = new StringBuilder(
                variableName.Length + 1);
            string underScore = "_";

            for (int counter = 0; counter < splitStrings.Length; counter++)
            {
                string currentString = splitStrings[counter];

                // The below code enables us to capitalize the tokens.
                currentString = currentString.ToLowerInvariant();

                // Append a capitalized first letter.
                strBuilder.Append(currentString.Substring(0, 1).ToUpperInvariant());

                // Append the remaining charecters in lower case.
                strBuilder.Append(currentString.Substring(1));

                // Append the tokens by using an underscore.
                strBuilder.Append(underScore);
            }
            newString = strBuilder.ToString();
            // The last underscore should not be returned.
            return newString.Substring(0, newString.Length - 1);
        }

        /// <summary>
        /// This method will be used to rectify the string read from the input file.
        /// </summary>
        /// <param name="stringToBeRectified">The string which has to be rectified.</param>
        /// <returns>The rectified string. All the characters present in TrimCharacters will be 
        /// removed.</returns>
        private string RectifyReadString(string stringToBeRectified)
        {
            // delete all the extra trim characters at the start/end of the line.
            stringToBeRectified = stringToBeRectified.Trim(trimCharacters);

            // Remove the last . if present.
            int indexOfEndOfLine = stringToBeRectified.IndexOf('.');
            if (indexOfEndOfLine > -1)
            {
                stringToBeRectified = stringToBeRectified.Substring(0, indexOfEndOfLine);
            }

            return stringToBeRectified;
        }
        /// <summary>
        /// This method will be utilized to check whether the code is commented out.
        /// </summary>
        /// <param name="readLine">The line read from the input stream, which should be checked for commenting.</param>
        /// <returns>true if the code is commented out, false otherwise.</returns>
        private static bool isCommented(string readLine)
        {
            if (
                readLine.StartsWith("*", StringComparison.InvariantCulture)
                ||
                readLine.Length == 0
                )
            {
                return true;
            }
            return false;
        }
        #endregion
    }
}
