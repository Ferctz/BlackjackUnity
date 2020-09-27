using Blackjack.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Blackjack
{
    public enum GameState : int
    {
        Shuffle = 0,
        Betting = 1,
        Dealing = 2,
        Playing = 3,
        Result = 4
    }

    public class BlackjackManager : MonoBehaviour
    {
        public IntVariable minimumBetAmount;
        public IntVariable startingPlayerCash;
        public IntVariable numberOfDecks;

        private StateMachine<GameState> stateMachine;

        private Deck deck;
        private int seed = 0;

        [SerializeField] private Sprite[] cardSprites;
        [SerializeField] private Sprite backsideSprite;

        [SerializeField] private Dealer dealer;
        [SerializeField] private Player[] players;

        [Header("UI")]
        #region UI
        [SerializeField] private Button dealButton;
        [SerializeField] private Button standButton;
        [SerializeField] private Button hitButton;
        [SerializeField] private Button doubleButton;
        [SerializeField] private Button rebetButton;
        [SerializeField] private Button newGameButton;
        [SerializeField] private CanvasGroup settingsScreen;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Slider minimumBetSlider;
        [SerializeField] private Text minimumBetValueText;
        [SerializeField] private Slider startingCashSlider;
        [SerializeField] private Text startingCashValueText;
        [SerializeField] private Button settingsBackButton;
        #endregion

        private void Awake()
        {
            dealButton.onClick.AddListener(DealCards);

            for (int i = 0; i < players.Length; i++)
            {
                int playerIndex = i;
                players[i].joinButton.onClick.AddListener(() => AddPlayer(playerIndex));
                players[i].betButton.onClick.AddListener(() => AddBet(playerIndex));
            }
        }

        private void Start()
        {
            // populate state machine with states & switch to inactive state
            stateMachine = new StateMachine<GameState>();
            stateMachine.Add(GameState.Shuffle, EnterShuffleState, UpdateShuffleState, null);
            stateMachine.Add(GameState.Betting, EnterBettingState, UpdateBettingState, ExitBettingState);
            stateMachine.Add(GameState.Dealing, EnterDealingState, null, null);
            stateMachine.Add(GameState.Playing, null, null, null);
            stateMachine.Add(GameState.Result, null, null, null);            

            stateMachine.SwitchTo(GameState.Shuffle);
        }

        private void Update()
        {
            stateMachine.Update();
        }

        public void AddPlayer(int playerId)
        {
            if (playerId < 0 || playerId > GameConstants.MAXIMUM_PLAYER_COUNT)
            {
                Debug.LogWarning("Invalid player Id");
                return;
            }

            players[playerId].Initialize(startingPlayerCash.Value);
        }

        public void AddBet(int playerId)
        {
            if (playerId < 0 || playerId > GameConstants.MAXIMUM_PLAYER_COUNT)
            {
                Debug.LogWarning("Invalid player Id");
                return;
            }

            players[playerId].AddBet(minimumBetAmount.Value);
        }

        #region UI Actions
        private void DealCards()
        {
            stateMachine.SwitchTo(GameState.Dealing);
        }

        #endregion

        #region State Machine Methods

        private void EnterShuffleState()
        {
            deck = new Deck();
            seed = (int)System.DateTime.Now.Ticks;
            deck.Shuffle(seed, numberOfDecks.Value);
        }

        private void UpdateShuffleState()
        {
            stateMachine.SwitchTo(GameState.Betting);
        }

        private void EnterBettingState()
        {
            dealButton.gameObject.SetActive(true);
            settingsButton.gameObject.SetActive(true);

            for (int i = 0; i < players.Length; i++)
            {
                if (players[i].scorerData.cash > 0)
                {
                    players[i].betButton.gameObject.SetActive(true);
                }
                else
                {
                    players[i].joinButton.gameObject.SetActive(true);
                }
            }
        }

        private void UpdateBettingState()
        {
            for (int i = 0; i < players.Length; i++)
            {
                // ie if player not initialized
                if (players[i].scorerData.hands == null || players[i].scorerData.hands.Count == 0)
                {
                    continue;
                }

                if (players[i].scorerData.hands[0].bet > 0)
                {
                    dealButton.interactable = true;
                    return;
                }
            }

            dealButton.interactable = false;
        }

        private void ExitBettingState()
        {
            dealButton.gameObject.SetActive(false);

            dealer.Initialize(startingPlayerCash.Value);
        }

        private void EnterDealingState()
        {
            dealButton.gameObject.SetActive(false);
            settingsButton.gameObject.SetActive(false);

            // turning off UI buttons from betting/joining/AI
            for (int i = 0; i < players.Length; i++)
            {
                players[i].joinButton.gameObject.SetActive(false);
                players[i].aiButton.gameObject.SetActive(false);
                players[i].betButton.gameObject.SetActive(false);
            }
        }

        #endregion
    }
}