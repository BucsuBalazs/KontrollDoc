using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms.Platform.UWP;
using Xamarin.Forms;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Controls;
using KontrollDoc.UWP;
using KontrollDoc.Resources.Renderers;

[assembly: ExportRenderer(typeof(BluePicker), typeof(BluePickerRenderer))]
namespace KontrollDoc.UWP
{
    public class BluePickerRenderer : PickerRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Picker> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                Control.BorderBrush = new Windows.UI.Xaml.Media.SolidColorBrush(Windows.UI.Color.FromArgb(255, 123, 104, 238));
            }
        }
    }
}
