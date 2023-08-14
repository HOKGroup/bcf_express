using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using BCFExpress.Handlers;
using BCFExpress.UI.Viewers;

namespace BCFExpress
{
    [Transaction(TransactionMode.Manual)]
    public class Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
        
            var uiApp = commandData.Application;

            BCFExpressExternalEventHandler bCFExpressExternalEventHandler = new BCFExpressExternalEventHandler(uiApp);
            ExternalEvent ev = ExternalEvent.Create(bCFExpressExternalEventHandler);
            //create object from main window 
            BCFUI bCFUI = new BCFUI(uiApp , ev , bCFExpressExternalEventHandler);
            bCFUI.Show();
            return Result.Succeeded;
        }

        public static string GetPath()
        {
            return typeof(Command).Namespace + "." + nameof(Command);
        }
    }
}
