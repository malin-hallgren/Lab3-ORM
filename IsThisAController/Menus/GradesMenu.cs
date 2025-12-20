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
            "Set Grade",
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
                    case 0: //Alarm List
                        var failingGrades = context.Grades.ToList().Where(g => g.GradeScaleId == 6).ToList();
                        Console.WriteLine("Students in need of support to pass:\n");
                        PrintStudentGrades(failingGrades, context);
                        MenuDriver.ReturnToMenu();
                        break;
                    case 1: //Grades by Course
                        List<string> courseNames = courses.Select(c => c.CourseName).ToList();

                        int selectedCourse = MenuDriver.Choice(courseNames, "Select Course");
                        var courseToView = courses[selectedCourse];
                        List<Grade> courseGrade = context.Grades.Where(g => g.CourseId == courseToView.CourseId).ToList();

                        Console.WriteLine($"All grades for {courseToView.CourseName}:\n");
                        PrintStudentGrades(courseGrade, context);

                        MenuDriver.ReturnToMenu();
                        break;
                    case 2: //Grades by Class
                        int selectedClass = MenuDriver.Choice(classes.Select(c => c.ClassName).ToList(), "Select Class");
                        var classToView = classes.FirstOrDefault(c => c.ClassName == classes[selectedClass].ClassName);
                        var filteredStudents = students.Where(s => s.StudentClassNavigation == classToView).ToList();
                        var filteredGrades = grades.Where(g => filteredStudents.Any(s => s.StudentId == g.StudentId)).ToList();
                        PrintStudentGrades(filteredGrades, context);
                        MenuDriver.ReturnToMenu();
                        break;
                    case 3://Set Grade
                        SetGrade(context, students, courses, grades);
                        grades = context.Grades.ToList(); 
                        MenuDriver.ReturnToMenu();
                        break;
                    case 4:
                        return;
                }
            }
        }

        // Let's user select a student and a grade a course that they do not yet have a grade in
        private static void SetGrade(SchoolDb2Context context, List<Student> students, List<Course> courses, List<Grade> grades)
        {
            var classToView = MainMenu.SelectClass(context);
            var filteredStudents = students.Where(s => s.StudentClassNavigation == classToView).ToList();
            var studentToGrade = MainMenu.SelectStudent(context, filteredStudents);
            List<Course> filteredCourses = courses
                .Where(c => !grades
                .Any(g => g.StudentId == studentToGrade.StudentId && g.CourseId == c.CourseId))
                .ToList();
            
            var courseToGrade = MainMenu.SelectCourse(context, filteredCourses);

            string gradingHeader = $"Which shall be set for grade for {studentToGrade.StudentFirstName} {studentToGrade.StudentLastName} in {courseToGrade.CourseName}?";
            var gradesMenu = context.GradeScales.Select(g => g.GradeLetter).ToList();
            int gradeSelection = MenuDriver.Choice(gradesMenu, gradingHeader);
            GradeScale gradeScaleToSet = context.GradeScales.FirstOrDefault(gs => gs.GradeLetter == gradesMenu[gradeSelection]);

            using var transaction = context.Database.BeginTransaction();

            try
            {
                context.Grades.Add(new Grade
                {
                    StudentId = studentToGrade.StudentId,
                    CourseId = courseToGrade.CourseId,
                    GradeScaleId = gradeScaleToSet.GradeScaleId,
                    GradeDate = DateOnly.FromDateTime(DateTime.Now)
                });

                context.SaveChanges();
                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new InvalidOperationException("Error setting grade.", ex);
            }
        }

        //Prints students and their grade in a course, ordered by student ID and course ID
        private static void PrintStudentGrades(List<Grade> grades, SchoolDb2Context context)
        {
            var sortedGrades = grades.OrderBy(g => g.StudentId).ThenBy(g => g.CourseId).ToList();

            Console.Clear();
            Console.WriteLine("\x1b[3J");

            foreach (var grade in sortedGrades)
            {
                var toPrint = context.Grades
                    .Include(g => g.Student)
                    .ThenInclude(s => s.StudentClassNavigation)
                    .Include(g => g.Course)
                    .Include(g => g.GradeScale)
                    .Where(g => g.GradeId == grade.GradeId)
                    .FirstOrDefault();

                string studentName = toPrint?.Student?.StudentFirstName + " " + toPrint?.Student?.StudentLastName;

                string studentGrade = $"{studentName, -20}" +
                                     $"{toPrint?.Student?.StudentClassNavigation?.ClassName, -10}" +
                                     $"{toPrint?.Course?.CourseName, -20}" +
                                     $"{toPrint?.GradeScale?.GradeLetter ?? "Not yet set"}";

                Console.WriteLine(studentGrade);
            }
        }
    }
}
