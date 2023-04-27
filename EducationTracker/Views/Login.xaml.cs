using c971.Services;
using System;
using System.Text.RegularExpressions;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace c971.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Login : ContentPage
    {
        public Login()
        {
            InitializeComponent();
        }

        async void loginButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (!String.IsNullOrWhiteSpace(userName.Text) && !String.IsNullOrWhiteSpace(passWord.Text))
                {
                    await DBservice.Init();

                    //Checks to see if login information is correct.
                    var user = await DBservice.GetUser(userName.Text, passWord.Text);

                    //If user has data then the login information was correct.
                    if (user != null)
                    {
                        DBservice.CurrentUser = user;
                        await Navigation.PushAsync(new Terms(DBservice.CurrentUser));
                    }

                    else
                    {
                        await DisplayAlert("Error",
                                           "Invalid credentials",
                                           "OK");
                    }
                }
                else
                {
                    await DisplayAlert("Error",
                                       "Please ensure no fields are left blank",
                                       "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error",
                                   ex.Message,
                                   "OK");
            }
        }

        public async void createButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                await DBservice.Init();
                //Check to see if user already exists
                var userCheck = await DBservice.GetUser(userName.Text, passWord.Text);

                //If usercheck returned no user then the user does not already exist
                if (userCheck == null)
                {
                    await DBservice.AddUser(userName.Text, passWord.Text);
                    await DisplayAlert("Success",
                                       "user with username of " + userName.Text + " and password of " + passWord.Text + " added successfully.",
                                       "OK");
                }
                else
                {
                    await DisplayAlert("Error",
                                       "A user with those credentials already exists, please login instead",
                                       "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error",
                                   ex.Message,
                                   "OK");
            }
        }

        //Below code is to ensure only lower and upper case letters can be entered for username and password.
        private void userName_TextChanged(object sender, TextChangedEventArgs e)
        {
            bool match = Regex.IsMatch(e.NewTextValue,
                                       "^[a-zA-Z]+$");

            string enteredText = e.NewTextValue;
            if (enteredText.Length > 0)
            {
                (sender
                 as Entry).Text = !match ? enteredText.Remove(enteredText.Length - 1) : enteredText;
            }
        }

        private void passWord_TextChanged(object sender, TextChangedEventArgs e)
        {
            bool match = Regex.IsMatch(e.NewTextValue,
                                       "^[a-zA-Z]+$");

            string enteredText = e.NewTextValue;
            if (enteredText.Length > 0)
            {
                (sender
                 as Entry).Text = !match ? enteredText.Remove(enteredText.Length - 1) : enteredText;
            }
        }
    }
}