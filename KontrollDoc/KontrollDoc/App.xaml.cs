using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace KontrollDoc
{
    public partial class App : Application
    {
        

        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new MainPage());

            //MainPage = new NavigationPage(new SecondPage());
            //MainPage = new NavigationPage(new LoginPage() );
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
