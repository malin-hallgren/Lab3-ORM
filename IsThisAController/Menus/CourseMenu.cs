using SchoolDb2App.Data;
using SchoolDb2App.Models;
using System;
using System.Collections.Generic;
using System.Text;
using SchoolDb2App.IsThisAController.Menus;
using Microsoft.EntityFrameworkCore;

namespace SchoolDb2App.IsThisAController.Menus
{
    internal class CourseMenu
    {
        private static List<string> options = new List<string>
        {
            "Show Course Information",
            "Add Course",
            "Back to Main Menu"
        };

        public static void Run(SchoolDb2Context context)
        {
            while (true)
            {
                int selection = MenuDriver.Choice(options, "Course Menu");
                switch (selection)
                {
                    case 0:
                        var course = MainMenu.SelectCourse(context, context.Courses.ToList());
                        PrintCourseInformation(context, course);
                        MenuDriver.ReturnToMenu();
                        break;
                    case 1:
                        AddCourse(context);
                        MenuDriver.ReturnToMenu();
                        break;
                    case 2:
                        return;
                }
            }
        }

        // prints course information, teacher and grade statistics
        private static void PrintCourseInformation(SchoolDb2Context context, Course course)
        {
            var courseWithEmployee = context.Courses
                .Include(c => c.Employee)
                .FirstOrDefault(c => c.CourseId == course.CourseId);

            int amountOfStudents = context.Grades
                .Where(g => g.CourseId == course.CourseId)
                .Select(g => g.StudentId)
                .Distinct()
                .Count();

            List<int> grades = context.Grades
                .Where(g => g.CourseId == course.CourseId)
                .Include(g => g.GradeScale)
                .Select(g => g.GradeScale.GradeNumeric)
                .ToList();

            float averageGrade = (float)grades.Sum()/amountOfStudents;
            float highestGrade = grades.Count > 0 ? grades.Max() : 0;
            float lowestGrade = grades.Count > 0 ? grades.Min() : 0;

            float[] gradeArray = { averageGrade, highestGrade, lowestGrade };
            char[] letterGrades = CalculateGrades(gradeArray);


            Console.WriteLine($"{courseWithEmployee.CourseName, - 10}" + $"{"- "}"
                + $"{courseWithEmployee.Employee.EmployeeFirstName} {courseWithEmployee.Employee.EmployeeLastName}\n\n" 
                + $"{"Amount of Students:", - 15}" + $"{amountOfStudents}\n"
                + $"{"Average Grade:", -15}" + $"{gradeArray[0]:F2} ({letterGrades[0]:F2})\n"
                + $"{"Highest Grade:", -15}" + $"{gradeArray[1]:F2} ({letterGrades[1]:F2})\n"
                + $"{"Lowest Grade:", -15}" + $"{gradeArray[2]:F2} ({letterGrades[2]:F2})");
        }

        // calculates letter grade based on numeric grade
        private static char[] CalculateGrades(float[] grades)
        {
            char[] letterGrades = new char[grades.Length];
            for (int i = 0; i < grades.Length; i++)
            {
                switch (grades[i])
                {
                    case < 10:
                        letterGrades[i] = 'F';
                        break;
                    case < 12:
                        letterGrades[i] = 'E';
                        break;
                    case < 15:
                        letterGrades[i] = 'D';
                        break;
                    case < 17:
                        letterGrades[i] = 'C';
                        break;
                    case < 20:
                        letterGrades[i] = 'B';
                        break;
                    case >= 20:
                        letterGrades[i] = 'A';
                        break;
                    default:
                        Console.WriteLine("Unexpected grade values. Are any grades set?");
                        break;
                }
            }

            return letterGrades;
        }

        private static void AddCourse(SchoolDb2Context context)
        {
            var teachers = context.Employees
                .Include(e => e.EmployeeRoles)
                .ThenInclude(er => er.Role)
                .Where(e => e.EmployeeRoles.Any(er => er.Role.RoleName == "Teacher"))
                .ToList();

            var newCourseName = InputHelpers.ValidString("Enter Course Name:");
            var selectedTeacher = MainMenu.SelectEmployee(context, teachers);

            

            using var transaction = context.Database.BeginTransaction();

            try
            {
                var newCourse = new Course
                {
                    CourseName = newCourseName,
                    EmployeeId = selectedTeacher.EmployeeId
                };

                context.Courses.Add(newCourse);
                context.SaveChanges();
                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new InvalidOperationException("Error creating course.", ex);
            }
        }
    }
}
