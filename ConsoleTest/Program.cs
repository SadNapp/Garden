using ConsoleTest;
using System;

namespace ConsoleTest
{
    class Program
    {
        public static Random random = new Random(); // Єдиний екземпляр Random для всієї гри

        static void Main(string[] args)
        {
           
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            // 1. Початкові налаштування консолі
            Console.Title = "Мій Консольний Город v1.4.0";
            Console.CursorVisible = false;
            Console.SetWindowSize(120, 40); // Збільшено розмір вікна для кращого UI
            Console.SetBufferSize(120, 40); // Розмір буфера також

            // Ініціалізуємо основний менеджер гри
            GameManager gameManager = new GameManager();

            // Запускаємо гру
            gameManager.StartGame();
        }
    }
}