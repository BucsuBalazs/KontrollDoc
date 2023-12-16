using FIT_Common;
using KontrollDoc.Services.DependencyServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace KontrollDoc.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Print : ContentPage
    {
        DB dbc = null;

        public Print(DB dbc)
        {
            InitializeComponent();

            this.dbc = dbc;
        }

        private void Print_Button_Clicked(object sender, EventArgs e)
        {
            if (Device.RuntimePlatform == Device.Android)
            {
                DependencyService.Get<IAndroidPrintService>().PrintAsync();
            }
            if (Device.RuntimePlatform == Device.UWP)
            {
                DependencyService.Get<IUWPPrintService>().PrintAsync();
            }
        }
    }
}