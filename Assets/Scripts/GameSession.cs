using System;
using UnityEngine;

namespace Blackjack
{
    [Serializable]
    public struct GameSession
    {
        [HideInInspector]
        public int seed;
        public int minimumBet;
        public int startingPlayerCash;

        public ScorerData[] playerDatas;

        public GameSession(int seed, int minimumBet, int startingPlayerCash)
        {
            this.seed = seed;
            this.minimumBet = minimumBet;
            this.startingPlayerCash = startingPlayerCash;
            playerDatas = new ScorerData[GameConstants.MAXIMUM_PLAYER_COUNT];
        }
    }
}