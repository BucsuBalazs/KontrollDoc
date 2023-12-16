using FIT_Common;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace KontrollDoc.Models
{
    /// <summary>
    /// Segéd osztály a partner helyseg és azon típusaival való munkálatokhoz
    /// </summary>
    internal class PartnerHelyseg
    {
        /// <summary>
        /// Inicializálja a <see cref="PartnerHelyseg"/> osztály új példányát.
        /// </summary>

        public PartnerHelyseg() { }

        /// <summary>
        /// Azonosito getter, setter
        /// </summary>
        public int Azonosito { get; set; }

        /// <summary>
        /// Megnevezes getter, setter
        /// </summary>
        public string Megnevezes { get; set; }
        /// <summary>
        /// Lekéri a partnerhelyek listáját az adatbázisból.
        /// </summary>
        /// <param name="dbc">Az adatbázis környezet.</param>
        /// <returns>A partnerhelyek listája.</returns>
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
