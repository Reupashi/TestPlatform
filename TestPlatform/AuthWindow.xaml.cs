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
using System.Windows.Shapes;

namespace TestPlatform
{
    /// <summary>
    /// Interaction logic for AuthWindow.xaml
    /// </summary>
    public partial class AuthWindow : Window
    {
        string charackter;
        ApplicationContext db;
        User authUser;
        string path = "log.txt";

        string login;
        string password;

        public AuthWindow()
        {
            db = new ApplicationContext(); //Виділення памяті і створення силки
            InitializeComponent();
        }
        private void Log(string eventName)
        {
            using (StreamWriter logger = new StreamWriter(path, true))
            {
                logger.WriteLine(DateTime.Now.Date.ToLongDateString() + " / " + DateTime.Now.ToLongTimeString() + " - " + eventName);
            }
        }
        private void Auth_Button_Click(object sender, RoutedEventArgs e)
        {
            login = Login.Text.ToLower().Trim();
            password = Password.Password.Trim();
            

            if (login.Length < 5) //Валідація полів вводу
            {
                Login.ToolTip = "Логін повинен містити більше 5 символів";
                Login.Background = Brushes.IndianRed;
            }
            else if (password.Length < 5) //Валідація полів вводу
            {
                Password.ToolTip = "Пароль повинен містити 5 або більше  символів";
                Password.Background = Brushes.IndianRed;
            }
            else
            {
                Login.ToolTip = "";
                Login.Background = Brushes.Transparent;
                Password.ToolTip = "";
                Password.Background = Brushes.Transparent;

                authUser = db.Users.Where(b => b.Login == login && b.Password == password).FirstOrDefault();
                
                if (authUser != null)
                {
                    charackter = authUser.Login;

                    if (authUser.IsAdmin == 1)
                    {
                        TestSolve testSolve = new TestSolve();
                        testSolve.Show();
                        this.Close();
                    }
                    else
                    {
                        TestSelect testSelect = new TestSelect();
                        testSelect.Show();
                        this.Close();
                        Log($"Користувач {authUser.Name} {authUser.Surname} авторизувався для проходження тестів, група {authUser.Group}");
                    }                
                }
                else
                {
                    MessageBox.Show("Введено некоректні дані");
                }
            }
        }

        private void Login_Button_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }
}
