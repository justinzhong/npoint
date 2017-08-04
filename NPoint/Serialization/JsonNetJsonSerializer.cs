using Newtonsoft.Json;
using System;

namespace NPoint.Serialization
{
    public class JsonNetJsonSerializer : IJsonSerializer
    {
        public T Deserialize<T>(string json)
        {
            if (string.IsNullOrEmpty(json)) throw new ArgumentException("String cannot be empty or null", nameof(json));

            return JsonConvert.DeserializeObject<T>(json);
        }

        public string Serialize(object graph)
        {
            return JsonConvert.SerializeObject(graph);
        }
    }
}