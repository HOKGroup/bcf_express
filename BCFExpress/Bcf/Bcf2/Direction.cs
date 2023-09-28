namespace BCFExpress.Bcf.Bcf2
{
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.34234")]
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = true)]
    public class Direction
    {
        private string guidField = "";
        private double xField;
        private double yField;
        private double zField;

        public Direction()
        {
            this.guidField = System.Guid.NewGuid().ToString();
        }

        public Direction(Direction direction)
        {
            this.Guid = direction.Guid;
            this.X = direction.X;
            this.Y = direction.Y;
            this.Z = direction.Z;
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

        public virtual Direction Clone()
        {
            return ((Direction)(this.MemberwiseClone()));
        }
    }
}
