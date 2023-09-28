
using System;
using System.ComponentModel;

namespace BCFExpress.Bcf.Bcf2
{
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.34234")]
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = true)]
    public class Component : INotifyPropertyChanged
    {
        private string guidField = "";
        private string originatingSystemField = "";
        private string authoringToolIdField = "";
        private string ifcGuidField = "";
        private bool selectedField;
        private bool selectedFieldSpecified;
        private bool visibleField;
        private byte[] colorField;
        private string viewPointGuidField = "";

        //extension
        private string elementName = "";
        private RevitExtension action = new RevitExtension();
        private RevitExtension responsibility = new RevitExtension();

        public Component()
        {
            this.guidField = System.Guid.NewGuid().ToString();
            this.visibleField = true;
        }

        public Component(Component component)
        {
            this.Guid = component.Guid;
            this.OriginatingSystem = component.OriginatingSystem;
            this.AuthoringToolId = component.AuthoringToolId;
            this.IfcGuid = component.IfcGuid;
            this.Selected = component.Selected;
            this.SelectedSpecified = component.SelectedSpecified;
            this.Visible = component.Visible;
            this.Color = component.Color;
        }

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public string Guid
        {
            get { return this.guidField; }
            set { this.guidField = value; NotifyPropertyChanged("Guid"); }
        }

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string OriginatingSystem
        {
            get { return this.originatingSystemField; }
            set { this.originatingSystemField = value; NotifyPropertyChanged("OriginatingSystem"); }
        }

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string AuthoringToolId
        {
            get { return this.authoringToolIdField; }
            set { this.authoringToolIdField = value; NotifyPropertyChanged("AuthoringToolId"); }
        }

        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "normalizedString")]
        public string IfcGuid
        {
            get { return this.ifcGuidField; }
            set { this.ifcGuidField = value; NotifyPropertyChanged("IfcGuid"); }
        }

        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool Selected
        {
            get { return this.selectedField; }
            set { this.selectedField = value; NotifyPropertyChanged("Selected"); }

        }

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool SelectedSpecified
        {
            get { return this.selectedFieldSpecified; }
            set { this.selectedFieldSpecified = value; NotifyPropertyChanged("SelectedSpecified"); }
        }

        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(true)]
        public bool Visible
        {
            get { return this.visibleField; }
            set { this.visibleField = value; NotifyPropertyChanged("Visible"); }
        }

        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "hexBinary")]
        public byte[] Color
        {
            get { return this.colorField; }
            set { this.colorField = value; NotifyPropertyChanged("Color"); }
        }

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public string ViewPointGuid
        {
            get { return this.viewPointGuidField; }
            set { this.viewPointGuidField = value; NotifyPropertyChanged("ViewPointGuid"); }
        }

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public string ElementName { get { return elementName; } set { elementName = value; NotifyPropertyChanged("ElementName"); } }

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public RevitExtension Action
        {
            get { return action; }
            set
            {
                if (null != value)
                {
                    action = value;
                    if (null != action.Color)
                    {
                        this.Color = action.Color;
                    }
                }

                NotifyPropertyChanged("Action");
            }
        }

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public RevitExtension Responsibility
        {
            get { return responsibility; }
            set
            {
                if (null != value)
                {
                    responsibility = value;
                    NotifyPropertyChanged("Responsibility");
                }
            }
        }

        public virtual Component Clone()
        {
            return ((Component)(this.MemberwiseClone()));
        }

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
    }
}
