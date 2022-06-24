using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace Localizer.Utils.Json
{
    public static class JsonUtil
    {
        private static readonly JsonSerializerOptions JsonSerializerOptions;
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
