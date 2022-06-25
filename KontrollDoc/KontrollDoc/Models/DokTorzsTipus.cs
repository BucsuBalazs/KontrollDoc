using System;
using System.Collections.Generic;
using System.Text;

namespace KontrollDoc.Models
{
    internal class DokTorzsTipus
    {
        private int _Azonosito;         // 1
        private string _Megnevezes;     // 2
        private string _TipusNev;       // 3
        private bool _Inaktiv;          // 4

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
    }
}
