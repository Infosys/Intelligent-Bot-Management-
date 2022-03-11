/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infosys.Solutions.ConfigurationManager.Infrastructure.Common
{
    public struct ApplicationConstants
    {

       /* public const string APP_NAME = "DIF";
        public const string DOCUMENTSTORE_KEY = "ImageFinderStore";
        public const string SERVICE_EXCEPTIONHANDLING_POLICY = "DIF.SERVICE";
        public const string ROBOT_CONFIGURATION_SERVICEINTERFACE = "/DocumentImageFinder/RobotConfiguration.svc";
        public const string CRAWLER_SERVICEINTERFACE = "/DocumentImageFinder/Crawler.svc";
        public const string IMAGE_RECOGNITION_SERVICEINTERFACE = "/DocumentImageFinder/ImageRecognition.svc";
        public const string SECURE_PASSCODE = "IAP2GO_SEC!URE";
        public const string JPEG_FILE_TYPE = "jpg";
        public const string XML_FILE_TYPE = "xml";
        public const string VALIDATION_ERROR_CODE = "1042";*/
        public const string WORKER_EXCEPTION_HANDLING_POLICY = "DocumentImageFinder.Worker";
        public const string SERVICE_EXCEPTIONHANDLING_POLICY = "Superbot.Service";

    }

    public enum ResourceLevel
    {
        LEVEL0,
        LEVEL1,
        LEVEL2
    }
  }

