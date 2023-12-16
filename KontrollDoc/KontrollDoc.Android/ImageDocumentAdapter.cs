using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Print.Pdf;
using Android.Print;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Android.Print.PrintDocumentAdapter;

namespace KontrollDoc.Droid
{
    /// <summary>
    /// Egy adapter osztály képek nyomtatásához.
    /// </summary>
    public class ImageDocumentAdapter : PrintDocumentAdapter
    {
        Context context;
        byte[] imageData;

        /// <summary>
        /// Inicializálja az ImageDocumentAdapter osztály új példányát.
        /// </summary>
        /// <param name="context">Az alkalmazás környezete.</param>
        /// <param name="imageData">A képadatokat reprezentáló bájttömb.</param>
        public ImageDocumentAdapter(Context context, byte[] imageData)
        {
            this.context = context;
            this.imageData = imageData;
        }
        /// <summary>
        /// Új nyomtatási munka indításakor hívják
        /// </summary>
        /// <param name="oldAttributes">A régi nyomtatási attribútumok.</param>
        /// <param name="newAttributes">Az új nyomtatási attribútumok.</param>
        /// <param name="cancellationSignal">Lemondási jelzés.</param>
        /// <param name="callback">Visszahívás az elrendezés eredményéhez.</param>
        /// <param name="extras">További paraméterek.</param>
        public override void OnLayout(PrintAttributes oldAttributes, PrintAttributes newAttributes, CancellationSignal cancellationSignal, LayoutResultCallback callback, Bundle extras)
        {
            // Új PdfDocument az új attribútumokkal.
            var pdfDocument = new PrintedPdfDocument(context, newAttributes);

            // Válaszoljon az eredményre, és adja át a létrehozott PdfDocumentot
            callback.OnLayoutFinished(new PrintDocumentInfo.Builder("KontrollDoc").SetContentType(PrintContentType.Document).SetPageCount(1).Build(), true);
            pdfDocument.Close();
        }
        /// <summary>
        /// A nyomtatási attribútumok változásakor hívják.
        /// </summary>
        /// <param name="ranges">A megírandó oldalak.</param>
        /// <param name="destination">A célfájl leírója.</param>
        /// <param name="cancellationSignal">Lemondási jelzés.</param>
        /// <param name="callback">Visszahívás az eredmény írása érdekében.</param>
        public override void OnWrite(PageRange[] ranges, ParcelFileDescriptor destination, CancellationSignal cancellationSignal, WriteResultCallback callback)
        {
            var printAttributes = new PrintAttributes.Builder()
                .SetMediaSize(PrintAttributes.MediaSize.IsoA4)
                .SetMinMargins(new PrintAttributes.Margins(0, 0, 0, 0))
                .Build();

            var pdfDocument = new PrintedPdfDocument(context, printAttributes);

            // Első lap kezsésa
            var page = pdfDocument.StartPage(0);

            // Bitmap dekarálása és a lap vászonra rajzoláshoz
            var bitmap = BitmapFactory.DecodeByteArray(imageData, 0, imageData.Length);

            // Vászon mérete.
            float scaleWidth = (float)(((float)page.Canvas.Width) / bitmap.Width * 1);
            float scaleHeight = (float)(((float)page.Canvas.Height) / bitmap.Height * 1);

            // Mátrix mérete.
            var matrix = new Matrix();

            // bitmap méretere vágása
            matrix.PostScale(scaleWidth, scaleHeight, bitmap.Width / 2, bitmap.Height / 2);
            bitmap = Bitmap.CreateBitmap(bitmap, 0, 0, bitmap.Width, bitmap.Height, matrix, true);

            // Bitmap rárajzolása a vászonra.
            page.Canvas.DrawBitmap(bitmap, 0, 0, null);

            // Finish the page
            pdfDocument.FinishPage(page);

            // PDF dokumentumot a cél ParcelFileDescriptorba írása.
            try
            {
                using (var fileOutputStream = new FileOutputStream(destination.FileDescriptor))
                using (var outputStream = new OutputStreamInvoker(fileOutputStream))
                {
                    pdfDocument.WriteTo(outputStream);
                }
            }
            catch (Exception e)
            {
                callback.OnWriteFailed(e.Message);
                return;
            }
            finally
            {
                pdfDocument.Close();
            }

            // Jelzi hogy az összes lap kész.
            callback.OnWriteFinished(new PageRange[] { new PageRange(0, 0) });
        }
    }
}