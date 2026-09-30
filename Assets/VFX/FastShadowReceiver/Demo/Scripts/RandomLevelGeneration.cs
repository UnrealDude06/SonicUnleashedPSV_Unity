using UnityEngine;

namespace FastShadowReceiver.Demo {
	public class RandomLevelGeneration : MonoBehaviour {
		public GameObject[] m_prefabObjects;
		public int m_objectCount = 20;
		public Bounds m_positionRange;
	
		private BinaryMeshTree m_meshTree;

		void Awake () {
			for (int i = 0; i < m_objectCount; ++i) {
				GameObject prefab = m_prefabObjects[Random.Range(0, m_prefabObjects.Length)];
				GameObject go = GameObject.Instantiate(prefab) as GameObject;
				go.transform.parent = transform;
				go.layer = gameObject.layer;
				Vector3 pos = new Vector3(Random.Range(m_positionRange.min.x, m_positionRange.max.x),
				                          Random.Range(m_positionRange.min.y, m_positionRange.max.y),
				                          Random.Range(m_positionRange.min.z, m_positionRange.max.z));
				go.transform.position = pos;
			}
			// create & build a mesh tree
			m_meshTree = ScriptableObject.CreateInstance<BinaryMeshTree>();
			// setting build parameters
			m_meshTree.srcMesh = gameObject;
			m_meshTree.layerMask = (1 << gameObject.layer);
			m_meshTree.excludeRenderTypes = new string[] {"Transparent"};
			m_meshTree.scaledOffset = 1;
			// start build in background thread.
			m_meshTree.AsyncBuild();
		}

		void Start()
		{
			foreach (MeshShadowReceiver receiver in ProjectorManager.Instance.receivers) {
				receiver.meshTransform = transform;
				receiver.meshTree = m_meshTree;
			}
			// If it is acceptable that shadows are invisible until the mesh building is completed, you don't need to wait for the mesh tree to be built here.
			// m_meshTree.WaitForBuild();
		}

		void OnDestroy()
		{
			DestroyObject(m_meshTree);
		}
	}	
}
