namespace BCFExpress.Bcf.Bcf2
{
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.34234")]
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = true)]
    public class OrthogonalCamera
    {
        private string guidField = "";
        private Point cameraViewPointField;
        private Direction cameraDirectionField;
        private Direction cameraUpVectorField;
        private double viewToWorldScaleField;
        private string viewPointGuidField = "";

        public OrthogonalCamera()
        {
            this.guidField = System.Guid.NewGuid().ToString();
            this.cameraUpVectorField = new Direction();
            this.cameraDirectionField = new Direction();
            this.cameraViewPointField = new Point();
        }

        public OrthogonalCamera(OrthogonalCamera orthoCamera)
        {
            this.Guid = orthoCamera.Guid;
            this.CameraUpVector = orthoCamera.CameraUpVector;
            this.CameraDirection = orthoCamera.CameraDirection;
            this.CameraViewPoint = orthoCamera.CameraViewPoint;
        }

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public string Guid
        {
            get { return this.guidField; }
            set { this.guidField = value; }
        }

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public Point CameraViewPoint
        {
            get { return this.cameraViewPointField; }
            set { this.cameraViewPointField = value; }
        }

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public Direction CameraDirection
        {
            get { return this.cameraDirectionField; }
            set { this.cameraDirectionField = value; }
        }

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public Direction CameraUpVector
        {
            get { return this.cameraUpVectorField; }
            set { this.cameraUpVectorField = value; }
        }

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public double ViewToWorldScale
        {
            get { return this.viewToWorldScaleField; }
            set { this.viewToWorldScaleField = value; }
        }

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public string ViewPointGuid
        {
            get { return this.viewPointGuidField; }
            set { this.viewPointGuidField = value; }
        }

        public virtual OrthogonalCamera Clone()
        {
            return ((OrthogonalCamera)(this.MemberwiseClone()));
        }
    }
}
