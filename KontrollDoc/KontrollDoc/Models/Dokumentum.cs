using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using FIT_Common;

namespace KontrollDoc.Models
{
    /// <summary>
    /// Dokumentum táblát reprezentáló osztály
    /// </summary>
    internal class Dokumentum
    {
        /// <summary>
        /// Inicializálja a <see cref="Dokumentum"/> osztály új példányát.
        /// </summary>
        public Dokumentum() { }

        /// <summary>
        /// Azonosito getter, setter
        /// </summary>
        public int Azonosito { get; set; }
        /// <summary>
        /// FoDokumentum getter, setter
        /// </summary>
        public int? FoDokumentum { get; set; }
        /// <summary>
        /// TipusAz getter, setter
        /// </summary>
        public int TipusAz { get; set; }
        /// <summary>
        /// Sorszam getter, setter
        /// </summary>
        public int Sorszam { get; set; }
        /// <summary>
        /// Iktato getter, setter
        /// </summary>
        public string Iktato { get; set; }
        /// <summary>
        /// PartnerAz getter, setter
        /// </summary>
        public int PartnerAz { get; set; }
        /// <summary>
        /// TemaAz getter, setter
        /// </summary>
        public int TemaAz { get; set; }
        /// <summary>
        /// Targy getter, setter
        /// </summary>
        public string Targy { get; set; }
        /// <summary>
        /// HordozoAz getter, setter
        /// </summary>
        public int HordozoAz { get; set; }
        /// <summary>
        /// UgyintezoAz getter, setter
        /// </summary>
        public int UgyintezoAz { get; set; }
        /// <summary>
        /// Datum getter, setter
        /// </summary>
        public DateTime Datum { get; set; }
        /// <summary>
        /// Hatarido getter, setter
        /// </summary>
        public DateTime? Hatarido { get; set; }
        /// <summary>
        /// FontossagAz getter, setter
        /// </summary>
        public int FontossagAz { get; set; }
        /// <summary>
        /// AllapotAz getter, setter
        /// </summary>
        public int AllapotAz { get; set; }
        /// <summary>
        /// ProjecktHivatkozasAz getter, setter
        /// </summary>
        public int ProjecktHivatkozasAz { get; set; }
        /// <summary>
        /// Megjegyzes getter, setter
        /// </summary>
        public string Megjegyzes { get; set; }
        /// <summary>
        /// Telefonszam getter, setter
        /// </summary>
        public string Telefonszam { get; set; }
        /// <summary>
        /// CRU getter, setter
        /// </summary>
        public int? CRU { get; set; }
        /// <summary>
        /// CRD getter, setter
        /// </summary>
        public DateTime? CRD { get; set; }
        /// <summary>
        /// LMU getter, setter
        /// </summary>
        public int? LMU { get; set; }
        /// <summary>
        /// LMD getter, setter
        /// </summary>
        public DateTime? LMD { get; set; }
        /// <summary>
        /// Bizalmas getter, setter
        /// </summary>
        public bool? Bizalmas { get; set; }
        /// <summary>
        /// Inaktiv getter, setter
        /// </summary>
        public bool? Inaktiv { get; set; }

        /// <summary>
        /// Lekéri a Dokumentum példányok listáját az adatbázisból.
        /// </summary>
        /// <param name="dbc">Az adatbázis környezet.</param>
        /// <returns>Dokumentum példányok listája.</returns>
        public List<Dokumentum> GetDokumentumok(DB dbc)
        {
            List<System.Data.SqlClient.SqlParameter> empty = new List<System.Data.SqlClient.SqlParameter>();
            var dt = dbc.GetTableFromSPAB("DokumentumLista", empty);

            List<Dokumentum> dokumentumok = new List<Dokumentum>();

            foreach (System.Data.DataRow row in dt.Rows)
            {
                Dokumentum dokumentum = new Dokumentum();
                dokumentum.Azonosito = (int)row["Azonosito"];
                if (row["FoDokumentum"] != DBNull.Value) { dokumentum.FoDokumentum = (int)row["FoDokumentum"]; } else { dokumentum.FoDokumentum = null; }
                dokumentum.TipusAz = (int)row["TipusAz"];
                dokumentum.Sorszam = (int)row["Sorszam"];
                if (row["Iktato"] != DBNull.Value) { dokumentum.Iktato = (string)row["Iktato"]; } else { dokumentum.Iktato = null; }
                dokumentum.PartnerAz = (int)row["PartnerAz"];
                dokumentum.TemaAz = (int)row["TemaAz"];
                dokumentum.Targy = (string)row["Targy"];
                dokumentum.HordozoAz = (int)row["HordozoAz"];
                dokumentum.UgyintezoAz = (int)row["UgyintezoAz"];
                dokumentum.Datum = (DateTime)row["Datum"];
                if (row["Hatarido"] != DBNull.Value) { dokumentum.Hatarido = (DateTime)row["Hatarido"]; } else { dokumentum.Hatarido = null; }
                dokumentum.FontossagAz = (int)row["FontossagAz"];
                dokumentum.AllapotAz = (int)row["AllapotAz"];
                dokumentum.ProjecktHivatkozasAz = (int)row["ProjektHivatkozasAz"];
                dokumentum.Megjegyzes = (string)row["Megjegyzes"];
                dokumentum.Telefonszam = row["Telefonszam"] as string ?? null;
                if (row["CRU"] != DBNull.Value) { dokumentum.CRU = (int)row["CRU"]; } else { dokumentum.CRU = null; }
                if (row["CRD"] != DBNull.Value) { dokumentum.CRD = (DateTime)row["CRD"]; } else { dokumentum.CRD = null; }
                if (row["LMU"] != DBNull.Value) { dokumentum.LMU = (int)row["LMU"]; } else { dokumentum.LMU = null; }
                if (row["LMD"] != DBNull.Value) { dokumentum.LMD = (DateTime)row["LMD"]; } else { dokumentum.LMD = null; }
                if (row["Bizalmas"] != DBNull.Value) { dokumentum.Bizalmas = (bool)row["Bizalmas"]; } else { dokumentum.Bizalmas = null; }
                if (row["Inaktiv"] != DBNull.Value) { dokumentum.Inaktiv = (bool)row["Inaktiv"]; } else { dokumentum.Inaktiv = null; }
                dokumentumok.Add(dokumentum);
            }
            return dokumentumok;
        }

    }
}
