using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Printing;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Printing;

namespace KontrollDoc.UWP
{
    /// <summary>
    /// Osztály, amely lehetővé teszi a dokumentumok UWP alkalmazásban történő nyomtatását.
    /// </summary>
    public class Print_UWP : IDisposable
    {
        // Az aktuális nézet PrintManagerje.
        PrintManager printmgr = PrintManager.GetForCurrentView();
        // A nyomtatandó dokumentum
        PrintDocument printDoc;
        // A nyomtatási feladatot reprezentáló PrintTask
        PrintTask printTask;
        // A thread-safe collection that contains the views to be printed
        ConcurrentBag<Windows.UI.Xaml.Controls.Image> ViewsToPrint = new ConcurrentBag<Windows.UI.Xaml.Controls.Image>();
        /// <summary>
        /// Inicializálja a Print_UWP osztály új példányát, és feliratkozik a PrintTaskRequested eseményre.
        /// </summary>
        public Print_UWP()
        {
            printmgr.PrintTaskRequested += Printmgr_PrintTaskRequested;
        }
        /// <summary>
        /// Leiratkozik a PrintTaskRequested eseményről, és felszabadítja a Print_UWP által használt összes erőforrást.
        /// </summary>
        public void Dispose()
        {
            printmgr.PrintTaskRequested -= Printmgr_PrintTaskRequested;
        }
        /// <summary>
        /// Aszinkron módon nyomtatja ki a megadott oldalakat.
        /// </summary>
        /// <param name="pagesData">A nyomtatandó oldalak adatai.</param>
        /// <returns>Egy feladat, amely az aszinkron műveletet reprezentálja.</returns>
        public async Task PrintUWpAsync(List<byte[]> pagesData)
        {
            // Végigiterál az oldalakon.
            foreach (var pageData in pagesData)
            {
                // Új BitmapImage.
                BitmapImage biSource = new BitmapImage();

                // Új InMemoryRandomAccessStream-et, és írja bele az oldaladatokat.
                using (InMemoryRandomAccessStream stream = new InMemoryRandomAccessStream())
                {
                    await stream.WriteAsync(pageData.AsBuffer());
                    stream.Seek(0);
                    await biSource.SetSourceAsync(stream);
                }
                // Új kép lértehozása, forrása a BitmapImage, és adja hozzá a nyomtatandó nézeteket listához.
                Windows.UI.Xaml.Controls.Image viewToPrint = new Windows.UI.Xaml.Controls.Image();
                viewToPrint.Source = biSource;
                ViewsToPrint.Add(viewToPrint);
            }
            // Ha már létezik PrintDocument, leiratkozás az eseményeiről.
            if (printDoc != null)
            {
                printDoc.GetPreviewPage -= PrintDoc_GetPreviewPage;
                printDoc.Paginate -= PrintDoc_Paginate;
                printDoc.AddPages -= PrintDoc_AddPages;
            }
            // Új PrintDocument, és iratkozzon fel az eseményeire.
            this.printDoc = new PrintDocument();
            printDoc.GetPreviewPage += PrintDoc_GetPreviewPage;
            printDoc.Paginate += PrintDoc_Paginate;
            printDoc.AddPages += PrintDoc_AddPages;

            try
            {
                // Dokumentum megnyitása a Nyomtatási API-ban.
                bool showprint = await PrintManager.ShowPrintUIAsync();
            }
            catch (Exception e)
            {
                // Kiírja a kivétel üzenetet a Debug kimenetbe.
                Debug.WriteLine($"Exception in PrintUWpAsync: {e}");
            }
        }
        /// <summary>
        /// Kezeli a PrintTaskRequested eseményt.
        /// </summary>
        /// <param name="sender">Az esemény forrása.</param>
        /// <param name="args">Az esemény</param>
        private void Printmgr_PrintTaskRequested(PrintManager sender, PrintTaskRequestedEventArgs args)
        {
            // késleltetés.
            var deff = args.Request.GetDeferral();
            // Egy új nyomtatási feladat létrehozása.
            printTask = args.Request.CreatePrintTask("KontrollDoc", OnPrintTaskSourceRequested);
            // késleltetés vége.
            deff.Complete();
        }
        /// <summary>
        /// Kezeli a PrintTaskSourceRequested eseményt.
        /// </summary>
        /// <param name="args">Az esemény adatai.</param>
        async void OnPrintTaskSourceRequested(PrintTaskSourceRequestedArgs args)
        {
            // késleltetés.
            var def = args.GetDeferral();
            // Felhasználói felület szálra.
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
            {
                // Ha a PrintDocument és a DocumentSource értéke nem null
                if (printDoc != null && printDoc.DocumentSource != null)
                {
                    // Nyomtatási feladat forrásának a beállítása.
                    args.SetSource(printDoc.DocumentSource);
                }
                else
                {
                    // Messagedialoggal küzlés hogy a dokumentum null
                    var dialog = new Windows.UI.Popups.MessageDialog("Üres a dokumentum.");
                    await dialog.ShowAsync();
                }
            });
            // késleltetés vége.
            def.Complete();
        }
        /// <summary>
        /// Kezeli az AddPages eseményt.
        /// </summary>
        /// <param name="sender">Az esemény forrása.</param>
        /// <param name="e">Az esemény adatai.</param>
        private void PrintDoc_AddPages(object sender, AddPagesEventArgs e)
        {
            // Hozzá adja az oldalakat a nyomtatáshoz.
            foreach (var viewToPrint in ViewsToPrint)
            {
                printDoc.AddPage(viewToPrint);
            }
            // Jelezi, hogy az összes oldal hozzá lett adva
            printDoc.AddPagesComplete();
        }

        /// <summary>
        /// Handles the Paginate event.
        /// </summary>
        /// <param name="sender">Az esemény forrása.</param>
        /// <param name="e">Az esemény adatai.</param>
        private void PrintDoc_Paginate(object sender, PaginateEventArgs e)
        {
            // Nyomtatási feladat beállításai.
            PrintTaskOptions opt = printTask.Options;
            // PrintDocument előnézeti oldalszámának beállítása
            printDoc.SetPreviewPageCount(ViewsToPrint.Count, PreviewPageCountType.Final);

        }

        /// <summary>
        /// Kezeli a GetPreviewPage eseményt.
        /// </summary>
        /// <param name="sender">Az esemény forrása.</param>
        /// <param name="e">Az esemény adatai.</param>
        private void PrintDoc_GetPreviewPage(object sender, GetPreviewPageEventArgs e)
        {
            // Inicializálja az oldalszámot.
            int pageNumber = 1;
            // Végig itárlá az összes lapon.
            foreach (var viewToPrint in ViewsToPrint)
            {
                // Ha az oldalszám megegyezik a kért oldalszámmal.
                if (pageNumber == e.PageNumber)
                {
                    // PrintDocument előnézeti oldalánka a beállítása.
                    printDoc.SetPreviewPage(e.PageNumber, viewToPrint);
                    break;
                }
                // Oldalszám növelése.
                pageNumber++;
            }
        }
    }
}
