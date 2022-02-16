/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
namespace Infosys.Solutions.Ainauto.Infrastructure.ProcessScheduler.Framework
{
    public interface IProcessHandler
    {
        //void Start(Drive[] drives, ModeType mode, string entityName, string id, string projectGUID, string organizationName );
        void Start(Drive[] drives, ModeType mode, string entityName, string id , int robotId, int runInstanceId, int robotTaskMapId);
    }
}
