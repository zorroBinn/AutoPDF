using PdfSharp.Pdf.IO;
using PdfSharp.Pdf;
using System.Collections.Generic;

namespace AutoPDF
{
    class PDFDocument
    {
        public string Path { get; set; } //Путь к документу
        public int PageCount { get; set; } //Количество страниц в документе
        public List<(double Width, double Height)> PageSizes { get; set; } //Исходные размеры каждой страницы

        //Конструктор
        public PDFDocument(string path)
        {
            Path = path;
            PageSizes = new List<(double Width, double Height)>();

            PdfDocument document = PdfReader.Open(path, PdfDocumentOpenMode.ReadOnly);
            PageCount = document.PageCount;

            foreach (PdfPage page in document.Pages)
            {
                PageSizes.Add((page.Width, page.Height));
            }
        }
    }
}
