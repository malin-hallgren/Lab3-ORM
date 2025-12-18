using Microsoft.EntityFrameworkCore;
using SchoolDb2App.Data;
using SchoolDb2App.IsThisAController;
using SchoolDb2App.IsThisAController.Menus;
using SchoolDb2App.Models;
using System;
using System.Collections.Generic;
using System.Text;
using static System.Collections.Specialized.BitVector32;

namespace SchoolDb2App.IsThisAController.Menus
{
    internal class MainMenu
    {
        private static List<string> options = new List<string>
        {
            "Show Students",
            "Add Student",
            "Show Employees",
            "Add Employee",
            "Course Management",
            "Grades Management",
            "Exit"
            // Expansion ideas: grade statistics, start course, set grades, alarm list (failing students)
        };

        public static void Run()
        {
            using var context = new SchoolDb2Context();

            while (true)
            {
                int selection = MenuDriver.Choice(options, "Welcome Admin");

                switch (selection)
                {
                    case 0:
                        ShowStudents(context.Students.ToList(), context);
                        MenuDriver.ReturnToMenu();
                        break;
                    case 1: //Take input and then create a student object to add to DB
                        CreateStudent(context);
                        MenuDriver.ReturnToMenu();
                        break;
                    case 2:
                        ShowEmployees(context.Employees.ToList(), context);
                        MenuDriver.ReturnToMenu();
                        break;
                    case 3: //take input to create an employee and add them to the db
                        var newEmployee = CreateEmployee(context);
                        Console.Clear();
                        PickRole(context, newEmployee);
                        PrintEmployee(context.Employees.ToList(), context);
                        MenuDriver.ReturnToMenu();
                        break;
                    case 4:
                        CourseMenu.Run(context);
                        MenuDriver.ReturnToMenu();
                        break;
                    case 5:
                        GradesMenu.Run(context);
                        MenuDriver.ReturnToMenu();
                        break;
                    case 6:
                        Environment.Exit(0);
                        break;
                }
            }
        }

        private static void ShowStudents(List<Student> students, SchoolDb2Context context)
        {
            var displayMode = new List<string>
            {
                "Display All Students",
                "Display Specific Class",
                "Return to Main Menu"
            };

            var sortingOptions = new List<string>
            {
                "Sort by First Name, Ascending",
                "Sort by Last Name, Ascending",
                "Sort by First Name, Descending",
                "Sort by Last Name, Descending",
                "No Sorting"
            };

            int displaySelection = MenuDriver.Choice(displayMode, "Select Display Mode");
            int sortingSelection = -1;
            List<Student> filteredStudents = new List<Student>();

            switch (displaySelection)
            {
                case 0:
                    Console.Clear();
                    sortingSelection = MenuDriver.Choice(sortingOptions, "Select Sorting Mode");
                    filteredStudents = SortStudents(sortingSelection, students);
                    break;
                case 1:
                    Console.Clear();
                    var classToView = SelectClass(context);
                    sortingSelection = MenuDriver.Choice(sortingOptions, "Select Sorting Mode");
                    filteredStudents = SortStudents(sortingSelection, students.Where(s => s.StudentClassNavigation == classToView).ToList());
                    break;
                case 2:
                    return;
            }
           // PrintStudent(filteredStudents, context);
            var selectedStudent = SelectStudent(context, filteredStudents);
            PrintStudent(new List<Student> { selectedStudent }, context);

        }

        private static List<Student> SortStudents(int sortingMode, List<Student> students)
        {
            return sortingMode switch
            {
                0 => students.OrderBy(s => s.StudentFirstName).ToList(),
                1 => students.OrderBy(s => s.StudentLastName).ToList(),
                2 => students.OrderByDescending(s => s.StudentFirstName).ToList(),
                3 => students.OrderByDescending(s => s.StudentLastName).ToList(),
                _ => new List<Student>(students)
            };
        }

