using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using BCFExpress.UI.Viewers;

namespace BCFExpress
{
    [Transaction(TransactionMode.Manual)]
    public class CommandSetting : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //create object from seeting window 
            SettingUI settingUI = new SettingUI();
            settingUI.Show();
            return Result.Succeeded;
        }

        public static string GetPath()
        {
            return typeof(CommandSetting).Namespace + "." + nameof(CommandSetting);
        }
    }
}
