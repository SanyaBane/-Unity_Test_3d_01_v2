using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.AutoAttack
{
    public class NPCAutoAttackController : AutoAttackController
    {
        protected override void Start()
        {
            base.Start();

            UpdateAutoAttackFromSO();
        }
    }
}
