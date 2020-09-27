using Blackjack.Utils;
using UnityEngine;

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

        [SerializeField] private Dealer dealer;
        [SerializeField] private Player[] players;

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