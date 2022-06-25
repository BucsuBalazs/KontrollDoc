using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace KontrollDoc.Layouts
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FeladatkezeloPage : ContentPage
    {
        public FeladatkezeloPage()
        {
            InitializeComponent();

        }

        async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Layouts.FeladatPage());
        }
    }
}