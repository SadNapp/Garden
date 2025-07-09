using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading;

namespace ConsoleTest
{
    class GameManager
    {
        private int currentDay;
        private bool isGameOver;
        private Plant[] plantSlots;
        private const int progressNeededForNextStage = 10;
        private static readonly string SaveFilePath = "garden_save.txt";

        // Залежності
        private ConsoleRenderer _renderer;
        private Player _player;
        private JobSystem _jobSystem;
        private Shop _shop;
        private WeatherSystem _weatherSystem;
        private AdventuresSystem _adventuresSystem;
        private NapManager _napManager;

        public GameManager()
        {
            _renderer = new ConsoleRenderer();
            _player = new Player(_renderer);
            _jobSystem = new JobSystem(_player, _renderer);

            // Ініціалізуємо слоти рослин перед передачею в Shop
            plantSlots = new Plant[3];
            for (int i = 0; i < plantSlots.Length; i++)
            {
                plantSlots[i] = new Plant();
            }

            _shop = new Shop(_player, _renderer, plantSlots);
            _weatherSystem = new WeatherSystem();
            _adventuresSystem = new AdventuresSystem(_player, _renderer);
            _napManager = new NapManager(_player, _renderer);   
        }
       
        public void StartGame()
        {
            _renderer.ShowWelcomeScreen();
            LoadGameState();

            // --- Основний цикл гри ---
            while (!isGameOver)
            {
                Console.Clear();
                _renderer.DrawBorder(ConsoleColor.DarkGreen);

                
                // Відображаємо статус
                _renderer.DisplayStatus(currentDay, _player, _weatherSystem, _jobSystem, plantSlots, progressNeededForNextStage);

                _renderer.DisplayInventory(_player);
                
                // Малюємо рослини у слотах                
                _renderer.DrawPlantsInSlots(plantSlots, progressNeededForNextStage);
                
                // Перевірка умов перемоги/програшу
                CheckGameEndConditions();
                if (isGameOver) break;

                _renderer.DrawText("Введіть команду:", 2, Console.WindowHeight - 4, ConsoleColor.Gray);
                Console.SetCursorPosition(2, Console.WindowHeight - 3);
                Console.ForegroundColor = ConsoleColor.White;
                string inputRaw = Console.ReadLine();
                string input;

                if (inputRaw != null)
                {
                    input = inputRaw.ToLower();
                }
                else
                {
                    input = string.Empty;
                }
                Console.ResetColor();

                ProcessCommand(input); // Обробляємо введену команду

                Thread.Sleep(100); // Коротка пауза між ітераціями циклу
            }

            // Екран завершення гри
            _renderer.ShowGameOverScreen(_player.Gold, _player.Level, _jobSystem.JobLevel);

            Console.Clear(); // Очищаємо консоль перед виходом
            SaveGameState(); // Зберігаємо стан гри при виході
        }

