using System;
using UnityEngine;

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
    }
}