        public static void PrintStudent(List<Student> studentsToPrint, SchoolDb2Context context)
        {
            Console.Clear();
            Console.WriteLine("\x1b[3J");

            var studentClassGrades= context.Students
                    .Include(s => s.StudentClassNavigation)
                    .Include(s => s.Grades).ThenInclude(g => g.Course)
                    .Include(s => s.Grades).ThenInclude(g => g.GradeScale)
                    .Where(s => studentsToPrint.Select(st => st.StudentId).Contains(s.StudentId))
                    .ToList();

            foreach (var student in studentsToPrint)
            {
                string studentFullName = $"{student.StudentFirstName} {student.StudentLastName}"; 
                //string studentClass = context.Classes.FirstOrDefault(c => c.ClassId == student.StudentClass)?.ClassName ?? "No Class Assigned";
                Console.WriteLine($"{studentFullName, -24} {student.StudentClassNavigation.ClassName}\n");

                var gradeList = new StringBuilder();
                foreach (var grade in student.Grades)
                {
                    gradeList.Append($"{grade.Course.CourseName, -25}{grade.GradeScale.GradeLetter, -10}{grade.GradeDate}\n");
                }
                Console.WriteLine(gradeList.ToString());
            }
            Console.ReadKey(true);
        }

        private static void CreateStudent(SchoolDb2Context context)
        {
            var classes = context.Classes.ToList();
            string studentFirstName = InputHelpers.ValidString("Input Student First Name");
            string studentLastName = InputHelpers.ValidString("Input Student Last Name");
            string ssn = InputHelpers.ValidString("Input Student SSN");
            int studentClassChoice = MenuDriver.Choice(classes.Select(c => c.ClassName).ToList(), "Select Class");

            var newStudent = new Student
            {
                StudentFirstName = studentFirstName,
                StudentLastName = studentLastName,
                StudentSsn = ssn,
                StudentClass = classes.Find(c => c.ClassId == classes[studentClassChoice].ClassId)?.ClassId
            };

            context.Students.Add(newStudent);
            context.SaveChanges();
        }
        private static void ShowEmployees(List<Employee> employees, SchoolDb2Context context)
        {
            var displayMode = new List<string>
            {
                "Display All Employees",
                "Display Employees in Role",
                "Return to Main Menu"
            };

            var sortingMode = new List<string>
            {
                "Sort by First Name, Ascending",
                "Sort by Last Name, Ascending",
                "Sort by First Name, Descending",
                "Sort by Last Name, Descending",
                "No Sorting"
            };

            int displaySelection = MenuDriver.Choice(displayMode, "Select Display Mode");
            int sortingSelection = -1;
            List<Employee> filteredEmployees = new List<Employee>();
            var roles = context.Roles.ToList();
            var employeeRoles = context.EmployeeRoles.ToList();

            switch (displaySelection)
            {
                case 0:
                    Console.Clear();
                    sortingSelection = MenuDriver.Choice(sortingMode, "Select Sorting Mode");
                    filteredEmployees = SortEmployees(sortingSelection, employees);
                    break;
                case 1:
                    Console.Clear();
                    int selectedRole = MenuDriver.Choice(roles.Select(r => r.RoleName).ToList(), "Select Role:") + 1;
                    sortingSelection = MenuDriver.Choice(sortingMode, "Select Sorting Mode");

                    //filter employees(e), by if they have the selectedRole in employeeRoles(er), if inner loop returns true, keep employee in the list sent to sorting
                    filteredEmployees = SortEmployees(sortingSelection, 
                                        employees
                                        .Where(e => employeeRoles
                                        .Any(er => er.EmployeeId == e.EmployeeId && er.RoleId == selectedRole)).ToList());
                    break;
                case 2:
                    return;
            }

            PrintEmployee(filteredEmployees, context);
        }

        private static Employee CreateEmployee(SchoolDb2Context context)
        {
            string employeeFirstName = InputHelpers.ValidString("Input Employee First Name");
            string employeeLastName = InputHelpers.ValidString("Input Employee Last Name");
            int employeeSalary = InputHelpers.ValidInt("Input Employee Salary");

            var newEmployee = new Employee
            {
                EmployeeFirstName = employeeFirstName,
                EmployeeLastName = employeeLastName,
                EmployeeSalary = employeeSalary,
                EmployeeStartDate = DateOnly.FromDateTime(DateTime.Now)
            };

            context.Employees.Add(newEmployee);
            context.SaveChanges();

            return newEmployee;
        }