        private void LoadGameState()
        {
            if (File.Exists(SaveFilePath))
            {
                try
                {
                    string[] lines = File.ReadAllLines(SaveFilePath);
                    // 16 глобальних полів + (7 полів на рослину * кількість слотів)
                    int expectedMinLines = 16 + (plantSlots.Length * 7);

                    if (lines.Length >= expectedMinLines)
                    {
                        currentDay = int.Parse(lines[0].Split(':')[1]);
                        _player.Energy = int.Parse(lines[1].Split(':')[1]);
                        _player.XP = int.Parse(lines[2].Split(':')[1]);
                        _player.Level = int.Parse(lines[3].Split(':')[1]);
                        _player.Gold = int.Parse(lines[4].Split(':')[1]);
                        _player.Emerald = int.Parse(lines[5].Split(':')[1]);
                        _player.Health = int.Parse(lines[5].Split(':')[1]); // Завантаження здоров'я гравця
                        _player.PesticideCount = int.Parse(lines[6].Split(':')[1]);
                        _player.MedicineCount = int.Parse(lines[7].Split(':')[1]);
                        _player.WaterPackCount = int.Parse(lines[8].Split(':')[1]);
                        _player.FertilizerPackCount = int.Parse(lines[9].Split(':')[1]);
                        _player.GrowthSerumCount = int.Parse(lines[10].Split(':')[1]);
                        _player.HealthPotionCount = int.Parse(lines[11].Split(':')[1]);
                        _weatherSystem.CurrentWeather = (WeatherType)Enum.Parse(typeof(WeatherType), lines[12].Split(':')[1]);
                        _jobSystem.JobLevel = int.Parse(lines[13].Split(':')[1]);
                        _jobSystem.JobXP = int.Parse(lines[14].Split(':')[1]);
                        _jobSystem.PassiveIncomeRate = int.Parse(lines[15].Split(':')[1]);
                        _jobSystem.IsWorking = bool.Parse(lines[16].Split(':')[1]);

                        // Завантаження стану рослин
                        for (int i = 0; i < plantSlots.Length; i++)
                        {
                            int offset = 16 + (i * 7); // Зміщення для кожного слота
                            if (lines.Length <= offset + 6)
                            {
                                plantSlots[i].Reset();
                                _renderer.DrawMessage($"Помилка завантаження слоту {i + 1}. Слот скинуто.", ConsoleColor.Yellow);
                                continue;
                            }

                            plantSlots[i].IsPlanted = bool.Parse(lines[offset].Split(':')[1]);
                            if (plantSlots[i].IsPlanted)
                            {
                                plantSlots[i].Stage = (PlantStage)Enum.Parse(typeof(PlantStage), lines[offset + 1].Split(':')[1]);
                                plantSlots[i].Health = int.Parse(lines[offset + 2].Split(':')[1]);
                                plantSlots[i].Water = int.Parse(lines[offset + 3].Split(':')[1]);
                                plantSlots[i].Fertilizer = int.Parse(lines[offset + 4].Split(':')[1]);
                                plantSlots[i].GrowProgress = int.Parse(lines[offset + 5].Split(':')[1]);
                                plantSlots[i].Quality = (PlantQuality)Enum.Parse(typeof(PlantQuality), lines[offset + 6].Split(':')[1]);
                            }
                            else
                            {
                                plantSlots[i].Reset();
                            }
                        }

                        _renderer.DrawMessage("Прогрес гри завантажено!", ConsoleColor.Green);
                    }
                    else
                    {
                        _renderer.DrawMessage("Файл збереження пошкоджено або неповний. Починаємо нову гру.", ConsoleColor.Red);
                        ResetGameState();
                    }
                }
                catch (Exception ex)
                {
                    _renderer.DrawMessage($"Помилка завантаження прогресу: {ex.Message}", ConsoleColor.Red);
                    _renderer.DrawMessage("Починаємо нову гру.", ConsoleColor.Yellow);
                    ResetGameState(); // Якщо помилка, починаємо нову гру
                }
            }
            else
            {
                _renderer.DrawMessage("Файл збереження не знайдено. Починаємо нову гру.", ConsoleColor.Yellow);
                ResetGameState();
            }
            Thread.Sleep(1500);
        }

        private void SaveGameState()
        {
            try
            {
                List<string> lines = new List<string>
                {
                    $"Day:{currentDay}",
                    $"Energy:{_player.Energy}",
                    $"XP:{_player.XP}",
                    $"Level:{_player.Level}",
                    $"Gold:{_player.Gold}",
                    $"Emerald:{_player.Emerald}",
                    $"Health:{_player.Health}",
                    $"Pesticides:{_player.PesticideCount}",
                    $"Medicine:{_player.MedicineCount}",
                    $"WaterPacks:{_player.WaterPackCount}",
                    $"FertilizerPacks:{_player.FertilizerPackCount}",
                    $"GrowthSerum:{_player.GrowthSerumCount}",
                    $"HealthPotion:{_player.HealthPotionCount}",
                    $"Weather:{_weatherSystem.CurrentWeather}",
                    $"JobLevel:{_jobSystem.JobLevel}",
                    $"JobXP:{_jobSystem.JobXP}",
                    $"PassiveIncomeRate:{_jobSystem.PassiveIncomeRate}",
                    $"IsWorking:{_jobSystem.IsWorking}"
                }; 

                // Зберігаємо стан кожної рослини
                foreach (var plant in plantSlots)
                {
                    lines.Add($"IsPlanted:{plant.IsPlanted}");
                    if (plant.IsPlanted)
                    {
                        lines.Add($"Stage:{plant.Stage}");
                        lines.Add($"Health:{plant.Health}");
                        lines.Add($"Water:{plant.Water}");
                        lines.Add($"Fertilizer:{plant.Fertilizer}");
                        lines.Add($"GrowProgress:{plant.GrowProgress}");
                        lines.Add($"Quality:{plant.Quality}");
                    }
                }

                File.WriteAllLines(SaveFilePath, lines);
                _renderer.DrawMessage("Прогрес гри збережено!", ConsoleColor.Green);
            }
            catch (Exception ex)
            {
                _renderer.DrawMessage($"Помилка збереження прогресу: {ex.Message}", ConsoleColor.Red);
            }
            Thread.Sleep(1000);
        }

