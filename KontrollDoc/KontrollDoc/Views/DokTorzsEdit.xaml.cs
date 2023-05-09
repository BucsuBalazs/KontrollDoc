using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using FIT_Common;
using KontrollDoc.Models;

namespace KontrollDoc.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class DokTorzsEdit : ContentPage
	{
        DB dbc;
        KapcssDokTorzs dokTorzs;
        string dokTipus;

        public DokTorzsEdit(DB dbc, string dokTipus)
        {
            InitializeComponent();
            this.dbc = dbc;
            this.dokTipus = dokTipus;
            Title= dokTipus;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            this.dokTorzs = new KapcssDokTorzs(dbc);

            List<DokTorzs> output = dokTorzs.GetDokTorzsObjects(dokTipus);


        DokTorzsListView.ItemsSource = dokTorzs.GetDokTorzsObjects(dokTipus);
        }
        private async void DokTorzsListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var Tapped = (DokTorzs)e.Item;
            await Navigation.PushAsync(new Views.DokTorzsItem(dbc, Tapped, dokTipus));
        }
        private async void Uj_DokTorzs_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Views.DokTorzsItem(dbc, dokTipus));
        }
    }
}