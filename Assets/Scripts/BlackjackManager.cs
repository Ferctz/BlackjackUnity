using Blackjack.Utils;
using System.Collections.Generic;
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
        private List<Card> cardsDealt = new List<Card>();

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
            standButton.onClick.AddListener(PlayerStand);
            hitButton.onClick.AddListener(PlayerHit);
            doubleButton.onClick.AddListener(PlayerDouble);
            rebetButton.onClick.AddListener(Rebet);
            newGameButton.onClick.AddListener(NewGame);
            settingsButton.onClick.AddListener(() => EnableSettingsScreen(true));
            settingsBackButton.onClick.AddListener(() => EnableSettingsScreen(false));
            minimumBetSlider.onValueChanged.AddListener(MinimumBetChanged);
            startingCashSlider.onValueChanged.AddListener(StartingCashChanged);

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
            stateMachine.Add(GameState.Result, EnterResultState, null, ExitResultState);            

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

                    cardsDealt.Add(card);
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

            StartDealerTurn();
        }

        private void StartDealerTurn()
        {
            // unhide dealer's first card
            Card hiddenCard = dealer.scorerData.hands[0].handCards[0];
            hiddenCard.cardData.isHidden = false;
            Sprite cardSprite;
            if (GetCardSprite(hiddenCard.cardData.suit, hiddenCard.cardData.rank, out cardSprite))
            {
                hiddenCard.Initialize(cardSprite, hiddenCard.cardData);
            }

            dealer.UpdateScore();

            while (dealer.scorerData.hands[0].score < 17)
            {
                DealCard(dealer, false);
                dealer.UpdateScore();
            }

            if (dealer.scorerData.hands[0].score <= 21)
            {
                dealer.SetHandState(HandState.Stand);
            }
        }

        private void ShowPlayerActionButtons(bool isVisible)
        {
            standButton.gameObject.SetActive(isVisible);
            hitButton.gameObject.SetActive(isVisible);
            doubleButton.gameObject.SetActive(isVisible);
        }

        private void ReturnCards()
        {
            for (int i = 0; i < cardsDealt.Count; i++)
            {
                cardPool.Release(cardsDealt[i]);
            }
            cardsDealt.Clear();
        }

        private void EnableSettingsScreen(bool enabled)
        {
            settingsScreen.interactable = enabled;
            settingsScreen.alpha = enabled ? 1f : 0f;

            minimumBetSlider.value = minimumBetAmount.Value;
            minimumBetValueText.text = minimumBetAmount.Value.ToString();
            startingCashSlider.value = startingPlayerCash.Value;
            startingCashValueText.text = startingPlayerCash.Value.ToString();
        }

        private void MinimumBetChanged(float value)
        {
            minimumBetAmount.Value = (int)value;
            minimumBetValueText.text = minimumBetAmount.Value.ToString();
        }

        private void StartingCashChanged(float value)
        {
            // if starting cash changes, min bet cannot exceed starting cash
            minimumBetSlider.maxValue = (int)value;

            startingPlayerCash.Value = (int)value;
            startingCashValueText.text = startingPlayerCash.Value.ToString();
        }

        #region UI Actions
        private void DealCards()
        {
            stateMachine.SwitchTo(GameState.Dealing);
        }

        private void PlayerStand()
        {
            players[currentPlayerTurnIndex].SetHandState(HandState.Stand);

            StartNextPlayerTurn();
        }

        private void PlayerHit()
        {
            DealCard(players[currentPlayerTurnIndex], false);

            // update score
            players[currentPlayerTurnIndex].UpdateScore();

            if (players[currentPlayerTurnIndex].scorerData.hands[0].handState == HandState.Bust)
            {
                StartNextPlayerTurn();
            }
        }

        private void PlayerDouble()
        {
            AddBet(currentPlayerTurnIndex);
            DealCard(players[currentPlayerTurnIndex], false);

            // update score
            players[currentPlayerTurnIndex].UpdateScore();

            if (players[currentPlayerTurnIndex].scorerData.hands[0].handState == HandState.Bust)
            {
                players[currentPlayerTurnIndex].scorerData.hands[0].handState = HandState.Stand;
            }

            StartNextPlayerTurn();
        }

        private void Rebet()
        {
            ReturnCards();
            dealer.ClearHand();
            dealer.Initialize(startingPlayerCash.Value);

            for (int i = 0; i < players.Length; i++)
            {
                if (players[i].scorerData.hands == null ||
                    players[i].scorerData.hands.Count == 0)
                {
                    continue;
                }

                players[i].ClearHand();
                players[i].Initialize(players[i].scorerData.cash);
            }

            if (deck.GetCardCount() < 52)
            {
                stateMachine.SwitchTo(GameState.Shuffle);
            }
            else
            {
                stateMachine.SwitchTo(GameState.Betting);
            }
        }

        private void NewGame()
        {
            ReturnCards();
            dealer.ClearHand();

            for (int i = 0; i < players.Length; i++)
            {
                if (players[i].scorerData.hands == null ||
                    players[i].scorerData.hands.Count == 0)
                {
                    continue;
                }

                players[i].ClearHand();
                players[i].SetCash(0, false);
            }

            stateMachine.SwitchTo(GameState.Shuffle);
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

        private void EnterResultState()
        {
            // compare player hands with dealer
            for (int i = 0; i < players.Length; i++)
            {
                // ie if player not initialized nor in this round
                if (players[i].scorerData.hands == null || players[i].scorerData.hands.Count == 0 ||
                    players[i].scorerData.hands[0].bet == 0)
                {
                    continue;
                }

                // TODO: once hand splitting is complete, iterate through all hands for possible wins
                if (players[i].scorerData.hands[0].handState == HandState.Bust)
                {
                    continue;
                }

                // award payout if hand score greater than dealer's, or if dealer has bust
                if (players[i].scorerData.hands[0].score > dealer.scorerData.hands[0].score ||
                    dealer.scorerData.hands[0].handState == HandState.Bust)
                {
                    players[i].SetHandState(HandState.Win);
                    int winnings = 2 * players[i].scorerData.hands[0].bet;
                    players[i].Payout(winnings);
                }
                else
                {
                    players[i].SetHandState(HandState.Lose);
                }
            }

            rebetButton.gameObject.SetActive(true);
            newGameButton.gameObject.SetActive(true);
        }

        private void ExitResultState()
        {
            rebetButton.gameObject.SetActive(false);
            newGameButton.gameObject.SetActive(false);
        }
        #endregion
    }
}