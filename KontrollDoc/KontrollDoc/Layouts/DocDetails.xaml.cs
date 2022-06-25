using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;
using System.IO;

using System.Data.SqlClient;
using CAVO3;

namespace KontrollDoc.Layouts
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DocDetails : ContentPage
    {
        public DocDetails()
        {
            InitializeComponent();
        }

        public static DB dbc = KontrollDoc.MainPage.dbc;
        public int selectedItemAzonosito = KontrollDoc.Layouts.DocList.selectedItemAzonosito;

        protected override void OnAppearing()
        {
            base.OnAppearing();

            List<string> tipusok = GetDokTorzs("Tipus");
            List<string> temak = GetDokTorzs("Tema");
            List<string> hordozok = GetDokTorzs("Hordozo");
            List<string> hivatkozasok = GetDokTorzs("Projekt");

            List<string> partnerkodok = GetPartner("kod");
            List<string> partnernevek = GetPartner("nev");

            Tipus_Picker.ItemsSource = tipusok;
            Tema_Picker.ItemsSource = temak;
            Hordozo_Picker.ItemsSource = hordozok;
            Project_Picker.ItemsSource = hivatkozasok;
            Partner_Kod_Picker.ItemsSource = partnerkodok;
            Partner_Nev_Picker.ItemsSource = partnernevek;



            if (selectedItemAzonosito != 0)
            {
                Cim.Text = "Dokumentum módosítása";

                List<SqlParameter> sqlParameters = new List<SqlParameter>();
                SqlParameter sqlParameter = new SqlParameter();
                sqlParameter.ParameterName = "@DokAz";
                sqlParameter.Value = selectedItemAzonosito;
                sqlParameters.Add(sqlParameter);

                System.Data.DataTable dataTable = dbc.GetTableFromSPAB("DokumentumAdat", sqlParameters);
                System.Data.DataRow dr = dataTable.Rows[0];

                List<SqlParameter> torzs = new List<SqlParameter>();
                System.Data.DataTable DokTorzs = dbc.GetTableFromSPAB("DokTorzsLista", torzs);

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

                List<SqlParameter> vmi = new List<SqlParameter>();
                System.Data.DataTable PartnerLista = dbc.GetTableFromSPAB("PartnerLista", vmi);

                for (int i = 0; i < PartnerLista.Rows.Count; i++)
                {
                    if ((int)dr["PartnerAz"] == (int)PartnerLista.Rows[i]["Azonosito"]) 
                    { 
                        Partner_Kod_Picker.SelectedItem = (string)PartnerLista.Rows[i]["kod"]; 
                        Partner_Nev_Picker.SelectedItem = (string)PartnerLista.Rows[i]["Nev"]; 
                    }
                }

                Sorszam_Entry.Text = dr["Sorszam"].ToString();

                Targy_Entry.Text = dr["Targy"].ToString();

                Telefon_Entry.Text = dr["Telefonszam"].ToString();

                var Read_Datum = DateTime.Parse(dr["Datum"].ToString());

                Felvetel_DatePicker.Date = Read_Datum;

                if (dr["Hatarido"]!= DBNull.Value) 
                { 
                    Hatarido_Checkbox.IsChecked = true;
                    Hatarido_DatePicker.IsEnabled = true;
                    var Read_Hatar = DateTime.Parse(dr["Hatarido"].ToString());
                    Hatarido_DatePicker.Date = Read_Hatar;
                }

                Megjegyzes_Entry.Text = dr["Megjegyzes"].ToString();

            }
        }
        List<string> GetPartner(string i) 
        {
            List<string> partnerkodok = new List<string>();
            List<string> partnernevek = new List<string>();

            List<SqlParameter> vmi = new List<SqlParameter>();
            System.Data.DataTable Partnerek = dbc.GetTableFromSPAB("PartnerLista", vmi);

            foreach (System.Data.DataRow dr in Partnerek.Rows)
            {
                string str1 = (string)dr["kod"];
                string str2 = (string)dr["Nev"];
                partnerkodok.Add(str1);
                partnernevek.Add(str2);
            }
            if (i == "kod") { return partnerkodok; } else if(i=="nev"){ return partnernevek; } else { return null; }

        }

        List<string> GetDokTorzs(string megnevezes)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            SqlParameter sqlParameter = new SqlParameter();
            sqlParameter.ParameterName = "@Tipus";
            sqlParameter.Value = megnevezes;
            sqlParameters.Add(sqlParameter);

            List<string> Tipus_Picker_List = new List<string>();

            System.Data.DataTable dataTable = dbc.GetTableFromSPAB("GetDokTorzs", sqlParameters);
            foreach (System.Data.DataRow dr in dataTable.Rows)
            {
                string str = (string)dr["Megnevezes"];
                Tipus_Picker_List.Add(str);
            }
            return Tipus_Picker_List;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        private void Save_Clicked(object sender, EventArgs e)
        {
            KontrollDoc.Layouts.DocList.selectedItemAzonosito = 0;

            this.Navigation.PopAsync();
        }

        private void Delete_Clicked(object sender, EventArgs e)
        {

        }

        async private void Csatolmany_hozzaadasa(object sender, EventArgs e)
        {
            var pickResult = await FilePicker.PickMultipleAsync(new PickOptions 
            { 
                PickerTitle = "Mellékletek kiválszatása"
            });

            if (pickResult != null) 
            { 
                var fileList = new List<FileBase>();

                foreach (FileResult file in pickResult) 
                {
                    //var stream = await file.OpenReadAsync();
                    //fileList.Add(file.FileName);
                }

                collectionView.ItemsSource = fileList;
            }
        }

        private void Hatarido_CheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if(Hatarido_Checkbox.IsChecked == true)
            {
                Hatarido_DatePicker.IsEnabled = true;
            } 
            else 
            { 
                Hatarido_DatePicker.IsEnabled=false; 
            }
        }

        private void Partner_Kod_Picker_SelectedIndexChanged(object sender, EventArgs e)
        {
            Partner_Nev_Picker.SelectedIndex = Partner_Kod_Picker.SelectedIndex;
        }

        private void Partner_Nev_Picker_SelectedIndexChanged(object sender, EventArgs e)
        {
            Partner_Kod_Picker.SelectedIndex = Partner_Nev_Picker.SelectedIndex;
        }
    }
}