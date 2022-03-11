/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Infosys.Solutions.Superbot.Resource.Entity
{
    public static class EntityExtension<T>
    {
        /// <summary>
        /// Extension to apply only the changes from one entity to a diferent entity of same types
        /// </summary>
        /// <param name="targetEntity">The entity to which the changes are to be applied</param>
        /// <param name="sourceEntity">The entity from which the changes are to be taken</param>
        public static void ApplyOnlyChanges(T targetEntity, T sourceEntity)
        {
            foreach (PropertyInfo property in targetEntity.GetType().GetProperties())
            {
                object value = property.GetValue(sourceEntity, null);
                if (value != null)
                {
                    if (value.GetType() == typeof(string))
                    {
                        string strValue = value as string;
                        if (strValue != null)
                            property.SetValue(targetEntity, value, null);
                    }

                    else if (value.GetType() == typeof(DateTime))
                    {
                        DateTime dtValue = (DateTime)value;
                        if (dtValue != null && dtValue != DateTime.MinValue)
                            property.SetValue(targetEntity, value, null);
                    }

                    else if (value.GetType() == typeof(int))
                    {
                        int inyValue = (int)value;
                        if (inyValue > 0)
                            property.SetValue(targetEntity, value, null);
                    }

                    else
                    {
                        property.SetValue(targetEntity, value, null);
                    }

                    //similarly add for other data types as needed
                }
            }
        }
    }
}
