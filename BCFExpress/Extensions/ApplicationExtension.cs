using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCFExpress
{
    public static class ApplicationExtension
    {
        public static RibbonPanel GetOrCreateRibbonPanel(this UIControlledApplication app, string tabName, string ribbonName)
        {
            var existingRibbon = app.GetRibbonPanels(tabName).FirstOrDefault(ribbon => ribbon.Name == ribbonName);
            if (existingRibbon != null)
                return existingRibbon;

            // Create the "element" panel.
            var createdPanel = app.CreateRibbonPanel(tabName, ribbonName);
            return createdPanel;
        }
    }
}

