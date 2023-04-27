using c971.Models;
using c971.Services;
using c971.Views;
using System;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace c971
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Terms : ContentPage
    {   
        //Used to ensure the NotificationCheck function only runs once
        public bool notifications = true;
        public Terms(User selectedUser)
        {
            InitializeComponent();
            DBservice.CurrentUser = selectedUser;
        }

        async void AddTerm_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AddTerm(DBservice.CurrentUser));
        }
        
        protected override async void OnAppearing()
        {
            base.OnAppearing();                 
            collectionViewTerms.ItemsSource = await DBservice.GetTerms(DBservice.CurrentUser);
            _ = DBservice.NotificationCheck(notifications);
            notifications = false;
        }

        async void collectionViewTerms_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection != null)
            {
                Term term = (Term)e.CurrentSelection.FirstOrDefault();
                await Navigation.PushAsync(new TermInformation(term));              
            }           
        }

        async void courseSearch_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CourseSearch(DBservice.CurrentUser));
        }

        async void academicReport_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AcademicReport(DBservice.CurrentUser));
        }
    }
}