using c971.Models;
using c971.Services;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace c971.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CourseSearch : ContentPage
    {
       
    public CourseSearch(User selecteduser)
        {
            InitializeComponent();
            DBservice.CurrentUser = selecteduser;
        }


        protected override async void OnAppearing()
        {
            base.OnAppearing();

            courseList.ItemsSource = new List<Course>();

            await DBservice.Init();
        }

        //show courses for the user currently logged in
        private async void searchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(searchBar.Text))
            {
                courseList.ItemsSource = null;
                return;
            }

            //retrieve all courses
            List<Course> allCourses = await DBservice._db.Table<Course>().ToListAsync();

            //retrieve terms that belong to the user currently logged in
            List<Term> userTerms = await DBservice._db.Table<Term>()
                                                      .Where(i => i.UserID == DBservice.CurrentUser.UserID)
                                                      .ToListAsync();

            //Get the TermID for each term associated with user
            List<int> termIDs = new List<int>();
            foreach(Term term in userTerms)
            {
                termIDs.Add(term.TermID);
            }

            //Use the TermIDs to get courses associated with those terms
            //The userCourses List is what is actually searched through
            List<Course> userCourses = await DBservice._db.Table<Course>()
                                                          .Where(i => termIDs.Contains(i.TermID))
                                                          .ToListAsync();

            //Show courses when there is a match
            courseList.ItemsSource = userCourses.Where(i => i.CourseName.ToLower().Contains(searchBar.Text.ToLower()))
                                                .ToList();
        }
    }
}