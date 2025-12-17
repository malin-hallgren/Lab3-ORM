using EFCodealong2.IsThisAController.Menus;
using Microsoft.EntityFrameworkCore;
using SchoolDb2App.Data;
using SchoolDb2App.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolDb2App.IsThisAController.Menus
{
    internal class GradesMenu
    {
        private static List<string> options = new List<string>
        {
            "Show Alarm List",
            "Show Grades by Course",
            "Show Grades by Class",
            "Show Grade Statistics",
            "Grade Course",
            "Back to Main Menu"
        };

        public static void Run(SchoolDb2Context context) // send the context lists?
        {
            while (true)
            {
                int selection = MenuDriver.Choice(options, "Grades Menu");
                var students = context.Students.ToList();
                var courses = context.Courses.ToList();
                var classes = context.Classes.ToList();
                var gradeScales = context.GradeScales.ToList();
                var grades = context.Grades.ToList();

                switch (selection)
                {
                    case 0:
                        var failingGrades = context.Grades.ToList().Where(g => g.GradeScaleId == 6).ToList();
                        Console.WriteLine("Students in need of support to pass:\n");
                        PrintStudentGrades(failingGrades, students, courses, gradeScales, classes);
                        MenuDriver.BackToSameMenu();
                        break;
                    case 1:
                        List<string> courseNames = courses.Select(c => c.CourseName).ToList();

                        int selectedCourse = MenuDriver.Choice(courseNames, "Select Course");
                        var courseToView = courses[selectedCourse];
                        List<Grade> courseGrade = context.Grades.Where(g => g.CourseId == courseToView.CourseId).ToList();

                        Console.WriteLine($"All grades for {courseToView.CourseName}:\n");
                        PrintStudentGrades(courseGrade, students, courses, gradeScales, classes);

                        MenuDriver.BackToSameMenu();
                        break;
                    case 2:
                        int selectedClass = MenuDriver.Choice(classes, "Select Class");
                        var classToView = classes[selectedClass];

                        MenuDriver.BackToSameMenu();
                        break;
                    case 3:
                        MenuDriver.BackToSameMenu();
                        break;
                    case 4:
                        MenuDriver.BackToSameMenu();
                        break;
                    case 5:
                        return;
                }
            }
        }


        //Can this use an include instead to also get the students with no grades?
        private static void PrintStudentGrades(List<Grade> grades, List<Student> students, List<Course> courses, List<GradeScale> gradeScales, List<Class> classes)
        {
            foreach (var grade in grades)
            {
                var student = students.FirstOrDefault(s => s.StudentId == grade.StudentId);
                var course = courses.FirstOrDefault(c => c.CourseId == grade.CourseId);
                var gradeScale = gradeScales.FirstOrDefault(g => g.GradeScaleId == grade.GradeScaleId);
                var studentClass = classes.FirstOrDefault(c => c.ClassId == student?.StudentClass);

                string studentName = $"{student?.StudentFirstName ?? "Unknown"} {student?.StudentLastName ?? "Unknown"}";
                string className = studentClass?.ClassName ?? "Unknown";
                string courseName = course?.CourseName ?? "Unknown";
                string gradeLetter = gradeScale?.GradeLetter ?? "?";

                Console.WriteLine($"{studentName,-20} {className,-10} {courseName,-20} {gradeLetter}");
            }
        }
    }
}
