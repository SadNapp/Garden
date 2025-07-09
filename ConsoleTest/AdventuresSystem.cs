using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleTest
{
    class AdventuresSystem
    {
        private Player _player;
        private ConsoleRenderer _renderer;
        private GameManager _gameManager;
        public bool isActive = false;


        // Ім'я, Шкода, Здоров'я, Опис атаки, XP за перемогу
        private Creature[] _creatures = new Creature[]
        {
         new Creature("Маленький Слимак", 5, 10, "Слимак повзе по нозі, завдаючи", 5),
         new Creature("Голодний Гремлін", 10, 20, "Кролик кусає вас за палець, завдаючи", 10),
         new Creature("Агресивний Кабан", 15, 30, "Ворона дзьобає вас, завдаючи", 15),
         new Creature("Дикий Вовк", 20, 40, "Їжак коле вас голками, завдаючи", 20),
         new Creature("Великий Гремлін", 25, 50, "Змія обвиває вас і стискає, завдаючи", 30),
         new Creature("Лісовий Дух", 0, 100, "Лісовий дух лякає вас, але не завдає фізичної шкоди", 0) // Істота, яка не завдає шкоди, але може мати інші ефекти
        };

        public AdventuresSystem(Player player, ConsoleRenderer renderer, GameManager gameManager)
        {
            _player = player;
            _renderer = renderer;
            _gameManager = gameManager;
        }

        public void StartAdventure()
        {
            _renderer.DrawMessage("Ви вирушаєте у пригоду...", ConsoleColor.DarkCyan);
            Thread.Sleep(1000);

            if(!_player.TrySpendEnergy(20))
            {
                _renderer.DrawMessage("Вам потрібно 20 енергії, щоб вирушити у пригоду.", ConsoleColor.Red);
                return;
            }

            int eventRoll = Program.random.Next(0, 100);
            if(eventRoll < 60)
            {
                FindItem();
            }
            else
            {
                EncountCreature();
            }
            _player.GainXP(10);
        }

        private void FindItem()
        {
            _renderer.DrawMessage("Ви досліджуєте місцевість і щось знаходите!", ConsoleColor.Green);
            Thread.Sleep(1000);

            int itemRoll = Program.random.Next(0, 100);
            if (itemRoll < 30)
            {
                _player.AddWaterPack(1);
                _renderer.DrawMessage("Ви знайшли пакет води! (+1 вода)", ConsoleColor.Blue);
            }
            else if(itemRoll < 90)
            {
                _player.AddHealthPotion(1);
                _renderer.DrawMessage("Ви знайшли зілля здоров'я! (+1 зілля здоров'я)", ConsoleColor.Red);
            }
            else
            {
               _player.AddGrowthSerum(1);
                _renderer.DrawMessage("Ви знайшли сироватку росту! (+1 сироватка росту)", ConsoleColor.Magenta);
            }
            Thread.Sleep(1000);
        }

        private void EncountCreature()
        {
            Creature encountCreature = _creatures[Program.random.Next(0, _creatures.Length)];
            _renderer.DrawMessage($"На вашому шляху з'явилася {encountCreature.Name}!", ConsoleColor.DarkRed);
            Thread.Sleep(1000);
            _renderer.DrawMessage($"очати битву з {encountCreature.Name}", ConsoleColor.DarkRed);
            
            switch(string.Empty)
            {
                case "битва":
                    Batlle();
                break;

                case "идти":
                    //пропустити бій
                    break;                   
            }
            Console.ReadLine();
            _renderer.DrawMessage($"Вона атакує вас і завдає {encountCreature.Damage} шкоди!", ConsoleColor.Red);
            _player.TakeDamage(encountCreature.Damage);
            Thread.Sleep(1500);
        }
                
        private void Batlle()
        {
            
        }
    }
}
