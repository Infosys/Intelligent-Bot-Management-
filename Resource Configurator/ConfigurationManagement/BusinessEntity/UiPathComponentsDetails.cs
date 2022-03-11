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

namespace Infosys.Solutions.Ainauto.ConfigurationManager.BusinessEntity
{
    public class UiPathRobot
    {
        public string MachineName { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Id { get; set; }
    }
    public class UiPathProcess
    {
        public string Key { get; set; }
        public string Name { get; set; }
        public string Id { get; set; }
        public string ProcessType { get; set; }
    }
}