        private void ResetGameState()
        {
            currentDay = 1;
            _player.Reset();
            _jobSystem.Reset();
            _weatherSystem.GenerateNewWeather(); // Скидаємо погоду

            isGameOver = false;

            for (int i = 0; i < plantSlots.Length; i++)
            {
                plantSlots[i].Reset(); // Скидаємо кожну рослину
            }
        }

        private void ProcessCommand(string input)
        {
            string[] commandParts = input.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (commandParts.Length == 0) return; // Пустий ввід

            string command = commandParts[0];

            // Очищаємо рядок повідомлень перед новим виводом
            _renderer.DrawText(new string(' ', Console.WindowWidth - 4), 2, Console.WindowHeight - 7, ConsoleColor.Black);

            switch (command)
            {
                case "допомога":
                    _renderer.ShowHelp();
                    break;
                case "полити":
                    if (commandParts.Length > 1 && int.TryParse(commandParts[1], out int slotIndexWater))
                    {
                        WaterPlant(slotIndexWater - 1); // -1 бо слоти 1-індексовані для користувача
                    }
                    else
                    {
                        _renderer.DrawMessage("Будь ласка, вкажіть номер слоту: полити [номер_слоту]", ConsoleColor.Red);
                    }
                    break;
                case "удобрити":
                    if (commandParts.Length > 1 && int.TryParse(commandParts[1], out int slotIndexFertilize))
                    {
                        FertilizePlant(slotIndexFertilize - 1);
                    }
                    else
                    {
                        _renderer.DrawMessage("Будь ласка, вкажіть номер слоту: удобрити [номер_слоту]", ConsoleColor.Red);
                    }
                    break;
                case "спати":
                    Sleep();
                    break;
                case "обприскати":
                    if (commandParts.Length > 1 && int.TryParse(commandParts[1], out int slotIndexSpray))
                    {
                        SprayPesticide(slotIndexSpray - 1);
                    }
                    else
                    {
                        _renderer.DrawMessage("Будь ласка, вкажіть номер слоту: обприскати [номер_слоту]", ConsoleColor.Red);
                    }
                    break;
                case "лікувати":
                    if (commandParts.Length > 1 && int.TryParse(commandParts[1], out int slotIndexCure))
                    {
                        CureDisease(slotIndexCure - 1);
                    }
                    else
                    {
                        _renderer.DrawMessage("Будь ласка, вкажіть номер слоту: лікувати [номер_слоту]", ConsoleColor.Red);
                    }
                    break;
                case "сироватка":
                    if (commandParts.Length > 1 && int.TryParse(commandParts[1], out int slotIndexSerum))
                    {
                        UseGrowthSerum(slotIndexSerum - 1);
                    }
                    else
                    {
                        _renderer.DrawMessage("Будь ласка, вкажіть номер слоту: сироватка [номер_слоту]", ConsoleColor.Red);
                    }
                    break;
                case "зілля_здоров'я":
                    if (commandParts.Length > 1 && int.TryParse(commandParts[1], out int slotIndexPotion))
                    {
                        UseHealthPotion(slotIndexPotion - 1);
                    }
                    else
                    {
                        _renderer.DrawMessage("Будь ласка, вкажіть номер слоту: зілля_здоров'я [номер_слоту]", ConsoleColor.Red);
                    }
                    break;
                case "статус":
                    _renderer.ShowDetailedStatus(currentDay, _player, _weatherSystem, _jobSystem, plantSlots, progressNeededForNextStage);
                    break;
                case "магазин":
                    _shop.ShowShop();
                    break;
                case "купити":
                    if (commandParts.Length > 1)
                    {
                        string item = commandParts[1].ToLower();
                        _shop.BuyItem(item);
                    }
                    else
                    {
                        _renderer.DrawMessage("Будь ласка, вкажіть, що купити (насіння, вода, добрива, інсектицид, ліки, сироватка, зілля_здоров'я).", ConsoleColor.Red);
                    }
                    break;
                case "зібрати":
                    if (commandParts.Length > 1 && int.TryParse(commandParts[1], out int slotIndexHarvest))
                    {
                        HarvestPlant(slotIndexHarvest - 1);
                    }
                    else
                    {
                        _renderer.DrawMessage("Будь ласка, вкажіть номер слоту: зібрати [номер_слоту]", ConsoleColor.Red);
                    }
                    break;
                case "посадити":
                    if (commandParts.Length > 1 && int.TryParse(commandParts[1], out int slotIndexPlant))
                    {
                        PlantSeed(slotIndexPlant - 1);
                    }
                    else
                    {
                        _renderer.DrawMessage("Будь ласка, вкажіть номер слоту: посадити [номер_слоту]", ConsoleColor.Red);
                    }
                    break;
                case "робота":
                    if (commandParts.Length > 1)
                    {
                        string jobCommand = commandParts[1].ToLower();
                        _jobSystem.ProcessJobCommand(jobCommand);
                    }
                    else
                    {
                        _jobSystem.ShowWorkMenu();
                    }
                    break;
                case "шлях":
                    _adventuresSystem.StartAdventure();
                    break;
                case "info":
                    _renderer.InfoUpdate();
                    break;
                case "вихід":
                    isGameOver = true;
                    _renderer.DrawMessage("Завершення гри. До побачення!", ConsoleColor.White);
                    break;
                default:
                    _renderer.DrawMessage($"Невідома команда: '{command}'. Введіть 'допомога'.", ConsoleColor.Red);
                    break;
            }
        }

