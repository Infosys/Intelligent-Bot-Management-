/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Json;

namespace Infosys.Solutions.ConfigurationManager.Infrastructure.Common
{
    public static class Utility
    {
        public static string GetLoggedInUser()
        {
            //windows authentication
            var name = OperationContext.Current.ServiceSecurityContext.WindowsIdentity.Name;

            //https
            if (String.IsNullOrEmpty(name))
                return OperationContext.Current.ServiceSecurityContext.PrimaryIdentity.Name;

            return name;
        }

        /// <summary>
        /// Used the serialize an object to Json so as to pass it to the controller
        /// </summary>
        /// <param name="objectToSerialize"></param>
        /// <returns></returns>
        public static string SerialiseToJSON(this object objectToSerialize)
        {
            string serializedString = string.Empty;
            try
            {
                using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                {
                    DataContractJsonSerializer serializer =
                        new DataContractJsonSerializer(objectToSerialize.GetType());
                    serializer.WriteObject(ms, objectToSerialize);
                    ms.Position = 0;

                    using (System.IO.StreamReader reader = new System.IO.StreamReader(ms))
                    {
                        serializedString = reader.ReadToEnd();
                    }
                }

            }
            catch (Exception)
            {
            }
            return serializedString;
        }

        /// <summary>
        /// Deserialize a JSON serialized string to object of given type.
        /// </summary>
        /// <typeparam name="T">Type of object</typeparam>
        /// <param name="serializedString">JSON serialized string</param>
        /// <returns></returns>
        public static T DeserializeFromJSON<T>(this string serializedString)
        {
            // validate input
            if (string.IsNullOrWhiteSpace(serializedString))
            {
                return default(T);
            }

            try
            {
                // try to deserialize to type T
                using (System.IO.MemoryStream ms = new System.IO.MemoryStream(Encoding.Unicode.GetBytes(serializedString)))
                {
                    DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
                    return (T)serializer.ReadObject(ms);
                }
            }
            catch (Exception)
            {
                // return null for reference types and default value for value types
                return default(T);
            }
        }
    }
}

