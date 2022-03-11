/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Caching;
using Infosys.Lif.LegacyCommon;
using Infosys.Lif.LegacyIntegratorService;

namespace Infosys.Lif
{
    public class MemoryDocAdapter : IAdapter
    {
        private const string REGION = "Region";
        private const string TRANSPORT_SECTION = "TransportSection";
        private const string DATA = "Data";
        private const string TARGETURLDETAILS = "TargetURLDetails";
        private const string SUCCESSFUL_DATA_SENT = "Document successfully uploaded.";
        private const string SUCCESSFUL_DATA_RECEIVED = "Document successfully downloaded.";
        private const string UNSUCCESSFUL_DATA_SENT = "Document couldn't be uploaded successfully.";
        private const string UNSUCCESSFUL_DATA_RECEIVED = "Document couldn't be downloaded successfully.";
        private const string SUCCESSFUL_DATA_DELETED = "Document successfully Deleted.";
        private const string UNSUCCESSFUL_DATA_DELETED = "Document couldn't be deleted successfully. ";
        private const int SUCCESSFUL_STATUS_CODE = 0;
        private const int UNSUCCESSFUL_STATUS_CODE = 1000;


        private NameValueCollection targetURLDetails;

        #region IAdapter Members

        public event ReceiveHandler Received;

        //ObjectCache cache = MemoryCache.Default;
        //CacheItemPolicy policy = new CacheItemPolicy();
        readonly static ObjectCache cache;
        readonly static CacheItemPolicy policy;
        private static CacheEntryRemovedCallback removeCallback = null;
        private const string LI_CONFIGURATION = "LISettings";

        static MemoryDocAdapter () {

            cache = MemoryCache.Default; //Define default Region 
            policy = new CacheItemPolicy();
            LifLogHandler.LogDebug("MemoryDocAdapter Adapter- MemoryDocAdapter static constructor executed", LifLogHandler.Layer.IntegrationLayer);
            // Read all config data into LISetttings object.
            LISettings liSettings = (LISettings)ConfigurationManager.GetSection(LI_CONFIGURATION);
            // Find the MemoryQueueDetails
            MemoryDoc memoryDoc = liSettings.MemoryDoc;
            MemoryDocDetailsCollection docDetailsCollection = memoryDoc.MemoryDocDetailsCollection;
            int expirationValue = 10;
            foreach (MemoryDocDetails memoryDocDetails in docDetailsCollection)
            {
                expirationValue = memoryDocDetails.MemoryCacheSlidingExpiration;
                break;
            }

            policy.SlidingExpiration = TimeSpan.FromMinutes(expirationValue);

            //RemovedCallback will call once Cached Item will be expired. 
            //We can use either CacheEntryUpdateCallback or CacheEntryRemovedCallback once. Other one should be null.
            removeCallback = new CacheEntryRemovedCallback(OnExpire);
            policy.RemovedCallback = removeCallback;


        }
        public bool Delete(ListDictionary messageDetails)
        {
            // validate input
            var region = messageDetails[REGION] as Region;
            var transport = messageDetails[TRANSPORT_SECTION] as MemoryDoc;
            var targetDetails = messageDetails[TARGETURLDETAILS] as NameValueCollection;
            if (region == null
                || transport == null
                || targetDetails == null)
            {
                LifLogHandler.LogError(
                    "MemoryDoc Adapter- Delete- One of Region, Transport section, Target details is not passed!",
                    LifLogHandler.Layer.IntegrationLayer);

                throw new ArgumentException(string.Format("{0}, {1}, {2} are required for delete operation!",
                    REGION, TRANSPORT_SECTION, TARGETURLDETAILS));
            }

            // check for mandatory parameters
            bool check = targetDetails.AllKeys.Contains("container_name")
                            && !string.IsNullOrWhiteSpace(targetDetails["container_name"]);
            if (!check)
                throw new LegacyException("MemoryDoc Adapter- 'container_name' missing in metadata.");

            check = targetURLDetails.AllKeys.Contains("file_name")
                               && !string.IsNullOrWhiteSpace(targetURLDetails["file_name"]);
            if (!check)
                throw new LegacyException("MemoryDoc Adapter- 'file_name' missing in metadata.");

            // Validates whether TransportName specified in the region, exists in IISBlobDetails section.
            MemoryDocDetails docDetails = ValidateTransportName(transport, region.TransportName);

            if (docDetails == null)
                throw new LegacyException(
                    string.Format("MemoryDoc Adapter- Could not find transport by name: {0}", region.TransportName));

            // delete the container
            var response = HandleStream(docDetails, DocAccessType.Delete, null);

            LifLogHandler.LogDebug("Response for delete: {0}", LifLogHandler.Layer.IntegrationLayer, response);

            // return true if response string is empty otherwise false
            return string.IsNullOrWhiteSpace(response);
        }

