using ConsoleTest;
using System;
using System.Linq; // Для String.Join

namespace ConsoleTest
{
    // Перерахування для типів погоди (повторно, щоб уникнути залежності від WeatherSystem)
    class ConsoleRenderer
    {
        private int consoleWidth;
        private int consoleHeight;

        public ConsoleRenderer()
        {
            consoleWidth = Console.WindowWidth;
            consoleHeight = Console.WindowHeight;
        }

        // Метод для малювання символу в певних координатах з кольором
        public void DrawChar(char character, int x, int y, ConsoleColor color)
        {
            if (x >= 0 && x < consoleWidth && y >= 0 && y < consoleHeight)
            {
                Console.SetCursorPosition(x, y);
                Console.ForegroundColor = color;
                Console.Write(character);
                Console.ResetColor();
            }
        }

        // Метод для малювання тексту в певних координатах з кольором
        public void DrawText(string text, int x, int y, ConsoleColor color)
        {
            if (x >= 0 && x < consoleWidth && y >= 0 && y < consoleHeight)
            {
                Console.SetCursorPosition(x, y);
                Console.ForegroundColor = color;
                Console.Write(text);
                Console.ResetColor();
            }
        }

        // Метод для малювання рамки
        public void DrawBorder(ConsoleColor color)
        {
            Console.ForegroundColor = color;

            // Верхня горизонтальна лінія
            for (int x = 0; x < consoleWidth; x++)
            {
                Console.SetCursorPosition(x, 0);
                Console.Write("═");
                Console.SetCursorPosition(x, consoleHeight - 1);
                Console.Write("═");
            }

            // Ліва та права вертикальні лінії
            for (int y = 0; y < consoleHeight; y++)
            {
                Console.SetCursorPosition(0, y);
                Console.Write("║");
                Console.SetCursorPosition(consoleWidth - 1, y);
                Console.Write("║");
            }

            // Кути
            Console.SetCursorPosition(0, 0); Console.Write("╔");
            Console.SetCursorPosition(consoleWidth - 1, 0); Console.Write("╗");
            Console.SetCursorPosition(0, consoleHeight - 1); Console.Write("╚");
            Console.SetCursorPosition(consoleWidth - 1, consoleHeight - 1); Console.Write("╝");

            Console.ResetColor();
        }

        // Метод для малювання рослин у слотах
        public void DrawPlantsInSlots(Plant[] plantSlots, int progressNeededForNextStage)
        {
            int startX = 10;
            int startY = 22; // Змінено Y-координату для рослин, щоб звільнити місце для роботи

            for (int i = 0; i < plantSlots.Length; i++)
            {
                Plant p = plantSlots[i];
                int currentX = startX + i * 35; // Зміщення для кожного слоту (збільшено)

                // Очищаємо область слоту
                for (int row = 0; row < 10; row++)
                {
                    DrawText(new string(' ', 30), currentX - 5, startY - 6 + row, ConsoleColor.Black); // Збільшено область очищення
                }

                DrawText($"--- Слот {i + 1} ---", currentX - 5, startY - 6, ConsoleColor.Gray);

                if (p.IsPlanted)
                {
                    DrawPlantVisual(currentX, startY, p.Stage);
                    DrawText($"Якість: {p.Quality}", currentX - 5, startY + 3, ConsoleColor.Magenta);
                    DrawText($"Здоров'я: {p.Health}/100 {DrawProgressBar(p.Health, 100, 10, GetHealthColor(p.Health), ConsoleColor.DarkRed)}", currentX - 5, startY + 4, GetHealthColor(p.Health));
                    DrawText($"Вода: {p.Water}/10 {DrawProgressBar(p.Water, 10, 10, GetWaterColor(p.Water), ConsoleColor.DarkBlue)}", currentX - 5, startY + 5, GetWaterColor(p.Water));
                    DrawText($"Добрива: {p.Fertilizer}/5 {DrawProgressBar(p.Fertilizer, 5, 10, GetFertilizerColor(p.Fertilizer), ConsoleColor.DarkYellow)}", currentX - 5, startY + 6, GetFertilizerColor(p.Fertilizer));
                    DrawText($"Прогрес: {p.GrowProgress}/{progressNeededForNextStage} {DrawProgressBar(p.GrowProgress, progressNeededForNextStage, 10, ConsoleColor.Green, ConsoleColor.DarkGreen)}", currentX - 5, startY + 7, ConsoleColor.DarkGreen);

                    if (p.HasPest) DrawText("🐛", currentX - 7, startY - 1, ConsoleColor.Red);
                    if (p.HasDisease) DrawText("🦠", currentX + 7, startY - 1, ConsoleColor.DarkRed);
                }
                else
                {
                    DrawText("[Пусто]", currentX - 2, startY, ConsoleColor.DarkGray);
                }
            }
        }

