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
    #region getPlatformDetails Model

    public class Resourcetype_detail
    {
        public string resourcetypename { get; set; }
    }

    public class Platform_Info
    {
        public string platformname { get; set; }
        public List<Resourcetype_detail> resourcetypedetails { get; set; }
    }
    #endregion

    public class ResourceTypeMetadata
    {
        public string platformid { get; set; }
        public string tenantid { get; set; }
        public string platformtype { get; set; }
        public List<Resourcetypedetails> resourcetypedetails { get; set; }
    }

    public class Parent_detail
    {
        public string resourcetypename { get; set; }
        public string resourcetypeid { get; set; }
        public string portfolioid { get; set; }
    }

    public class Child_detail
    {
        public string resourcetypename { get; set; }
        public string resourcetypeid { get; set; }
        public string portfolioid { get; set; }
        public int priority { get; set; }
    }

    public class Resourcetypedata
    {
        public bool issecret { get; set; }

        public string attributename { get; set; }
        public string DefaultValue { get; set; }
        public string attributetype { get; set; }
        public string description { get; set; }
        public string displayname { get; set; }
        public bool ismandatory { get; set; }
        public string Sequence { get; set; }
    }

    public class Logdetails
    {
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public DateTime ValidityStart { get; set; }
        public DateTime ValidityEnd { get; set; }
    }

    public class Resourcetypedetails
    {
        public string resourcetypename { get; set; }
        public string resourcetypeid { get; set; }
        public bool ismainentry { get; set; }
        public bool ismappingdeleted { get; set; }
        public bool isnewmapping { get; set; }
        public string portfolioid { get; set; }
        public List<Parent_detail> parentdetails { get; set; }
        public List<Child_detail> childdetails { get; set; }
        public List<Resourcetypedata> resourcetypemetadata { get; set; }
        public Logdetails logdetails { get; set; }
    }

    public class ResourceType_Attribute
    {
        public string resourcetypeid { get; set; }
        public string attributename { get; set; }
        public string defaultvalue { get; set; }
        public string description { get; set; }
        public string attributetype { get; set; }
        public string displayname { get; set; }
        public bool issecret { get; set; }
        

    }
}
