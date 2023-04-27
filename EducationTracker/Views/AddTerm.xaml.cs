using c971.Models;
using c971.Services;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace c971.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddTerm : ContentPage
    {
        public AddTerm(User selectedUser)
        {
            InitializeComponent();
            DBservice.CurrentUser = selectedUser;
            userIDLink.Text = selectedUser.UserID.ToString();  
        }

        async void save_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (!String.IsNullOrWhiteSpace(termName.Text))
                {
                    if (startDatePicker.Date <= endDatePicker.Date)
                    {
                        await DBservice.AddTerm(termName.Text, startDatePicker.Date, endDatePicker.Date, int.Parse(userIDLink.Text));
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
            catch(Exception ex)
            {
                await DisplayAlert("Error",
                                   ex.Message,
                                   "OK");
            }
        }
    }
}