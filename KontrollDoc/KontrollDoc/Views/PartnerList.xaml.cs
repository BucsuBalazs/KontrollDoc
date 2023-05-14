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
    public partial class PartnerList : TabbedPage
    {
        DB dbc;
        int selectedItemAzonosito;

        List<Partner> partners;

        public PartnerList(DB dbc)
        {
            InitializeComponent();

            this.dbc = dbc;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            Partner partnerek = new Partner(dbc);

            partners = partnerek.GetPartnerLista();

            PartnerListView.ItemsSource= partners;
            countLabel.Text = "összesen: " + partners.Count;
            Filter();
        }

        async private void Uj_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Views.PartnerNew(dbc));
        }

        void Filter()
        {
            List<Partner> szurt = partners;

            if (!string.IsNullOrEmpty(Kod_Entry.Text))
            {
                szurt = szurt.FindAll(P => P.kod == Kod_Entry.Text);
            }

            if (!string.IsNullOrEmpty(Nev_Entry.Text))
            {
                szurt = szurt.FindAll(P => P.Nev == Nev_Entry.Text);
            }

            PartnerListView.ItemsSource = szurt;
            int itemCount = ((IList)PartnerListView.ItemsSource)?.Count ?? 0;
            countLabel.Text = "összesen: " + itemCount;
        }

        async private void PartnerListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            Partner obj = (Partner)e.Item;
            selectedItemAzonosito = obj.Azonosito;

            await Navigation.PushAsync(new Views.PartnerEdit(dbc, selectedItemAzonosito));
        }

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