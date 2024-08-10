using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace Moshi.MMO;

public class GameHub : Hub
{
    private static ConcurrentDictionary<string, PlayerInfo> _players = new ConcurrentDictionary<string, PlayerInfo>();

    public override async Task OnConnectedAsync()
    {
        string playerId = Context.ConnectionId;
        var playerInfo = new PlayerInfo { X = 400, Y = 300 }; // Start at center
        _players.TryAdd(playerId, playerInfo);

        await Clients.All.SendAsync("PlayerJoined", playerId, playerInfo.X, playerInfo.Y);
        await Clients.Caller.SendAsync("GetAllPlayers", _players);

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        string playerId = Context.ConnectionId;
        _players.TryRemove(playerId, out _);

        await Clients.All.SendAsync("PlayerLeft", playerId);

        await base.OnDisconnectedAsync(exception);
    }

    public async Task MovePlayer(int x, int y)
    {
        string playerId = Context.ConnectionId;
        if (_players.TryGetValue(playerId, out PlayerInfo playerInfo))
        {
            playerInfo.X = x;
            playerInfo.Y = y;
            await Clients.Others.SendAsync("PlayerMoved", playerId, x, y);
        }
    }
}

public class PlayerInfo
{
    public int X { get; set; }
    public int Y { get; set; }
}
