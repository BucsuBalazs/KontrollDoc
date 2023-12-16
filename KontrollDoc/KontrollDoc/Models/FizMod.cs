using FIT_Common;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace KontrollDoc.Models
{
    /// <summary>
    /// FizMod táblát reprezentáló osztály
    /// </summary>
    internal class FizMod
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
        /// Inicializálja a <see cref="FizMod"/> osztály új példányát.
        /// </summary>
        public FizMod() { }

        /// <summary>
        /// Lekéri a FizMod példányok listáját az adatbázisból.
        /// </summary>
        /// <param name="dbc">Az adatbázis környezet</param>
        /// <returns>A FizMod példányok listája.</returns>
        public List<FizMod> GetFizMod(DB dbc)
        {
            List<SqlParameter> empty = new List<SqlParameter>();

            List<FizMod> Fizmods = new List<FizMod>();

            System.Data.DataTable dataTable = dbc.GetTableFromSPAB("GetFizModList", empty);
            foreach (System.Data.DataRow dr in dataTable.Rows)
            {
                FizMod fizmod = new FizMod();
                fizmod.Azonosito = (int)dr["Azonosito"];
                fizmod.Megnevezes = (string)dr["Megnevezes"];

                Fizmods.Add(fizmod);
            }

            return Fizmods;
        }
    }
}
