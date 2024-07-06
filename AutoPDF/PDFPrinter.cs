using PdfiumViewer;
using System.Drawing;
using System.Drawing.Printing;

class PDFPrinter
{
    private PrintDocument printDocument; //Объект для управления процессом печати
    private PdfDocument pdfDoc; //Объект, представляющий PDF-документ
    private double printerWidth; //Ширина принтера в мм

    //Конструктор без параметров. Подписывается на события
    public PDFPrinter()
    {
        printDocument = new PrintDocument();
        printDocument.QueryPageSettings += PrintDocument_QueryPageSettings;
        printDocument.PrintPage += PrintDocument_PrintPage;
    }

    //Конструктор с параметрами, загружает PDF-документ и устанавливает ширину принтера
    public PDFPrinter(string fileName, double printerWidth) : this()
    {
        OpenDocument(fileName);
        this.printerWidth = printerWidth;
    }

    //Открывает документ
    public void OpenDocument(string fileName)
    {
        pdfDoc?.Dispose();
        pdfDoc = PdfDocument.Load(fileName);
    }

    //Запускает печать с указанными настройками принтера
    public void Print(PrinterSettings printerSettings)
    {
        printDocument.PrinterSettings = printerSettings;
        printDocument.Print();
    }

    //Настраивает параметры страницы перед печатью
    private void PrintDocument_QueryPageSettings(object sender, QueryPageSettingsEventArgs e)
    {
        var size = pdfDoc.PageSizes[0];

        double pageWidthInInches = printerWidth / 25.4;
        double pageHeightInInches = size.Height / 72.0;

        //Установка пользовательского размера бумаги
        e.PageSettings.PaperSize = new PaperSize("Custom", (int)(pageWidthInInches * 100), (int)(pageHeightInInches * 100 + 100));

        //Установка книжного формата
        e.PageSettings.Landscape = false;
    }

    //Обработчик события для печати содержимого страницы
    private void PrintDocument_PrintPage(object sender, PrintPageEventArgs e)
    {
        Graphics graphics = e.Graphics;
        graphics.PageUnit = GraphicsUnit.Point;
        var pageSize = pdfDoc.PageSizes[0];
        using (var page = pdfDoc.Render(0, (int)pageSize.Width, (int)pageSize.Height, true))
        {
            graphics.DrawImage(page, 0, 0, pageSize.Width, pageSize.Height);
        }
        e.HasMorePages = false;
    }
}