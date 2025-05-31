using System;
using System.Linq;

namespace StudentGradingSystem
{
    /// <summary>
    /// Main program class that serves as the entry point for the Student Grading System.
    /// Handles the user interface, menu system, and program flow.
    /// </summary>
    class Program
    {
        /// <summary>
        /// Singleton instance of the GradingSystem that manages all student records and operations.
        /// </summary>
        private static readonly GradingSystem system = GradingSystem.Instance;

        /// <summary>
        /// The main entry point for the application.
        /// Initializes the console interface and runs the main program loop.
        /// </summary>
        /// <param name="args">Command line arguments (not used in this application)</param>
        static void Main(string[] args)
        {
            Console.Title = "Student Grading System";
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Welcome to the Student Grading System!");
            Console.ResetColor();

            while (true)
            {
                DisplayMainMenu();
                string? choice = Console.ReadLine()?.Trim();

                switch (choice)
                {
                    case "1":
                        CreateNewStudent();
                        break;
                    case "2":
                        EnterMarks();
                        break;
                    case "3":
                        UpdateMarks();
                        break;
                    case "4":
                        ShowStudentRecord();
                        break;
                    case "5":
                        ShowAllStudents();
                        break;
                    case "6":
                        ShowSystemStats();
                        break;
                    case "7":
                        ExportDataToCSV();
                        break;
                    case "8":
                        if (ConfirmExit())
                            return;
                        break;
                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("\nInvalid choice. Please try again.");
                        Console.ResetColor();
                        break;
                }

                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey(true);
                Console.Clear();
            }
        }

        /// <summary>
        /// Displays the main menu options to the user.
        /// Shows all available operations in the system.
        /// </summary>
        private static void DisplayMainMenu()
        {
            Console.WriteLine("\n=== Main Menu ===");
            Console.WriteLine("1. Create New Student Record");
            Console.WriteLine("2. Enter Marks for Student");
            Console.WriteLine("3. Update Student Marks");
            Console.WriteLine("4. Show Student Record");
            Console.WriteLine("5. Show All Students");
            Console.WriteLine("6. Show System Statistics");
            Console.WriteLine("7. Export Data to CSV");
            Console.WriteLine("8. Exit");
            Console.Write("\nEnter your choice (1-8): ");
        }

        /// <summary>
        /// Creates a new student record in the system.
        /// Prompts for student name and generates a unique student number.
        /// </summary>
        /// <remarks>
        /// The student number is automatically generated as an 8-digit unique identifier.
        /// Student names cannot be empty and are trimmed of whitespace.
        /// </remarks>
        private static void CreateNewStudent()
        {
            Console.WriteLine("\n=== Create New Student Record ===");
            Console.Write("Enter student name: ");
            string? name = Console.ReadLine()?.Trim();

            if (string.IsNullOrWhiteSpace(name))
            {
                ShowError("Student name cannot be empty.");
                return;
            }

            try
            {
                string studentNumber = system.CreateNewRecord(name);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\nStudent record created successfully!");
                Console.WriteLine($"Student Number: {studentNumber}");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }

        /// <summary>
        /// Handles the process of entering marks for a student.
        /// Validates the student number and prompts for 6 marks.
        /// </summary>
        /// <remarks>
        /// Each mark must be between 0 and 100.
        /// The student must exist in the system.
        /// </remarks>
        private static void EnterMarks()
        {
            Console.WriteLine("\n=== Enter Marks ===");
            string? studentNumber = GetStudentNumber();

            if (studentNumber != null)
            {
                if (system.EnterMarks(studentNumber))
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("\nMarks entered successfully!");
                    Console.ResetColor();
                }
            }
        }

        /// <summary>
        /// Updates existing marks for a student, adding to their mark history.
        /// Validates the student number and prompts for new marks.
        /// </summary>
        /// <remarks>
        /// Each mark must be between 0 and 100.
        /// The student must exist in the system.
        /// Previous marks are preserved in the history.
        /// </remarks>
        private static void UpdateMarks()
        {
            Console.WriteLine("\n=== Update Marks ===");
            string? studentNumber = GetStudentNumber();

            if (studentNumber != null)
            {
                if (system.UpdateMarks(studentNumber))
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("\nMarks updated successfully!");
                    Console.ResetColor();
                }
            }
        }

        /// <summary>
        /// Displays the complete record for a specific student.
        /// Shows student information and all mark attempts.
        /// </summary>
        /// <remarks>
        /// The student must exist in the system.
        /// Shows all historical mark entries with timestamps.
        /// </remarks>
        private static void ShowStudentRecord()
        {
            Console.WriteLine("\n=== Show Student Record ===");
            string? studentNumber = GetStudentNumber();

            if (studentNumber != null)
            {
                var student = system.GetStudent(studentNumber);
                if (student != null)
                {
                    student.DisplayRecordAsTable();
                }
            }
        }

