using System;
using System.Linq;
using System.Text.Json.Serialization;

namespace StudentGradingSystem
{
    public class MarkSet
    {
        [JsonPropertyName("attemptDate")]
        public DateTime AttemptDate { get; set; }

        [JsonPropertyName("marks")]
        public int[] Marks { get; set; } = Array.Empty<int>();

        [JsonPropertyName("average")]
        public double Average { get; set; }

        [JsonPropertyName("passStatus")]
        public string PassStatus { get; set; } = string.Empty;

        public MarkSet()
        {
            // Required for JSON deserialization
            AttemptDate = DateTime.Now;
        }

        public MarkSet(int[] marks)
        {
            if (marks == null || marks.Length != 6)
                throw new ArgumentException("Exactly 6 marks are required.");

            if (marks.Any(m => m < 0 || m > 100))
                throw new ArgumentException("All marks must be between 0 and 100.");

            Marks = marks;
            AttemptDate = DateTime.Now;
            CalculateAverage();
            DeterminePassStatus();
        }

        public void CalculateAverage()
        {
            if (Marks != null && Marks.Length > 0)
            {
                Average = Marks.Average();
            }
        }

        public void DeterminePassStatus()
        {
            PassStatus = Average >= 40 ? "PASS" : "FAIL";
        }

        public void DisplayMarks()
        {
            if (Marks == null || Marks.Length == 0)
            {
                Console.WriteLine("No marks available.");
                return;
            }

            for (int i = 0; i < Marks.Length; i++)
            {
                Console.WriteLine($"Mark {i + 1}: {Marks[i]}");
            }
            Console.WriteLine($"Average: {Average:F2}");
            Console.WriteLine($"Status: {PassStatus}");
        }
    }
} 