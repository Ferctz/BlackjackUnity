using Blackjack.Utils;
using UnityEngine;

namespace Blackjack
{
    public class CardPool : Pool<Card>
    {
        private void Awake()
        {
            myInstantiate = InstantiateCard;
            myDisable = DisableCard;
            myReset = ResetCard;
        }

        private Card InstantiateCard(Card o, Vector3 pos, Quaternion rot)
        {
            Card card = Instantiate(o);
            return card;
        }

        private void DisableCard(Card o)
        {
            o.DisableCard();
            o.gameObject.SetActive(false);
        }

        private void ResetCard(Card o, Vector3 pos, Quaternion rot)
        {
            o.gameObject.SetActive(true);
        }
    }
}