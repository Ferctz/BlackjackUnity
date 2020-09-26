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
        private StateMachine<GameState> stateMachine;

        private void Start()
        {
            // populate state machine with states & switch to inactive state
            stateMachine = new StateMachine<GameState>();
            stateMachine.Add(GameState.Shuffle, null, null, null);
            stateMachine.Add(GameState.Betting, null, null, null);
            stateMachine.Add(GameState.Dealing, null, null, null);
            stateMachine.Add(GameState.Playing, null, null, null);
            stateMachine.Add(GameState.Result, null, null, null);            

            stateMachine.SwitchTo(GameState.Shuffle);
        }
    }
}