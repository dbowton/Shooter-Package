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
    public Inventory inventory;
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
    }

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
            print("DIstance: " + Vector3.Distance(transform.position, agent.destination));
            if(Mathf.Pow(Vector3.Distance(transform.position, agent.destination),2) < (Mathf.Pow(controller.radius, 2) + Mathf.Pow(controller.height,2)))
            {
                isActiveVendor = true;
                isGoingToShop = false;
            }
        }

        if(possibleLocations.Count == 0)
        {
            isActiveVendor = false;
            SetTargetLocation(Vector3.zero);
        }

        if(isActiveVendor)
        {
            //            (bool hit, RaycastHit hitInfo) lookingAt = player.LookingAt();
            //            if (lookingAt.hit && lookingAt.hitInfo.collider.gameObject.Equals(gameObject))
            if (Vector3.Distance(transform.position, playerTransform.position) < 12)
            {
                Vector3 lookAt = playerTransform.position;
                lookAt.y = transform.position.y;

                transform.LookAt(lookAt, Vector3.up);
            }

            if (Vector3.Distance(transform.position, playerTransform.position) < 2)
            {
                print("x - trade");

                if(Input.GetKeyDown(KeyCode.X))
                {
                    dialogueSystem.ReturnToConversation();
                }
            }
        }
    }

    [SerializeField] DialogueSystem dialogueSystem;

    public void BeginTrade()
    {
        GameManager.Instance.BeginTrade(this);
    }
}
