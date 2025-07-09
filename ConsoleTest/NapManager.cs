using System;
using System.Collections.Generic;
using System.Threading;

namespace ConsoleTest
{
     class NapManager
    {
        private Player _player;
        private ConsoleRenderer _renderer;

        public List<string> Nightmare = new List<string>
        {
             // Сон 3: Заплутаний ліс Мавки
        "|Заплутаний ліс Мавки|" + Environment.NewLine +
        "Я заблукав у густому лісі, де дерева переплетені своїми гілками, " +
        "утворюючи непрохідні хащі. Звідусіль лунає заворожливий жіночий сміх. " +
        "З-за дерев випливають бліді постаті з довгим розпущеним волоссям, їхні очі світяться в темряві, " +
        "і вони кличуть мене вглиб лісу, обіцяючи невідомі насолоди, але в їхніх голосах чується смертельна загроза.",

        // Сон 4: Зачароване коло Відьми
        "|Зачароване коло Відьми|" + Environment.NewLine +
        "Я опинився в лісовій галявині, де горить багаття, " +
        "а навколо нього танцюють страшні відьми. Їхні обличчя спотворені злістю, " +
        "а їхні голоси хрипкі та мерзенні. " +
        "Вони кличуть мене в своє коло, пропонуючи взяти участь у їхньому жахливому ритуалі, " +
        "і я відчуваю, що не можу втекти від їхньої магічної влади."
        };
        public List<string> Dreams = new List<string>
        {
        // Сон 1: Гроза над степом
        "|Гроза над степом|" + Environment.NewLine + // Заголовок сну, після нього перехід на новий рядок
        "Навколо мене розкинувся безкрайній український степ. " +
        "Небо темніє, і над ним збираються грозові хмари. " +
        "Перші краплі дощу падають на розігріту землю, і повітря наповнюється свіжим ароматом озону. " +
        "Блискавки розтинають небо, освітлюючи на мить безкраї простори, " +
        "а потім знову занурюючи їх у темряву. Це величне видовище, що нагадує про силу природи.",

        // Сон 2: Світанок у Карпатах
        "|Світанок у Карпатах|" + Environment.NewLine +
        "Я стою на вершині гори, а навколо мене розкинулися Карпати. " +
        "Перші промені сонця пробиваються крізь густі смерекові ліси, " +
        "розфарбовуючи небо в ніжні відтінки рожевого та золотого. " +
        "Повітря наповнене ароматом хвої та свіжості, а десь вдалині чути дзвінкий спів птахів. " +
        "Здається, що навіть час зупинився, спостерігаючи за цією дивовижною красою.",
                
        };

        private Random random = new Random();

        public NapManager(Player player, ConsoleRenderer renderer)
        {
            _player = player;
            _renderer = renderer;
        }

        public void ProcessSleep()
        {
            int dreamRoll = Program.random.Next(0, 100);
            if(dreamRoll < 70)
            {
                int randomIndex = Program.random.Next(Dreams.Count);
                string currentDream = Dreams[randomIndex];
                _renderer.DrawMessage($"Ваш сон: {currentDream}", ConsoleColor.Green);
                _player.Heal(5);
                _player.GainXP(5);
            }
            else
            {
                int ranadomIndexNithmare = Program.random.Next(Nightmare.Count);
                string currentNightmare = Nightmare[ranadomIndexNithmare];
                _renderer.DrawMessage($"Ваш сон: {currentNightmare}", ConsoleColor.DarkRed);
                _player.TakeDamage(5);
                _player.RestoreEnergy(-15);
            }
            Thread.Sleep(1500);
        }
    }
}
