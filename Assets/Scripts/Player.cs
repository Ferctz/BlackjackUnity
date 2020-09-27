using UnityEngine;
using UnityEngine.UI;

namespace Blackjack
{
    public class Player : Scorer
    {
        #region UI
        public Button joinButton;
        public Button aiButton;
        public Button betButton;

        public Text betText;
        public Text cashText;

        public GameObject playerTurnIndicator;
        #endregion


    }
}