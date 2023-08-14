using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCFExpress
{
    public class PushButtonDataModel
    {



        #region Properities
        /// <summary>
        /// Button Lable 
        /// </summary>
        public string BtnName { get; set; }
        /// <summary>
        /// Panel to Add the button
        /// </summary>
        public RibbonPanel Panel { get; set; }
        /// <summary>
        /// The targeted command path
        /// </summary>
        public string CommandNameSpacePath { get; set; }
        /// <summary>
        /// Button Tool tip instructions
        /// </summary>
        public string Tooltip { get; set; }
        /// <summary>
        /// Button icon name
        /// </summary>
        public string IconImageName { get; set; }
        /// <summary>
        /// Button tooltip image name
        /// </summary>
        public string TooltipImageName { get; set; }

        #endregion





    }

}
