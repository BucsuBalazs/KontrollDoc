using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using FIT_Common;
using System.Data.SqlClient;

namespace KontrollDoc.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MenuPage : ContentPage
    {
        DB dbc = null;
        private long Jogosultsag;

        public MenuPage(DB dbc)
        {
            InitializeComponent();

            this.dbc = dbc;
            welcomeText.Text = "Üdvözöljük "+ this.dbc.FelhasznaloNev +"!";

            List<SqlParameter> GetJogFelhparams = new List<SqlParameter>();

            SqlParameter JogAz = new SqlParameter();
            JogAz.ParameterName = "@JogAz";
            JogAz.Value = 72;
            GetJogFelhparams.Add(JogAz);

            SqlParameter FelhAz = new SqlParameter();
            FelhAz.ParameterName = "@FelhAz";
            FelhAz.Value = this.dbc.ABFelhasznaloId;
            GetJogFelhparams.Add(FelhAz);

            Jogosultsag = dbc.ExecuteSPAB("GetJogFelh", GetJogFelhparams);

            if (Jogosultsag != 1) 
            {
                Dok.IsEnabled= false;
                Part.IsEnabled= false;
                Irat.IsEnabled= false;
            }


        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

        }

        async void Docs_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Views.DocList(dbc));
        }

        async void Partner_Body_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Views.PartnerList(dbc));
        }

        async void Doc_Body_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Views.Access(Jogosultsag));
        }

        async void Archives_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Views.Archives(dbc));
        }


        protected void GoFit(object sender, EventArgs e)
        {
            Launcher.OpenAsync("http://www.fit.hu/");
        }

        async void Logout_Clicked(object sender, EventArgs e)
        {
            dbc.Logout();
            Navigation.InsertPageBefore(new LoginPage(), this);
            await Navigation.PopAsync();
        }
    }
}