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
    /// <summary>
    /// Egy ContentPage amely dokumentum törzzsel való műveletekhez biztosít eseményeket.
    /// </summary>
    public partial class DokTorzsEdit : ContentPage
	{
        /// <summary>
        /// Az adatbázis környezet.
        /// </summary>
        DB dbc;
        /// <summary>
        /// A dokumentumok törzs adatai.
        /// </summary>
        KapcssDokTorzs dokTorzs;
        /// <summary>
        /// Kiválasztott típus
        /// </summary>
        string dokTipus;
        /// <summary>
        /// Inicializálja a <see cref="DokTorzsEdit"/> osztály új példányát.
        /// </summary>
        /// <param name="dbc">Az adatbázis környezet.</param>
        /// <param name="dokTipus">A kiválasztott típus.</param>

        public DokTorzsEdit(DB dbc, string dokTipus)
        {
            InitializeComponent();
            this.dbc = dbc;
            this.dokTipus = dokTipus;
            Title= dokTipus;
        }
        /// <summary>
        /// Lap megjelenésekor betölti az kiválasztott doktorzs adatait egy listába és megjeleníti a felhasználónak.
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();

            this.dokTorzs = new KapcssDokTorzs(dbc);

            List<DokTorzs> output = dokTorzs.GetDokTorzsObjects(dokTipus);

            DokTorzsListView.ItemsSource = dokTorzs.GetDokTorzsObjects(dokTipus);
        }
        /// <summary>
        /// Kezeli a DokTorzsListView ItemTapped eseményét.
        /// </summary>
        private async void DokTorzsListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            // navigálás DokTorzsItem lapra a kiválasztott doktorzs tipussal.
            var Tapped = (DokTorzs)e.Item;
            await Navigation.PushAsync(new Views.DokTorzsItem(dbc, Tapped, dokTipus));
        }
        /// <summary>
        /// Kezeli az Új DokTorzs gomb eseménykezelője.
        /// </summary>
        private async void Uj_DokTorzs_Clicked(object sender, EventArgs e)
        {
            // navigálás DokTorzsItem lapra.
            await Navigation.PushAsync(new Views.DokTorzsItem(dbc, dokTipus));
        }
    }
}