        // Метод для малювання візуального представлення рослини залежно від етапу росту
        private void DrawPlantVisual(int x, int y, PlantStage stage)
        {
            // Очищаємо область, де буде малюватися рослина
            for (int i = 0; i < 5; i++) // Висота малюнка
            {
                DrawText(new string(' ', 15), x - 7, y - 4 + i, ConsoleColor.Black);
            }

            // Малювання базових елементів та кореня
            DrawText("  _|_", x - 2, y + 1, ConsoleColor.DarkYellow); // Коренева система/основа

            switch (stage)
            {
                case PlantStage.Seed:
                    DrawChar('●', x, y, ConsoleColor.DarkYellow);
                    break;
                case PlantStage.Sprout:
                    DrawText("  ^", x - 1, y - 1, ConsoleColor.Green);
                    DrawText(" / \\", x - 2, y, ConsoleColor.Green);
                    break;
                case PlantStage.SmallPlant:
                    DrawText("  /\\", x - 1, y - 2, ConsoleColor.Green);
                    DrawText(" /__\\", x - 2, y - 1, ConsoleColor.Green);
                    DrawText("  ||", x - 1, y, ConsoleColor.DarkYellow);
                    break;
                case PlantStage.MediumPlant:
                    DrawText("  _--_", x - 2, y - 3, ConsoleColor.Green);
                    DrawText(" /____\\", x - 3, y - 2, ConsoleColor.Green);
                    DrawText(" |    |", x - 3, y - 1, ConsoleColor.DarkYellow);
                    DrawText("  ||", x - 1, y, ConsoleColor.DarkYellow);
                    break;
                case PlantStage.MaturePlant:
                    DrawText("    /\\", x - 2, y - 4, ConsoleColor.Green);
                    DrawText("   /  \\", x - 3, y - 3, ConsoleColor.Green);
                    DrawText("  /____\\", x - 4, y - 2, ConsoleColor.Green);
                    DrawText(" |      |", x - 4, y - 1, ConsoleColor.DarkYellow);
                    DrawText("  ||", x - 1, y, ConsoleColor.DarkYellow);
                    break;
                case PlantStage.Flowering:
                    DrawText("    /\\", x - 2, y - 4, ConsoleColor.Green);
                    DrawText("   /  \\", x - 3, y - 3, ConsoleColor.Green);
                    DrawText("  /____\\", x - 4, y - 2, ConsoleColor.Green);
                    DrawText(" |  @   |", x - 4, y - 1, ConsoleColor.DarkYellow); // Квітка
                    DrawText("  ||", x - 1, y, ConsoleColor.DarkYellow);
                    break;
                case PlantStage.Fruiting:
                    DrawText("    /\\", x - 2, y - 4, ConsoleColor.Green);
                    DrawText("   /  \\", x - 3, y - 3, ConsoleColor.Green);
                    DrawText("  /____\\", x - 4, y - 2, ConsoleColor.Green);
                    DrawText(" |  O   |", x - 4, y - 1, ConsoleColor.DarkYellow); // Плід
                    DrawText("  ||", x - 1, y, ConsoleColor.DarkYellow);
                    break;
            }
        }

