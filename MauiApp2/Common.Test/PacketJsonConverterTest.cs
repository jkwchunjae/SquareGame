using Common.Packet;
using Common.Packet.ClientToServer;
using Common.Packet.ServerToClient;
using Newtonsoft.Json;

namespace Common.Test;

public class PacketJsonConverterTest
{
    [Fact]
    public void ConvertTest_CS_Ping()
    {
        var ping = new CS_Ping() { Value = 123 };
        var jsonText = JsonConvert.SerializeObject(ping);
        var packet = JsonConvert.DeserializeObject<PacketBase>(jsonText);
        var pingData = packet as CS_Ping;

        Assert.Equal(PacketType.CS_Ping, packet?.Type);
        Assert.Equal(ping.Value, pingData?.Value);
    }

    [Fact]
    public void ConverTest_SC_Pong()
    {
        var pong = new SC_Pong() { Value = 123 };
        var jsonText = JsonConvert.SerializeObject(pong);
        var packet = JsonConvert.DeserializeObject<PacketBase>(jsonText);
        var pongData = packet as SC_Pong;

        Assert.Equal(PacketType.SC_Pong, packet?.Type);
        Assert.Equal(pong.Value, pongData?.Value);
    }
}
