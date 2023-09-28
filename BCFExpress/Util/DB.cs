using Autodesk.Revit.DB;
using BCFExpress.Bcf;
using BCFExpress.Bcf.Bcf2;
using BCFExpress.UI.Viewers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BCFExpress.Util
{
    internal class DB
    {
        public static Markup SelectedMarkup { get; internal set; }
        public static List<Element> RvtElements { get; internal set; }
        public static List<Category> SelectedCategories { get; internal set; }
        public static bool IsHighlightChecked { get; internal set; }
        public static bool IsIsolateChecked { get; internal set; }
        public static bool IsSectionBoxChecked { get; internal set; }
        public static RevitComponent SelectedComponent { get; internal set; }
        public static List<RevitComponent> RvtComponents { get; internal set; }
        public static string SelectedBCFZip { get; internal set; }
        public static IEnumerable<BcfFile> SelectedBcfFiles { get; internal set; }
        private static  List<BCFUI> bCFUIs = new List<BCFUI>();

        public static List<BCFUI> BCFUIs
        {
            get { return bCFUIs; }
            set { bCFUIs =  value; }
        }

    }
}
