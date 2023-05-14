using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using FIT_Common;
using KontrollDoc.Models;

namespace KontrollDoc.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DocDetails : ContentPage
    {
        DB dbc;
        int selectedItemAzonosito;
        //public static IEnumerable<FileResult> csatolmany = null;
        List<FileResult> csatolmanyok_list = new List<FileResult>();
        List<DokHivatkozas> csatolmanyok_class_list = new List<DokHivatkozas>();

        KapcssDokTorzs doktorzs;
        List<Partner> partnerek;
        List<Partner> sortedPartnerek;

        public DocDetails(DB dbc, int selectedItemAzonosito)
        {
            InitializeComponent();
            this.dbc = dbc;
            this.selectedItemAzonosito = selectedItemAzonosito;
            Partner partner = new Partner(dbc);
            this.partnerek = partner.GetPartnerLista();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            this.doktorzs = new KapcssDokTorzs(dbc);
            List<string> tipusok = this.doktorzs.GetDokTorzs("Tipus");
            List<string> temak = this.doktorzs.GetDokTorzs("Tema");
            List<string> hordozok = this.doktorzs.GetDokTorzs("Hordozo");
            List<string> hivatkozasok = this.doktorzs.GetDokTorzs("Projekt");

            sortedPartnerek = partnerek.OrderBy(p => p.Nev).ToList();

            Tipus_Picker.ItemsSource = tipusok;
            Tema_Picker.ItemsSource = temak;
            Hordozo_Picker.ItemsSource = hordozok;
            Project_Picker.ItemsSource = hivatkozasok;

            Partner_Nev_Picker.ItemsSource = sortedPartnerek;
            Partner_Nev_Picker.ItemDisplayBinding = new Binding("Nev");

            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            SqlParameter sqlParameter = new SqlParameter();
            sqlParameter.ParameterName = "@DokAz";
            sqlParameter.Value = selectedItemAzonosito;
            sqlParameters.Add(sqlParameter);

            System.Data.DataTable dataTable = dbc.GetTableFromSPAB("DokumentumAdat", sqlParameters);
            System.Data.DataRow dr = dataTable.Rows[0];

            List<SqlParameter> torzs = new List<SqlParameter>();
            DataTable DokTorzs = dbc.GetTableFromSPAB("DokTorzsLista", torzs);

            for (int i = 0; i < DokTorzs.Rows.Count; i++)
            {
                if ((int)dr["TipusAz"] == (int)DokTorzs.Rows[i]["Azonosito"]) { Tipus_Picker.SelectedItem = (string)DokTorzs.Rows[i]["Megnevezes"]; }
                if ((int)dr["TemaAz"] == (int)DokTorzs.Rows[i]["Azonosito"]) { Tema_Picker.SelectedItem = (string)DokTorzs.Rows[i]["Megnevezes"]; }
                if ((int)dr["HordozoAz"] == (int)DokTorzs.Rows[i]["Azonosito"]) { Hordozo_Picker.SelectedItem = (string)DokTorzs.Rows[i]["Megnevezes"]; }
                if ((int)dr["ProjektHivatkozasAz"] == (int)DokTorzs.Rows[i]["Azonosito"]) { Project_Picker.SelectedItem = (string)DokTorzs.Rows[i]["Megnevezes"]; }
            }

            Iktato_Entry.Text = (string)dr["Iktato"];

            if ((int)dr["Inaktiv"] == 0)
            {
                Hasznalt_CheckBox.IsChecked = false;
            }
            else
            {
                Hasznalt_CheckBox.IsChecked = true;
            }

            for (int i = 0; i < sortedPartnerek.Count; i++)
            {
                Partner selected = sortedPartnerek.Find(p => p.Azonosito == (int)dr["PartnerAz"]);
                if (selected != null)
                {
                    Partner_Kod_Entry.Text = selected.kod;
                    Partner_Nev_Picker.SelectedItem = selected;
                }
            }

            /*List<SqlParameter> empty = new List<SqlParameter>();
            System.Data.DataTable PartnerLista = dbc.GetTableFromSPAB("PartnerLista", empty);

            for (int i = 0; i < PartnerLista.Rows.Count; i++)
            {
                if ((int)dr["PartnerAz"] == (int)PartnerLista.Rows[i]["Azonosito"])
                {
                    Partner_Kod_Entry.Text = (string)PartnerLista.Rows[i]["kod"];
                    Partner_Nev_Picker.SelectedItem = (string)PartnerLista.Rows[i]["Nev"];
                }
            }*/

            Sorszam_Entry.Text = dr["Sorszam"].ToString();

            Targy_Entry.Text = dr["Targy"].ToString();

            Telefon_Entry.Text = dr["Telefonszam"].ToString();

            var Read_Datum = DateTime.Parse(dr["Datum"].ToString());

            Felvetel_DatePicker.Date = Read_Datum;

            if (dr["Hatarido"] != DBNull.Value)
            {
                Hatarido_Checkbox.IsChecked = true;
                Hatarido_DatePicker.IsEnabled = true;
                var Read_Hatar = DateTime.Parse(dr["Hatarido"].ToString());
                Hatarido_DatePicker.Date = Read_Hatar;
                Hatarido_DatePicker.BackgroundColor = Color.White;
            }



            Megjegyzes_Entry.Text = dr["Megjegyzes"].ToString();

            //sqlParameters.Clear();

            List<SqlParameter> sqlParameters2 = new List<SqlParameter>();
            SqlParameter sqlParameter2 = new SqlParameter();
            sqlParameter2.ParameterName = "@DokAz";
            sqlParameter2.Value = this.selectedItemAzonosito;
            sqlParameters2.Add(sqlParameter2);

            DataTable dataTable2 = dbc.GetTableFromSPAB("DokHivatkozasAdat", sqlParameters2);
            //System.Data.DataRow dr2 = dataTable.Rows[0];

            foreach (DataRow dr2 in dataTable2.Rows)
            {
                DokHivatkozas csat = new DokHivatkozas();
                csat.Azonosito = (int)dr2["Azonosito"];
                csat.DokFilenev = dr2["DokFilenev"].ToString();
                csat.DokPath = dr2["DokPath"].ToString();
                csat.Inaktiv = (bool)dr2["Inaktiv"];
                csatolmanyok_class_list.Add(csat);
                //csat.ImageData = (byte[])dr2["ImageData"];

                FileResult file = new FileResult(csat.DokPath);
                file.FileName = csat.DokFilenev;
                //file = new MemoryStream(csat.ImageData);
                this.csatolmanyok_list.Add(file);

                //var fileStream = new MemoryStream(csat.ImageData);

                //var asd = new FileBase(fileStream);
                //File.Wr
                //var asd = new File(fileStream, "application/octet-stream", csat.DokFilenev);
            }
            //csatolmany.
            csatolmanyok_ListView.ItemsSource = this.csatolmanyok_list;

            int sorszam = int.Parse(Sorszam_Entry.Text);
            Dokumentum getdocs = new Dokumentum();
            List<Dokumentum> docs = getdocs.GetDokumentumok(dbc);
            var results = docs.FindAll(dok => dok.FoDokumentum == sorszam);
            Mellékletek_ListView.ItemsSource = results;

            Dokhelye dokhelye = new Dokhelye();
            List<Dokhelye> doks = dokhelye.GetDocHely(dbc);
            var geth = doks.Find(d => d.Dokaz == selectedItemAzonosito);

            Irattar1_Entry.Text = geth.Irattar1;
            Irattar2_Entry.Text = geth.Irattar2;
            Irattar3_Entry.Text = geth.Irattar3;
            Egyeb_Entry.Text = geth.Egyeb;

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

                    foreach (var file in csatolmany)
                    {
                        List<SqlParameter> KapcsDokHivFrissitparams = new List<SqlParameter>();

                        SqlParameter DokAzparam = new SqlParameter();
                        DokAzparam.ParameterName = "@DokAz";
                        DokAzparam.Value = this.selectedItemAzonosito;
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

                        var ujHivAz = dbc.ExecuteSPAB("KapcsDokHivUj", KapcsDokHivFrissitparams);

                        DokHivatkozas csat = new DokHivatkozas();
                        csat.Azonosito = (int)ujHivAz;
                        csatolmanyok_class_list.Add(csat);
                    }


                    this.csatolmanyok_list.AddRange(csatolmany);

                    csatolmanyok_ListView.ItemsSource = null;
                    csatolmanyok_ListView.ItemsSource = this.csatolmanyok_list;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }

        async private void csat_letoltes_Clicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var item = button.BindingContext as FileResult;
            var index = csatolmanyok_ListView.TabIndex;
            var hiv = this.csatolmanyok_class_list[index];

            List<SqlParameter> DokHivatkozasGetHivparams = new List<SqlParameter>();
            SqlParameter HivAz = new SqlParameter();
            HivAz.ParameterName = "@HivAz";
            HivAz.Value = hiv.Azonosito;
            DokHivatkozasGetHivparams.Add(HivAz);
            DataTable dt = dbc.GetTableFromSPAB("DokHivatkozasGetHiv", DokHivatkozasGetHivparams);
            DataRow dr = dt.Rows[0];

            DokHivatkozas download = new DokHivatkozas();
            download.Azonosito = (int)dr["Azonosito"];
            download.DokFilenev = dr["DokFilenev"].ToString();
            download.DokPath = dr["DokPath"].ToString();
            download.Inaktiv = (bool)dr["Inaktiv"];
            download.ImageData = (byte[])dr["ImageData"];

            try
            {
                string filePath = await SaveFileToDownloadsFolder(download.DokFilenev, download.ImageData);

                await Launcher.OpenAsync(new OpenFileRequest
                {
                    File = new ReadOnlyFile(filePath)
                });

                //await DisplayAlert("Siker", filePath, "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Hiba", ex.Message, "OK");
            }

        }


        public async Task<string> SaveFileToDownloadsFolder(string fileName, byte[] fileBytes)
        {
            // Get the Downloads folder path
            var downloadsPath = Path.Combine(FileSystem.CacheDirectory, "Downloads");

            // Create the Downloads folder if it doesn't exist
            if (!Directory.Exists(downloadsPath))
            {
                Directory.CreateDirectory(downloadsPath);
            }

            // Create the file path
            var filePath = Path.Combine(downloadsPath, fileName);

            // Write the file to disk
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await fileStream.WriteAsync(fileBytes, 0, fileBytes.Length);
            }

            return filePath;
        }

        private void csat_torles_Clicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var item = button.BindingContext as FileResult;
            var index = csatolmanyok_ListView.TabIndex;
            var hiv = this.csatolmanyok_class_list[index];

            List<SqlParameter> KapcsDokHivFrissitparams = new List<SqlParameter>();

            SqlParameter DokAzparam = new SqlParameter();
            DokAzparam.ParameterName = "@DokAz";
            DokAzparam.Value = this.selectedItemAzonosito;
            KapcsDokHivFrissitparams.Add(DokAzparam);

            SqlParameter HivAzparam = new SqlParameter();
            HivAzparam.ParameterName = "@HivAz";
            HivAzparam.Value = hiv.Azonosito;
            KapcsDokHivFrissitparams.Add(HivAzparam);

            SqlParameter Kapcsparam = new SqlParameter();
            Kapcsparam.ParameterName = "@Kapcs";
            Kapcsparam.Value = 0;
            KapcsDokHivFrissitparams.Add(Kapcsparam);

            dbc.ExecuteSPAB("KapcsDokHivFrissit", KapcsDokHivFrissitparams);

            List<SqlParameter> DokHivTorolparams = new List<SqlParameter>();
            SqlParameter HivAz = new SqlParameter();
            HivAz.ParameterName = "@HivAz";
            HivAz.Value = hiv.Azonosito;
            DokHivTorolparams.Add(HivAz);
            dbc.ExecuteSPAB("DokHivatkozasTorol", DokHivTorolparams);

            this.csatolmanyok_class_list.Remove(hiv);
            this.csatolmanyok_list.Remove(item);
            csatolmanyok_ListView.ItemsSource = null;
            csatolmanyok_ListView.ItemsSource = this.csatolmanyok_list;
        }

        private void Hatarido_CheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
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

        private async void Frissit_Clicked(object sender, EventArgs e)
        {
            try
            {
                Dokumentum modosito_dokumentum = new Dokumentum();
                modosito_dokumentum.Azonosito = this.selectedItemAzonosito;
                modosito_dokumentum.FoDokumentum = null;
                var Tipus = this.doktorzs.dokTorzs.Find(d => d.Megnevezes == Tipus_Picker.SelectedItem.ToString());
                modosito_dokumentum.TipusAz = Tipus.Azonosito;
                modosito_dokumentum.Iktato = Iktato_Entry.Text;
                //var Partner = this.partnerek.Find(d => d.kod == Partner_Kod_Picker.SelectedItem.ToString());
                modosito_dokumentum.PartnerAz = int.Parse( Partner_Kod_Entry.Text);
                var Tema = this.doktorzs.dokTorzs.Find(d => d.Megnevezes == Tema_Picker.SelectedItem.ToString());
                modosito_dokumentum.TemaAz = Tema.Azonosito;
                modosito_dokumentum.Targy = Targy_Entry.Text;
                var Hordozo = this.doktorzs.dokTorzs.Find(d => d.Megnevezes == Hordozo_Picker.SelectedItem.ToString());
                modosito_dokumentum.HordozoAz = Hordozo.Azonosito;
                modosito_dokumentum.UgyintezoAz = (int)dbc.FelhasznaloId;
                modosito_dokumentum.Datum = Felvetel_DatePicker.Date;
                if (Hatarido_Checkbox.IsChecked) { modosito_dokumentum.Hatarido = Hatarido_DatePicker.Date; }
                modosito_dokumentum.FontossagAz = 7;
                modosito_dokumentum.AllapotAz = 10;
                var hiv = this.doktorzs.dokTorzs.Find(d => d.Megnevezes == Project_Picker.SelectedItem.ToString());
                modosito_dokumentum.ProjecktHivatkozasAz = hiv.Azonosito;
                modosito_dokumentum.Megjegyzes = Megjegyzes_Entry.Text;
                modosito_dokumentum.Telefonszam = Telefon_Entry.Text;
                modosito_dokumentum.Bizalmas = null;
                if (Hasznalt_CheckBox.IsChecked) { modosito_dokumentum.Inaktiv = true; }
                else { modosito_dokumentum.Inaktiv = false; }

                List<SqlParameter> DokumentumModositparams = new List<SqlParameter>();

                SqlParameter DokAzparam = new SqlParameter();
                DokAzparam.ParameterName = "@DokAz";
                DokAzparam.Value = modosito_dokumentum.Azonosito;
                DokumentumModositparams.Add(DokAzparam);

                SqlParameter FoDokumentum = new SqlParameter();
                FoDokumentum.ParameterName = "@FoDokumentum";
                FoDokumentum.Value = modosito_dokumentum.FoDokumentum;
                DokumentumModositparams.Add(FoDokumentum);

                SqlParameter Iktato = new SqlParameter();
                Iktato.ParameterName = "@Iktato";
                Iktato.Value = modosito_dokumentum.Iktato;
                DokumentumModositparams.Add(Iktato);

                SqlParameter PartnerAz = new SqlParameter();
                PartnerAz.ParameterName = "@PartnerAz";
                PartnerAz.Value = modosito_dokumentum.PartnerAz;
                DokumentumModositparams.Add(PartnerAz);

                SqlParameter TemaAz = new SqlParameter();
                TemaAz.ParameterName = "@TemaAz";
                TemaAz.Value = modosito_dokumentum.TemaAz;
                DokumentumModositparams.Add(TemaAz);

                SqlParameter Targy = new SqlParameter();
                Targy.ParameterName = "@Targy";
                Targy.Value = modosito_dokumentum.Targy;
                DokumentumModositparams.Add(Targy);

                SqlParameter HordozoAz = new SqlParameter();
                HordozoAz.ParameterName = "@HordozoAz";
                HordozoAz.Value = modosito_dokumentum.HordozoAz;
                DokumentumModositparams.Add(HordozoAz);

                SqlParameter UgyintezoAz = new SqlParameter();
                UgyintezoAz.ParameterName = "@UgyintezoAz";
                UgyintezoAz.Value = modosito_dokumentum.UgyintezoAz;
                DokumentumModositparams.Add(UgyintezoAz);

                SqlParameter TipusAz = new SqlParameter();
                TipusAz.ParameterName = "@TipusAz";
                TipusAz.Value = modosito_dokumentum.TipusAz;
                DokumentumModositparams.Add(TipusAz);

                SqlParameter Datum = new SqlParameter();
                Datum.ParameterName = "@Datum";
                Datum.Value = modosito_dokumentum.Datum;
                DokumentumModositparams.Add(Datum);

                SqlParameter Hatarido = new SqlParameter();
                Hatarido.ParameterName = "@Hatarido";
                Hatarido.Value = modosito_dokumentum.Hatarido;
                DokumentumModositparams.Add(Hatarido);

                SqlParameter FontossagAz = new SqlParameter();
                FontossagAz.ParameterName = "@FontossagAz";
                FontossagAz.Value = modosito_dokumentum.FontossagAz;
                DokumentumModositparams.Add(FontossagAz);

                SqlParameter AllapotAz = new SqlParameter();
                AllapotAz.ParameterName = "@AllapotAz";
                AllapotAz.Value = modosito_dokumentum.AllapotAz;
                DokumentumModositparams.Add(AllapotAz);

                SqlParameter ProjektHivatkozas = new SqlParameter();
                ProjektHivatkozas.ParameterName = "@ProjektHivatkozas";
                ProjektHivatkozas.Value = modosito_dokumentum.ProjecktHivatkozasAz;
                DokumentumModositparams.Add(ProjektHivatkozas);

                SqlParameter Megjegyzes = new SqlParameter();
                Megjegyzes.ParameterName = "@Megjegyzes";
                Megjegyzes.Value = modosito_dokumentum.Megjegyzes;
                DokumentumModositparams.Add(Megjegyzes);

                SqlParameter Telefonszam = new SqlParameter();
                Telefonszam.ParameterName = "@Telefonszam";
                Telefonszam.Value = modosito_dokumentum.Azonosito;
                DokumentumModositparams.Add(Telefonszam);

                SqlParameter UId = new SqlParameter();
                UId.ParameterName = "@UId";
                UId.Value = null;
                DokumentumModositparams.Add(UId);

                SqlParameter Bizalmas = new SqlParameter();
                Bizalmas.ParameterName = "@Bizalmas";
                Bizalmas.Value = modosito_dokumentum.Bizalmas;
                DokumentumModositparams.Add(Bizalmas);

                SqlParameter Inaktiv = new SqlParameter();
                Inaktiv.ParameterName = "@Inaktiv";
                Inaktiv.Value = modosito_dokumentum.Inaktiv;
                DokumentumModositparams.Add(Inaktiv);


                dbc.ExecuteSPAB("DokumentumModosit", DokumentumModositparams);


                await Navigation.PopAsync();
            }
            catch (Exception ex) {
                await DisplayAlert("Hiba", ex.Message, "ok");
            }
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

        private async void Hozzaadas_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Views.DocNew(dbc, int.Parse(Sorszam_Entry.Text)));
        }

        async private void Hely_Button_Clicked(object sender, EventArgs e)
        {
            List<SqlParameter> DokHelyeModositparams = new List<SqlParameter>();

            SqlParameter DokAzparam2 = new SqlParameter();
            DokAzparam2.ParameterName = "@DokAz";
            DokAzparam2.Value = selectedItemAzonosito;
            DokHelyeModositparams.Add(DokAzparam2);

            SqlParameter Irattar1 = new SqlParameter();
            Irattar1.ParameterName = "@Irattar1";
            if (string.IsNullOrEmpty(Irattar1_Entry.Text)) { Irattar1.Value = ""; } else { Irattar1.Value = Irattar1_Entry.Text; }
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

            await DisplayAlert("Siker","Helyezze át a dokumnetumot a megfelelő Irattárba!","Ok");
        }
    }
}