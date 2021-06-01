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
using TestPlatform.Entityes;


namespace TestPlatform
{
    /// <summary>
    /// Логика взаимодействия для ForStudent.xaml
    /// </summary>
    public partial class ForStudent : Window
    {
        ApplicationContext db;
        List<Question> mixedQuestions;
        List<Answear> answears;
        int Question_id;
        int test_id;
        int counter = 0;
        int points = 0;

        int checked_answ1 = 0;
        int checked_answ2 = 0;
        int checked_answ3 = 0;

        public ForStudent()
        {
            InitializeComponent();
            db = new ApplicationContext(); //Виділення памяті і створення сcилки database
            
            
        }

        public void Mix()
        {
            int.TryParse(testId.Text.ToString(), out test_id);
            Console.WriteLine("//////////////////" + test_id);

            List<Test> tests = db.Tests.ToList();
            Test test = tests.Find(b => b.test_id == test_id);
            this.Title = test.Name.ToString();

            Random rnd = new Random();

            if ((int)test_id != -1) // якщо тест було обрано він перемішується в новий список 
            {
                mixedQuestions = db.Questions.Where(b => b.test_id == test_id).ToList();
                for (int i = mixedQuestions.Count() - 1; i >= 1; i--)
                {
                    int j = rnd.Next(i + 1);
                    var temp = mixedQuestions[j];
                    mixedQuestions[j] = mixedQuestions[i];
                    mixedQuestions[i] = temp;
                }
                foreach (Question question in mixedQuestions)
                {
                    Console.WriteLine("//////////////////////////////////" + question.Question_Text);
                }
            }
            else
            {
                MessageBox.Show("Оберіть тест для проходження");
            }
        }
        
        private void start_Click(object sender, RoutedEventArgs e)
        {
            Mix();
            Button start = (Button)sender;
            start.Visibility = Visibility.Collapsed;
            ch_1.Visibility = Visibility.Visible;
            ch_2.Visibility = Visibility.Visible;
            ch_3.Visibility = Visibility.Visible;

            //next_Click(sender, e);
            question.Text = mixedQuestions[counter].Question_Text;
            Question_id = mixedQuestions[counter].question_id;
            answears = db.Answears.Where(b => b.question_id == Question_id).ToList();
            ch_1.Content = answears[0].Answeare.ToString();
            ch_2.Content = answears[1].Answeare.ToString();
            ch_3.Content = answears[2].Answeare.ToString();

            ch_1.IsChecked = false;
            ch_2.IsChecked = false;
            ch_3.IsChecked = false;
            checked_answ1 = 0;
            checked_answ2 = 0;
            checked_answ3 = 0;

        }

        private void next_Click(object sender, RoutedEventArgs e)
        {

            if(checked_answ1 == checked_answ2 && checked_answ3 == checked_answ1 && checked_answ1 == 0)
            {
                MessageBox.Show("Ви не обрали відповіді");
                return;
            }
            else
            {
                points += checked_answ1 * answears[0].Right;
                Console.WriteLine("p1/" + checked_answ1 * answears[0].Right);              
                points += checked_answ2 * answears[1].Right;
                Console.WriteLine("p2/" + checked_answ2 * answears[1].Right);               
                points += checked_answ3 * answears[2].Right;
                Console.WriteLine("p3/" + checked_answ3 * answears[2].Right);
                ch_1.IsChecked = false;
                ch_2.IsChecked = false;
                ch_3.IsChecked = false;
                checked_answ1 = 0;
                checked_answ2 = 0;
                checked_answ3 = 0;

                counter += 1;

                if (counter != mixedQuestions.Count)
                {
                    question.Text = mixedQuestions[counter].Question_Text;/////////////////
                    Question_id = mixedQuestions[counter].question_id;
                    answears = db.Answears.Where(b => b.question_id == Question_id).ToList();
                    ch_1.Content = answears[0].Answeare.ToString();
                    ch_2.Content = answears[1].Answeare.ToString();
                    ch_3.Content = answears[2].Answeare.ToString();
                    Console.WriteLine(points);
                }
                else
                {
                    question.Text = (points).ToString();
                    next.Visibility = Visibility.Collapsed;
                    ch_1.Visibility = Visibility.Collapsed;
                    ch_2.Visibility = Visibility.Collapsed;
                    ch_3.Visibility = Visibility.Collapsed;

                }
            }
        }

        private void RadioButton1_Checked(object sender, RoutedEventArgs e)
        {
            checked_answ1 = 1;
            checked_answ2 = 0;
            checked_answ3 = 0;
        }
        private void RadioButton2_Checked(object sender, RoutedEventArgs e)
        {
            checked_answ1 = 0;
            checked_answ2 = 1;
            checked_answ3 = 0;
        }
        private void RadioButton3_Checked(object sender, RoutedEventArgs e)
        {
            checked_answ1 = 0;
            checked_answ2 = 0;
            checked_answ3 = 1;
        }
    }
}
