/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using Infosys.Lif.LegacyCommon;
using Infosys.Lif.LegacyIntegratorService;

namespace Infosys.Lif
{
    public class IIS_DocAdapter : IAdapter
    {
        private const string REGION = "Region";
        private const string TRANSPORT_SECTION = "TransportSection";
        private const string DATA = "Data";
        private const string TARGETURLDETAILS = "TargetURLDetails";
        private const string SUCCESSFUL_DATA_SENT = "Document successfully uploaded.";
        private const string SUCCESSFUL_DATA_RECEIVED = "Document successfully downloaded.";
        private const string UNSUCCESSFUL_DATA_SENT = "Document couldn't be uploaded successfully. ";
        private const string UNSUCCESSFUL_DATA_RECEIVED = "Document couldn't be downloaded successfully. ";
        private const int SUCCESSFUL_STATUS_CODE = 0;
        private const int UNSUCCESSFUL_STATUS_CODE = 1000;

        private Stream dataStream = null;
        private NameValueCollection targetURLDetails;

        #region IAdapter Members

        public event ReceiveHandler Received;

        public string Send(System.Collections.Specialized.ListDictionary adapterDetails, string message)
        {
            string response = SUCCESSFUL_DATA_SENT;
            Infosys.Lif.LegacyIntegratorService.IISDoc transportSection = null;
            Infosys.Lif.LegacyIntegratorService.Region regionToBeUsed = null;
            try
            {
                LifLogHandler.LogDebug("IIS_Doc Adapter- Send called", LifLogHandler.Layer.IntegrationLayer);
                foreach (DictionaryEntry items in adapterDetails)
                {
                    if (items.Key.ToString() == REGION)
                    {
                        regionToBeUsed = items.Value as Region;
                    }
                    else if (items.Key.ToString() == TRANSPORT_SECTION)
                    {
                        transportSection = items.Value as Infosys.Lif.LegacyIntegratorService.IISDoc;
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
                    throw new LegacyException("IIS_Doc Adapter- File Stream and metadata details both cannot be empty.");

                // check for mandatory parameters
                bool check = targetURLDetails.AllKeys.Contains("UriScheme")
                            && !string.IsNullOrWhiteSpace(targetURLDetails["UriScheme"]);
                if (!check)
                    throw new LegacyException("IIS_Doc Adapter- 'UriScheme' missing in metadata.");

                check = targetURLDetails.AllKeys.Contains("RootDNS")
                                && !string.IsNullOrWhiteSpace(targetURLDetails["RootDNS"]);
                if (!check)
                    throw new LegacyException("IIS_Doc Adapter- 'RootDNS' missing in metadata.");

                check = targetURLDetails.AllKeys.Contains("container_name")
                                && !string.IsNullOrWhiteSpace(targetURLDetails["container_name"]);
                if (!check)
                    throw new LegacyException("IIS_Doc Adapter- 'container_name' missing in metadata.");

                check = targetURLDetails.AllKeys.Contains("file_name")
                                && !string.IsNullOrWhiteSpace(targetURLDetails["file_name"]);
                if (!check)
                    throw new LegacyException("IIS_Doc Adapter- 'file_name' missing in metadata.");

                // Validates whether TransportName specified in the region, exists in IISBlobDetails section.
                IISDocDetails docDetails = ValidateTransportName(transportSection, regionToBeUsed.TransportName);

                response = HandleStream(docDetails, DocAccessType.Send);
            }
            catch (LegacyException exception)
            {
                LifLogHandler.LogError("IIS_Doc Adapter- Send- exception raised, reason- " + exception.Message,
                    LifLogHandler.Layer.IntegrationLayer);
                throw exception;
            }
            catch (Exception exception)
            {
                LifLogHandler.LogError("IIS_Doc Adapter- Send- exception raised, reason- " + exception.Message,
                    LifLogHandler.Layer.IntegrationLayer);
                throw exception;
            }
            return response;
        }

        public void Receive(ListDictionary adapterDetails)
        {
            Infosys.Lif.LegacyIntegratorService.IISDoc transportSection = null;
            Infosys.Lif.LegacyIntegratorService.Region regionToBeUsed = null;
            try
            {
                LifLogHandler.LogDebug("IIS_Doc Adapter- Receive called", LifLogHandler.Layer.IntegrationLayer);
                foreach (DictionaryEntry items in adapterDetails)
                {
                    if (items.Key.ToString() == REGION)
                    {
                        regionToBeUsed = items.Value as Region;
                    }
                    else if (items.Key.ToString() == TRANSPORT_SECTION)
                    {
                        transportSection = items.Value as Infosys.Lif.LegacyIntegratorService.IISDoc;
                    }
                    else if (items.Key.ToString() == TARGETURLDETAILS)
                    {
                        targetURLDetails = items.Value as NameValueCollection;
                    }
                }

                //verify the data stream and the details regarding the taget URL
                if (targetURLDetails == null)
                    throw new LegacyException("IIS_Doc Adapter- Metadata details are not provided.");

                // check for mandatory parameters
                bool check = targetURLDetails.AllKeys.Contains("UriScheme")
                                && !string.IsNullOrWhiteSpace(targetURLDetails["UriScheme"]);
                if (!check)
                    throw new LegacyException("IIS_Doc Adapter- 'UriScheme' missing in metadata.");

                check = targetURLDetails.AllKeys.Contains("RootDNS")
                                && !string.IsNullOrWhiteSpace(targetURLDetails["RootDNS"]);
                if (!check)
                    throw new LegacyException("IIS_Doc Adapter- 'RootDNS' missing in metadata.");

                check = targetURLDetails.AllKeys.Contains("container_name")
                                && !string.IsNullOrWhiteSpace(targetURLDetails["container_name"]);
                if (!check)
                    throw new LegacyException("IIS_Doc Adapter- 'container_name' missing in metadata.");

                check = targetURLDetails.AllKeys.Contains("file_name")
                                && !string.IsNullOrWhiteSpace(targetURLDetails["file_name"]);
                if (!check)
                    throw new LegacyException("IIS_Doc Adapter- 'file_name' missing in metadata.");

                // Validates whether TransportName specified in the region, exists in IISBlobDetails section.
                IISDocDetails docDetails = ValidateTransportName(transportSection, regionToBeUsed.TransportName);

                HandleStream(docDetails, DocAccessType.Receive);

            }
            catch (LegacyException exception)
            {
                LifLogHandler.LogError("IIS_Doc Adapter- Receive- exception raised, reason- " + exception.Message,
                    LifLogHandler.Layer.IntegrationLayer);
                throw exception;
            }
            catch (Exception exception)
            {
                LifLogHandler.LogError("IIS_Doc Adapter- Receive- exception raised, reason- " + exception.Message,
                    LifLogHandler.Layer.IntegrationLayer);
                throw exception;
            }
        }

        public bool Delete(ListDictionary messageDetails)
        {
            // validate input
            var region = messageDetails[REGION] as Region;
            var transport = messageDetails[TRANSPORT_SECTION] as IISDoc;
            var targetDetails = messageDetails[TARGETURLDETAILS] as NameValueCollection;
            if (region == null
                || transport == null
                || targetDetails == null)
            {
                LifLogHandler.LogError(
                    "IIS_Doc Adapter- Delete- One of Region, Transport section, Target details is not passed!",
                    LifLogHandler.Layer.IntegrationLayer);

                throw new ArgumentException(string.Format("{0}, {1}, {2} are required for delete operation!",
                    REGION, TRANSPORT_SECTION, TARGETURLDETAILS));
            }

            // check for mandatory parameters
            bool check = targetURLDetails.AllKeys.Contains("UriScheme")
                        && !string.IsNullOrWhiteSpace(targetURLDetails["UriScheme"]);
            if (!check)
                throw new LegacyException("IIS_Doc Adapter- 'UriScheme' missing in metadata.");
            check = targetDetails.AllKeys.Contains("RootDNS")
                            && !string.IsNullOrWhiteSpace(targetDetails["RootDNS"]);
            if (!check)
                throw new LegacyException("IIS_Doc Adapter- 'RootDNS' missing in metadata.");

            check = targetDetails.AllKeys.Contains("container_name")
                            && !string.IsNullOrWhiteSpace(targetDetails["container_name"]);
            if (!check)
                throw new LegacyException("IIS_Doc Adapter- 'container_name' missing in metadata.");

            check = targetDetails.AllKeys.Contains("company_id")
                            && !string.IsNullOrWhiteSpace(targetDetails["company_id"]);
            if (!check)
                throw new LegacyException("IIS_Doc Adapter- 'company_id' missing in metadata.");

            // Validates whether TransportName specified in the region, exists in IISBlobDetails section.
            IISDocDetails docDetails = ValidateTransportName(transport, region.TransportName);

            if (docDetails == null)
                throw new LegacyException(
                    string.Format("IIS_Doc Adapter- Could not find transport by name: {0}", region.TransportName));

            // delete the container
            var response = HandleStream(docDetails, DocAccessType.Delete, targetDetails);

            LifLogHandler.LogDebug("Response for delete: {0}", LifLogHandler.Layer.IntegrationLayer, response);

            // return true if response string is empty otherwise false
            return string.IsNullOrWhiteSpace(response);
        }

        #endregion

        private string HandleStream(
            IISDocDetails docDetails, DocAccessType accessType, NameValueCollection targetDetails = null)
        {
            LifLogHandler.LogDebug("IIS_Doc Adapter- HandleStream called for access type- " + accessType.ToString(),
                LifLogHandler.Layer.IntegrationLayer);
            string response = "";
            try
            {
                if (docDetails != null)
                {
                    switch (accessType)
                    {
                        case DocAccessType.Send:
                            response = SendDocument(docDetails, response);
                            break;
                        case DocAccessType.Receive:
                            response = ReceiveDocument(docDetails, response);
                            break;
                        case DocAccessType.Delete:
                            response = DeleteDocument(docDetails, targetDetails);
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
                LifLogHandler.LogError("IIS_Doc Adapter- HandleStream- exception raised, reason- {0}",
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


        /// <summary>
        /// 
        /// </summary>
        /// <param name="docDetails"></param>
        /// <param name="targetDetails"></param>
        /// <returns></returns>
        private string DeleteDocument(IISDocDetails docDetails, NameValueCollection targetDetails)
        {
            // validate input for delete specific parameters
            if (string.IsNullOrWhiteSpace(targetDetails["url_suffix"]))
                throw new Exception("url_suffix value missing in parameters.");

            // build uri to send request to
            var uri = new UriBuilder();
            switch (targetURLDetails["UriScheme"].ToLower())
            {
                case "https":
                    uri.Scheme = Uri.UriSchemeHttps;
                    uri.Port = int.Parse(targetURLDetails["Port"] ?? "443");
                    break;
                default:
                    uri.Scheme = Uri.UriSchemeHttp;
                    uri.Port = int.Parse(targetURLDetails["Port"] ?? "80");
                    break;

            }
            uri.Host = targetDetails["RootDNS"];
            uri.Path = Path.Combine(docDetails.DocumentsVirtualDirectoryFromRoot, targetDetails["url_suffix"]);

            WebRequest request = HttpWebRequest.Create(uri.Uri);
            //to handle the PPTWare task- 7911, 7910
            request.UseDefaultCredentials = true;
            //request.Credentials = CredentialCache.DefaultNetworkCredentials;

            //Issure observed when testing with Mask Detector blob storage access.Added to handle 401 Authorization challenge on each request.
            //Placing this only first request should 
            //get 401 then subsequent request should not do an Authorization challenge and again raise 401 errors
            request.PreAuthenticate = true;
            request.Method = "DELETE";

            // pass headers received in target url details
            foreach (var key in targetDetails.AllKeys)
            {
                LifLogHandler.LogDebug("IIS_Doc Adapter- Adding Key: {0}\tValue: {1}",
                    LifLogHandler.Layer.IntegrationLayer,
                    key, targetDetails[key]);
                request.Headers.Add(key, targetDetails[key]);
            }

            // Send delete request
            LifLogHandler.LogDebug("IIS_Doc Adapter- HandleStream- trying to delete container...",
                LifLogHandler.Layer.IntegrationLayer);
            using (HttpWebResponse response = (HttpWebResponse) request.GetResponse())
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    LifLogHandler.LogDebug("IIS_Doc Adapter- HandleStream- file deleted",
                        LifLogHandler.Layer.IntegrationLayer);
                    return null;
                }
                else
                {
                    LifLogHandler.LogError("Server rejected delete request with status: {0} - {1}",
                        LifLogHandler.Layer.IntegrationLayer, response.StatusCode, response.StatusDescription);

                    return string.Format("{0}-{1}", response.StatusCode, response.StatusDescription);
                }
            }
        }

        private string ReceiveDocument(IISDocDetails docDetails, string response)
        {
            //format the receive url
            LifLogHandler.LogDebug(
                "IIS_Doc Adapter- ReceiveDocument- formating web request for RECEIVE operation...",
                LifLogHandler.Layer.IntegrationLayer);

            var regionId = targetURLDetails["region_id"];

            var uri = new UriBuilder();
            switch (targetURLDetails["UriScheme"].ToLower())
            {
                case "https":
                    uri.Scheme = Uri.UriSchemeHttps;
                    uri.Port = int.Parse(targetURLDetails["Port"] ?? "443");
                    break;
                default:
                    uri.Scheme = Uri.UriSchemeHttp;
                    uri.Port = int.Parse(targetURLDetails["Port"] ?? "80");
                    break;

            }
            uri.Host = targetURLDetails["RootDNS"];
            // include region id in path, if not empty
            uri.Path = string.IsNullOrWhiteSpace(regionId) ?
                Path.Combine(docDetails.DocumentsVirtualDirectoryFromRoot,
                                targetURLDetails["container_name"],
                                targetURLDetails["file_name"]) :
                Path.Combine(docDetails.DocumentsVirtualDirectoryFromRoot,
                                regionId,
                                targetURLDetails["container_name"],
                                targetURLDetails["file_name"]);

            WebRequest requestReceive = HttpWebRequest.Create(uri.Uri);

            //to handle the PPTWare task- 7911, 7910
            requestReceive.UseDefaultCredentials = true;
            //Issure observed when testing with Mask Detector blob storage access.Added to handle 401 Authorization challenge on each request.
            //Placing this only first request should 
            //get 401 then subsequent request should not do an Authorization challenge and again raise 401 errors
            requestReceive.PreAuthenticate = true;
            //request.Credentials = CredentialCache.DefaultNetworkCredentials;
            requestReceive.Method = "GET";
            // pass headers received in target url details
            foreach (var key in targetURLDetails.AllKeys)
            {
                LifLogHandler.LogDebug("IIS_Doc Adapter- Adding Key: {0}\tValue: {1}", LifLogHandler.Layer.IntegrationLayer,
                    key, targetURLDetails[key]);
                // ignore fields with no value
                if (string.IsNullOrWhiteSpace(targetURLDetails[key]))
                    continue;
                //encoding to Base64 to support multiple languages
                string value = EncodeStringToBase64(targetURLDetails[key]);
                requestReceive.Headers.Add(key, value);
            }
            using (HttpWebResponse responseReceive = (HttpWebResponse) requestReceive.GetResponse())
            {
                if (responseReceive.StatusCode == HttpStatusCode.OK)
                {
                    response = SUCCESSFUL_DATA_RECEIVED;
                    LifLogHandler.LogDebug(
                        "IIS_Doc Adapter- ReceiveDocument- file received and accordingly raising Received event...",
                        LifLogHandler.Layer.IntegrationLayer);
                    Stream inFileStream = responseReceive.GetResponseStream();
                    LifLogHandler.LogDebug(
                       "IIS_Doc Adapter- ReceiveDocument- Response Uri- {0}",
                       LifLogHandler.Layer.IntegrationLayer, responseReceive.ResponseUri);
                    LifLogHandler.LogDebug(
                        "IIS_Doc Adapter- ReceiveDocument- Content length- {0}",
                        LifLogHandler.Layer.IntegrationLayer, responseReceive.ContentLength.ToString());
                    LifLogHandler.LogDebug(
                       "IIS_Doc Adapter- ReceiveDocument- Status Code- {0}",
                       LifLogHandler.Layer.IntegrationLayer, responseReceive.StatusCode);
                    LifLogHandler.LogDebug(
                      "IIS_Doc Adapter- ReceiveDocument- Status Description- {0}",
                      LifLogHandler.Layer.IntegrationLayer, responseReceive.StatusDescription);
                    //raise the Received event
                    ReceiveEventArgs args = new ReceiveEventArgs();
                    args.ResponseDetails = new ListDictionary();
                    args.ResponseDetails.Add("DataStream", inFileStream);
                    args.ResponseDetails.Add("FileName", targetURLDetails["file_name"]);
                    args.ResponseDetails.Add("Response", SUCCESSFUL_DATA_RECEIVED);
                    args.ResponseDetails.Add("StatusCode", SUCCESSFUL_STATUS_CODE);
                    if (Received != null)
                    {
                        Received(args);
                    }
                }
                else
                {
                    response = UNSUCCESSFUL_DATA_RECEIVED + ". " + responseReceive.StatusDescription;
                    Exception exResponse = new Exception(response);
                    throw exResponse;
                }
            }
            return response;
        }

        private string SendDocument(IISDocDetails docDetails, string response)
        {
            LifLogHandler.LogDebug(
                "IIS_Doc Adapter- HandleStream- formating web request for SEND operation...",
                LifLogHandler.Layer.IntegrationLayer);

            // build uri to send request to
            var uri = new UriBuilder();
            switch (targetURLDetails["UriScheme"].ToLower())
            {
                case "https":
                        uri.Scheme = Uri.UriSchemeHttps;
                        uri.Port = int.Parse(targetURLDetails["Port"] ?? "443");
                        break;
                default:
                        uri.Scheme = Uri.UriSchemeHttp;
                        uri.Port = int.Parse(targetURLDetails["Port"] ?? "80");
                        break;

            }

            uri.Host = targetURLDetails["RootDNS"];
            uri.Path = docDetails.DocumentsVirtualDirectoryFromRoot;
            WebRequest request = HttpWebRequest.Create(uri.Uri);
            //to handle the PPTWare task- 7911, 7910
            request.UseDefaultCredentials = true;
            //request.Credentials = CredentialCache.DefaultNetworkCredentials;
            //Issure observed when testing with Mask Detector blob storage access.Added to handle 401 Authorization challenge on each request.
            //Placing this only first request should 
            //get 401 then subsequent request should not do an Authorization challenge and again raise 401 errors
            request.PreAuthenticate = true;
            request.Method = "POST";

            // add headers
            LifLogHandler.LogDebug("IIS_Doc Adapter- Adding Key: {0}\tValue: {1}", LifLogHandler.Layer.IntegrationLayer,
                "application_type", "lif_document_handler_as_blob");
            request.Headers.Add("application_type", EncodeStringToBase64("lif_document_handler_as_blob"));
            LifLogHandler.LogDebug("IIS_Doc Adapter- Adding Key: {0}\tValue: {1}", LifLogHandler.Layer.IntegrationLayer,
                "block_size", docDetails.DataBlockSize);
            request.Headers.Add("block_size", EncodeStringToBase64(docDetails.DataBlockSize.ToString()));
            LifLogHandler.LogDebug("IIS_Doc Adapter- Adding Key: {0}\tValue: {1}", LifLogHandler.Layer.IntegrationLayer,
                "documents_VD_from_Root", docDetails.DocumentsVirtualDirectoryFromRoot);
            request.Headers.Add("documents_VD_from_Root",
                EncodeStringToBase64(docDetails.DocumentsVirtualDirectoryFromRoot));
            // pass headers received in target url details
            foreach (var key in targetURLDetails.AllKeys)
            {
                LifLogHandler.LogDebug("IIS_Doc Adapter- Adding Key: {0}\tValue: {1}", LifLogHandler.Layer.IntegrationLayer,
                    key, targetURLDetails[key]);
                // ignore fields with no value
                if (string.IsNullOrWhiteSpace(targetURLDetails[key]))
                    continue;
                //encoding to Base64 to support multiple languages
                string value = EncodeStringToBase64(targetURLDetails[key]);
                request.Headers.Add(key, value);
            }

            Stream outFileStream = null;
            // check if file is to be passed in request body
            if (dataStream != null)
            {
                // open request stream
                outFileStream = request.GetRequestStream();
                // write to request stream in chuncks, so that data is sent to server in chunks
                int blockSize = 1;
                if (docDetails.DataBlockSize > 0)
                    blockSize = docDetails.DataBlockSize;
                int blockSizeInBytes = blockSize * 1024;
                byte[] buffer = new byte[blockSizeInBytes];
                int bytesRead = 0;
                dataStream.Position = 0;
                while ((bytesRead = dataStream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    outFileStream.Write(buffer, 0, bytesRead);
                }
            }
            // finish uploading process
            LifLogHandler.LogDebug("IIS_Doc Adapter- HandleStream- trying to upload the file stream...",
                LifLogHandler.Layer.IntegrationLayer);
            using (HttpWebResponse responseSent = (HttpWebResponse) request.GetResponse())
            {
                if (responseSent.StatusCode == HttpStatusCode.OK || responseSent.StatusCode == HttpStatusCode.Created)
                {
                    response = SUCCESSFUL_DATA_SENT;
                    LifLogHandler.LogDebug("IIS_Doc Adapter- HandleStream- file uploaded",
                        LifLogHandler.Layer.IntegrationLayer);
                }
                else
                {
                    response = UNSUCCESSFUL_DATA_SENT + ". " + responseSent.StatusDescription;
                    Exception exResponse = new Exception(response);
                    throw exResponse;
                }
            }

            if (outFileStream != null)
                outFileStream.Close();
            if (dataStream != null)
                dataStream.Close();
            return response;
        }

        private string EncodeStringToBase64(string key)
        {
            byte[] toEncodeAsBytes = System.Text.Encoding.Unicode.GetBytes(key);
            string value = System.Convert.ToBase64String(toEncodeAsBytes);
            value = "=?utf-8?B?" + value + "?=";
            return value;
        }

        /// <summary>
        /// Validates whether TransportName specified in the region, exists in IISDocDetails
        /// section. If it found, it returns corresponding IISBlobDetails object.
        /// </summary>
        /// <param name="transportSection">MSMQ section</param>
        /// <param name="transportName">name of the transport</param>
        private IISDocDetails ValidateTransportName(Infosys.Lif.LegacyIntegratorService.IISDoc transportSection,
            string transportName)
        {
            LifLogHandler.LogDebug("IIS_Doc Adapter- ValidateTransportName called...",
                LifLogHandler.Layer.IntegrationLayer);
            IISDocDetails blobDetails = null;
            bool isTransportNameExists = false;
            // Find the IBMMQ region to which it should connect for sending message.
            for (int count = 0; count < transportSection.IISDocDetailsCollection.Count; count++)
            {
                blobDetails = transportSection.IISDocDetailsCollection[count] as IISDocDetails;
                if (blobDetails.TransportName == transportName)
                {
                    isTransportNameExists = true;
                    break;
                }
            }
            // If MSMQ region is not set in the config then throw the exception
            if (!isTransportNameExists)
            {
                throw new LegacyException("IIS_Doc Adapter- " + transportName + " is not defined in MSMQDetails section");
            }
            return blobDetails;
        }
    }

    enum DocAccessType
    {
        Send,
        Receive,
        Delete
    }
}
