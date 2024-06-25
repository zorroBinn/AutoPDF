namespace AutoPDF
{
    class Printer
    {
        public double WidthPaperSheet { get; set; } //Ширина листа бумаги принтера
        public double HeightPaperSheet { get; set; } //Высота листа бумаги принтера

        public Printer(double widthPaperSheet, double heightPaperSheet)
        {
            WidthPaperSheet = widthPaperSheet;
            HeightPaperSheet = heightPaperSheet;
        }
    }
}
