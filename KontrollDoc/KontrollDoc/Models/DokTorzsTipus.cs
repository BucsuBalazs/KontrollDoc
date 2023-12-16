using FIT_Common;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace KontrollDoc.Models
{
    /// <summary>
    /// DokTorzsTipus táblát reprezentáló osztály
    /// </summary>
    public class DokTorzsTipus
    {
        /// <summary>
        /// Adatbázis kontextus.
        /// </summary>
        DB dbc;
        private int _Azonosito;
        private string _Megnevezes;
        private string _TipusNev;
        private bool _Inaktiv;
        /// <summary>
        /// Inicializálja a <see cref="DokTorzsTipus"/> osztály új példányát.
        /// </summary>
        public DokTorzsTipus() { }
        /// <summary>
        /// Inicializálja a <see cref="DokTorzsTipus"/> osztály új példányát megadott adatbázis kontextussal.
        /// </summary>
        /// <param name="dbc">Az adatbázis környezet.</param>
        public DokTorzsTipus(DB dbc) 
        {
            this.dbc = dbc;
        }

        /// <summary>
        /// Inicializálja a <see cref="DokTorzsTipus"/> osztály új példányát megadott értékekkel.
        /// </summary>
        /// <param name="azon">Az azonosító.</param>
        /// <param name="megnevezes">A Megnevezés</param>
        /// <param name="tipusaz">A típus azonosító</param>
        /// <param name="inaktiv">Az inaktív állapot.</param>
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
        /// <summary>
        /// Azonosito getter, setter
        /// </summary>
        public int Azonosito { get; set; }
        /// <summary>
        /// Megnevezes getter, setter
        /// </summary>
        public string Megnevezes { get; set; }
        /// <summary>
        /// TipusNev getter, setter
        /// </summary>
        public string TipusNev { get; set; }
        /// <summary>
        /// Inaktiv getter, setter
        /// </summary>
        public bool Inaktiv { get; set; }

        /// <summary>
        /// Lekéri a DokTorzsTipus példányok listáját az adatbázisból.
        /// </summary>
        /// <returns>A DokTorzsTipus példányok listája.</returns>
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
