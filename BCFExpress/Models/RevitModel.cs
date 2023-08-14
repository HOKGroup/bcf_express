using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using BCFExpress.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCFExpress.Models
{
    internal class RevitModel
    {
        private static Document document;
        private static UIDocument uiDocument;
        private static UIApplication uIApplication;
        public static MdlExternalEventHandler MdlExternalEventHandler { get; set; }
        public static UIDocument UIDocument
        {
            get => RevitModel.uiDocument;
            set
            {
                RevitModel.uiDocument = value;
                if (RevitModel.uiDocument == null)
                    return;
                RevitModel.document = value.Document;
            }
        }
        public static UIApplication UIApplication
        {
            get => RevitModel.uIApplication;
            set
            {
                RevitModel.uIApplication = value;
                if (RevitModel.UIApplication == null)
                    return;
                RevitModel.UIDocument = value.ActiveUIDocument;
            }
        }
        public static Document Document => RevitModel.UIDocument == null ? (Document)null : RevitModel.UIApplication.ActiveUIDocument.Document;
    }
}
