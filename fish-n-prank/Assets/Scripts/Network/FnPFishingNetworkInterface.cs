using UnityEngine;
using Mirror;
using System;
using System.Linq;
using System.Collections.Generic;

public class FnPFishingNetworkInterface : NetworkBehaviour
{

    #region SERVER
    [Command(requiresAuthority = false)]
    public void CmdStartFishing()
    {
        // server choses a random fish
        // TODO: Create variables for areas and rods
        List<AvailabilityArea> areas = Enum.GetValues(typeof(AvailabilityArea)).Cast<AvailabilityArea>().ToList();
        List<FishingRod> rods = Enum.GetValues(typeof(AvailabilityArea)).Cast<FishingRod>().ToList();
        Offshore offshore = new Offshore();
        SelectedFish fishSO = GameStateManager.FishesManager.GetRandomFish(areas, rods, offshore);
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
    private void TargetSetBitingFish(NetworkConnection target, SelectedFish _fishSO)
    {
        FishingManager.Instance.StartFishingGameplay(_fishSO);
    }
    #endregion


}