        // Допоміжний метод для малювання прогрес-бару
        public string DrawProgressBar(int current, int max, int width, ConsoleColor barColor, ConsoleColor bgColor)
        {
            if (max == 0) return new string('─', width); // Уникнути ділення на нуль
            int progress = (int)Math.Round((double)current / max * width);
            string progressBar = new string('█', progress) + new string('─', width - progress);
            return $"[{progressBar}]"; // Повертаємо рядок, щоб потім встановити колір
        }

        public ConsoleColor GetHealthColor(int health)
        {
            if (health < 30) return ConsoleColor.Red;
            if (health < 70) return ConsoleColor.Yellow;
            return ConsoleColor.Green;
        }

        public ConsoleColor GetWaterColor(int water)
        {
            if (water < 3) return ConsoleColor.Red;
            if (water < 6) return ConsoleColor.Yellow;
            return ConsoleColor.Blue;
        }

        public ConsoleColor GetFertilizerColor(int fertilizer)
        {
            if (fertilizer < 1) return ConsoleColor.Red;
            if (fertilizer < 3) return ConsoleColor.Yellow;
            return ConsoleColor.DarkYellow;
        }

        public void DrawMessage(string message, ConsoleColor color)
        {
            // Очищаємо область повідомлень перед виводом нового
            DrawText(new string(' ', consoleWidth - 4), 2, consoleHeight - 7, ConsoleColor.Black); // Рядок для повідомлень
            DrawText(message, 2, consoleHeight - 7, color);
            System.Threading.Thread.Sleep(1000); // Пауза, щоб користувач міг прочитати повідомлення
        }

        public void ShowWelcomeScreen()
        {
            Console.Clear();
            DrawBorder(ConsoleColor.DarkGreen);
            DrawText("███╗   ███╗ ██████╗ ██████╗  ██████╗ ██╗    ██╗", 25, 5, ConsoleColor.Green);
            DrawText("████╗ ████║██╔═══██╗██╔══██╗██╔═══██╗██║    ██║", 25, 6, ConsoleColor.Green);
            DrawText("██╔████╔██║██║   ██║██████╔╝██║   ██║██║ █╗ ██║", 25, 7, ConsoleColor.Green);
            DrawText("██║╚██╔╝██║██║   ██║██╔══██╗██║   ██║██║███╗██║", 25, 8, ConsoleColor.Green);
            DrawText("██║ ╚═╝ ██║╚██████╔╝██║  ██║╚██████╔╝╚██████╔╝", 25, 9, ConsoleColor.Green);
            DrawText("╚═╝     ╚═╝ ╚═════╝ ╚═╝  ╚═╝ ╚═════╝  ╚═════╝ ", 25, 10, ConsoleColor.Green);
            DrawText("Ласкаво просимо до вашого Консольного Городу!", 28, 15, ConsoleColor.White);
            DrawText("Версія 1.4.0 - Модульна Архітектура!", 20, 17, ConsoleColor.Yellow);
            DrawText("Натисніть будь-яку клавішу, щоб почати...", 30, 20, ConsoleColor.Gray);
            Console.ReadKey(true);
        }

        public void ShowGameOverScreen(int gold, int playerLevel, int jobLevel)
        {
            Console.Clear();
            DrawBorder(ConsoleColor.DarkGreen);
            // Перевіряємо, чи була хоча б одна рослина успішно зібрана
            if (gold > 0 || playerLevel > 1 || jobLevel > 0) // Простий індикатор успіху
            {
                DrawText("🎉🎉🎉 ПЕРЕМОГА! 🎉🎉�", 38, 10, ConsoleColor.Yellow);
                DrawText("Ви успішно вирощували та заробляли!", 30, 12, ConsoleColor.Green);
                DrawText($"Ви заробили {gold} золота!", 38, 14, ConsoleColor.Yellow);
                DrawText($"Досягнутий рівень: {playerLevel}!", 38, 15, ConsoleColor.Cyan);
                if (jobLevel > 0)
                {
                    DrawText($"Рівень роботи: {jobLevel}!", 38, 16, ConsoleColor.DarkGreen);
                }
            }
            else
            {
                DrawText("💔💔💔 ГРА ЗАВЕРШЕНА 💔💔💔", 35, 10, ConsoleColor.Red);
                DrawText("Усі ваші рослини загинули або ви здалися...", 32, 12, ConsoleColor.DarkRed);
            }
            DrawText("Натисніть будь-яку клавішу для виходу.", 35, 30, ConsoleColor.White);
            Console.ReadKey(true);
        }

