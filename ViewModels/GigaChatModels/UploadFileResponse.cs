using System;

namespace IntellectFlow.ViewModels.GigaChatModels
{
    public class UploadFileResponse
    {
        public int bytes { get; set; }
        public int created_at { get; set; }
        public string filename { get; set; }
        public string id { get; set; }
        public string @object { get; set; } // "file"
        public string purpose { get; set; }
        public string access_policy { get; set; }

        public Guid FileId => Guid.Parse(id);
    }
}