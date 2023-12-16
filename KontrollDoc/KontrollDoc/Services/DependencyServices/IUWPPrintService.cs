using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace KontrollDoc.Services.DependencyServices
{
    /// <summary>
    /// Nyomtatási módszereket biztosít egy UWP-alkalmazásban.
    /// </summary>
    public interface IUWPPrintService
    {
        /// <summary>
        /// Aszinkron nyomtatási feladatot indít.
        /// </summary>
        /// <returns>Egy feladat, amely az aszinkron nyomtatási műveletet képviseli.</returns>
        Task PrintAsync();
        /// <summary>
        /// Aszinkron módon nyomtat egy fájlt egy bájttömbből.
        /// </summary>
        /// <param name="fileData">A fájladatokat tartalmazó bájttömb</param>
        /// <param name="fileformat">A nyomtatandó fájl formátuma.</param>
        /// <returns>Egy feladat, amely az aszinkron nyomtatási műveletet képviseli.</returns>
        Task PrintByteArrayAsync(byte[] fileData, string fileformat);
    }
}
