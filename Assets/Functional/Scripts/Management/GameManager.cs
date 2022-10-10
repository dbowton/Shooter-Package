using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : Singleton<GameManager>
{
    public enum State
    {
        TITLE,
        MAIN,
        PLAYER,
        INVENTORY,
        TRADE,
        GAMEOVER
    }

    [SerializeField] private PlayerManager testplayer;

    public PlayerInput input;

    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject tradeScreenPrefab;
    [SerializeField] private GameObject inventoryScreenPrefab;

    private PlayerManager player;
    private TradeManager tradeManager;
    private InventoryManager inventoryMangager;

    public List<AI> ai = new List<AI>();

    [HideInInspector] public List<Interactable> activeInteractables = new List<Interactable>();

    [HideInInspector] public List<Timer> timers = new List<Timer>();

    private void OnValidate()
    {
        if (playerPrefab != null)
            if (!playerPrefab.TryGetComponent<PlayerManager>(out _))
            {
                playerPrefab = null;
                Debug.LogError("PlayerPrefab Must Contain the PLAYER script");
            }

        if (tradeScreenPrefab != null)
            if (!tradeScreenPrefab.TryGetComponent<TradeManager>(out _))
            {
                tradeScreenPrefab = null;
                Debug.LogError("TradeScreenPrefab Must Contain the TradeManager script");
            }
    }

    private State gameState = State.TITLE;
    public State GameState { get { return gameState; } set { SetState(value); } }

    private void SetState(State state)
    {
        //On State Leave
        {
            switch (gameState)
            {
                case State.MAIN:
                    break;
                case State.TITLE:
                    break;
                case State.PLAYER:
                    testplayer.active = false;
//                    testplayer.gameAnimator.enabled = false;
//                    testplayer.viewAnimator.enabled = false;

                    List<Interactable> tempInteractables = new List<Interactable>();
                    activeInteractables.ForEach(x =>
                    {
                        x.RangeTimer = 0.0f;
                        x.update(Time.deltaTime);
                        if (x.RangeTimer > 0)
                            tempInteractables.Add(x);
                    });
                    activeInteractables = tempInteractables;

                    break;
                case State.INVENTORY:
                    break;
                case State.TRADE:

                    Destroy(tradeManager.gameObject);
                    tradeManager = null;

                    break;
                case State.GAMEOVER:
                    break;
                default:
                    break;
            }
        }

        gameState = state;

        //On State Enter
        {
            switch (gameState)
            {
                case State.MAIN:

                    break;
                case State.TITLE:

                    break;
                case State.PLAYER:
//                    if (!player) player = Instantiate(playerPrefab).GetComponent<Player>();
                    testplayer.active = true;
//                    testplayer.gameAnimator.enabled = true;
//                    testplayer.viewAnimator.enabled = true;

                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;

                    break;
                case State.INVENTORY:
                    Cursor.lockState = CursorLockMode.Confined;
                    Cursor.visible = true;
                    break;
                case State.TRADE:
                    Cursor.lockState = CursorLockMode.Confined;
                    Cursor.visible = true;
                    break;
                case State.GAMEOVER:

                    break;
                default:
                    break;
            }

            input.currentActionMap = input.actions.FindActionMap(gameState.ToString().ToLower(), false);
        }
    }

    private void Update()
    {
        timers.ForEach(x => x.Update());

        switch (GameState)
        {
            case State.MAIN:
                GameState = State.PLAYER;
                break;
            case State.TITLE:
                GameState = State.MAIN;
                break;
            case State.PLAYER:
                //  update
                {
                    List<AI> tempAi = new List<AI>();
                    ai.ForEach(x => tempAi.Add(x));
                    tempAi.ForEach(x => x.update(Time.deltaTime));

                    List<Interactable> tempInteractables = new List<Interactable>();
                    activeInteractables.ForEach(x =>
                    {
                        x.update(Time.deltaTime);
                        if (x.RangeTimer > 0)
                            tempInteractables.Add(x);
                    });

                    activeInteractables = tempInteractables;
                }

                break;
            case State.INVENTORY:
                break;
            case State.TRADE:
                tradeManager.update(Time.deltaTime);
                break;
            case State.GAMEOVER:
                break;
            default:
                break;
        }
    }

    public void Trade(Container container)
    {
        GameState = State.TRADE;
        tradeManager = Instantiate(tradeScreenPrefab).GetComponent<TradeManager>();
        tradeManager.SetUp(ref testplayer, ref container);
    }

    public void OnInventoryReturn(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.performed)
            GameState = State.PLAYER;
    }
}
