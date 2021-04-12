using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TestPlatform
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ApplicationContext db;

        string login;
        string password;
        string rep_password;
        string email;

        public MainWindow()
        {
            InitializeComponent();
            db = new ApplicationContext(); //Виділення памяті і створення силки

            List<User> users = db.Users.ToList();
            string show = "";
            foreach (User user in users)
                show += "Login " + user.Login + Environment.NewLine;
            Console.WriteLine(show);
        }

        private void Login_Button_Click(object sender, RoutedEventArgs e)
        {
            login = Login.Text.ToLower().Trim();
            password = Password.Password.Trim();
            rep_password = Rep_password.Password.Trim();
            email = Email.Text.ToLower().Trim();

            if (login.Length < 5) //Валідація полів вводу
            {
                Login.ToolTip = "Логін повинен містити більше 5 символів";
                Login.Background = Brushes.IndianRed;
            }
            else if (password.Length < 7) //Валідація полів вводу
            {
                Password.ToolTip = "Пароль повинен містити більше 7 символів";
                Password.Background = Brushes.IndianRed;
            }
            else if (password != rep_password) //Валідація полів вводу
            {
                Rep_password.ToolTip = "Паролі не збігаються";
                Rep_password.Background = Brushes.IndianRed;
            }
            else if (email.Length < 5 || !email.Contains("@") || !email.Contains(".")) //Валідація полів вводу
            {
                Email.ToolTip = "Не по патерну пошти";
                Email.Background = Brushes.IndianRed;
            }
            else
            {
                Login.ToolTip = "";
                Login.Background = Brushes.Transparent;
                Password.ToolTip = "";
                Password.Background = Brushes.Transparent;
                Rep_password.ToolTip = "";
                Rep_password.Background = Brushes.Transparent;
                Email.ToolTip = "";
                Email.Background = Brushes.Transparent;

                try
                {
                    User user = new User(login, password, email);
                    db.Users.Add(user);
                    db.SaveChanges();
                }
                catch { MessageBox.Show("Даний користувач вже зареєстрований"); }

            }

        }

        private void Auth_Button_Click(object sender, RoutedEventArgs e)
        {
            AuthWindow authWindow = new AuthWindow();
            authWindow.Show();
            this.Hide();
        }
    }
}
