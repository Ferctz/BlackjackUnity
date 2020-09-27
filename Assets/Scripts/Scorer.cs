using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Blackjack
{
    public enum HandState : byte
    {
        Invalid = 0,
        Playing = 1,
        Stand = 2,
        Bust = 3,
        Blackjack = 4,
        Win = 5,
        Lose = 6
    }

    public class Scorer : MonoBehaviour
    {
        public ScorerData playerData;

        #region UI
        public Text scoreText;
        public Text handState;
        #endregion
    }

    [Serializable]
    public struct ScorerData
    {
        public int cash;

        [SerializeField]
        public List<Hand> hands;

        public ScorerData(ScorerData playerData)
        {
            cash = playerData.cash;
            hands = playerData.hands;
        }
    }

    [Serializable]
    public class Hand
    {
        public HandState handState;
        public int bet;
        public int score;
        public List<Card> handCards;

        public Hand()
        {
            handCards = new List<Card>();
        }
    }

    
}