        public void Receive(ListDictionary adapterDetails)
        {
            Infosys.Lif.LegacyIntegratorService.MemoryDoc transportSection = null;
            Infosys.Lif.LegacyIntegratorService.Region regionToBeUsed = null;
                try
                {
                    LifLogHandler.LogDebug("MemoryDoc Adapter- Receive called", LifLogHandler.Layer.IntegrationLayer);
                    foreach (DictionaryEntry items in adapterDetails)
                    {
                        if (items.Key.ToString() == REGION)
                        {
                            regionToBeUsed = items.Value as Region;
                        }
                        else if (items.Key.ToString() == TRANSPORT_SECTION)
                        {
                            transportSection = items.Value as Infosys.Lif.LegacyIntegratorService.MemoryDoc;
                        }
                        else if (items.Key.ToString() == TARGETURLDETAILS)
                        {
                            targetURLDetails = items.Value as NameValueCollection;
                        }
                    }

                    //verify the data stream and the details regarding the taget URL
                    if (targetURLDetails == null)
                        throw new LegacyException("MemoryDoc Adapter- Metadata details are not provided.");

                    // check for mandatory parameters

                    bool check = targetURLDetails.AllKeys.Contains("container_name")
                                    && !string.IsNullOrWhiteSpace(targetURLDetails["container_name"]);
                    if (!check)
                        throw new LegacyException("MemoryDoc Adapter- 'container_name' missing in metadata.");

                    check = targetURLDetails.AllKeys.Contains("file_name")
                                    && !string.IsNullOrWhiteSpace(targetURLDetails["file_name"]);
                    if (!check)
                        throw new LegacyException("MemoryDoc Adapter- 'file_name' missing in metadata.");



                    // Validates whether TransportName specified in the region, exists in IISBlobDetails section.
                    MemoryDocDetails docDetails = ValidateTransportName(transportSection, regionToBeUsed.TransportName);

                    HandleStream(docDetails, DocAccessType.Receive, null);

                }
                catch (LegacyException exception)
                {
                    LifLogHandler.LogError("MemoryDoc Adapter- Receive- exception raised, reason- " + exception.Message,
                        LifLogHandler.Layer.IntegrationLayer);
                    throw exception;
                }
                catch (Exception exception)
                {
                    LifLogHandler.LogError("MemoryDoc Adapter- Receive- exception raised, reason- " + exception.Message,
                        LifLogHandler.Layer.IntegrationLayer);
                    throw exception;
                }
            
        }

