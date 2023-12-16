using System;
using System.Collections.Generic;
using System.Text;

namespace KontrollDoc.Models
{
    /// <summary>
    /// CégKontroll adatbázist reprezentáló osztály
    /// </summary>
    internal class Adatbazis
    {
        /// <summary>
        /// ID getter, setter
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nev getter, setter
        /// </summary>
        public string Nev { get; set; }

        /// <summary>
        /// CegId getter, setter
        /// </summary>
        public int CegId { get; set; }

        /// <summary>
        /// Inicializálja az <see cref="Adatbazis"/> osztály új példányát.
        /// </summary>
        public Adatbazis() { }
    }
}
