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
    /// Логика взаимодействия для TestSelect.xaml
    /// </summary>
    public partial class TestSelect : Window
    {
        ApplicationContext db;
        List<Question> mixedQuestions;
        object bgSender;
        int test_id = -1;


        public TestSelect()
        {
            InitializeComponent();
            db = new ApplicationContext(); //Виділення памяті і створення сcилки database
            create_test_list();


        }
        #region list_Of_Tests

        private void create_test_list()
        {
            
            List<Test> tests = db.Tests.ToList();

            foreach (Test test in tests)
            {
                var item = new TreeViewItem();
                item.Tag = test.test_id;
                item.ToolTip = item.Tag;
                item.AddHandler(TreeViewItem.GotFocusEvent, new RoutedEventHandler(Test_Click));
                item.Header = test.Name.ToString();

                testsTree.Items.Add(item);   
            }
        }

        private void refresh_list_Click(object sender, RoutedEventArgs e)
        {
            testsTree.Items.Clear();
            create_test_list();
        }
        //private void TreeViewItem_Expanded(object sender, RoutedEventArgs e)
        //{
        //    int temp = test_id;
        //    TreeViewItem tvItem = (TreeViewItem)sender;

        //    List<Test> tests = db.Tests.ToList();
        //    foreach (Test test in tests)
        //    {
        //        if (tvItem.Header.ToString() == test.Name)
        //        {
        //            test_id = test.test_id;
        //        }
        //    }
        //    questions = db.Questions.Where(b => b.test_id == test_id).ToList();

        //    int question_id = 0;
        //    tvItem.Items.Clear();

        //    foreach (Question question in questions)
        //    {
        //        var item = new TreeViewItem();
        //        item.Tag = question_id;
        //        item.AddHandler(TreeViewItem.GotFocusEvent, new RoutedEventHandler(Question_Click));
        //        item.Header = question.Question_Text.ToString();
        //        item.ToolTip = item.Tag;

        //        tvItem.Items.Add(item);
        //        question_id += 1;
        //    }
        //    test_id = temp;
        //}
        private void Test_Click(object sender, RoutedEventArgs e)
        {
            TreeViewItem tvItem = (TreeViewItem)sender;

            List<Test> tests = db.Tests.Where(b => b.test_id.ToString() == tvItem.Tag.ToString()).ToList();
            Test test = tests.Find(b => b.test_id.ToString() == tvItem.Tag.ToString());
            TestName.Text = test.Name.ToString();
            TestDescription.Text = test.Description.ToString();
            test_id = test.test_id;
            testId.Text = test_id.ToString();
        }
        private void TreeViewItem_Focused(object sender, RoutedEventArgs e)
        {
            if (bgSender != null)
            {
                TreeViewItem tvItemBG = (TreeViewItem)bgSender;
                tvItemBG.Background = Brushes.White;
            }
            TreeViewItem tvItem = (TreeViewItem)sender;

            List<Test> tests = db.Tests.ToList();
            foreach (Test test in tests)
            {
                if (tvItem.Header.ToString() == test.Name)
                {
                    test_id = test.test_id;
                }
            }
            tvItem.Background = Brushes.ForestGreen;
            bgSender = sender;
        }
        private void TreeViewItem_LostFocus(object sender, RoutedEventArgs e)
        {
            TreeViewItem tvItem = (TreeViewItem)sender;
            tvItem.Background = Brushes.White;

        }
        #endregion

        public void start_Click(object sender, RoutedEventArgs e)
        {
            if ((int)test_id != -1) 
            {
                List<Question> questions = db.Questions.Where(b => b.test_id == test_id).ToList();
                if (questions.Count !=0)
                {
                    ForStudent forSt = new ForStudent();
                    forSt.testId.Text = this.testId.Text;
                    forSt.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Даний тест не містить питань, оберіть інший");
                    return;
                }
            
            }
            else
            {
                MessageBox.Show("Оберіть тест для проходження");
            }
            
        }

    }
}
