using System;
using System.Diagnostics;

namespace Assets.Scripts.SerializableData
{
    [DebuggerDisplay("ActionCellData - ActionCellIndex: {ActionCellIndex}; ActionId: {ActionId};")]
    [Serializable]
    public class ActionCellData
    {
        public int ActionCellIndex;

        public string ActionId;
    }
}
