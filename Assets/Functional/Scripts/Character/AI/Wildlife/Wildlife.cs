using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(NavMeshAgent))]
public class Wildlife : AI
{
    Transform player { get { return GameObject.Find("Player").transform; } }

    NavMeshAgent agent;
    CharacterController controller;

    [SerializeField] [Range(0, 100)] float passiveness;
    [SerializeField] [Range(0, 100)] float curiosity;
    [SerializeField] [Range(0, 100)] float hostility;
    [SerializeField] [Range(0, 100)] float selfPreservation;

    [SerializeField] float teritory = 50f;

    Stopwatch stateWatch;
    [SerializeField] Transform eyes;

    List<(float value, Action action)> stateWeights = new List<(float value, Action action)>();

    Action currentStateFunction;
    float minMoveDistance = 0.25f;

    [SerializeField] float speed = 4.0f;
    [SerializeField] float attackRange = 1.25f;
    Timer attackTimer;
    [SerializeField] float attackIntervals = 0.75f;
    [SerializeField] float runDistance = 2f;
    [SerializeField] float maxAngle = 22.5f;
 
    [SerializeField] List<Collider> bounds;

    [Space(15)]
    [Header("Debug Tools")]
    [SerializeField] bool setPassive = false;
    [SerializeField] bool setCurious = false;
    [SerializeField] bool setHostile = false;
    [SerializeField] bool setFleeing = false;

    #region Passive Properties
    float passiveStateChangeTime = 0;
    float minPassiveStateChangeTime = 2f;
    float maxPassiveStateChangeTime = 2f;

    float nextPassiveChange = 0;
    float minPassiveChangeVal = 0;
    float maxPassiveChangeVal = 1;
    float minPassiveDistanceVal = 4;
    float maxPassiveDistanceVal = 8;
    #endregion
    #region Fleeing Properties
    float endFleeTime = 0;
    float minEndFleeTime = 0.5f;
    float maxEndFleeTime = 1f;

    float fleeHealth = 0;
    #endregion

    private void Awake()
    {
        currentStateFunction = PassiveUpdate;

        Vector4 weights = new Vector4(passiveness, curiosity, hostility, selfPreservation).normalized;

        stateWeights.Add((weights.x, PassiveUpdate));
        stateWeights.Add((weights.y, CuriousUpdate));
        stateWeights.Add((weights.z, HostileUpdate));
        stateWeights.Add((weights.w, FleeingUpdate));

        stateWeights = stateWeights.OrderBy(x => x.value).ToList();
        controller = GetComponent<CharacterController>();
        agent = GetComponent<NavMeshAgent>();

        stateWatch = new Stopwatch();
        attackTimer = new Timer(attackIntervals);

        fleeHealth = health.CurrentHealth;

        agent.speed = speed;
        minMoveDistance = controller.radius * 1.1f;
        SetTargetLocation(transform.position);
    }

    public override void update(float dt)
    {
        base.update(dt);
        stateWatch.Update(dt);
        attackTimer.Update(dt);
        currentStateFunction.Invoke();
        print(currentStateFunction.Method.Name);

        if(setPassive)
        {
            ChangeState(PassiveUpdate);
            setPassive = false;
        }
        if (setCurious)
        {
            ChangeState(CuriousUpdate);
            setCurious = false;
        }
        if (setHostile)
        {
            ChangeState(HostileUpdate);
            setHostile = false;
        }
        if (setFleeing)
        {
            ChangeState(FleeingUpdate);
            setFleeing = false;
        }
    }

    void ChangeState(Action newState)
    {
        SetTargetLocation(transform.position);
        stateWatch.Reset();
        currentStateFunction = newState;
    }

    void PassiveUpdate() 
    {
        if(NoticesPlayer())
        {
            if(stateWatch.GetElapsed >= passiveStateChangeTime)
            {
                passiveStateChangeTime = stateWatch.GetElapsed + UnityEngine.Random.Range(minPassiveStateChangeTime, maxPassiveStateChangeTime);
                float ranNum = UnityEngine.Random.Range(0, 1f);
                foreach(var (value, action) in stateWeights)
                {
                    if (ranNum < value)
                    {
                        ChangeState(action);
                        break;
                    }
                }
            }
        }

        if (Vector3.Distance(transform.position, agent.destination) >= minMoveDistance)
        {
            nextPassiveChange += Time.deltaTime;
        } 
        else if(stateWatch.GetElapsed > nextPassiveChange)
        {
            nextPassiveChange = stateWatch.GetElapsed + (UnityEngine.Random.Range(minPassiveChangeVal, maxPassiveChangeVal));

            while (!SetTargetLocation(transform.position + UnityEngine.Random.insideUnitSphere * UnityEngine.Random.Range(minPassiveDistanceVal, maxPassiveDistanceVal)));
        }
    }

    void CuriousUpdate() 
    { 
        print("curious"); 
    }

    void HostileUpdate()
    {
        print("Hostile");
        if (attackTimer.IsOver && Vector3.Distance(transform.position, player.position) <= attackRange)
        {
            attackTimer.Reset();
            print("Attack");
        }

        SetTargetLocation(player.position);
    }

    void FleeingUpdate() 
    {
        if(Vector3.Distance(transform.position, player.position) > runDistance * 2)
        {

            if (stateWatch.GetElapsed >= endFleeTime)
            {
                fleeHealth = health.CurrentHealth;
                ChangeState(PassiveUpdate);
            }
        }

        if (Vector3.Distance(transform.position, agent.destination) < minMoveDistance)
        {
            Vector3 position;

            Vector3 playerAdjusted = player.position;
            playerAdjusted.y = transform.position.y;

            Vector3 playerCreatureVector = (transform.position - playerAdjusted).normalized;

            float degrees = UnityEngine.Random.Range(-85f, 85f);
            float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
            float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);

            float tx = playerCreatureVector.x;
            float tz = playerCreatureVector.z;

            playerCreatureVector.x = (cos * tx) - (sin * tz);
            playerCreatureVector.z = (sin * tx) + (cos * tz);

            position = transform.position + playerCreatureVector * runDistance;

            if(!SetTargetLocation(position)) FleeingUpdate();
        }
    }

    public override void Hit()
    {
        endFleeTime = stateWatch.GetElapsed + UnityEngine.Random.Range(minEndFleeTime, maxEndFleeTime);
        print("Hit");

        base.Hit();

        //  random between hostile or fleeing
        if (currentStateFunction == FleeingUpdate)
        {
            if(fleeHealth / health.MaxHealth > new Vector2(hostility, selfPreservation).normalized.x)
            {
                ChangeState(HostileUpdate);
            }

            return;
        }
            
        Action newState = (UnityEngine.Random.Range(0f, 1f) < new Vector2(selfPreservation, hostility).normalized.x) ? FleeingUpdate : HostileUpdate;
        if(newState != currentStateFunction) ChangeState(newState);
    }

    // Simple only checks based on player pos not all of the player
    bool NoticesPlayer()
    {
        return Vector3.Angle(eyes.forward, player.position - transform.position) < maxAngle;
    }

    bool SetTargetLocation(Vector3 targetLocation)
    {
        return agent.SetDestination(targetLocation);
    }
}
