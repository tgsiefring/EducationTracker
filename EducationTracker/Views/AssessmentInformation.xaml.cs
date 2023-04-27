using c971.Models;
using c971.Services;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace c971.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AssessmentInformation : ContentPage
    {
        public AssessmentInformation(Assessment selectedAssessment)
        {
            InitializeComponent();
            DBservice.CurrentAssessment = selectedAssessment;

            AssessmentIDEntry.Text = selectedAssessment.AssessmentID.ToString();
            AssessmentNameEntry.Text = selectedAssessment.AssessmentName;
            AssessmentStartDatePicker.Date = selectedAssessment.AssessmentStartDate;
            AssessmentEndDatePicker.Date = selectedAssessment.AssessmentEndDate;
            AssessmentTypePicker.SelectedItem = selectedAssessment.AssessmentType;
            CourseIDLink.Text = selectedAssessment.CourseID.ToString();
            assessmentSwitch.IsToggled = selectedAssessment.AssessmentNotification;
        }

        async void updateAssessment_Clicked(object sender, EventArgs e)
        {
            if (!String.IsNullOrWhiteSpace(AssessmentNameEntry.Text))
            {
                if (AssessmentStartDatePicker.Date <= AssessmentEndDatePicker.Date)
                {
                    await DBservice.UpdateAssessment(Int32.Parse(AssessmentIDEntry.Text), AssessmentNameEntry.Text, AssessmentStartDatePicker.Date, AssessmentEndDatePicker.Date,
                                                     AssessmentTypePicker.SelectedItem.ToString(), assessmentSwitch.IsToggled);
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

        async void deleteAssessment_Clicked(object sender, EventArgs e)
        {
            var id = int.Parse(AssessmentIDEntry.Text);
            await DBservice.RemoveAssessment(id);
            await Navigation.PopAsync();
        }
    }
}