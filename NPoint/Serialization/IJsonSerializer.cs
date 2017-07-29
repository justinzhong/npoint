namespace NPoint.Serialization
{
    public interface IJsonSerializer
    {
        T Deserialize<T>(string json);

        string Serialize(object graph);
    }
}