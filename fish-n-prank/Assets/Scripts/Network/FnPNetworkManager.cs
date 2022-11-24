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

        // Server choses a random player skin
        //GameStateManager.CharactersManager.SetPlayerSkin(conn.identity.gameObject, CHARACTER.RANDOM.ToString());

        conn.identity.gameObject.GetComponent<NetworkPlayer>().SetUseCharacter(CHARACTER.RANDOM.ToString());

        // Hack: force Grumpycat skin (for harcoded prefab NetworkdAnimator)
        //GameStateManager.CharactersManager.SetPlayerSkin(conn.identity.gameObject, "GRUMPY_CAT");
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
