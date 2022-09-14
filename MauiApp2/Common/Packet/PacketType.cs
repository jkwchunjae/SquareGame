using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Common.Packet;

[JsonConverter(typeof(StringEnumConverter))]
public enum PacketType
{
    CS_Ping,
    CS_Login,
    CS_Pick,

    SC_Pong,
    SC_YourRole,
    SC_Board,
    SC_Result,
}
