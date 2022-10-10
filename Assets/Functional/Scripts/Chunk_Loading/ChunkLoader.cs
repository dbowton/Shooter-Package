using System.IO;
using UnityEngine;

public class ChunkLoader : MonoBehaviour
{
    public const int BitFlag_Transform = (1 << 0);
    public const int BitFlag_RigidBody = (1 << 1);
    public const int BitFlag_Collider  = (1 << 2);
    public const int BitFlag_Renderer  = (1 << 3);
    public const int BitFlag_Enemy     = (1 << 4);
    public const int BitFlag_Health    = (1 << 5);

    public string fileLocation = "./chunkInfo/startup.txt";

//    public static AssetBundle assetbundle;
    public static AssetBundle sceneBundle;

    //    public static AssetBundle chunk_bundle;

    public static float chunkVerticalSize = 50;

    private void Start()
    {
//        assetbundle = AssetBundle.LoadFromFile("C:\\Users\\dbowton\\Downloads\\GameEngines\\UnityProjects\\HexLoader\\AssetBundles\\cubes");
        //        chunk_bundle = AssetBundle.LoadFromFile("C:\\Users\\dbowton\\Downloads\\GameEngines\\UnityProjects\\HexLoader\\AssetBundles\\chunks");

        string readInfo; 
        if (File.Exists(fileLocation))
            readInfo = File.ReadAllLines(fileLocation)[0];
        else
            readInfo = "Chunk (0,0)";

        //  Seperates coords from Chunk Name
        string[] readPos = readInfo.Substring(readInfo.IndexOf('(') + 1).Trim(')').Split(',');

        Vector3 pos = Vector3.zero;
        pos.x = float.Parse(readPos[0]) * 4;
        pos.z = float.Parse(readPos[1]) * 4;

//        Chunk startingChunk = ((GameObject)GameObject.Instantiate(chunk_bundle.LoadAsset("Chunk_Prefab"), GameObject.Find("ChunkLoader").transform)).GetComponent<Chunk>();
        
        Chunk startingChunk = Instantiate(ChunkAssetManager.chunkPrefab, GameObject.Find("ChunkLoader").transform).GetComponentInChildren<Chunk>(true);

        startingChunk.gameObject.transform.parent.localPosition = pos;
        startingChunk.gameObject.transform.parent.name = "Chunk (" + readPos[0] + "," + readPos[1] + ")";
        startingChunk.fileLocation = "./chunkInfo/" + startingChunk.gameObject.transform.parent.name + ".txt";

        startingChunk.ActivateChunks(startingChunk);
    }

    private void OnApplicationQuit()
    {
        if(Chunk.primaryChunks.Count > 0)
            File.WriteAllLines(fileLocation, new string[]{ Chunk.primaryChunks[0].gameObject.transform.parent.name });
        else
            File.WriteAllLines(fileLocation, new string[] { "Chunk (0,0)" });
    }
}
