using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogueLikeGame
{
    internal class Weapon
    {
        public string name;
        public int damage;
        public int reloadSpeed;
        public int reloadCounter;
        public int projectileSpeed;

        public Weapon(string name, int damage, int reloadSpeed, int reloadCounter, int projectileSpeed) 
        { 
            this.name = name;
            this.damage = damage;
            this.reloadSpeed = reloadSpeed;
            this.reloadCounter = reloadCounter;
            this.projectileSpeed = projectileSpeed;
        }
    }
}