        private static void PickRole(SchoolDb2Context context, Employee newEmployee)
        {
            var roles = context.Roles.ToList();
            var roleMenu = roles.Select(r => r.RoleName).ToList();
            roleMenu.Add("Finish Role Selection");

            var selectedRoles = new List<int>();
            bool selectingRoles = true;

            while (selectingRoles)
            {
                int selectedRole = MenuDriver.Choice(roleMenu, "Select Role for Employee");
                if (selectedRole == roleMenu.Count - 1)
                {
                    if (selectedRoles.Count == 0)
                    {
                        Console.WriteLine("Employee must have at least one role.\nPress ENTER to return to role selection...");
                        Console.ReadLine();
                        Console.Clear();
                        continue;
                    }

                    selectingRoles = false;
                }
                else if (selectedRoles.Contains(selectedRole))
                {
                    Console.WriteLine("This role has already been selected.\nPress Enter to return to role selection..."); //could I colour already selected roles?
                    Console.ReadLine();
                    Console.Clear();
                    continue;
                }
                else
                {
                    selectedRoles.Add(selectedRole);
                }
            }

            foreach (var role in selectedRoles)
            {
                var newEmployeeRole = new EmployeeRole
                {
                    EmployeeId = newEmployee.EmployeeId,
                    RoleId = roles[role].RoleId
                };
                context.EmployeeRoles.Add(newEmployeeRole);
            }
            context.SaveChanges();
        }
        private static List<Employee> SortEmployees(int sortingMode, List<Employee> employees)
        {
            return sortingMode switch
            {
                0 => employees.OrderBy(s => s.EmployeeFirstName).ToList(),
                1 => employees.OrderBy(s => s.EmployeeLastName).ToList(),
                2 => employees.OrderByDescending(s => s.EmployeeFirstName).ToList(),
                3 => employees.OrderByDescending(s => s.EmployeeLastName).ToList(),
                _ => new List<Employee>(employees)
            };
        }

        private static void PrintEmployee(List<Employee> employeesToPrint, SchoolDb2Context context)
        {
            Console.Clear();
            Console.WriteLine("\x1b[3J");
            var employeeIds = employeesToPrint.Select(e => e.EmployeeId).ToList();

            var employeesWithRoles = context.Employees
                    .Include(e => e.EmployeeRoles)
                    .ThenInclude(er => er.Role)
                    .Where(e => employeeIds.Contains(e.EmployeeId))
                    .ToList();

            foreach (var emp in employeesWithRoles)
            {
                string empRole = string.Join(", ", emp.EmployeeRoles.Select(er => er.Role.RoleName));

                Console.WriteLine($"{emp.EmployeeFirstName,-5} {emp.EmployeeLastName,-25} {empRole}");
            }
        }

        public static Class SelectClass(SchoolDb2Context context)
        {
            var classSelection = MenuDriver.Choice(context.Classes.Select(c => c.ClassName).ToList(), "Select Class");
            var classToView = context.Classes.FirstOrDefault(c => c.ClassId == context.Classes.ToList()[classSelection].ClassId);
            return classToView; 
        }

        public static Student SelectStudent(SchoolDb2Context context, List<Student> filteredStudents)
        {
            var studentSelection = MenuDriver.Choice(filteredStudents.Select(s => s.StudentFirstName + " " + s.StudentLastName).ToList(), "Select Student");
            var selectedStudent = filteredStudents.FirstOrDefault(s => s.StudentId == filteredStudents[studentSelection].StudentId);
            return selectedStudent;
        }

        public static Course SelectCourse(SchoolDb2Context context, List<Course> filteredCourses)
        {
            var courseSelection = MenuDriver.Choice(filteredCourses.Select(c => c.CourseName).ToList(), "Select Course");
            var courseToView = context.Courses.FirstOrDefault(c => c.CourseId == filteredCourses[courseSelection].CourseId);
            return courseToView;
        }

        public static Employee SelectEmployee(SchoolDb2Context context, List<Employee> filteredEmployee)
        {
            var employeeSelection = MenuDriver.Choice(filteredEmployee.Select(e => e.EmployeeFirstName + " " + e.EmployeeLastName).ToList(), "Select Employee");
            var selectedEmployee = filteredEmployee.FirstOrDefault(e => e.EmployeeId == filteredEmployee[employeeSelection].EmployeeId);
            return selectedEmployee;

        }
    }
}