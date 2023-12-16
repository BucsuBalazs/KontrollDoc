using FIT_Common;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace KontrollDoc.Models
{
    /// <summary>
    /// ArTip táblát reprezentáló osztály
    /// </summary>
    internal class ArTip
    {
        /// <summary>
        /// Azonosito getter, setter
        /// </summary>
        public int Azonosito { get; set; }
        /// <summary>
        /// Megnevezes getter, setter
        /// </summary>
        public string Megnevezes { get; set; }

        /// <summary>
        /// Inicializálja az <see cref="ArTip"/> osztály új példányát.
        /// </summary>
        public ArTip() { }

        /// <summary>
        /// Lekéri az ArTip példányok listáját az adatbázisból.
        /// </summary>
        /// <param name="dbc">Az adatbázis contextus</param>
        /// <returns>Az ArTip példányok listája.</returns>
        public List<ArTip> GetArTip(DB dbc)
        {
            List<SqlParameter> empty = new List<SqlParameter>();

            List<ArTip> ArTips = new List<ArTip>();

            System.Data.DataTable dataTable = dbc.GetTableFromSPAB("GetArtipList", empty);
            foreach (System.Data.DataRow dr in dataTable.Rows)
            {
                ArTip arTip = new ArTip();
                arTip.Azonosito = (int)dr["Azonosito"];
                arTip.Megnevezes = (string)dr["Megnevezes"];

                ArTips.Add(arTip);
            }

            return ArTips;
        }
    }
}
