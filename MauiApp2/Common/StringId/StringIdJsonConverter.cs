using Newtonsoft.Json;

namespace Common;

public class StringIdJsonConverter<T> : JsonConverter<T> where T : StringId, new()
{
    public override T? ReadJson(JsonReader reader, Type objectType, T? existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        if (reader.Value is null)
            return default;

        if (reader.Value is string)
        {
            var stringId = new T();
            stringId.Id = reader.Value as string ?? throw new StringIdConvertException();
            return stringId;
        }

        return default;
    }

    public override void WriteJson(JsonWriter writer, T? value, JsonSerializer serializer)
    {
        writer.WriteValue(value?.ToString() ?? throw new StringIdConvertException());
    }
}
