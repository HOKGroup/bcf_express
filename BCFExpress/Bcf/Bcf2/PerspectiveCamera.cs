namespace BCFExpress.Bcf.Bcf2
{
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.34234")]
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = true)]
    public class PerspectiveCamera
    {
        private string guidField = "";
        private Point cameraViewPointField;
        private Direction cameraDirectionField;
        private Direction cameraUpVectorField;
        private double fieldOfViewField;
        private string viewPointGuidField = "";

        public PerspectiveCamera()
        {
            this.guidField = System.Guid.NewGuid().ToString();
            this.cameraUpVectorField = new Direction();
            this.cameraDirectionField = new Direction();
            this.cameraViewPointField = new Point();
        }

        public PerspectiveCamera(PerspectiveCamera persCamera)
        {
            this.Guid = persCamera.Guid;
            this.CameraViewPoint = persCamera.CameraViewPoint;
            this.CameraDirection = persCamera.CameraDirection;
            this.CameraUpVector = persCamera.CameraUpVector;
            this.FieldOfView = persCamera.FieldOfView;
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
        public double FieldOfView
        {
            get { return this.fieldOfViewField; }
            set { this.fieldOfViewField = value; }
        }

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public string ViewPointGuid
        {
            get { return viewPointGuidField; }
            set { viewPointGuidField = value; }
        }

        public virtual PerspectiveCamera Clone()
        {
            return ((PerspectiveCamera)(this.MemberwiseClone()));
        }
    }
}
