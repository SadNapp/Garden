using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleTest
{
    public class Creature
    {
        public string Name { get; set; }
        public int Damage { get; set; }
        public int Health { get; set; }
        public string AttackDescription { get; set; }
        public int XPValue { get; set; }

        public Creature(string name, int damage, int health, string attackDescription, int xPValue)
        {
            Name = name;
            Damage = damage;
            Health = health;
            AttackDescription = attackDescription;
            XPValue = xPValue;
        }

    }
}
