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
    /// <summary>
    /// Egy TabbedPage amely megjeleníti és feltölti adatokkal a Listázás című ContentPage-t és a Szűrés című ContentPage-t.
    /// </summary>
    public partial class DocList : TabbedPage
    {
        /// <summary>
        /// Az adatbázis környezet.
        /// </summary>
        DB dbc;
        /// <summary>
        /// A kiválasztott dokumentum azonosítója.
        /// </summary>
        int selectedItemAzonosito;

        /// <summary>
        /// A dokumentumok törzs adatai.
        /// </summary>
        KapcssDokTorzs doktorzs;
        /// <summary>
        /// Lista a partnerekről.
        /// </summary>
        List<Partner> partnerek;
        /// <summary>
        /// Partnerek sorba rendezett listája.
        /// </summary>
        List<Partner> sortedPartnerek;
        /// <summary>
        /// A dokumentumok listája.
        /// </summary>
        List<Dokumentum> dokumentumok;
        /// <summary>
        /// A dokumentumok listája.
        /// </summary>
        /// <summary>
        /// Azt jelzi, hogy az értékváltozás programozottan aktiválódik-e.
        /// </summary>
        private bool isProgrammaticChange = false;

        /// <summary>
        /// Inicializálja a <see cref="DocList"/> osztály új példányát.
        /// </summary>
        /// <param name="dbc">Az adatbázis környezet.</param>
        public DocList(DB dbc)
        {
            InitializeComponent();
            this.dbc = dbc;
            this.doktorzs = new KapcssDokTorzs(dbc);
            Partner partner = new Partner(dbc);
            this.partnerek = partner.GetPartnerLista();
        }

        /// <summary>
        /// Az oldal megjelenésekor hívják. Betölti a dokumentumokat.
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();

            LoadData();
        }

        /// <summary>
        /// Betölti az adatokat a Listázás című ContentPage-be és a Szűrés című ContentPage-be
        /// </summary>
        private void LoadData()
        {
            // szűrök beállítása
            Iktato_Entry.Text = null;
            Partner_Kod_Entry.Text = null;
            Sorszam_Entry.Text = null;
            Targy_Entry.Text = null;
            Telefon_Entry.Text = null;
            Megjegyzes_Entry.Text = null;
            Hasznalt_CheckBox.IsChecked = false;
            Felvetel_Checkbox.IsChecked = false;
            Hatarido_Checkbox.IsChecked = false;
            // A szűrő pickerekbe való listák beállítása
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

            //sortedPartnerek = partnerek.OrderBy(p => p.Nev).ToList();

            // A pickerekbe való listák betöltése a pickerekbe
            Tipus_Picker.ItemsSource = tipusok;
            Tema_Picker.ItemsSource = temak;
            Hordozo_Picker.ItemsSource = hordozok;
            Project_Picker.ItemsSource = hivatkozasok;
            Partner_Nev_Picker.ItemsSource = partnerek;
            Partner_Nev_Picker.ItemDisplayBinding = new Binding("Nev");

            // Dokumentumok betöltése
            Dokumentum docs = new Dokumentum();

            dokumentumok = docs.GetDokumentumok(dbc);

            // documentumok megjelenítése
            DocListView.ItemsSource = dokumentumok;
            Filter();
            int itemCount = ((IList)DocListView.ItemsSource).Count;
            countLabel.Text = "összesen: " + itemCount;

        }
        /// <summary>
        /// Kezeli az Uj gomb eseményét.
        /// </summary>
        async private void Uj_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Views.DocNew(dbc));
        }
        /// <summary>
        /// Kezeli a Újratölt gomb eseményét.
        /// </summary>
        private void Refresh_Clicked(object sender, EventArgs e)
        {
            LoadData();
        }
        /// <summary>
        /// Kezeli a DocListView ItemTapped eseményét.
        /// </summary>
        async private void DocListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            // kiválasztott elem azonosítójának beállítása.
            Dokumentum obj = (Dokumentum)e.Item;
            selectedItemAzonosito = obj.Azonosito;
            // navigálás a DocDetails lapra.
            await Navigation.PushAsync(new Views.DocDetails(dbc, selectedItemAzonosito));
        }

        /// <summary>
        /// Szűri a dokumentumokat a kiválasztott kritériumok alapján.
        /// </summary>
        void Filter()
        {
            // Új lista szűrésre
            List<Dokumentum> szurt = dokumentumok;
            // dokumentumok szűrése a megfelelő kritériumok mentén és a szűrt listába helyezése.
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
                var Partner = this.partnerek.Find(p => p.kod == Partner_Kod_Entry.Text.ToString());
                if (Partner != null) 
                {
                    szurt = szurt.FindAll(dok => dok.PartnerAz == Partner.Azonosito);
                }
            }
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
            // szűrt lista megjelenítése
            DocListView.ItemsSource = szurt;
            int itemCount = ((IList)DocListView.ItemsSource)?.Count ?? 0;
            countLabel.Text = "összesen: " + itemCount;
        }

        /// <summary>
        /// Kezeli a Tipus_Picker SelectedIndexChanged eseményét.
        /// </summary>
        private void Tipus_Picker_SelectedIndexChanged(object sender, EventArgs e)
        {
            Filter();
        }

        /// <summary>
        /// Kezeli a Iktato_Entry TextChanged eseményét.
        /// </summary>
        private void Iktato_Entry_TextChanged(object sender, TextChangedEventArgs e)
        {
            Filter();
        }
        /// <summary>
        /// Kezeli a Hasznalt_CheckBox CheckedChanged eseményét.
        /// </summary>
        private void Hasznalt_CheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            Filter();
        }
        /// <summary>
        /// Kezeli a Partner_Nev_Entry TextChanged eseményét.
        /// </summary>
        private void Partner_Kod_Entry_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (isProgrammaticChange) return;
            var keres = partnerek.Find(p => p.kod == Partner_Kod_Entry.Text);
            if (keres != null)
            {
                isProgrammaticChange = true;
                Partner_Nev_Picker.SelectedItem = keres;
                isProgrammaticChange = false;
            }
            else
            {
                isProgrammaticChange = true;
                Partner_Nev_Picker.SelectedItem = null;
                isProgrammaticChange = false;
            }
            Filter();
        }
        /// <summary>
        /// Kezeli a Partner_Nev_Picker SelectedIndexChanged eseményét.
        /// </summary>
        private void Partner_Nev_Picker_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isProgrammaticChange) return;
            if (Partner_Nev_Picker.SelectedItem != null)
            {
                Partner selectedPartner = (Partner)Partner_Nev_Picker.SelectedItem;
                isProgrammaticChange = true;
                Partner_Kod_Entry.Text = selectedPartner.kod;
                isProgrammaticChange = false;
            }
            Filter();
        }
        /// <summary>
        /// Kezeli a Sorszam_Entry TextChanged eseményét.
        /// </summary>
        private void Sorszam_Entry_TextChanged(object sender, TextChangedEventArgs e)
        {
            Filter();
        }
        /// <summary>
        /// Kezeli a Tema_Picker SelectedIndexChanged eseményét.
        /// </summary>
        private void Tema_Picker_SelectedIndexChanged(object sender, EventArgs e)
        {
            Filter();
        }
        /// <summary>
        /// Kezeli a Targy_Entry TextChanged eseményét.
        /// </summary>
        private void Targy_Entry_TextChanged(object sender, TextChangedEventArgs e)
        {
            Filter();
        }
        /// <summary>
        /// Kezeli a Telefon_Entry TextChanged eseményét.
        /// </summary>
        private void Telefon_Entry_TextChanged(object sender, TextChangedEventArgs e)
        {
            Filter();
        }
        /// <summary>
        /// Kezeli a Hordozo_Picker SelectedIndexChanged eseményét.
        /// </summary>
        private void Hordozo_Picker_SelectedIndexChanged(object sender, EventArgs e)
        {
            Filter();
        }
        /// <summary>
        /// Kezeli a Felvetel_Checkbox CheckedChanged eseményét.
        /// </summary>
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
        /// <summary>
        /// Kezeli a Felvetel_DatePicker PropertyChanged eseményét.
        /// </summary>
        private void Felvetel_DatePicker_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Filter();
        }
        /// <summary>
        /// Kezeli a Hatarido_Checkbox CheckedChanged eseményét.
        /// </summary>
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
        /// <summary>
        /// Kezeli a Hatarido_DatePicker PropertyChanged eseményét.
        /// </summary>
        private void Hatarido_DatePicker_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Filter();
        }
        /// <summary>
        /// Kezeli a Project_Picker SelectedIndexChanged eseményét.
        /// </summary>
        private void Project_Picker_SelectedIndexChanged(object sender, EventArgs e)
        {
            Filter();
        }
        /// <summary>
        /// Kezeli a Megjegyzes_Entry TextChanged eseményét.
        /// </summary>
        private void Megjegyzes_Entry_TextChanged(object sender, TextChangedEventArgs e)
        {
            Filter();
        }

    }
}