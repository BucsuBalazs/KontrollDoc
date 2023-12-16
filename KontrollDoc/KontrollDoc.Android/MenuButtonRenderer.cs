using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using KontrollDoc.Droid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using KontrollDoc.Resources.Renderers;

[assembly: ExportRenderer(typeof(MenuButton), typeof(MenuButtonRenderer))]
namespace KontrollDoc.Droid
{
    public class MenuButtonRenderer : ButtonRenderer
    {
        MenuButtonRenderer(Context context) : base(context)
        {
        
        }

        protected override void OnElementChanged(Xamarin.Forms.Platform.Android.ElementChangedEventArgs<Xamarin.Forms.Button> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                // Keep the base design for Android
            }
        }
    }
}