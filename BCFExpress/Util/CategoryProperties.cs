using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BCFExpress.UI.Viewers.BCFUI;

namespace BCFExpress.Util
{
    public class CategoryProperties : INotifyPropertyChanged
    {
        private string categoryName = "";
        private ElementId categoryId = ElementId.InvalidElementId;
        private bool selected = true;

        public string CategoryName { get { return categoryName; } set { categoryName = value; NotifyPropertyChanged("CategoryName"); } }
        public ElementId CategoryId { get { return categoryId; } set { categoryId = value; NotifyPropertyChanged("CategoryId"); } }
        public bool Selected { get { return selected; } set { selected = value; NotifyPropertyChanged("Selected"); } }

        public CategoryProperties(Category category)
        {
            categoryName = category.Name;
            categoryId = category.Id;
            var categoryFound = from cat in DB.SelectedCategories where cat.Id == categoryId select cat;
           
        }

        public CategoryProperties()
        {
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
    }
}
