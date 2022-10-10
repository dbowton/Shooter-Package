using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Wildlife : AI
{
    public State state = State.Passive;
    public enum State
    {
        Passive,
        Curious,
        Hostile,
        Fleeing        
    }

    [SerializeField] [Range(0, 100)] float selfPreservation;
    [SerializeField] [Range(0, 100)] float hostility;
    [SerializeField] [Range(0, 100)] float curiosity;

    [SerializeField] float teritory = 50f;

    Timer stateTimer;

    public override void update(float dt)
    {
        base.update(dt);

        float distance = Vector3.Distance(GameObject.Find("Player").transform.position, transform.position);

        print("state: " + state.ToString());

        switch (state)
        {
            case State.Passive:
                break;
            case State.Curious:
                break;
            case State.Hostile:
                break;
            case State.Fleeing:
                break;
            default:
                break;
        }
    }
}
