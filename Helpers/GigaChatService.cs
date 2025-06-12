using System;
using System.Net;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Text.Json;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using IntellectFlow.DataModel;
using System.Text.RegularExpressions;

namespace IntellectFlow.Helpers
{
    public class GigaChatService
    {
        private const string ClientId = "ZTNiN2MzZjItYTA2Zi00YzgzLTlmMGEtNmQxNWViNGYyZjBhOmJkMjE1MWE2LWE5YTQtNDc4Ni04Mzg2LWJjNjNiYTY2NjQ3ZA==";
        private const string AuthUrl = "https://ngw.devices.sberbank.ru:9443/api/v2/oauth";
        private const string ChatCompletionUrl = "https://gigachat.devices.sberbank.ru/api/v1/chat/completions";

        private string _accessToken = string.Empty;
        private DateTime _tokenExpiration = DateTime.MinValue;

        public async Task<string> GetAccessToken()
        {
            if (!string.IsNullOrEmpty(_accessToken) && DateTime.UtcNow < _tokenExpiration.AddMinutes(-5))
            {
                return _accessToken;
            }

            try
            {
                var request = (HttpWebRequest)WebRequest.Create(AuthUrl);
                request.Method = "POST";
                request.Headers.Add("Authorization", $"Basic {ClientId}");
                request.Headers.Add("RqUID", Guid.NewGuid().ToString());
                request.ContentType = "application/x-www-form-urlencoded";

                using (var requestStream = await request.GetRequestStreamAsync())
                {
                    byte[] contentBytes = Encoding.UTF8.GetBytes("scope=GIGACHAT_API_PERS");
                    await requestStream.WriteAsync(contentBytes, 0, contentBytes.Length);
                }

                using (var response = (HttpWebResponse)await request.GetResponseAsync())
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    var responseBody = await reader.ReadToEndAsync();
                    using var json = JsonDocument.Parse(responseBody);

                    _accessToken = json.RootElement.GetProperty("access_token").GetString();
                    var expiresAtElement = json.RootElement.GetProperty("expires_at");

                    long expiresAtValue = expiresAtElement.ValueKind == JsonValueKind.String
                        ? long.Parse(expiresAtElement.GetString())
                        : expiresAtElement.GetInt64();

                    try
                    {
                        _tokenExpiration = DateTimeOffset.FromUnixTimeSeconds(expiresAtValue).UtcDateTime;
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        _tokenExpiration = DateTimeOffset.FromUnixTimeMilliseconds(expiresAtValue).UtcDateTime;
                    }

                    if (_tokenExpiration < DateTime.UtcNow)
                    {
                        throw new Exception("Получен уже истёкший токен");
                    }

                    return _accessToken;
                }
            }
            catch (WebException ex)
            {
                string errorDetails = ex.Response != null
                    ? new StreamReader(ex.Response.GetResponseStream()).ReadToEnd()
                    : ex.Message;

                throw new Exception($"Ошибка получения токена авторизации: {errorDetails}", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Непредвиденная ошибка при получении токена", ex);
            }
        }

        public async Task<string> ReadFileContentWithLimit(string filePath, int maxLength)
        {
            try
            {
                if (string.IsNullOrEmpty(filePath)) return string.Empty;
                if (!File.Exists(filePath)) return string.Empty;

                string extension = Path.GetExtension(filePath).ToLower();
                string content;

                if (extension == ".txt" || extension == ".cs" || extension == ".py" ||
                    extension == ".json" || extension == ".html")
                {
                    content = await File.ReadAllTextAsync(filePath);
                }
                else if (extension == ".docx")
                {
                    content = "DOCX файл: требуется реализация чтения";
                }
                else if (extension == ".pdf")
                {
                    content = "PDF файл: требуется реализация чтения";
                }
                else
                {
                    return $"Неподдерживаемый формат файла: {extension}";
                }

                return content.Length > maxLength ? content.Substring(0, maxLength) + "..." : content;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ошибка чтения файла {filePath}: {ex.Message}");
                return string.Empty;
            }
        }

