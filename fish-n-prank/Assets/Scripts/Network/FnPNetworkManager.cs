using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class FnPNetworkManager : NetworkManager
{
    public override void OnClientConnect()
    {
        base.OnClientConnect();

        Debug.Log("Client has connected to a server!");
    }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);

        Debug.Log($"A new player was added! There an now {numPlayers} connected players.");
    }
}
