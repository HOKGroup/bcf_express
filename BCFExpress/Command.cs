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
using BCFExpress.Util;
using Microsoft.SqlServer.Server;

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

            if (DB.BCFUIs.Count==0)
            {
                BCFUI bCFUI = new BCFUI(uiApp , ev , bCFExpressExternalEventHandler);
                bCFUI.Show();
                
               DB.BCFUIs.Add(bCFUI);
            }
            else
            {
                TaskDialog.Show("Error", "There Is An Opening Window....");
                DB.BCFUIs.FirstOrDefault().Activate();


            }
  


            return Result.Succeeded;
        }

        public static string GetPath()
        {
            return typeof(Command).Namespace + "." + nameof(Command);
        }
    }
}
