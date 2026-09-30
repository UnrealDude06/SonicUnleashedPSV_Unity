using UnityEngine;

namespace FastShadowReceiver.Demo {
	public class SceneController : MonoBehaviour {
		public string[] m_scenes = new string[] {
			"Projector - Plane",
			"Projector - Mesh",
			"Projector - Terrain",
			"Shadowmap - Plane",
			"Shadowmap - Mesh",
			"MultiProjector - Plane",
			"MultiProjector - Mesh",
			"RandomSpawn - Plane",
			"RandomSpawn - Mesh",
			"SpotLights + Fake Shadows",
			"BulletMarks",
			"Multithreaded Raycast",
		};
		private int m_currentScene = -1;
		void Awake () {
			DontDestroyOnLoad(this);
#if UNITY_WEBPLAYER
			if (Application.isWebPlayer) {
				Application.ExternalEval(
					"var p = window.location.search;" +
					"var i = p.indexOf(\"scene=\");" +
					"if (0 <= i) {" +
					"  var j = p.indexOf(\"&\",i);" +
					"  if (0 < j) {" +
					"    p = p.substring(i+6,j);" +
					"  }" +
					"  else {" +
					"    p = p.substring(i+6);" +
					"  }" +
					"  u.getUnity().SendMessage(\"SceneController\", \"LoadScene\", decodeURIComponent(p));" +
					"}"
					);
			}
#endif
		}

		void Start () {
			if (m_currentScene == -1) {
				m_currentScene = 0;
				Application.LoadLevel(m_scenes[0]);
			}
		}

		void LoadScene(string sceneName)
		{
			for (int i = 0; i < m_scenes.Length; ++i) {
				if (string.Compare(sceneName, m_scenes[i], true) == 0) {
					m_currentScene = i;
					Application.LoadLevel(m_scenes[i]);
				}
			}
		}

		void OnGUI () {
			const int sceneNameWidth = 300;
			const int buttonWidth = 100;
			const int totalWidth = 2*buttonWidth + sceneNameWidth;
			const int height = 50;
			const int sceneNameHeight = 24;
			if (GUI.Button(new Rect(0.5f*(Screen.width - totalWidth), Screen.height - height, buttonWidth, height), "Prev")) {
				m_currentScene = (m_currentScene + m_scenes.Length - 1) % m_scenes.Length;
				Application.LoadLevel(m_scenes[m_currentScene]);
			}
			GUI.Box(new Rect(0.5f*(Screen.width - sceneNameWidth), Screen.height - 0.5f*(sceneNameHeight + height), sceneNameWidth, sceneNameHeight), m_scenes[m_currentScene]);
			if (GUI.Button(new Rect(0.5f*(Screen.width - totalWidth) + buttonWidth + sceneNameWidth, Screen.height - height, buttonWidth, height), "Next")) {
				m_currentScene = (m_currentScene + 1) % m_scenes.Length;
				Application.LoadLevel(m_scenes[m_currentScene]);
			}
		}
	}
}