        public void ShowHelp()
        {
            Console.Clear();
            DrawBorder(ConsoleColor.DarkGreen);
            DrawText("--- ДОСТУПНІ КОМАНДИ ---", 2, 2, ConsoleColor.White);
            DrawText("посадити [номер_слоту] - Посадити насіння у вільний слот (10 золота).", 2, 4, ConsoleColor.Green);
            DrawText("полити [номер_слоту] - Використати пакет води для рослини (10 енергії).", 2, 5, ConsoleColor.Blue);
            DrawText("удобрити [номер_слоту] - Використати пакет добрив для рослини (15 енергії).", 2, 6, ConsoleColor.DarkYellow);
            DrawText("обприскати [номер_слоту] - Видалити шкідників (1 інсектицид, 20 енергії).", 2, 7, ConsoleColor.DarkCyan);
            DrawText("лікувати [номер_слоту] - Вилікувати хворобу (1 ліки, 25 енергії).", 2, 8, ConsoleColor.DarkMagenta);
            DrawText("сироватка [номер_слоту] - Використати сироватку росту (1 сироватка, 20 енергії).", 2, 9, ConsoleColor.Green);
            DrawText("зілля_здоров'я [номер_слоту] - Використати зілля здоров'я (1 зілля, 15 енергії).", 2, 10, ConsoleColor.Red);
            DrawText("зібрати [номер_слоту] - Зібрати плоди, якщо рослина готова (30 енергії).", 2, 11, ConsoleColor.Yellow);
            DrawText("спати - Пропустити день, відновити енергію, оновити світ.", 2, 13, ConsoleColor.Gray);
            DrawText("магазин - Відкрити магазин для купівлі предметів.", 2, 14, ConsoleColor.Yellow);
            DrawText("купити [насіння/вода/добрива/інсектицид/ліки/сироватка/зілля_здоров'я] - Купити предмет.", 2, 15, ConsoleColor.Yellow);
            DrawText("робота - Відкрити меню пасивного заробітку (Інвестиції).", 2, 16, ConsoleColor.Green);
            DrawText("шлях - Відправитися у пригоду, щоб знайти предмети або зустріти істот.", 2, 17, ConsoleColor.DarkCyan); // Нова команда
            DrawText("статус - Показати детальний стан городу та гравця.", 2, 18, ConsoleColor.Gray);
            DrawText("вихід - Завершити гру.", 2, 19, ConsoleColor.Gray);
            DrawText("Натисніть будь-яку клавішу, щоб продовжити...", 2, consoleHeight - 4, ConsoleColor.Gray);
            Console.ReadKey(true);
        }

