using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KontrollDoc.Droid;
using Xamarin.Forms;
using KontrollDoc.Services.DependencyServices;
using Android.Print;
using System.Threading.Tasks;
using Xamarin.Essentials;
using System.IO;

// Dependency Service regisztrálása az assemblynek.
[assembly: Dependency(typeof(AndroidPrintService))]
namespace KontrollDoc.Droid
{
    /// <summary>
    /// Nyomtatási funkciókat implementáló osztály.
    /// </summary>
    internal class AndroidPrintService : IAndroidPrintService
    {
        /// <summary>
        /// Aszinkron módon nyomtatja ki a felhasználó által kiválasztott fájlt.
        /// </summary>
        /// <returns>Egy feladat, amely az aszinkron műveletet reprezentálja.</returns>
        public async Task PrintAsync()
        {
            try
            {
                // Fájl picker létrehozása és beállítása
                var customFileType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DevicePlatform.Android, new[] { "image/jpeg", "image/png", "application/pdf" } },
                });

                var options = new PickOptions
                {
                    PickerTitle = "Please select a file",
                    FileTypes = customFileType,
                };

                var result = await FilePicker.PickAsync(options);
                if (result != null)
                {
                    // Kiválasztott fájl átkonvertálása bájt tömbbé
                    byte[] fileData = null;
                    using (var stream = await result.OpenReadAsync())
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            stream.CopyTo(memoryStream);
                            fileData = memoryStream.ToArray();
                        }
                    }

                    // Hivatkozás a PrintManager rendszerszolgáltatásra.
                    var printManager = (PrintManager)MainActivity.Instance.GetSystemService(Context.PrintService);

                    // Osztály példány létrehozása, amely a PrintDocumentAdaptertől örököl.
                    PrintDocumentAdapter printAdapter;
                    if (result.FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
                    {
                        // Pdf adapter megadása.
                        printAdapter = new PdfDocumentAdapter(MainActivity.Instance, fileData);
                    }
                    else
                    {
                        // Image adapter megadása.
                        printAdapter = new ImageDocumentAdapter(MainActivity.Instance, fileData);
                    }

                    // PrintManager Print metódusának a meghívása, ezzel a átadáva a nyomtatóadapternek.
                    var printJob = printManager.Print("MyPrintJob", printAdapter, null);
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Aszinkron módon nyomtat egy képet vagy egy bájttömbként ábrázolt PDF-dokumentumot.
        /// </summary>
        /// <param name="imageData">A nyomtatandó képet vagy PDF-dokumentumot képviselő bájttömb.</param>
        /// <param name="fileformat">A fájl formátuma. Az érvényes formátumok: „.jpg”, „.png” és „.pdf”.</param>
        /// <returns>Egy feladat, amely az aszinkron műveletet reprezentálja.</returns>
        public async Task PrintByteArrayAsync(byte[] imageData, string fileformat)
        {
            try
            {
                if (fileformat == ".pdf")
                {
                    // Hivatkozás a PrintManager rendszerszolgáltatásra.
                    var printManager = (PrintManager)MainActivity.Instance.GetSystemService(Context.PrintService);

                    // Pdf adapter megadása.
                    var printAdapter = new PdfDocumentAdapter(MainActivity.Instance, imageData);

                    // Call the PrintManager's Print method, passing it the print adapter
                    var printJob = printManager.Print("MyPrintJob", printAdapter, null);
                }
                else
                {
                    // Hivatkozás a PrintManager rendszerszolgáltatásra.
                    var printManager = (PrintManager)MainActivity.Instance.GetSystemService(Context.PrintService);

                    // Image adapter megadása.
                    var printAdapter = new ImageDocumentAdapter(Android.App.Application.Context, imageData);

                    // PrintManager Print metódusának a meghívása, ezzel a átadáva a nyomtatóadapternek.
                    var printJob = printManager.Print("MyPrintJob", printAdapter, null);
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.Message);
            }
        }
    }
}