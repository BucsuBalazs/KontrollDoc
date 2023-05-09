using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

using System.Data.SqlClient;

using FIT_Common;

namespace KontrollDoc.Views
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            user.Text = "BBalazs";
            password.Text = "Bazsa123";
        }


        async void OnNextPageButtonClicked(object sender, EventArgs e)
        {

            DB dbc = new DB("ANCSA", true);

            long uid = dbc.LoginPassThrough(user.Text, password.Text);
            if (uid != 0)
            {
                dbc.SetAdatbazisId(241);

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
