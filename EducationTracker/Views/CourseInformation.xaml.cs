using c971.Models;
using c971.Services;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;

namespace c971.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CourseInformation : ContentPage
    {
        public CourseInformation(Course selectedCourse)
        {
            InitializeComponent();

            DBservice.CurrentCourse = selectedCourse;

            CourseIDEntry.Text = selectedCourse.CourseID.ToString();
            CourseNameEntry.Text = selectedCourse.CourseName;
            startDatePicker.Date = selectedCourse.CourseStartDate;
            endDatePicker.Date = selectedCourse.CourseEndDate;
            CourseStatusPicker.SelectedItem = selectedCourse.CourseStatus;
            InstructorNameEntry.Text = selectedCourse.InstructorName;
            InstructorPhoneEntry.Text = selectedCourse.InstructorPhone;
            InstructorEmailEntry.Text = selectedCourse.InstructorEmail;
            CourseNotesEntry.Text = selectedCourse.Notes;
            TermIDLink.Text = selectedCourse.TermID.ToString();
            courseSwitch2.IsToggled = selectedCourse.Notification;
        }

        async void UpdateCourse_Clicked(object sender, EventArgs e)
        {
            if (!String.IsNullOrWhiteSpace(CourseNameEntry.Text) && !String.IsNullOrWhiteSpace(CourseStatusPicker.ToString()) && !String.IsNullOrWhiteSpace(InstructorNameEntry.Text)
            && !String.IsNullOrWhiteSpace(InstructorPhoneEntry.Text) && !String.IsNullOrWhiteSpace(InstructorEmailEntry.Text) && !String.IsNullOrWhiteSpace(CourseNotesEntry.Text) 
            && InstructorEmailEntry.Text.Contains("@"))
            {
                if (startDatePicker.Date <= endDatePicker.Date)
                {
                    await DBservice.UpdateCourse(Int32.Parse(CourseIDEntry.Text), CourseNameEntry.Text, startDatePicker.Date, endDatePicker.Date, CourseStatusPicker.SelectedItem.ToString(), 
                        InstructorNameEntry.Text,InstructorPhoneEntry.Text, InstructorEmailEntry.Text, CourseNotesEntry.Text, courseSwitch2.IsToggled);
                    await Navigation.PopAsync();
                }
                else
                {
                    await DisplayAlert("Error",
                                       "Start date must be on or before the end date",
                                       "OK");
                }
            }
            else
            {
                await DisplayAlert("Error",
                                   "Please ensure valid data is entered",
                                   "OK");
            }
        }

        async void DeleteCourse_Clicked(object sender, EventArgs e)
        {
            var assessmentQuery = await DBservice.GetAssessments(DBservice.CurrentCourse);
            foreach (Assessment assessment in assessmentQuery)
            {
                await DBservice.RemoveAssessment(assessment.AssessmentID);
            }

            var id = int.Parse(CourseIDEntry.Text);
            await DBservice.RemoveCourse(id);
            await Navigation.PopAsync();
        }

        async void ViewAssessments_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Assessments(DBservice.CurrentCourse));
        }

        async void ShareButton_Clicked(object sender, EventArgs e)
        {
            await Share.RequestAsync(new ShareTextRequest
            {
                Text = CourseNotesEntry.Text,
                Title = "Share"
            });
        }
    }
}