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

        [SerializeField] private CardPool cardPool;

        private int currentPlayerTurnIndex = -1;

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
            stateMachine.Add(GameState.Dealing, EnterDealingState, UpdateDealingState, null);
            stateMachine.Add(GameState.Playing, EnterPlayingState, UpdatePlayingState, null);
            stateMachine.Add(GameState.Result, null, null, null);            

            stateMachine.SwitchTo(GameState.Shuffle);
        }

        private void Update()
        {
            stateMachine.Update();
        }

        /// <summary> When a player hits "Join". Initializes a player to play bets </summary>
        /// <param name="playerId"> index of player from players array </param>
        public void AddPlayer(int playerId)
        {
            if (playerId < 0 || playerId > GameConstants.MAXIMUM_PLAYER_COUNT)
            {
                Debug.LogWarning("Invalid player Id");
                return;
            }

            players[playerId].Initialize(startingPlayerCash.Value);
        }

        /// <summary> Adds a bet matching minimum bet from player's wallet to enter dealing phase </summary>
        /// <param name="playerId"></param>
        public void AddBet(int playerId)
        {
            if (playerId < 0 || playerId > GameConstants.MAXIMUM_PLAYER_COUNT)
            {
                Debug.LogWarning("Invalid player Id");
                return;
            }

            players[playerId].AddBet(minimumBetAmount.Value);
        }

        private void DealCard(Scorer scorer, bool isHidden)
        {
            CardData cardData;
            if (deck.GetTopCard(out cardData))
            {
                cardData.isHidden = isHidden;
                Sprite cardSprite = backsideSprite;
                if (isHidden || GetCardSprite(cardData.suit, cardData.rank, out cardSprite))
                {
                    Card card = cardPool.Create(Vector3.zero, Quaternion.identity);
                    card.Initialize(cardSprite, cardData);

                    scorer.AddCard(card);
                }
            }
        }

        private bool GetCardSprite(Suit suit, Rank rank, out Sprite sprite)
        {
            sprite = null;
            if ((suit < 0 || suit >= Suit.COUNT) ||
                (rank < 0 || rank >= Rank.COUNT))
            {
                Debug.LogWarning("Invalid Suit or Rank! Suit: " + suit + ", Rank: " + rank);
                return false;
            }

            int spriteIndex = ((int)suit * (int)Rank.COUNT) + (int)rank;
            sprite = cardSprites[spriteIndex];

            return true;
        }

        private void StartNextPlayerTurn()
        {
            if (currentPlayerTurnIndex >= 0)
            {
                players[currentPlayerTurnIndex].EndTurn();
            }

            currentPlayerTurnIndex++;
            while (currentPlayerTurnIndex < players.Length)
            {
                if (players[currentPlayerTurnIndex].scorerData.hands == null ||
                    players[currentPlayerTurnIndex].scorerData.hands.Count == 0 ||
                    players[currentPlayerTurnIndex].scorerData.hands[0].bet == 0)
                {
                    currentPlayerTurnIndex++;
                    continue;
                }
                else // ie. valid player
                {
                    players[currentPlayerTurnIndex].StartTurn();
                    return;
                }
            }

            // no players left to go to, dealer's turn
            ShowPlayerActionButtons(false);

            // go to dealer
        }

        private void ShowPlayerActionButtons(bool isVisible)
        {
            standButton.gameObject.SetActive(isVisible);
            hitButton.gameObject.SetActive(isVisible);
            doubleButton.gameObject.SetActive(isVisible);
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

        private void UpdateDealingState()
        {
            // TODO: modify this into an elegant wait and do in between each deal
            // for now just deal all cards at once and advance to next state
            for (int i = 0; i < players.Length; i++)
            {
                if (players[i].scorerData.hands == null ||
                    players[i].scorerData.hands.Count == 0 ||
                    players[i].scorerData.hands[0].bet == 0)
                {
                    continue;
                }
                DealCard(players[i], false);
                players[i].UpdateScore();
            }
            DealCard(dealer, true);

            for (int i = 0; i < players.Length; i++)
            {
                if (players[i].scorerData.hands == null ||
                    players[i].scorerData.hands.Count == 0 ||
                    players[i].scorerData.hands[0].bet == 0)
                {
                    continue;
                }
                DealCard(players[i], false);
                players[i].UpdateScore();
            }

            DealCard(dealer, false);
            dealer.UpdateScore();

            stateMachine.SwitchTo(GameState.Playing);
        }

        private void EnterPlayingState()
        {
            ShowPlayerActionButtons(true);

            currentPlayerTurnIndex = -1;
            StartNextPlayerTurn();
        }

        private void UpdatePlayingState()
        {
            // advance to results state only once dealer is in stand or bust
            if (dealer.scorerData.hands[0].handState == HandState.Stand ||
                dealer.scorerData.hands[0].handState == HandState.Bust)
            {
                stateMachine.SwitchTo(GameState.Result);
            }
        }
        #endregion
    }
}