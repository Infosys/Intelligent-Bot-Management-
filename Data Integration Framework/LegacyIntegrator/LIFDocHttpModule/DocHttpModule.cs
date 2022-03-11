/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Web;
using System.DirectoryServices;
using System.IO;
using System.Net;

namespace Infosys.Lif
{
    public class DocHttpModule : IHttpModule
    {
        private const string APPLICATION_TYPE = "lif_document_handler_as_blob";
        private string responseString = "File successfully uploaded.";
        private bool isSuccess = true;
        
        #region IHttpModule Members

        public void Dispose()
        {
            //throw new NotImplementedException();
        }

        public void Init(HttpApplication context)
        {
            context.BeginRequest += new EventHandler(context_BeginRequest);
            context.EndRequest += new EventHandler(context_EndRequest);
        }

        void context_EndRequest(object sender, EventArgs e)
        {
            HttpContext context = ((HttpApplication)sender).Context;
            context.Response.StatusDescription = responseString;
            if (isSuccess)
            {
                context.Response.StatusCode = 201;
                //context.Response.Status = "OK";
            }
            else
            {
                context.Response.StatusCode = 500;
            }
        }

        void context_BeginRequest(object sender, EventArgs e)
        {
            HttpContext context = ((HttpApplication)sender).Context;
            if (context.Request.RequestType == "POST")
            {
                if (context.Request.Headers.AllKeys.Contains("application_type") && context.Request.Headers["application_type"] == APPLICATION_TYPE)
                {
                    try
                    {
                        string containerName = context.Request.Headers["container_name"];
                        string fileName = context.Request.Headers["file_name"];
                        string documentsVDFromRoot = context.Request.Headers["documents_VD_from_Root"];
                        String strPath = @"IIS://localhost/W3SVC/6/Root/" + documentsVDFromRoot; //Documents;
                        DirectoryEntry documentsVD = new DirectoryEntry(strPath);

                        //get the physical path
                        string documentsRootPath = (string)documentsVD.Properties["Path"][0];

                        //check if the physical path for the new VD already exists
                        //and create the physical path if doesnot exist
                        string newVDPath = documentsRootPath + @"\" + containerName;
                        if (!System.IO.Directory.Exists(newVDPath))
                        {
                            System.IO.Directory.CreateDirectory(newVDPath);
                        }

                        //create the virtual directory if alreday doesnot exist
                        DirectoryEntries childVDs = documentsVD.Children;
                        //IEnumerable<DirectoryEntry> matchingVDs = childVDs.Cast<DirectoryEntry>().Where(v => v.Name == containerName);
                        IEnumerable<DirectoryEntry> matchingVDs = childVDs.Cast<DirectoryEntry>().Where(v => v.Name.ToLower() == containerName.ToLower());
                        if (matchingVDs == null || (matchingVDs != null && matchingVDs.Count() <= 0))
                        {
                            DirectoryEntry myDirectoryEntry = childVDs.Add(containerName, documentsVD.SchemaClassName);
                            myDirectoryEntry.Properties["Path"][0] = newVDPath;
                            myDirectoryEntry.Properties["AppFriendlyName"][0] = containerName;
                            myDirectoryEntry.CommitChanges();
                        }

                        //upload the file to the so-formed virtual directory
                        Stream inFileStream = context.Request.InputStream;
                        FileStream outFileStream = new FileStream(newVDPath + @"\" + fileName, FileMode.Create, FileAccess.Write);

                        //check if the header having the block size is provided
                        //default is 1 KB i.e. 1024 bytes
                        int blockSize = 1;
                        if (context.Request.Headers["block_size"] != null && int.Parse(context.Request.Headers["block_size"]) > 0)
                            blockSize = int.Parse(context.Request.Headers["block_size"]);
                        int blockSizeInBytes = blockSize * 1024;
                        byte[] buffer = new byte[blockSizeInBytes];
                        int bytesRead = 0;
                        while ((bytesRead = inFileStream.Read(buffer, 0, buffer.Length)) != 0)
                        {
                            outFileStream.Write(buffer, 0, bytesRead);
                        }
                        outFileStream.Close();
                        inFileStream.Close();

                        //end the request
                        isSuccess = true;
                        ((HttpApplication)sender).CompleteRequest();
                    }
                    catch (Exception ex)
                    {
                        responseString = ex.Message;
                        isSuccess = false;
                        if (ex.InnerException != null)
                        {
                            responseString = responseString + ". Inner Error Message- " + ex.InnerException.Message;
                        }
                    }
                }
            }
        }

        #endregion
    }
}
