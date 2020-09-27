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

        public void AddBet(int betAmount, int handIndex = 0)
        {
            if (betAmount > scorerData.cash)
            {
                Debug.Log("Not enough cash in wallet!");
                return;
            }

            int newCash = scorerData.cash - betAmount;
            SetCash(newCash);

            scorerData.hands[handIndex].bet += betAmount;
            betText.text = "±" + scorerData.hands[handIndex].bet.ToString();
        }

        public void SetCash(int cash, bool show = true)
        {
            scorerData.cash = cash;
            cashText.text = show ? cash.ToString() : string.Empty;
        }

        public void StartTurn()
        {
            playerTurnIndicator.SetActive(true);
        }

        public void EndTurn()
        {
            playerTurnIndicator.SetActive(false);
        }
    }
}