        public string Send(System.Collections.Specialized.ListDictionary adapterDetails, string message)
        {
            string response = SUCCESSFUL_DATA_SENT;
            Infosys.Lif.LegacyIntegratorService.MemoryDoc transportSection = null;
            Infosys.Lif.LegacyIntegratorService.Region regionToBeUsed = null;
            Stream dataStream = null;
            try
            {
                LifLogHandler.LogDebug("MemoryDoc Adapter- Send called", LifLogHandler.Layer.IntegrationLayer);
                foreach (DictionaryEntry items in adapterDetails)
                {
                    if (items.Key.ToString() == REGION)
                    {
                        regionToBeUsed = items.Value as Region;
                    }
                    else if (items.Key.ToString() == TRANSPORT_SECTION)
                    {
                        transportSection = items.Value as Infosys.Lif.LegacyIntegratorService.MemoryDoc;
                    }
                    else if (items.Key.ToString() == DATA)
                    {
                        dataStream = items.Value as Stream;
                    }
                    else if (items.Key.ToString() == TARGETURLDETAILS)
                    {
                        targetURLDetails = items.Value as NameValueCollection;
                    }
                }

                //verify the data stream and the details regarding the taget URL
                if (dataStream == null && targetURLDetails == null)
                    throw new LegacyException("MemoryDoc Adapter- File Stream and metadata details both cannot be empty.");

                // check for mandatory parameters
                bool check = targetURLDetails.AllKeys.Contains("container_name")
                                && !string.IsNullOrWhiteSpace(targetURLDetails["container_name"]);
                if (!check)
                    throw new LegacyException("MemoryDoc Adapter- 'container_name' missing in metadata.");

                check = targetURLDetails.AllKeys.Contains("file_name")
                                && !string.IsNullOrWhiteSpace(targetURLDetails["file_name"]);
                if (!check)
                    throw new LegacyException("MemoryDoc Adapter- 'file_name' missing in metadata.");

                // Validates whether TransportName specified in the region, exists in IISBlobDetails section.
                MemoryDocDetails docDetails = ValidateTransportName(transportSection, regionToBeUsed.TransportName);

                response = HandleStream(docDetails, DocAccessType.Send,dataStream);
                
            }
            catch (LegacyException exception)
            {
                LifLogHandler.LogError("MemoryDoc Adapter- Send- exception raised, reason- " + exception.Message,
                    LifLogHandler.Layer.IntegrationLayer);
                throw exception;
            }
            catch (Exception exception)
            {
                LifLogHandler.LogError("MemoryDoc Adapter- Send- exception raised, reason- " + exception.Message,
                    LifLogHandler.Layer.IntegrationLayer);
                throw exception;
            }
            finally
            {
                if (dataStream != null)
                {
                    dataStream.Dispose();                    
                    dataStream = null;
                }
            }
            return response;
        }
        #endregion

        /// <summary>
        /// Validates whether TransportName specified in the region, exists in MemoryDocDetails
        /// section. If it found, it returns corresponding MemoryDocDetails object.
        /// </summary>
        /// <param name="transportSection">MemoryDoc section</param>
        /// <param name="transportName">name of the transport</param>
        private MemoryDocDetails ValidateTransportName(Infosys.Lif.LegacyIntegratorService.MemoryDoc transportSection,
            string transportName)
        {
            LifLogHandler.LogDebug("MemoryDoc Adapter- ValidateTransportName called...",
                LifLogHandler.Layer.IntegrationLayer);
            MemoryDocDetails blobDetails = null;
            bool isTransportNameExists = false;
            // Find the MemoryDoc region to which it should connect for sending message.
            for (int count = 0; count < transportSection.MemoryDocDetailsCollection.Count; count++)
            {
                blobDetails = transportSection.MemoryDocDetailsCollection[count] as MemoryDocDetails;
                if (blobDetails.TransportName == transportName)
                {
                    isTransportNameExists = true;
                    break;
                }
            }
            // If MSMQ region is not set in the config then throw the exception
            if (!isTransportNameExists)
            {
                throw new LegacyException("MemoryDoc Adapter- " + transportName + " is not defined in MSMQDetails section");
            }
            return blobDetails;
        }

