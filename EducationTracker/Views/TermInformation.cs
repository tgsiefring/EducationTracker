using c971.Models;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using c971.Views;
using c971.Services;

namespace c971
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TermInformation : ContentPage
    {
        public TermInformation(Term selectedTerm)
        {
            InitializeComponent();

            DBservice.CurrentTerm = selectedTerm;

            TermIDEntry.Text = selectedTerm.TermID.ToString();
            TermNameEntry.Text = selectedTerm.TermName;
            startDatePicker.Date = selectedTerm.TermStartDate;
            endDatePicker.Date = selectedTerm.TermEndDate;
            UserIDEntry.Text = selectedTerm.UserID.ToString();
        }

        async void Update_Clicked(object sender, EventArgs e)
        { 
            if(!String.IsNullOrWhiteSpace(TermIDEntry.Text) && !String.IsNullOrWhiteSpace(TermNameEntry.Text))
            {
                if (startDatePicker.Date <= endDatePicker.Date)
                {
                    await DBservice.UpdateTerm(Int32.Parse(TermIDEntry.Text), TermNameEntry.Text, startDatePicker.Date, endDatePicker.Date, Int32.Parse(UserIDEntry.Text));
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

        async void Delete_Clicked(object sender, EventArgs e)
        {
            var courseQuery = await DBservice.GetCourses(DBservice.CurrentTerm);
            foreach(Course course in courseQuery)
            {
                var assessmentQuery = await DBservice.GetAssessments(course);
                    foreach(Assessment assessment in assessmentQuery)
                {
                    await DBservice.RemoveAssessment(assessment.AssessmentID);
                }
                await DBservice.RemoveCourse(course.CourseID);
            }

            var id = int.Parse(TermIDEntry.Text);
            await DBservice.RemoveTerm(id);
            await Navigation.PopAsync();
        }

        async void ViewCourses_Clicked(object sender, EventArgs e)
        {
           
            await Navigation.PushAsync(new Courses(DBservice.CurrentTerm));
        }
    }
}