﻿using System.Drawing.Printing;

namespace AutoPDF
{
    class PDFPrinter
    {
        //Печатает документ
        public void PrintPDF(string pdfPath, double paperWidth, double paperHeight)
        {
            using (var document = PdfiumViewer.PdfDocument.Load(pdfPath))
            {
                PrintDocument printDocument = document.CreatePrintDocument();
                printDocument.PrinterSettings = new PrinterSettings();

                //Установка пользовательских размеров бумаги
                PaperSize paperSize = new PaperSize("Custom", (int)(paperWidth * 100 / 25.4), (int)(paperHeight * 100 / 25.4));
                printDocument.DefaultPageSettings.PaperSize = paperSize;
                printDocument.DefaultPageSettings.Landscape = false;

                printDocument.PrintController = new StandardPrintController();
                printDocument.Print();
            }
        }
    }
}
