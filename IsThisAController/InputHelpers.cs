using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolDb2App.IsThisAController
{
    internal class InputHelpers
    {
        public static string ValidString(string prompt)
        {

            Console.Clear();
            string? input = String.Empty;

            while (true)
            {
                Console.WriteLine(prompt);
                input = Console.ReadLine();

                if(String.IsNullOrWhiteSpace(input))
                {
                    Console.WriteLine("Input may not be empty.\nPress ENTER to try again...");
                    Console.ReadLine();
                    Console.Clear();
                    continue;
                }

                else
                {
                    return input.Trim();
                }
            }
        }

        public static int ValidInt(string prompt)
        {
            Console.Clear();
            string? input = String.Empty;
            int output = -1;

            while (true)
            {
                Console.WriteLine(prompt);

                if(!int.TryParse(Console.ReadLine(), out output))
                {
                    Console.WriteLine("Please input a valid number with no decimal points.\nPress ENTER to try again...");
                    Console.ReadLine();
                    continue;
                }
                else
                {
                    return output;
                }
            }
        }
    }
}
