using System;
using System.Linq;

namespace StudentGradingSystem
{
    /// <summary>
    /// Represents a Computer Science subject in the grading system
    /// </summary>
    public class Subject
    {
        public string Code { get; }
        public string Name { get; }
        public int CreditHours { get; }
        public int PassingMark { get; }

        public Subject(string code, string name, int creditHours, int passingMark)
        {
            Code = code;
            Name = name;
            CreditHours = creditHours;
            PassingMark = passingMark;
        }

        public override string ToString()
        {
            return $"{Code} - {Name} ({CreditHours} credits)";
        }
    }

    /// <summary>
    /// Manages the Computer Science subjects and mark entry process
    /// </summary>
    public static class Subjects
    {
        // Core Computer Science Subjects (6 subjects)
        public static readonly Subject[] All = new Subject[]
        {
            new Subject("CS101", "Programming Fundamentals", 4, 40),
            new Subject("CS102", "Object-Oriented Programming", 4, 40),
            new Subject("CS103", "Data Structures", 4, 40),
            new Subject("CS104", "Database Systems", 3, 40),
            new Subject("CS105", "Web Development", 3, 40),
            new Subject("CS106", "Computer Networks", 3, 40)
        };

        /// <summary>
        /// Displays available subjects for mark entry
        /// </summary>
        public static void DisplaySubjectsForMarkEntry()
        {
            Console.WriteLine("\nEnter marks for the following subjects:");
            Console.WriteLine("----------------------------------------");
            
            for (int i = 0; i < All.Length; i++)
            {
                var subject = All[i];
                Console.WriteLine($"{i + 1}. {subject.Code} - {subject.Name}");
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Gets marks for all subjects from user input
        /// </summary>
        /// <returns>Dictionary of subject codes and their marks</returns>
        public static Dictionary<string, int> GetMarksFromUser()
        {
            var marks = new Dictionary<string, int>();
            
            Console.WriteLine("\nEnter marks for each subject (0-100):");
            Console.WriteLine("-------------------------------------");

            foreach (var subject in All)
            {
                bool validInput = false;
                while (!validInput)
                {
                    Console.Write($"Enter mark for {subject.Code} ({subject.Name}): ");
                    string? input = Console.ReadLine()?.Trim();
                    
                    if (string.IsNullOrEmpty(input))
                    {
                        Console.WriteLine("Error: Mark cannot be empty.");
                        continue;
                    }

                    if (int.TryParse(input, out int mark) && mark >= 0 && mark <= 100)
                    {
                        marks[subject.Code] = mark;
                        validInput = true;
                    }
                    else
                    {
                        Console.WriteLine("Error: Please enter a valid number between 0 and 100.");
                    }
                }
            }

            return marks;
        }

        /// <summary>
        /// Gets a subject by its code
        /// </summary>
        public static Subject? GetByCode(string code)
        {
            return All.FirstOrDefault(s => s.Code.Equals(code, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Displays all available subjects
        /// </summary>
        public static void DisplayAllSubjects()
        {
            Console.WriteLine("\nAvailable Computer Science Subjects:");
            Console.WriteLine("----------------------------------");
            
            foreach (var subject in All)
            {
                Console.WriteLine(subject.ToString());
            }
            Console.WriteLine();
        }
    }
} 