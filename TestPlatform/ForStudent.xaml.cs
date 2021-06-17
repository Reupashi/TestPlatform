using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
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
        System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
        Test test;

        int Question_id;
        int test_id;
        int counter = 0;
        int points = 0;
        int time = 0;
        string path = "log.txt";

        int checked_answ1 = 0;
        int checked_answ2 = 0;
        int checked_answ3 = 0;

        public ForStudent()
        {
            InitializeComponent();
            db = new ApplicationContext(); //Виділення памяті і створення сcилки database
            string chaerackter = "test";

            List<Test> tests = db.Tests.Where(b => b.test_id == test_id).ToList();
            test = tests.Find(b => b.test_id == test_id);
            this.Closing += new CancelEventHandler(MainWindow_Closing);

            Log($"______________________________ Тест здає : {chaerackter} ______________________________");

            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);

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
        private void Log(string eventName)
        {
            using (StreamWriter logger = new StreamWriter(path, true))
            {
                logger.WriteLine(DateTime.Now.Date.ToLongDateString() + " / " + DateTime.Now.ToLongTimeString() + " - " + eventName);
            }
        }
        private void Timer(bool work)
        {

            if (work == true)
            {
                dispatcherTimer.Start();
            }
            else if (work == false)
            {
                dispatcherTimer.Stop();
            }
        }
        private void start_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Mix();
                Button start = (Button)sender;
                start.Visibility = Visibility.Collapsed;
                ch_1.Visibility = Visibility.Visible;
                ch_2.Visibility = Visibility.Visible;
                ch_3.Visibility = Visibility.Visible;

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

                List<Test> tests = db.Tests.Where(b => b.test_id == test_id).ToList();
                Test test = tests.Find(b => b.test_id == test_id);
                time = test.Time;
                Timer(true);
                Log($"Тест розпочався, часу на проходження: ({time} секунд)");
                Log($"Питання номер {counter + 1}: {question.Text}");
            }
            catch
            {
                MessageBox.Show("Даний тест не містить питань");
                EndTest();
                TestSelect ts = new TestSelect();
                ts.Show();
                this.Close();
            }
        }


        private void next_Click(object sender, RoutedEventArgs e)
        {

            if (checked_answ1 == checked_answ2 && checked_answ3 == checked_answ1 && checked_answ1 == 0)
            {
                MessageBox.Show("Ви не обрали відповіді");
                return;
            }
            else
            {
                if (checked_answ1 == 1)
                {
                    Log($"Обрана відповідь 1({answears[0].Right}): {answears[0].Answeare}" + $"  ///Часу залишилось: {time} секунд");
                }
                else if (checked_answ2 == 1)
                {
                    Log($"Обрана відповідь 2({answears[1].Right}): {answears[1].Answeare}" + $"  ///Часу залишилось: {time} секунд");
                }
                else
                {
                    Log($"Обрана відповідь 3({answears[2].Right}): {answears[2].Answeare}" + $"  ///Часу залишилось: {time} секунд");
                }

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

                    Log($"Питання номер {counter + 1}: {question.Text}");

                    Question_id = mixedQuestions[counter].question_id;
                    answears = db.Answears.Where(b => b.question_id == Question_id).ToList();
                    ch_1.Content = answears[0].Answeare.ToString();
                    ch_2.Content = answears[1].Answeare.ToString();
                    ch_3.Content = answears[2].Answeare.ToString();

                    Console.WriteLine(points);
                }
                else
                {
                    EndTest();
                }
            }
        }
        private void EndTest()
        {
            time = 0;
            Timer(false);
            time_str.Text = "0:0";
            Log($"-----------------------------------------------------Набрано балів : {points}");
            question.Text = "Ви набрали балів : " + (points).ToString();
            next.Visibility = Visibility.Collapsed;
            ch_1.Visibility = Visibility.Collapsed;
            ch_2.Visibility = Visibility.Collapsed;
            ch_3.Visibility = Visibility.Collapsed;
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
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (time != 0)
            {
                time -= 1;
                time_str.Text = time / 60 + ":" + time % 60;
            }
            else
            {
                Timer(false);
                EndTest();

            }
        }


        void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            if (mixedQuestions != null && time != 0)
            {
                MessageBoxResult result = MessageBox.Show($"Ви справді хочете закрити тест та завершити його, без можливості повернення? ", "My App", MessageBoxButton.YesNo);
                switch (result)
                {
                    case MessageBoxResult.Yes:
                        EndTest();
                        Log("Window Closed before end of time");
                        break;
                    case MessageBoxResult.No:
                        e.Cancel = true;
                        break;
                }
            }
            else
            {
                Log("-----------------------------------------------------Window Closed" + "\n");
                return;
            }
        }


    }
}
