using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using FIT_Common;

namespace KontrollDoc.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MenuPage : ContentPage
    {
        DB dbc = null;

        public MenuPage(DB dbc)
        {
            InitializeComponent();

            this.dbc = dbc;
            welcomeText.Text = "Üdvözöljük "+ this.dbc.FelhasznaloNev +"!";


        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

        }

        async void Docs_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Views.DocList(dbc));
        }


        protected void GoFit(object sender, EventArgs e)
        {
            Launcher.OpenAsync("http://www.fit.hu/");
        }
    }
}