using BitterClient.Model;
using Grpc.Core;
using Grpc.Net.Client;
using Slothy.Protocol.Base;
using Slothy.Protocol.UserLobby;
using static Slothy.Protocol.UserLobby.UserLobbyApi;
using GetUserProfileRequest = BitterClient.Model.GetUserProfileRequest;
using MatchRequest = BitterClient.Model.MatchRequest;
using MatchResponse = BitterClient.Model.MatchResponse;
using MatchStatusResponse = BitterClient.Model.MatchStatusResponse;

namespace BitterClient;

public interface IBitterUserLobby
{
    Task<BitterAuthResponse> AuthenticateAsync(BitterAuthRequest authRequest);
    Task<GetUserProfileResponse> GetUserProfileAsync(GetUserProfileRequest userProfileRequest);
    Task<MatchResponse> MatchAsync(MatchRequest matchRequest);
    Task<MatchStatusResponse> GetMatchStatusAsync(MatchStatusRequest matchStatusRequest);
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

    public async Task<MatchResponse> MatchAsync(MatchRequest matchRequest)
    {
        Metadata? headers = string.IsNullOrEmpty(_token) ? null : new Metadata
        {
            { "Authorization", $"Bearer {_token}" },
        };
        var res = await _lobbyClient.MatchAsync(new Slothy.Protocol.UserLobby.MatchRequest(), headers);

        return new();
    }

    public async Task<MatchStatusResponse> GetMatchStatusAsync(MatchStatusRequest matchStatusRequest)
    {
        Metadata? headers = string.IsNullOrEmpty(_token) ? null : new Metadata
        {
            { "Authorization", $"Bearer {_token}" },
        };
        var res = await _lobbyClient.GetMatchStatusAsync(new Slothy.Protocol.UserLobby.GetMatchStatusRequest(), headers);

        return new MatchStatusResponse
        {
            IpAddress = res.IpAddress,
            Port = res.Port,
        };
    }
}
