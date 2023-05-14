using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using System.Data.SqlClient;
using FIT_Common;
using KontrollDoc.Models;
using System.Collections;

namespace KontrollDoc.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DocList : TabbedPage
    {
        DB dbc;
        int selectedItemAzonosito;

        KapcssDokTorzs doktorzs;
        List<Partner> partnerek;
        List<Partner> sortedPartnerek;

        List<Dokumentum> dokumentumok;

        public DocList(DB dbc)
        {
            InitializeComponent();
            this.dbc = dbc;
            this.doktorzs = new KapcssDokTorzs(dbc);
            Partner partner = new Partner(dbc);
            this.partnerek = partner.GetPartnerLista();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            List<string> tipusok = new List<string>();
            tipusok.Add("");
            tipusok.AddRange(this.doktorzs.GetDokTorzs("Tipus"));

            List<string> temak = new List<string>();
            temak.Add("");
            temak.AddRange(this.doktorzs.GetDokTorzs("Tema"));

            List<string> hordozok = new List<string>();
            hordozok.Add("");
            hordozok.AddRange(this.doktorzs.GetDokTorzs("Hordozo"));

            List<string> hivatkozasok = new List<string>(); 
            hivatkozasok.Add("");
            hivatkozasok.AddRange(this.doktorzs.GetDokTorzs("Projekt"));

            var kodok = this.partnerek.Select(p => p.kod).ToList();
            List<string> partnerkodok = new List<string>();
            partnerkodok.Add("");
            partnerkodok.AddRange(kodok);

            var nevek = this.partnerek.Select(p => p.Nev).ToList();
            List<string> partnernevek = new List<string>();
            partnernevek.Add("");
            partnernevek.AddRange(nevek);

            sortedPartnerek = partnerek.OrderBy(p => p.Nev).ToList();

            Tipus_Picker.ItemsSource = tipusok;
            Tema_Picker.ItemsSource = temak;
            Hordozo_Picker.ItemsSource = hordozok;
            Project_Picker.ItemsSource = hivatkozasok;
            Partner_Nev_Picker.ItemsSource = sortedPartnerek;
            Partner_Nev_Picker.ItemDisplayBinding = new Binding("Nev");

            Dokumentum docs = new Dokumentum();

            dokumentumok = docs.GetDokumentumok(dbc);

            DocListView.ItemsSource = dokumentumok;
            Filter();
            int itemCount = ((IList)DocListView.ItemsSource).Count;
            countLabel.Text = "összesen: "+itemCount;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        async private void Uj_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Views.DocNew(dbc));
        }

        async private void DocListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            //Tipus_Picker.sel

            Dokumentum obj = (Dokumentum)e.Item;
            selectedItemAzonosito = obj.Azonosito;
            await Navigation.PushAsync(new Views.DocDetails(dbc, selectedItemAzonosito));
        }

        void Filter()
        {
            List<Dokumentum> szurt = dokumentumok;

            if (Tipus_Picker.SelectedItem != null && !string.IsNullOrEmpty(Tipus_Picker.SelectedItem.ToString())){
                var Tipus = this.doktorzs.dokTorzs.Find(torzs => torzs.Megnevezes == Tipus_Picker.SelectedItem.ToString());
                szurt = szurt.FindAll(dok => dok.TipusAz == Tipus.Azonosito);
            }
            if (!string.IsNullOrEmpty(Iktato_Entry.Text))
            {
                szurt = szurt.FindAll(dok => dok.Iktato == Iktato_Entry.Text);
            }
            if (Hasznalt_CheckBox.IsChecked) 
            {
                szurt = szurt.FindAll(dok => dok.Inaktiv == true);
            }
            if (!string.IsNullOrEmpty(Partner_Kod_Entry.Text))
            {
                var Partner = this.partnerek.Find(d => d.kod == Partner_Kod_Entry.Text);
                if (Partner != null) 
                {
                    szurt = szurt.FindAll(dok => dok.PartnerAz == Partner.Azonosito);
                }
            }
            /*if (Partner_Nev_Picker.SelectedItem != null && !string.IsNullOrEmpty(Partner_Nev_Picker.SelectedItem.ToString())) 
            {
                //var Partner = this.partnerek.Find(d => d.kod == Partner_Nev_Picker.SelectedItem);
                szurt = szurt.FindAll(dok => dok == Partner_Nev_Picker.SelectedItem);
            }*/
            if (!string.IsNullOrEmpty(Sorszam_Entry.Text)) 
            {
                szurt = szurt.FindAll(dok => dok.Sorszam == int.Parse(Sorszam_Entry.Text));
            }
            if (Tema_Picker.SelectedItem != null && !string.IsNullOrEmpty(Tema_Picker.SelectedItem.ToString()))
            {
                var Tema = this.doktorzs.dokTorzs.Find(torzs => torzs.Megnevezes == Tema_Picker.SelectedItem.ToString());
                szurt = szurt.FindAll(dok => dok.TemaAz == Tema.Azonosito);
            }
            if (!string.IsNullOrEmpty(Targy_Entry.Text)) 
            {
                szurt = szurt.FindAll(dok => dok.Targy == Targy_Entry.Text);
            }
            if (!string.IsNullOrEmpty(Telefon_Entry.Text))
            {
                szurt = szurt.FindAll(dok => dok.Telefonszam == Telefon_Entry.Text);
            }
            if (Hordozo_Picker.SelectedItem != null && !string.IsNullOrEmpty(Hordozo_Picker.SelectedItem.ToString()))
            {
                var Hordozo = this.doktorzs.dokTorzs.Find(torzs => torzs.Megnevezes == Hordozo_Picker.SelectedItem.ToString());
                szurt = szurt.FindAll(dok => dok.HordozoAz == Hordozo.Azonosito);
            }
            if (Felvetel_Checkbox.IsChecked && Felvetel_DatePicker != null && Felvetel_DatePicker.Date > new DateTime(1900, 1, 1) ) 
            {
                if (szurt != null)
                {
                    szurt = szurt.FindAll(dok => dok.Datum == Felvetel_DatePicker.Date);
                }
            }
            if (Hatarido_Checkbox.IsChecked && Hatarido_DatePicker != null && Hatarido_DatePicker.Date > new DateTime(1900, 1, 1))
            {
                if (szurt != null)
                {
                    szurt = szurt.FindAll(dok => dok.Hatarido == Hatarido_DatePicker.Date);
                }
            }
            if (Project_Picker.SelectedItem != null && !string.IsNullOrEmpty(Project_Picker.SelectedItem.ToString()))
            {
                var Hiv = this.doktorzs.dokTorzs.Find(torzs => torzs.Megnevezes == Project_Picker.SelectedItem.ToString());
                szurt = szurt.FindAll(dok => dok.ProjecktHivatkozasAz == Hiv.Azonosito);
            }
            if (!string.IsNullOrEmpty(Megjegyzes_Entry.Text))
            {
                szurt = szurt.FindAll(dok => dok.Megjegyzes == Megjegyzes_Entry.Text);
            }

            DocListView.ItemsSource = szurt;
            int itemCount = ((IList)DocListView.ItemsSource)?.Count ?? 0;
            countLabel.Text = "összesen: " + itemCount;
        }

        private void Tipus_Picker_SelectedIndexChanged(object sender, EventArgs e)
        {
            Filter();
        }

        private void Iktato_Entry_TextChanged(object sender, TextChangedEventArgs e)
        {
            Filter();
        }

        private void Hasznalt_CheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            Filter();
        }

        private void Partner_Kod_Entry_TextChanged(object sender, TextChangedEventArgs e)
        {

            var keres = sortedPartnerek.Find(p => p.kod == Partner_Kod_Entry.Text);
            if (keres != null)
            {
                Partner_Nev_Picker.SelectedItem = keres;
            }
            Filter();
        }

        private void Partner_Nev_Picker_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Partner_Nev_Picker.SelectedItem != null)
            {
                Partner talalt = (Partner)Partner_Nev_Picker.SelectedItem;
                if (talalt != null)
                {
                    Partner_Kod_Entry.Text = talalt.kod.ToString();
                }
                
            }
            Filter();
        }


        private void Sorszam_Entry_TextChanged(object sender, TextChangedEventArgs e)
        {
            Filter();
        }

        private void Tema_Picker_SelectedIndexChanged(object sender, EventArgs e)
        {
            Filter();
        }

        private void Targy_Entry_TextChanged(object sender, TextChangedEventArgs e)
        {
            Filter();
        }

        private void Telefon_Entry_TextChanged(object sender, TextChangedEventArgs e)
        {
            Filter();
        }

        private void Hordozo_Picker_SelectedIndexChanged(object sender, EventArgs e)
        {
            Filter();
        }

        private void Felvetel_Checkbox_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (Felvetel_Checkbox.IsChecked == true)
            {
                Felvetel_DatePicker.IsEnabled = true;
            }
            else
            {
                Felvetel_DatePicker.IsEnabled = false;
            }
        }

        private void Felvetel_DatePicker_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Filter();
        }

        private void Hatarido_Checkbox_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (Hatarido_Checkbox.IsChecked == true)
            {
                Hatarido_DatePicker.IsEnabled = true;
            }
            else
            {
                Hatarido_DatePicker.IsEnabled = false;
            }
        }

        private void Hatarido_DatePicker_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Filter();
        }

        private void Project_Picker_SelectedIndexChanged(object sender, EventArgs e)
        {
            Filter();
        }

        private void Megjegyzes_Entry_TextChanged(object sender, TextChangedEventArgs e)
        {
            Filter();
        }

    }
}