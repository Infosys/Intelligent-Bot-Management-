using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
/****************************************************************
 * This file is a part of the Legacy Parser utility. 
 * This class is a utility used by the CICS specific implementation of the 
 * host specific type controller. This parses the Object.LST file to 
 * retrieve the various parameters such as the primary keys, the object 
 * id of the entities being generated.
 ***************************************************************/

namespace Infosys.Lif.LegacyParser
{
    /// <summary>
    /// The object list parser parses the Object.LST file and builds up a 
    /// Dictionary object with the object id as the key for various parameters.
    /// This can be utilized to retrieve various important parameters 
    /// regarding the object on the mainframe.
    /// </summary>
    public class ObjectListParser : IDisposable
    {
        /// <summary>
        /// This reader will be utilized to read the file,
        /// </summary>
        StreamReader reader;
        /// <summary>
        /// This is the internal method of the Obejct list parser and will be 
        /// used to parse and build the dictionary object. After the above step it 
        /// assignes each entity passed as input, an object id.
        /// </summary>
        /// <param name="fileName">The path to the Object.LST file</param>
        /// <param name="entities">The entities for which the Object id should be decided.</param>
        /// <returns>The entity collection passed as input with the Object id for each entity filled up.</returns>
        internal GenericCollection<Entity> Parse(string fileName, GenericCollection<Entity> entities)
        {
            //Open the file.
            FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            reader = new StreamReader(fs);

            // Call BuildObjectDictionary and retrieve the dictionary object, which 
            // contains each object as value, and program id as key for each key value pair.
            Dictionary<string, ObjectElement> objectDictionary = BuildObjectDictionary();

            //Loop the input entity collection and set the various parameters.
            for (int entityLooper = 0; entityLooper < entities.Count; entityLooper++)
            {
                Entity currEntity = entities[entityLooper];
                //does the object list contain the Program Id as mentioned in the entity?
                if ((currEntity.ObjectId == null || currEntity.ObjectId.Length == 0)
                    && objectDictionary.ContainsKey(currEntity.ProgramId))
                {
                    ObjectElement currEntityObject = objectDictionary[currEntity.ProgramId];
                    {
                        //Set various object parameters for the entity.
                        currEntity.ObjectId = currEntityObject.ObjectId;
                    }
                }
                else
                {
                    if (currEntity.ObjectId == null || currEntity.ObjectId.Length < 8)
                    {
                        int initialLength = (currEntity.ObjectId == null) ? 0 : currEntity.ObjectId.Length;
                        for (int i = 0; i < 8 - initialLength; i++)
                        {
                            // The program id is not present in the Object.LST. default it to 8 spaces.
                            currEntity.ObjectId += " ";
                        }
                    }
                }
            }

            return entities;
        }

        /// <summary>
        /// This method will be used to parse the object.lst file.
        /// The Parse method opens the file and this method utilizes the open stream 
        /// to retrieve the object parameters for each program.
        /// </summary>
        /// <returns>Returns a dictionary object with Program id as key and 
        /// the object parameters as the Value.</returns>
        private Dictionary<string, ObjectElement> BuildObjectDictionary()
        {
            // The object dictionary which will be returned.
            Dictionary<string, ObjectElement> objectDictionary
                = new Dictionary<string, ObjectElement>();
            // Loop until end of stream.
            while (!reader.EndOfStream)
            {
                // The cahracters used to split each line into keywords.
                char[] splitChars ={ '\0', ' ', ',', '\t' };

                // Read a corrected input line.
                string readLine = ReadLine();

                // Split the read line into tokens
                string[] tokens
                    = readLine.Split(splitChars,
                    StringSplitOptions.RemoveEmptyEntries);

                // The line should have atleast 2 tokens.
                if (tokens.Length > 2)
                {
                    // If the key already exists it means, that various 
                    // tables are being accessed. or it may be an error in the file.
                    // The tables accessed are not being utilized here. 
                    // So ignore (this ignores even if it is an error).
                    // As the error can only be an human error.
                    if (objectDictionary.ContainsKey(tokens[0]))
                    {
                        // The table name also is present. 
                        if (tokens.Length > 4)
                        {
                            ObjectElement objectElement = objectDictionary[tokens[0]];
                            objectElement.ProgramId = tokens[0];
                            objectElement.OperationsAllowed = tokens[1];
                            objectElement.ObjectId = tokens[2];
                            //Should add table accessed name if necessary.
                        }
                    }
                    else
                    {
                        // Create a new ObjectElement object and add it to the dictionary.
                        ObjectElement objectElement = new ObjectElement();

                        // Program id.
                        objectElement.ProgramId = tokens[0];

                        // The various operations allowed on this object.
                        objectElement.OperationsAllowed = tokens[1];

                        // The objectid as the mainframe accepts it.
                        objectElement.ObjectId = tokens[2];

                        // Add the ObjectElement to the Dictionary object.
                        objectDictionary.Add(tokens[0], objectElement);
                    }
                }
            }
            return objectDictionary;
        }
        /// <summary>
        /// This method is used to read a line from the opened Stream. 
        /// It also performs all the trimming,etc.
        /// </summary>
        /// <returns>A neatly trimmed line read from the file.</returns>
        private string ReadLine()
        {
            string readLine;

            readLine = reader.ReadLine();


            // The characters which have to be used to trim the read line. 
            char[] trimCharacters = { '\n', '\r', '\t', '\0', ' ' };
            readLine = readLine.Trim(trimCharacters);


            return readLine;
        }

        #region Start of ObjectElement class
        /// <summary>
        /// This class will be used to store the various parameters retrieved 
        /// for each object on the mainframe from the Object.LST file.
        /// </summary>
        private class ObjectElement
        {
            string _programId;
            string _operationsAllowed;
            string _objectId;

            /// <summary>
            /// The program id associated with this object.
            /// </summary>
            public string ProgramId
            {
                get { return _programId; }
                set { _programId = value; }
            }


            /// <summary>
            /// All the operations possible for this object.
            /// </summary>
            public string OperationsAllowed
            {
                get { return _operationsAllowed; }
                set { _operationsAllowed = value; }
            }

            /// <summary>
            /// The id of the object as the mainframe recognises it.
            /// </summary>
            public string ObjectId
            {
                get { return _objectId; }
                set { _objectId = value; }
            }
        }
        #endregion End of ObjectElement class



        #region IDisposable Members
        /// <summary>
        /// Called while disposing this object. Will be used to close the stream
        /// </summary>
        public void Dispose()
        {
            reader.Close();
            reader = null;
        }
        #endregion
    }
}
