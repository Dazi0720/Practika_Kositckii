using System.Windows;
using System.Windows.Controls;

namespace CashRegisterWPF
{
    public partial class RegisterWindow : Window
    {
        private AuthService authService;

        public RegisterWindow(AuthService authService)
        {
            InitializeComponent();
            this.authService = authService;
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Password;
            string confirmPassword = txtConfirmPassword.Password;
            string role = ((ComboBoxItem)cmbRole.SelectedItem).Content.ToString() == "Администратор" ? "admin" : "cashier";

            if (password != confirmPassword)
            {
                MessageBox.Show("Пароли не совпадают!", "Ошибка");
                return;
            }

            if (authService.Register(username, password, role))
            {
                MessageBox.Show("Пользователь успешно зарегистрирован!", "Успех");
                this.Close();
            }
        }

    }
}