        public void DisplayStatus(int currentDay, Player player, WeatherSystem weatherSystem, JobSystem jobSystem, Plant[] plantSlots, int progressNeededForNextStage)
        {
            // Очищаємо верхню частину для статусу
            for (int i = 2; i <= 17; i++)
            {
                DrawText(new string(' ', consoleWidth - 4), 2, i, ConsoleColor.Black);
            }

            DrawText("--- СТАТУС ГРАВЦЯ ---", 2, 2, ConsoleColor.White);
            DrawText($"День: {currentDay}", 2, 3, ConsoleColor.Cyan);
            DrawText($"Енергія: {player.Energy}/100 {DrawProgressBar(player.Energy, 100, 10, ConsoleColor.Magenta, ConsoleColor.DarkGray)}", 2, 4, player.Energy < 30 ? ConsoleColor.Red : ConsoleColor.Magenta);
            // Виправлено: звернення до статичної константи XP_PER_LEVEL через ім'я класу Player
            DrawText($"XP: {player.XP}/{player.Level * Player.XP_PER_LEVEL} (Рівень: {player.Level}) {DrawProgressBar(player.XP, player.Level * Player.XP_PER_LEVEL, 10, ConsoleColor.DarkCyan, ConsoleColor.DarkGray)}", 2, 5, ConsoleColor.DarkCyan);
            DrawText($"Золото: {player.Gold} 💰", 2, 6, ConsoleColor.Yellow);
            DrawText($"Смарагд: {player.Emerald} 💎", 2, 7, ConsoleColor.Green); // Додано смарагди
            DrawText($"Погода: {weatherSystem.GetWeatherName(weatherSystem.CurrentWeather)} {weatherSystem.GetWeatherASCII(weatherSystem.CurrentWeather)}", 2, 8, weatherSystem.GetWeatherColor(weatherSystem.CurrentWeather));


            // Статус пасивної роботи
            DrawText("--- РОБОТА (Інвестиції) ---", 2, 10, ConsoleColor.White); // Зміщено на 1 рядок вниз
            if (jobSystem.IsWorking)
            {
                // Виправлено: звернення до статичної константи JOB_MAX_LEVEL через ім'я класу JobSystem
                DrawText($"Рівень: {jobSystem.JobLevel}/{JobSystem.JOB_MAX_LEVEL}", 2, 11, ConsoleColor.Green); // Зміщено на 1 рядок вниз
                DrawText($"XP: {jobSystem.JobXP}/{jobSystem.GetJobXPNeededForNextLevel()} {DrawProgressBar(jobSystem.JobXP, jobSystem.GetJobXPNeededForNextLevel(), 10, ConsoleColor.DarkGreen, ConsoleColor.DarkGray)}", 2, 12, ConsoleColor.DarkGreen); // Зміщено на 1 рядок вниз
                DrawText($"Дохід/день: {jobSystem.PassiveIncomeRate} золота", 2, 13, ConsoleColor.Yellow); // Зміщено на 1 рядок вниз
            }
            else
            {
                DrawText("Статус: Неактивно", 2, 11, ConsoleColor.Red); // Зміщено на 1 рядок вниз
                DrawText("Ви можете почати інвестиції командою 'робота почати'", 2, 12, ConsoleColor.Gray); // Зміщено на 1 рядок вниз
            }


            DrawText("--- СТАН РОСЛИН У СЛОТАХ ---", consoleWidth / 2 + 5, 2, ConsoleColor.White);
            for (int i = 0; i < plantSlots.Length; i++)
            {
                Plant p = plantSlots[i];
                string plantStatus = p.IsPlanted ?
                    $"Слот {i + 1}: Етап: {p.Stage}, Здоров'я: {p.Health}/100 ({GetHealthColor(p.Health)}), Прогрес: {p.GrowProgress}/{progressNeededForNextStage}, Якість: {p.Quality}" :
                    $"Слот {i + 1}: Порожній";
                DrawText(plantStatus, consoleWidth / 2 + 5, 3 + i * 2, ConsoleColor.Gray);
                if (p.IsPlanted && p.HasPest) DrawText("🐛", consoleWidth / 2 + 5 + plantStatus.Length, 3 + i * 2, ConsoleColor.Red);
                if (p.IsPlanted && p.HasDisease) DrawText("🦠", consoleWidth / 2 + 5 + plantStatus.Length + 2, 3 + i * 2, ConsoleColor.DarkRed);
            }
        }

