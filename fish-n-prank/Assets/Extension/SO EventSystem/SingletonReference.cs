using UnityEngine;

public class SingletonReference : MonoBehaviour
{
    public GameStateManager m_gamestateManager;
    private void Awake()
    {
        m_gamestateManager.Init();
    }
}
