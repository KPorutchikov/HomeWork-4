using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace MySerialization
{
    public class SerializatorToJson<T> where T : class, new()
    {
        private string Filename {  get; set; }

        public SerializatorToJson( string filename) 
        { 
            Filename = filename;
        }

        public void Serialize(IList<T> data)
        {
            var serializer = new JsonSerializer();

            using (var sw = new StreamWriter(Filename))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                foreach (var d in data)
                {
                    serializer.Serialize(writer, d);
                }
            }
        }

        public IList<T> Deserialize()
        {
            IList<T> list = null;

            using (var sw = new StreamReader(Filename))
            list = FromDelimitedJson<T>(sw).ToList();

            return list;
        }

        private static IEnumerable<T> FromDelimitedJson<T>(TextReader reader, JsonSerializerSettings settings = null)
        {
            using (var jsonReader = new JsonTextReader(reader) { CloseInput = false, SupportMultipleContent = true })
            {
                var serializer = JsonSerializer.CreateDefault(settings);

                while (jsonReader.Read())
                {
                    if (jsonReader.TokenType == JsonToken.Comment)
                        continue;
                    yield return serializer.Deserialize<T>(jsonReader);
                }
            }
        }
    }


}
