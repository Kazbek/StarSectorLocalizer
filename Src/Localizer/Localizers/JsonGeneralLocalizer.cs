using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Localizer.Localizers
{
    public static class JsonGeneralLocalizer
    {
        private static readonly List<char> startTrailingChars = new List<char> { '\t', ' ' };
        public static int Localize(string targetJsonPath, IDictionary<string, string> dictionary)
        {
            StringBuilder sb = new StringBuilder();

            foreach (var l in File.ReadAllLines(targetJsonPath))
            {
                for(int i = 0; i < l.Length; i++)
                {
                    if (startTrailingChars.Contains(l[i]))
                        continue;
                    if (l[i] == '#')
                        break;
                    sb.AppendLine(l);
                    break;
                }
            }

            JsonNode doc = JsonNode.Parse(sb.ToString(),null, new JsonDocumentOptions { AllowTrailingCommas = true });
            int localized = LocalizeElement(doc.Root, dictionary);

            using FileStream fs = File.Create(targetJsonPath);
            using var writer = new Utf8JsonWriter(fs, options: new JsonWriterOptions
            {
                Indented = true
            });
            doc.WriteTo(writer);

            return localized;
        }

        private static int LocalizeElement(JsonNode jsonNode, IDictionary<string, string> dictionary)
        {
            int localized = 0;
            if(jsonNode is JsonObject jsonObject)
            {
                Dictionary<string, string> replace = new Dictionary<string, string>();
                foreach(var prop in jsonObject)
                {
                    if (prop.Value is JsonValue jsonValue)
                    {
                        if (jsonValue.TryGetValue(out string value) && value != null)
                        {
                            if (dictionary.TryGetValue(value, out string translation))
                                replace.Add(prop.Key, translation);
                        }

                    }
                    else
                    {
                        localized += LocalizeElement(prop.Value, dictionary);

                    }
                }

                foreach(var pair in replace)
                {
                    jsonObject.Remove(pair.Key);
                    jsonObject.Add(pair.Key, pair.Value);
                    localized++;
                }
            }
            else if(jsonNode is JsonArray jsonArray)
            {
                //foreach(var item in jsonArray)
                for (int i = 0; i < jsonArray.Count; i++)
                {
                    var item = jsonArray[i];

                    if (item is JsonValue jsonValue)
                    {
                        if (jsonValue.TryGetValue(out string value) && value != null)
                        {
                            if (dictionary.TryGetValue(value, out string translation))
                            {
                                jsonArray[i] = (JsonValue)translation;
                                localized++;
                            }
                        }

                    }
                    else
                    {
                        localized += LocalizeElement(item, dictionary);

                    }
                }
            }
            else if(jsonNode is JsonValue jsonValue)
            {
                if(jsonValue.TryGetValue(out string value) && value != null)
                {
                    jsonNode = (JsonValue)"asdasdasdsads";
                    if (dictionary.TryGetValue(value, out string translation))
                        jsonValue = (JsonValue) translation;
                }

            }

            return localized;
        }
    }
}
