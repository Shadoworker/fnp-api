using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
  
    public void GetFish()
    {
        APIManager.instance.GetFish("l3v3l/getFish");
    }

    public void UpdatePlayerResources()
    {

        fnpUpdateBody _body = new fnpUpdateBody();
        _body.player_id = "0d5356d5-1c67-4e6b-bc8f-30571af18382";
        _body.game_id = "fc778f60-980a-406a-a420-1bada05b182f";
        
        _body.resources = new List<fnpResourceBody>(){
            new fnpResourceBody(){resource_id = "0cd2ffa3-7fa6-4d52-8644-ad1e7dad70a1" , amount = 1}
            };

        APIManager.instance.UpdatePlayerResources("l3v3l/updatePlayerResource", JsonUtility.ToJson(_body));

    }
}
