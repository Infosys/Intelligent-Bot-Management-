/******************************************************************************
 * This file is a part of the Legacy Integration Framework.
 * This file handles laoding and retrieval of config file
 * Copyright (c) 2003 - 2005 Infosys Technologies Ltd. All Rights Reserved.
 *****************************************************************************/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Collections;
using System.Text;
using System.Configuration;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using System.Resources;
using System.Runtime.Remoting;

namespace Infosys.Lif.DataLoader
{
    /// <summary>
    /// Manages the task of loading configuration file, retrieving data and accordingly fetches data for passed object
    /// </summary>
    public class DataLoadManager
    {
        //creating hashtable objects
        Hashtable htConfigNonFilter = new Hashtable();
        Hashtable htConfigWithFilter = new Hashtable();

        //object to fetch Config file details
        DataLoaderSettings configurations;               

        /// <summary>
        /// load config details
        /// </summary>
        public DataLoadManager()
        {
            //Call function to get config details            
            configurations = RetrieveConfigDetails();
        }
         
        /// <summary>
        /// Retrieves configuration details from config file
        /// </summary>
        /// <returns> object of type DataLoaderSettings </returns>
        private static DataLoaderSettings RetrieveConfigDetails()
        {
            try
            {
                //retrieve the config section from config file in DataLoaderSettings object
                DataLoaderSettings dataLoaderDetails = (DataLoaderSettings)ConfigurationManager.GetSection("DataLoaderSettings");
                return dataLoaderDetails;
            }
            catch (Exception ex)
            {      
                //if config file could not be read or data could not be retrieved          
                throw new LoaderException(Infosys.Lif.DataLoader.Properties.Resource1.ExcepDataLoadMgrConfigFile, ex.InnerException);
            }
        }

        /// <summary>
        /// build a hash table : class name as key and entity collection as value
        /// </summary>
        private void BuildHashTableWithFilter()
        {                        
            //populating data for hash table with filter - class name as key
            foreach (Entity entity in configurations.EntityCollection)
            {
                if (!htConfigWithFilter.ContainsKey(entity.ClassName))
                {
                    htConfigWithFilter.Add(entity.ClassName, entity.TestDataCollection);
                }
                else
                {
                    //throw exception if trying to insert duplicate key(class name) in hash table                   
                    throw new LoaderException(Infosys.Lif.DataLoader.Properties.Resource1.ExcepDataLoadMgrEntityName);
                }
            }
        }

        /// <summary>
        /// build a hash table : class name as key and Default source as value
        /// </summary>
        private void BuildHashTableWithoutFilter()
        {
            //populating data for hash table without filter - class name as key
            foreach (Entity entity in configurations.EntityCollection)
            {
                if (!htConfigNonFilter.ContainsKey(entity.ClassName))
                {
                    htConfigNonFilter.Add(entity.ClassName, entity.DefaultSource);
                }
                else
                {
                    //throw exception if trying to insert duplicate key(class name) in hash table
                    throw new LoaderException(Infosys.Lif.DataLoader.Properties.Resource1.ExcepDataLoadMgrEntityName);
                }
            }
        }

                        
        /// <summary>
        /// fetches data corresponding to passed object and filter conditions
        /// </summary>
        /// <param name="entity"> entity passed </param>
        /// <param name="filter"> filter conditions </param>
        /// <returns> object containing data </returns>
        public T FetchData <T> (T entity, string filter)
        {
            string fileName = string.Empty;

            //call function to populate hash table for search if not already done
            if (htConfigWithFilter.Count == 0)
            {
                BuildHashTableWithFilter();
            }
                        
            //searching the hash table by key i.e. entity name
            foreach (DictionaryEntry deConfig in htConfigWithFilter)
            {                
                Infosys.Lif.DataLoader.TestDataCollection tdCollection;
                if ((deConfig.Key.ToString()).Equals((entity.GetType()).ToString()))
                {
                    tdCollection = (TestDataCollection)deConfig.Value;

                    //searching for the corresponding filename for a filter
                    foreach (TestData td in tdCollection)
                    {
                        if(td.Filter.Equals(filter))
                        {
                            fileName = td.FileName;
                            break;
                        }
                    }
                }
            }

            //parameters to call GetData() function from Activator
            string assemblyName;
            string typeName;

            assemblyName = configurations.__LoaderClass;
            typeName = configurations.__LoaderType;

            //calling CustomLoader.GetData() thru Activator
            Object entityReturned;
            try
            {
                ObjectHandle objHandle = Activator.CreateInstanceFrom(assemblyName, typeName);
                IDataLoader dlObject;
                dlObject = (IDataLoader)objHandle.Unwrap();                
                entityReturned = dlObject.GetData(fileName, entity);
            }
            catch (Exception ex)
            {                
                //if error in calling the method
                throw new LoaderException(Infosys.Lif.DataLoader.Properties.Resource1.ExcepDataLoadMgrActivator, ex.InnerException);
            }

            return (T)entityReturned;                            
        }

        /// <summary>
        /// fetches data corresponding to passed object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public T FetchData <T> (T entity)
        {
            string fileName = string.Empty;

            //call function to populate hash table for search if not already done
            if (htConfigNonFilter.Count == 0)
            {
                BuildHashTableWithoutFilter();
            }
            
            //searching the hash table by key i.e. entity name
            foreach (DictionaryEntry deConfig in htConfigNonFilter)
            {
                if(deConfig.Key.Equals(entity.GetType().ToString()))
                {
                    fileName = deConfig.Value.ToString();
                    break;
                }
            }

            //parameters to call GetData() function from Activator
            string assemblyName;
            string typeName;

            assemblyName = configurations.__LoaderClass;
            typeName = configurations.__LoaderType;

            //calling CustomLoader.GetData() thru Activator
            Object entityReturned;
            try
            {
                ObjectHandle objHandle = Activator.CreateInstanceFrom(assemblyName, typeName);
                IDataLoader dlObject;
                dlObject = (IDataLoader)objHandle.Unwrap();                
                entityReturned = dlObject.GetData(fileName, entity);
            }
            catch (Exception ex)
            {
                //if error in calling the method
                throw new LoaderException(Infosys.Lif.DataLoader.Properties.Resource1.ExcepDataLoadMgrActivator, ex.InnerException);
            }

            return (T)entityReturned;
        }
    }
}
