using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(UnityEngine.AI.NavMeshAgent))]
public class NPC : AI
{
    public ComplexInventory inventory;
    [SerializeField] [Tooltip("This allows a vendor w/out a shop")] bool isVendor;
    bool isActiveVendor = false;
    bool isGoingToShop = false;

    [SerializeField] List<NPCLocation> locations = new List<NPCLocation>();

    [System.Serializable]
    public struct NPCLocation
    {
        public Transform location;
        public float standardDuration;
        public float startTime;
        public float endTime;
        public InterestReason interestReason;
    }

    public enum InterestReason
    {
        Shop,
        Other
    }

    public override void update(float dt)
    {
        base.update(dt);

        if(agent.remainingDistance < 0.5f)
        {
            animator.SetFloat("Speed", 0);
        }
        else animator.SetFloat("Speed", 1);

        int selected = 0;
        if (prevSelected > 0) selected = 0;
        else if(UnityEngine.Random.Range(0, 2) == 0)
        {
            selected = UnityEngine.Random.Range(0, 5);
        }

        if (isActiveVendor) selected = 0;

        animator.SetInteger("ranNum", selected);
        prevSelected = selected;
    }

    int prevSelected;

    protected override void PassiveUpdate()
    {
        List<NPCLocation> possibleLocations = locations.Where(x => x.startTime <= GameManager.Instance.gameTime && x.endTime >= GameManager.Instance.gameTime).ToList();
        if (!isActiveVendor && !isGoingToShop && possibleLocations.Count > 0 && possibleLocations.Any(x => x.interestReason.Equals(InterestReason.Shop)))
        {
            isGoingToShop = true;
            SetTargetLocation(possibleLocations.Where(x => x.interestReason.Equals(InterestReason.Shop)).First().location.transform.position);
        }
        else if(isGoingToShop)
        {
            if(Mathf.Pow(Vector3.Distance(transform.position, agent.destination),2) < (Mathf.Pow(controller.radius, 2) + Mathf.Pow(controller.height,2)))
            {
                isActiveVendor = true;
                isGoingToShop = false;
            }
        }
        else if(!isActiveVendor && possibleLocations.Any(x => !x.interestReason.Equals(InterestReason.Shop)))
        {
            SetTargetLocation(possibleLocations.Where(x => !x.interestReason.Equals(InterestReason.Shop)).First().location.transform.position);
        }

        if(possibleLocations.Count == 0)
        {
            isActiveVendor = false;
            SetTargetLocation(Vector3.zero);
        }

        if(isActiveVendor)
        {
            if (Vector3.Distance(transform.position, playerTransform.position) < 12)
            {
                Vector3 lookAt = playerTransform.position;
                lookAt.y = transform.position.y;

                transform.LookAt(lookAt, Vector3.up);
            }

            if (Vector3.Distance(transform.position, playerTransform.position) < 2 &&
                Physics.Raycast(SmartCamera.main.transform.position, SmartCamera.main.transform.forward, out RaycastHit hitInfo) &&
                hitInfo.transform.root.gameObject.Equals(gameObject))
            {
                print("x - trade");
                if (interactObject == null) interactObject = Instantiate(interactPrefab);


                if (Input.GetKeyDown(KeyCode.X))
                {
                    dialogueSystem.StartConversation();
                    Destroy(interactObject);
                }
            }
            else if (interactObject) Destroy(interactObject);
        }
        else if (interactObject) Destroy(interactObject);
    }

    [SerializeField] GameObject interactPrefab;
    GameObject interactObject;


    [SerializeField] NewDialogueSystem dialogueSystem;

    public void BeginTrade()
    {
        GameManager.Instance.BeginTrade(this);
    }
}
