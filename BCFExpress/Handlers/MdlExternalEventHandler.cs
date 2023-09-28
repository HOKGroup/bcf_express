using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Windows;
using BCFExpress.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using TaskDialog = Autodesk.Revit.UI.TaskDialog;

namespace BCFExpress.Handlers
{
    public class MdlExternalEventHandler : IExternalEventHandler
    {
        public Action<List<Element>> RunMethod;
        public List<Action> RunMethodGenericList;
        public Action CallBackFunction;
        public Action<object> CallBackFunctionObjectPar;
        private string _transactionName;
        private List<Element> elements;

        private ExternalEvent ExternalEvent { get; set; }

        public MdlExternalEventHandler()
        {
            this.ExternalEvent = ExternalEvent.Create((IExternalEventHandler)this);
            this.elements = new List<Element>();
            RunMethodGenericList = new List<Action>();
        }

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        public void Execute(UIApplication app)
        {

            Transaction transaction = new Transaction(RevitModel.Document);
            int num1 = (int)transaction.Start(this._transactionName);
            foreach (var RunMethodGeneric in RunMethodGenericList)
            {
                try
                {
                    RunMethodGeneric();

                }
                catch (Exception ex)
                {
                    Autodesk.Revit.UI.TaskDialog.Show("error", "Error handling external event execute\n"
                        + "\nMessage ---\n{0}" + ex.Message
                        + "\nSource ---\n{0}" + ex.Source
                        + "\nStackTrace ---\n{0}" + ex.StackTrace
                        + "\nTargetSite ---\n{0}" + ex.TargetSite);



                }
            }
            RunMethodGenericList.Clear();
            RevitModel.Document.Regenerate();
            int num2 = (int)transaction.Commit();
            if (this.CallBackFunction == null)
                return;
            this.CallBackFunction();
        }

        public void Run(string transactionName)
        {
            this._transactionName = transactionName;
            try
            {
                IntPtr foregroundWindow = MdlExternalEventHandler.GetForegroundWindow();
                MdlExternalEventHandler.SetForegroundWindow(ComponentManager.ApplicationWindow);
                MdlExternalEventHandler.SetForegroundWindow(foregroundWindow);
                int num = (int)this.ExternalEvent.Raise();
            }
            catch (System.Exception ex)
            {
                TaskDialog.Show("error", "Error handling external event run of transaction name: " + transactionName);
                // new BIMAIException("Error handling external event run of transaction name: " + transactionName, ex).MsgError();

            }
        }

        public void Run(Action runMethod, Action callbackfunction, string tansactionName)
        {
            this._transactionName = tansactionName;
            this.RunMethodGenericList.Add(runMethod);
            this.CallBackFunction = callbackfunction;
            try
            {
                IntPtr foregroundWindow = MdlExternalEventHandler.GetForegroundWindow();
                MdlExternalEventHandler.SetForegroundWindow(ComponentManager.ApplicationWindow);
                MdlExternalEventHandler.SetForegroundWindow(foregroundWindow);
                int num = (int)this.ExternalEvent.Raise();
            }
            catch (System.Exception ex)
            {
                TaskDialog.Show("error", string.Format("Error handling external event run\nAction: {0}\nCallback: {1}\nTransaction name: {2}", (object)runMethod, (object)this.CallBackFunction, (object)this._transactionName));
                // new BIMAIException(string.Format("Error handling external event run\nAction: {0}\nCallback: {1}\nTransaction name: {2}", (object)runMethod, (object)this.CallBackFunction, (object)this._transactionName), ex).MsgError();

            }
        }

        public void Run(Action runMethod, string tansactionName)
        {
            this._transactionName = tansactionName;
            this.RunMethodGenericList.Add(runMethod);
            this.CallBackFunction = (Action)null;
            try
            {
                IntPtr foregroundWindow = MdlExternalEventHandler.GetForegroundWindow();
                MdlExternalEventHandler.SetForegroundWindow(ComponentManager.ApplicationWindow);
                MdlExternalEventHandler.SetForegroundWindow(foregroundWindow);
                int num = (int)this.ExternalEvent.Raise();
            }
            catch (System.Exception ex)
            {
                TaskDialog.Show("error", string.Format("Error handling external event run\nAction: {0}\nTransaction name: {1}", (object)runMethod, (object)this._transactionName));


            }
        }


        public string GetName() => "";
    }
}
