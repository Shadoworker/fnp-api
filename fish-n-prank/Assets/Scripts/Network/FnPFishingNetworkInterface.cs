using UnityEngine;
using Mirror;

public class FnPFishingNetworkInterface : NetworkBehaviour
{
    #region Singleton
    public static FnPFishingNetworkInterface Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
    #endregion

    #region SERVER
    [Command(requiresAuthority = false)]
    public void CmdStartFishing()
    {
        // server choses a random fish
        FishSO fishSO = GameStateManager.FishesManager.GetRandomFish();

        Debug.Log($"CmdStartFishing: Server chose {fishSO.name}");
        TargetSetBitingFish(fishSO);
    }
    #endregion

    #region CLIENT
    [TargetRpc]
    private void TargetSetBitingFish(FishSO _fishSO)
    {
        FishingController.Instance.StartFishingGameplay(_fishSO);
    }
    #endregion


}
