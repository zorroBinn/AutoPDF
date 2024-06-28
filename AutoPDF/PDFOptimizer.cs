using PdfiumViewer;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace AutoPDF
{
    class PDFOptimizer
    {
        //Собирает непустые обрезанные страницы
        public List<Bitmap> Optimize(PDFDocument document, int targetDPI)
        {
            var optimizedImages = new List<Bitmap>();
            for (int i = 0; i < document.PageCount; i++)
            {
                using (var pdfDocument = PdfiumViewer.PdfDocument.Load(document.Path))
                {
                    int dpiX = targetDPI == 0 ? GetOriginalDPI(pdfDocument, i, "X") : targetDPI;
                    int dpiY = targetDPI == 0 ? GetOriginalDPI(pdfDocument, i, "Y") : targetDPI;
                    using (var image = RenderPage(pdfDocument, i, dpiX, dpiY))
                    {
                        var croppedImage = CropImage(image);
                        if (croppedImage != null)
                        {
                            var rotatedImage = RotateImage(croppedImage);
                            optimizedImages.Add(croppedImage);
                        }
                    }
                }
            }
            return optimizedImages;
        }

        //Получает исходный DPI страницы
        private int GetOriginalDPI(PdfiumViewer.PdfDocument pdfDocument, int pageIndex, string XorY)
        {
            using (var page = pdfDocument.Render(pageIndex, 72, 72, PdfRenderFlags.CorrectFromDpi))
            {
                if (XorY == "X")
                {
                    var widthInPixels = page.Width;
                    double dpiX = widthInPixels / (pdfDocument.PageSizes[pageIndex].Width / 72.0);
                    return (int)Math.Round(dpiX);
                }
                else
                {
                    var heightInPixels = page.Height;
                    double dpiY = heightInPixels / (pdfDocument.PageSizes[pageIndex].Height / 72.0);
                    return (int)Math.Round(dpiY);
                }
            }
        }

        //Рендерит Bitmap по странице документа
        private Bitmap RenderPage(PdfiumViewer.PdfDocument pdfDocument, int pageIndex, int dpiX, int dpiY)
        {
            var pageImage = pdfDocument.Render(pageIndex, dpiX, dpiY, PdfRenderFlags.CorrectFromDpi);
            //Конвертирует Image в Bitmap
            Bitmap bitmap = new Bitmap(pageImage.Width, pageImage.Height);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.DrawImage(pageImage, 0, 0, pageImage.Width, pageImage.Height);
            }
            return bitmap;
        }

        //Обрезает страницу
        private Bitmap CropImage(Bitmap source)
        {
            int top = 0, bottom = source.Height - 1, left = 0, right = source.Width - 1;
            bool found = false;  //Флаг непустой страницы
            //Поиск верхней не белой точки
            for (int y = 0; y < source.Height; y++)
            {
                for (int x = 0; x < source.Width; x++)
                {
                    if (source.GetPixel(x, y).ToArgb() != Color.White.ToArgb())
                    {
                        top = y;
                        found = true;
                        break;
                    }
                }
                if (found) break;
            }
            //Поиск нижней не белой точки
            found = false;
            for (int y = source.Height - 1; y >= 0; y--)
            {
                for (int x = 0; x < source.Width; x++)
                {
                    if (source.GetPixel(x, y).ToArgb() != Color.White.ToArgb())
                    {
                        bottom = y;
                        found = true;
                        break;
                    }
                }
                if (found) break;
            }
            //Поиск левой не белой точки
            found = false;
            for (int x = 0; x < source.Width; x++)
            {
                for (int y = 0; y < source.Height; y++)
                {
                    if (source.GetPixel(x, y).ToArgb() != Color.White.ToArgb())
                    {
                        left = x;
                        found = true;
                        break;
                    }
                }
                if (found) break;
            }
            //Поиск правой не белой точки
            found = false;
            for (int x = source.Width - 1; x >= 0; x--)
            {
                for (int y = 0; y < source.Height; y++)
                {
                    if (source.GetPixel(x, y).ToArgb() != Color.White.ToArgb())
                    {
                        right = x;
                        found = true;
                        break;
                    }
                }
                if (found) break;
            }
            //Если страница пустая - возвращает NULL
            if (!found || top == bottom && left == right)
            {
                return null;
            }
            //Создаём Bitmap обрезанный страницы
            Rectangle cropRect = new Rectangle(left, top, right - left + 1, bottom - top + 1);
            Bitmap target = new Bitmap(cropRect.Width, cropRect.Height);
            using (Graphics g = Graphics.FromImage(target))
            {
                g.DrawImage(source, new Rectangle(0, 0, target.Width, target.Height), cropRect, GraphicsUnit.Pixel);
            }
            return target;
        }
        
        //Поворачивает страницу, если её ширина больше высоты
        private Bitmap RotateImage(Bitmap image)
        {
            if (image.Width > image.Height)
            {
                image.RotateFlip(RotateFlipType.Rotate90FlipNone);
            }
            return image;
        }
    }
}
