using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimplePlayer : MonoBehaviour
{
    public SimpleInventory inventory;
    public SimpleMovement movement;


    public (bool hit, RaycastHit hitInfo) LookingAt()
    {
        bool hit = Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hitInfo);
        return (hit, hitInfo);
    }

    // Update is called once per frame
    public void update(float dt)
    {
        movement.update(dt);

        if(Input.GetKeyDown(KeyCode.Space))
        {
            GameObject.Find("Wildlife").GetComponent<Health>().Damage(new Assets.Scripts.Items.Weapons.Damage(Assets.Scripts.Items.Weapons.DamageType.SLASHING, 1, 0));
        }
    }
}
