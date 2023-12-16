using System;
using System.Collections.Generic;
using System.Text;

namespace KontrollDoc.Models
{
    /// <summary>
    /// Dolgozo táblát reprezentáló osztály
    /// </summary>
    internal class Dolgozo
    {
        /// <summary>
        /// Inicializálja a <see cref="Dolgozo"/> osztály új példányát.
        /// </summary>
        public Dolgozo() { }
        /// <summary>
        /// Azonosito getter, setter
        /// </summary>
        public int Azonosito { get; set; }
        /// <summary>
        /// Nev getter, setter
        /// </summary>
        public string Nev { get; set; }
        /// <summary>
        /// Usernev getter, setter
        /// </summary>
        public string Usernev { get; set; }

    }
}
