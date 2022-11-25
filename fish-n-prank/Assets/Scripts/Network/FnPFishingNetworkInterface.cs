using UnityEngine;
using Mirror;

public class FnPFishingNetworkInterface : NetworkBehaviour
{
    public void RequestStartFishing()
    {
        CmdStartFishing();
    }

    #region SERVER
    [Command(requiresAuthority = false)]
    public void CmdStartFishing()
    {
        // server choses a random fish
        FishSO fishSO = GameStateManager.FishesManager.GetRandomFish();

        Debug.Log($"CmdStartFishing: Server chose {fishSO.name}");
        NetworkIdentity opponentIdentity = GetComponent<NetworkIdentity>();
        TargetSetBitingFish(opponentIdentity.connectionToClient, fishSO);
    }
    #endregion

    #region CLIENT
    [TargetRpc]
    private void TargetSetBitingFish(NetworkConnection target, FishSO _fishSO)
    {
        Debug.LogWarning("TargetSetBitingFish will start fishing gameplay.");
        FishingController.Instance.StartFishingGameplay(_fishSO);
    }
    #endregion


}