        public void ShowDetailedStatus(int currentDay, Player player, WeatherSystem weatherSystem, JobSystem jobSystem, Plant[] plantSlots, int progressNeededForNextStage)
        {
            Console.Clear();
            DrawBorder(ConsoleColor.DarkGreen);
            DrawText("--- ДЕТАЛЬНИЙ СТАТУС ГРАВЦЯ ---", 2, 2, ConsoleColor.White);
            DrawText($"День: {currentDay}", 2, 4, ConsoleColor.Cyan);
            DrawText($"Енергія: {player.Energy}/100", 2, 5, ConsoleColor.Magenta);
            // Виправлено: звернення до статичної константи XP_PER_LEVEL через ім'я класу Player
            DrawText($"XP: {player.XP}/{player.Level * Player.XP_PER_LEVEL} (Рівень: {player.Level})", 2, 6, ConsoleColor.DarkCyan);
            DrawText($"Золото: {player.Gold}", 2, 7, ConsoleColor.Yellow);
            DrawText($"Смарагд: {player.Emerald}", 2, 8, ConsoleColor.Green); // Додано смарагди
            DrawText($"Погода: {weatherSystem.GetWeatherName(weatherSystem.CurrentWeather)} {weatherSystem.GetWeatherASCII(weatherSystem.CurrentWeather)}", 2, 9, weatherSystem.GetWeatherColor(weatherSystem.CurrentWeather)); // Зміщено на 1 рядок вниз

            DrawText("--- ІНВЕНТАР ---", 2, 11, ConsoleColor.White); // Зміщено на 1 рядок вниз
            DrawText($"Інсектициди: {player.PesticideCount}", 2, 12, ConsoleColor.DarkCyan); // Зміщено на 1 рядок вниз
            DrawText($"Ліки: {player.MedicineCount}", 2, 13, ConsoleColor.DarkYellow); // Зміщено на 1 рядок вниз
            DrawText($"Вода (паки): {player.WaterPackCount}", 2, 14, ConsoleColor.Blue); // Зміщено на 1 рядок вниз
            DrawText($"Добрива (паки): {player.FertilizerPackCount}", 2, 15, ConsoleColor.DarkGreen); // Зміщено на 1 рядок вниз
            DrawText($"Сироватка росту: {player.GrowthSerumCount}", 2, 16, ConsoleColor.Green); // Зміщено на 1 рядок вниз
            DrawText($"Зілля здоров'я: {player.HealthPotionCount}", 2, 17, ConsoleColor.Red); // Зміщено на 1 рядок вниз


            DrawText("--- ДЕТАЛЬНИЙ СТАТУС РОБОТИ (Інвестиції) ---", 2, 19, ConsoleColor.White); // Зміщено на 1 рядок вниз
            if (jobSystem.IsWorking)
            {
                // Виправлено: звернення до статичної константи JOB_MAX_LEVEL через ім'я класу JobSystem
                DrawText($"Рівень інвестицій: {jobSystem.JobLevel}/{JobSystem.JOB_MAX_LEVEL}", 2, 20, ConsoleColor.Green); // Зміщено на 1 рядок вниз
                DrawText($"XP інвестицій: {jobSystem.JobXP}/{jobSystem.GetJobXPNeededForNextLevel()}", 2, 21, ConsoleColor.DarkGreen); // Зміщено на 1 рядок вниз
                DrawText($"Поточний дохід/день: {jobSystem.PassiveIncomeRate} золота", 2, 22, ConsoleColor.Yellow); // Зміщено на 1 рядок вниз
            }
            else
            {
                DrawText("Статус: Неактивно", 2, 20, ConsoleColor.Red); // Зміщено на 1 рядок вниз
                DrawText("Ви можете почати інвестиції командою 'робота почати'", 2, 21, ConsoleColor.Gray); // Зміщено на 1 рядок вниз
            }


            DrawText($"--- СТАН РОСЛИН У СЛОТАХ ---", consoleWidth / 2 + 5, 2, ConsoleColor.White);
            for (int i = 0; i < plantSlots.Length; i++)
            {
                Plant p = plantSlots[i];
                DrawText($"Слот {i + 1}:", consoleWidth / 2 + 5, 4 + i * 5, ConsoleColor.White);
                if (p.IsPlanted)
                {
                    DrawText($"  Етап: {p.Stage}", consoleWidth / 2 + 8, 5 + i * 5, ConsoleColor.Yellow);
                    DrawText($"  Здоров'я: {p.Health}/100 {(p.Health < 40 ? "(Низьке!)" : "")}", consoleWidth / 2 + 8, 6 + i * 5, GetHealthColor(p.Health));
                    DrawText($"  Вода: {p.Water}/10 {(p.Water < 3 ? "(Мало!)" : "")}", consoleWidth / 2 + 8, 7 + i * 5, GetWaterColor(p.Water));
                    DrawText($"  Добрива: {p.Fertilizer}/5 {(p.Fertilizer < 1 ? "(Мало!)" : "")}", consoleWidth / 2 + 8, 8 + i * 5, GetFertilizerColor(p.Fertilizer));
                    DrawText($"  Прогрес до наступного етапу: {p.GrowProgress}/{progressNeededForNextStage}", consoleWidth / 2 + 8, 9 + i * 5, ConsoleColor.DarkGreen);
                    DrawText($"  Якість: {p.Quality} (x{p.GetQualityMultiplier():F1})", consoleWidth / 2 + 8, 10 + i * 5, ConsoleColor.Magenta);
                    DrawText($"  Шкідники: {(p.HasPest ? "Так" : "Ні")}", consoleWidth / 2 + 8, 11 + i * 5, p.HasPest ? ConsoleColor.Red : ConsoleColor.Gray);
                    DrawText($"  Хвороба: {(p.HasDisease ? "Так" : "Ні")}", consoleWidth / 2 + 8, 12 + i * 5, p.HasDisease ? ConsoleColor.DarkRed : ConsoleColor.Gray);
                }
                else
                {
                    DrawText("  Порожній", consoleWidth / 2 + 8, 5 + i * 5, ConsoleColor.Gray);
                }
            }
            DrawText("Натисніть будь-яку клавішу, щоб продовжити...", 2, consoleHeight - 4, ConsoleColor.Gray);
            Console.ReadKey(true);
        }

