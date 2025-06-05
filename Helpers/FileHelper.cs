using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntellectFlow.Helpers
{
    public class FileHelper
    {
        private const string BasePath = "Files/Teachers";

        public static string SaveTeacherFile(string teacherName, string disciplineName, string courseName, string subFolder, string sourceFilePath)
        {
            // Формируем путь
            string folderPath = Path.Combine(BasePath, SanitizeFileName(teacherName), SanitizeFileName(disciplineName), SanitizeFileName(courseName), subFolder);

            // Создаём директории, если их нет
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            // Имя файла
            string fileName = Path.GetFileName(sourceFilePath);

            // Итоговый путь
            string destFilePath = Path.Combine(folderPath, fileName);

            // Копируем файл (если уже есть — перезаписываем)
            File.Copy(sourceFilePath, destFilePath, overwrite: true);

            return destFilePath;
        }

        // Чтобы избежать проблем с недопустимыми символами в папках
        private static string SanitizeFileName(string name)
        {
            foreach (var c in Path.GetInvalidFileNameChars())
            {
                name = name.Replace(c, '_');
            }
            return name;
        }
    }
}
