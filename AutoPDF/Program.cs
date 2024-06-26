using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace AutoPDF
{
    class Program
    {
        static void Main(string[] args)
        {
            string configFilePath = "AutoPDF_config.ini";
            // Проверяем, существует ли файл конфигурации
            if (!File.Exists(configFilePath))
            {
                WriteError("Файл конфигурации не найден.");
                return;
            }

            try
            {
                ConfigManager configManager = new ConfigManager(configFilePath);
                string printerWidthStr = null, printerHeightStr = null, pdfPath = null, optimizedPdfPath = null, targetDPIStr = null;
                //Проверяем и получаем значения конфигурации. Если конфигурация некорректна, записываем ошибку в файл и завершаем выполнение
                if (!configManager.TryGetValue("printerWidth", out printerWidthStr) ||
                    !double.TryParse(printerWidthStr, out double printerWidth) || printerWidth <= 0 ||
                    !configManager.TryGetValue("printerHeight", out printerHeightStr) ||
                    !double.TryParse(printerHeightStr, out double printerHeight) || printerHeight <= 0 ||
                    !configManager.TryGetValue("filePath", out pdfPath) ||
                    !configManager.TryGetValue("optimizedPath", out optimizedPdfPath) ||
                    !configManager.TryGetValue("targetDPI", out targetDPIStr) ||
                    !int.TryParse(targetDPIStr, out int targetDPI) || targetDPI <= 0)
                {
                    WriteError("Неправильная конфигурация. Проверьте AutoPDF_config.config");
                    return;
                }
                
                string fileName = Path.GetFileNameWithoutExtension(pdfPath);
                if (!File.Exists(pdfPath))
                {
                    WriteError($"Файл {fileName} не найден.");
                    return;
                }
                string resultFilePath = "AutoPDF_result.txt";
                Printer printer = new Printer(printerWidth, printerHeight);
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

                using (StreamWriter writer = new StreamWriter(resultFilePath))
                {
                    writer.WriteLine($"Результат работы AutoPDF с файлом {fileName}");
                    if (analyzer.CanPrint(document, printer))
                    {
                        double fillPercentage = analyzer.CalculateFillPercentage(optimizedImages);
                        writer.WriteLine($"Документ может быть распечатан на принтере. Процент заполнения: {fillPercentage}%");

                        PDFCreator creator = new PDFCreator();
                        creator.CreatePDF(optimizedImages, optimizedPdfPath);

                        writer.WriteLine($"Обработанный PDF-файл сохранён в {optimizedPdfPath}");
                    }
                    else
                    {
                        writer.WriteLine("Размер документа превышает возможности принтера.");
                    }
                }
            }
            catch (Exception ex)
            {
                WriteError($"Произошла непредвиденная ошибка: {ex.Message}");
            }
        }

        //Записывает ошибки в файл
        static void WriteError(string message)
        {
            string resultFilePath = "AutoPDF_result.txt";
            using (StreamWriter writer = new StreamWriter(resultFilePath))
            {
                writer.WriteLine(message);
            }
        }
    }
}

