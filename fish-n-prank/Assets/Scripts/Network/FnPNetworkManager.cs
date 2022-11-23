using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class FnPNetworkManager : NetworkManager
{
    #region SERVER
    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);
        Debug.Log($"A new player was added! There an now {numPlayers} connected players.");

        // Spawn player skin
        //GameObject playerSkin = GameStateManager.CharactersManager.SetPlayerSkin(conn.identity.gameObject, CHARACTER.RANDOM.ToString());
        //GameStateManager.CharactersManager.SetPlayerSkin(conn.identity.gameObject, CHARACTER.RANDOM.ToString());

        // Hack: force Grumpycat skin (for harcoded prefab NetworkdAnimator)
        GameStateManager.CharactersManager.SetPlayerSkin(conn.identity.gameObject, "GRUMPY_CAT");

        //NetworkServer.Spawn(playerSkin, conn.identity.gameObject);
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
