using System.Text.Json;

namespace Common.Utils
{
    public static class JsonUtil
    {
        public static readonly JsonSerializerOptions JsonSerializerOptions;
        static JsonUtil()
        {
            JsonSerializerOptions = new JsonSerializerOptions
            {
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = true
            };
        }

        public static string Serialize<T>(T value)
        {
            return JsonSerializer.Serialize(value, JsonSerializerOptions);
        }
    }
}
