using UnityEngine;
using System.IO;
using Newtonsoft.Json;

public static class SaveLoadSystem
{
    public static string folderName = "Saves";
    // Метод для сохранения данных
    public static void SaveGame<T>(T data, string fileName)
    {
        // Преобразование объекта в JSON строку
        string json = JsonConvert.SerializeObject(data, Formatting.Indented);

        // Получаем путь к файлу сохранения
        string folderPath = Path.Combine(Application.persistentDataPath, folderName);

        if(!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        string filePath = Path.Combine(folderPath, fileName);

        // Записываем JSON строку в файл
        File.WriteAllText(filePath, json);
    }

    // Метод для загрузки данных
    public static T LoadGame<T>(string fileName)
    {
        // Получаем путь к файлу загрузки
        string folderPath = Path.Combine(Application.persistentDataPath, folderName);

        if (!Directory.Exists(folderPath))
        {
            Debug.LogWarning("Directory not found: " + folderPath);
            return default(T);
        }
        else
        {
            string filePath = Path.Combine(folderPath, fileName);
            if (File.Exists(filePath))
            {
                // Считываем JSON строку из файла
                string json = File.ReadAllText(filePath);

                // Преобразуем JSON строку в объект нужного типа
                return JsonConvert.DeserializeObject<T>(json);
            }
            else
            {
                Debug.LogWarning("File not found: " + filePath);
                return default(T);
            }

        }
    }
}
