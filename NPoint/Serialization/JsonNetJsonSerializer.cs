using Newtonsoft.Json;

namespace NPoint.Serialization
{
    public class JsonNetJsonSerializer : IJsonSerializer
    {
        public T Deserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        public string Serialize(object graph)
        {
            return JsonConvert.SerializeObject(graph);
        }
    }
}