using UnityEditor;
using UnityEngine;

public class ScaleFreezer : MonoBehaviour {
    [MenuItem("Tools/Freeze Level Mesh Scales")]
    static void FreezeScales() {
        foreach (GameObject obj in Selection.gameObjects) {
            Transform[] all = obj.GetComponentsInChildren<Transform>();
            foreach (Transform t in all) {
                MeshFilter mf = t.GetComponent<MeshFilter>();
                if (mf != null && mf.sharedMesh != null) {
                    Mesh mesh = Instantiate(mf.sharedMesh);
                    Vector3[] verts = mesh.vertices;
                    for (int i = 0; i < verts.Length; i++) {
                        verts[i] = Vector3.Scale(verts[i], t.localScale);
                    }
                    mesh.vertices = verts;
                    mesh.RecalculateBounds();
                    mesh.RecalculateNormals();
                    mf.sharedMesh = mesh;
                    t.localScale = Vector3.one;
                }
            }
        }
    }
}