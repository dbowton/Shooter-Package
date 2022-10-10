using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TradeManager : MonoBehaviour
{
    [SerializeField] private TMP_Text playerNameIndicator;
    [SerializeField] private TMP_Text playerScreen;

    [SerializeField] private TMP_Text containerNameIndicator;
    [SerializeField] private TMP_Text containerScreen;

    [SerializeField] private TMP_Text systemButtons;

    [SerializeField] private int showCount = 10;

//    private List<Item> playerItems = new List<Item>();
//    private List<Item> containerItems = new List<Item>();

    private bool isPlayerManage = false;
    private int containerIndex = 0;
    private int playerIndex = 0;

    private string playerName;
    private string containerName;

    private float heldUp = 0;
    private float heldDown = 0;
    private float heldEnter = 0;
    private float heldTime = 0.5f;
    private float heldReduction = 0.75f;

    private PlayerManager player;
    private Container container;

    private void Start()
    {
        systemButtons.text = "";

        systemButtons.text += "[tab] - switch inventories   |   [E] - transfer   |   [esc] - leave";
    }

    public void SetUp(ref PlayerManager player, ref Container container)
    {
        playerName = player.Name;
        containerName = container.gameObject.name;

        playerNameIndicator.text = playerName;
        containerNameIndicator.text = containerName;

        this.player = player;
        this.container = container;
    }

    public void update(float dt)
    {
        //  updates container items
        {
            containerNameIndicator.text = containerName;
            if (!isPlayerManage)
                containerNameIndicator.text += "-";

            containerScreen.text = "";

            int start = Mathf.Min(containerIndex, Mathf.Max(container.ItemList.Count - showCount, 0));
            int valid = Mathf.Max((containerIndex > 0 && containerIndex < container.ItemList.Count - 2) ? start - 1 : start, 0);
            for (int i = 0; i < showCount; i++, valid++)
            {
                if (valid < start + showCount - ((containerIndex > 0 && containerIndex < container.ItemList.Count - 2) ? 1 : 0) && valid < container.ItemList.Count && valid >= 0)
                {
                    if (valid == containerIndex && !isPlayerManage)
                        containerScreen.text += "- " + container.ItemList[valid].Name + " -";
                    else
                        containerScreen.text += container.ItemList[valid].Name;
                }


                containerScreen.text += "\n";
            }

            if (container.ItemList.Count > 0)
                containerScreen.text += "\nE - Transfer";
        }


        //  updates playerItems
        {
            playerNameIndicator.text = playerName;
            if (isPlayerManage)
                playerNameIndicator.text += "-";

            playerScreen.text = "";

/*            int start = Mathf.Min(playerIndex, Mathf.Max(player.ItemList.Count - showCount, 0));
            int valid = Mathf.Max((playerIndex > 0 && playerIndex < player.ItemList.Count - 2) ? start - 1 : start, 0);
            for (int i = 0; i < showCount; i++, valid++)
            {
                if (valid < start + showCount - ((playerIndex > 0 && playerIndex < player.ItemList.Count - 2) ? 1 : 0) && valid < player.ItemList.Count && valid >= 0)
                {
                    if (valid == playerIndex && isPlayerManage)
                        playerScreen.text += "- " + player.ItemList[valid].Name + " -";
                    else
                        playerScreen.text += player.ItemList[valid].Name;
                }

                playerScreen.text += "\n";
            }*/
        }


        //  manages user input
        {
            //  switch to other inventory
            {
                if(Input.GetKeyDown(KeyCode.Tab))
                {
                    isPlayerManage = !isPlayerManage;
                    return;
                }
            }

            //  manage index scrolling
            {
                if ((Input.GetKey(KeyCode.E) && !Input.GetKeyDown(KeyCode.E)) || (Input.GetKey(KeyCode.Return) && !Input.GetKeyDown(KeyCode.Return)))
                    heldEnter += dt;
                else
                    heldEnter = 0;

                if ((Input.GetKey(KeyCode.W) && !Input.GetKeyDown(KeyCode.W)) || (Input.GetKey(KeyCode.UpArrow) && !Input.GetKeyDown(KeyCode.UpArrow)))
                    heldUp += dt;
                else
                    heldUp = 0;

                if ((Input.GetKey(KeyCode.S) && !Input.GetKeyDown(KeyCode.S)) || (Input.GetKey(KeyCode.DownArrow) && !Input.GetKeyDown(KeyCode.DownArrow)))
                    heldDown += dt;
                else
                    heldDown = 0;

                if (Input.mouseScrollDelta.y < 0 || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow) || heldDown > heldTime)
                {
                    if (isPlayerManage)
                    {
                        //if (playerIndex < player.ItemList.Count - 1) playerIndex++;
                    }
                    else
                    {
                        if (containerIndex < container.ItemList.Count - 1) containerIndex++;
                    }

                    heldDown = heldTime * heldReduction;
                }

                if (Input.mouseScrollDelta.y > 0 || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow) || heldUp > heldTime)
                {
                    if (isPlayerManage)
                    {
                        if (playerIndex > 0) playerIndex--;
                    }
                    else
                    {
                        if (containerIndex > 0) containerIndex--;
                    }

                    heldUp = heldTime * heldReduction;
                }
            }

            //  transfer if valid index
            {
                if(Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Return) || heldEnter > heldTime)
                {
                    if(isPlayerManage)
                    {
                        /*if(playerIndex >= 0 && playerIndex < player.ItemList.Count)
                        {
                            List<Item> tempList = new List<Item>();
                            tempList.AddRange(container.ItemList);
                            tempList.Add(player.ItemList[playerIndex]);
                            container.ItemList = tempList;

                            tempList = new List<Item>();
                            tempList.AddRange(player.ItemList);
                            tempList.Remove(player.ItemList[playerIndex]);
                            player.ItemList = tempList;

                            if (playerIndex > player.ItemList.Count - 1) playerIndex = player.ItemList.Count - 1;
                        }*/
                    }
                    else
                    {
                        if (containerIndex >= 0 && containerIndex < container.ItemList.Count)
                        {
                            List<Item> tempList = new List<Item>();
                            //tempList.AddRange(player.ItemList);
                            tempList.Add(container.ItemList[containerIndex]);
                            //player.ItemList = tempList;

                            tempList = new List<Item>();
                            tempList.AddRange(container.ItemList);
                            tempList.Remove(container.ItemList[containerIndex]);
                            container.ItemList = tempList;

                            if (containerIndex > container.ItemList.Count - 1) containerIndex = container.ItemList.Count - 1;
                        }
                    }

                    heldEnter = heldTime * heldReduction;
                    return;
                }
            }

            //  exit trade
            {
                if(Input.GetKeyDown(KeyCode.Escape))
                    GameManager.Instance.GameState = GameManager.State.PLAYER;
            }
        }
    }
}
