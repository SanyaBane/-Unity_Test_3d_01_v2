using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Assets.Scripts.SerializableData
{
    [DebuggerDisplay("ActionBarData - EJob: {Job}; ActionBarIndex: {ActionBarIndex};")]
    [Serializable]
    public class ActionBarData
    {
        public EJob Job;

        public int ActionBarIndex;

        public List<ActionCellData> ListActionCellData;

        public ActionBarData()
        {
            ListActionCellData = new List<ActionCellData>();

            for (int i = 0; i < 12; i++)
            {
                ListActionCellData.Add(new ActionCellData()
                {
                    ActionCellIndex = i,
                    ActionId = null
                });
            }
        }
    }
}
