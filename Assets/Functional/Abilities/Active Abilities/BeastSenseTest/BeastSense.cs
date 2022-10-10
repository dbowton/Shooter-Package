using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BeastSense : ActiveAbility
{
    Camera secondaryCamera;

    private Timer beastTimer;

    List<SavedObject> objects;
    [SerializeField] LayerMask layer;

    [SerializeField] List<AbilityStats> levelStats;

    [System.Serializable]
    class AbilityStats : GeneralStats
    {
        [Header("BeastSense Ability Info")]
        public float radius;
        public List<RecognizedType> recognizedTypes;
    }

    struct SavedObject
    {
        public GameObject go;
        public Material material;
        public LayerMask layer;
    }

    [System.Serializable]
    struct RecognizedType
    {
        public string type;
        public Material renderedMaterial;
    }

    void Start()
    {
        GameObject go = new GameObject();
        go.name = "beast camera";

        go.transform.parent = Camera.main.transform;
        go.transform.localPosition = Vector3.zero;
        go.transform.localEulerAngles = Vector3.zero;
        go.transform.localScale = Vector3.one;
        
        secondaryCamera = go.AddComponent<Camera>();
        secondaryCamera.clearFlags = CameraClearFlags.Depth;
        secondaryCamera.cullingMask = layer.value;
        secondaryCamera.fieldOfView = Camera.main.fieldOfView;

        timer = new Timer();
        beastTimer = new Timer(Deactivate);

        objects = new List<SavedObject>();

        Innit();
    }

    public void Innit()
    {
        if (level == 0) return;

        abilityImage.sprite = levelStats[level - 1].lvlSprite;

        timer.Modify(levelStats[level - 1].cooldown);
        beastTimer.Modify(levelStats[level - 1].duration);
    }

    void Deactivate()
    {
        foreach (var set in objects)
        {
            if(set.go)
            {
                set.go.GetComponent<Renderer>().material = set.material;
                set.go.layer = set.layer.value;
            }
        }

        objects.Clear();
        timer.Resume();

        if (abilityBackground.gameObject.TryGetComponent<Image>(out Image image))
        {
            image.color = chargingColor;
        }
    }

    void Update()
    {
        if (level == 0) return;

        timer.Update();
        beastTimer.Update();

        if (beastTimer.IsOver && timer.IsOver)
        {
            if(abilityBackground.gameObject.TryGetComponent<Image>(out Image image))
            {
                image.color = readyColor;
            }           
        }

        Vector3 backgroundScale = abilityBackground.localScale;
        if (!beastTimer.IsOver)
        {
            Pulse();

            backgroundScale.y = 1 - Mathf.Min(beastTimer.GetElapsed, 1);
        }
        else
        {
            backgroundScale.y = Mathf.Min(timer.GetElapsed, 1);
        }
        
        abilityBackground.localScale = backgroundScale;

        if (Input.GetKey(button))
        {
            Activate();
        }
    }

    void Pulse()
    {
        List<Collider> colliders = Physics.OverlapSphere(transform.position, levelStats[level - 1].radius).ToList();

        List<SavedObject> newSavedObjects = new List<SavedObject>();

        foreach (var c in colliders)
        {
            bool newFoundObject = true;

            for(int i = 0; i < objects.Count; i++)
            {
                if (c.gameObject.Equals(objects[i].go))
                {
                    newFoundObject = false;
                    newSavedObjects.Add(objects[i]);
                    objects.RemoveAt(i);
                    break;
                }
            }

            if (!newFoundObject) continue;
                

            if (c.gameObject.TryGetComponent<Renderer>(out Renderer renderer))
            {
                foreach (var t in levelStats[level - 1].recognizedTypes)
                {
                    if (c.gameObject.TryGetComponent(System.Type.GetType(t.type), out _))
                    {
                        SavedObject newObject = new SavedObject();
                        newObject.go = c.gameObject;
                        newObject.material = renderer.material;
                        newObject.layer = c.gameObject.layer;

                        newSavedObjects.Add(newObject);

                        c.gameObject.layer = (int)Mathf.Log(layer.value, 2);
                        c.gameObject.GetComponent<Renderer>().material = t.renderedMaterial;

                        break;
                    }
                }
            }
        }

        foreach(var set in objects)
        {
            if (set.go)
            {
                set.go.GetComponent<Renderer>().material = set.material;
                set.go.layer = set.layer.value;
            }
        }

        objects = newSavedObjects;
    }


    void Activate()
    {
        if (!(beastTimer.IsOver && timer.IsOver)) return;

        beastTimer.Reset();
        timer.Reset();
        timer.Pause();

        if (abilityBackground.gameObject.TryGetComponent<Image>(out Image image))
        {
            image.color = activeColor;
        }

        secondaryCamera.farClipPlane = levelStats[level - 1].radius * 2;

        Pulse();
    }
}
