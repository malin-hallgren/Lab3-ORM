using SchoolDb2App.Data;
using System;
using System.Collections.Generic;
using System.Text;
using SchoolDb2App.IsThisAController.Menus;
using EFCodealong2.IsThisAController.Menus;

namespace SchoolDb2App.IsThisAController.Menus
{
    internal class StatisticsMenu
    {
        private static List<string> options = new List<string>
        {
            "Show Average Grades by Year",
            "Show Average Grades by Course",
            "Show Top Performing Students",
            "Back to Grades Menu"
        };

        public static void Run(SchoolDb2Context context)
        {
            while (true)
            {
                int selection = MenuDriver.Choice(options, "Statistics Menu");
                switch (selection)
                {
                    case 0:
                        Console.WriteLine("Feature not implemented yet.");
                        MenuDriver.BackToSameMenu();
                        break;
                    case 1:
                        // Show Average Grades by Course
                        Console.WriteLine("Feature not implemented yet.");
                        MenuDriver.BackToSameMenu();
                        break;
                    case 2:
                        // Show Top Performing Students
                        Console.WriteLine("Feature not implemented yet.");
                        MenuDriver.BackToSameMenu();
                        break;
                    case 3:
                        // Back to Grades Menu
                        return;
                }
            }
        }
    }
}
