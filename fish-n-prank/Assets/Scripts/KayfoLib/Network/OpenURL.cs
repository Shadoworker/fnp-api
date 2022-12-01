using UnityEngine;

namespace Kayfo
{
    public class OpenURL : MonoBehaviour
    {
        public string m_url;

        public void Open()
        {
            Application.OpenURL(m_url);
        }
    }
}
