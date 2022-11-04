using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(NavMeshAgent))]
public abstract class AI : Character
{
    public Transform playerTransform { get { return GameManager.Instance.player.transform; } }
    public PlayerManager player { get { return GameManager.Instance.player; } }
//    public SimpleGameManager gameManager { get { return GameObject.Find("GameManager").GetComponent<SimpleGameManager>(); } }
        
    
    //PlayerManager player { get { return GameManager.Instance.player; } }
    //Transform playerTransform { get { return player.transform; } }

    public Animator animator;

    public override void Die()
    {
        print(Name + " died");
        
        base.Die();
        GameManager.Instance.ai.Remove(this);
//        animator.SetTrigger("death");
        Destroy(gameObject);
    }


    protected NavMeshAgent agent;
    protected CharacterController controller;

    [SerializeField] [Range(0, 100)] protected float passiveness;
    [SerializeField] [Range(0, 100)] protected float curiosity;
    [SerializeField] [Range(0, 100)] protected float hostility;
    [SerializeField] [Range(0, 100)] protected float selfPreservation;

    //    [SerializeField] float teritory = 50f;

    Stopwatch stateWatch;
    [SerializeField] Transform eyes;

    List<(float value, Action action)> stateWeights = new List<(float value, Action action)>();

    Action currentStateFunction;
    float minMoveDistance = 0.25f;

    [SerializeField] float speed = 4.0f;
    float RunDistance { get { return speed / 2f; } }
    [SerializeField] float maxAngle = 22.5f;

    Timer attackTimer;
    [SerializeField] float attackRange = 1.25f;
    [SerializeField] float attackIntervals = 0.75f;

    [SerializeField] List<Collider> bounds;

    [Space(15)]
    [Header("Debug Tools")]
    [SerializeField] bool setPassive = false;
    [SerializeField] bool setCurious = false;
    [SerializeField] bool setHostile = false;
    [SerializeField] bool setFleeing = false;

    #region Passive Properties
    float passiveStateChangeTime = 0;

    float MinPassiveStateChangeTime { get { return (13f / 100 * (passiveness) + 2); } }
    float MaxPassiveStateChangeTime { get { return (1f / 5 * (passiveness) + 10); } }

    float nextPassiveChange = 0;

    float MinPassiveChangeVal { get { return (1f / 5 * (passiveness) + 5); } }
    float MaxPassiveChangeVal { get { return (1f / 2 * (passiveness) + 15); } }

    float MinPassiveDistanceVal { get { return (2f / 25 * (passiveness) + 2); } }
    float MaxPassiveDistanceVal { get { return (13f / 100 * (passiveness) + 8); } }

    #endregion
    #region Fleeing Properties
    float endFleeTime = 0;
    float MinEndFleeTime { get { return (9f / 100 * (selfPreservation) + 3); } }
    float MaxEndFleeTime { get { return (13f / 100 * (selfPreservation) + 5); } }
    float FleeMultiplier { get { return (3.5f / 100 * (selfPreservation) + 1.5f); } }
    float fleeHealth = 0;
    #endregion

    protected virtual void Awake()
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

        GameManager.Instance.ai.Add(this);
    }

    public override void update(float dt)
    {
        base.update(dt);
        stateWatch.Update(dt);
        attackTimer.Update(dt);
        currentStateFunction.Invoke();
//        print(currentStateFunction.Method.Name);

        if (setPassive)
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

    protected virtual void PassiveUpdate()
    {
        if (NoticesPlayer())
        {
            if (stateWatch.GetElapsed >= passiveStateChangeTime)
            {
                passiveStateChangeTime = stateWatch.GetElapsed + UnityEngine.Random.Range(MinPassiveStateChangeTime, MaxPassiveStateChangeTime);
                float ranNum = UnityEngine.Random.Range(0, 1f);
                foreach (var (value, action) in stateWeights)
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
        else if (stateWatch.GetElapsed > nextPassiveChange)
        {
            nextPassiveChange = stateWatch.GetElapsed + (UnityEngine.Random.Range(MinPassiveChangeVal, MaxPassiveChangeVal));

            while (!SetTargetLocation(transform.position + UnityEngine.Random.insideUnitSphere * UnityEngine.Random.Range(MinPassiveDistanceVal, MaxPassiveDistanceVal))) ;
        }
    }

    protected virtual void CuriousUpdate()
    {
        print("curious");
    }

    protected virtual void HostileUpdate()
    {
        if (attackTimer.IsOver && Vector3.Distance(transform.position, playerTransform.position) <= attackRange)
        {
            attackTimer.Reset();
            print("Attack");
        }

        SetTargetLocation(playerTransform.position);
    }

    protected virtual void FleeingUpdate()
    {
        if (Vector3.Distance(transform.position, playerTransform.position) > RunDistance * FleeMultiplier)
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

            Vector3 playerAdjusted = playerTransform.position;
            playerAdjusted.y = transform.position.y;

            Vector3 playerCreatureVector = (transform.position - playerAdjusted).normalized;

            float degrees = UnityEngine.Random.Range(-85f, 85f);
            float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
            float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);

            float tx = playerCreatureVector.x;
            float tz = playerCreatureVector.z;

            playerCreatureVector.x = (cos * tx) - (sin * tz);
            playerCreatureVector.z = (sin * tx) + (cos * tz);

            position = transform.position + playerCreatureVector * RunDistance;

            if (!SetTargetLocation(position)) FleeingUpdate();
        }
    }

    public override void Hit()
    {
        endFleeTime = stateWatch.GetElapsed + UnityEngine.Random.Range(MinEndFleeTime, MaxEndFleeTime);

        base.Hit();

        //  random between hostile or fleeing
        if (currentStateFunction == FleeingUpdate)
        {
            if (fleeHealth / health.MaxHealth < new Vector2(hostility, selfPreservation).normalized.x)
            {
                ChangeState(HostileUpdate);
            }

            return;
        }

        Action newState = (UnityEngine.Random.Range(0f, 1f) < new Vector2(selfPreservation, hostility).normalized.x) ? FleeingUpdate : HostileUpdate;
        if (newState != currentStateFunction) ChangeState(newState);
    }

    // Simple only checks based on player pos not all of the player
    protected bool NoticesPlayer()
    {
        return Vector3.Angle(eyes.forward, playerTransform.position - transform.position) < maxAngle;
    }

    protected bool SetTargetLocation(Vector3 targetLocation)
    {
        return agent.SetDestination(targetLocation);
    }
}
