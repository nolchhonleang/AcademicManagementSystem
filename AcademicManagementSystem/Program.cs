using System;
using System.Data;
using System.Data.SQLite;

namespace AcademicManagementSystem
{
    class Program
    {
        private static string connectionString = "Data Source=academicDB.db;Version=3;";

        static void Main(string[] args)
        {
            // Create database and tables if they don't exist.
            if (!System.IO.File.Exists("academicDB.db"))
            {
                SQLiteConnection.CreateFile("academicDB.db");
                CreateTables();
            }

            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== Academic Management System ===");
                Console.WriteLine("1. Manage Students");
                Console.WriteLine("2. Manage Teachers");
                Console.WriteLine("3. Manage Courses");
                Console.WriteLine("4. Manage Enrollments");
                Console.WriteLine("5. Exit");
                Console.Write("Select an option: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        ManageStudents();
                        break;
                    case "2":
                        ManageTeachers();
                        break;
                    case "3":
                        ManageCourses();
                        break;
                    case "4":
                        ManageEnrollments();
                        break;
                    case "5":
                        return;
                    default:
                        Console.WriteLine("Invalid choice! Press any key to try again.");
                        Console.ReadKey();
                        break;
                }
            }
        }

        #region Database and Table Creation
        private static void CreateTables()
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                // Create Students table
                string createStudents = @"CREATE TABLE IF NOT EXISTS Students (
                                            ID INTEGER PRIMARY KEY AUTOINCREMENT, 
                                            Name TEXT NOT NULL, 
                                            Age INTEGER, 
                                            StudentID TEXT UNIQUE, 
                                            Major TEXT
                                          );";
                // Create Teachers table
                string createTeachers = @"CREATE TABLE IF NOT EXISTS Teachers (
                                            ID INTEGER PRIMARY KEY AUTOINCREMENT,
                                            Name TEXT NOT NULL,
                                            Department TEXT,
                                            Email TEXT UNIQUE
                                          );";
                // Create Courses table
                string createCourses = @"CREATE TABLE IF NOT EXISTS Courses (
                                            ID INTEGER PRIMARY KEY AUTOINCREMENT,
                                            Title TEXT NOT NULL,
                                            Credits INTEGER,
                                            TeacherID INTEGER,
                                            FOREIGN KEY (TeacherID) REFERENCES Teachers(ID)
                                          );";
                // Create Enrollments table
                string createEnrollments = @"CREATE TABLE IF NOT EXISTS Enrollments (
                                                ID INTEGER PRIMARY KEY AUTOINCREMENT,
                                                StudentID INTEGER,
                                                CourseID INTEGER,
                                                EnrollmentDate TEXT,
                                                Grade TEXT,
                                                FOREIGN KEY (StudentID) REFERENCES Students(ID),
                                                FOREIGN KEY (CourseID) REFERENCES Courses(ID)
                                             );";

                using (var cmd = new SQLiteCommand(connection))
                {
                    cmd.CommandText = createStudents;
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = createTeachers;
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = createCourses;
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = createEnrollments;
                    cmd.ExecuteNonQuery();
                }
            }
        }
        #endregion

        #region Student Management
        private static void ManageStudents()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== Student Management ===");
                Console.WriteLine("1. Add Student");
                Console.WriteLine("2. Update Student");
                Console.WriteLine("3. Delete Student");
                Console.WriteLine("4. View All Students");
                Console.WriteLine("5. Search Student by Name");
                Console.WriteLine("6. Back to Main Menu");
                Console.Write("Select an option: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        AddStudent();
                        break;
                    case "2":
                        UpdateStudent();
                        break;
                    case "3":
                        DeleteStudent();
                        break;
                    case "4":
                        ViewAllStudents();
                        break;
                    case "5":
                        SearchStudent();
                        break;
                    case "6":
                        return;
                    default:
                        Console.WriteLine("Invalid option! Press any key to try again.");
                        Console.ReadKey();
                        break;
                }
            }
        }

        private static void AddStudent()
        {
            Console.Clear();
            Console.WriteLine("Enter Student Details:");
            Console.Write("Name: ");
            string name = Console.ReadLine();
            Console.Write("Age: ");
            int age = int.Parse(Console.ReadLine());
            Console.Write("Student ID (unique): ");
            string studentID = Console.ReadLine();
            Console.Write("Major: ");
            string major = Console.ReadLine();

            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string query = "INSERT INTO Students (Name, Age, StudentID, Major) VALUES (@Name, @Age, @StudentID, @Major)";
                using (var cmd = new SQLiteCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Name", name);
                    cmd.Parameters.AddWithValue("@Age", age);
                    cmd.Parameters.AddWithValue("@StudentID", studentID);
                    cmd.Parameters.AddWithValue("@Major", major);
                    cmd.ExecuteNonQuery();
                }
            }
            Console.WriteLine("Student added successfully! Press any key to continue.");
            Console.ReadKey();
        }

        private static void UpdateStudent()
        {
            Console.Clear();
            Console.Write("Enter the ID of the student to update: ");
            int id = int.Parse(Console.ReadLine());
            Console.Write("New Name: ");
            string name = Console.ReadLine();
            Console.Write("New Age: ");
            int age = int.Parse(Console.ReadLine());
            Console.Write("New Student ID: ");
            string studentID = Console.ReadLine();
            Console.Write("New Major: ");
            string major = Console.ReadLine();

            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string query = "UPDATE Students SET Name = @Name, Age = @Age, StudentID = @StudentID, Major = @Major WHERE ID = @ID";
                using (var cmd = new SQLiteCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@ID", id);
                    cmd.Parameters.AddWithValue("@Name", name);
                    cmd.Parameters.AddWithValue("@Age", age);
                    cmd.Parameters.AddWithValue("@StudentID", studentID);
                    cmd.Parameters.AddWithValue("@Major", major);
                    cmd.ExecuteNonQuery();
                }
            }
            Console.WriteLine("Student updated successfully! Press any key to continue.");
            Console.ReadKey();
        }

        private static void DeleteStudent()
        {
            Console.Clear();
            Console.Write("Enter the ID of the student to delete: ");
            int id = int.Parse(Console.ReadLine());

            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string query = "DELETE FROM Students WHERE ID = @ID";
                using (var cmd = new SQLiteCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@ID", id);
                    cmd.ExecuteNonQuery();
                }
            }
            Console.WriteLine("Student deleted successfully! Press any key to continue.");
            Console.ReadKey();
        }

        private static void ViewAllStudents()
        {
            Console.Clear();
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT * FROM Students";
                using (var cmd = new SQLiteCommand(query, connection))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        Console.WriteLine("=== All Students ===");
                        while (reader.Read())
                        {
                            Console.WriteLine($"ID: {reader["ID"]} | Name: {reader["Name"]} | Age: {reader["Age"]} | StudentID: {reader["StudentID"]} | Major: {reader["Major"]}");
                        }
                    }
                }
            }
            Console.WriteLine("Press any key to return.");
            Console.ReadKey();
        }

        private static void SearchStudent()
        {
            Console.Clear();
            Console.Write("Enter name to search: ");
            string searchTerm = Console.ReadLine();

            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT * FROM Students WHERE Name LIKE @SearchTerm";
                using (var cmd = new SQLiteCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@SearchTerm", "%" + searchTerm + "%");
                    using (var reader = cmd.ExecuteReader())
                    {
                        Console.WriteLine("=== Search Results ===");
                        while (reader.Read())
                        {
                            Console.WriteLine($"ID: {reader["ID"]} | Name: {reader["Name"]} | Age: {reader["Age"]} | StudentID: {reader["StudentID"]} | Major: {reader["Major"]}");
                        }
                    }
                }
            }
            Console.WriteLine("Press any key to return.");
            Console.ReadKey();
        }
        #endregion

        #region Teacher Management
        private static void ManageTeachers()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== Teacher Management ===");
                Console.WriteLine("1. Add Teacher");
                Console.WriteLine("2. Update Teacher");
                Console.WriteLine("3. Delete Teacher");
                Console.WriteLine("4. View All Teachers");
                Console.WriteLine("5. Search Teacher by Name");
                Console.WriteLine("6. Back to Main Menu");
                Console.Write("Select an option: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        AddTeacher();
                        break;
                    case "2":
                        UpdateTeacher();
                        break;
                    case "3":
                        DeleteTeacher();
                        break;
                    case "4":
                        ViewAllTeachers();
                        break;
                    case "5":
                        SearchTeacher();
                        break;
                    case "6":
                        return;
                    default:
                        Console.WriteLine("Invalid option! Press any key to try again.");
                        Console.ReadKey();
                        break;
                }
            }
        }

        private static void AddTeacher()
        {
            Console.Clear();
            Console.WriteLine("Enter Teacher Details:");
            Console.Write("Name: ");
            string name = Console.ReadLine();
            Console.Write("Department: ");
            string department = Console.ReadLine();
            Console.Write("Email (unique): ");
            string email = Console.ReadLine();

            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string query = "INSERT INTO Teachers (Name, Department, Email) VALUES (@Name, @Department, @Email)";
                using (var cmd = new SQLiteCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Name", name);
                    cmd.Parameters.AddWithValue("@Department", department);
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.ExecuteNonQuery();
                }
            }
            Console.WriteLine("Teacher added successfully! Press any key to continue.");
            Console.ReadKey();
        }

        private static void UpdateTeacher()
        {
            Console.Clear();
            Console.Write("Enter the ID of the teacher to update: ");
            int id = int.Parse(Console.ReadLine());
            Console.Write("New Name: ");
            string name = Console.ReadLine();
            Console.Write("New Department: ");
            string department = Console.ReadLine();
            Console.Write("New Email: ");
            string email = Console.ReadLine();

            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string query = "UPDATE Teachers SET Name = @Name, Department = @Department, Email = @Email WHERE ID = @ID";
                using (var cmd = new SQLiteCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@ID", id);
                    cmd.Parameters.AddWithValue("@Name", name);
                    cmd.Parameters.AddWithValue("@Department", department);
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.ExecuteNonQuery();
                }
            }
            Console.WriteLine("Teacher updated successfully! Press any key to continue.");
            Console.ReadKey();
        }

        private static void DeleteTeacher()
        {
            Console.Clear();
            Console.Write("Enter the ID of the teacher to delete: ");
            int id = int.Parse(Console.ReadLine());

            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string query = "DELETE FROM Teachers WHERE ID = @ID";
                using (var cmd = new SQLiteCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@ID", id);
                    cmd.ExecuteNonQuery();
                }
            }
            Console.WriteLine("Teacher deleted successfully! Press any key to continue.");
            Console.ReadKey();
        }

        private static void ViewAllTeachers()
        {
            Console.Clear();
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT * FROM Teachers";
                using (var cmd = new SQLiteCommand(query, connection))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        Console.WriteLine("=== All Teachers ===");
                        while (reader.Read())
                        {
                            Console.WriteLine($"ID: {reader["ID"]} | Name: {reader["Name"]} | Department: {reader["Department"]} | Email: {reader["Email"]}");
                        }
                    }
                }
            }
            Console.WriteLine("Press any key to return.");
            Console.ReadKey();
        }

        private static void SearchTeacher()
        {
            Console.Clear();
            Console.Write("Enter name to search: ");
            string searchTerm = Console.ReadLine();

            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT * FROM Teachers WHERE Name LIKE @SearchTerm";
                using (var cmd = new SQLiteCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@SearchTerm", "%" + searchTerm + "%");
                    using (var reader = cmd.ExecuteReader())
                    {
                        Console.WriteLine("=== Search Results ===");
                        while (reader.Read())
                        {
                            Console.WriteLine($"ID: {reader["ID"]} | Name: {reader["Name"]} | Department: {reader["Department"]} | Email: {reader["Email"]}");
                        }
                    }
                }
            }
            Console.WriteLine("Press any key to return.");
            Console.ReadKey();
        }
        #endregion

        #region Course Management
        private static void ManageCourses()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== Course Management ===");
                Console.WriteLine("1. Add Course");
                Console.WriteLine("2. Update Course");
                Console.WriteLine("3. Delete Course");
                Console.WriteLine("4. View All Courses");
                Console.WriteLine("5. Search Course by Title");
                Console.WriteLine("6. Back to Main Menu");
                Console.Write("Select an option: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        AddCourse();
                        break;
                    case "2":
                        UpdateCourse();
                        break;
                    case "3":
                        DeleteCourse();
                        break;
                    case "4":
                        ViewAllCourses();
                        break;
                    case "5":
                        SearchCourse();
                        break;
                    case "6":
                        return;
                    default:
                        Console.WriteLine("Invalid option! Press any key to try again.");
                        Console.ReadKey();
                        break;
                }
            }
        }

        private static void AddCourse()
        {
            Console.Clear();
            Console.WriteLine("Enter Course Details:");
            Console.Write("Title: ");
            string title = Console.ReadLine();
            Console.Write("Credits: ");
            int credits = int.Parse(Console.ReadLine());
            Console.Write("Teacher ID (if assigned, else 0): ");
            int teacherId = int.Parse(Console.ReadLine());

            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string query = "INSERT INTO Courses (Title, Credits, TeacherID) VALUES (@Title, @Credits, @TeacherID)";
                using (var cmd = new SQLiteCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Title", title);
                    cmd.Parameters.AddWithValue("@Credits", credits);
                    cmd.Parameters.AddWithValue("@TeacherID", teacherId == 0 ? DBNull.Value : (object)teacherId);
                    cmd.ExecuteNonQuery();
                }
            }
            Console.WriteLine("Course added successfully! Press any key to continue.");
            Console.ReadKey();
        }

        private static void UpdateCourse()
        {
            Console.Clear();
            Console.Write("Enter the ID of the course to update: ");
            int id = int.Parse(Console.ReadLine());
            Console.Write("New Title: ");
            string title = Console.ReadLine();
            Console.Write("New Credits: ");
            int credits = int.Parse(Console.ReadLine());
            Console.Write("New Teacher ID (if assigned, else 0): ");
            int teacherId = int.Parse(Console.ReadLine());

            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string query = "UPDATE Courses SET Title = @Title, Credits = @Credits, TeacherID = @TeacherID WHERE ID = @ID";
                using (var cmd = new SQLiteCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@ID", id);
                    cmd.Parameters.AddWithValue("@Title", title);
                    cmd.Parameters.AddWithValue("@Credits", credits);
                    cmd.Parameters.AddWithValue("@TeacherID", teacherId == 0 ? DBNull.Value : (object)teacherId);
                    cmd.ExecuteNonQuery();
                }
            }
            Console.WriteLine("Course updated successfully! Press any key to continue.");
            Console.ReadKey();
        }

        private static void DeleteCourse()
        {
            Console.Clear();
            Console.Write("Enter the ID of the course to delete: ");
            int id = int.Parse(Console.ReadLine());

            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string query = "DELETE FROM Courses WHERE ID = @ID";
                using (var cmd = new SQLiteCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@ID", id);
                    cmd.ExecuteNonQuery();
                }
            }
            Console.WriteLine("Course deleted successfully! Press any key to continue.");
            Console.ReadKey();
        }

        private static void ViewAllCourses()
        {
            Console.Clear();
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT C.ID, C.Title, C.Credits, T.Name AS Teacher FROM Courses C LEFT JOIN Teachers T ON C.TeacherID = T.ID";
                using (var cmd = new SQLiteCommand(query, connection))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        Console.WriteLine("=== All Courses ===");
                        while (reader.Read())
                        {
                            Console.WriteLine($"ID: {reader["ID"]} | Title: {reader["Title"]} | Credits: {reader["Credits"]} | Teacher: {reader["Teacher"]}");
                        }
                    }
                }
            }
            Console.WriteLine("Press any key to return.");
            Console.ReadKey();
        }

        private static void SearchCourse()
        {
            Console.Clear();
            Console.Write("Enter course title to search: ");
            string searchTerm = Console.ReadLine();

            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT * FROM Courses WHERE Title LIKE @SearchTerm";
                using (var cmd = new SQLiteCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@SearchTerm", "%" + searchTerm + "%");
                    using (var reader = cmd.ExecuteReader())
                    {
                        Console.WriteLine("=== Search Results ===");
                        while (reader.Read())
                        {
                            Console.WriteLine($"ID: {reader["ID"]} | Title: {reader["Title"]} | Credits: {reader["Credits"]}");
                        }
                    }
                }
            }
            Console.WriteLine("Press any key to return.");
            Console.ReadKey();
        }
        #endregion

        #region Enrollment Management
        private static void ManageEnrollments()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== Enrollment Management ===");
                Console.WriteLine("1. Enroll Student in a Course");
                Console.WriteLine("2. Update Enrollment (Grade)");
                Console.WriteLine("3. Delete Enrollment");
                Console.WriteLine("4. View All Enrollments");
                Console.WriteLine("5. Back to Main Menu");
                Console.Write("Select an option: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        EnrollStudent();
                        break;
                    case "2":
                        UpdateEnrollment();
                        break;
                    case "3":
                        DeleteEnrollment();
                        break;
                    case "4":
                        ViewAllEnrollments();
                        break;
                    case "5":
                        return;
                    default:
                        Console.WriteLine("Invalid option! Press any key to try again.");
                        Console.ReadKey();
                        break;
                }
            }
        }

        private static void EnrollStudent()
        {
            Console.Clear();
            Console.Write("Enter Student ID: ");
            int studentId = int.Parse(Console.ReadLine());
            Console.Write("Enter Course ID: ");
            int courseId = int.Parse(Console.ReadLine());
            string enrollmentDate = DateTime.Now.ToString("yyyy-MM-dd");

            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string query = "INSERT INTO Enrollments (StudentID, CourseID, EnrollmentDate) VALUES (@StudentID, @CourseID, @EnrollmentDate)";
                using (var cmd = new SQLiteCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@StudentID", studentId);
                    cmd.Parameters.AddWithValue("@CourseID", courseId);
                    cmd.Parameters.AddWithValue("@EnrollmentDate", enrollmentDate);
                    cmd.ExecuteNonQuery();
                }
            }
            Console.WriteLine("Enrollment successful! Press any key to continue.");
            Console.ReadKey();
        }

        private static void UpdateEnrollment()
        {
            Console.Clear();
            Console.Write("Enter Enrollment ID to update grade: ");
            int id = int.Parse(Console.ReadLine());
            Console.Write("Enter new Grade (e.g., A, B, etc.): ");
            string grade = Console.ReadLine();

            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string query = "UPDATE Enrollments SET Grade = @Grade WHERE ID = @ID";
                using (var cmd = new SQLiteCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Grade", grade);
                    cmd.Parameters.AddWithValue("@ID", id);
                    cmd.ExecuteNonQuery();
                }
            }
            Console.WriteLine("Enrollment updated successfully! Press any key to continue.");
            Console.ReadKey();
        }

        private static void DeleteEnrollment()
        {
            Console.Clear();
            Console.Write("Enter Enrollment ID to delete: ");
            int id = int.Parse(Console.ReadLine());

            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string query = "DELETE FROM Enrollments WHERE ID = @ID";
                using (var cmd = new SQLiteCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@ID", id);
                    cmd.ExecuteNonQuery();
                }
            }
            Console.WriteLine("Enrollment deleted successfully! Press any key to continue.");
            Console.ReadKey();
        }

        private static void ViewAllEnrollments()
        {
            Console.Clear();
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string query = @"SELECT E.ID, S.Name AS Student, C.Title AS Course, E.EnrollmentDate, E.Grade 
                                 FROM Enrollments E 
                                 LEFT JOIN Students S ON E.StudentID = S.ID 
                                 LEFT JOIN Courses C ON E.CourseID = C.ID";
                using (var cmd = new SQLiteCommand(query, connection))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        Console.WriteLine("=== All Enrollments ===");
                        while (reader.Read())
                        {
                            Console.WriteLine($"Enrollment ID: {reader["ID"]} | Student: {reader["Student"]} | Course: {reader["Course"]} | Date: {reader["EnrollmentDate"]} | Grade: {reader["Grade"]}");
                        }
                    }
                }
            }
            Console.WriteLine("Press any key to return.");
            Console.ReadKey();
        }
        #endregion
    }
}