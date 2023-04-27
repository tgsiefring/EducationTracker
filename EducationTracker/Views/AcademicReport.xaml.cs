using c971.Models;
using c971.Services;
using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace c971.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AcademicReport : ContentPage
    {
        public AcademicReport(User selecteduser)
        {
            InitializeComponent();
            DBservice.CurrentUser = selecteduser;
        }

        async void createReport_Clicked(object sender, EventArgs e)
        {
            var report = await DBservice.CreateReport(DBservice.CurrentUser);

            invisTerm.IsVisible = true;
            invisCourse.IsVisible = true;
            invisAssessment.IsVisible = true;

            title.Text = report.Title;
            date.Text = $"For your schedule as of {report.Date} you have: ";
            terms.Text = report.Terms.ToString();
            courses.Text = report.Courses.ToString();
            assessments.Text = report.Assessments.ToString();
        }
    }
}