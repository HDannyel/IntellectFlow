using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntellectFlow.DataModel
{
    public class Document : BaseEntity
    {
        public int Id { get; set; }
        public required string FileName { get; set; } // Имя
        public required string FilePath { get; set; }  // Путь к файлу на сервере 
        public required string ContentType { get; set; } // Тип

    }
}
