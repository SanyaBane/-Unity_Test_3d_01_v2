using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
