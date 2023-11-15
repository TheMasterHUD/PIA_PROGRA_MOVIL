using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Threading.Tasks;
using Plugin.CloudFirestore;

namespace ChatApp.Pages.Auth
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Signup : ContentPage
    {
        DataClass dataClass = DataClass.GetInstance;
        public Signup()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
        }

        private async void Btn_Back(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        [Obsolete]
        private async void Btn_SignUp(object sender, EventArgs e)
        {
            UsernameFrame.BorderColor = Color.FromRgb(189, 189, 189);
            EmailFrame.BorderColor = Color.FromRgb(189, 189, 189);
            PasswordFrame.BorderColor = Color.FromRgb(189, 189, 189);
            ConfirmPasswordFrame.BorderColor = Color.FromRgb(189, 189, 189);

            if (string.IsNullOrEmpty(UsernameEntry.Text) || string.IsNullOrEmpty(EmailEntry.Text) || string.IsNullOrEmpty(PasswordEntry.Text) || string.IsNullOrEmpty(ConfirmPasswordEntry.Text))
            {
                if (string.IsNullOrEmpty(UsernameEntry.Text))
                {
                    UsernameFrame.BorderColor = Color.FromRgb(244, 67, 54);
                }

                if (string.IsNullOrEmpty(EmailEntry.Text))
                {
                    EmailFrame.BorderColor = Color.FromRgb(244, 67, 54);
                }

                if (string.IsNullOrEmpty(PasswordEntry.Text))
                {
                    PasswordFrame.BorderColor = Color.FromRgb(244, 67, 54);
                }
                
                if (string.IsNullOrEmpty(ConfirmPasswordEntry.Text))
                {
                    ConfirmPasswordFrame.BorderColor = Color.FromRgb(244, 67, 54);
                }
                
                await DisplayAlert("Error", "Casillas sin rellenar", "", "ACEPTAR");
                return;
            }

            if (!ValidateEmail.IsValidEmail(EmailEntry.Text))
            {
                await DisplayAlert("Error", "La direccion de correo electronico esta mal formateada.", "", "ACEPTAR");
        
                return;
            }

            if (PasswordEntry.Text != ConfirmPasswordEntry.Text)
            {
                ConfirmPasswordEntry.Text = "";
                ConfirmPasswordEntry.Focus();
                            
                await DisplayAlert("Error", "Las contraseñas no coinciden.", "", "ACEPTAR");
              
                return;
            }

            if (PasswordEntry.Text.Length < 6 && ConfirmPasswordEntry.Text.Length < 6)
            {
                await DisplayAlert("Error", "La contraseña es invalida. [ La contraseña debe contener al menos 6 caracteres ]", "", "ACEPTAR");
                return;
            }

            ActivityIndicator.IsRunning = true;
            FirebaseAuthResponseModel res = new FirebaseAuthResponseModel() { };
            res = await DependencyService.Get<IFirebaseAuth>().SignUpWithEmailPassword(UsernameEntry.Text, EmailEntry.Text, PasswordEntry.Text);

            if (res.Status != true)
            {
                ActivityIndicator.IsRunning = false;
                await DisplayAlert("Error", "El correo ya tiene una cuenta registrada, favor de ingresar uno diferente", "ACEPTAR");
                return;

            }

            try
            {
                await CrossCloudFirestore.Current
                    .Instance
                    .GetCollection("users")
                    .GetDocument(dataClass.loggedInUser.uid)
                    .SetDataAsync(dataClass.loggedInUser);


                await DisplayAlert("EXITOSO", res.Response, "ACEPTAR");
                await Navigation.PopAsync();

            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "ACEPTAR");
            }
            ActivityIndicator.IsRunning = false;
        }

        private void Focused_Username(object sender, EventArgs e)
        {
            UsernameFrame.BorderColor = Color.FromRgb(189, 189, 189);
        }

        private void Focused_Email(object sender, EventArgs e)
        {
            EmailFrame.BorderColor = Color.FromRgb(189, 189, 189);
        }

        private void Focused_Password(object sender, EventArgs e)
        {
            PasswordFrame.BorderColor = Color.FromRgb(189, 189, 189);
        }

        private void Focused_ConfirmPassword(object sender, EventArgs e)
        {
            ConfirmPasswordFrame.BorderColor = Color.FromRgb(189, 189, 189);
        }
        private void toggleVisibility(object sender, EventArgs e)
        {
            if (!PasswordEntry.IsPassword)
            {
                eyeButton1.Source = "hide";
                eyeButton2.Source = "hide";
                PasswordEntry.IsPassword = true;
                ConfirmPasswordEntry.IsPassword = true;

                return;
            }

            eyeButton1.Source = "show";
            eyeButton2.Source = "show";
            PasswordEntry.IsPassword = false;
            ConfirmPasswordEntry.IsPassword = false;
        }
    }
}