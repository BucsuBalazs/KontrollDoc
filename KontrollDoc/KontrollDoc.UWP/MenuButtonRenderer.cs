using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using KontrollDoc.Resources.Renderers;
using Xamarin.Forms.Platform.UWP;
using KontrollDoc.UWP;
using Windows.UI.Xaml.Controls;

[assembly: ExportRenderer(typeof(MenuButton), typeof(MenuButtonRenderer))]
namespace KontrollDoc.UWP
{
    internal class MenuButtonRenderer : ButtonRenderer
    {
        protected override void OnElementChanged(Xamarin.Forms.Platform.UWP.ElementChangedEventArgs<Xamarin.Forms.Button> e)
        {
            base.OnElementChanged(e);
        }
    
    }
}
