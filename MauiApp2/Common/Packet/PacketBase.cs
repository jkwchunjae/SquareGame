using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Common.Packet;

[JsonConverter(typeof(PacketJsonConverter))]
public class PacketBase
{
    public PacketType Type { get; set; }
}
