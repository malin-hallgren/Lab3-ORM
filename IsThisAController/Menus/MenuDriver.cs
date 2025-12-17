using System;
using System.Collections.Generic;
using System.Text;

namespace EFCodealong2.IsThisAController.Menus
{
    internal class MenuDriver
    {
        public static int Choice<T>(List<T> options, string title = "Title")
        {
            Console.CursorVisible = false;
            int selected = 0;
            int prevSelected = selected;
            Console.Clear();

            Draw(options, selected, prevSelected, title);

            while(true)
            {
                var pressed = Console.ReadKey(true).Key;

                switch (pressed)
                {
                    case ConsoleKey.UpArrow:
                    case ConsoleKey.W:
                        prevSelected = selected;
                        selected = (selected - 1 + options.Count) % options.Count;
                        ClearSelectLines(selected, prevSelected, 2);
                        Draw(options, selected, prevSelected, title);
                        break;
                    case ConsoleKey.DownArrow:
                    case ConsoleKey.S:
                        prevSelected = selected;
                        selected = (selected + 1) % options.Count;
                        ClearSelectLines(selected, prevSelected, 2);
                        Draw(options, selected, prevSelected, title);
                        break;
                    case ConsoleKey.Enter:
                        Console.CursorVisible = true;
                        Console.Clear();
                        return selected;
                }
            }

        }

        public static void Draw<T>(List<T> options, int selected, int prevSelcted, string title = "Title")
        {
            Console.WriteLine($"{title}\n");

            for(int i = 0; i < options.Count; i++)
            {
                if ( i == selected)
                {
                    Console.BackgroundColor = ConsoleColor.Gray;
                    Console.ForegroundColor = ConsoleColor.Black;

                    Console.WriteLine(options[i]);

                    Console.ResetColor();
                }
                else
                {
                    Console.WriteLine(options[i]);
                }
            }
        }

        public static void ClearSelectLines(int selected, int prevSelected, int topBuffer)
        {
            Console.SetCursorPosition(0, selected + topBuffer);
            Console.Write(new string(' ', Console.BufferWidth));
            Console.SetCursorPosition(0, prevSelected + topBuffer);
            Console.Write(new string(' ', Console.BufferWidth));
            Console.SetCursorPosition(0, 0);
        }

        public static void ReturnToMenu()
        {
            Console.WriteLine("\nPress any key to return to the menu...");
            Console.ReadKey(true);
        }
    }
}
