using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCFExpress.Bcf.Bcf2
{

    public class Result 
    {
        public  string Category { get; set; }
        public string SelectedElementRevitUniqueID { get; set; }
        public string IssueElementUniqueID { get; set; }
        public string IssueElementIFCGUID { get; set; }
        public string SelectedElementRevitVersionGUID { get; set; }
        public BcfBoundingBoxXYZ SelectedElementBoundingBox { get; set; }
        public string IgnoreOrIssue { get; set; } // 1 for Ignore, 2 for Issue
        public DateTime DateTimeStamp { get; set; }
        public string RevitUserName { get; set; }


    }
    public class BcfBoundingBoxXYZ
    {
        public bool Enabled { get; set; }
        public Max Max { get; set; }
        public Min Min { get; set; }
        public Transform Transform { get; set; }
    }
    public class Max
    {
        public Max()
        {
            
        }
        public Max(double _X, double _Y, double _Z)
        {
            X = _X; Y = _Y; Z = _Z;
        }
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
    }
    public class Min
    {
        public Min()
        {
            
        }
        public Min(double _X, double _Y, double _Z)
        {
            X = _X; Y = _Y; Z = _Z;
        }
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
    }
    public class Transform
    {
        public Transform()
        {
            
        }
        public Transform(Origin _Origin , BasisX _BasisX, BasisY _BasisY, BasisZ _BasisZ)
        {
            Origin = _Origin;   
            BasisX = _BasisX;
            BasisY = _BasisY;
            BasisZ = _BasisZ;
        }
        public Origin Origin { get; set; }
        public BasisX BasisX { get; set; }
        public BasisY BasisY { get; set; }
        public BasisZ BasisZ { get; set; }
    }
    public class Origin
    {
        public Origin() { }
        public Origin(double _X, double _Y, double _Z)
        {
            X = _X; Y = _Y; Z = _Z;
        }
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
    }
    public class BasisX
    {
        public BasisX() { }
        public BasisX(double _X, double _Y, double _Z)
        {
            X = _X; Y = _Y; Z = _Z;
        }
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
    }
    public class BasisY
    {
        public BasisY() { }
        public BasisY(double _X, double _Y, double _Z)
        {
            X = _X; Y = _Y; Z = _Z;
        }
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
    }
    public class BasisZ
    {
        public BasisZ() { }
        public BasisZ(double _X, double _Y, double _Z)
        {
            X = _X; Y = _Y; Z = _Z;
        }
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
    }
}
