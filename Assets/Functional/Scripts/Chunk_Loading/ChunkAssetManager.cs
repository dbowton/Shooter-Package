using System.Collections.Generic;
using UnityEngine;


//  can be removed once scene bundle is built
public class ChunkAssetManager : MonoBehaviour
{
    [SerializeField] List<GameObject> testPrefabsInput = new List<GameObject>();

    public static List<GameObject> testPrefabs = new List<GameObject>();

    [SerializeField] GameObject chunkPrefabInput;
    public static GameObject chunkPrefab;

    private void Awake()
    {
        testPrefabs = testPrefabsInput;
        chunkPrefab = chunkPrefabInput;
    }
}
