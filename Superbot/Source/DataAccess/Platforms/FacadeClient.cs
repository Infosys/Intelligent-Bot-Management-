/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using Infosys.Ainauto.Framework.Facade;
using Infosys.Ainauto.Framework.Facade.Entity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Infosys.Solutions.Ainauto.Resource.DataAccess
{
    public class FacadeClient
    {
        static class InterfaceConsts
        {
            public const string IAdapterBase = "Infosys.Ainauto.Adapters.Interfaces.IAdapterBase";
            public const string IDBAdapter = "Infosys.Ainauto.Adapters.Interfaces.IDBAdapter";
            public const string IApiAdapter = "Infosys.Ainauto.Adapters.Interfaces.IApiAdapter";

        }
        #region common
        /// <summary>
        /// 
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        private List<DataRow> ConvertToDataSet (string json)
        {
            List<DataRow> lists = null;

            if (json != null)
            {

                XmlDocument doc = (XmlDocument)JsonConvert.DeserializeXmlNode(json, "rows");
                string xml = doc.InnerXml;
               


                using (MemoryStream xmlStream = new MemoryStream())
                {
                    XmlDocument xDoc = new XmlDocument();
                    xDoc.LoadXml(xml);
                    xDoc.Save(xmlStream);
                    xmlStream.Seek(0, System.IO.SeekOrigin.Begin);
                    DataSet dataset = new DataSet();
                    dataset.ReadXml(xmlStream);
                    if (dataset.Tables.Count > 0)
                    {
                        DataTable table = dataset.Tables[0];
                        lists = table?.AsEnumerable().ToList();
                    }
                }
                
            }

            return lists;
        }
        #endregion
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        //public List<DataRow>  GeHealthCheckPlatformDBMetrics(Expendo queryParam, Expendo filterParam)
        public List<DataRow> GeHealthCheckPlatformDBMetrics()
        {

            const string ADAPTER_TYPE = "AA";
            const string ADAPTER_METHOD_NAME = "Get";


            /*Dictionary<string, string> dictQueryParams = new Dictionary<string, string>();
            //dictQueryParams.Add("bot_name", "mybotname");
            //dictQueryParams.Add("resource_id", "myresourceid");

            Dictionary<string, string> dictConfigOverride = new Dictionary<string, string>();
            dictConfigOverride.Add("Uid", "sa");
            
            Facade facade = new Facade(); // Pass dev/Test to the Facade
            return ConvertToDataSet(facade.Execute(ADAPTER_TYPE, "Get", dictQueryParams, dictConfigOverride));*/


            Facade facade = new Facade();
            FacadeResult facadeResult = null;


            Dictionary<string, string> dictQueryParams = new Dictionary<string, string>();
            //dictQueryParams.Add("param1", "1");
            //dictQueryParams.Add("param2", "2");

            Dictionary<string, string> dictConfigOverride = new Dictionary<string, string>();
            dictConfigOverride.Add("Uid", "sa");
            // dictConfigOverride.Add("SQLQuery", "select * from  mytable where resourceId =  {resource_id}"); // OverRide

           // dictConfigOverride.Add("SQLQuery", "Select bot_name from DB_Bot_Details where {param1}!={param2}");

            //Test default Interface
            facadeResult = facade.Execute(ADAPTER_TYPE, InterfaceConsts.IDBAdapter,"Get" , "botdetails",  dictQueryParams, dictConfigOverride); //  If Interace name is null retuns the first interface

            // Invoke the  DBAdatper - 1. Default Interace and 2. Custom Interface
            //json =  facade.Execute(ADAPTER_TYPE, "Update", InterfaceConsts.IDBAdapter, dictQueryParams, dictConfigOverride);
            return ConvertToDataSet(facadeResult.Data);           

        }

        public string TestFacadeClient()
        {

            const string ADAPTER_TYPE = "AA";
            const string ADAPTER_METHOD_NAME = "Get";
           
            Facade facade = new Facade();
            FacadeResult facadeResult = null;


            Dictionary<string, string> dictQueryParams = new Dictionary<string, string>();
            //dictQueryParams.Add("param1", "1");
            //dictQueryParams.Add("param2", "2");

            Dictionary<string, string> dictConfigOverride = new Dictionary<string, string>();
            dictConfigOverride.Add("Uid", "sa");
            

            //Test default Interface
            facadeResult = facade.Execute(ADAPTER_TYPE, InterfaceConsts.IDBAdapter, "Get", "botdetails", dictQueryParams, dictConfigOverride); //  If Interace name is null retuns the first interface


            return "Status:" + facadeResult.Status + " Validation Error:" + facadeResult.ValidationDetails.Errors + " Data:" + facadeResult.Data.Count();

        }
    }
}