        // Новий метод для відображення магазину з покращеним UI
        public void ShowShopUI(Player player)
        {
            Console.Clear();
            DrawBorder(ConsoleColor.DarkCyan); // Рамка для магазину

            DrawText("╔═══════════════════════════════════════════════════════════════════════════════════════════════════════════════════╗", 0, 0, ConsoleColor.DarkCyan);
            DrawText("║                                                                                                                   ║", 0, 1, ConsoleColor.DarkCyan);
            DrawText("║                                          ✨ ЛАСКАВО ПРОСИМО ДО МАГАЗИНУ ✨                                           ║", 0, 2, ConsoleColor.Yellow);
            DrawText("║                                                                                                                   ║", 0, 3, ConsoleColor.DarkCyan);
            DrawText("╠═══════════════════════════════════════════════════════════════════════════════════════════════════════════════════╣", 0, 4, ConsoleColor.DarkCyan);

            DrawText($"Ваше золото: {player.Gold} 💰", 5, 6, ConsoleColor.Yellow);
            DrawText($"Ваші смарагди: {player.Emerald} 💎", 5, 7, ConsoleColor.Green); // Додано смарагди в магазині

            DrawText("--- ДОСТУПНІ ТОВАРИ ---", 5, 9, ConsoleColor.White); // Зміщено на 1 рядок вниз
            DrawText("  🌱 Насіння (для 1 рослини) - 10 золота", 5, 11, ConsoleColor.Green); // Зміщено на 1 рядок вниз
            DrawText("  💧 Вода (1 пак)           - 5 золота", 5, 12, ConsoleColor.Blue); // Зміщено на 1 рядок вниз
            DrawText("  🌿 Добрива (1 пак)        - 10 золота", 5, 13, ConsoleColor.DarkYellow); // Зміщено на 1 рядок вниз
            DrawText("  🧪 Інсектицид (1 шт)      - 15 золота", 5, 14, ConsoleColor.DarkCyan); // Зміщено на 1 рядок вниз
            DrawText("  🩹 Ліки (1 шт)            - 20 золота", 5, 15, ConsoleColor.DarkMagenta); // Зміщено на 1 рядок вниз
            DrawText("  ✨ Сироватка росту (1 шт) - 30 золота", 5, 16, ConsoleColor.Green); // Зміщено на 1 рядок вниз
            DrawText("  ❤️ Зілля здоров'я (1 шт)  - 25 золота", 5, 17, ConsoleColor.Red); // Зміщено на 1 рядок вниз

            DrawText("-------------------------------------------------------------------------------------------------------------------", 0, 19, ConsoleColor.DarkCyan); // Зміщено на 1 рядок вниз
            DrawText("Щоб купити, введіть 'купити [назва_товару]'. Наприклад: купити вода", 5, 21, ConsoleColor.Gray); // Зміщено на 1 рядок вниз
            DrawText("Натисніть будь-яку клавішу, щоб вийти з магазину...", 5, consoleHeight - 4, ConsoleColor.Gray);
            Console.ReadKey(true);
        }

