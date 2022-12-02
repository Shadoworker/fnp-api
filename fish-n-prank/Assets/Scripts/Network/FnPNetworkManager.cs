using UnityEngine;
using Mirror;

public class FnPNetworkManager : NetworkManager
{
    #region SERVER
    public override void OnServerAddPlayer(NetworkConnectionToClient _conn) // TODO: try remove conn
    {
        base.OnServerAddPlayer(_conn);
        Debug.Log($"A new player was added! There an now {numPlayers} connected players.");

        // Server choses a random player skin
        _conn.identity.gameObject.GetComponent<NetworkPlayer>().SetUseCharacter(CHARACTER.RANDOM.ToString());
    }
    #endregion

    #region CLIENT
    public override void OnClientConnect()
    {
        base.OnClientConnect();

        Debug.Log("Client has connected to a server!");
    }
    #endregion

}
