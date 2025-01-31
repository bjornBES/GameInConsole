using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameInConsole.Game.Data
{
    public class Stats
    {
        public int HP { get; set; }
        public int AttackPower { get; set; }
        
        public Stats(int hp, int attackPower)
        {
            HP = hp;
            AttackPower = attackPower;
        }
    }
}
