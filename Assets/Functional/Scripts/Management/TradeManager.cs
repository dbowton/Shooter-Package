using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class TradeManager : MonoBehaviour
{
    [SerializeField] private TMP_Text playerNameIndicator;
    [SerializeField] private TMP_Text playerInventoryScreen;
    [SerializeField] private TMP_Text playerMoneyScreen;

    [SerializeField] private TMP_Text traderNameIndicator;
    [SerializeField] private TMP_Text otherInventoryScreen;
    [SerializeField] private TMP_Text otherMoneyScreen;

    [SerializeField] private TMP_Text systemButtons;

    [SerializeField] private int showCount = 10;

    GameObject centerDisplayObject;
    [SerializeField] GameObject centerDisplayLocation;
    [SerializeField] TMP_Text itemDescription;

    [SerializeField] Color displayObjectColor;

    private bool isPlayerManage = false;
    private int playerIndex = 0;
    private int otherIndex = 0;

    private string playerName;
    private string tradersName;

    private float heldUp = 0;
    private float heldDown = 0;
    private float heldEnter = 0;
    private float heldTime = 0.5f;
    private float heldReduction = 0.75f;

    private Inventory playerInventory;
    private Inventory otherInventory;
    
    private void Start()
    {
        systemButtons.text = "";
        systemButtons.text += "[tab] - switch inventories   |   [E] - transfer   |   [esc] - leave";
    }

    public void SetUp(PlayerManager player, NPC npc)
    {        
        playerName = player.Name;
        tradersName = npc.Name;

        playerNameIndicator.text = player.Name;
        traderNameIndicator.text = npc.Name;

        this.playerInventory = player.playerData.inventory;
        this.otherInventory = npc.inventory;

        DisplayChange();
    }

    public void update(float dt)
    {
        #region update center display
        if(centerDisplayObject)
        {
            centerDisplayObject.transform.Rotate(15 * dt * Vector3.up);
        }
            
        #endregion

        #region manage input
        //  switch to other inventory
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                isPlayerManage = !isPlayerManage;
                DisplayChange();
                return;
            }
        }

        //  manage index scrolling
        {
            if ((Input.GetKey(KeyCode.E) && !Input.GetKeyDown(KeyCode.E)) || (Input.GetKey(KeyCode.Return) && !Input.GetKeyDown(KeyCode.Return)))
                heldEnter += Time.deltaTime;
            else
                heldEnter = 0;

            if ((Input.GetKey(KeyCode.W) && !Input.GetKeyDown(KeyCode.W)) || (Input.GetKey(KeyCode.UpArrow) && !Input.GetKeyDown(KeyCode.UpArrow)))
                heldUp += Time.deltaTime;
            else
                heldUp = 0;

            if ((Input.GetKey(KeyCode.S) && !Input.GetKeyDown(KeyCode.S)) || (Input.GetKey(KeyCode.DownArrow) && !Input.GetKeyDown(KeyCode.DownArrow)))
                heldDown += Time.deltaTime;
            else
                heldDown = 0;

            if (Input.mouseScrollDelta.y < 0 || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow) || heldDown > heldTime)
            {
                if (isPlayerManage)
                {
                    if (playerIndex < playerInventory.Items.Count - 1)
                    {
                        playerIndex++;
                        DisplayChange();
                    }
                }
                else
                {
                    if (otherIndex < otherInventory.Items.Count - 1)
                    {
                        otherIndex++;
                        DisplayChange();
                    }
                }

                    heldDown = heldTime * heldReduction;
            }

            if (Input.mouseScrollDelta.y > 0 || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow) || heldUp > heldTime)
            {
                if (isPlayerManage)
                {
                    if (playerIndex > 0)
                    {
                        playerIndex--;
                        DisplayChange();
                    }
                }
                else
                {
                    if (otherIndex > 0)
                    {
                        otherIndex--;
                        DisplayChange();
                    }
                }

                heldUp = heldTime * heldReduction;
            }
        }

        //  transfer if valid index
        {
            if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Return) || heldEnter > heldTime)
            {
                if (isPlayerManage)
                {
                    // sell item to trader

                    if(playerIndex >= 0 && playerIndex < playerInventory.Items.Count)
                    {
                        Item soldItem = playerInventory.Items[playerIndex];
                        if (otherInventory.money >= soldItem.Value)
                        {
                            playerInventory.money += soldItem.Value;
                            otherInventory.money -= soldItem.Value;
                            playerInventory.Items.Remove(soldItem);
                            otherInventory.AddItem(soldItem);
                            DisplayChange();
                        }
                        else
                        {
                            print(tradersName + " cannot afford to buy " + soldItem.Name + " for " + soldItem.Value);
                        }

                        if (playerIndex > playerInventory.Items.Count - 1) playerIndex = playerInventory.Items.Count - 1;
                    }
                }
                else
                {
                    // buy item from trader
                    if (otherIndex >= 0 && otherIndex < otherInventory.Items.Count)
                    {
                        Item boughtItem = otherInventory.Items[otherIndex];
                        if(playerInventory.money >= boughtItem.Value)
                        {
                            playerInventory.money -= boughtItem.Value;
                            otherInventory.money += boughtItem.Value;
                            playerInventory.AddItem(boughtItem);
                            otherInventory.Items.Remove(boughtItem);
                            DisplayChange();
                        }
                        else
                        {
                            print("You cannot afford to buy " + boughtItem.Name + " for " + boughtItem.Value);
                        }

                        if (otherIndex > otherInventory.Items.Count - 1) otherIndex = otherInventory.Items.Count - 1;
                    }
                }

                heldEnter = heldTime * heldReduction;
                return;
            }
        }

        //  exit trade
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                GameManager.Instance.EndTrade();
//                GameObject.Find("GameManager").GetComponent<SimpleGameManager>().EndTrade();
                Destroy(centerDisplayObject);
                //GameManager.Instance.GameState = GameManager.State.PLAYER;
            }
        }
        #endregion
    }

    public void DisplayChange()
    {
        #region update trader Items
        otherMoneyScreen.text = "$" + otherInventory.money;
        traderNameIndicator.text = tradersName;
        if (!isPlayerManage)
            traderNameIndicator.text += "-";

        otherInventoryScreen.text = "";

        int traderStart = Mathf.Min(otherIndex, Mathf.Max(otherInventory.Items.Count - showCount, 0));
        int traderValid = Mathf.Max((otherIndex > 0 && otherIndex < otherInventory.Items.Count - 2) ? traderStart - 1 : traderStart, 0);
        for (int i = 0; i < showCount; i++, traderValid++)
        {
            if (traderValid < traderStart + showCount - ((otherIndex > 0 && otherIndex < otherInventory.Items.Count - 2) ? 1 : 0) && traderValid < otherInventory.Items.Count && traderValid >= 0)
            {
                print("Requested: " + traderValid + " Max: " + otherInventory.Items.Count);

                if (traderValid == otherIndex && !isPlayerManage)
                    otherInventoryScreen.text += "- " + otherInventory.Items[traderValid].Name + " -";
                else
                    otherInventoryScreen.text += otherInventory.Items[traderValid].Name;
            }

            otherInventoryScreen.text += "\n";
        }
        #endregion
        #region update player Items
        playerMoneyScreen.text = "$" + playerInventory.money;
        playerNameIndicator.text = playerName;
        if (isPlayerManage)
            playerNameIndicator.text += "-";

        playerInventoryScreen.text = "";

        int playerStart = Mathf.Min(playerIndex, Mathf.Max(playerInventory.Items.Count - showCount, 0));
        int playerValid = Mathf.Max((playerIndex > 0 && playerIndex < playerInventory.Items.Count - 2) ? playerStart - 1 : playerStart, 0);
        for (int i = 0; i < showCount; i++, playerValid++)
        {
            if (playerValid < playerStart + showCount - ((playerIndex > 0 && playerIndex < playerInventory.Items.Count - 2) ? 1 : 0) && playerValid < playerInventory.Items.Count && playerValid >= 0)
            {
                if (playerValid == playerIndex && isPlayerManage)
                    playerInventoryScreen.text += "- " + playerInventory.Items[playerValid].Name + " -";
                else
                    playerInventoryScreen.text += playerInventory.Items[playerValid].Name;
            }

            playerInventoryScreen.text += "\n";
        }
        #endregion

        #region update center display
        Destroy(centerDisplayObject);
        if(isPlayerManage)
        {
            if(playerIndex >= 0 && playerIndex < playerInventory.Items.Count)
            {
                itemDescription.text = playerInventory.Items[playerIndex].GetDesc();

/*                if (testCase)
                {
                    Vector3 pos = Camera.main.transform.position + Camera.main.transform.forward * 3;
                    centerDisplayObject = Instantiate(playerInventory.items[playerIndex].visualPrefab);
                    centerDisplayObject.transform.position = pos;
                    centerDisplayObject.GetComponent<Renderer>().material.color = displayObjectColor;
                }*/

            }
        }
        else
        {
            if (otherIndex >= 0 && otherIndex < otherInventory.Items.Count)
            {
                itemDescription.text = otherInventory.Items[otherIndex].GetDesc();

/*                if(testCase)
                {
                    centerDisplayObject = Instantiate(otherInventory.items[otherIndex].visualPrefab);
                    Vector3 pos = Camera.main.transform.position + Camera.main.transform.forward * 3;
                    centerDisplayObject.transform.position = pos;
                    centerDisplayObject.GetComponent<Renderer>().material.color = displayObjectColor;
                }*/
            }
        }

        #endregion
    }
}