        /// <summary>
        /// Displays a list of all students in the system.
        /// Shows student numbers and names in alphabetical order.
        /// </summary>
        /// <remarks>
        /// Includes a count of total students.
        /// Students are sorted by name for easy reference.
        /// </remarks>
        private static void ShowAllStudents()
        {
            Console.WriteLine("\n=== All Students ===");
            var students = system.GetAllStudents().ToList();

            if (!students.Any())
            {
                Console.WriteLine("No students in the system.");
                return;
            }

            // ASCII Table Constants and Helpers (Similar to StudentRecord.cs)
            const int numberWidth = 15; // Adjust width as needed
            const int nameWidth = 30;   // Adjust width as needed
            const char borderChar = '|';
            const char horizontalChar = '-';
            const char cornerChar = '+';

            string CreateBorder(int numW, int nameW)
            {
                return cornerChar +
                       new string(horizontalChar, numW) + cornerChar +
                       new string(horizontalChar, nameW) + cornerChar;
            }

            string CreateRow(string number, string name, int numW, int nameW)
            {
                return borderChar +
                       number.PadRight(numW) + borderChar +
                       name.PadRight(nameW) + borderChar;
            }

            // Display Table Header
            Console.WriteLine(CreateBorder(numberWidth, nameWidth));
            Console.WriteLine(CreateRow("Student Number", "Student Name", numberWidth, nameWidth));
            Console.WriteLine(CreateBorder(numberWidth, nameWidth));

            // Display Student Data
            foreach (var (number, name) in students)
            {
                Console.WriteLine(CreateRow(number, name, numberWidth, nameWidth));
            }

            // Display Table Footer
            Console.WriteLine(CreateBorder(numberWidth, nameWidth));

            Console.WriteLine($"\nTotal Students: {students.Count}");
        }

        /// <summary>
        /// Displays system-wide statistics including total students and pass rate.
        /// </summary>
        /// <remarks>
        /// Shows the average pass rate across all students with marks.
        /// Calculates statistics based on the latest mark attempts.
        /// </remarks>
        private static void ShowSystemStats()
        {
            Console.WriteLine("\n=== System Statistics ===");
            Console.WriteLine($"Total Students: {system.StudentCount}");
            Console.WriteLine($"Average Pass Rate: {system.AveragePassRate:F2}%");
        }

        /// <summary>
        /// Prompts for and validates a student number from user input.
        /// </summary>
        /// <returns>The validated student number, or null if invalid</returns>
        /// <remarks>
        /// Validates that the student number:
        /// - Is not empty
        /// - Exists in the system
        /// </remarks>
        private static string? GetStudentNumber()
        {
            Console.Write("Enter student number: ");
            string? studentNumber = Console.ReadLine()?.Trim();

            if (string.IsNullOrWhiteSpace(studentNumber))
            {
                ShowError("Student number cannot be empty.");
                return null;
            }

            if (!system.GetAllStudentNumbers().Contains(studentNumber))
            {
                ShowError("Student record not found.");
                return null;
            }

            return studentNumber;
        }

        /// <summary>
        /// Prompts the user to confirm program exit.
        /// </summary>
        /// <returns>True if the user confirms exit, false otherwise</returns>
        /// <remarks>
        /// Accepts 'y', 'yes', 'n', or 'no' as valid responses.
        /// Case-insensitive input handling.
        /// </remarks>
        private static bool ConfirmExit()
        {
            Console.Write("\nAre you sure you want to exit? (y/n): ");
            string? response = Console.ReadLine()?.Trim().ToLower();
            return response == "y" || response == "yes";
        }

        /// <summary>
        /// Displays an error message to the user in red text.
        /// </summary>
        /// <param name="message">The error message to display</param>
        /// <remarks>
        /// Uses red console color for visibility.
        /// Resets console color after display.
        /// </remarks>
        private static void ShowError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\nError: {message}");
            Console.ResetColor();
        }

        /// <summary>
        /// Exports all student data to a CSV file.
        /// </summary>
        /// <remarks>
        /// Prompts for a filename without extension.
        /// Creates a CSV file with all student records and marks.
        /// Includes headers and formatted data.
        /// </remarks>
        private static void ExportDataToCSV()
        {
            Console.WriteLine("\n=== Export Data to CSV ===");
            Console.Write("Enter filename (without extension): ");
            string? filename = Console.ReadLine()?.Trim();

            if (string.IsNullOrWhiteSpace(filename))
            {
                ShowError("Filename cannot be empty.");
                return;
            }

            try
            {
                string filePath = system.ExportToCSV(filename);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\nData exported successfully to: {filePath}");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                ShowError($"Failed to export data: {ex.Message}");
            }
        }
    }
} 