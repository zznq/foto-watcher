using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;

namespace foto_watcher {
    class FotoPrinter {
        private static FotoPrinter _printer;
        private String _path;

        private PrintDocument printDoc;

        private FotoPrinter() {
            printDoc = new PrintDocument();

            const string printerName = "HP Photosmart Plus B210 series";

            printDoc.PrinterSettings.PrinterName = printerName;
            if (!printDoc.PrinterSettings.IsValid) {
                string msg = String.Format(
                   "Can't find printer \"{0}\".", printerName);
                throw new Exception(msg);
            }

            printDoc.PrintController = new StandardPrintController();

            Margins margins = new Margins(0, 0, 0, 0);
            printDoc.DefaultPageSettings.Margins = margins;

            printDoc.DefaultPageSettings.Color = true;

            printDoc.DefaultPageSettings.Landscape = false;

            IEnumerable<PaperSize> sizes = printDoc.PrinterSettings.PaperSizes.Cast<PaperSize>();
            printDoc.DefaultPageSettings.PaperSize = sizes.Where(f => f.PaperName == "4x6in.").First();

            IEnumerable<PaperSource> sources = printDoc.PrinterSettings.PaperSources.Cast<PaperSource>();
            printDoc.DefaultPageSettings.PaperSource = sources.Where(f => f.SourceName == "Photo Tray").First();

            printDoc.PrintPage += new PrintPageEventHandler(PrintPage);
        }

        public static FotoPrinter Instance() {
            if (_printer == null) {
                _printer = new FotoPrinter();
            }

            return _printer;
        }

        // Handler for PrintPageEvents
        private void PrintPage(object sender, PrintPageEventArgs ev) {
            System.Drawing.Image image = System.Drawing.Image.FromFile(_path, true);

            ev.Graphics.DrawImage(image, -10, -12);

            ev.HasMorePages = false;
        }

        public void Print(string path) {
            Console.WriteLine("Printing File: {0}", path);
            _path = path;
            
            printDoc.Print();
        }
    }
}
