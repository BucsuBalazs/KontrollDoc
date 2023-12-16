using KontrollDoc.UWP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using KontrollDoc.Services.DependencyServices;
using Windows.Devices.Enumeration;
using Windows.Devices.Scanners;
using System.IO;

// Dependency Service regisztrálása az assemblynek.
[assembly: Dependency(typeof(UWPScanService))]

namespace KontrollDoc.UWP
{
    /// <summary>
    /// Szkennelési funkciókat implementáló osztály.
    /// </summary>
    internal class UWPScanService : IUWPScanService
    {
        /// <summary>
        /// Aszinkron módon lekéri az összes elérhető szkenner nevét.
        /// </summary>
        /// <returns>Egy feladat, amely az aszinkron műveletet reprezentálja. A feladat eredménye a szkennernevek listáját tartalmazza.</returns>
        public async Task<List<string>> GetScannersAsync()
        {
            // Elérhető szkennerek lekérése
            DeviceInformationCollection deviceInfoCollection = await DeviceInformation.FindAllAsync(ImageScanner.GetDeviceSelector());

            // Elérhető szkennerek neveinek a kilistázása
            List<string> scannerNames = deviceInfoCollection.Select(deviceInfo => deviceInfo.Name).ToList();

            return scannerNames;
        }
        /// <summary>
        /// Aszinkron módon szkennel egy dokumentumot a megadott szkenner és formátum használatával.
        /// </summary>
        /// <param name="scannerName">A szkenner neve.</param>
        /// <param name="formatString">A beolvasott dokumentum formátuma. Az érvényes formátumok: „JPG”, „PNG” és „TIFF”.</param>
        /// <returns>Egy feladat, amely az aszinkron műveletet reprezentálja. A feladat eredménye FileResultként tartalmazza a beolvasott dokumentumot.</returns>
        public async Task<Xamarin.Essentials.FileResult> ScanAsync(string scannerName, string formatString)
        {
            ImageScannerFormat format;
            // Kiválasztott formátum beállítása
            switch (formatString.ToUpper())
            {
                case "JPG":
                case "JPEG":
                    format = ImageScannerFormat.Jpeg;
                    break;
                case "PNG":
                    format = ImageScannerFormat.Png;
                    break;
                case "TIFF":
                    format = ImageScannerFormat.Tiff;
                    break;
                default:
                    throw new ArgumentException("Invalid format. Valid formats are 'JPG', 'PNG', and 'TIFF'.");
            }

            // Elérhető szkennerek lekérése
            var deviceInfoCollection = await DeviceInformation.FindAllAsync(ImageScanner.GetDeviceSelector());

            // Megfelelő szkenner kiválasztása
            var deviceInfo = deviceInfoCollection.FirstOrDefault(di => di.Name == scannerName);

            if (deviceInfo != null)
            {
                var scanner = await ImageScanner.FromIdAsync(deviceInfo.Id);

                // Configuráció beállítása
                var scanSource = scanner.DefaultScanSource;
                var scanResolution = new ImageScannerResolution { DpiX = 300, DpiY = 300 };

                if (scanSource == ImageScannerScanSource.Flatbed)
                {
                    var flatbedConfiguration = scanner.FlatbedConfiguration;
                    flatbedConfiguration.Format = format;
                    flatbedConfiguration.DesiredResolution = scanResolution;
                }
                else if (scanSource == ImageScannerScanSource.Feeder)
                {
                    var feederConfiguration = scanner.FeederConfiguration;
                    feederConfiguration.Format = format;
                    feederConfiguration.DesiredResolution = scanResolution;
                }

                // Új mappa választó
                var folderPicker = new Windows.Storage.Pickers.FolderPicker
                {
                    SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary
                };
                // mappa választó filterezése
                folderPicker.FileTypeFilter.Add(".jpg");
                folderPicker.FileTypeFilter.Add(".png");
                folderPicker.FileTypeFilter.Add(".tiff");

                // Picker megjelenítése és kiválasztott mappa mentése
                Windows.Storage.StorageFolder folder = await folderPicker.PickSingleFolderAsync();

                if (folder != null)
                {
                    // Szkennelés indítása
                    var scanResult = await scanner.ScanFilesToFolderAsync(scanSource, folder);

                    // Ha a szkennelés sikeres volt return FileResult.
                    if (scanResult.ScannedFiles.Count > 0)
                    {
                        var file = scanResult.ScannedFiles[0];
                        return new Xamarin.Essentials.FileResult(file.Path);
                    }
                }
                else
                {
                    var dialog = new Windows.UI.Popups.MessageDialog("No folder selected");
                    await dialog.ShowAsync();
                }
            }

            // Ha a szkennelés nem volt sikeres return null.
            return null;
        }
    }
}
