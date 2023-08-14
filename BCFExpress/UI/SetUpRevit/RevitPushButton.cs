using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BCFExpress
{
    public static class RevitPushButton
    {


        #region Methods

        /// <summary>
        /// Method to create push button based on data provided in <see cref="PushButtonDataModel"/>
        /// </summary>
        /// <param name="btnData"></param>
        /// <returns></returns>
        public static PushButton CreatePushButton(PushButtonDataModel data)
        {
            var btnName = Guid.NewGuid().ToString();

            var btnData = new PushButtonData(btnName, data.BtnName, Assembly.GetExecutingAssembly().Location.ToString(), data.CommandNameSpacePath)
            {
                LargeImage = ResourceImage.GetIcon(data.IconImageName),
                // ToolTipImage = ResourceImage.GetIcon(data.TooltipImageName),
                ToolTip = data.Tooltip
            };

            return data.Panel.AddItem(btnData) as PushButton;

        }

        #endregion
    }

}
