using System;
using UnityEngine;
using UnityEngine.UI;

namespace Blackjack
{
    public enum Suit : byte
    {
        Club = 0,
        Diamond = 1,
        Heart = 2,
        Spade = 3,
        COUNT = 4
    }

    public enum Rank : byte
    {
        Ace = 0,
        Two = 1,
        Three = 2,
        Four = 3,
        Five = 4,
        Six = 5,
        Seven = 6,
        Eight = 7,
        Nine = 8,
        Ten = 9,
        Jack = 10,
        Queen = 11,
        King = 12,
        COUNT = 13
    }

    public class Card : MonoBehaviour
    {
        public CardData cardData;

        public Image image;

        public void Initialize(Sprite sprite, CardData cardData)
        {
            image.sprite = sprite;
            this.cardData = cardData;
        }

        public void DisableCard()
        {
            image.sprite = null;
            cardData = default(CardData);
        }
    }

    [Serializable]
    public struct CardData
    {
        public Suit suit;
        public Rank rank;

        public bool isHidden;

        public CardData(Suit suit, Rank rank, bool isHidden)
        {
            this.suit = suit;
            this.rank = rank;
            this.isHidden = isHidden;
        }

        /// <summary>  </summary>
        /// <param name="rank"></param>
        /// <param name="isHandSoft"> a "soft" hand means aces are valued as 11, else 1 </param>
        /// <returns> Numerical value of card based on rank </returns>
        public static int CardValue(Rank rank, bool isHandSoft)
        {
            switch (rank)
            {
                case Rank.Ace:
                    return isHandSoft ? 11 : 1;
                case Rank.Two: return 2;
                case Rank.Three: return 3;
                case Rank.Four: return 4;
                case Rank.Five: return 5;
                case Rank.Six: return 6;
                case Rank.Seven: return 7;
                case Rank.Eight: return 8;
                case Rank.Nine: return 9;
                case Rank.Ten:
                case Rank.Jack:
                case Rank.Queen:
                case Rank.King:
                    return 10;
            }

            Debug.LogWarning("Invalid Rank: " + rank);
            return 0;
        }
    }
}