using c971.Models;
using c971.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace c971.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Courses : ContentPage
    {
        public Courses(Term selectedTerm)
        {
            InitializeComponent();
            DBservice.CurrentTerm = selectedTerm;           
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            collectionViewCourses.ItemsSource = await DBservice.GetCourses(DBservice.CurrentTerm);            
        }

        async void collectionViewCourses_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection != null)
            {
                Course course = (Course)e.CurrentSelection.FirstOrDefault();
                await Navigation.PushAsync(new CourseInformation(course));               
            }
        }

        async void addCourse_Clicked(object sender, EventArgs e)
        {          
            IEnumerable<Course> courses = await DBservice.GetCourses(DBservice.CurrentTerm);

            if (courses.ToList().Count < 6)
            {
                await Navigation.PushAsync(new AddCourse(DBservice.CurrentTerm));
            }
            else
            {
                await DisplayAlert("Error",
                                   "Unable to add more courses",
                                   "OK");
            }
        }
    }
}