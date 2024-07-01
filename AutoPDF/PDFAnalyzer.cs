using System.Collections.Generic;
using System.Drawing;

namespace AutoPDF
{
    class PDFAnalyzer
    {
        //Проверяет возможность принтера распечатать оптимизированный документ
        public bool CanPrint(PDFDocument document, Printer printer)
        {
            foreach (var pageSize in document.PageSizes)
            {
                if (pageSize.Width * 25.4 / 72 > printer.WidthPaperSheet || pageSize.Height * 25.4 / 72 > printer.HeightPaperSheet)
                {
                    return false;
                }
            }
            
            return true;
        }

        //Считает процент заполнения
        public double CalculateFillPercentage(List<Bitmap> optimizedImages)
        {
            double totalPixels = 0; //Общее количество пикселей
            double coloredPixels = 0; //Количество не белых пикселей
            
            foreach (var image in optimizedImages)
            {
                totalPixels += image.Width * image.Height;
                for (int y = 0; y < image.Height; y++)
                {
                    for (int x = 0; x < image.Width; x++)
                    {
                        if (image.GetPixel(x, y).ToArgb() != Color.White.ToArgb())
                        {
                            coloredPixels++;
                        }
                    }
                }
            }
            
            return (coloredPixels / totalPixels) * 100;
        }
    }
}
