using UnityEngine;

namespace Kayfo
{
	public class HideOnPlatform : MonoBehaviour
	{
		public bool m_iOS = false;
		public bool m_android = false;

		// Use this for initialization
		void Awake()
		{
#if UNITY_IOS
			this.gameObject.SetActive(!m_iOS);
#elif UNITY_ANDROID
			this.gameObject.SetActive(!m_android);
#endif
		}
	}
}