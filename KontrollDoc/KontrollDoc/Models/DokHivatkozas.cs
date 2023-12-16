using System;
using System.Collections.Generic;
using System.Text;

namespace KontrollDoc.Models
{
    /// <summary>
    /// DokHivatkozas táblát reprezentáló osztály
    /// </summary>
    internal class DokHivatkozas
    {
        private int _Azonosito;
        private string _DokFilenev;
        private string _DokPath;
        private bool _Inaktiv;
        private byte[] _ImageData;

        /// <summary>
        /// Inicializálja a <see cref="DokHivatkozas"/> osztály új példányát.
        /// </summary>
        public DokHivatkozas() { }

        /// <summary>
        /// Inicializálja a <see cref="DokHivatkozas"/> osztály új példányát megadott értékekkel.
        /// </summary>
        /// <param name="azon">Az azonosító.</param>
        /// <param name="dokfilenev">A dokumentum fájlneve.</param>
        /// <param name="dokpath">A dokumentum elérési útja.</param>
        /// <param name="inaktiv">Az inaktív állapot.</param>
        /// <param name="imagedata">A képadatok.</param>
        public DokHivatkozas(
            int azon,
            string dokfilenev,
            string dokpath,
            bool inaktiv,
            byte[] imagedata

            )
        {
            this._Azonosito = azon;
            this._DokFilenev = dokfilenev;
            this._DokPath = dokpath;
            this._Inaktiv = inaktiv;
            this._ImageData = imagedata;
        }

        /// <summary>
        /// Azonosito getter, setter
        /// </summary>
        public int Azonosito { get; set; }
        /// <summary>
        /// DokFilenev getter, setter
        /// </summary>
        public string DokFilenev { get; set; }
        /// <summary>
        /// DokPath getter, setter
        /// </summary>
        public string DokPath { get; set; }
        /// <summary>
        /// Inaktiv getter, setter
        /// </summary>
        public bool Inaktiv { get; set; }
        /// <summary>
        /// ImageData getter, setter
        /// </summary>
        public byte[] ImageData { get; set; }
    }
}
