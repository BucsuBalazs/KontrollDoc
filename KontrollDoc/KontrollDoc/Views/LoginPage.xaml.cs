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
    /// <summary>
    /// A LoginPage osztály egy ContentPage, amely a bejelentkezési funkciókat biztosítja.
    /// </summary>
    public partial class LoginPage : ContentPage
    {
        /// <summary>
        /// A bejelentkezési műveletekhez használt adatbázis-környezet.
        /// </summary>
        DB dbc;

        /// <summary>
        /// Inicializálja a LoginPage osztály új példányát.
        /// </summary>
        public LoginPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Akkor hívják, ha az oldal láthatóvá válik. Inicializálja az adatbázis-környezetet.
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();

            this.dbc = new DB("KontrollDoc", true);
        }

        /// <summary>
        /// Akkor hívják, ha a jelszó szövege megváltozik. Megpróbál bejelentkezni, és ennek megfelelően frissíti a felhasználói felületet.
        /// </summary>
        private void password_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Meg próbálja validálni a megadott felhasználót és jelszót
            long uid = dbc.LoginPassThrough(user.Text, password.Text);
            if (uid != 0)
            {
                errorMessage.Text = "Válassz Adatbázist";
                errorMessage.IsVisible = true;

                // Sikeres validálás után elérhető adatbázisok lekérése
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

                // Talált adatbázisok betöltése az adatbazis_Picker pickerbe és láthatóvá tétele.
                adatbazis_Picker.ItemsSource = adatbazisok;
                adatbazis_Picker.ItemDisplayBinding = new Binding("Id");
                adatbazis_Picker.IsVisible = true;
            }
            else
            {
                // Sikertelen validáláss után hiba üzenet küldése a felhazsnálónak
                errorMessage.Text = "Hibás felhasználónév vagy jelszó!";
                errorMessage.IsVisible = true;

            }
        }

        /// <summary>
        /// Meghívás, ha a kiválasztott adatbázis megváltozik. Láthatóvá teszi a bejelentkezési gombot.
        /// </summary>
        private void adatbazis_SelectedIndexChanged(object sender, EventArgs e)
        {
            Belep.IsVisible = true;
        }

        /// <summary>
        /// A Belépés gombra kattintás hívja meg. Megpróbál bejelentkezni, és sikeres esetben a következő oldalra navigál.
        /// </summary>
        async void OnNextPageButtonClicked(object sender, EventArgs e)
        {
            // Újra validál
            long uid = dbc.LoginPassThrough(user.Text, password.Text);
            if (uid != 0)
            {
                // Adatbázis beállítása
                var db = (Adatbazis)adatbazis_Picker.SelectedItem;
                dbc.SetAdatbazisId(db.Id);

                // MenuPage beállítása ez a lap elé a navigálási stackben és vissza navigálás a MenuPage lapra.
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
