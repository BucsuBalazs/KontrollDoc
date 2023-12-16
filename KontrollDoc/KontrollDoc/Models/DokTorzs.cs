using FIT_Common;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Text;
using System.Linq;

namespace KontrollDoc.Models
{
    /// <summary>
    /// DokHivatkozas táblát reprezentáló osztály
    /// </summary>
    public class DokTorzs
    {
        /// <summary>
        /// Adatbázis kontextus.
        /// </summary>
        DB dbc;
        /// <summary>
        /// Azonosito getter, setter
        /// </summary>
        public int Azonosito { get; set; }
        /// <summary>
        /// Megnevezes getter, setter
        /// </summary>
        public string Megnevezes { get; set; }
        /// <summary>
        /// TipusAz getter, setter
        /// </summary>
        public int TipusAz { get; set; }
        /// <summary>
        /// Inaktiv getter, setter
        /// </summary>
        public bool Inaktiv { get; set; }

        /// <summary>
        /// Inicializálja a <see cref="DokTorzs"/> osztály új példányát meghatározott adatbázis-környezettel.
        /// </summary>
        /// <param name="dbc">Az adatbázis környezet.</param>
        public DokTorzs(DB dbc) 
        {
            this.dbc = dbc;
        }
        /// <summary>
        /// Inicializálja a <see cref="DokTorzs"/> osztály új példányát.
        /// </summary>
        public DokTorzs()
        {
        }
        /// <summary>
        /// Lekéri a dokumentumnevek listáját az adatbázisból.
        /// </summary>
        /// <param name="megnevezes">A dokumentum neve.</param>
        /// <returns>Dokumentumnevek listája.</returns>
        public List<string> GetDokTorzs(string megnevezes)
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
        /// <summary>
        /// Lekéri a DokTorzs példányok listáját az adatbázisból.
        /// </summary>
        /// <returns>A DokTorzs példányok listája.</returns>
        public List<DokTorzs> GetDokTorzsTable() {

            List<SqlParameter> empty = new List<SqlParameter>();

            List<DokTorzs> dokTorzs = new List<DokTorzs>();

            System.Data.DataTable dataTable = dbc.GetTableFromSPAB("DokTorzsLista", empty);
            foreach (System.Data.DataRow dr in dataTable.Rows)
            {
                DokTorzs dok = new DokTorzs();
                dok.Azonosito = (int)dr["Azonosito"];
                dok.Megnevezes = (string)dr["Megnevezes"];
                dok.TipusAz = (int)dr["TipusAz"];
                dok.Inaktiv = (bool)dr["Inaktiv"];

                dokTorzs.Add(dok);
            }

            return dokTorzs;
        }
    }
}
