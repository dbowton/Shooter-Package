using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleGameManager : MonoBehaviour
{
    public float gameTime = 0f;
    [SerializeField] bool AddFifteenSeconds = false;
    [SerializeField] GameObject tradeWindow;
    TradeManager activeTradeManager;
    [SerializeField] SimplePlayer player;

    public List<AI> ai = new List<AI>();

    bool isPaused = false;

    void Update()
    {
        if(!isPaused)
        {
            if(AddFifteenSeconds)
            {
                gameTime += 15f;
                AddFifteenSeconds = false;
            }

            gameTime += Time.deltaTime;
            gameTime %= 300f;

            player.update(Time.deltaTime);
            foreach (var go in ai) go.update(Time.deltaTime);
        }

        else
        {
            if (activeTradeManager) activeTradeManager.update(Time.deltaTime);
        }
    }

    public void BeginTrade(NPC trader)
    {
        activeTradeManager = Instantiate(tradeWindow).GetComponent<TradeManager>();
//        activeTradeManager.SetUp(player, trader);
        isPaused = true;
    }

    public void EndTrade()
    {
        isPaused = false;

        Destroy(activeTradeManager.gameObject);
        activeTradeManager = null;
    }
}
