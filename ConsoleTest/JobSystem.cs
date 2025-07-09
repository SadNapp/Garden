using ConsoleTest;
using System;

namespace ConsoleTest
{
    class JobSystem
    {
        public int JobLevel { get; set; }
        public int JobXP { get; set; }
        public int PassiveIncomeRate { get; set; }
        public bool IsWorking { get; set; }

        public const int JOB_MAX_LEVEL = 10;
        public const int JOB_XP_PER_LEVEL = 50;

        private Player _player;
        private ConsoleRenderer _renderer;

        public JobSystem(Player player, ConsoleRenderer renderer)
        {
            _player = player;
            _renderer = renderer;
            Reset();
        }

        public void Reset()
        {
            JobLevel = 0;
            JobXP = 0;
            PassiveIncomeRate = 0;
            IsWorking = false;
        }

        public int GetJobXPNeededForNextLevel()
        {
            if (JobLevel == 0) return 50; // XP для першого рівня
            return JobLevel * JOB_XP_PER_LEVEL;
        }

        public void ProcessJobCommand(string jobCommand)
        {
            switch (jobCommand)
            {
                case "почати":
                    StartPassiveJob();
                    break;
                case "покращити":
                    UpgradePassiveJob();
                    break;
                case "статус":
                    ShowWorkMenu();
                    break;
                case "зупинити":
                    StopPassiveJob();
                    break;
                default:
                    _renderer.DrawMessage($"Невідома команда для роботи: '{jobCommand}'. Спробуйте 'почати', 'покращити', 'статус', 'зупинити'.", ConsoleColor.Red);
                    break;
            }
        }

        public void ShowWorkMenu()
        {
            Console.Clear();
            _renderer.DrawBorder(ConsoleColor.DarkGreen);
            _renderer.DrawText("--- МЕНЮ РОБОТИ (Інвестиції в садівництво) ---", 2, 2, ConsoleColor.White);
            _renderer.DrawText($"Ваше золото: {_player.Gold}", 2, 4, ConsoleColor.Yellow);
            _renderer.DrawText($"Рівень інвестицій: {JobLevel}/{JOB_MAX_LEVEL}", 2, 5, ConsoleColor.Green);
            _renderer.DrawText($"XP інвестицій: {JobXP}/{GetJobXPNeededForNextLevel()} {_renderer.DrawProgressBar(JobXP, GetJobXPNeededForNextLevel(), 15, ConsoleColor.DarkGreen, ConsoleColor.DarkGray)}", 2, 6, ConsoleColor.DarkGreen);
            _renderer.DrawText($"Поточний дохід/день: {PassiveIncomeRate} золота", 2, 7, ConsoleColor.Yellow);
            _renderer.DrawText($"Статус: {(IsWorking ? "Активно" : "Неактивно")}", 2, 8, IsWorking ? ConsoleColor.Green : ConsoleColor.Red);

            _renderer.DrawText("", 2, 10, ConsoleColor.Gray);
            _renderer.DrawText("Доступні дії:", 2, 11, ConsoleColor.White);
            _renderer.DrawText("  робота почати - Почати інвестувати (вартість: 50 золота, 10 енергії).", 2, 12, ConsoleColor.Cyan);
            _renderer.DrawText($"  робота покращити - Підвищити рівень інвестицій (вартість: {GetJobUpgradeCost()} золота, 20 енергії, потрібно {GetJobXPNeededForNextLevel()} XP).", 2, 13, ConsoleColor.Magenta);
            _renderer.DrawText("  робота зупинити - Зупинити активні інвестиції.", 2, 14, ConsoleColor.Red);
            _renderer.DrawText("  робота статус - Показати це меню.", 2, 15, ConsoleColor.Gray);

            _renderer.DrawText("Натисніть будь-яку клавішу, щоб продовжити...", 2, Console.WindowHeight - 4, ConsoleColor.Gray);
            Console.ReadKey(true);
        }

        public int GetJobUpgradeCost()
        {
            return (JobLevel + 1) * 20; // Вартість зростає з рівнем
        }

        public void StartPassiveJob()
        {
            if (IsWorking)
            {
                _renderer.DrawMessage("Інвестиції вже активні. Ви можете їх покращити або зупинити.", ConsoleColor.Yellow);
                return;
            }
            if (!_player.TrySpendEnergy(10)) { return; }
            if (!_player.TrySpendGold(50)) { _player.RestoreEnergy(10); return; } // Повертаємо енергію, якщо не вистачило золота

            JobLevel = 1;
            PassiveIncomeRate = 5; // Базовий дохід
            IsWorking = true;
            _renderer.DrawMessage("Ви почали інвестувати в садівництво! Тепер ви будете отримувати пасивний дохід.", ConsoleColor.Green);
            GainJobXP(10); // XP за початок роботи
        }

        public void UpgradePassiveJob()
        {
            if (!IsWorking)
            {
                _renderer.DrawMessage("Спочатку почніть інвестувати, щоб їх покращити.", ConsoleColor.Red);
                return;
            }
            if (JobLevel >= JOB_MAX_LEVEL)
            {
                _renderer.DrawMessage("Ваші інвестиції досягли максимального рівня!", ConsoleColor.Yellow);
                return;
            }
            if (!_player.TrySpendEnergy(20)) { return; }

            int upgradeCost = GetJobUpgradeCost();
            if (!_player.TrySpendGold(upgradeCost)) { _player.RestoreEnergy(20); return; } // Повертаємо енергію, якщо не вистачило золота

            if (JobXP < GetJobXPNeededForNextLevel())
            {
                _renderer.DrawMessage($"Недостатньо XP для покращення (потрібно {GetJobXPNeededForNextLevel()} XP).", ConsoleColor.Red);
                _player.RestoreEnergy(20); // Повертаємо енергію
                _player.AddGold(upgradeCost); // Повертаємо золото
                return;
            }

            JobXP -= GetJobXPNeededForNextLevel(); // Витрачаємо XP
            JobLevel++;
            PassiveIncomeRate += 5; // Збільшуємо дохід
            _renderer.DrawMessage($"Інвестиції покращено до рівня {JobLevel}! Щоденний дохід збільшився.", ConsoleColor.Green);
            GainJobXP(10); // XP за покращення
        }

        public void StopPassiveJob()
        {
            if (!IsWorking)
            {
                _renderer.DrawMessage("Пасивні інвестиції неактивні.", ConsoleColor.Yellow);
                return;
            }
            IsWorking = false;
            PassiveIncomeRate = 0;
            _renderer.DrawMessage("Ви зупинили пасивні інвестиції.", ConsoleColor.Red);
        }

        public void GainJobXP(int amount)
        {
            JobXP += amount;
            _renderer.DrawMessage($"Ви отримали {amount} XP для роботи!", ConsoleColor.DarkGreen);
        }
    }
}