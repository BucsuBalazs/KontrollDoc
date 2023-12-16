using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms;
using KontrollDoc.Droid;
using KontrollDoc.Resources.Renderers;

[assembly: ExportRenderer(typeof(BluePicker), typeof(BluePickerRenderer))]
namespace KontrollDoc.Droid
{
    public class BluePickerRenderer : PickerRenderer
    {
        public BluePickerRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Picker> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                Control.Background.SetColorFilter(Android.Graphics.Color.Blue, Android.Graphics.PorterDuff.Mode.SrcAtop);
            }
        }
    }
}