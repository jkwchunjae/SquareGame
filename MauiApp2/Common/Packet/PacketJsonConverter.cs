using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Reflection;

namespace Common.Packet;

internal class PacketJsonConverter : JsonConverter<PacketBase>
{
    public override PacketBase? ReadJson(JsonReader reader, Type objectType, PacketBase? existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        JObject? objLogData = serializer.Deserialize(reader) as JObject;
        var logTypeEnum = objLogData?.Value<PacketType>("Type");

        if (logTypeEnum == null)
        {
            throw new PacketJsonConvertException();
        }
        var packetType = Type.GetType(logTypeEnum?.ToString() ?? string.Empty);
        if (packetType == null)
            return new PacketBase();

        PacketBase? packetData = Activator.CreateInstance(packetType) as PacketBase;

        foreach (var prop in packetType.GetProperties())
        {
            var jsonPropertyAttr = prop.GetCustomAttribute<JsonPropertyAttribute>();
            var propertyName = jsonPropertyAttr?.PropertyName ?? prop.Name;
            var property = objLogData?.Properties().FirstOrDefault(p => p.Name.Equals(propertyName, StringComparison.OrdinalIgnoreCase));
            var value = property?.Value?.ToObject(prop.PropertyType);
            prop.SetValue(packetData, value);
        }

        return packetData;
    }

    public override void WriteJson(JsonWriter writer, PacketBase? packetData, JsonSerializer serializer)
    {
        var logTypeEnum = packetData?.Type;
        var packetType = Type.GetType(logTypeEnum?.ToString() ?? string.Empty);

        if (packetType == null)
            throw new PacketJsonConvertException();

        writer.WriteStartObject();
        foreach (var prop in packetType!.GetProperties())
        {
            var jsonProperty = prop.GetCustomAttribute<JsonPropertyAttribute>();
            var propertyName = jsonProperty?.PropertyName ?? prop.Name;
            writer.WritePropertyName(propertyName);
            serializer.Serialize(writer, prop.GetValue(packetData));
        }
        writer.WriteEndObject();
    }

}
