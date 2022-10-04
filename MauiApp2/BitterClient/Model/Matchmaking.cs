namespace BitterClient.Model;

public record MatchRequest
{
}
public record MatchResponse
{
}
public record MatchStatusRequest
{
    public string? TestIpAddress { get; init; }
    public int TestPort { get; init; }
}
public record MatchStatusResponse
{
    public string? IpAddress { get; init; }
    public int Port { get; init; }
}
