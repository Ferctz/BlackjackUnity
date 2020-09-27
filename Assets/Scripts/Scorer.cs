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
        public ScorerData scorerData;

        #region UI
        public Text scoreText;
        public Text handState;
        public Transform handRoot;
        #endregion

        public virtual void Initialize(int startingCash)
        {
            scorerData.cash = startingCash;
            scorerData.hands = new List<Hand>();
            Hand hand = new Hand();
            scorerData.hands.Add(hand);
        }

        public void AddCard(Card card)
        {
            scorerData.hands[0].handCards.Add(card);

            card.transform.SetParent(handRoot);
            card.transform.localPosition = Vector3.zero;
            card.transform.localScale = Vector3.one;
        }

        /// <summary> Generate a score based on cards in hand as per blackjack rules </summary>
        public void UpdateScore()
        {
            // getting a hand's score as soft (ie. aces are 11s)
            int score = 0;
            for (int i = 0; i < scorerData.hands[0].handCards.Count; i++)
            {
                if (scorerData.hands[0].handCards[i].cardData.isHidden)
                {
                    continue;
                }
                score += CardData.CardValue(scorerData.hands[0].handCards[i].cardData.rank, true);
            }

            // if soft hand score is a bust, try to score as hard (ie. aces are 1s)
            if (score > 21)
            {
                score = 0;
                for (int i = 0; i < scorerData.hands[0].handCards.Count; i++)
                {
                    score += CardData.CardValue(scorerData.hands[0].handCards[i].cardData.rank, false);
                }

                if (score > 21)
                {
                    SetHandState(HandState.Bust);
                }
            }

            scorerData.hands[0].score = score;
            scoreText.text = score.ToString();
        }

        public void SetHandState(HandState state)
        {
            scorerData.hands[0].handState = state;

            handState.text = state.ToString();
        }
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