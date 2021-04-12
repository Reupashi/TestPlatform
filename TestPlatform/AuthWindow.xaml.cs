using System;
using System.Collections.Generic;
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
        ApplicationContext db;

        string login;
        string password;

        public AuthWindow()
        {
            db = new ApplicationContext(); //Виділення памяті і створення силки
            InitializeComponent();
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
            else if (password.Length < 7) //Валідація полів вводу
            {
                Password.ToolTip = "Пароль повинен містити більше 7 символів";
                Password.Background = Brushes.IndianRed;
            }
            else
            {
                Login.ToolTip = "";
                Login.Background = Brushes.Transparent;
                Password.ToolTip = "";
                Password.Background = Brushes.Transparent;

                User authUser = null;
                using (ApplicationContext db = new ApplicationContext())
                {
                    authUser = db.Users.Where(b => b.Login == login && b.Password == password).FirstOrDefault();
                }
                if (authUser != null)
                {
                    TestSolve testSolve = new TestSolve();
                    testSolve.Show();
                    this.Hide();
                }
                    else
                    {
                    MessageBox.Show("Введено некоректні дані");
                    }
                

            }

        }
    }
}
