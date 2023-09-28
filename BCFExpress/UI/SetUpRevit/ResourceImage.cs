using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace BCFExpress
{
    public static class ResourceImage
    {
        public static BitmapImage GetIcon(string iconName)
        {
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(
                typeof(ResourceImage).Namespace + ".UI.Resourses.Icons." + iconName);

            var image = new BitmapImage();

            image.BeginInit();
            image.StreamSource = stream;
            image.EndInit();

            return image;

        }

    }
}
