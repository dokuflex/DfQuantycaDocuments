using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DfQuantycaDocuments.Models
{

    // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class Index
    {

        private IndexFolderLevel1[] folderLevel1Field;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("FolderLevel1")]
        public IndexFolderLevel1[] FolderLevel1
        {
            get
            {
                return this.folderLevel1Field;
            }
            set
            {
                this.folderLevel1Field = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class IndexFolderLevel1
    {

        private string masterIdField;

        private string nameField;

        private string detailField;

        private string pathField;

        private IndexFolderLevel1FolderLevel2[] folderLevel2Field;

        /// <remarks/>
        public string MasterId
        {
            get
            {
                return this.masterIdField;
            }
            set
            {
                this.masterIdField = value;
            }
        }

        /// <remarks/>
        public string Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        public string Detail
        {
            get
            {
                return this.detailField;
            }
            set
            {
                this.detailField = value;
            }
        }

        /// <remarks/>
        public string Path
        {
            get
            {
                return this.pathField;
            }
            set
            {
                this.pathField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("FolderLevel2")]
        public IndexFolderLevel1FolderLevel2[] FolderLevel2
        {
            get
            {
                return this.folderLevel2Field;
            }
            set
            {
                this.folderLevel2Field = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class IndexFolderLevel1FolderLevel2
    {

        private string codeField;

        private string nameField;

        private string pathField;

        /// <remarks/>
        public string Code
        {
            get
            {
                return this.codeField;
            }
            set
            {
                this.codeField = value;
            }
        }

        /// <remarks/>
        public string Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        public string Path
        {
            get
            {
                return this.pathField;
            }
            set
            {
                this.pathField = value;
            }
        }
    }


}