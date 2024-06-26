using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace AutoPDF
{
    class PDFCreator
    {
        //Создаёт PDF-файл из страниц-изображений и сохраняет по указанному пути
        public void CreatePDF(List<Bitmap> images, string outputPath)
        {
            PdfDocument document = new PdfDocument();
            foreach (var image in images)
            {
                PdfPage page = document.AddPage();
                page.Width = image.Width;
                page.Height = image.Height;

                using (XGraphics gfx = XGraphics.FromPdfPage(page))
                {
                    using (var stream = new MemoryStream())
                    {
                        image.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                        stream.Seek(0, SeekOrigin.Begin);
                        using (XImage xImage = XImage.FromStream(stream))
                        {
                            gfx.DrawImage(xImage, 0, 0, image.Width, image.Height);
                        }
                    }
                }
            }
            document.Save(outputPath);
        }
    }
}
