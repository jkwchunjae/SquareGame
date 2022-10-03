namespace BitterClient.Model;

public class BitterAuthRequest
{
    public UserId UserId { get; init; }

    public BitterAuthRequest(UserId userId)
    {
        UserId = userId;
    }
}

public class BitterAuthResponse
{
    public string? Token { get; set; }
}
