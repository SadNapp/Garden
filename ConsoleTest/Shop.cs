using ConsoleTest;
using System;
using System.Linq;

namespace ConsoleTest
{
    class Shop
    {
        private Player _player;
        private ConsoleRenderer _renderer;
        private Plant[] _plantSlots; // Потрібно для перевірки вільних слотів для насіння

        public Shop(Player player, ConsoleRenderer renderer, Plant[] plantSlots)
        {
            _player = player;
            _renderer = renderer;
            _plantSlots = plantSlots;
        }

        public void ShowShop()
        {
            _renderer.ShowShopUI(_player); // Викликаємо новий метод відображення магазину з ConsoleRenderer
        }

        public void BuyItem(string item)
        {
            int cost = 0;
            string itemName = "";
            bool success = false;

            switch (item)
            {
                case "насіння":
                    cost = 10;
                    itemName = "Насіння";
                    if (_player.Gold >= cost && _plantSlots.Any(p => !p.IsPlanted)) // Перевіряємо наявність вільного слоту
                    {
                        _player.TrySpendGold(cost); // Вже перевірено наявність золота
                        _renderer.DrawMessage($"Ви купили {itemName} за {cost} золота. Тепер посадіть його командою 'посадити [номер_слоту]'.", ConsoleColor.Green);
                        success = true;
                    }
                    else if (_player.Gold < cost)
                    {
                        _renderer.DrawMessage($"Недостатньо золота для покупки {itemName}.", ConsoleColor.Red);
                    }
                    else
                    {
                        _renderer.DrawMessage("Немає вільних слотів для посадки насіння!", ConsoleColor.Red);
                    }
                    return;
                case "вода":
                    cost = 5;
                    itemName = "Вода";
                    if (_player.TrySpendGold(cost))
                    {
                        _player.AddWaterPack(1);
                        success = true;
                    }
                    break;
                case "добрива":
                    cost = 10;
                    itemName = "Добрива";
                    if (_player.TrySpendGold(cost))
                    {
                        _player.AddFertilizerPack(1);
                        success = true;
                    }
                    break;
                case "інсектицид":
                    cost = 15;
                    itemName = "Інсектицид";
                    if (_player.TrySpendGold(cost))
                    {
                        _player.AddPesticide(1);
                        success = true;
                    }
                    break;
                case "ліки":
                    cost = 20;
                    itemName = "Ліки";
                    if (_player.TrySpendGold(cost))
                    {
                        _player.AddMedicine(1);
                        success = true;
                    }
                    break;
                case "сироватка":
                    cost = 30;
                    itemName = "Сироватка росту";
                    if (_player.TrySpendGold(cost))
                    {
                        _player.AddGrowthSerum(1);
                        success = true;
                    }
                    break;
                case "зілля_здоров'я":
                    cost = 25;
                    itemName = "Зілля здоров'я";
                    if (_player.TrySpendGold(cost))
                    {
                        _player.AddHealthPotion(1);
                        success = true;
                    }
                    break;
                default:
                    _renderer.DrawMessage($"Невідомий товар: '{item}'.", ConsoleColor.Red);
                    return;
            }

            if (success)
            {
                _renderer.DrawMessage($"Ви купили {itemName} за {cost} золота.", ConsoleColor.Green);
            }
            else
            {
                // Повідомлення про недостатньо золота вже виводиться через TrySpendGold
            }
        }
    }
}