using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms.PlatformConfiguration;

namespace KontrollDoc.Services.DependencyServices
{
    /// <summary>
    /// UWP-n való szkenneléshez interface osztály
    /// </summary>
    public interface IUWPScanService
    {
        // metódus interface az eléhető szkennerek kiválasztásához
        Task<Xamarin.Essentials.FileResult> ScanAsync(string scannerName, string formatString);
        // metódus interface a szkennerelés megkezdéséhez
        Task<List<string>> GetScannersAsync();
    }
}
