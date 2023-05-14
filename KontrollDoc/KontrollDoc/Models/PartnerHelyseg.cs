using FIT_Common;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace KontrollDoc.Models
{
    internal class PartnerHelyseg
    {

        public PartnerHelyseg() { }


        public int Azonosito { get; set; }

        public string Megnevezes { get; set; }

        public List<PartnerHelyseg>  GetHelysegek(DB dbc) 
        {
            List<SqlParameter> empty = new List<SqlParameter>();

            List<PartnerHelyseg> Helysegek = new List<PartnerHelyseg>();

            System.Data.DataTable dataTable = dbc.GetTableFromSPAB("HelysegLista", empty);
            foreach (System.Data.DataRow dr in dataTable.Rows)
            {
                PartnerHelyseg Helyseg = new PartnerHelyseg();
                Helyseg.Azonosito = (int)dr["Azonosito"];
                Helyseg.Megnevezes = (string)dr["Megnevezes"];

                Helysegek.Add(Helyseg);
            }

            return Helysegek;

        }
    }
}
