using KontrollDoc.UWP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KontrollDoc.Services.DependencyServices;
using Xamarin.Forms;
using Windows.Data.Pdf;
using Windows.Storage.Streams;
using Windows.Storage;


// Dependency Service regisztrálása az assemblynek.
[assembly: Dependency(typeof(UWPPrintService))]
namespace KontrollDoc.UWP
{
    /// <summary>
    /// Nyomtatási funkciókat implementáló osztály.
    /// </summary>
    internal class UWPPrintService : IUWPPrintService
    {
        /// <summary>
        /// Aszinkron módon nyomtatja ki a felhasználó által kiválasztott fájlt.
        /// </summary>
        /// <returns>Egy feladat, amely az aszinkron műveletet reprezentálja</returns>
        public async Task PrintAsync()
        {
            try
            {
                // Fájl picker de
                var picker = new Windows.Storage.Pickers.FileOpenPicker();
                // Fájl picker beállítása.
                picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
                picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
                // Szűrők beállítása a fájl pickerhez.
                picker.FileTypeFilter.Add(".jpg");
                picker.FileTypeFilter.Add(".jpeg");
                picker.FileTypeFilter.Add(".png");
                picker.FileTypeFilter.Add(".pdf");

                // fájl picker megnyitása
                var file = await picker.PickSingleFileAsync();

                if (file != null)
                {
                    byte[] fileData = null;
                    // Kiválasztott fájl megnyitása
                    using (var stream = await file.OpenReadAsync())
                    {
                        // Új DataReader-t az adatfolyamból
                        using (var dataReader = new Windows.Storage.Streams.DataReader(stream))
                        {
                            // Új bájttömböt az adatfolyam méretéből.
                            var bytes = new byte[stream.Size];
                            // Adatfolyamot betöltése a DataReaderbe.
                            await dataReader.LoadAsync((uint)stream.Size);
                            // DataReader bájtjainak beolvaása bájttömbbe.
                            dataReader.ReadBytes(bytes);
                            // Rendelje hozzá a bájttömböt a fileData-hoz
                            fileData = bytes;
                        }
                    }

                    if (fileData != null)
                    {

                        List<byte[]> pagesData = null;
                        // Ha a fájl típusa PDF
                        if (file.FileType.ToLower() == ".pdf")
                        {
                            // Konvertálja át a PDF-fájlt képpé
                            var pdfStream = await ConvertPdfToImage(file);
                            // Konvertálja a képfolyamot bájttömbbé
                            pagesData = await ConvertStreamToByteArray(pdfStream);
                        }
                        else
                        {
                            // Az input byte tömb lapokra osztása
                            pagesData = new List<byte[]> { fileData };

                        }
                        // Nyomtató osztály Inicializálása.
                        using (Print_UWP print_UWP = new Print_UWP())
                        {
                            // Fájl nyomtatása.
                            await print_UWP.PrintUWpAsync(pagesData);
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Aszinkron módon nyomtat egy bájttömböt.
        /// </summary>
        /// <param name="fileData">A nyomtatandó fájladatokat képviselő bájttömb.</param>
        /// <param name="fileformat">A fájl formátuma.</param>
        /// <returns>Egy feladat, amely az aszinkron műveletet reprezentálja.</returns>
        public async Task PrintByteArrayAsync(byte[] fileData, string fileformat)
        {
            List<byte[]> pagesData = null;
            // Ha pdf fájl
            if (fileformat.ToLower() == ".pdf")
            {
                // Írja be a bájttömböt egy ideiglenes fájlba
                var tempFolder = Windows.Storage.ApplicationData.Current.TemporaryFolder;
                var tempFile = await tempFolder.CreateFileAsync("temp.pdf", Windows.Storage.CreationCollisionOption.ReplaceExisting);
                await Windows.Storage.FileIO.WriteBytesAsync(tempFile, fileData);

                // Konvertálja át a PDF-fájlt képpé
                var pdfStream = await ConvertPdfToImage(tempFile);
                // Konvertálja a képfolyamot bájttömbbé
                pagesData = await ConvertStreamToByteArray(pdfStream);

                // Törölje az ideiglenes fájlt
                await tempFile.DeleteAsync();
            }
            else
            {
                // Az input byte tömb lapokra osztása
                pagesData = new List<byte[]> { fileData };
            }
            // Nyomtató osztály Inicializálása.
            using (Print_UWP print_UWP = new Print_UWP())
            {
                // Fájl nyomtatása.
                await print_UWP.PrintUWpAsync(pagesData);
            }
        }

        /// <summary>
        /// Aszinkron módon konvertálja a PDF-fájlt képfolyamok listájára.
        /// </summary>
        /// <param name="pdfFile">A konvertálandó PDF fájl.</param>
        /// <returns>Egy feladat, amely az aszinkron műveletet reprezentálja. A feladat eredménye a képfolyamok listáját tartalmazza.</returns>
        public async Task<List<IRandomAccessStream>> ConvertPdfToImage(StorageFile pdfFile)
        {
            // PDF dokumentum betöltése
            var pdfDocument = await PdfDocument.LoadFromFileAsync(pdfFile);

            // Lista létre hozása az egyes lapok adatfolyamainak tárolására
            var streams = new List<IRandomAccessStream>();

            // Lapozzon végig a dokumentum minden oldalán.
            for (uint i = 0; i < pdfDocument.PageCount; i++)
            {
                // Jelenlegi lap kiválasztása.
                var pdfPage = pdfDocument.GetPage(i);

                // Új stream.
                var stream = new InMemoryRandomAccessStream();

                // Létre jön egy PdfPageRenderOptions objektumot a kimeneti kép méretének megadásához
                var renderOptions = new PdfPageRenderOptions();
                renderOptions.DestinationHeight = (uint)pdfPage.Size.Height;
                renderOptions.DestinationWidth = (uint)pdfPage.Size.Width;

                // Az lap renderelése a streambe.
                await pdfPage.RenderToStreamAsync(stream, renderOptions);

                // hozzáadás a lapok listához.
                streams.Add(stream);
            }
            // Vissza adjuk a streameket.
            return streams;
        }

        /// <summary>
        /// A folyamok listáját aszinkron módon bájttömbök listájává konvertálja.
        /// </summary>
        /// <param name="streams">A konvertálandó adatfolyamok listája.</param>
        /// <returns>Egy feladat, amely az aszinkron műveletet reprezentálja. A feladat eredménye bájttömbök listáját tartalmazza.</returns>
        public async Task<List<byte[]>> ConvertStreamToByteArray(List<IRandomAccessStream> streams)
        {
            var byteArrays = new List<byte[]>();

            // Iteráció a lista minden egyes adatfolyamán keresztül
            foreach (var stream in streams)
            {
                // új DataReader az adatfolyamból
                var reader = new DataReader(stream.GetInputStreamAt(0));

                // új bájttömböt az adatfolyam méretével
                var bytes = new byte[stream.Size];
                // Adatfolyam betöltse a DataReaderbe
                await reader.LoadAsync((uint)stream.Size);
                // DataReader bájtjai beolvasása a bájttömbbe
                reader.ReadBytes(bytes);
                // Adja hozzá a bájttömböt a bájttömbök listájához
                byteArrays.Add(bytes);
            }

            // Visszaadja a bájttömbök listáját
            return byteArrays;
        }
    }
}
