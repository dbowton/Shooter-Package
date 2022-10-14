using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimplePlayer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            GameObject.Find("Wildlife").GetComponent<Health>().Damage(new Assets.Scripts.Items.Weapons.Damage(Assets.Scripts.Items.Weapons.DamageType.SLASHING, 1, 0));
        }
    }
}
