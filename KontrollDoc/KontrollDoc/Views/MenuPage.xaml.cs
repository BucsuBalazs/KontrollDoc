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
using KontrollDoc.Services.DependencyServices;

namespace KontrollDoc.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]

    /// <summary>
    /// A MenuPage osztály egy ContentPage, amely a különböző lapok közti navigálásra szolgál.
    /// </summary>
    public partial class MenuPage : ContentPage
    {
        /// <summary>
        /// Használt adatbázis kontextus
        /// </summary>
        DB dbc = null;

        /// <summary>
        /// Felhasználó jogosultsági szintje.
        /// </summary>
        private long Jogosultsag;

        /// <summary>
        /// Inicializálja a MenuPage osztály új példányát.
        /// </summary>
        /// <param name="dbc">Az adatbázis környezet.</param>
        public MenuPage(DB dbc)
        {
            InitializeComponent();

            // Adatbázis környezet beállítása.
            this.dbc = dbc;
            welcomeText.Text = "Üdvözöljük "+ this.dbc.FelhasznaloNev +"!";

            // Felhazsnáló jogosultságának lekérése, beállítása.
            List<SqlParameter> GetJogFelhparams = new List<SqlParameter>();

            SqlParameter JogAz = new SqlParameter();
            JogAz.ParameterName = "@JogAz";
            JogAz.Value = 72;
            GetJogFelhparams.Add(JogAz);

            SqlParameter FelhAz = new SqlParameter();
            FelhAz.ParameterName = "@FelhAz";
            FelhAz.Value = this.dbc.ABFelhasznaloId;
            GetJogFelhparams.Add(FelhAz);

            // felhasználó jogosultság lekérése az adatbázisból
            Jogosultsag = dbc.ExecuteSPAB("GetJogFelh", GetJogFelhparams);

            // Felhazsnáló jogosultságának leellenőrzése.
            if (Jogosultsag != 1) 
            {
                Dok.IsEnabled= false;
                Part.IsEnabled= false;
                Irat.IsEnabled= false;
            }

            // Szkennelés funkció elérésének a letiltása androidra.
            if (Device.RuntimePlatform == Device.Android)
            {
                Szken.IsEnabled = false;
            }

        }

        /// <summary>
        /// Eseménykezelő a Dokumentumok gomb kattintási eseményéhez.
        /// </summary>
        async void Docs_Clicked(object sender, EventArgs e)
        {
            // Navigálás a Dokumentumok lapra.
            await Navigation.PushAsync(new Views.DocList(dbc));
        }

        /// <summary>
        /// Eseménykezelő a Partnerek gombra kattintási eseményhez.
        /// </summary>
        async void Partner_Body_Clicked(object sender, EventArgs e)
        {
            // Navigálás a Partnerek lapra.
            await Navigation.PushAsync(new Views.PartnerList(dbc));
        }

        /// <summary>
        /// Eseménykezelő a Hozzáférés gombra kattintási eseményhez.
        /// </summary>
        async void Doc_Body_Clicked(object sender, EventArgs e)
        {
            // Navigálás a Hozzáférés lapra.
            await Navigation.PushAsync(new Views.Access(Jogosultsag));
        }

        /// <summary>
        /// Eseménykezelő a Irattár gombra kattintási eseményhez.
        /// </summary>
        async void Archives_Clicked(object sender, EventArgs e)
        {
            // Navigálás a Irattár lapra.
            await Navigation.PushAsync(new Views.Archives(dbc));
        }

        /// <summary>
        /// Eseménykezelő a Szkennelés gombra kattintási eseményhez.
        /// </summary>
        async void Scan_Clicked(object sender, EventArgs e)
        {
            // Navigálás a Szkennelés lapra.
            await Navigation.PushAsync(new Views.Scan(dbc));
        }

        /// <summary>
        /// Eseménykezelő a Nyomtatás gombra kattintási eseményhez.
        /// </summary>
        async void Print_Clicked(object sender, EventArgs e)
        {
            // Navigálás a Nyomtatás lapra.
            await Navigation.PushAsync(new Views.Print(dbc));
        }

        /// <summary>
        /// Eseménykezelő a Cégkontrol labelra kattintási eseményhez.
        /// </summary>
        protected void GoFit(object sender, EventArgs e)
        {
            // Navigálás a fit weblapra.
            Launcher.OpenAsync("http://www.fit.hu/");
        }

        /// <summary>
        /// Eseménykezelő a Kijelentkezés gombra kattintási eseményhez.
        /// </summary>
        async void Logout_Clicked(object sender, EventArgs e)
        {
            // Kijelentkezés az adatbázisból és navigálás a Login lapra.
            dbc.Logout();
            Navigation.InsertPageBefore(new LoginPage(), this);
            await Navigation.PopAsync();
        }
    }
}