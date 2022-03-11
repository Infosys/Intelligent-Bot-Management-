

using System;
using System.Xml.Serialization;
using System.Collections;
using System.Xml.Schema;
using System.ComponentModel;

namespace Infosys.Lif.LegacyWorkbench.ReportManager
{

	public struct Declarations
	{
		public const string SchemaVersion = "";
	}


	[Serializable]
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public class FileCollection : ArrayList
	{
		public Infosys.Lif.LegacyWorkbench.ReportManager.File Add(Infosys.Lif.LegacyWorkbench.ReportManager.File obj)
		{
			base.Add(obj);
			return obj;
		}

		public Infosys.Lif.LegacyWorkbench.ReportManager.File Add()
		{
			return Add(new Infosys.Lif.LegacyWorkbench.ReportManager.File());
		}

		public void Insert(int index, Infosys.Lif.LegacyWorkbench.ReportManager.File obj)
		{
			base.Insert(index, obj);
		}

		public void Remove(Infosys.Lif.LegacyWorkbench.ReportManager.File obj)
		{
			base.Remove(obj);
		}

		new public Infosys.Lif.LegacyWorkbench.ReportManager.File this[int index]
		{
			get { return (Infosys.Lif.LegacyWorkbench.ReportManager.File) base[index]; }
			set { base[index] = value; }
		}
	}



	[XmlRoot(ElementName="File",IsNullable=false),Serializable]
	public class File
	{