        private string HandleStream(MemoryDocDetails docDetails, DocAccessType accessType, Stream dataStream)
        {
            LifLogHandler.LogDebug("MemoryDoc Adapter- HandleStream called for access type- " + accessType.ToString(),
                LifLogHandler.Layer.IntegrationLayer);
            string response = "";
            try
            {
                if (docDetails != null)
                {
                    //Add to cache
                   // double cacheExpiration = docDetails.MemoryCacheSlidingExpiration;
                    //policy.SlidingExpiration = TimeSpan.FromMinutes(cacheExpiration);
                    string documentsVDFromRoot = docDetails.DocumentsVirtualDirectoryFromRoot;
                    string containerName = targetURLDetails["container_name"];
                    string fileName = targetURLDetails["file_name"];
                    string cacheKey = @"/" + documentsVDFromRoot + @"/" + containerName + @"/" + fileName;

                    switch (accessType)
                    {
                        case DocAccessType.Send:
                            if (dataStream != null)
                            {
                                byte[] _buffer = new byte[dataStream.Length];
                                
                                int _bytesRead = 0;

                                while ((_bytesRead = dataStream.Read(_buffer, 0, _buffer.Length)) != 0)
                                {                                    
                                    //Copy stream to bytearray to pass to method to write file so that the stream object can be 
                                    //released while writing to local file                                   

                                }

                                cache.Set(cacheKey, _buffer, policy);
                                
                                //Console.WriteLine("Cache Count After upload" + cache.GetCount() + ":Time " + DateTime.Now.ToString());
                                LifLogHandler.LogDebug("MemoryDoc Adapter- Data uploaded into memory :" + cacheKey,
               LifLogHandler.Layer.IntegrationLayer);
                                response = SUCCESSFUL_DATA_SENT;
                                _buffer = null;
                            }
                                                       
                            break;
                        case DocAccessType.Receive:
                            // response = ReceiveDocument(docDetails, response);

                            
                            byte[] DataStream = cache[cacheKey] as byte[];

                            if (DataStream != null)
                            {
                                LifLogHandler.LogDebug("MemoryDoc Adapter- Data downloaded from memory :" + cacheKey,
               LifLogHandler.Layer.IntegrationLayer);
                                Stream outDataStream = new MemoryStream(DataStream);
                                ReceiveEventArgs args = new ReceiveEventArgs();
                                args.ResponseDetails = new ListDictionary();
                                args.ResponseDetails.Add("DataStream", outDataStream);
                                args.ResponseDetails.Add("FileName", targetURLDetails["file_name"]);
                                args.ResponseDetails.Add("Response", SUCCESSFUL_DATA_RECEIVED);
                                args.ResponseDetails.Add("StatusCode", SUCCESSFUL_STATUS_CODE);
                                if (Received != null)
                                {
                                    Received(args);
                                }
                            } else {
                                response = UNSUCCESSFUL_DATA_RECEIVED ;
                                string errormsg = response + "Because data is not avavilable in memory cache:" + cacheKey;
                                //Console.WriteLine("errormsg:" + errormsg);
                                Exception exResponse = new Exception(errormsg);
                                throw exResponse;
                            }                        

                            break;
                        case DocAccessType.Delete:                           
                            if (cache.Contains(cacheKey))
                            {
                                cache.Remove(cacheKey);
                                response = SUCCESSFUL_DATA_DELETED;
                            }else
                            {
                                response = UNSUCCESSFUL_DATA_DELETED;
                            }
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                if (accessType == DocAccessType.Send)
                    response = UNSUCCESSFUL_DATA_SENT + ex.Message;
                else if (accessType == DocAccessType.Receive)
                    response = UNSUCCESSFUL_DATA_RECEIVED + ex.Message;
                else
                    response = ex.Message;

                if (ex.InnerException != null)
                {
                    response = response + ". Inner Error Message- " + ex.InnerException.Message;
                }

                //and then raise the event for receive operation
                LifLogHandler.LogError("MemoryDoc Adapter- HandleStream- exception raised, reason- {0}",
                    LifLogHandler.Layer.IntegrationLayer, ex);
                ReceiveEventArgs args = new ReceiveEventArgs();
                args.ResponseDetails = new ListDictionary();
                args.ResponseDetails.Add("Response", response);
                args.ResponseDetails.Add("StatusCode", UNSUCCESSFUL_STATUS_CODE);
                if (Received != null)
                {
                    Received(args);
                }
            }

            return response;
        }

        private static void OnExpire(CacheEntryRemovedArguments cacheEntryRemovedArguments)
        {
             LifLogHandler.LogDebug("MemoryDoc Adapter- CacheEntry OnExpire, Key - {0} Expired. Reason: {1}",
               LifLogHandler.Layer.IntegrationLayer, cacheEntryRemovedArguments.CacheItem.Key, cacheEntryRemovedArguments.RemovedReason.ToString());
            //Console.WriteLine($"Key [{cacheEntryRemovedArguments.CacheItem.Key}] Expired. Reason: {cacheEntryRemovedArguments.RemovedReason.ToString()}");
        }


    }



    enum DocAccessType
    {
        Send,
        Receive,
        Delete
    }
}
