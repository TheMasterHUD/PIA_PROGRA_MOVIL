using Plugin.CloudFirestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ChatApp.Pages.Tabbed
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainTabbed : ContentPage
    {
        bool isFriendsNotExist = true;
        bool isLoaded = false;
        DataClass dataClass = DataClass.GetInstance;

        public MainTabbed()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);

            //MessagingCenter.Subscribe<MainPage>(this, "RefreshMainPage", (sender) =>
            //{
            //    isLoaded = false;
            //});

            checkUserContacts();
            // Crear instancia de Profile
            Profile profileComponent = new Profile();
        }
        
        protected override void OnAppearing()
        {
            base.OnAppearing();
            isLoaded = false;
        }

        private void Nav_Chat(object sender, EventArgs e)
        {
            Chats.IsVisible = true;
            Profile.IsVisible = false;
            SearchBar.IsVisible = true;
            AlertLabel.IsVisible = isFriendsNotExist;

            ChatImage.Source = "home_enabled.png";
            ChatLabel.TextColor = Color.FromHex("#e91e63");
            ProfileImage.Source = "profile_disabled.png";
            ProfileLabel.TextColor = Color.FromHex("#bcbcbc");
        }

        private void Nav_InfoApp(object sender, EventArgs e)
        {
            Navigation.PushAsync(new InfoApp());
        }

        private void Nav_Profile(object sender, EventArgs e)
        {
            Chats.IsVisible = false;
            AlertLabel.IsVisible = false;
            Profile.IsVisible = true;
            //Style for new nav items
            //InfoAppImage.IsVisible = false;
            InfoAppLabel.TextColor = Color.FromHex("#bcbcbc");
            //AddContactsImage.IsVisible = false;
            HelpLabel.TextColor = Color.FromHex("#bcbcbc");
            //Original navbar styles
            SearchBar.IsVisible = false;
            ChatImage.Source = "home_disabled.png";
            ChatLabel.TextColor = Color.FromHex("#bcbcbc");
            ProfileImage.Source = "profile_enabled.png";
            ProfileLabel.TextColor = Color.FromHex("#e91e63");
        }

        private void Nav_AddContacts(object sender, EventArgs e)
        {
            Navigation.PushAsync(new SearchResults());
        }

        private void Nav_Help(object sender, EventArgs e)
        {
            Navigation.PushAsync(new Help());
        }

        //private void Nav_Chat(object sender, EventArgs e)
        //{
        //    Navigation.PushAsync(new Chats());
        //}


        private async void Nav_Result(object sender, EventArgs e)
        {
            if (isLoaded == false)
            {
                isLoaded = true;
                await Navigation.PushModalAsync(new SearchResults(), true);
            }
        }

        private async void checkUserContacts()
        {
            string id = dataClass.loggedInUser.uid;
            var firestoreUserContactList = await CrossCloudFirestore.Current.Instance.Collection("users").WhereEqualsTo("uid", id).GetAsync();
            var UserContactList = firestoreUserContactList.ToObjects<UserModel>().FirstOrDefault();

            if (UserContactList != null && UserContactList.contacts.Count != 0) // If naa kay friends e hide ang label
            {
                isFriendsNotExist = false;
                AlertLabel.IsVisible = false;
            } 
            else
            {
                AlertLabel.IsVisible = true;
            }
        }
    }
}