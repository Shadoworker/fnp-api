using UnityEngine;
using Mirror;

public class FnPFishingNetworkInterface : NetworkBehaviour
{

    #region SERVER
    [Command(requiresAuthority = false)]
    public void CmdStartFishing()
    {
        // server choses a random fish
        FishSO fishSO = GameStateManager.FishesManager.GetRandomFish();

        Debug.Log($"CmdStartFishing: Server chose fish {fishSO.name}");
        NetworkIdentity clientIdentity = GetComponent<NetworkIdentity>();
        TargetSetBitingFish(clientIdentity.connectionToClient, fishSO);
    }
    #endregion

    #region CLIENT
    [Client]
    public void RequestStartFishing()
    {
        CmdStartFishing();
    }

    [TargetRpc]
    private void TargetSetBitingFish(NetworkConnection target, FishSO _fishSO)
    {
        FishingController.Instance.StartFishingGameplay(_fishSO);
    }
    #endregion


}
