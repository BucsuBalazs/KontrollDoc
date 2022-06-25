using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

using System.Data.SqlClient;

using CAVO3;

namespace KontrollDoc
{
    public partial class MainPage : ContentPage
    {

        public MainPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            user.Text = "BBalazs";
            password.Text = "Bazsa123";
        }

        public static DB dbc;
        public static long uid;

        async void OnNextPageButtonClicked(object sender, EventArgs e)
        {
            dbc = new DB("KontrollDoc", true);
            long uid = dbc.LoginPassThrough(user.Text, password.Text);
            if (uid != 0)
            {
                dbc.SetAdatbazisId(241);
                await Navigation.PushAsync(new SecondPage());
            }
            else
            {
                errorMessage.Text = "Hibás felhasználónév vagy jelszó!";
                errorMessage.IsVisible = true;

            }
        }
    }
}