		[XmlAttribute(AttributeName="Name",DataType="string")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public string __Name;
		
		[XmlIgnore]
		public string Name
		{ 
			get { return __Name; }
			set { __Name = value; }
		}

        [XmlAttribute(AttributeName = "Project", DataType = "string")]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public string __Project;

        [XmlIgnore]
        public string Project
        {
            get { return __Project; }
            set { __Project = value; }
        }

		[XmlAttribute(AttributeName="Group",DataType="string")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public string __Group;
		
		[XmlIgnore]
		public string Group
		{ 
			get { return __Group; }
			set { __Group = value; }
		}

		[XmlAttribute(AttributeName="Type",DataType="string")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public string __Type;
		
		[XmlIgnore]
		public string Type
		{ 
			get { return __Type; }
			set { __Type = value; }
		}

		[XmlAttribute(AttributeName="Location",DataType="string")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public string __Location;
		
		[XmlIgnore]
		public string Location
		{ 
			get { return __Location; }
			set { __Location = value; }
		}

        [XmlAttribute(AttributeName = "TimeTakenForGeneration", DataType = "string")]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public string __TimeTakenForGeneration;

        [XmlIgnore]
        public string TimeTakenForGeneration
        {
            get { return __TimeTakenForGeneration; }
            set { __TimeTakenForGeneration = value; }
        }

        [XmlAttribute(AttributeName = "CapersJonesConversionFactor", DataType = "decimal")]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public decimal __CapersJonesConversionFactor;

        [XmlIgnore]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public bool __CapersJonesConversionFactorSpecified;

        [XmlIgnore]
        public decimal CapersJonesConversionFactor
        {
            get { return __CapersJonesConversionFactor; }
            set { __CapersJonesConversionFactor = value; __CapersJonesConversionFactorSpecified = true; }
        }

        [XmlAttribute(AttributeName = "CustomConversionFactor", DataType = "decimal")]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public decimal __CustomConversionFactor;

        [XmlIgnore]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public bool __CustomConversionFactorSpecified;

        [XmlIgnore]
        public decimal CustomConversionFactor
        {
            get { return __CustomConversionFactor; }
            set { __CustomConversionFactor = value; __CustomConversionFactorSpecified = true; }
        }

		[XmlElement(ElementName="LinesOfCode",IsNullable=false,Form=XmlSchemaForm.Qualified,DataType="integer")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public string __LinesOfCode;
		
		[XmlIgnore]
		public string LinesOfCode
		{ 
			get { return __LinesOfCode; }
			set { __LinesOfCode = value; }
		}

		[XmlElement(ElementName="CommentsCount",IsNullable=false,Form=XmlSchemaForm.Qualified,DataType="integer")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public string __CommentsCount;
		
		[XmlIgnore]
		public string CommentsCount
		{ 
			get { return __CommentsCount; }
			set { __CommentsCount = value; }
		}

		[XmlElement(ElementName="BlankLinesCount",IsNullable=false,Form=XmlSchemaForm.Qualified,DataType="integer")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public string __WhiteSpaceCount;
		
		[XmlIgnore]
		public string BlankLinesCount
		{ 
			get { return __WhiteSpaceCount; }
			set { __WhiteSpaceCount = value; }
		}

		public File()
		{
		}
	}


	[XmlRoot(ElementName="ModelObjects",IsNullable=false),Serializable]
	public class ModelObjects
	{
		[System.Runtime.InteropServices.DispIdAttribute(-4)]
		public IEnumerator GetEnumerator() 
		{
			return FileCollection.GetEnumerator();
		}

		public Infosys.Lif.LegacyWorkbench.ReportManager.File Add(Infosys.Lif.LegacyWorkbench.ReportManager.File obj)
		{
			return FileCollection.Add(obj);
		}

		[XmlIgnore]
		public Infosys.Lif.LegacyWorkbench.ReportManager.File this[int index]
		{
			get { return (Infosys.Lif.LegacyWorkbench.ReportManager.File) FileCollection[index]; }
		}

		[XmlIgnore]
        public int Count 
		{
            get { return FileCollection.Count; }
        }

        public void Clear()
		{
            FileCollection.Clear();
        }

		public Infosys.Lif.LegacyWorkbench.ReportManager.File Remove(int index) 
		{ 
            Infosys.Lif.LegacyWorkbench.ReportManager.File obj = FileCollection[index];
            FileCollection.Remove(obj);
			return obj;
        }

        public void Remove(object obj)
		{
            FileCollection.Remove(obj);
        }

		[XmlAttribute(AttributeName="NumberOfFiles",DataType="integer")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public string __NumberOfFiles;
		
		[XmlIgnore]
		public string NumberOfFiles
		{ 
			get { return __NumberOfFiles; }
			set { __NumberOfFiles = value; }
		}

		[XmlElement(Type=typeof(Infosys.Lif.LegacyWorkbench.ReportManager.File),ElementName="File",IsNullable=false,Form=XmlSchemaForm.Qualified)]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public FileCollection __FileCollection;
		
		[XmlIgnore]
		public FileCollection FileCollection
		{
			get
			{
				if (__FileCollection == null) __FileCollection = new FileCollection();
				return __FileCollection;
			}
			set {__FileCollection = value;}
		}

		public ModelObjects()
		{
		}
	}


	[XmlRoot(ElementName="Serializers",IsNullable=false),Serializable]
	public class Serializers
	{

		[XmlAttribute(AttributeName="NumberOfFiles",DataType="integer")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public string __NumberOfFiles;
		
		[XmlIgnore]
		public string NumberOfFiles
		{ 
			get { return __NumberOfFiles; }
			set { __NumberOfFiles = value; }
		}

		[XmlElement(Type=typeof(Infosys.Lif.LegacyWorkbench.ReportManager.Contracts),ElementName="Contracts",IsNullable=false,Form=XmlSchemaForm.Qualified)]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public Infosys.Lif.LegacyWorkbench.ReportManager.Contracts __Contracts;
		
		[XmlIgnore]
		public Infosys.Lif.LegacyWorkbench.ReportManager.Contracts Contracts
		{
			get
			{
				if (__Contracts == null) __Contracts = new Infosys.Lif.LegacyWorkbench.ReportManager.Contracts();		
				return __Contracts;
			}
			set {__Contracts = value;}
		}

		[XmlElement(Type=typeof(Infosys.Lif.LegacyWorkbench.ReportManager.ModelObjects),ElementName="ModelObjects",IsNullable=false,Form=XmlSchemaForm.Qualified)]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public Infosys.Lif.LegacyWorkbench.ReportManager.ModelObjects __ModelObjects;
		
		[XmlIgnore]
		public Infosys.Lif.LegacyWorkbench.ReportManager.ModelObjects ModelObjects
		{
			get
			{
				if (__ModelObjects == null) __ModelObjects = new Infosys.Lif.LegacyWorkbench.ReportManager.ModelObjects();		
				return __ModelObjects;
			}
			set {__ModelObjects = value;}
		}

		public Serializers()
		{
		}
	}


	[XmlRoot(ElementName="DataEntities",IsNullable=false),Serializable]
	public class DataEntities
	{
        [XmlAttribute(AttributeName = "NumberOfFiles", DataType = "integer")]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public string __NumberOfFiles;

        [XmlIgnore]
        public string NumberOfFiles
        {
            get { return __NumberOfFiles; }
            set { __NumberOfFiles = value; }
        }

		[XmlElement(Type=typeof(Infosys.Lif.LegacyWorkbench.ReportManager.Contracts),ElementName="Contracts",IsNullable=false,Form=XmlSchemaForm.Qualified)]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public Infosys.Lif.LegacyWorkbench.ReportManager.Contracts __Contracts;
		
		[XmlIgnore]
		public Infosys.Lif.LegacyWorkbench.ReportManager.Contracts Contracts
		{
			get
			{
				if (__Contracts == null) __Contracts = new Infosys.Lif.LegacyWorkbench.ReportManager.Contracts();		
				return __Contracts;
			}
			set {__Contracts = value;}
		}

		[XmlElement(Type=typeof(Infosys.Lif.LegacyWorkbench.ReportManager.ModelObjects),ElementName="ModelObjects",IsNullable=false,Form=XmlSchemaForm.Qualified)]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public Infosys.Lif.LegacyWorkbench.ReportManager.ModelObjects __ModelObjects;
		
		[XmlIgnore]
		public Infosys.Lif.LegacyWorkbench.ReportManager.ModelObjects ModelObjects
		{
			get
			{
				if (__ModelObjects == null) __ModelObjects = new Infosys.Lif.LegacyWorkbench.ReportManager.ModelObjects();		
				return __ModelObjects;
			}
			set {__ModelObjects = value;}
		}

		public DataEntities()
		{
		}
	}


	[XmlRoot(ElementName="ServiceInterface",IsNullable=false),Serializable]
	public class ServiceInterface
	{
        [XmlAttribute(AttributeName = "NumberOfFiles", DataType = "integer")]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public string __NumberOfFiles;

        [XmlIgnore]
        public string NumberOfFiles
        {
            get { return __NumberOfFiles; }
            set { __NumberOfFiles = value; }
        }

		[System.Runtime.InteropServices.DispIdAttribute(-4)]
		public IEnumerator GetEnumerator() 
		{
			return FileCollection.GetEnumerator();
		}

		public Infosys.Lif.LegacyWorkbench.ReportManager.File Add(Infosys.Lif.LegacyWorkbench.ReportManager.File obj)
		{
			return FileCollection.Add(obj);
		}

		[XmlIgnore]
		public Infosys.Lif.LegacyWorkbench.ReportManager.File this[int index]
		{
			get { return (Infosys.Lif.LegacyWorkbench.ReportManager.File) FileCollection[index]; }
		}

		[XmlIgnore]
        public int Count 
		{
            get { return FileCollection.Count; }
        }

        public void Clear()
		{
            FileCollection.Clear();
        }

		public Infosys.Lif.LegacyWorkbench.ReportManager.File Remove(int index) 
		{ 
            Infosys.Lif.LegacyWorkbench.ReportManager.File obj = FileCollection[index];
            FileCollection.Remove(obj);
			return obj;
        }

        public void Remove(object obj)
		{
            FileCollection.Remove(obj);
        }

		[XmlAttribute(AttributeName="type",DataType="string")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public string __type;
		
		[XmlIgnore]
		public string type
		{ 
			get { return __type; }
			set { __type = value; }
		}

        //[XmlAttribute(AttributeName="NumberofFiles",DataType="integer")]
        //[EditorBrowsable(EditorBrowsableState.Advanced)]
        //public string __NumberofFiles;
		
        //[XmlIgnore]
        //public string NumberofFiles
        //{ 
        //    get { return __NumberofFiles; }
        //    set { __NumberofFiles = value; }
        //}

		[XmlElement(Type=typeof(Infosys.Lif.LegacyWorkbench.ReportManager.File),ElementName="File",IsNullable=false,Form=XmlSchemaForm.Qualified)]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public FileCollection __FileCollection;
		
		[XmlIgnore]
		public FileCollection FileCollection
		{
			get
			{
				if (__FileCollection == null) __FileCollection = new FileCollection();
				return __FileCollection;
			}
			set {__FileCollection = value;}
		}

		public ServiceInterface()
		{
		}
	}


	[XmlRoot(ElementName="LegacyWorkbenchReport",IsNullable=false),Serializable]
	public class LegacyWorkbenchReport
	{

		[XmlAttribute(AttributeName="TotalTimetaken",DataType="string")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public string __TotalTimetaken;
		
		[XmlIgnore]
		public string TotalTimetaken
		{ 
			get { return __TotalTimetaken; }
			set { __TotalTimetaken = value; }
		}

		[XmlAttribute(AttributeName="TotalNoOfFiles",DataType="integer")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
        public string __TotalNoOfFiles;
		
		[XmlIgnore]
        public string TotalNoOfFiles
		{
            get { return __TotalNoOfFiles; }
            set { __TotalNoOfFiles = value; }
		}

        /// <summary>
        /// //////changes added..remove if error occurs
        /// </summary>
        /// 
        [XmlElement(Type = typeof(Infosys.Lif.LegacyWorkbench.ReportManager.File), ElementName = "File", IsNullable = false, Form = XmlSchemaForm.Qualified)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public FileCollection __FileCollection;

        [XmlIgnore]
        public FileCollection FileCollection
        {
            get
            {
                if (__FileCollection == null) __FileCollection = new FileCollection();
                return __FileCollection;
            }
            set { __FileCollection = value; }
        }

		[XmlElement(Type=typeof(Infosys.Lif.LegacyWorkbench.ReportManager.Configurations),ElementName="Configurations",IsNullable=false,Form=XmlSchemaForm.Qualified)]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public Infosys.Lif.LegacyWorkbench.ReportManager.Configurations __Configurations;
		
		[XmlIgnore]
		public Infosys.Lif.LegacyWorkbench.ReportManager.Configurations Configurations
		{
			get
			{
				if (__Configurations == null) __Configurations = new Infosys.Lif.LegacyWorkbench.ReportManager.Configurations();		
				return __Configurations;
			}
			set {__Configurations = value;}
		}

		[XmlElement(Type=typeof(Infosys.Lif.LegacyWorkbench.ReportManager.DataEntities),ElementName="DataEntities",IsNullable=false,Form=XmlSchemaForm.Qualified)]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public Infosys.Lif.LegacyWorkbench.ReportManager.DataEntities __DataEntities;
		
		[XmlIgnore]
		public Infosys.Lif.LegacyWorkbench.ReportManager.DataEntities DataEntities
		{
			get
			{
				if (__DataEntities == null) __DataEntities = new Infosys.Lif.LegacyWorkbench.ReportManager.DataEntities();		
				return __DataEntities;
			}
			set {__DataEntities = value;}
		}

		[XmlElement(Type=typeof(Infosys.Lif.LegacyWorkbench.ReportManager.ServiceInterface),ElementName="ServiceInterface",IsNullable=false,Form=XmlSchemaForm.Qualified)]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public Infosys.Lif.LegacyWorkbench.ReportManager.ServiceInterface __ServiceInterface;
		
		[XmlIgnore]
		public Infosys.Lif.LegacyWorkbench.ReportManager.ServiceInterface ServiceInterface
		{
			get
			{
				if (__ServiceInterface == null) __ServiceInterface = new Infosys.Lif.LegacyWorkbench.ReportManager.ServiceInterface();		
				return __ServiceInterface;
			}
			set {__ServiceInterface = value;}
		}

		[XmlElement(Type=typeof(Infosys.Lif.LegacyWorkbench.ReportManager.Serializers),ElementName="Serializers",IsNullable=false,Form=XmlSchemaForm.Qualified)]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public Infosys.Lif.LegacyWorkbench.ReportManager.Serializers __Serializers;
		
		[XmlIgnore]
		public Infosys.Lif.LegacyWorkbench.ReportManager.Serializers Serializers
		{
			get
			{
				if (__Serializers == null) __Serializers = new Infosys.Lif.LegacyWorkbench.ReportManager.Serializers();		
				return __Serializers;
			}
			set {__Serializers = value;}
		}

		public LegacyWorkbenchReport()
		{
		}
	}


	[XmlRoot(ElementName="Contracts",IsNullable=false),Serializable]
	public class Contracts
	{
		[System.Runtime.InteropServices.DispIdAttribute(-4)]
		public IEnumerator GetEnumerator() 
		{
			return FileCollection.GetEnumerator();
		}

		public Infosys.Lif.LegacyWorkbench.ReportManager.File Add(Infosys.Lif.LegacyWorkbench.ReportManager.File obj)
		{
			return FileCollection.Add(obj);
		}

		[XmlIgnore]
		public Infosys.Lif.LegacyWorkbench.ReportManager.File this[int index]
		{
			get { return (Infosys.Lif.LegacyWorkbench.ReportManager.File) FileCollection[index]; }
		}

		[XmlIgnore]
        public int Count 
		{
            get { return FileCollection.Count; }
        }

        public void Clear()
		{
            FileCollection.Clear();
        }

		public Infosys.Lif.LegacyWorkbench.ReportManager.File Remove(int index) 
		{ 
            Infosys.Lif.LegacyWorkbench.ReportManager.File obj = FileCollection[index];
            FileCollection.Remove(obj);
			return obj;
        }

        public void Remove(object obj)
		{
            FileCollection.Remove(obj);
        }

		[XmlAttribute(AttributeName="NumberOfFiles",DataType="integer")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public string __NumberOfFiles;
		
		[XmlIgnore]
		public string NumberOfFiles
		{ 
			get { return __NumberOfFiles; }
			set { __NumberOfFiles = value; }
		}

		[XmlElement(Type=typeof(Infosys.Lif.LegacyWorkbench.ReportManager.File),ElementName="File",IsNullable=false,Form=XmlSchemaForm.Qualified)]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public FileCollection __FileCollection;
		
		[XmlIgnore]
		public FileCollection FileCollection
		{
			get
			{
				if (__FileCollection == null) __FileCollection = new FileCollection();
				return __FileCollection;
			}
			set {__FileCollection = value;}
		}

		public Contracts()
		{
		}
	}


	[XmlRoot(ElementName="Configurations",IsNullable=false),Serializable]
	public class Configurations
	{
		[System.Runtime.InteropServices.DispIdAttribute(-4)]
		public IEnumerator GetEnumerator() 
		{
			return FileCollection.GetEnumerator();
		}

		public Infosys.Lif.LegacyWorkbench.ReportManager.File Add(Infosys.Lif.LegacyWorkbench.ReportManager.File obj)
		{
			return FileCollection.Add(obj);
		}

		[XmlIgnore]
		public Infosys.Lif.LegacyWorkbench.ReportManager.File this[int index]
		{
			get { return (Infosys.Lif.LegacyWorkbench.ReportManager.File) FileCollection[index]; }
		}

		[XmlIgnore]
        public int Count 
		{
            get { return FileCollection.Count; }
        }

        public void Clear()
		{
            FileCollection.Clear();
        }

		public Infosys.Lif.LegacyWorkbench.ReportManager.File Remove(int index) 
		{ 
            Infosys.Lif.LegacyWorkbench.ReportManager.File obj = FileCollection[index];
            FileCollection.Remove(obj);
			return obj;
        }

        public void Remove(object obj)
		{
            FileCollection.Remove(obj);
        }

		[XmlAttribute(AttributeName="NumberOfFiles",DataType="integer")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public string __NumberOfFiles;
		
		[XmlIgnore]
		public string NumberOfFiles
		{ 
			get { return __NumberOfFiles; }
			set { __NumberOfFiles = value; }
		}

		[XmlElement(Type=typeof(Infosys.Lif.LegacyWorkbench.ReportManager.File),ElementName="File",IsNullable=false,Form=XmlSchemaForm.Qualified)]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public FileCollection __FileCollection;
		
		[XmlIgnore]
		public FileCollection FileCollection
		{
			get
			{
				if (__FileCollection == null) __FileCollection = new FileCollection();
				return __FileCollection;
			}
			set {__FileCollection = value;}
		}

		public Configurations()
		{
		}
	}
}
