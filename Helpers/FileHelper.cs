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
        private readonly string _rootFolder;

        public FileHelper(string rootFolder)
        {
            _rootFolder = rootFolder;
        }


        public string GetTeacherFilePath(string teacherName, string disciplineName, string courseName, string fileType, string fileName)
        {
            // Формируем путь по частям
            var path = Path.Combine(_rootFolder, "Teachers", SanitizeFileName(teacherName),
                SanitizeFileName(disciplineName), SanitizeFileName(courseName), SanitizeFileName(fileType));

            // Создаём папки, если нет
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            // Полный путь к файлу
            return Path.Combine(path, fileName);
        }
        public string GetStudentFilePath(string studentName, string disciplineName, string courseName, string fileType, string fileName)
        {
            var path = Path.Combine(_rootFolder, "Students", SanitizeFileName(studentName),
                SanitizeFileName(disciplineName), SanitizeFileName(courseName), SanitizeFileName(fileType));

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            return Path.Combine(path, fileName);
        }


        private string SanitizeFileName(string name)
        {
            foreach (var c in Path.GetInvalidFileNameChars())
            {
                name = name.Replace(c, '_');
            }
            return name;
        }
    }
}
