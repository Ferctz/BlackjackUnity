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

        private void Start()
        {
            // populate state machine with states & switch to inactive state
            stateMachine = new StateMachine<GameState>();
            stateMachine.Add(GameState.Shuffle, EnterShuffleState, null, null);
            stateMachine.Add(GameState.Betting, null, null, null);
            stateMachine.Add(GameState.Dealing, null, null, null);
            stateMachine.Add(GameState.Playing, null, null, null);
            stateMachine.Add(GameState.Result, null, null, null);            

            stateMachine.SwitchTo(GameState.Shuffle);
        }

        #region State Machine Methods

        private void EnterShuffleState()
        {
            deck = new Deck();
            seed = (int)System.DateTime.Now.Ticks;
            deck.Shuffle(seed, numberOfDecks.Value);
        }
        #endregion
    }
}