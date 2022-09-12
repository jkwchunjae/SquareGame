using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Common.Packet;

[JsonConverter(typeof(StringEnumConverter))]
public enum PacketType
{
    CS_Ping,

    SC_Pong,
}
