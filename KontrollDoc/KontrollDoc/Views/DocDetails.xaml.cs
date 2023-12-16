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
using KontrollDoc.Services.DependencyServices;

namespace KontrollDoc.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    /// <summary>
    /// Egy ContentPage amely Dokumentum adatainak való megjelenítéséért és frissítésért felelős
    /// </summary>
    public partial class DocDetails : ContentPage
    {
        /// <summary>
        /// Az adatbázis környezet.
        /// </summary>
        DB dbc;
        /// <summary>
        /// Kiválasztott dokumentum azonosítója.
        /// </summary>
        int selectedItemAzonosito;
        /// <summary>
        /// Csatolmány fájlainak.
        /// </summary>
        List<FileResult> csatolmanyok_list = new List<FileResult>();

        /// <summary>
        /// Csatolmányok osztálynak lista.
        /// </summary>
        List<DokHivatkozas> csatolmanyok_class_list = new List<DokHivatkozas>();
        /// <summary>
        /// A dokumentumok törzs adatai.
        /// </summary>
        KapcssDokTorzs doktorzs;
        /// <summary>
        /// A partnerek listája.
        /// </summary>
        List<Partner> partnerek;
        List<Partner> sortedPartnerek;

        private bool isProgrammaticChange = false;

        /// <summary>
        /// Inicializálja a <see cref="DocDetails"/> osztály új példányát a kiválasztott dokumentum meghatározott sorszámával.
        /// </summary>
        /// <param name="dbc">Az adatbázis környezet.</param>
        /// <param name="selectedItemAzonosito">A kiválasztott dokumentum sorszáma.</param>
        public DocDetails(DB dbc, int selectedItemAzonosito)
        {
            InitializeComponent();
            this.dbc = dbc;
            this.selectedItemAzonosito = selectedItemAzonosito;
            Partner partner = new Partner(dbc);
            this.partnerek = partner.GetPartnerLista();
        }
        /// <summary>
        /// Az oldal megjelenésekor hívják. Betölti az doktorzset és a kivaálsztott dokumentum adatait, csatolmányait.
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();

            // Doktorzs betöltése a pickerekbe.
            this.doktorzs = new KapcssDokTorzs(dbc);
            List<string> tipusok = this.doktorzs.GetDokTorzs("Tipus");
            List<string> temak = this.doktorzs.GetDokTorzs("Tema");
            List<string> hordozok = this.doktorzs.GetDokTorzs("Hordozo");
            List<string> hivatkozasok = this.doktorzs.GetDokTorzs("Projekt");

            Tipus_Picker.ItemsSource = tipusok;
            Tema_Picker.ItemsSource = temak;
            Hordozo_Picker.ItemsSource = hordozok;
            Project_Picker.ItemsSource = hivatkozasok;

            Partner_Nev_Picker.ItemsSource = partnerek;
            Partner_Nev_Picker.ItemDisplayBinding = new Binding("Nev");

            // Dokumentum adatainak lekérése.
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            SqlParameter sqlParameter = new SqlParameter();
            sqlParameter.ParameterName = "@DokAz";
            sqlParameter.Value = selectedItemAzonosito;
            sqlParameters.Add(sqlParameter);

            System.Data.DataTable dataTable = dbc.GetTableFromSPAB("DokumentumAdat", sqlParameters);
            System.Data.DataRow dr = dataTable.Rows[0];

            // A dokumentumnak megfelelő doktorzs kiválasztása.
            List<SqlParameter> torzs = new List<SqlParameter>();
            DataTable DokTorzs = dbc.GetTableFromSPAB("DokTorzsLista", torzs);

            for (int i = 0; i < DokTorzs.Rows.Count; i++)
            {
                if ((int)dr["TipusAz"] == (int)DokTorzs.Rows[i]["Azonosito"]) { Tipus_Picker.SelectedItem = (string)DokTorzs.Rows[i]["Megnevezes"]; }
                if ((int)dr["TemaAz"] == (int)DokTorzs.Rows[i]["Azonosito"]) { Tema_Picker.SelectedItem = (string)DokTorzs.Rows[i]["Megnevezes"]; }
                if ((int)dr["HordozoAz"] == (int)DokTorzs.Rows[i]["Azonosito"]) { Hordozo_Picker.SelectedItem = (string)DokTorzs.Rows[i]["Megnevezes"]; }
                if ((int)dr["ProjektHivatkozasAz"] == (int)DokTorzs.Rows[i]["Azonosito"]) { Project_Picker.SelectedItem = (string)DokTorzs.Rows[i]["Megnevezes"]; }
            }

            // Dokumentum további adatainak betöltése.
            Iktato_Entry.Text = (string)dr["Iktato"];

            if ((int)dr["Inaktiv"] == 0)
            {
                Hasznalt_CheckBox.IsChecked = false;
            }
            else
            {
                Hasznalt_CheckBox.IsChecked = true;
            }

            for (int i = 0; i < partnerek.Count; i++)
            {
                Partner selected = partnerek.Find(p => p.Azonosito == (int)dr["PartnerAz"]);
                if (selected != null)
                {
                    Partner_Kod_Entry.Text = selected.kod;
                    Partner_Nev_Picker.SelectedItem = selected;
                }
            }

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

            // Dokumentum csatolmányainak letöltése.
            List<SqlParameter> sqlParameters2 = new List<SqlParameter>();
            SqlParameter sqlParameter2 = new SqlParameter();
            sqlParameter2.ParameterName = "@DokAz";
            sqlParameter2.Value = this.selectedItemAzonosito;
            sqlParameters2.Add(sqlParameter2);

            DataTable dataTable2 = dbc.GetTableFromSPAB("DokHivatkozasAdat", sqlParameters2);

            foreach (DataRow dr2 in dataTable2.Rows)
            {
                DokHivatkozas csat = new DokHivatkozas();
                csat.Azonosito = (int)dr2["Azonosito"];
                csat.DokFilenev = dr2["DokFilenev"].ToString();
                csat.DokPath = dr2["DokPath"].ToString();
                csat.Inaktiv = (bool)dr2["Inaktiv"];
                csatolmanyok_class_list.Add(csat);

                FileResult file = new FileResult(csat.DokPath);
                file.FileName = csat.DokFilenev;
                this.csatolmanyok_list.Add(file);

            }
            csatolmanyok_ListView.ItemsSource = this.csatolmanyok_list;

            // Mellékletek betöltése.
            int sorszam = int.Parse(Sorszam_Entry.Text);
            Dokumentum getdocs = new Dokumentum();
            List<Dokumentum> docs = getdocs.GetDokumentumok(dbc);
            var results = docs.FindAll(dok => dok.FoDokumentum == sorszam);
            Mellékletek_ListView.ItemsSource = results;

            // Irattárak betöltése.
            Dokhelye dokhelye = new Dokhelye();
            List<Dokhelye> doks = dokhelye.GetDocHely(dbc);
            var geth = doks.Find(d => d.Dokaz == selectedItemAzonosito);

            Irattar1_Entry.Text = geth.Irattar1;
            Irattar2_Entry.Text = geth.Irattar2;
            Irattar3_Entry.Text = geth.Irattar3;
            Egyeb_Entry.Text = geth.Egyeb;

        }
        /// <summary>
        /// A Hozzáadás gomb eseménykezelője.
        /// </summary>
        async private void Csatolmany_Hozzaadasa_Clicked(object sender, EventArgs e)
        {
            try
            {
                // Filepickerrel csatolmányok bekérése a usertől.
                var csatolmany = await FilePicker.PickMultipleAsync();

                if (csatolmany != null)
                {
                    // át iterálás minden eegyes csatolmányon
                    foreach (var file in csatolmany)
                    {
                        // csatolmány feltöltésének beállítása
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

                        // file streamek átkonvertálása bájt tömbbe.
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

                        // csatolmány feltöltése.
                        var ujHivAz = dbc.ExecuteSPAB("KapcsDokHivUj", KapcsDokHivFrissitparams);

                        DokHivatkozas csat = new DokHivatkozas();
                        csat.Azonosito = (int)ujHivAz;
                        csatolmanyok_class_list.Add(csat);
                    }

                    // feltöltött csatolmány hozzáadása a már meglévő csatományokhoz
                    this.csatolmanyok_list.AddRange(csatolmany);
                    // feltöltött csatolmányok megjelenítése.
                    csatolmanyok_ListView.ItemsSource = null;
                    csatolmanyok_ListView.ItemsSource = this.csatolmanyok_list;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Csatolmány hozzáadása megszakadt", "OK");
            }
        }
        /// <summary>
        /// A Megnyitás gomb eseménykezelője.
        /// </summary>
        async private void csat_megnyitas_Clicked(object sender, EventArgs e)
        {
            // Fájl kiválasztása.
            var button = sender as Button;
            var item = button.BindingContext as FileResult;
            var index = csatolmanyok_ListView.TabIndex;
            var hiv = this.csatolmanyok_class_list[index];

            // Kiválasztott fájl letöltéséhez beállítások.
            List<SqlParameter> DokHivatkozasGetHivparams = new List<SqlParameter>();
            SqlParameter HivAz = new SqlParameter();
            HivAz.ParameterName = "@HivAz";
            HivAz.Value = hiv.Azonosito;
            DokHivatkozasGetHivparams.Add(HivAz);
            // Kiválasztott fájl letöltése.
            DataTable dt = dbc.GetTableFromSPAB("DokHivatkozasGetHiv", DokHivatkozasGetHivparams);
            DataRow dr = dt.Rows[0];

            // letöltött fájl beállítása
            DokHivatkozas download = new DokHivatkozas();
            download.Azonosito = (int)dr["Azonosito"];
            download.DokFilenev = dr["DokFilenev"].ToString();
            download.DokPath = dr["DokPath"].ToString();
            download.Inaktiv = (bool)dr["Inaktiv"];
            download.ImageData = (byte[])dr["ImageData"];

            try
            {
                // Letöltött fájl ideiglenes lementése és megjelenítése.
                string filePath = await SaveFileToDownloadsFolder(download.DokFilenev, download.ImageData);

                await Launcher.OpenAsync(new OpenFileRequest
                {
                    File = new ReadOnlyFile(filePath)
                });

            }
            catch (Exception ex)
            {
                await DisplayAlert("Hiba", "csatollmány megnyitása megszakadt", "OK");
            }

        }

        /// <summary>
        /// Aszinkron módon menti a fájlt az alkalmazás gyorsítótárának Letöltések mappájába.
        /// </summary>
        /// <param name="fileName">A mentendő fájl neve.</param>
        /// <param name="fileBytes">A fájl tartalmát reprezentáló bájttömb.</param>
        /// <returns>A mentett fájl teljes elérési útja.</returns>
        public async Task<string> SaveFileToDownloadsFolder(string fileName, byte[] fileBytes)
        {
            // Letöltések mappa elérési útja.
            var downloadsPath = Path.Combine(FileSystem.CacheDirectory, "Downloads");

            // Ha nincs letöltések csinálunk.
            if (!Directory.Exists(downloadsPath))
            {
                Directory.CreateDirectory(downloadsPath);
            }

            // útvonal összerakása.
            var filePath = Path.Combine(downloadsPath, fileName);

            // Fájl lementése.
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await fileStream.WriteAsync(fileBytes, 0, fileBytes.Length);
            }

            // útvonal vissza adása.
            return filePath;
        }
        /// <summary>
        /// Nyomtatás gomb esemény kezelője.
        /// </summary>
        async private void csat_nyomtatas_Clicked(object sender, EventArgs e)
        {
            // Fájl kiválasztása.
            var button = sender as Button;
            var item = button.BindingContext as FileResult;
            var index = csatolmanyok_ListView.TabIndex;
            var hiv = this.csatolmanyok_class_list[index];

            // Kiválasztott fájl letöltéséhez beállítások.
            List<SqlParameter> DokHivatkozasGetHivparams = new List<SqlParameter>();
            SqlParameter HivAz = new SqlParameter();
            HivAz.ParameterName = "@HivAz";
            HivAz.Value = hiv.Azonosito;
            DokHivatkozasGetHivparams.Add(HivAz);

            // Kiválasztott fájl letöltése.
            DataTable dt = dbc.GetTableFromSPAB("DokHivatkozasGetHiv", DokHivatkozasGetHivparams);
            DataRow dr = dt.Rows[0];

            // letöltött fájl beállítása
            DokHivatkozas download = new DokHivatkozas();
            download.Azonosito = (int)dr["Azonosito"];
            download.DokFilenev = dr["DokFilenev"].ToString();
            download.DokPath = dr["DokPath"].ToString();
            download.Inaktiv = (bool)dr["Inaktiv"];
            download.ImageData = (byte[])dr["ImageData"];

            // fájl formátum beállítása
            var fileFormat = Path.GetExtension(download.DokFilenev).ToLower();
            var compatibleFormats = new[] { ".png", ".jpg", ".jpeg", ".pdf" };

            if (compatibleFormats.Contains(fileFormat))
            {
                // Ha UWP-n fut
                if (Device.RuntimePlatform == Device.UWP)
                {
                    // UWP nyomtatási dependecy service futtatása.
                    DependencyService.Get<IUWPPrintService>().PrintByteArrayAsync(download.ImageData, fileFormat);
                }
                if (Device.RuntimePlatform == Device.Android) 
                {
                    // Android nyomtatási dependecy service futtatása.
                    DependencyService.Get<IAndroidPrintService>().PrintByteArrayAsync(download.ImageData, fileFormat);
                }
            }
            else
            {
                await DisplayAlert("Error", "Nyomtatás csak .pdf .png .jpg .jpeg kiterjesztésű fájlokra alkalmazható", "Ok");
            }
        }

        /// <summary>
        /// A Törlés gomb eseménykezelője.
        /// </summary>
        private void csat_torles_Clicked(object sender, EventArgs e)
        {
            // Fájl kiválasztása.
            var button = sender as Button;
            var item = button.BindingContext as FileResult;
            var index = csatolmanyok_ListView.TabIndex;
            var hiv = this.csatolmanyok_class_list[index];

            // Kiválasztott fájl frissítéséhez beállítások.
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

            // Kiválasztott fájl frissítése.
            dbc.ExecuteSPAB("KapcsDokHivFrissit", KapcsDokHivFrissitparams);

            // Kiválasztott fájl törléséhez beállítások.
            List<SqlParameter> DokHivTorolparams = new List<SqlParameter>();
            SqlParameter HivAz = new SqlParameter();
            HivAz.ParameterName = "@HivAz";
            HivAz.Value = hiv.Azonosito;
            DokHivTorolparams.Add(HivAz);

            // Kiválasztott fájl törlése beállítások.
            dbc.ExecuteSPAB("DokHivatkozasTorol", DokHivTorolparams);

            // csatolmány kivétele a csatolmányok listából.
            this.csatolmanyok_class_list.Remove(hiv);
            this.csatolmanyok_list.Remove(item);
            csatolmanyok_ListView.ItemsSource = null;
            csatolmanyok_ListView.ItemsSource = this.csatolmanyok_list;
        }

        /// <summary>
        /// Kezeli a Hatarido_CheckBox CheckedChanged eseményét.
        /// </summary>
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
        /// A Frissít gomb eseménykezelője.
        /// </summary>
        private async void Frissit_Clicked(object sender, EventArgs e)
        {
            try
            {
                // Frissített dokumentum adatainak begyűjtése.
                Dokumentum modosito_dokumentum = new Dokumentum();
                modosito_dokumentum.Azonosito = this.selectedItemAzonosito;
                modosito_dokumentum.FoDokumentum = null;
                var Tipus = this.doktorzs.dokTorzs.Find(d => d.Megnevezes == Tipus_Picker.SelectedItem.ToString());
                modosito_dokumentum.TipusAz = Tipus.Azonosito;
                modosito_dokumentum.Iktato = Iktato_Entry.Text;
                var Partner = this.partnerek.Find(d => d.kod == Partner_Kod_Entry.Text);
                //modosito_dokumentum.PartnerAz = int.Parse( Partner_Kod_Entry.Text);
                modosito_dokumentum.PartnerAz = Partner.Azonosito;
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

                // Frissített dokumentum adatainak beállítása feltöltésere.
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

                // dokumentum frissítése.
                dbc.ExecuteSPAB("DokumentumModosit", DokumentumModositparams);


                await Navigation.PopAsync();
            }
            catch (Exception ex) {
                await DisplayAlert("Hiba", "Frissítés sikertelen", "ok");
            }
        }

        /// <summary>
        /// A Tipus gomb eseménykezelője.
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
        /// <summary>
        /// A Melléklet Hozzáadás gomb eseménykezelője.
        /// </summary>
        private async void Hozzaadas_Clicked(object sender, EventArgs e)
        {
            // navigálás DokNew lapra a dokumentum sorszámával.
            await Navigation.PushAsync(new Views.DocNew(dbc, int.Parse(Sorszam_Entry.Text)));
        }
        /// <summary>
        /// A Áthelyez gomb eseménykezelője.
        /// </summary>
        async private void Hely_Button_Clicked(object sender, EventArgs e)
        {
            // Dokumentum áthelyezésének beállítása
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

            // Dokumentum áthelyezése
            dbc.ExecuteSPAB("DokHelyeModosit", DokHelyeModositparams);
            
            // User informálása
            await DisplayAlert("Siker","Helyezze át a dokumnetumot a megfelelő Irattárba!","Ok");
        }
    }
}