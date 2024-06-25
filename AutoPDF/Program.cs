using System;
using System.Collections.Generic;
using System.Drawing;

namespace AutoPDF
{
    class Program
    {
        static void Main(string[] args)
        {
            string pdfPath = "C://Users//zorro//Downloads//granitsy_ZSO.pdf";
            string optimizedPdfPath = "C://Users//zorro//Downloads//optimized_granitsy_ZSO.pdf";
            int targetDPI = 250; //Целевое количество точек на дюйм (качество изображения)
            Printer printer = new Printer(10000, 10000);
            PDFDocument document = new PDFDocument(pdfPath);
            PDFOptimizer optimizer = new PDFOptimizer();
            PDFAnalyzer analyzer = new PDFAnalyzer();

            List<Bitmap> optimizedImages = optimizer.Optimize(document, targetDPI);

            document.PageSizes = new List<(double Width, double Height)>();
            foreach (var image in optimizedImages)
            {
                document.PageSizes.Add((image.Width, image.Height));
            }
            document.PageCount = document.PageSizes.Count;

            if (analyzer.CanPrint(document, printer))
            {
                double fillPercentage = analyzer.CalculateFillPercentage(optimizedImages);
                Console.WriteLine($"The document can be printed. Fill percentage: {fillPercentage}%");

                PDFCreator creator = new PDFCreator();
                creator.CreatePDF(optimizedImages, optimizedPdfPath);

                Console.WriteLine($"Optimized PDF saved to {optimizedPdfPath}");
            }
            else
            {
                Console.WriteLine("The document size exceeds the printer's capacity.");
            }
        }
    }
}

