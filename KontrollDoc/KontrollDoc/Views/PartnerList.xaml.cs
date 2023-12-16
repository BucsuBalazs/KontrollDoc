using FIT_Common;
using KontrollDoc.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace KontrollDoc.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    /// <summary>
    /// Egy TabbedPage amely megjeleníti és feltölti adatokkal a Listázás című ContentPage-t és a Szűrés című ContentPage-t.
    /// </summary>
    public partial class PartnerList : TabbedPage
    {
        /// <summary>
        /// Az adatbázis környezet.
        /// </summary>
        DB dbc;
        /// <summary>
        /// A kiválasztott partner azonosítója.
        /// </summary>
        int selectedItemAzonosito;
        /// <summary>
        /// Lista a partnerekről.
        /// </summary>
        List<Partner> partners;
        /// <summary>
        /// Inicializálja a <see cref="PartnerList"/> osztály új példányát.
        /// </summary>
        /// <param name="dbc">Az adatbázis környezet.</param>
        public PartnerList(DB dbc)
        {
            InitializeComponent();

            this.dbc = dbc;
        }
        /// <summary>
        /// Az oldal megjelenésekor hívják. Betölti a Partnereket.
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();

            Partner partnerek = new Partner(dbc);

            partners = partnerek.GetPartnerLista();

            PartnerListView.ItemsSource= partners;
            countLabel.Text = "összesen: " + partners.Count;
            Filter();
        }
        /// <summary>
        /// a Újratölt gomb eseménykezelője.
        /// </summary>
        async private void Uj_Clicked(object sender, EventArgs e)
        {
            // navigálás a PartnerNew lapra.
            await Navigation.PushAsync(new Views.PartnerNew(dbc));
        }
        /// <summary>
        /// Szűri a partnereket a kiválasztott kritériumok alapján.
        /// </summary>
        void Filter()
        {
            // Új lista szűrésre
            List<Partner> szurt = partners;
            // partnerek szűrése a megfelelő kritériumok mentén és a szűrt listába helyezése.
            if (!string.IsNullOrEmpty(Kod_Entry.Text))
            {
                szurt = szurt.FindAll(P => P.kod == Kod_Entry.Text);
            }

            if (!string.IsNullOrEmpty(Nev_Entry.Text))
            {
                szurt = szurt.FindAll(P => P.Nev == Nev_Entry.Text);
            }
            // szűrt lista megjelenítése
            PartnerListView.ItemsSource = szurt;
            int itemCount = ((IList)PartnerListView.ItemsSource)?.Count ?? 0;
            countLabel.Text = "összesen: " + itemCount;
        }
        /// <summary>
        /// Kezeli a PartnerListView ItemTapped eseményét.
        /// </summary>
        async private void PartnerListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            Partner obj = (Partner)e.Item;
            selectedItemAzonosito = obj.Azonosito;

            await Navigation.PushAsync(new Views.PartnerEdit(dbc, selectedItemAzonosito));
        }
        /// <summary>
        /// Kezeli a PartnerListView ItemTapped eseményét.
        /// </summary>
        private void Kod_Entry_TextChanged(object sender, TextChangedEventArgs e)
        {
            Filter();
        }

        private void Nev_Entry_TextChanged(object sender, TextChangedEventArgs e)
        {
            Filter();
        }
    }
}