        private void PlantSeed(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= plantSlots.Length)
            {
                _renderer.DrawMessage("Невірний номер слоту.", ConsoleColor.Red);
                return;
            }
            if (plantSlots[slotIndex].IsPlanted)
            {
                _renderer.DrawMessage($"Слот {slotIndex + 1} вже зайнятий.", ConsoleColor.Red);
                return;
            }
            if (!_player.TrySpendGold(10)) // Вартість насіння
            {
                return;
            }

            plantSlots[slotIndex].Reset(); // Скидаємо, щоб отримати нову якість та стан
            plantSlots[slotIndex].IsPlanted = true;
            _renderer.DrawMessage($"Ви посадили насіння у слот {slotIndex + 1}!", ConsoleColor.Green);
            _player.GainXP(5); // XP за посадку
        }

        private void WaterPlant(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= plantSlots.Length || !plantSlots[slotIndex].IsPlanted)
            {
                _renderer.DrawMessage("Невірний слот або він порожній.", ConsoleColor.Red);
                return;
            }
            if (!_player.TrySpendEnergy(10)) { return; }
            if (!_player.HasWaterPack()) { _renderer.DrawMessage("У вас немає води в інвентарі! Купіть її в магазині.", ConsoleColor.Red); _player.RestoreEnergy(10); return; }

