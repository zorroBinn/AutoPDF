using System.Collections.Generic;
using System.IO;

namespace AutoPDF
{
    class ConfigManager
    {
        private readonly Dictionary<string, string> configValues; //Словарь с ключами и значениями
        
        //Конструктор с параметром
        public ConfigManager(string filePath)
        {
            configValues = new Dictionary<string, string>();
            LoadConfig(filePath);
        }

        //Построчно читает файл конфигурации и создаёт словарь ключей и значений
        private void LoadConfig(string filePath)
        {
            var lines = File.ReadAllLines(filePath);
            
            foreach (var line in lines)
            {
                //Пропускаем комментарии и пустые строки
                if (string.IsNullOrWhiteSpace(line) || line.Trim().StartsWith(";"))
                    continue;

                //Разделяем строку на ключ и значение
                var parts = line.Split('=');
                
                if (parts.Length == 2)
                {
                    var key = parts[0].Trim();
                    var value = parts[1].Trim().Trim('"');
                    configValues[key] = value;
                }
            }
        }

        //Проверяет, что значение по ключу не пустое
        public bool TryGetValue(string key, out string value)
        {
            return configValues.TryGetValue(key, out value);
        }
    }
}
