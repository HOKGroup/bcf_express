namespace BCFExpress.Bcf.Bcf2
{
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.34234")]
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = true)]
    public class Line
    {
        private string guidField = "";
        private Point startPointField;
        private Point endPointField;

        private string viewPointGuidField = "";

        public Line()
        {
            this.guidField = System.Guid.NewGuid().ToString();
            this.endPointField = new Point();
            this.startPointField = new Point();
        }

        public Line(Line line)
        {
            this.Guid = line.Guid;
            this.EndPoint = line.EndPoint;
            this.StartPoint = line.StartPoint;
        }

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public string Guid
        {
            get { return this.guidField; }
            set { this.guidField = value; }
        }

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public Point StartPoint
        {
            get { return this.startPointField; }
            set { this.startPointField = value; }
        }

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public Point EndPoint
        {
            get { return this.endPointField; }
            set { this.endPointField = value; }
        }

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public string ViewPointGuid
        {
            get { return viewPointGuidField; }
            set { viewPointGuidField = value; }
        }

        public virtual Line Clone()
        {
            return ((Line)(this.MemberwiseClone()));
        }
    }
}
