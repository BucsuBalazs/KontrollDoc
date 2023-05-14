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

namespace KontrollDoc.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DocNew : ContentPage
    {
        DB dbc;
        List<FileResult> csatolmanyok_list = new List<FileResult>();
        List<Partner> partnerek;
        List<Partner> sortedPartnerek;
        KapcssDokTorzs doktorzs;
        int Fodokumentum_Sorszama = 0;

        public DocNew(DB dbc)
        {
            InitializeComponent();
            this.dbc = dbc;
        }

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

        protected override void OnAppearing() {
            base.OnAppearing();

            doktorzs = new KapcssDokTorzs(dbc);

            List<string> tipusok = doktorzs.GetDokTorzs("Tipus");
            List<string> temak = doktorzs.GetDokTorzs("Tema");
            List<string> hordozok = doktorzs.GetDokTorzs("Hordozo");
            List<string> hivatkozasok = doktorzs.GetDokTorzs("Projekt");

            Partner partner = new Partner(dbc);

            partnerek = partner.GetPartnerLista();
            sortedPartnerek = partnerek.OrderBy(p => p.Nev).ToList();

            Tipus_Picker.ItemsSource = tipusok;
            Tema_Picker.ItemsSource = temak;
            Hordozo_Picker.ItemsSource = hordozok;
            Project_Picker.ItemsSource = hivatkozasok;
            Partner_Nev_Picker.ItemsSource = sortedPartnerek;
            Partner_Nev_Picker.ItemDisplayBinding = new Binding("Nev");
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        async private void Csatolmany_Hozzaadasa_Clicked(object sender, EventArgs e)
        {

            try
            {
                var csatolmany = await FilePicker.PickMultipleAsync();

                if (csatolmany != null)
                {
                    csatolmanyok_list.AddRange(csatolmany);
                    csatolmanyok_ListView.ItemsSource = csatolmanyok_list;

                    //csatolmanyok_ListView.ItemsSource.ad

                    // Get the stream of the selected csatolmany
                    // Stream stream = await csatolmany.OpenReadAsync();

                    // Do something with the stream
                    // ...
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }
        private void Csatolmany_Torlese_Clicked(object sender, EventArgs e)
        {
            //csatolmany = null;
            //csatolmanyok_ListView.ItemsSource = null;
        }

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

        private void Partner_Kod_Entry_TextChanged(object sender, TextChangedEventArgs e)
        {
            var keres = sortedPartnerek.Find(p => p.kod == Partner_Kod_Entry.Text);
            if (keres != null)
            {
                Partner_Nev_Picker.SelectedItem = keres;
            }
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
        }


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

        private async void Rogzit_Clicked(object sender, EventArgs e)
        {

            /*List<System.Data.SqlClient.SqlParameter> empty = new List<System.Data.SqlClient.SqlParameter>();
            DataTable DokTorzsLista = dbc.GetTableFromSPAB("DokTorzsLista", empty);

            var asd = Tipus_Picker.SelectedItem;
            DataRow[] Tipus = DokTorzsLista.Select($"Megnevezes =  '{Tipus_Picker.SelectedItem}'" );
            var TipusId = (int)Tipus[0][0];

            DataRow[] temak = DokTorzsLista.Select($"Megnevezes = '{Tema_Picker.SelectedItem}'");
            var TemaId = (int)temak[0][0];

            DataRow[] hordozok = DokTorzsLista.Select($"Megnevezes = '{Hordozo_Picker.SelectedItem}'");
            var HordozokId = (int)hordozok[0][0];

            DataRow[] hivatkozasok = DokTorzsLista.Select($"Megnevezes = '{Project_Picker.SelectedItem}'");
            var HivId = (int)hordozok[0][0];*/
            try
            {

                var Tipus = this.doktorzs.dokTorzs.Find(d => d.Megnevezes == Tipus_Picker.SelectedItem.ToString());
                var Tema = this.doktorzs.dokTorzs.Find(d => d.Megnevezes == Tema_Picker.SelectedItem.ToString());
                var Hordozo = this.doktorzs.dokTorzs.Find(d => d.Megnevezes == Hordozo_Picker.SelectedItem.ToString());
                //var Hiv = this.doktorzs.dokTorzs.Find(d => d.Megnevezes == Project_Picker.SelectedItem.ToString());

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

                SqlParameter PartnerAz = new SqlParameter();
                PartnerAz.ParameterName = "@PartnerAz";
                //var Partner = this.partnerek.Find(d => d.kod == Partner_Kod_Picker.SelectedItem.ToString());
                PartnerAz.Value = Partner_Kod_Entry.Text;
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
                Megjegyzes.Value = Megjegyzes_Entry.Text;
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

                long DokAz = dbc.ExecuteSPAB("DokumentumUj", sqlParameters);


                foreach (var file in csatolmanyok_list)
                {

                    //List<SqlParameter> DokHivatkozasUjparams = new List<SqlParameter>();

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

                    dbc.ExecuteSPAB("KapcsDokHivFrissit", KapcsDokHivFrissitparams);
                }
                
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

                dbc.ExecuteSPAB("DokHelyeModosit", DokHelyeModositparams);


                await Navigation.PopAsync();
            }
            catch(Exception ex) { await DisplayAlert("Hiba", ex.Message, "ok"); }

        }

        private async void Tipus_Button_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Views.DokTorzsEdit(dbc, "Tipus"));
        }

        private async void Tema_Button_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Views.DokTorzsEdit(dbc, "Tema"));
        }

        private async void Hordozo_Button_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Views.DokTorzsEdit(dbc, "Hordozo"));
        }

        private async void Project_Button_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Views.DokTorzsEdit(dbc, "Projekt"));
        }
    }
}