        public async Task<(string Comment, int Score)> AnalyzeSubmission(
       string token,
       string assignmentContent,
       string submissionContent)
        {
            var messages = new List<ChatMessage>
    {
        new ChatMessage
        {
            role = "system",
            content = @"Ты - преподаватель, который анализирует решения студентов. Твои задачи:

1. Сравни ответ студента с исходным заданием
2. Определи:
   - Соответствует ли ответ заданию
   - Полноту решения
   - Наличие ошибок
   - Качество выполнения
3. Дай развернутый комментарий:
   - Укажи конкретные ошибки
   - Отметь правильные части решения
   - Предложи улучшения
4. Поставь оценку от 1 до 5:
   - 1: Не соответствует заданию
   - 2: Частичное соответствие с грубыми ошибками
   - 3: Основные требования выполнены, но есть недочеты
   - 4: Хорошее решение с мелкими погрешностями
   - 5: Идеальное решение

Формат ответа:
КОММЕНТАРИЙ: [твой комментарий]
ОЦЕНКА: [число от 1 до 5]"
        },
        new ChatMessage
        {
            role = "user",
            content = $"ИСХОДНОЕ ЗАДАНИЕ:\n{assignmentContent}\n\n" +
                      $"ОТВЕТ СТУДЕНТА:\n{submissionContent}"
        }
    };

            var payload = new
            {
                model = "GigaChat-Max",
                messages,
                temperature = 0.7,
                max_tokens = 1000
            };

            try
            {
                var request = (HttpWebRequest)WebRequest.Create(ChatCompletionUrl);
                request.Method = "POST";
                request.Headers.Add("Authorization", $"Bearer {token}");
                request.ContentType = "application/json";
                request.Timeout = 30000;

                using (var requestStream = await request.GetRequestStreamAsync())
                {
                    byte[] jsonBytes = JsonSerializer.SerializeToUtf8Bytes(payload);
                    await requestStream.WriteAsync(jsonBytes, 0, jsonBytes.Length);
                }

                using (var response = (HttpWebResponse)await request.GetResponseAsync())
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    var responseBody = await reader.ReadToEndAsync();
                    using var doc = JsonDocument.Parse(responseBody);

                    var aiResponse = doc.RootElement
                        .GetProperty("choices")[0]
                        .GetProperty("message")
                        .GetProperty("content")
                        .GetString();

                    return ParseAIResponse(aiResponse);
                }
            }
            catch (WebException ex) when (ex.Response != null)
            {
                using var stream = ex.Response.GetResponseStream();
                using var reader = new StreamReader(stream);
                var errorResponse = await reader.ReadToEndAsync();
                throw new Exception($"API Error: {errorResponse}", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка при анализе решения", ex);
            }
        }
        public async Task<string> GeneralChatAsync(string token, List<ChatMessage> messages)
        {
            var payload = new
            {
                model = "GigaChat-Max",
                messages,
                temperature = 0.7,
                max_tokens = 2000
            };

            try
            {
                var request = (HttpWebRequest)WebRequest.Create(ChatCompletionUrl);
                request.Method = "POST";
                request.Headers.Add("Authorization", $"Bearer {token}");
                request.ContentType = "application/json";
                request.Timeout = 30000;

                using (var requestStream = await request.GetRequestStreamAsync())
                {
                    byte[] jsonBytes = JsonSerializer.SerializeToUtf8Bytes(payload);
                    await requestStream.WriteAsync(jsonBytes, 0, jsonBytes.Length);
                }

                using (var response = (HttpWebResponse)await request.GetResponseAsync())
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    var responseBody = await reader.ReadToEndAsync();
                    using var doc = JsonDocument.Parse(responseBody);
                    return doc.RootElement
                        .GetProperty("choices")[0]
                        .GetProperty("message")
                        .GetProperty("content")
                        .GetString();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка в чате с ИИ", ex);
            }
        }


        private (string Comment, int Score) ParseAIResponse(string aiResponse)
        {
            string comment = "";
            int score = 0;

            var commentMatch = Regex.Match(aiResponse, @"КОММЕНТАРИЙ:\s*(.*?)\s*ОЦЕНКА:", RegexOptions.Singleline);
            var scoreMatch = Regex.Match(aiResponse, @"ОЦЕНКА:\s*(\d)");

            if (commentMatch.Success)
            {
                comment = commentMatch.Groups[1].Value.Trim();
            }

            if (scoreMatch.Success && int.TryParse(scoreMatch.Groups[1].Value, out int parsedScore))
            {
                score = parsedScore;
            }

            return (comment, score);

        }
    


        public class ChatMessage
        {
            public string role { get; set; }
            public string content { get; set; }
        }
    }
}