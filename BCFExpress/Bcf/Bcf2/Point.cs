namespace BCFExpress.Bcf.Bcf2
{
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.34234")]
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = true)]
    public class Point
    {
        private string guidField = "";
        private double xField;
        private double yField;
        private double zField;

        public Point()
        {
            this.guidField = System.Guid.NewGuid().ToString();
        }

        public Point(Point point)
        {
            this.Guid = point.Guid;
            this.X = point.X;
            this.Y = point.Y;
            this.Z = point.Z;

        }

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public string Guid
        {
            get { return this.guidField; }
            set { this.guidField = value; }
        }

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public double X
        {
            get { return this.xField; }
            set { this.xField = value; }
        }

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public double Y
        {
            get { return this.yField; }
            set { this.yField = value; }
        }

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public double Z
        {
            get { return this.zField; }
            set { this.zField = value; }
        }

        public virtual Point Clone()
        {
            return ((Point)(this.MemberwiseClone()));
        }
    }
}
