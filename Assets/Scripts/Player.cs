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

        public override void Initialize(int startingCash)
        {
            base.Initialize(startingCash);

            joinButton.gameObject.SetActive(false);
            aiButton.gameObject.SetActive(false);
            betButton.gameObject.SetActive(true);
            betText.gameObject.SetActive(true);
            cashText.gameObject.SetActive(true);

            cashText.text = startingCash.ToString();
        }
    }
}