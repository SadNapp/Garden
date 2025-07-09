using ConsoleTest;
using System;

namespace ConsoleTest
{
    class Player
    {
        public int Energy { get; set; }
        public int XP { get; set; }
        public int Level { get; set; }
        public int Gold { get; set; }
        public int Health { get; set; } // Додано здоров'я гравця
        public int Emerald { get; set; } // Додано смарагди

        public int PesticideCount { get; set; }
        public int MedicineCount { get; set; }
        public int WaterPackCount { get; set; }
        public int FertilizerPackCount { get; set; }
        public int GrowthSerumCount { get; set; }
        public int HealthPotionCount { get; set; }

        public const int XP_PER_LEVEL = 100; // Скільки XP потрібно для рівня

        private ConsoleRenderer _renderer;

        public Player(ConsoleRenderer renderer)
        {
            _renderer = renderer;
            Reset();
        }

        public void Reset()
        {
            Energy = 100;
            XP = 0;
            Level = 1;
            Gold = 150;
            Health = 100; // Початкове здоров'я гравця
            Emerald = 0; // Початкова кількість смарагдів
            PesticideCount = 0;
            MedicineCount = 0;
            WaterPackCount = 0;
            FertilizerPackCount = 0;
            GrowthSerumCount = 0;
            HealthPotionCount = 0;
        }

        public void GainXP(int amount)
        {
            XP += amount;
            _renderer.DrawMessage($"Ви отримали {amount} XP!", ConsoleColor.DarkGreen);
            CheckLevelUp();
        }

        public void CheckLevelUp()
        {
            while (XP >= Level * XP_PER_LEVEL)
            {
                XP -= (Level * XP_PER_LEVEL); // Віднімаємо XP, необхідний для поточного рівня
                Level++; // Збільшуємо рівень
                Energy = Math.Min(100, Energy + 20); // Бонус до енергії при рівні
                Health = Math.Min(100, Health + 10); // Бонус до здоров'я при рівні
                _renderer.DrawMessage($"Вітаємо! Ви досягли Рівня {Level}!", ConsoleColor.Yellow);
                System.Threading.Thread.Sleep(1500);
                _renderer.DrawMessage("Ваша максимальна енергія та здоров'я збільшились!", ConsoleColor.Yellow);
                System.Threading.Thread.Sleep(1000);
            }
        }

        public bool TrySpendEnergy(int amount)
        {
            if (Energy >= amount)
            {
                Energy -= amount;
                return true;
            }
            _renderer.DrawMessage($"Недостатньо енергії (потрібно {amount} енергії)!", ConsoleColor.Red);
            return false;
        }

        public bool TrySpendGold(int amount)
        {
            if (Gold >= amount)
            {
                Gold -= amount;
                return true;
            }
            _renderer.DrawMessage($"Недостатньо золота (потрібно {amount} золота)!", ConsoleColor.Red);
            return false;
        }

        public void AddGold(int amount)
        {
            Gold += amount;
        }

        public void AddEmerald(int amount)
        {
            Emerald += amount;
        }

        public bool HasPesticide() { return PesticideCount > 0; }
        public void UsePesticide() { PesticideCount--; }
        public void AddPesticide(int amount) { PesticideCount += amount; }

        public bool HasMedicine() { return MedicineCount > 0; }
        public void UseMedicine() { MedicineCount--; }
        public void AddMedicine(int amount) { MedicineCount += amount; }

        public bool HasWaterPack() { return WaterPackCount > 0; }
        public void UseWaterPack() { WaterPackCount--; }
        public void AddWaterPack(int amount) { WaterPackCount += amount; }

        public bool HasFertilizerPack() { return FertilizerPackCount > 0; }
        public void UseFertilizerPack() { FertilizerPackCount--; }
        public void AddFertilizerPack(int amount) { FertilizerPackCount += amount; }

        public bool HasGrowthSerum() { return GrowthSerumCount > 0; }
        public void UseGrowthSerum() { GrowthSerumCount--; }
        public void AddGrowthSerum(int amount) { GrowthSerumCount += amount; }

        public bool HasHealthPotion() { return HealthPotionCount > 0; }
        public void UseHealthPotion() { HealthPotionCount--; }
        public void AddHealthPotion(int amount) { HealthPotionCount += amount; }

        public void RestoreEnergy(int amount)
        {
            Energy = Math.Min(100, Energy + amount);
        }

        public void TakeDamage(int amount)
        {
            Health = Math.Max(0, Health - amount);
            _renderer.DrawMessage($"Ви отримали {amount} шкоди! Залишилось здоров'я: {Health}", ConsoleColor.Red);
        }

        public void Heal(int amount)
        {
            Health = Math.Min(100, Health + amount);
            _renderer.DrawMessage($"Ви відновили {amount} здоров'я! Поточне здоров'я: {Health}", ConsoleColor.Green);
        }
    }
}