using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace StudentGradingSystem
{
    /// <summary>
    /// Represents the data structure for storing all student records
    /// </summary>
    public class StudentData
    {
        [JsonPropertyName("students")]
        public List<StudentRecord> Students { get; set; } = new List<StudentRecord>();

        [JsonPropertyName("lastUpdated")]
        public DateTime LastUpdated { get; set; } = DateTime.Now;
    }

    /// <summary>
    /// Core system class that manages all student grading operations
    /// Implements the Singleton pattern to ensure a single instance manages the system
    /// </summary>
    public sealed class GradingSystem
    {
        private static readonly Lazy<GradingSystem> instance = new Lazy<GradingSystem>(() => new GradingSystem());
        private readonly Dictionary<string, StudentRecord> students;
        private readonly string dataFilePath;
        private readonly Random random;
        private readonly JsonSerializerOptions jsonOptions;
        private readonly object fileLock = new object();

        /// <summary>
        /// Gets the singleton instance of the GradingSystem
        /// </summary>
        public static GradingSystem Instance => instance.Value;

        /// <summary>
        /// Gets the total number of students in the system
        /// </summary>
        public int StudentCount => students.Count;

        /// <summary>
        /// Gets the average pass rate across all students
        /// </summary>
        public double AveragePassRate
        {
            get
            {
                var studentsWithMarks = students.Values.Where(s => s.HasMarks()).ToList();
                if (!studentsWithMarks.Any()) return 0;

                int passCount = studentsWithMarks.Count(s => s.GetLatestMarks()?.PassStatus == "PASS");
                return (double)passCount / studentsWithMarks.Count * 100;
            }
        }

        private GradingSystem()
        {
            students = new Dictionary<string, StudentRecord>();
            dataFilePath = Path.Combine("data", "students.json");
            random = new Random();
            jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            // Ensure data directory exists
            Directory.CreateDirectory(Path.GetDirectoryName(dataFilePath)!);
            LoadFromFile();
        }

        /// <summary>
        /// Creates a new student record with a unique 8-digit student number
        /// </summary>
        /// <param name="studentName">The name of the student</param>
        /// <returns>The generated student number</returns>
        /// <exception cref="ArgumentException">Thrown when student name is invalid</exception>
        public string CreateNewRecord(string studentName)
        {
            if (string.IsNullOrWhiteSpace(studentName))
                throw new ArgumentException("Student name cannot be empty.");

            if (studentName.Length > 50)
                throw new ArgumentException("Student name cannot exceed 50 characters.");

            string studentNumber;
            int attempts = 0;
            const int maxAttempts = 100;

            do
            {
                studentNumber = GenerateStudentNumber();
                attempts++;
                if (attempts >= maxAttempts)
                    throw new InvalidOperationException("Unable to generate unique student number after multiple attempts.");
            } while (students.ContainsKey(studentNumber));

            var record = new StudentRecord(studentNumber, studentName);
            students.Add(studentNumber, record);
            SaveToFile();
            return studentNumber;
        }

        /// <summary>
        /// Generates a unique 8-digit student number
        /// </summary>
        private string GenerateStudentNumber()
        {
            return random.Next(10000000, 100000000).ToString();
        }

        /// <summary>
        /// Enters marks for a student
        /// </summary>
        /// <param name="studentNumber">The student's number</param>
        /// <returns>True if marks were entered successfully</returns>
        public bool EnterMarks(string studentNumber)
        {
            if (!ValidateStudentNumber(studentNumber))
                return false;

            try
            {
                // Display available subjects
                Subjects.DisplaySubjectsForMarkEntry();
                
                // Get marks for all subjects
                var marks = Subjects.GetMarksFromUser();
                
                // Add marks to student record
                students[studentNumber].AddMarks(marks);
                SaveToFile();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Updates marks for a student, adding to their history
        /// </summary>
        /// <param name="studentNumber">The student's number</param>
        /// <returns>True if marks were updated successfully</returns>
        public bool UpdateMarks(string studentNumber)
        {
            if (!ValidateStudentNumber(studentNumber))
                return false;

            try
            {
                // Show current marks if any exist
                var student = GetStudent(studentNumber);
                if (student != null && student.HasMarks())
                {
                    Console.WriteLine("\nCurrent marks for student:");
                    student.DisplayRecordAsTable();
                }

                // Display available subjects
                Subjects.DisplaySubjectsForMarkEntry();
                
                // Get new marks for all subjects
                var marks = Subjects.GetMarksFromUser();
                
                // Add new marks to student record
                student.AddMarks(marks);
                SaveToFile();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Validates a student number
        /// </summary>
        private bool ValidateStudentNumber(string studentNumber)
        {
            if (string.IsNullOrWhiteSpace(studentNumber))
            {
                Console.WriteLine("Error: Student number cannot be empty.");
                return false;
            }

            if (!students.ContainsKey(studentNumber))
            {
                Console.WriteLine("Error: Student record not found.");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Displays a student's record
        /// </summary>
        /// <param name="studentNumber">The student's number</param>
        public void ShowRecord(string studentNumber)
        {
            if (!ValidateStudentNumber(studentNumber))
                return;

            var student = GetStudent(studentNumber);
            if (student != null)
            {
                student.DisplayRecordAsTable();
            }
        }

        /// <summary>
        /// Gets a list of all student numbers
        /// </summary>
        public IEnumerable<string> GetAllStudentNumbers()
        {
            return students.Keys.OrderBy(k => k);
        }

        /// <summary>
        /// Gets a list of all student names and numbers
        /// </summary>
        public IEnumerable<(string Number, string Name)> GetAllStudents()
        {
            return students.Values
                .OrderBy(s => s.StudentName)
                .Select(s => (s.StudentNumber, s.StudentName));
        }

        /// <summary>
        /// Saves the current state to file
        /// </summary>
        private void SaveToFile()
        {
            try
            {
                var studentData = new StudentData
                {
                    Students = students.Values.ToList(),
                    LastUpdated = DateTime.Now
                };

                string jsonString = JsonSerializer.Serialize(studentData, jsonOptions);
                lock (fileLock)
                {
                    File.WriteAllText(dataFilePath, jsonString);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving to file: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Loads the state from file
        /// </summary>
        private void LoadFromFile()
        {
            if (!File.Exists(dataFilePath))
                return;

            try
            {
                string jsonString;
                lock (fileLock)
                {
                    jsonString = File.ReadAllText(dataFilePath);
                }

                var studentData = JsonSerializer.Deserialize<StudentData>(jsonString, jsonOptions);

                if (studentData?.Students != null)
                {
                    students.Clear();
                    foreach (var student in studentData.Students)
                    {
                        students[student.StudentNumber] = student;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading from file: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Exports student data to a CSV file
        /// </summary>
        /// <param name="filename">The base filename without extension</param>
        /// <returns>The full path to the created file</returns>
        public string ExportToCSV(string filename)
        {
            if (string.IsNullOrWhiteSpace(filename))
                throw new ArgumentException("Filename cannot be empty.", nameof(filename));

            // Ensure filename is valid
            filename = string.Join("_", filename.Split(Path.GetInvalidFileNameChars()));
            string filePath = Path.Combine("exports", $"{filename}_{DateTime.Now:yyyyMMdd_HHmmss}.csv");

            // Ensure exports directory exists
            Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);

            using var writer = new StreamWriter(filePath);
            
            // Write header with subject codes
            var header = new List<string> { "Student Number", "Student Name", "Attempt Date" };
            header.AddRange(Subjects.All.Select(s => s.Code));
            header.AddRange(new[] { "Average", "Status" });
            writer.WriteLine(string.Join(",", header));

            // Write data
            foreach (var student in students.Values.OrderBy(s => s.StudentNumber))
            {
                if (!student.HasMarks())
                {
                    writer.WriteLine($"{student.StudentNumber},{student.StudentName},,,,,");
                    continue;
                }

                foreach (var entry in student.MarksHistory)
                {
                    var row = new List<string>
                    {
                        student.StudentNumber,
                        student.StudentName,
                        entry.Timestamp.ToString("yyyy-MM-dd HH:mm:ss")
                    };

                    // Add marks for each subject in order
                    foreach (var subject in Subjects.All)
                    {
                        row.Add(entry.SubjectMarks[subject.Code].ToString());
                    }

                    // Add average and status
                    row.Add(entry.AverageMark.ToString("F2"));
                    row.Add(entry.PassStatus);

                    writer.WriteLine(string.Join(",", row));
                }
            }

            return Path.GetFullPath(filePath);
        }

        /// <summary>
        /// Gets a student record by student number
        /// </summary>
        /// <param name="studentNumber">The student's number</param>
        /// <returns>The student record if found, null otherwise</returns>
        public StudentRecord? GetStudent(string studentNumber)
        {
            if (string.IsNullOrWhiteSpace(studentNumber))
                return null;

            return students.TryGetValue(studentNumber, out var student) ? student : null;
        }
    }
} 