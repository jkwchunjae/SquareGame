using BitterClient.Model;
using Grpc.Core;
using Grpc.Net.Client;
using Slothy.Protocol.Base;
using Slothy.Protocol.UserLobby;
using static Slothy.Protocol.UserLobby.UserLobbyApi;
using GetUserProfileRequest = BitterClient.Model.GetUserProfileRequest;

namespace BitterClient;

public interface IBitterUserLobby
{
    Task<BitterAuthResponse> AuthenticateAsync(BitterAuthRequest authRequest);
    Task<GetUserProfileResponse> GetUserProfileAsync(GetUserProfileRequest userProfileRequest);
}

public class BitterUserLobby : IBitterUserLobby
{
    private UserLobbyApiClient _lobbyClient { get; init; }
    private string? _token;
    public BitterUserLobby(string lobbyUrl)
    {
        var channel = GrpcChannel.ForAddress(lobbyUrl);
        _lobbyClient = new UserLobbyApiClient(channel);
    }

    public async Task<BitterAuthResponse> AuthenticateAsync(BitterAuthRequest authRequest)
    {
        var res = await _lobbyClient.AuthenticateAsync(new AuthReq
        {
            UserId = authRequest.UserId.ToString(),
        });

        if (!string.IsNullOrEmpty(res?.Token))
        {
            _token = res.Token;
        }

        return new BitterAuthResponse
        {
            Token = res?.Token,
        };
    }

    public async Task<GetUserProfileResponse> GetUserProfileAsync(GetUserProfileRequest userProfileRequest)
    {
        Metadata? headers = string.IsNullOrEmpty(_token) ? null : new Metadata
        {
            { "Authorization", $"Bearer {_token}" },
        };
        var res = await _lobbyClient.GetUserProfileAsync(new Slothy.Protocol.UserLobby.GetUserProfileRequest(), headers);

        if (!string.IsNullOrEmpty(res?.UserId))
        {
            return new GetUserProfileResponse
            {
                UserId = new UserId(res.UserId),
            };
        }
        else
        {
            return new GetUserProfileResponse();
        }
    }
}
