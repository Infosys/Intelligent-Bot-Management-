using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Infosys.Lif.LegacyParser.LanguageConverters
{
    internal class CobolFileParser
    {
        internal string LinkageSection;
        internal string programId;
        internal CobolFileParser(string filePath)
        {
            int indexOfLastSlash = filePath.LastIndexOf('\\');
            programId = filePath.Substring(indexOfLastSlash + 1);
            programId = programId.Substring(0, programId.LastIndexOf('.'));
            StreamReader reader = new StreamReader(filePath);
            StringBuilder currentStringBuilder = new StringBuilder();
            bool linkageSectionEnded = false;
            while (!reader.EndOfStream && !linkageSectionEnded)
            {
                bool isNextWOrdProgramId = false;
                string readLine = reader.ReadLine();
                readLine = readLine.Trim();
                char[] splitterChars = { '.' };
                string[] splitLines = readLine.Split(splitterChars, StringSplitOptions.RemoveEmptyEntries);
                for (int splitLineLooper = 0;
                    splitLineLooper < splitLines.Length;
                    splitLineLooper++)
                {
                    if (splitLines[splitLineLooper].Trim().StartsWith("*"))
                    {
                        // Ignore as this is a comment
                    }
                    else if (isNextWOrdProgramId)
                    {

                        isNextWOrdProgramId = false;
                    }
                    else if (splitLines[splitLineLooper].Trim().ToLowerInvariant().Equals("PROGRAM-ID"))
                    {
                        isNextWOrdProgramId = true;
                    }
                    else if (splitLines[splitLineLooper].Trim().StartsWith("/"))
                    {
                        // Continuation of prev line, so we can ignore the /
                        LinkageSection += splitLines[splitLineLooper].Substring(1);
                    }
                    else if ((splitLines[splitLineLooper].ToLowerInvariant().IndexOf("linkage") > -1)
                    &&
                    (splitLines[splitLineLooper].ToLowerInvariant().IndexOf("section") > -1)
                    )
                    {
                        // the keyword "linkage section" occurs
                        LinkageSection = string.Empty;
                    }
                    else if ((splitLines[splitLineLooper].ToLowerInvariant().IndexOf("procedure") > -1)
                        &&
                        (splitLines[splitLineLooper].ToLowerInvariant().IndexOf("division") > -1))
                    {
                        // The linkage section has ended
                        linkageSectionEnded = true;
                        break;
                    }
                    else
                    {
                        LinkageSection += Environment.NewLine + splitLines[splitLineLooper].Trim();
                    }

                }
            }
        }
    }
}
