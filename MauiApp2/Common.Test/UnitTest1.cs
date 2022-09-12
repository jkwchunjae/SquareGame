using Common.Packet;

namespace Common.Test;

public class UnitTest1
{
    [Fact]
    public void PacketType과Class이름이같아야한다()
    {
        var packetTypes = Enum.GetValues<PacketType>();

        var assembly = typeof(PacketType).Assembly;
        var types = assembly.GetTypes();
        foreach (var packetType in packetTypes)
        {
            var type = types.FirstOrDefault(t => t.Name == packetType.ToString());

            Assert.NotNull(type);
        }
    }
}