            if (plantSlots[slotIndex].Water < 10)
            {
                plantSlots[slotIndex].Water++;
                _player.UseWaterPack();
                _renderer.DrawMessage($"Ви полили рослину у слоті {slotIndex + 1}. Рівень води збільшився.", ConsoleColor.Green);
                _player.GainXP(5); // XP за полив
            }
            else
            {
                _renderer.DrawMessage("Рівень води вже максимальний!", ConsoleColor.Red);
                _player.RestoreEnergy(10); // Повертаємо енергію
            }
        }

        private void FertilizePlant(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= plantSlots.Length || !plantSlots[slotIndex].IsPlanted)
            {
                _renderer.DrawMessage("Невірний слот або він порожній.", ConsoleColor.Red);
                return;
            }
            if (!_player.TrySpendEnergy(15)) { return; }
            if (!_player.HasFertilizerPack()) { _renderer.DrawMessage("У вас немає добрив в інвентарі! Купіть їх в магазині.", ConsoleColor.Red); _player.RestoreEnergy(15); return; }

            if (plantSlots[slotIndex].Fertilizer < 5)
            {
                plantSlots[slotIndex].Fertilizer++;
                _player.UseFertilizerPack();
                _renderer.DrawMessage($"Ви удобрили рослину у слоті {slotIndex + 1}. Рівень добрив збільшився.", ConsoleColor.Green);
                _player.GainXP(8); // XP за удобрення
            }
            else
            {
                _renderer.DrawMessage("Рівень добрив вже максимальний!", ConsoleColor.Red);
                _player.RestoreEnergy(15); // Повертаємо енергію
            }
        }

        private void UseGrowthSerum(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= plantSlots.Length || !plantSlots[slotIndex].IsPlanted)
            {
                _renderer.DrawMessage("Невірний слот або він порожній.", ConsoleColor.Red);
                return;
            }
            if (!_player.TrySpendEnergy(20)) { return; }
            if (!_player.HasGrowthSerum()) { _renderer.DrawMessage("У вас немає сироватки росту!", ConsoleColor.Red); _player.RestoreEnergy(20); return; }

            Plant p = plantSlots[slotIndex];
            if ((int)p.Stage < Enum.GetValues(typeof(PlantStage)).Length - 1)
            {
                p.Stage++; // Переходимо на наступний етап
                p.GrowProgress = 0; // Скидаємо прогрес
                _player.UseGrowthSerum();
                _renderer.DrawMessage($"Ви використали сироватку росту на рослині у слоті {slotIndex + 1}. Вона перейшла на етап: {p.Stage}!", ConsoleColor.Green);
                _player.GainXP(25); // XP за використання сироватки
            }
            else
            {
                _renderer.DrawMessage("Рослина вже досягла останнього етапу росту!", ConsoleColor.Yellow);
                _player.RestoreEnergy(20); // Повертаємо енергію
            }
        }

        private void UseHealthPotion(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= plantSlots.Length || !plantSlots[slotIndex].IsPlanted)
            {
                _renderer.DrawMessage("Невірний слот або він порожній.", ConsoleColor.Red);
                return;
            }
            if (!_player.TrySpendEnergy(15)) { return; }
            if (!_player.HasHealthPotion()) { _renderer.DrawMessage("У вас немає зілля здоров'я!", ConsoleColor.Red); _player.RestoreEnergy(15); return; }

            Plant p = plantSlots[slotIndex];
            if (p.Health < 100)
            {
                p.Health = Math.Min(100, p.Health + 30); // Відновлюємо 30 здоров'я
                _player.UseHealthPotion();
                _renderer.DrawMessage($"Ви використали зілля здоров'я на рослині у слоті {slotIndex + 1}. Здоров'я відновлено!", ConsoleColor.Green);
                _player.GainXP(10); // XP за використання зілля
            }
            else
            {
                _renderer.DrawMessage("Здоров'я рослини вже максимальне!", ConsoleColor.Yellow);
                _player.RestoreEnergy(15); // Повертаємо енергію
            }
        }

        
        private void Sleep()
        {           
            currentDay++;
            _player.RestoreEnergy(50 + (_player.Level * 2)); // Відновлюємо енергію, бонус від рівня
            _renderer.DrawMessage("Ви поспали. День пройшов, енергія відновлена.", ConsoleColor.Blue);

            //Викликаємо NapManeger для обробки!
            _napManager.ProcessSleep();

            // Застосовуємо пасивний дохід
            if (_jobSystem.IsWorking)
            {
                _player.AddGold(_jobSystem.PassiveIncomeRate);
                _renderer.DrawMessage($"Пасивний дохід: +{_jobSystem.PassiveIncomeRate} золота! Всього золота: {_player.Gold}", ConsoleColor.Yellow);
                _jobSystem.GainJobXP(5); // XP за пасивний дохід
                Thread.Sleep(1000);
            }

            UpdateGameWorld(); // Оновлюємо світ після сну
        }

        private void SprayPesticide(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= plantSlots.Length || !plantSlots[slotIndex].IsPlanted)
            {
                _renderer.DrawMessage("Невірний слот або він порожній.", ConsoleColor.Red);
                return;
            }
            if (!_player.TrySpendEnergy(20)) { return; }
            if (!_player.HasPesticide()) { _renderer.DrawMessage("У вас немає інсектицидів!", ConsoleColor.Red); _player.RestoreEnergy(20); return; }

            Plant p = plantSlots[slotIndex];
            if (p.HasPest)
            {
                p.HasPest = false; // Видаляємо шкідників з цієї рослини
                _player.UsePesticide();
                _renderer.DrawMessage($"Ви обприскали рослину у слоті {slotIndex + 1}. Шкідники зникли!", ConsoleColor.Green);
                _player.GainXP(15); // XP за боротьбу зі шкідниками
            }
            else
            {
                _renderer.DrawMessage("Шкідників немає, обприскування не потрібне.", ConsoleColor.Yellow);
                _player.RestoreEnergy(20); // Повертаємо енергію
            }
        }

        private void CureDisease(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= plantSlots.Length || !plantSlots[slotIndex].IsPlanted)
            {
                _renderer.DrawMessage("Невірний слот або він порожній.", ConsoleColor.Red);
                return;
            }
            if (!_player.TrySpendEnergy(25)) { return; }
            if (!_player.HasMedicine()) { _renderer.DrawMessage("У вас немає ліків!", ConsoleColor.Red); _player.RestoreEnergy(25); return; }

            Plant p = plantSlots[slotIndex];
            if (p.HasDisease)
            {
                p.HasDisease = false; // Видаляємо хворобу з цієї рослини
                _player.UseMedicine();
                _renderer.DrawMessage($"Ви вилікували рослину у слоті {slotIndex + 1}. Хвороба зникла!", ConsoleColor.Green);
                _player.GainXP(20); // XP за лікування
            }
            else
            {
                _renderer.DrawMessage("Рослина не хвора, лікування не потрібне.", ConsoleColor.Yellow);
                _player.RestoreEnergy(25); // Повертаємо енергію
            }
        }

        private void HarvestPlant(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= plantSlots.Length || !plantSlots[slotIndex].IsPlanted)
            {
                _renderer.DrawMessage("Невірний слот або він порожній.", ConsoleColor.Red);
                return;
            }
            if (!_player.TrySpendEnergy(30)) { return; }

            Plant p = plantSlots[slotIndex];
            if (p.Stage == PlantStage.Fruiting && p.GrowProgress >= progressNeededForNextStage)
            {
                int baseGold = Program.random.Next(30, 60);
                int earnedGold = (int)(baseGold * p.GetQualityMultiplier() * (1 + _player.Level * 0.05)); // Бонус від якості та рівня
                _player.AddGold(earnedGold);
                _renderer.DrawMessage($"Ви успішно зібрали врожай зі слоту {slotIndex + 1}! Зароблено {earnedGold} золота.", ConsoleColor.Yellow);
                _player.GainXP(30 + (int)(earnedGold / 5)); // XP за збір врожаю (залежить від золота)

                p.Reset(); // Скидаємо рослину до початкового стану для нового циклу
                _renderer.DrawMessage($"Слот {slotIndex + 1} готовий для нового насіння!", ConsoleColor.Green);
                Thread.Sleep(1500);
            }
            else
            {
                _renderer.DrawMessage("Рослина ще не готова до збору врожаю або не досягла етапу плодоношення!", ConsoleColor.Red);
                _player.RestoreEnergy(30); // Повертаємо енергію
            }
        }

        private void UpdateGameWorld()
        {
            _weatherSystem.GenerateNewWeather();
            _renderer.DrawMessage($"Погода змінилася на: {_weatherSystem.GetWeatherName(_weatherSystem.CurrentWeather)}!", _weatherSystem.GetWeatherColor(_weatherSystem.CurrentWeather));
            Thread.Sleep(1000);

            foreach (var p in plantSlots)
            {
                if (!p.IsPlanted) continue; // Пропускаємо порожні слоти

                // Зменшення ресурсів та здоров'я з часом
                p.Water = Math.Max(0, p.Water - 1); // Рослина споживає воду щодня
                p.Fertilizer = Math.Max(0, p.Fertilizer - 1); // Добрива виснажуються

                // Вплив погоди
                switch (_weatherSystem.CurrentWeather)
                {
                    case WeatherType.Rainy:
                        p.Water = Math.Min(10, p.Water + 2); // Дощ додає води
                        _renderer.DrawMessage($"Дощ полив рослину у слоті {Array.IndexOf(plantSlots, p) + 1}!", ConsoleColor.Blue);
                        Thread.Sleep(500);
                        break;
                    case WeatherType.Drought:
                        p.Water = Math.Max(0, p.Water - 2); // Засуха висушує
                        p.Health = Math.Max(0, p.Health - 10); // Засуха шкодить здоров'ю
                        _renderer.DrawMessage($"Засуха шкодить рослині у слоті {Array.IndexOf(plantSlots, p) + 1}!", ConsoleColor.DarkRed);
                        Thread.Sleep(500);
                        break;
                    case WeatherType.Foggy: // Ефект туману
                        p.GrowProgress = Math.Max(0, p.GrowProgress - 1); // Туман сповільнює ріст
                        if (Program.random.Next(0, 100) < 10 && !p.HasDisease) // 10% шанс на хворобу в тумані
                        {
                            p.HasDisease = true;
                            _renderer.DrawMessage($"Через туман рослина у слоті {Array.IndexOf(plantSlots, p) + 1} захворіла!", ConsoleColor.DarkRed);
                            Thread.Sleep(500);
                        }
                        _renderer.DrawMessage($"Туман сповільнює ріст рослини у слоті {Array.IndexOf(plantSlots, p) + 1}...", ConsoleColor.DarkGray);
                        Thread.Sleep(500);
                        break;
                }

                // Вплив відсутності ресурсів на здоров'я рослини
                if (p.Water == 0)
                {
                    p.Health = Math.Max(0, p.Health - 10);
                    _renderer.DrawMessage($"Рослина у слоті {Array.IndexOf(plantSlots, p) + 1} страждає від спраги!", ConsoleColor.Red);
                    Thread.Sleep(500);
                }
                if (p.Fertilizer == 0)
                {
                    p.Health = Math.Max(0, p.Health - 5);
                    _renderer.DrawMessage($"Рослині у слоті {Array.IndexOf(plantSlots, p) + 1} бракує добрив!", ConsoleColor.Red);
                    Thread.Sleep(500);
                }

                // Шкідники завдають шкоди
                if (p.HasPest)
                {
                    p.Health = Math.Max(0, p.Health - 15);
                    _renderer.DrawMessage($"Шкідники продовжують завдавати шкоди рослині у слоті {Array.IndexOf(plantSlots, p) + 1}!", ConsoleColor.Red);
                    Thread.Sleep(500);
                }

                // Хвороба завдає шкоди
                if (p.HasDisease)
                {
                    p.Health = Math.Max(0, p.Health - 20);
                    _renderer.DrawMessage($"Хвороба погіршує стан рослини у слоті {Array.IndexOf(plantSlots, p) + 1}!", ConsoleColor.DarkRed);
                    Thread.Sleep(500);
                }

                // Випадкова поява шкідників
                if (!p.HasPest && Program.random.Next(0, 100) < 20) // 20% шанс на появу шкідників
                {
                    p.HasPest = true;
                    _renderer.DrawMessage($"У слоті {Array.IndexOf(plantSlots, p) + 1} з'явилися шкідники!", ConsoleColor.DarkRed);
                    Thread.Sleep(500);
                }

                // Випадкова поява хвороби (менший шанс, якщо здоров'я високе)
                if (!p.HasDisease && Program.random.Next(0, 100) < (p.Health < 50 ? 15 : 5)) // 5% шанс, якщо здорова, 15% якщо хвора
                {
                    p.HasDisease = true;
                    _renderer.DrawMessage($"Рослина у слоті {Array.IndexOf(plantSlots, p) + 1} захворіла!", ConsoleColor.DarkRed);
                    Thread.Sleep(500);
                }

                // Прогрес росту від ресурсів
                if (p.Water > 0 && p.Fertilizer > 0 && !p.HasPest && !p.HasDisease)
                {
                    p.GrowProgress += 1; // Додатковий прогрес за наявності обох ресурсів та відсутності проблем
                }

                // Перевірка переходу на наступний етап
                if (p.GrowProgress >= progressNeededForNextStage)
                {
                    if ((int)p.Stage < Enum.GetValues(typeof(PlantStage)).Length - 1)
                    {
                        p.Stage++; // Переходимо на наступний етап
                        p.GrowProgress = 0; // Скидаємо прогрес
                        _renderer.DrawMessage($"Рослина у слоті {Array.IndexOf(plantSlots, p) + 1} перейшла на етап: {p.Stage}!", ConsoleColor.Yellow);
                        Thread.Sleep(2000); // Пауза для відображення повідомлення
                    }
                }
            }
        }

        private void CheckGameEndConditions()
        {
            // Гра завершується (програш), якщо всі рослини загинули АБО слоти порожні,
            // І гравець не може посадити нове насіння (немає золота АБО немає вільних слотів)
            // І немає активного пасивного доходу
            bool allPlantsDeadOrEmpty = plantSlots.All(p => !p.IsPlanted || p.Health <= 0);
            bool canAffordNewSeed = _player.Gold >= 10;
            bool hasEmptySlot = plantSlots.Any(p => !p.IsPlanted);

            if (allPlantsDeadOrEmpty && !canAffordNewSeed && !hasEmptySlot && !_jobSystem.IsWorking)
            {
                isGameOver = true;
            }
        }
    }
}