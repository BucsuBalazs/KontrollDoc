using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

using System.Data.SqlClient;

using FIT_Common;
using System.Data;
using KontrollDoc.Models;

namespace KontrollDoc.Views
{
    public partial class LoginPage : ContentPage
    {

        DB dbc;
        public LoginPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            this.dbc = new DB("KontrollDoc", true);

            

        }

        private void password_TextChanged(object sender, TextChangedEventArgs e)
        {
            long uid = dbc.LoginPassThrough(user.Text, password.Text);
            if (uid != 0)
            {
                errorMessage.Text = "Válassz Adatbázist";
                errorMessage.IsVisible = true;

                DataTable DT = dbc.GetAdatbazisok();

                List<Adatbazis> adatbazisok = new List<Adatbazis>();

                foreach (DataRow row in DT.Rows)
                {
                    Adatbazis ad = new Adatbazis();
                    ad.Id = (int)row["Id"];
                    ad.Nev = (string)row["Nev"];
                    ad.CegId = (int)row["CegId"];
                    adatbazisok.Add(ad);
                }

                adatbazis_Picker.ItemsSource = adatbazisok;
                adatbazis_Picker.ItemDisplayBinding = new Binding("Id");
                adatbazis_Picker.IsVisible = true;
            }
            else
            {
                errorMessage.Text = "Hibás felhasználónév vagy jelszó!";
                errorMessage.IsVisible = true;

            }
        }

        private void adatbazis_SelectedIndexChanged(object sender, EventArgs e)
        {
            Belep.IsVisible = true;
        }

        async void OnNextPageButtonClicked(object sender, EventArgs e)
        {


            long uid = dbc.LoginPassThrough(user.Text, password.Text);
            if (uid != 0)
            {

                var db = (Adatbazis)adatbazis_Picker.SelectedItem;
                dbc.SetAdatbazisId(db.Id);

                Navigation.InsertPageBefore(new MenuPage(dbc), this);
                await Navigation.PopAsync();
            }
            else
            {
                errorMessage.Text = "Hibás felhasználónév vagy jelszó!";
                errorMessage.IsVisible = true;

            }
        }
    }
}
