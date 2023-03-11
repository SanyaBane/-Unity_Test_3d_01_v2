using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.UI.PlayerMessages
{
    public class PlayerErrorMessageContainer : MonoBehaviour
    {
        public List<PlayerErrorMessage> ErrorMessages;

        public void DisplayErrorMessage(string text)
        {
            //Debug.Log(text);
        
            var oldestErrorMessage = ErrorMessages.OrderBy(x => x.LastTimeDisplayed).First();
        
            oldestErrorMessage.DisplayErrorMessage(text);
            oldestErrorMessage.transform.SetAsFirstSibling();
        }
    }
}
