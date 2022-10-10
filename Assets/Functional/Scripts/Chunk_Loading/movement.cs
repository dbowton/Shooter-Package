using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class movement : MonoBehaviour
{
    const string fileLocation = "./playerInfo/playerInfo.txt";

    [SerializeField] float commonLocalScale = 1;
    [SerializeField] float commonYpos = 0;

    private void Start()
    {
        Vector3 pos = new Vector3(0, 0.5f, 0);
        Vector3 rot = Vector3.zero;
        Vector3 scl = Vector3.one * 0.5f;

        if (File.Exists(fileLocation))
        {
            List<string> readInfo = File.ReadAllLines(fileLocation).ToList();

            pos.x = float.Parse(readInfo[0]);
            pos.y = float.Parse(readInfo[1]);
            pos.z = float.Parse(readInfo[2]);

            rot.x = float.Parse(readInfo[3]);
            rot.y = float.Parse(readInfo[4]);
            rot.z = float.Parse(readInfo[5]);

            scl.x = float.Parse(readInfo[6]);
            scl.y = float.Parse(readInfo[7]);
            scl.z = float.Parse(readInfo[8]);
        }

        gameObject.transform.position = pos;
        gameObject.transform.rotation = Quaternion.Euler(rot);
        gameObject.transform.localScale = scl;

        gameObject.GetComponent<Renderer>().material.color = Color.green;
    }

    private void OnApplicationQuit()
    {
        if (File.Exists(fileLocation))
            File.Delete(fileLocation);

        List<string> writeInfo = new List<string>();
        writeInfo.Add(transform.position.x.ToString());
        writeInfo.Add(transform.position.y.ToString());
        writeInfo.Add(transform.position.z.ToString());

        writeInfo.Add(transform.rotation.eulerAngles.x.ToString());
        writeInfo.Add(transform.rotation.eulerAngles.y.ToString());
        writeInfo.Add(transform.rotation.eulerAngles.z.ToString());

        writeInfo.Add(transform.localScale.x.ToString());
        writeInfo.Add(transform.localScale.y.ToString());
        writeInfo.Add(transform.localScale.z.ToString());

        File.WriteAllLines(fileLocation, writeInfo);
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.D))
            transform.position = transform.position + 3 * Time.deltaTime * Vector3.right;

        if (Input.GetKey(KeyCode.A))
            transform.position = transform.position + 3 * Time.deltaTime * Vector3.left;

        if (Input.GetKey(KeyCode.S))
            transform.position = transform.position + 3 * Time.deltaTime * Vector3.back;

        if (Input.GetKey(KeyCode.W))
            transform.position = transform.position + 3 * Time.deltaTime * Vector3.forward;
    
        if(Input.GetKey(KeyCode.Q))
            transform.rotation *= Quaternion.Euler(Vector3.up * 0.5f);

        if (Input.GetKey(KeyCode.E))
            transform.rotation *= Quaternion.Euler(Vector3.down * 0.5f);

        if (Input.GetKeyDown(KeyCode.UpArrow))
            commonYpos += 1;

        if (Input.GetKeyDown(KeyCode.DownArrow))
            commonYpos -= 1;

        if (Input.GetKeyDown(KeyCode.LeftArrow))
            commonLocalScale *= 0.9f;

        if (Input.GetKeyDown(KeyCode.RightArrow))
            commonLocalScale *= 1.1f;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            //          GameObject go = (GameObject)GameObject.Instantiate(ChunkLoader.assetbundle.LoadAsset("Test 1"), transform.position + Vector3.up * commonYpos, transform.rotation);
            //          go.transform.localScale = Vector3.one * commonLocalScale;

            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.transform.position = transform.position + Vector3.up * commonYpos;
            go.transform.rotation = transform.rotation;


            go.transform.localScale = Vector3.one * commonLocalScale;
        }
    }
}
