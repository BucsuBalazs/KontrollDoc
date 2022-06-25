using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using CAVO3;

namespace KontrollDoc
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SecondPage : ContentPage
    {

        public SecondPage()
        {
            InitializeComponent();

            welcomeText.Text = "Üdvözöljük "+KontrollDoc.MainPage.dbc.FelhasznaloNev+"!";
        }
        

        async void Docs_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Layouts.DocList());
        }

        async void Feladat_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Layouts.FeladatkezeloPage());
        }

        protected void GoFit(object sender, EventArgs e)
        {
            Launcher.OpenAsync("http://fit.hu/fit_3.0/Default.aspx");
        }
    }
}