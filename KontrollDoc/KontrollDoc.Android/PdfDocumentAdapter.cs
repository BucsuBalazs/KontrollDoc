using Android.App;
using Android.Content;
using Android.OS;
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
    public class PdfDocumentAdapter : PrintDocumentAdapter
    {
        private readonly Context _context;
        private readonly byte[] _pdfBytes;

        /// <summary>
        /// Inicializálja az PdfDocumentAdapter osztály új példányát.
        /// </summary>
        /// <param name="context">Az alkalmazás környezete.</param>
        /// <param name="imageData">A képadatokat reprezentáló bájttömb.</param>
        public PdfDocumentAdapter(Context context, byte[] pdfBytes)
        {
            _context = context;
            _pdfBytes = pdfBytes;
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
            if (cancellationSignal.IsCanceled)
            {
                // jelzi hogy a nyomtatási munka megszakadt.
                callback.OnLayoutCancelled();
            }
            else
            {
                // Új PrintDocumentInfo objektum
                PrintDocumentInfo info = new PrintDocumentInfo.Builder("KontrollDoc").SetContentType(PrintContentType.Document).Build();
                // Jelzi hogy kész a layout.
                callback.OnLayoutFinished(info, true);
            }
        }
        /// <summary>
        /// A nyomtatási attribútumok változásakor hívják.
        /// </summary>
        /// <param name="pages">A megírandó oldalak.</param>
        /// <param name="destination">A célfájl leírója.</param>
        /// <param name="cancellationSignal">Lemondási jelzés.</param>
        /// <param name="callback">Visszahívás az eredmény írása érdekében.</param>
        public override void OnWrite(PageRange[] pages, ParcelFileDescriptor destination, CancellationSignal cancellationSignal, WriteResultCallback callback)
        {
            InputStream input = null;
            OutputStream output = null;

            try
            {
                // input stream.
                input = new ByteArrayInputStream(_pdfBytes);
                // output stream.
                output = new FileOutputStream(destination.FileDescriptor);

                // Buffer adatok olvasához és írásához

                byte[] buf = new byte[16384];
                int size;

                // Át írjuk az adatokat a destination.FileDescriptorba.
                while ((size = input.Read(buf)) >= 0 && !cancellationSignal.IsCanceled)
                {
                    output.Write(buf, 0, size);
                }


                if (cancellationSignal.IsCanceled)
                {
                    // ha megszakadt az adatok 
                    callback.OnWriteCancelled();
                }
                else
                {
                    // Jelzi a rendszernek hogy az összes lap kész.
                    callback.OnWriteFinished(new PageRange[] { PageRange.AllPages });
                }
            }
            catch (Exception e)
            {
                callback.OnWriteFailed(e.Message);
            }
            finally
            {
                try
                {
                    input.Close();
                    output.Close();
                }
                catch (IOException e)
                {
                    System.Console.WriteLine(e.Message);
                }
            }
        }
    }
}