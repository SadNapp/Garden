using System;

namespace ConsoleTest
{
    // Перерахування для етапів росту рослини
    enum PlantStage
    {
        Seed = 0,
        Sprout = 1,
        SmallPlant = 2,
        MediumPlant = 3,
        MaturePlant = 4,
        Flowering = 5,
        Fruiting = 6
        //Decay = 8
    }

    // Перерахування для якості рослини
    enum PlantQuality
    {
        Common,
        Uncommon,
        Rare,
        UltraRare,
        Epic,
        Legendary
    }

    // Клас, що представляє одну рослину в слоті
    class Plant
    {
        public PlantStage Stage { get; set; }
        public int Health { get; set; } // Здоров'я рослини (0-100)
        public int Water { get; set; } // Рівень води (0-10)
        public int Fertilizer { get; set; } // Рівень добрив (0-5)
        public int GrowProgress { get; set; } // Прогрес до наступного етапу
        public bool HasPest { get; set; }
        public bool HasDisease { get; set; }
        public PlantQuality Quality { get; set; } // Якість рослини
        public bool IsPlanted { get; set; } // Чи є рослина в цьому слоті

        // Конструктор за замовчуванням для нових рослин або скидання
        public Plant()
        {
            Reset(); // Ініціалізуємо рослину в початковий стан
        }

        // Метод для скидання рослини до стану "не посаджено"
        public void Reset()
        {
            Stage = PlantStage.Seed;
            Health = 100;
            Water = 5;
            Fertilizer = 0;
            GrowProgress = 0;
            HasPest = false;
            HasDisease = false;
            Quality = GenerateRandomQuality(); // Генеруємо випадкову якість при скиданні
            IsPlanted = false;
        }

        // Генерує випадкову якість рослини
        private PlantQuality GenerateRandomQuality()
        {
            // Використовуємо глобальний Random з Program, щоб уникнути однакових послідовностей
            int rand = Program.random.Next(0, 100);
            if (rand < 20) return PlantQuality.Common; // 70%
            if (rand < 40) return PlantQuality.Uncommon; // 60%
            if (rand < 50) return PlantQuality.Rare; // 50%
            if(rand < 70) return PlantQuality.UltraRare; // 30%
            if(rand < 80) return PlantQuality.Epic; // 20%             
            return PlantQuality.Legendary; // 5%
        }

        // Отримує множник XP/Золота залежно від якості
        public double GetQualityMultiplier()
        {
            switch (Quality)
            {
                case PlantQuality.Common: return 1.0;
                case PlantQuality.Uncommon: return 2.2;
                case PlantQuality.Rare: return 3.5;
                case PlantQuality.UltraRare: return 5.0;
                case PlantQuality.Epic: return 10.0;
                case PlantQuality.Legendary: return 50.0;
                default: return 1.0;
            }
        }
    }
}