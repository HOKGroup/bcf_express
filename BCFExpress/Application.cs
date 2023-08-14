using Autodesk.Revit.UI;
using BCFExpress.Handlers;
using BCFExpress.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BCFExpress
{
    internal class Application:IExternalApplication
    {
        static readonly string tabName = "BCF Express";


        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        public Result OnStartup(UIControlledApplication app)
        {
            try
            {
                RevitModel.MdlExternalEventHandler = new MdlExternalEventHandler();
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyResolve);
                app.ViewActivated += App_ViewActivated;

                app.CreateRibbonTab(tabName);

                var panel = app.GetOrCreateRibbonPanel(tabName, "BCF Express");

                var textbtndata = new PushButtonDataModel()
                {
                    BtnName = "Open",
                    Panel = panel,
                    Tooltip = "This addin helps you to \n Export data to exel sheet",
                    IconImageName = "bcf.png",
                    // TooltipImageName = "5.PNG",
                    CommandNameSpacePath = Command.GetPath(),

                };
                var textbtndata2 = new PushButtonDataModel()
                {
                    BtnName = "Setting",
                    Panel = panel,
                    Tooltip = "This addin helps you to \n Export data to exel sheet",
                    IconImageName = "setting.png",
                    // TooltipImageName = "5.PNG",
                    
                    CommandNameSpacePath = CommandSetting.GetPath(),

                };
                
                _ = RevitPushButton.CreatePushButton(textbtndata);
                _ = RevitPushButton.CreatePushButton(textbtndata2);

                return Result.Succeeded;

            }
            catch (Exception m)
            {

                TaskDialog.Show("Error", m.Message);

                return Result.Failed;
            }


        }

        private void App_ViewActivated(object sender, Autodesk.Revit.UI.Events.ViewActivatedEventArgs e)
        {

            RevitModel.UIApplication = sender as UIApplication;
        }

        private Assembly AssemblyResolve(object sender, ResolveEventArgs args)
        {
            int position = args.Name.IndexOf(",");
            if (position > -1)
            {
                try
                {
                    string assemblyName = args.Name.Substring(0, position);
                    string assemblyFullPath = string.Empty;

                    //look in main folder
                    assemblyFullPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\" + assemblyName + ".dll";
                    if (File.Exists(assemblyFullPath))
                        return Assembly.LoadFrom(assemblyFullPath);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
            }
            return null;
        }


    }
}
