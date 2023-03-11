using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.HorizontalMultipleRowsLayoutGroup
{
    public class HorizontalLayoutRow : MonoBehaviour
    {
        private HorizontalLayoutGroup _HorizontalLayoutGroup;
        public HorizontalLayoutGroup HorizontalLayoutGroup
        {
            get
            {
                if (_HorizontalLayoutGroup == null)
                    _HorizontalLayoutGroup = this.GetComponent<HorizontalLayoutGroup>();

                return _HorizontalLayoutGroup;
            }
        }
    }
}