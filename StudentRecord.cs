using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace StudentGradingSystem
{
    /// <summary>
    /// Represents a single student's record including their marks history
    /// </summary>
    public class StudentRecord
    {
        /// <summary>
        /// Gets the student's unique 8-digit number
        /// </summary>
        [JsonPropertyName("studentNumber")]
        public string StudentNumber { get; }

        /// <summary>
        /// Gets the student's full name
        /// </summary>
        [JsonPropertyName("studentName")]
        public string StudentName { get; }

        /// <summary>
        /// Gets the list of mark entries for this student
        /// </summary>
        [JsonPropertyName("marksHistory")]
        public List<MarksEntry> MarksHistory { get; } = new List<MarksEntry>();

        /// <summary>
        /// Gets the date and time when this record was created
        /// </summary>
        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; } = DateTime.Now;

        /// <summary>
        /// Gets the date and time when this record was last modified
        /// </summary>
        [JsonPropertyName("lastModified")]
        public DateTime LastModified { get; private set; } = DateTime.Now;

        /// <summary>
        /// Gets the total number of mark entries in the history
        /// </summary>
        public int MarksHistoryCount => MarksHistory.Count;

        /// <summary>
        /// Gets whether the student has any marks recorded
        /// </summary>
        public bool HasMarks() => MarksHistory.Any();

        /// <summary>
        /// Gets the latest marks entry for this student
        /// </summary>
        public MarksEntry? GetLatestMarks() => MarksHistory.LastOrDefault();

        /// <summary>
        /// Gets the average mark across all entries
        /// </summary>
        public double AverageMark
        {
            get
            {
                if (!HasMarks()) return 0;
                return MarksHistory.Average(m => m.AverageMark);
            }
        }

        /// <summary>
        /// Gets the highest average mark achieved
        /// </summary>
        public double HighestAverageMark
        {
            get
            {
                if (!HasMarks()) return 0;
                return MarksHistory.Max(m => m.AverageMark);
            }
        }

        /// <summary>
        /// Gets the lowest average mark achieved
        /// </summary>
        public double LowestAverageMark
        {
            get
            {
                if (!HasMarks()) return 0;
                return MarksHistory.Min(m => m.AverageMark);
            }
        }

        /// <summary>
        /// Gets the number of times the student has passed
        /// </summary>
        public int PassCount => MarksHistory.Count(m => m.PassStatus == "PASS");

        /// <summary>
        /// Gets the number of times the student has failed
        /// </summary>
        public int FailCount => MarksHistory.Count(m => m.PassStatus == "FAIL");

        /// <summary>
        /// Creates a new student record
        /// </summary>
        /// <param name="studentNumber">The student's unique number</param>
        /// <param name="studentName">The student's full name</param>
        /// <exception cref="ArgumentException">Thrown when student number or name is invalid</exception>
        public StudentRecord(string studentNumber, string studentName)
        {
            if (string.IsNullOrWhiteSpace(studentNumber))
                throw new ArgumentException("Student number cannot be empty.", nameof(studentNumber));

            if (string.IsNullOrWhiteSpace(studentName))
                throw new ArgumentException("Student name cannot be empty.", nameof(studentName));

            if (studentNumber.Length != 8 || !studentNumber.All(char.IsDigit))
                throw new ArgumentException("Student number must be 8 digits.", nameof(studentNumber));

            StudentNumber = studentNumber;
            StudentName = studentName.Trim();
        }

        /// <summary>
        /// Adds a new set of marks to the student's history
        /// </summary>
        /// <param name="subjectMarks">Dictionary of subject codes and their marks</param>
        /// <exception cref="ArgumentException">Thrown when marks are invalid</exception>
        public void AddMarks(Dictionary<string, int> subjectMarks)
        {
            if (subjectMarks == null)
                throw new ArgumentException("Marks cannot be null.", nameof(subjectMarks));

            if (subjectMarks.Count != Subjects.All.Length)
                throw new ArgumentException($"Exactly {Subjects.All.Length} subject marks are required.", nameof(subjectMarks));

            if (subjectMarks.Values.Any(m => m < 0 || m > 100))
                throw new ArgumentException("All marks must be between 0 and 100.", nameof(subjectMarks));

            // Validate that all required subjects are present
            foreach (var subject in Subjects.All)
            {
                if (!subjectMarks.ContainsKey(subject.Code))
                    throw new ArgumentException($"Missing marks for subject {subject.Code}", nameof(subjectMarks));
            }

            var entry = new MarksEntry(subjectMarks);
            MarksHistory.Add(entry);
            LastModified = DateTime.Now;
        }

        /// <summary>
        /// Displays the student's record and mark history in a formatted ASCII table
        /// </summary>
        public void DisplayRecordAsTable()
        {
            // Constants for table formatting
            const int attemptWidth = 8;
            const int dateWidth = 10;
            const int markWidth = 8; // Increased width for marks to accommodate "N/A"
            const int avgWidth = 8; // Increased width for average
            const int statusWidth = 10; // Increased width for status
            const char borderChar = '|';
            const char horizontalChar = '-';
            const char cornerChar = '+';

            // Helper function to create a horizontal border
            string CreateBorder(int attemptW, int dateW, int markW, int avgW, int statusW, int subjectCount)
            {
                return cornerChar +
                       new string(horizontalChar, attemptW) + cornerChar +
                       new string(horizontalChar, dateW) + cornerChar +
                       string.Join(cornerChar, Enumerable.Repeat(new string(horizontalChar, markW), subjectCount)) + cornerChar +
                       new string(horizontalChar, avgW) + cornerChar +
                       new string(horizontalChar, statusW) + cornerChar;
            }

            // Helper function to create a row with data
            string CreateRow(string attempt, string date, string[] marks, string avg, string status, int attemptW, int dateW, int markW, int avgW, int statusW)
            {
                return borderChar +
                       attempt.PadRight(attemptW) + borderChar +
                       date.PadRight(dateW) + borderChar +
                       string.Join(borderChar, marks.Select(m => m.PadRight(markW))) + borderChar +
                       avg.PadRight(avgW) + borderChar +
                       status.PadRight(statusW) + borderChar;
            }

            var subjectCodes = Subjects.All.Select(s => s.Code).ToArray();
            int subjectCount = subjectCodes.Length;

            // Display student information header
            Console.WriteLine("\nStudent Record Summary");
            // Pass dimensions to CreateBorder based on constants and dynamic subject count
            Console.WriteLine(CreateBorder(attemptWidth, dateWidth, markWidth, avgWidth, statusWidth, subjectCount));
            // Pass dimensions to CreateRow based on constants
            Console.WriteLine(CreateRow(
                "Attempt", "Date", 
                subjectCodes, // Use dynamically generated subject codes
                "Avg", "Status",
                attemptWidth, dateWidth, markWidth, avgWidth, statusWidth
            ));
            // Pass dimensions to CreateBorder based on constants and dynamic subject count
            Console.WriteLine(CreateBorder(attemptWidth, dateWidth, markWidth, avgWidth, statusWidth, subjectCount));

            // Display student summary row
            string status = HasMarks() ? 
                (GetLatestMarks()?.PassStatus ?? "N/A") : "No Marks";
            
            Console.WriteLine(CreateRow(
                "Latest", 
                LastModified.ToString("MM/dd/yy"),
                Subjects.All.Select(s => 
                    GetLatestMarks()?.SubjectMarks.TryGetValue(s.Code, out int mark) == true ? 
                    mark.ToString() : "N/A").ToArray(),
                $"{AverageMark:F1}",
                status,
                attemptWidth, dateWidth, markWidth, avgWidth, statusWidth // Pass dimensions
            ));
            // Pass dimensions to CreateBorder based on constants and dynamic subject count
            Console.WriteLine(CreateBorder(attemptWidth, dateWidth, markWidth, avgWidth, statusWidth, subjectCount));

            // Display mark history if available
            if (HasMarks())
            {
                Console.WriteLine("\nMark History:");
                // Pass dimensions to CreateBorder based on constants and dynamic subject count
                Console.WriteLine(CreateBorder(attemptWidth, dateWidth, markWidth, avgWidth, statusWidth, subjectCount));
                // Pass dimensions to CreateRow based on constants
                Console.WriteLine(CreateRow(
                    "Attempt", "Date", 
                    subjectCodes, // Use dynamically generated subject codes
                    "Avg", "Status",
                    attemptWidth, dateWidth, markWidth, avgWidth, statusWidth
                ));
                // Pass dimensions to CreateBorder based on constants and dynamic subject count
                Console.WriteLine(CreateBorder(attemptWidth, dateWidth, markWidth, avgWidth, statusWidth, subjectCount));

                for (int i = 0; i < MarksHistory.Count; i++)
                {
                    var entry = MarksHistory[i];
                    var marks = Subjects.All.Select(s => 
                        entry.SubjectMarks.TryGetValue(s.Code, out int mark) ? 
                        mark.ToString() : "N/A").ToArray();

                    Console.WriteLine(CreateRow(
                        (i + 1).ToString(),
                        entry.Timestamp.ToString("MM/dd/yy"),
                        marks,
                        $"{entry.AverageMark:F1}",
                        entry.PassStatus,
                        attemptWidth, dateWidth, markWidth, avgWidth, statusWidth // Pass dimensions
                    ));
                }
                // Pass dimensions to CreateBorder based on constants and dynamic subject count
                Console.WriteLine(CreateBorder(attemptWidth, dateWidth, markWidth, avgWidth, statusWidth, subjectCount));
            }

            // Display summary statistics
            if (HasMarks())
            {
                Console.WriteLine($"\nPass Count: {PassCount} | Fail Count: {FailCount}");
            }
            Console.WriteLine();
        }
    }

    /// <summary>
    /// Represents a single entry of marks for a student
    /// </summary>
    public class MarksEntry
    {
        /// <summary>
        /// Gets the dictionary of subject codes and their marks
        /// </summary>
        [JsonPropertyName("subjectMarks")]
        public Dictionary<string, int> SubjectMarks { get; }

        /// <summary>
        /// Gets the timestamp when these marks were entered
        /// </summary>
        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; } = DateTime.Now;

        /// <summary>
        /// Gets the average of all marks
        /// </summary>
        [JsonPropertyName("averageMark")]
        public double AverageMark => SubjectMarks.Values.Average();

        /// <summary>
        /// Gets the pass/fail status based on the average mark
        /// </summary>
        [JsonPropertyName("passStatus")]
        public string PassStatus
        {
            get
            {
                // A student passes if they meet the passing mark for each subject
                bool allSubjectsPassed = Subjects.All.All(subject => 
                    SubjectMarks.TryGetValue(subject.Code, out int mark) && mark >= subject.PassingMark);
                
                return allSubjectsPassed ? "PASS" : "FAIL";
            }
        }

        /// <summary>
        /// Creates a new marks entry
        /// </summary>
        /// <param name="subjectMarks">Dictionary of subject codes and their marks</param>
        public MarksEntry(Dictionary<string, int> subjectMarks)
        {
            SubjectMarks = subjectMarks;
        }
    }
} 