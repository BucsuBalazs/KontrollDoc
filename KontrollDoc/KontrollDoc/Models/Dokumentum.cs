using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using FIT_Common;

namespace KontrollDoc.Models
{
    internal  class Dokumentum
    {
        // dbo.Dokumentum
        private int _Azonosito;         // 1
        private int? _FoDokumentum;     // 2
        private int _TipusAz;           // 3
        private int _Sorszam;           // 4
        private string _Iktato;            // 5
        private int _PartnerAz;         // 6
        private int _TemaAz;            // 7
        private string _Targy;          // 8
        private int _HordozoAz;         // 9
        private int _UgyintezoAz;       // 10
        private DateTime _Datum;        // 11
        private DateTime? _Hatarido;     // 12
        private int _FontossagAz;       // 13
        private int _AllapotAz;         // 14
        private int _ProjecktHivatkozasAz;// 15
        private string _Megjegyzes;     // 16
        private string _Telefonszam;    // 17
        private int? _CRU;               // 18
        private DateTime? _CRD;          // 19
        private int? _LMU;               // 20
        private DateTime? _LMD;          // 21
        private bool? _Bizalamas;         // 22
        private bool? _Inaktiv;           // 23

        public Dokumentum() { }

        public Dokumentum(
            int azon,
            int? fodokumentum,
            int tipus,
            int sorszam,
            string iktatoSzam,
            int partnerAz,
            int temaAz,
            string targy,
            int hordozo,
            int ugyintezo,
            DateTime datum,
            DateTime? hatarido,
            int fontossag,
            int allapot,
            int projecktHivatkozasAz,
            string megjegyzes,
            string Telefonszam,
            int? cru,
            DateTime? crd,
            int? lmu,
            DateTime? lmd,
            bool? bizalams,
            bool? inaktiv) 
        {
            this._Azonosito = azon;
            this._CRU = cru;
            this._CRD = crd;
            this._LMU = lmu;
            this._LMD = lmd;
            this._Bizalamas = bizalams;
            this._Iktato = iktatoSzam;
            this._PartnerAz = partnerAz;
            this._TipusAz = tipus;
            this._Sorszam = sorszam;
            this._FoDokumentum = fodokumentum;
            this._Targy = targy;
            this._Telefonszam = Telefonszam;
            this._HordozoAz = hordozo;
            this._TemaAz = temaAz;
            this._UgyintezoAz = ugyintezo;
            this._AllapotAz = allapot;
            this._FontossagAz = fontossag;
            this._Megjegyzes = megjegyzes;
            this._ProjecktHivatkozasAz = projecktHivatkozasAz;
            this._Datum = datum;
            this._Hatarido = hatarido;
            this._Inaktiv = inaktiv;
        }

        ~Dokumentum() { Console.WriteLine("Dokumentum torolve"); }



        public int Azonosito { get => _Azonosito; set => _Azonosito = value; }
        public int? FoDokumentum { get => _FoDokumentum; set => _FoDokumentum = value; }
        public int TipusAz { get => _TipusAz; set => _TipusAz = value; }
        public int Sorszam { get => _Sorszam; set => _Sorszam = value; }
        public string Iktato { get => _Iktato; set => _Iktato = value; }
        public int PartnerAz { get => _PartnerAz; set => _PartnerAz = value; }
        public int TemaAz { get => _TemaAz; set => _TemaAz = value; }
        public string Targy { get => _Targy; set => _Targy = value; }
        public int HordozoAz { get => _HordozoAz; set => _HordozoAz = value; }
        public int UgyintezoAz { get => _UgyintezoAz; set => _UgyintezoAz = value; }
        public DateTime Datum { get => _Datum; set => _Datum = value; }
        public DateTime? Hatarido { get => _Hatarido; set => _Hatarido = value; }
        public int FontossagAz { get => _FontossagAz; set => _FontossagAz = value; }
        public int AllapotAz { get => _AllapotAz; set => _AllapotAz = value; }
        public int ProjecktHivatkozasAz { get => _ProjecktHivatkozasAz; set => _ProjecktHivatkozasAz = value; }
        public string Megjegyzes { get => _Megjegyzes; set => _Megjegyzes = value; }

        private string _telefonszam = null;

        public string Telefonszam
        {
            get { return _telefonszam; }
            set { _telefonszam = value; }
        }
        public int? CRU { get => _CRU; set => _CRU = value; }
        public DateTime? CRD { get => _CRD; set => _CRD = value; }
        public int? LMU { get => _LMU; set => _LMU = value; }
        public DateTime? LMD { get => _LMD; set => _LMD = value; }
        public bool? Bizalmas { get => _Bizalamas; set => _Bizalamas = value; }
        public bool? Inaktiv { get => _Inaktiv; set => _Inaktiv = value; }

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
