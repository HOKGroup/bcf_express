
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCFExpress.Bcf.Bcf2
{
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.34234")]
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public class VisualizationInfo : INotifyPropertyChanged
    {
        private ObservableCollection<Component> componentsField;
        private OrthogonalCamera orthogonalCameraField;
        private PerspectiveCamera perspectiveCameraField;
        private List<Line> linesField;
        private List<ClippingPlane> clippingPlanesField;
        private List<VisualizationInfoBitmaps> bitmapsField;
        private string viewPointGuidField = "";
        private bool isPerspectiveField;

        public VisualizationInfo()
        {
            this.bitmapsField = new List<VisualizationInfoBitmaps>();
            this.clippingPlanesField = new List<ClippingPlane>();
            this.linesField = new List<Line>();
            this.perspectiveCameraField = new PerspectiveCamera();
            this.orthogonalCameraField = new OrthogonalCamera();
            this.componentsField = new ObservableCollection<Component>();
            this.isPerspectiveField = false;
        }

        public VisualizationInfo(VisualizationInfo visInfo)
        {
            this.Components = visInfo.Components;
            this.OrthogonalCamera = visInfo.OrthogonalCamera;
            this.PerspectiveCamera = visInfo.PerspectiveCamera;
            this.Lines = visInfo.Lines;
            this.ClippingPlanes = visInfo.ClippingPlanes;
            this.Bitmaps = visInfo.Bitmaps;
            this.isPerspectiveField = visInfo.isPerspectiveField;
        }

        [System.Xml.Serialization.XmlArrayItemAttribute(IsNullable = false)]
        public ObservableCollection<Component> Components
        {
            get { return this.componentsField; }
            set { this.componentsField = value; }
        }

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public OrthogonalCamera OrthogonalCamera
        {
            get { return this.orthogonalCameraField; }
            set { this.orthogonalCameraField = value; isPerspectiveField = (orthogonalCameraField.ViewToWorldScale != 0) ? false : true; }
        }

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public PerspectiveCamera PerspectiveCamera
        {
            get { return this.perspectiveCameraField; }
            set { this.perspectiveCameraField = value; isPerspectiveField = (perspectiveCameraField.FieldOfView != 0) ? true : false; }
        }

        [System.Xml.Serialization.XmlArrayItemAttribute(IsNullable = false)]
        public List<Line> Lines
        {
            get { return this.linesField; }
            set { this.linesField = value; }
        }

        [System.Xml.Serialization.XmlArrayItemAttribute(IsNullable = false)]
        public List<ClippingPlane> ClippingPlanes
        {
            get { return this.clippingPlanesField; }
            set { this.clippingPlanesField = value; }
        }

        [System.Xml.Serialization.XmlElementAttribute("Bitmaps")]
        public List<VisualizationInfoBitmaps> Bitmaps
        {
            get { return this.bitmapsField; }
            set { this.bitmapsField = value; }
        }

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public string ViewPointGuid
        {
            get { return this.viewPointGuidField; }
            set { this.viewPointGuidField = value; }
        }

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool IsPersepective
        {
            get { return this.isPerspectiveField; }
            set { this.isPerspectiveField = value; }
        }

        public virtual VisualizationInfo Clone()
        {
            return ((VisualizationInfo)(this.MemberwiseClone()));
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

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.34234")]
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public class VisualizationInfoBitmaps
    {
        private string guidField = "";
        private BitmapFormat bitmapField;
        private string referenceField = "";
        private Point locationField;
        private Direction normalField;
        private Direction upField;
        private double heightField;
        private byte[] bitmapImageField;
        private string viewPointGuidField = "";

        public VisualizationInfoBitmaps()
        {
            this.bitmapField = BitmapFormat.PNG;
            this.guidField = System.Guid.NewGuid().ToString();
            this.upField = new Direction();
            this.normalField = new Direction();
            this.locationField = new Point();
        }

        public VisualizationInfoBitmaps(VisualizationInfoBitmaps visInfoBitmaps)
        {
            this.Guid = visInfoBitmaps.Guid;
            this.Bitmap = visInfoBitmaps.Bitmap;
            this.Reference = visInfoBitmaps.Reference;
            this.Location = visInfoBitmaps.Location;
            this.Normal = visInfoBitmaps.Normal;
            this.Up = visInfoBitmaps.Up;
            this.Height = visInfoBitmaps.Height;
            this.BitmapImage = visInfoBitmaps.BitmapImage;
        }

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public string Guid
        {
            get { return this.guidField; }
            set { this.guidField = value; }
        }

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public BitmapFormat Bitmap
        {
            get { return this.bitmapField; }
            set { this.bitmapField = value; }
        }

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Reference
        {
            get { return this.referenceField; }
            set { this.referenceField = value; }
        }

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public Point Location
        {
            get { return this.locationField; }
            set { this.locationField = value; }
        }

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public Direction Normal
        {
            get { return this.normalField; }
            set { this.normalField = value; }
        }

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public Direction Up
        {
            get { return this.upField; }
            set { this.upField = value; }
        }

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public double Height
        {
            get { return this.heightField; }
            set { this.heightField = value; }
        }

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public byte[] BitmapImage
        {
            get { return this.bitmapImageField; }
            set { this.bitmapImageField = value; }
        }

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public string ViewPointGuid
        {
            get { return this.viewPointGuidField; }
            set { this.viewPointGuidField = value; }
        }

        public virtual VisualizationInfoBitmaps Clone()
        {
            return ((VisualizationInfoBitmaps)(this.MemberwiseClone()));
        }
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.34234")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public enum BitmapFormat
    {
        PNG,
        JPG
    }
}
