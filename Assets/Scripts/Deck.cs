using System.Collections.Generic;
using Random = System.Random;

namespace Blackjack
{
    public class Deck
    {
        private Stack<CardData> cards;
        public void Shuffle(int seed, int decks)
        {
            CardData[] allCards = new CardData[decks * 52];
            int iter = 0;
            for (int i = 0; i < decks; i++)
            {
                for (int j = 0; j < (int)Suit.COUNT; j++)
                {
                    for (int k = 0; k < (int)Rank.COUNT; k++)
                    {
                        allCards[iter] = new CardData((Suit)j, (Rank)k, false);
                        iter++;
                    }
                }
            }

            Random rand = new Random(seed);
            Utils.Utils.Shuffle(rand, allCards);

            if (cards != null)
            {
                cards.Clear();
            }
            else
            {
                cards = new Stack<CardData>(decks * 52);
            }

            for (int i = 0; i < allCards.Length; i++)
            {
                cards.Push(allCards[i]);
            }
        }

        public bool GetTopCard(out CardData card)
        {
            if (cards.Count > 0)
            {
                card = cards.Pop();
                return true;
            }

            card = default(CardData);
            return false;
        }
    }
}