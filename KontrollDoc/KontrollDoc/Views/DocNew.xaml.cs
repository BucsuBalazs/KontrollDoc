using FIT_Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using System.Data.SqlClient;
using KontrollDoc.Models;
using Xamarin.Essentials;
using System.Data;
using System.IO;
using KontrollDoc.Services.DependencyServices;

namespace KontrollDoc.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    /// <summary>
    /// Egy ContentPage amely az új dokumentum létrehozsához használt eseményeket metódusokat biztosítja.
    /// </summary>
    public partial class DocNew : ContentPage
    {
        /// <summary>
        /// Az adatbázis környezet.
        /// </summary>
        DB dbc;
        /// <summary>
        /// A kiválasztott file-ok listája
        /// </summary>
        List<FileResult> csatolmanyok_list = new List<FileResult>();
        /// <summary>
        /// A partnerek listája.
        /// </summary>
        List<Partner> partnerek;
        /// <summary>
        /// Partnerek sorba rendezett listája.
        /// </summary>
        List<Partner> sortedPartnerek;
        /// <summary>
        /// A dokumentumok törzs adatai.
        /// </summary>
        KapcssDokTorzs doktorzs;
        /// <summary>
        /// A fődokumentum sorszáma.
        /// </summary>
        int Fodokumentum_Sorszama = 0;
        /// <summary>
        /// Azt jelzi, hogy az értékváltozás programozottan aktiválódik-e.
        /// </summary>
        private bool isProgrammaticChange = false;
        /// <summary>
        /// Inicializálja a <see cref="DocNew"/> osztály új példányát.
        /// </summary>
        /// <param name="dbc">Az adatbázis környezet.</param>
        public DocNew(DB dbc)
        {
            InitializeComponent();
            this.dbc = dbc;
        }
        /// <summary>
        /// Inicializálja a <see cref="DocNew"/> osztály új példányát a fő dokumentum meghatározott sorszámával.
        /// </summary>
        /// <param name="dbc">Az adatbázis környezet.</param>
        /// <param name="Fodokumentum_Sorszama">A fődokumentum sorszáma.</param>
        public DocNew(DB dbc, int Fodokumentum_Sorszama)
        {
            InitializeComponent();
            this.dbc = dbc;
            this.Fodokumentum_Sorszama = Fodokumentum_Sorszama;

            if (Fodokumentum_Sorszama != 0)
            {
                FoDokuumentum_CheckBox.IsChecked = false;
                FoDokuumentum_Entry.BackgroundColor = Color.LightGray;
                FoDokuumentum_Entry.IsEnabled = false;
                FoDokuumentum_CheckBox.IsEnabled = false;

                FoDokuumentum_Entry.Text = this.Fodokumentum_Sorszama.ToString();
            }
        }
        /// <summary>
        /// Az oldal megjelenésekor hívják. Betölti a doktorzset és megjeleníti a pickerekbe.
        /// </summary>
        protected override void OnAppearing() {
            base.OnAppearing();

            // Szkennelés letiltása androidon.
            if (Device.RuntimePlatform == Device.Android)
            {
                szkan.IsEnabled = false;
            }

            // Doktorzs adatok listákba töltése.
            doktorzs = new KapcssDokTorzs(dbc);

            List<string> tipusok = doktorzs.GetDokTorzs("Tipus");
            List<string> temak = doktorzs.GetDokTorzs("Tema");
            List<string> hordozok = doktorzs.GetDokTorzs("Hordozo");
            List<string> hivatkozasok = doktorzs.GetDokTorzs("Projekt");

            // Partner adatok listákba töltése.
            Partner partner = new Partner(dbc);

            partnerek = partner.GetPartnerLista();
            //sortedPartnerek = partnerek.OrderBy(p => p.Nev).ToList();

            // Listák pickerekbe töltése.
            Tipus_Picker.ItemsSource = tipusok;
            Tema_Picker.ItemsSource = temak;
            Hordozo_Picker.ItemsSource = hordozok;
            Project_Picker.ItemsSource = hivatkozasok;
            Partner_Nev_Picker.ItemsSource = partnerek;
            Partner_Nev_Picker.ItemDisplayBinding = new Binding("Nev");

            // Csatolmányok megjelenítése.
            csatolmanyok_ListView.ItemsSource = csatolmanyok_list;
        }
        /// <summary>
        /// A Szkennelés gomb eseménykezelője.
        /// </summary>
        private async void Szkenneles_Clicked(object sender, EventArgs e)
        {
            // Navigálás a Scan lapra.
            await Navigation.PushAsync(new Views.Scan(dbc));
        }
        /// <summary>
        /// A Kiválasztás gomb eseménykezelője.
        /// </summary>
        async private void Csatolmany_Kivalasztasa_Clicked(object sender, EventArgs e)
        {
            // Eddigi csatolmányok kiszedése.
            csatolmanyok_list.Clear();
            csatolmanyok_ListView.ItemsSource = null;
            try
            {
                // Fájl kereső megjelenítése.
                var csatolmany = await FilePicker.PickMultipleAsync();

                if (csatolmany != null)
                {
                    // Kiválasztott csatolmányok lementése és megjelenítése.
                    csatolmanyok_list.AddRange(csatolmany);
                    csatolmanyok_ListView.ItemsSource = csatolmanyok_list;
                }
            }
            catch (Exception)
            {
                await DisplayAlert("Error", "Fájlok kiválasztása megszakadt", "OK");
            }
        }

        /// <summary>
        /// Kezeli a Csatolmany_Torlese gomb eseménykezelője.
        /// </summary>
        private void Csatolmany_Torlese_Clicked(object sender, EventArgs e) 
        {
            // Csatolmányok lista kiürítése, csatolmanyok_ListView kiürítése.
            csatolmanyok_list.Clear();
            csatolmanyok_ListView.ItemsSource = null;
        }

        /// <summary>
        /// Kezeli a Hatarido_CheckBox CheckedChanged eseményét.
        /// </summary>
        private void Hatarido_CheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (Hatarido_Checkbox.IsChecked == true)
            {
                Hatarido_DatePicker.IsEnabled = true;
                Hatarido_DatePicker.BackgroundColor = Color.White;
            }
            else
            {
                Hatarido_DatePicker.IsEnabled = false;
                Hatarido_DatePicker.BackgroundColor = Color.LightGray;
            }
        }
        /// <summary>
        /// Kezeli a Partner_Kod_Entry TextChanged eseményét.
        /// </summary>
        private void Partner_Kod_Entry_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (isProgrammaticChange) return;
            var keres = partnerek.Find(p => p.kod == Partner_Kod_Entry.Text);
            if (keres != null)
            {
                // Beállítja a pickert is a megfelelő kódú partnerre.
                isProgrammaticChange = true;
                Partner_Nev_Picker.SelectedItem = keres;
                isProgrammaticChange = false;
            }
            else
            {
                // üres ha nincs kiválasztott partner
                isProgrammaticChange = true;
                Partner_Nev_Picker.SelectedItem = null;
                isProgrammaticChange = false;
            }
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
                // Beállítja a kod_entryt is a megfelelő nevű partnerre.
                isProgrammaticChange = true;
                Partner_Kod_Entry.Text = selectedPartner.kod;
                isProgrammaticChange = false;
            }
        }

        /// <summary>
        /// Kezeli a FoDokuumentum_CheckBox CheckedChanged eseményét.
        /// </summary>
        private void FoDokuumentum_CheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (FoDokuumentum_CheckBox != null && FoDokuumentum_CheckBox.IsChecked == true)
            {
                FoDokuumentum_Entry.IsEnabled = false;
                FoDokuumentum_Entry.BackgroundColor = Color.LightGray;
            }
            else
            {
                FoDokuumentum_Entry.IsEnabled = true;
                FoDokuumentum_Entry.BackgroundColor = Color.White;
            }
        }
        /// <summary>
        /// Kezeli a Kiválasztás gomb eseménykezelője. Egy tárolt eljárások futtatásával rögzíti a dokumentumot és catolmányát az adatbázisban.
        /// </summary>
        private async void Rogzit_Clicked(object sender, EventArgs e)
        {
            try
            {
                // Kiválasztott adatok kiszedése a pickerekből.
                var Tipus = this.doktorzs.dokTorzs.Find(d => d.Megnevezes == Tipus_Picker.SelectedItem.ToString());
                var Tema = this.doktorzs.dokTorzs.Find(d => d.Megnevezes == Tema_Picker.SelectedItem.ToString());
                var Hordozo = this.doktorzs.dokTorzs.Find(d => d.Megnevezes == Hordozo_Picker.SelectedItem.ToString());

                // Sql paraméterek beállítása a felhasználó által megadott adatok alapján.
                List<SqlParameter> sqlParameters = new List<SqlParameter>();

                SqlParameter FoDokumentum = new SqlParameter();
                FoDokumentum.ParameterName = "@FoDokumentum";
                if (FoDokuumentum_CheckBox.IsChecked) { FoDokumentum.Value = null; } else { FoDokumentum.Value = int.Parse(FoDokuumentum_Entry.Text); }
                sqlParameters.Add(FoDokumentum);

                SqlParameter TipusAz = new SqlParameter();
                TipusAz.ParameterName = "@TipusAz";
                TipusAz.Value = Tipus.Azonosito;
                sqlParameters.Add(TipusAz);

                SqlParameter Iktato = new SqlParameter();
                Iktato.ParameterName = "@Iktato";
                Iktato.Value = Iktato_Entry.Text;
                sqlParameters.Add(Iktato);

                var Partner = this.partnerek.Find(d => d.kod == Partner_Kod_Entry.Text);
                SqlParameter PartnerAz = new SqlParameter();
                PartnerAz.ParameterName = "@PartnerAz";
                PartnerAz.Value = Partner.Azonosito;
                sqlParameters.Add(PartnerAz);

                SqlParameter TemaAz = new SqlParameter();
                TemaAz.ParameterName = "@TemaAz";
                TemaAz.Value = Tema.Azonosito;
                sqlParameters.Add(TemaAz);

                SqlParameter Targy = new SqlParameter();
                Targy.ParameterName = "@Targy";
                Targy.Value = Targy_Entry.Text;
                sqlParameters.Add(Targy);

                SqlParameter HordozoAz = new SqlParameter();
                HordozoAz.ParameterName = "@HordozoAz";
                HordozoAz.Value = Hordozo.Azonosito;
                sqlParameters.Add(HordozoAz);

                SqlParameter UgyintezoAz = new SqlParameter();
                UgyintezoAz.ParameterName = "@UgyintezoAz";
                UgyintezoAz.Value = (int)dbc.ABFelhasznaloId;
                sqlParameters.Add(UgyintezoAz);

                SqlParameter Datum = new SqlParameter();
                Datum.ParameterName = "@Datum";
                Datum.Value = (DateTime)Felvetel_DatePicker.Date;
                sqlParameters.Add(Datum);

                if (Hatarido_Checkbox.IsChecked == true)
                {
                    SqlParameter Hatarido = new SqlParameter();
                    Hatarido.ParameterName = "@Hatarido";
                    Hatarido.Value = (DateTime)Hatarido_DatePicker.Date;
                    sqlParameters.Add(Hatarido);
                }

                SqlParameter FontossagAz = new SqlParameter();
                FontossagAz.ParameterName = "@FontossagAz";
                FontossagAz.Value = 7;
                sqlParameters.Add(FontossagAz);

                SqlParameter AllapotAz = new SqlParameter();
                AllapotAz.ParameterName = "@AllapotAz";
                AllapotAz.Value = 10;
                sqlParameters.Add(AllapotAz);

                SqlParameter ProjektHivatkozas = new SqlParameter();
                ProjektHivatkozas.ParameterName = "@ProjektHivatkozas";
                var Hiv = this.doktorzs.dokTorzs.Find(torzs => torzs.Megnevezes == Project_Picker.SelectedItem.ToString());
                ProjektHivatkozas.Value = Hiv.Azonosito;
                sqlParameters.Add(ProjektHivatkozas);

                SqlParameter Megjegyzes = new SqlParameter();
                Megjegyzes.ParameterName = "@Megjegyzes";
                if (Megjegyzes_Entry.Text == null) { Megjegyzes.Value = ""; } else { Megjegyzes.Value = Megjegyzes_Entry.Text; }
                sqlParameters.Add(Megjegyzes);

                SqlParameter Telefonszam = new SqlParameter();
                Telefonszam.ParameterName = "@Telefonszam";
                Telefonszam.Value = Telefon_Entry.Text;
                sqlParameters.Add(Telefonszam);

                SqlParameter Bizalmas = new SqlParameter();
                Bizalmas.ParameterName = "@Bizalmas";
                Bizalmas.Value = false;
                sqlParameters.Add(Bizalmas);

                SqlParameter Inaktiv = new SqlParameter();
                Inaktiv.ParameterName = "@Inaktiv";
                if (Hasznalt_CheckBox.IsChecked) { Inaktiv.Value = true; }
                else { Inaktiv.Value = false; }
                sqlParameters.Add(Inaktiv);

                // Tárolt eljárás futtatásával a dokumentum adatainak rögzítése az adatbázisban.
                long DokAz = dbc.ExecuteSPAB("DokumentumUj", sqlParameters);


                // Dokumentumhoz tartozó csatolmányok feldolgozása, tárolt eljáráshoz való sql paraméterek beállítása.
                foreach (var file in csatolmanyok_list)
                {
                    if (file != null)
                    {

                        List<SqlParameter> KapcsDokHivFrissitparams = new List<SqlParameter>();

                        SqlParameter DokAzparam = new SqlParameter();
                        DokAzparam.ParameterName = "@DokAz";
                        DokAzparam.Value = DokAz;
                        KapcsDokHivFrissitparams.Add(DokAzparam);

                        SqlParameter HivAzparam = new SqlParameter();
                        HivAzparam.ParameterName = "@HivAz";
                        HivAzparam.Value = 0;
                        KapcsDokHivFrissitparams.Add(HivAzparam);

                        SqlParameter Kapcsparam = new SqlParameter();
                        Kapcsparam.ParameterName = "@Kapcs";
                        Kapcsparam.Value = 1;
                        KapcsDokHivFrissitparams.Add(Kapcsparam);


                        byte[] fileBytes;

                        using (var stream = await file.OpenReadAsync())
                        {
                            fileBytes = new byte[stream.Length];
                            await stream.ReadAsync(fileBytes, 0, (int)stream.Length);
                        }

                        SqlParameter ImageData = new SqlParameter();
                        ImageData.ParameterName = "@ImageData";
                        ImageData.Value = fileBytes;
                        KapcsDokHivFrissitparams.Add(ImageData);

                        SqlParameter DokFilenev = new SqlParameter();
                        DokFilenev.ParameterName = "@DokFilenev";
                        DokFilenev.Value = file.FileName;
                        KapcsDokHivFrissitparams.Add(DokFilenev);

                        SqlParameter DokPath = new SqlParameter();
                        DokPath.ParameterName = "@DokPath";
                        DokPath.Value = file.FullPath;
                        KapcsDokHivFrissitparams.Add(DokPath);

                        // Tárolt eljárás futtatásával a dokumentum csatolmányainak mentése az adatbázisban.
                        dbc.ExecuteSPAB("KapcsDokHivFrissit", KapcsDokHivFrissitparams);
                    }
                }

                // Dokumentum helyéről szóló adatok feldolgozása, tárolt eljáráshoz való sql paraméterek beállítása.
                List<SqlParameter> DokHelyeModositparams = new List<SqlParameter>();

                SqlParameter DokAzparam2 = new SqlParameter();
                DokAzparam2.ParameterName = "@DokAz";
                DokAzparam2.Value = DokAz;
                DokHelyeModositparams.Add(DokAzparam2);

                SqlParameter Irattar1 = new SqlParameter();
                Irattar1.ParameterName = "@Irattar1";
                if (string.IsNullOrEmpty(Irattar1_Entry.Text)) { Irattar1.Value = "";  } else { Irattar1.Value = Irattar1_Entry.Text; }
                DokHelyeModositparams.Add(Irattar1);

                SqlParameter Irattar2 = new SqlParameter();
                Irattar2.ParameterName = "@Irattar2";
                if (string.IsNullOrEmpty(Irattar2_Entry.Text)) { Irattar2.Value = ""; } else { Irattar2.Value = Irattar2_Entry.Text; }
                DokHelyeModositparams.Add(Irattar2);

                SqlParameter Irattar3 = new SqlParameter();
                Irattar3.ParameterName = "@Irattar3";
                if (string.IsNullOrEmpty(Irattar3_Entry.Text)) { Irattar3.Value = ""; } else { Irattar3.Value = Irattar3_Entry.Text; }
                DokHelyeModositparams.Add(Irattar3);

                SqlParameter Egyeb = new SqlParameter();
                Egyeb.ParameterName = "@Egyeb";
                if (string.IsNullOrEmpty(Egyeb_Entry.Text)) { Egyeb.Value = ""; } else { Egyeb.Value = Egyeb_Entry.Text; }
                DokHelyeModositparams.Add(Egyeb);

                // Tárolt eljárás futtatásával a Dokumentum helyéről szóló adatok mentése az adatbázisban.
                dbc.ExecuteSPAB("DokHelyeModosit", DokHelyeModositparams);


                await Navigation.PopAsync();
            }
            catch(Exception ex) { await DisplayAlert("Hiba", "Figyeljen arra hogy minden *-gal jelölt adatot ki kell tölteni!", "ok"); }

        }
        /// <summary>
        /// Kezeli a Tipus gomb eseménykezelője.
        /// </summary>
        private async void Tipus_Button_Clicked(object sender, EventArgs e)
        {
            // navigálás DokTorzsEdit lapra Tipus doktörzs megnevezésével
            await Navigation.PushAsync(new Views.DokTorzsEdit(dbc, "Tipus"));
        }
        /// <summary>
        /// Kezeli a Téma gomb eseménykezelője.
        /// </summary>
        private async void Tema_Button_Clicked(object sender, EventArgs e)
        {
            // navigálás DokTorzsEdit lapra Téma doktörzs megnevezésével
            await Navigation.PushAsync(new Views.DokTorzsEdit(dbc, "Tema"));
        }
        /// <summary>
        /// Kezeli a Hordozó gomb eseménykezelője.
        /// </summary>
        private async void Hordozo_Button_Clicked(object sender, EventArgs e)
        {
            // navigálás DokTorzsEdit lapra Hordozó doktörzs megnevezésével
            await Navigation.PushAsync(new Views.DokTorzsEdit(dbc, "Hordozo"));
        }
        /// <summary>
        /// Kezeli a projekt gomb eseménykezelője.
        /// </summary>
        private async void Project_Button_Clicked(object sender, EventArgs e)
        {
            // navigálás DokTorzsEdit lapra Projekt doktörzs megnevezésével
            await Navigation.PushAsync(new Views.DokTorzsEdit(dbc, "Projekt"));
        }
    }
}