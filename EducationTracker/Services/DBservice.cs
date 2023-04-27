using c971.Models;
using Plugin.LocalNotifications;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace c971.Services
{
    public static class DBservice
    {
        //CurrentTerm, CurrentCourse, CurrentAssessment, and CurrentUser are used as placeholders to capture and pass data from a specific term, course, assessment, or user.
        public static Term CurrentTerm { get; set; }

        public static Course CurrentCourse { get; set; }

        public static Assessment CurrentAssessment { get; set; }

        public static User CurrentUser { get; set; }

        public static SQLiteAsyncConnection _db;

        //Sets up the database and creates tables if the database does not currently exist.
        public static async Task Init()
        {           
            if (_db != null)
            {
                return;
            }

            var databasePath = Path.Combine(FileSystem.AppDataDirectory, "Terms.db");

            _db = new SQLiteAsyncConnection(databasePath);

            //The commented code below is used to drop tables for testing purposes.
            //await _db.DropTableAsync<Term>();
            //await _db.DropTableAsync<Course>();
            //await _db.DropTableAsync<Assessment>();
            //await _db.DropTableAsync<User>();

            await _db.CreateTableAsync<Term>();
            await _db.CreateTableAsync<Course>();
            await _db.CreateTableAsync<Assessment>();
            await _db.CreateTableAsync<User>();        
        }

        //Create report of users remaining terms, courses, and assessments
        public static async Task<Report> CreateReport(User user)
        {
            await Init();

            Report report = new Report();

            report.Title = "Academic Information Report";

            report.Date = DateTime.Now.ToString();

            //must retrieve only terms, courses, and assessments for user currently logged in

            //Get the terms of the current user
            List<Term> userTerms = (List<Term>)await GetTerms(CurrentUser);
            report.Terms = userTerms.Count;

            //Get TermID for each term
            List<int> termIDs = (from Term term in userTerms
                                 select term.TermID).ToList();

            //Use TermIDs to get courses associated with each ID
            List<Course> userCourses = await _db.Table<Course>()
                                                .Where(i => termIDs.Contains(i.TermID))
                                                .ToListAsync();
            report.Courses = userCourses.Count;

            //Get CourseID for each course
            List<int> courseIDs = (from Course course in userCourses
                                   select course.CourseID).ToList();

            //Use CourseIDs to get assessments associated with each ID
            List<Assessment> userAssessments = await _db.Table<Assessment>()
                                                        .Where(i => courseIDs.Contains(i.CourseID))
                                                        .ToListAsync();
            report.Assessments = userAssessments.Count;

            return report;
        }

        //Shows notifications of course or assessment starting/ending today if any exist.
        public static async Task NotificationCheck(bool boolean)
        {
            if (boolean == true)
            {
                var courseList = await DBservice._db.Table<Course>().ToListAsync();
                var assessmentList = await _db.Table<Assessment>().ToListAsync();
               
                foreach (Course course in courseList)
                {
                    if (course.Notification && course.CourseStartDate == DateTime.Today)
                    { 
                        CrossLocalNotifications.Current.Show("Course Starting", "Course: " + course.CourseName + " starts today: " + DateTime.Now.Date.ToShortDateString() + "", 1, DateTime.Now); 
                    }

                    if (course.Notification && course.CourseEndDate == DateTime.Today) 
                    { 
                        CrossLocalNotifications.Current.Show("Course Ending", "Course: " + course.CourseName + " ends today: " + DateTime.Now.Date.ToShortDateString() + "", 2, DateTime.Now);
                    }                   
                }

                foreach (Assessment assessment in assessmentList)
                {
                   if (assessment.AssessmentNotification && assessment.AssessmentStartDate == DateTime.Today) 
                    { 
                        CrossLocalNotifications.Current.Show("Assessment Starting", "Assessment: " + assessment.AssessmentName + " starts today: " + DateTime.Now.Date.ToShortDateString() + "", 
                            3, DateTime.Now); 
                    }

                   if (assessment.AssessmentNotification && assessment.AssessmentEndDate == DateTime.Today)
                    { 
                        CrossLocalNotifications.Current.Show("Assessment Due", "Assessment: " + assessment.AssessmentName + " is due today: " + DateTime.Now.Date.ToShortDateString() + "",
                            4, DateTime.Now); 
                    }
                }                
            }
        }

        //Gets user account associated with username and password
        public static async Task<User> GetUser(string username, string password)
        {
        await Init();

        User user = await _db.Table<User>()
                             .Where(i => i.UserName == username && i.UserPass == password)
                             .FirstOrDefaultAsync();

        return user;
        }

        
        //Adds a new user to the User table.
        public static async Task AddUser(string username, string password)
        {
            await Init();

            User user = new User
            {
                UserName = username,
                UserPass = password
            };
            await _db.InsertAsync(user);
        }

        //Adds a new term to the Term table.
        public static async Task AddTerm(string termName, DateTime termStartDate, DateTime termEndDate, int userID)
        {      
            await Init();

            Term term = new Term
            {
                TermName = termName,
                TermStartDate = termStartDate,
                TermEndDate = termEndDate,
                UserID = userID
            };

            await _db.InsertAsync(term);
        }

        //Removes selected term from the Term table.
        public static async Task RemoveTerm(int termID)
        {
            await Init();

            await _db.DeleteAsync<Term>(termID);
        }

        //Gets all terms in the Term table and puts them in a list.
        public static async Task<IEnumerable<Term>> GetTerms(User user)
        {
            await Init();

            List<Term> terms = await _db.Table<Term>()
                                        .Where(i => i.UserID == user.UserID)
                                        .ToListAsync();
            return terms;
        }

        //Updates selected term in the Term table.
        public static async Task UpdateTerm(int termID, string termName, DateTime termStartDate, DateTime termEndDate, int userID)
        {
            await Init();

            Term term = await _db.Table<Term>()
                                 .Where(i => i.TermID == termID)
                                 .FirstOrDefaultAsync();           
                               
            if (term != null)
            {
                term.TermName = termName;
                term.TermStartDate = termStartDate;
                term.TermEndDate = termEndDate;
                term.UserID = userID;

                await _db.UpdateAsync(term);
            }
        }

        //Adds a new course to the Course table.
        public static async Task AddCourse(string courseName, DateTime courseStartDate, DateTime courseEndDate, 
            string courseStatus, string instructorName, string instructorPhone, string instructorEmail, string notes, int termID, bool notification)
        {
            await Init();

            Course course = new Course
            {
                CourseName = courseName,
                CourseStartDate = courseStartDate,
                CourseEndDate = courseEndDate,
                CourseStatus = courseStatus,
                InstructorName = instructorName,
                InstructorPhone = instructorPhone,
                InstructorEmail = instructorEmail,
                Notes = notes,
                TermID = termID,
                Notification = notification
            };
            await _db.InsertAsync(course);
        }

        //Gets the courses in the Course table and puts them in a list.
        public static async Task<IEnumerable<Course>> GetCourses(Term term)
        {
            await Init();          
            
            List<Course> courses = await _db.Table<Course>()
                                            .Where(i => i.TermID == term.TermID)
                                            .ToListAsync();
            return courses;
        }


        //Updates the selected course in the Course table.
        public static async Task UpdateCourse(int courseID, string courseName, DateTime courseStartDate, DateTime courseEndDate, string courseStatus, string instructorName,
                                                string instructorPhone, string instructorEmail, string notes, bool notification)
        {
            await Init();

            Course course = await _db.Table<Course>()
                                     .Where(i => i.CourseID == courseID)
                                     .FirstOrDefaultAsync();
          
            if (course != null)
            {
                course.CourseName = courseName;
                course.CourseStartDate = courseStartDate;
                course.CourseEndDate = courseEndDate;
                course.CourseStatus = courseStatus;
                course.InstructorName = instructorName;
                course.InstructorPhone = instructorPhone;
                course.InstructorEmail = instructorEmail;
                course.Notes = notes;
                course.Notification = notification;

                await _db.UpdateAsync(course);
            }
        }

        //Removes the selected course in the Course table.
        public static async Task RemoveCourse(int courseID)
        {
            await Init();

            await _db.DeleteAsync<Course>(courseID);
        }

        //Gets the assessments in the Assessment table and puts them in a list.
        public static async Task<IEnumerable<Assessment>> GetAssessments(Course course)
        {
            await Init();
            
            List<Assessment> assessments = await _db.Table<Assessment>()
                                                    .Where(i => i.CourseID == course.CourseID)
                                                    .ToListAsync();
            return assessments;
        }
      
        //Adds an assessment to the Assessment table.
        public static async Task AddAssessment(string assessmentName, DateTime assessmentStartDate, DateTime assessmentEndDate,
            string assessmentType, int courseID, bool assessmentNotification)
        {
            await Init();
            Assessment assessment = new Assessment
            {
                AssessmentName = assessmentName,
                AssessmentStartDate = assessmentStartDate,
                AssessmentEndDate = assessmentEndDate,
                AssessmentType = assessmentType,
                CourseID = courseID,
                AssessmentNotification = assessmentNotification
            };
            await _db.InsertAsync(assessment);
        }

        //Updates selected assessment in the Assessment table.
        public static async Task UpdateAssessment(int assessmentID, string assessmentName, DateTime assessmentStartDate, DateTime assessmentEndDate, string assessmentType, bool assessmentNotification)
        {
            await Init();

            Assessment assessment = await _db.Table<Assessment>()
                                             .Where(i => i.AssessmentID == assessmentID)
                                             .FirstOrDefaultAsync();

            if (assessment != null)
            {
                assessment.AssessmentName = assessmentName;
                assessment.AssessmentStartDate = assessmentStartDate;
                assessment.AssessmentEndDate = assessmentEndDate;
                assessment.AssessmentType = assessmentType;
                assessment.AssessmentNotification = assessmentNotification;

                await _db.UpdateAsync(assessment);
            }
        }

        //Removes selected assessment from the Assessment table.
        public static async Task RemoveAssessment(int assessmentID)
        {
            await Init();

            await _db.DeleteAsync<Assessment>(assessmentID);
        }
    }
}
