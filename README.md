Student Grading System

A comprehensive console-based application for managing student records and grades in a Computer Science program, developed as part of CET137 Assessment 1.

Table of Contents
1. Project Overview
2. System Architecture
3. Features
4. Technical Implementation
5. Class Structure
6. Data Management
7. How to Run
8. Usage Guide
9. Assessment Requirements

Project Overview

The Student Grading System is designed to manage academic records for Computer Science students, handling multiple subjects, grade calculations, and academic history tracking. The system implements robust data validation, persistent storage, and comprehensive reporting features.

System Architecture

The system follows a modular, object-oriented design with the following key components:

1. Core Classes
   - StudentRecord: Student information and mark history management
   - Subject: Computer Science subject definition and rules
   - MarksEntry: Individual assessment attempt records
   - MarkSet: Mark calculation and status determination

2. Utility Components
   - Data Validation
   - File I/O Operations
   - User Interface Management
   - Error Handling

Features

Core Functionality
- Create and manage student records with unique 8-digit identifiers
- Record and validate marks for 6 Computer Science subjects
- Track multiple assessment attempts with timestamps
- Calculate averages and determine pass/fail status
- Generate detailed academic reports

Data Management
- Persistent storage using structured text files
- Automatic backup and recovery
- Data validation and integrity checks
- History tracking and versioning

User Interface
- Clean and intuitive console interface
- Clear error messages and user guidance
- Input validation and error prevention
- Efficient navigation and operation

Technical Implementation

Technology Stack
- Language: C# (.NET Core)
- Storage: Text-based file system
- Architecture: Object-Oriented Design
- Interface: Console Application

Key Classes

1. Program.cs
   - Application entry point
   - Menu system and user interface
   - Program flow control
   - Input/output management

2. StudentRecord.cs
   - Student data management
   - Mark history tracking
   - Record display formatting
   - Data validation

3. Subject.cs
   - Subject definitions
   - Credit hour management
   - Passing criteria
   - Grade calculations

4. MarkSet.cs
   - Mark validation
   - Average calculations
   - Pass/fail determination
   - Result formatting

5. GradingSystem.cs
   - System operations coordination
   - File operations management
   - Data persistence
   - Error handling

Data Management

Storage Structure
- Student records: student_records.txt
- Subject definitions: subjects.txt
- Backup data: backup/

Data Format
{
  "studentNumber": "12345678",
  "studentName": "John Doe",
  "marksHistory": [
    {
      "timestamp": "2024-03-15T10:30:00",
      "marks": [85, 92, 78, 88, 90, 87],
      "average": 86.67,
      "status": "PASS"
    }
  ]
}

How to Run

Prerequisites
- .NET SDK 6.0 or higher
- Windows/Linux/macOS operating system
- Terminal/Command Prompt

Installation
1. Clone the repository
2. Navigate to the project directory
3. Run the following commands:
   dotnet restore
   dotnet build
   dotnet run

Usage Guide

Main Menu Options
1. Create New Student Record
2. Enter Student Marks
3. View Student Record
4. Update Student Marks
5. Display All Records
6. Exit

Data Entry Guidelines
- Student Number: 8 digits
- Marks: 0-100 range
- Subject Selection: Choose from available subjects
- Updates: Previous records preserved

Assessment Requirements

Section 1: UML Class Diagram (10%)
The system implements a comprehensive object-oriented design with:
- Four main classes: StudentRecord, MarksEntry, Subject, and GradingSystem
- Clear relationships between classes (composition, association, dependency)
- Well-defined data types and visibility modifiers
- Complete method signatures with parameters and return types

Key Class Relationships:
- StudentRecord manages multiple MarksEntry instances
- MarksEntry references multiple Subject instances
- GradingSystem coordinates overall system operations
- Subject defines the core academic structure

Section 2: Implementation
- Object-oriented principles
- Data validation and error handling
- File operations and data persistence
- User interface and interaction
- Code organization and documentation

Section 3: Testing and Documentation
- Comprehensive testing
- Error handling verification
- User guide and documentation
- Code comments and explanations

Contributing

This project is part of an academic assessment and is not open for contributions.

License

This project is created for educational purposes as part of CET137 Assessment 1. 