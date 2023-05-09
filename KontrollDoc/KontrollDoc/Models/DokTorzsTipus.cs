using FIT_Common;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace KontrollDoc.Models
{
    public class DokTorzsTipus
    {
        DB dbc;
        private int _Azonosito;         // 1
        private string _Megnevezes;     // 2
        private string _TipusNev;       // 3
        private bool _Inaktiv;          // 4


        public DokTorzsTipus() { }
        public DokTorzsTipus(DB dbc) 
        {
            this.dbc = dbc;
        }

        public DokTorzsTipus(
            int azon,
            string megnevezes,
            string tipusaz,
            bool inaktiv)
        {
            this._Azonosito = azon;
            this._Megnevezes = megnevezes;
            this._TipusNev = tipusaz;
            this._Inaktiv = inaktiv;
        }

        public int Azonosito { get; set; }
        public string Megnevezes { get; set; }
        public string TipusNev { get; set; }
        public bool Inaktiv { get; set; }

        public List<DokTorzsTipus> GetDokTorzsTipusTable()
        {

            List<SqlParameter> empty = new List<SqlParameter>();

            List<DokTorzsTipus> dokTorzs = new List<DokTorzsTipus>();

            System.Data.DataTable dataTable = dbc.GetTableFromSPAB("DokTorzsTipusLista", empty);
            foreach (System.Data.DataRow dr in dataTable.Rows)
            {
                DokTorzsTipus dok = new DokTorzsTipus();
                dok.Azonosito = (int)dr["Azonosito"];
                dok.Megnevezes = (string)dr["Megnevezes"];
                dok.TipusNev = (string)dr["TipusNev"];
                dok.Inaktiv = (bool)dr["Inaktiv"];

                dokTorzs.Add(dok);
            }

            return dokTorzs;
        }
    }
}
