using FIT_Common;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace KontrollDoc.Models
{
    /// <summary>
    /// Dokhelye táblát reprezentáló osztály
    /// </summary>
    internal class Dokhelye
    {
        /// <summary>
        /// Inicializálja az <see cref="Dokhelye"/> osztály új példányát.
        /// </summary>
        public Dokhelye() { }
        /// <summary>
        /// Azonosito getter, setter
        /// </summary>
        public int Azonosito { get; set; }
        /// <summary>
        /// Dokaz getter, setter
        /// </summary>
        public int Dokaz { get; set;}

        /// <summary>
        /// Lekéri vagy beállítja az első dokumentumtárat.
        /// </summary>
        public string Irattar1 { get; set; }
        /// <summary>
        /// Lekéri vagy beállítja az második dokumentumtárat.
        /// </summary>
        public string Irattar2 { get; set; }
        /// <summary>
        /// Lekéri vagy beállítja az harmadik dokumentumtárat.
        /// </summary>
        public string Irattar3 { get; set; }
        /// <summary>
        /// Lekéri vagy beállítja a továbbított azonosítót.
        /// </summary>
        public int? TovabbitvaAz { get; set;}
        /// <summary>
        /// Lekéri vagy beállítja a kiegészítő információkat.
        /// </summary>
        public string Egyeb { get; set; }
        /// <summary>
        /// Lekéri vagy beállítja a dokhelye sor készítőjének az azonosítóját.
        /// </summary>
        public int CRU { get; set; }
        /// <summary>
        /// Lekéri vagy beállítja a dokhelye keletkezésének a dátumát.
        /// </summary>
        public DateTime CRD { get; set; }
        /// <summary>
        /// Lekéri vagy beállítja a dokhelye sor utolsó frissítőjének az azonosítóját.
        /// </summary>
        public int LMU { get; set; }
        /// <summary>
        /// Lekéri vagy beállítja a dokhelye sor utolsó frissítőjének a dátumát.
        /// </summary>
        public DateTime LMD { get; set; }

        /// <summary>
        /// Lekéri az Dokhelye példányok listáját az adatbázisból.
        /// </summary>
        /// <param name="dbc">Az adatbázis contextus</param>
        /// <returns>Az Dokhelye példányok listája.</returns>
        public List<Dokhelye> GetDocHely(DB dbc)
        {
            List<SqlParameter> empty = new List<SqlParameter>();

            List<Dokhelye> doks = new List<Dokhelye>();

            System.Data.DataTable dataTable = dbc.GetTableFromSPAB("DokIrattarList", empty);
            foreach (System.Data.DataRow dr in dataTable.Rows)
            {
                Dokhelye dokhelye = new Dokhelye();
                dokhelye.Azonosito = (int)dr["Azonosito"];
                dokhelye.Dokaz = (int)dr["DokAZ"];
                dokhelye.Irattar1 = dr["Irattar1"] as string ?? null;
                dokhelye.Irattar2 = dr["Irattar2"] as string ?? null;
                dokhelye.Irattar3 = dr["Irattar3"] as string ?? null;
                dokhelye.TovabbitvaAz = dr["TovabbitvaAz"] is DBNull ? null : (int?)dr["TovabbitvaAz"];
                dokhelye.Egyeb = dr["Egyeb"] as string ?? null;
                dokhelye.CRU = (int)dr["CRU"];
                dokhelye.CRD = (DateTime)dr["CRD"];
                dokhelye.LMU = (int)dr["LMU"];
                dokhelye.LMD = (DateTime)dr["LMD"];

                doks.Add(dokhelye);
            }

            return doks;
        }
    }
}
