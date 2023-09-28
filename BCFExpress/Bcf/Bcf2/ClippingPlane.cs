namespace BCFExpress.Bcf.Bcf2
{
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.34234")]
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = true)]
    public class ClippingPlane
    {
        private string guidField = "";
        private Point locationField;
        private Direction directionField;
        private string viewPointGuidField = "";

        public ClippingPlane()
        {
            this.guidField = System.Guid.NewGuid().ToString();
            this.directionField = new Direction();
            this.locationField = new Point();
        }

        public ClippingPlane(ClippingPlane clippingPlane)
        {
            this.Guid = clippingPlane.Guid;
            this.Location = clippingPlane.Location;
            this.Direction = clippingPlane.Direction;
        }

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public string Guid
        {
            get { return this.guidField; }
            set { this.guidField = value; }
        }

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public Point Location
        {
            get { return this.locationField; }
            set { this.locationField = value; }
        }

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public Direction Direction
        {
            get { return this.directionField; }
            set { this.directionField = value; }
        }

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public string ViewPointGuid
        {
            get { return this.viewPointGuidField; }
            set { this.viewPointGuidField = value; }
        }

        public virtual ClippingPlane Clone()
        {
            return ((ClippingPlane)(this.MemberwiseClone()));
        }
    }
}
