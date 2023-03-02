using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.SerializableData
{
    [Serializable]
    public class ActionBarsDataHolder
    {
        public List<ActionBarData> ListActionBarData = new List<ActionBarData>();

        public static void SerializeActionBarsDataHolder(ActionBarsDataHolder actionBarsDataHolder)
        {
            var newSerializedActionBarsDataHolder = JsonUtility.ToJson(actionBarsDataHolder);
            PlayerPrefs.SetString(Constants.PLAYER_PREFS_KEY_LIST_ACTION_BARS, newSerializedActionBarsDataHolder);
        }

        public static ActionBarsDataHolder DeserializeActionBarsDataHolder()
        {
            if (!PlayerPrefs.HasKey(Constants.PLAYER_PREFS_KEY_LIST_ACTION_BARS))
                return new ActionBarsDataHolder();

            var serializedActionBarsDataHolder = PlayerPrefs.GetString(Constants.PLAYER_PREFS_KEY_LIST_ACTION_BARS, null);
            var deserializedActionBarsDataHolder = JsonUtility.FromJson<ActionBarsDataHolder>(serializedActionBarsDataHolder);
            return deserializedActionBarsDataHolder;
        }
    }
}
