using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Chunk : MonoBehaviour
{
    private BoxCollider boxCollider;

    public static List<Chunk> primaryChunks = new List<Chunk>();

    public List<Chunk> neighbor_chunks = new List<Chunk>();
    public List<string> neighbor_strings = new List<string>();

    public string fileLocation;

    private void Awake()
    {
        if (gameObject.TryGetComponent<BoxCollider>(out BoxCollider collider))
        {
            boxCollider = collider;
            boxCollider.size = new Vector3(1, ChunkLoader.chunkVerticalSize, 1);
            boxCollider.center = new Vector3(0, ChunkLoader.chunkVerticalSize * 0.5f, 0);
        }

        gameObject.transform.parent.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
            ActivateChunks(this);
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
            DisableChunks(this);
    }

    public void ActivateChunks(Chunk newChunk)
    {
        //  used for first chunk activation
        if (!newChunk.gameObject.activeInHierarchy)
        {
            newChunk.gameObject.transform.parent.gameObject.SetActive(true);
            Load(newChunk);
        }

        {
            //  loop through all possible neighbors
            foreach (string chunk_string in newChunk.neighbor_strings)
            {
                //  filters out invalid chunks and already initialized chunks
                if (File.Exists("./chunkInfo/" + chunk_string + ".txt") && !newChunk.neighbor_chunks.Any(x => x.gameObject.transform.parent.gameObject.name == chunk_string))
                {
                    string readInfo = chunk_string.Substring(chunk_string.IndexOf('(') + 1);
                    readInfo = readInfo.Trim(')');

                    string[] readPos = readInfo.Split(',');

                    Vector3 pos = Vector3.zero;
                    pos.x = float.Parse(readPos[0]) * 4;
                    pos.z = float.Parse(readPos[1]) * 4;

//                    Chunk new_chunk = ((GameObject)GameObject.Instantiate(ChunkLoader.chunk_bundle.LoadAsset("Chunk_Prefab"), GameObject.Find("ChunkLoader").transform)).GetComponentInChildren<Chunk>(true);
                    Chunk new_chunk = ((GameObject)GameObject.Instantiate(ChunkAssetManager.chunkPrefab, GameObject.Find("ChunkLoader").transform)).GetComponentInChildren<Chunk>(true);
                    new_chunk.gameObject.transform.parent.localPosition = pos;

                    new_chunk.gameObject.transform.parent.name = "Chunk (" + readPos[0] + "," + readPos[1] + ")";
                    new_chunk.fileLocation = "./chunkInfo/" + new_chunk.gameObject.transform.parent.name + ".txt";
                    new_chunk.gameObject.transform.parent.gameObject.SetActive(true);

                    Load(new_chunk);
                }

            }
        }

        if(!primaryChunks.Contains(newChunk))
            primaryChunks.Add(newChunk);
    }

    public void DisableChunks(Chunk oldChunk)
    {
        primaryChunks.Remove(oldChunk);

        for(int i = 0; i < oldChunk.neighbor_chunks.Count; )
        {
            if (primaryChunks.Contains(oldChunk.neighbor_chunks[i]) || primaryChunks.Any(x => x.neighbor_chunks.Contains(oldChunk.neighbor_chunks[i])))
                i++;
            else
            {
                List<Chunk> tempChunks = new List<Chunk>();

                tempChunks.AddRange(oldChunk.neighbor_chunks[i].neighbor_chunks);

                Chunk removeChunk = oldChunk.neighbor_chunks[i];

                foreach (Chunk neighbor in tempChunks)
                    neighbor.neighbor_chunks.Remove(removeChunk);

                Save(removeChunk);
                oldChunk.neighbor_chunks.Remove(removeChunk);
                Destroy(removeChunk.gameObject.transform.parent.gameObject);
            }
        }

        foreach (Chunk oldNeighbor in oldChunk.neighbor_chunks)
        {
            if (!primaryChunks.Contains(oldNeighbor) && !primaryChunks.Any(x => x.neighbor_chunks.Contains(oldNeighbor)))
            {
                List<Chunk> tempChunks = new List<Chunk>();
                
                tempChunks.AddRange(oldNeighbor.neighbor_chunks);

                foreach (Chunk neighbor in tempChunks)
                    neighbor.neighbor_chunks.Remove(oldNeighbor);

                Save(oldNeighbor);
                Destroy(oldNeighbor.gameObject.transform.parent.gameObject);
            }
        }
    }

    public void Save(Chunk chunk)
    {
        List<string> saveInfo = new List<string>();

        Collider[] colliders = Physics.OverlapBox(chunk.gameObject.transform.position + 0.5f * ChunkLoader.chunkVerticalSize * Vector3.up, new Vector3(transform.localScale.x * 0.5f, ChunkLoader.chunkVerticalSize * 0.5f, transform.localScale.z * 0.5f));
        
        List<GameObject> gameObjects = new List<GameObject>();

        foreach (var c in colliders.Where(x => !x.CompareTag("chunk") && !x.CompareTag("Player")))
        {
            Vector3 relativeObjectPos = chunk.boxCollider.transform.InverseTransformDirection(c.gameObject.transform.position);

            if (chunk.boxCollider.bounds.Contains(relativeObjectPos))
                gameObjects.Add(c.gameObject);
        }

        foreach(var go in gameObjects)
        {
            int readinfo = 0;

            List<string> objectInfo = new List<string>();

            if (go.TryGetComponent<Transform>(out Transform objectTransform))
            {
                readinfo |= ChunkLoader.BitFlag_Transform;

                string objectName = go.name;
                if (objectName.Contains("(Clone)"))
                    objectName = objectName.Replace("(Clone)", "");

                objectInfo.Add(objectName);

                objectInfo.AddRange(objectTransform.WriteOutTransform());
            }

            if (go.TryGetComponent<Rigidbody>(out Rigidbody objectRigidbody))
            {
                readinfo |= ChunkLoader.BitFlag_RigidBody;

                objectInfo.AddRange(objectRigidbody.WriteOutRigidbody());
            }


            saveInfo.Add(readinfo.ToString());
            saveInfo.AddRange(objectInfo);
        }

        if (File.Exists(chunk.fileLocation))
            File.Delete(chunk.fileLocation);

        File.WriteAllLines(chunk.fileLocation, saveInfo);

        for(int i = gameObjects.Count; i > 0; i--)
            Destroy(gameObjects[i - 1]);
    }

    public void Load(Chunk chunk)
    {
        string workingFilePath = Application.dataPath + "/.." + chunk.fileLocation.Substring(1);

        if (!File.Exists(workingFilePath)) return;

        List<string> chunkInfo = File.ReadLines(workingFilePath).ToList();

        string workingChunkLocation = chunk.gameObject.transform.parent.name.Substring(chunk.gameObject.transform.parent.name.IndexOf('(') + 1);
        workingChunkLocation = workingChunkLocation.Trim(')');

        string[] workingChunkPos = workingChunkLocation.Split(',');

        chunk.neighbor_strings.Clear();

        chunk.neighbor_strings.Add("Chunk (" + (int.Parse(workingChunkPos[0]) - 1) + "," + (int.Parse(workingChunkPos[1]) + 1) + ")");
        chunk.neighbor_strings.Add("Chunk (" + (int.Parse(workingChunkPos[0]) - 1) + "," + (int.Parse(workingChunkPos[1])) + ")");
        chunk.neighbor_strings.Add("Chunk (" + (int.Parse(workingChunkPos[0]) - 1) + "," + (int.Parse(workingChunkPos[1]) - 1) + ")");
        chunk.neighbor_strings.Add("Chunk (" + (int.Parse(workingChunkPos[0])) + "," + (int.Parse(workingChunkPos[1]) + 1) + ")");
        chunk.neighbor_strings.Add("Chunk (" + (int.Parse(workingChunkPos[0])) + "," + (int.Parse(workingChunkPos[1]) - 1) + ")");
        chunk.neighbor_strings.Add("Chunk (" + (int.Parse(workingChunkPos[0]) + 1) + "," + (int.Parse(workingChunkPos[1]) + 1) + ")");
        chunk.neighbor_strings.Add("Chunk (" + (int.Parse(workingChunkPos[0]) + 1) + "," + (int.Parse(workingChunkPos[1])) + ")");
        chunk.neighbor_strings.Add("Chunk (" + (int.Parse(workingChunkPos[0]) + 1) + "," + (int.Parse(workingChunkPos[1]) - 1) + ")");

        foreach (string connectionString in chunk.neighbor_strings)
        {
            GameObject attemptedConnection = GameObject.Find(connectionString);

            if (attemptedConnection)
            {
                if(!chunk.neighbor_chunks.Contains(attemptedConnection.GetComponentInChildren<Chunk>()))
                    chunk.neighbor_chunks.Add(attemptedConnection.GetComponentInChildren<Chunk>());
                
                if(!attemptedConnection.GetComponentInChildren<Chunk>().neighbor_chunks.Contains(chunk))
                    attemptedConnection.GetComponentInChildren<Chunk>().neighbor_chunks.Add(chunk);
            }
        }

        if(ChunkLoader.sceneBundle && ChunkLoader.sceneBundle.Contains("Prefab_" + chunk.gameObject.transform.parent.gameObject.name))
            Instantiate(ChunkLoader.sceneBundle.LoadAsset("Prefab_" + chunk.gameObject.transform.parent.gameObject.name), chunk.gameObject.transform);


        if (ChunkAssetManager.testPrefabs.Any(x => x.name == "Prefab_" + chunk.gameObject.transform.parent.gameObject.name))
        {
            Instantiate(ChunkAssetManager.testPrefabs.First(x => x.name == "Prefab_" + chunk.gameObject.transform.parent.gameObject.name), chunk.gameObject.transform.parent);
        }

        int readLine = 0;
        for (; readLine < chunkInfo.Count;)
        {
            int bitmap = int.Parse(chunkInfo[readLine]);
            readLine++;

//            GameObject go = (GameObject)Instantiate(ChunkLoader.assetbundle.LoadAsset(chunkInfo[readLine]));
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            readLine++;

            if ((bitmap & ChunkLoader.BitFlag_Transform) != 0)
                go.transform.ReadInComponent(chunkInfo, ref readLine);

            if ((bitmap & ChunkLoader.BitFlag_RigidBody) != 0)
                go.GetComponent<Rigidbody>().ReadInComponent(chunkInfo, ref readLine);
        }
    }
    private void OnApplicationQuit()
    {
        Save(this);
    }
}