        // Новий метод для відображення інвентарю окремо
        public void DisplayInventory(Player player)
        {
            // Очищаємо область інвентарю перед виводом нового
            for (int i = 14; i <= 20; i++) // Рядки для інвентарю
            {
                DrawText(new string(' ', 40), 2, i, ConsoleColor.Black); // Ширина блоку інвентарю
            }

            DrawText("--- ІНВЕНТАР ---", 2, 14, ConsoleColor.White);
            DrawText($"Інсектициди: {player.PesticideCount} 🧪", 2, 15, ConsoleColor.DarkCyan);
            DrawText($"Ліки: {player.MedicineCount} 🩹", 2, 16, ConsoleColor.DarkYellow);
            DrawText($"Вода (паки): {player.WaterPackCount} 💧", 2, 17, ConsoleColor.Blue);
            DrawText($"Добрива (паки): {player.FertilizerPackCount} 🌿", 2, 18, ConsoleColor.DarkGreen);
            DrawText($"Сироватка росту: {player.GrowthSerumCount} ✨", 2, 19, ConsoleColor.Green);
            DrawText($"Зілля здоров'я: {player.HealthPotionCount} ❤️", 2, 20, ConsoleColor.Red);
        }

        public void InfoUpdate()
        {
            Console.Clear();
            DrawBorder(ConsoleColor.DarkMagenta); // Красива рамка для блокнота

            DrawText("╔═══════════════════════════════════════════════════════════════════════════════════════════════════════════════════╗", 0, 0, ConsoleColor.DarkMagenta);
            DrawText("║                                                                                                                   ║", 0, 1, ConsoleColor.DarkMagenta);
            DrawText("║                                         📝 БЛОКНОТ ОНОВЛЕНЬ РОЗРОБНИКА 📝                                          ║", 0, 2, ConsoleColor.Yellow);
            DrawText("║                                                                                                                   ║", 0, 3, ConsoleColor.DarkMagenta);
            DrawText("╠═══════════════════════════════════════════════════════════════════════════════════════════════════════════════════╣", 0, 4, ConsoleColor.DarkMagenta);

            // Тут ви можете додавати свої оновлення
            DrawText("   [08.07.2025] - Додано смарагди та оновлено UI магазину.", 5, 6, ConsoleColor.White);
            DrawText("   [08.07.2025] - Перенесено логіку снів до NapManager.", 5, 7, ConsoleColor.White);
            DrawText("   [08.07.2025] - Розширено клас Creature (здоров'я, опис атаки).", 5, 8, ConsoleColor.White);
            DrawText("   [08.07.2025] - Покращено відображення інвентарю в статусі.", 5, 9, ConsoleColor.White);
            DrawText("   [08.07.2025] - Додано цей блокнот оновлень. 🎉", 5, 10, ConsoleColor.White);
            DrawText("   [09.07.2025] - Планується: система крафту та нові пригоди!", 5, 12, ConsoleColor.Green);
            DrawText("   [09.07.2025] - Дослідити можливість додавання міні-ігор.", 5, 13, ConsoleColor.Green);
            DrawText("   [10.07.2025] - Розглянути інтеграцію з файловою системою для авто-логінгу активності.", 5, 14, ConsoleColor.Green);


            DrawText("-------------------------------------------------------------------------------------------------------------------", 0, consoleHeight - 6, ConsoleColor.DarkMagenta);
            DrawText("Натисніть будь-яку клавішу, щоб продовжити...", 5, consoleHeight - 4, ConsoleColor.Gray);
            Console.ReadKey(true);
        }
    }
}
