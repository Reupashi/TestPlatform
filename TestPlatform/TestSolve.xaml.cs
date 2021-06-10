using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
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
using TestPlatform.Entityes;

namespace TestPlatform
{
    /// <summary>
    /// Interaction logic for TestSolve.xaml
    /// </summary>
    public partial class TestSolve : Window
    {
        ApplicationContext db;
        List<Question> questions;
        List<Answear> answears;
        object bgSender;
        string savePath;
        string testName;
        int test_id;
        int Question_id;
        bool isExport = false;
        bool isSaveAsTest = false;

        int checked_answ1 = 0;
        int checked_answ2 = 0;
        int checked_answ3 = 0;


        public TestSolve()
        {
            InitializeComponent();
            db = new ApplicationContext(); //Виділення памяті і створення сcилки database
            cmbFontFamily.ItemsSource = Fonts.SystemFontFamilies.OrderBy(f => f.Source);
            cmbFontSize.ItemsSource = new List<double>() { 8, 9, 10, 11, 12, 14, 16, 18, 20, 22, 24, 26, 28, 36, 48, 72 };

            List<Test> tests = db.Tests.ToList();
            testsTree.ItemsSource = tests.ToList();
        }

        public void ClearForm()
        {
            Ans1.SelectAll();
            Ans1.Cut();
            Ans2.SelectAll();
            Ans2.Cut();
            Ans3.SelectAll();
            Ans3.Cut();
            txtBox.SelectAll();
            txtBox.Cut();
            ch_1.IsChecked = false;
            ch_2.IsChecked = false;
            ch_3.IsChecked = false;
        }

        public TextRange In_teg(TextRange range)
        {
            if (range.Text.Length !=0)
            {
                range.Text = range.Text.Substring(0, range.Text.Length - 2);
            }
            else if (range.Text.Length == 0)
            {
                range.Text = "";
            }
            return range;
        }
        private void Is_Export(TextRange range, FileStream fileStream)
        {
            string question = range.Text;
            if (question.Contains("<Question>"))
            {
                question = question.Remove(0, 11);
                question = question.Remove(question.IndexOf("</Question>") + 1);
                question = $"<Question> {In_teg(range).Text} </Question>";
            }
            else
            {
                question = $"<Question> {In_teg(range).Text} </Question>";
            }

            string temp = range.Text;
            range.Text = question;
            range.Save(fileStream, DataFormats.Rtf);
            fileStream.Dispose();
            range.Text = temp;
            isExport = true;
            isSaveAsTest = false;

        }

        private void Is_Save_As_Test(TextRange range, TextRange answ1, TextRange answ2, TextRange answ3 )
        {
            List<Test> tests = db.Tests.ToList();
            Question question = new Question(In_teg(range).Text, test_id);
            db.Questions.Add(question);

            db.SaveChanges();

            Question_id = question.question_id;

            

            Answear answear1 = new Answear(In_teg(answ1).Text, Question_id, checked_answ1);
            db.Answears.Add(answear1);
            Answear answear2 = new Answear(In_teg(answ2).Text, Question_id, checked_answ2);
            db.Answears.Add(answear2);
            Answear answear3 = new Answear(In_teg(answ3).Text, Question_id, checked_answ3);
            db.Answears.Add(answear3);

            db.SaveChanges();

            ClearForm();
        }

        private void Is_Save_As_Test(TextRange range)
        {
            List<Test> tests = db.Tests.ToList();
            Question question = new Question(In_teg(range).Text, test_id);
            db.Questions.Add(question);

            db.SaveChanges();

            //using (SQLiteConnection Connect = new SQLiteConnection(@"Data Source=C:\Users\Reihpashi\Desktop\TestPlatform\TestPlatform\LoginsDB.db;
            //            Version=3;"))
            //{
            //    string commandText = "UPDATE [Questions] SET [question_text] = @value WHERE [question_id] = @id";
            //    SQLiteCommand Command = new SQLiteCommand(commandText, Connect);
            //    Command.Parameters.AddWithValue("@value", "Aboba");
            //    Command.Parameters.AddWithValue("@id", 2); // присваиваем переменной номер (id) записи, которую будем обновлять
            //    Connect.Open();
            //    // Command.ExecuteNonQuery(); // можно эту строку вместо двух последующих строк
            //    // Int32 _rowsUpdate = Command.ExecuteNonQuery(); // sql возвращает сколько строк обработано
            //    // MessageBox.Show("Обновлено строк: " + _rowsUpdate);
            //    Connect.Close();
            //}

            isSaveAsTest = true;
            isExport = false;
            txtBox.SelectAll();
            txtBox.Cut();
        }

        #region editing_buttons
        private void Create_test(object sender, RoutedEventArgs e)
        {
            NewTestWindow newTestWindow = new NewTestWindow();
            if (newTestWindow.ShowDialog() == true)
            {
                testName = newTestWindow.testName.Text;
                string testDescription = newTestWindow.testDescription.Text;
                int time = 1800;
                int.TryParse(newTestWindow.time_for_test.Text, out time);
                Test test = new Test(testName, testDescription, time);
                db.Tests.Add(test);
                db.SaveChanges();
                isSaveAsTest = true;
                isExport = false;

            }
            List<Test> tests = db.Tests.ToList();
            test_id = tests.Count();
        }
        private void Export(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "Rich Text Format (*.rtf)|*.rtf|All files (*.*)|*.*";
            isExport = true;
            isSaveAsTest = false;
            if (dlg.ShowDialog() == true)
                try
                {
                    {
                        using (FileStream fileStream = new FileStream(dlg.FileName, FileMode.Create))
                        {
                            TextRange range = new TextRange(txtBox.Document.ContentStart, txtBox.Document.ContentEnd);

                            Is_Export(range, fileStream);

                            savePath = dlg.FileName;
                        }
                    }
                }
                catch
                {
                    MessageBox.Show("Файл виконується іншою програмою, спробуйте зберегти файл окремо");
                }

        }

        private void Save_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "Rich Text Format (*.rtf)|*.rtf|All files (*.*)|*.*";
            isExport = false;
            isSaveAsTest = false;
            if (dlg.ShowDialog() == true) { 
                try
                {
                    using (FileStream fileStream = new FileStream(dlg.FileName, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                    {
                        TextRange range = new TextRange(txtBox.Document.ContentStart, txtBox.Document.ContentEnd);
                        TextRange answ1 = new TextRange(Ans1.Document.ContentStart, Ans1.Document.ContentEnd);
                        TextRange answ2 = new TextRange(Ans2.Document.ContentStart, Ans2.Document.ContentEnd);
                        TextRange answ3 = new TextRange(Ans3.Document.ContentStart, Ans3.Document.ContentEnd);
                        var text = range.Text + "\n" + answ1.Text + answ2.Text + answ3.Text;
                        var temp = answ1.Text;
                        answ1.Text = text;

                        answ1.Save(fileStream, DataFormats.Rtf);
                        answ1.Text = temp;

                        savePath = dlg.FileName;
                        fileStream.Dispose();
                    }
                  
                 }
                catch
                { MessageBox.Show("Файл виконується іншою програмою, спробуйте зберегти файл окремо"); }
            }
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            try
            {
                if (isExport == true && isSaveAsTest == false)
                {
                    FileStream fileStream = new FileStream(savePath, FileMode.Open);
                    TextRange range = new TextRange(txtBox.Document.ContentStart, txtBox.Document.ContentEnd);
                    Is_Export(range, fileStream);
                }
                else if (isExport == false && isSaveAsTest == true)
                {
                    Save_As_Test(sender, e);
                }
                else
                {
                    FileStream fileStream = new FileStream(savePath, FileMode.Open);
                    TextRange range = new TextRange(txtBox.Document.ContentStart, txtBox.Document.ContentEnd);
                    range.Save(fileStream, DataFormats.Rtf);
                    fileStream.Dispose();
                }
            }
            catch { MessageBox.Show("Спочатку збережіть файл як (CTRL + S)"); }
        }

        private void Open_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Rich Text Format (*.rtf)|*.rtf|All files (*.*)|*.*";
            if (dlg.ShowDialog() == true)
            {
                FileStream fileStream = new FileStream(dlg.FileName, FileMode.Open);
                TextRange range = new TextRange(txtBox.Document.ContentStart, txtBox.Document.ContentEnd);
                range.Load(fileStream, DataFormats.Rtf);
                savePath = dlg.FileName;
                fileStream.Dispose();

            }
        }

        private void New_file(object sender, RoutedEventArgs e)
        {
            try
            {
                using (FileStream fileStream = new FileStream(savePath, FileMode.Open))
                {
                    TextRange range = new TextRange(txtBox.Document.ContentStart, txtBox.Document.ContentEnd);

                    string now = range.Text;
                    range.Save(fileStream, DataFormats.Rtf);
                    fileStream.Dispose();
                    savePath = "";
                    ClearForm();
                }
            }
            catch
            {
                SaveFileDialog dlg = new SaveFileDialog();
                dlg.Filter = "Rich Text Format (*.rtf)|*.rtf|All files (*.*)|*.*";
                if (dlg.ShowDialog() == true)
                    try
                    {
                        {
                            using (FileStream fileStream = new FileStream(dlg.FileName, FileMode.Create))
                            {
                                TextRange range = new TextRange(txtBox.Document.ContentStart, txtBox.Document.ContentEnd);
                                range.Save(fileStream, DataFormats.Rtf);
                                //save = dlg.FileName;
                                savePath = "";
                                ClearForm();
                            }
                        }
                    }
                    catch { }
            }

        }

        private void Save_As_Test(object sender, RoutedEventArgs e)
        {
            try {
                TextRange range = new TextRange(txtBox.Document.ContentStart, txtBox.Document.ContentEnd);

                List<Question> questions_names = db.Questions.Where(b => b.test_id == test_id).ToList();
                foreach (Question question in questions_names)
                {
                    if (question.Question_Text.Trim() == range.Text.ToString().Trim())
                    {
                        MessageBox.Show("Текст питання не може повторюватись");
                        return;
                    }
                }

                int i = range.Text.Trim().Length;
                if (i != 0)
                {
                    TextRange answ1 = new TextRange(Ans1.Document.ContentStart, Ans1.Document.ContentEnd);
                    TextRange answ2 = new TextRange(Ans2.Document.ContentStart, Ans2.Document.ContentEnd);
                    TextRange answ3 = new TextRange(Ans3.Document.ContentStart, Ans3.Document.ContentEnd);
                    isSaveAsTest = true;
                    isExport = false;
                    Is_Save_As_Test(range, answ1, answ2, answ3);
                    refresh_list_Click(sender, e);
                }
                else
                {
                    MessageBox.Show("Питання не може бути порожнім");
                }
            }
            catch {
                MessageBox.Show("Оберіть тест у який бажаєте додати питання");
            };
        }

        private void cmbFontFamily_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbFontFamily.SelectedItem != null)
                txtBox.Selection.ApplyPropertyValue(Inline.FontFamilyProperty, cmbFontFamily.SelectedItem);
        }

        private void cmbFontSize_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                txtBox.Selection.ApplyPropertyValue(Inline.FontSizeProperty, cmbFontSize.Text);
            }
            catch { }
        }

        #endregion

        private void selected_text(object sender, RoutedEventArgs e)
        {
            object
            temp = txtBox.Selection.GetPropertyValue(Inline.FontWeightProperty);
            btnBold.IsChecked = (temp != DependencyProperty.UnsetValue) && (temp.Equals(FontWeights.Bold));
            temp = txtBox.Selection.GetPropertyValue(Inline.FontStyleProperty);
            btnItalic.IsChecked = (temp != DependencyProperty.UnsetValue) && (temp.Equals(FontStyles.Italic));
            temp = txtBox.Selection.GetPropertyValue(Inline.TextDecorationsProperty);
            btnUnderline.IsChecked = (temp != DependencyProperty.UnsetValue) && (temp.Equals(TextDecorations.Underline));

            temp = txtBox.Selection.GetPropertyValue(Inline.FontFamilyProperty);
            cmbFontFamily.SelectedItem = temp;
            temp = txtBox.Selection.GetPropertyValue(Inline.FontSizeProperty);
            cmbFontSize.Text = temp.ToString();


            if (txtBox_Copy == null) return;
            txtBox_Copy.Text = string.Format("Виділено : \"{0}\" ", txtBox.Selection.Text);
        }

        #region list_Of_Tests

        private void refresh_list_Click(object sender, RoutedEventArgs e)
        {
            List<Test> tests = db.Tests.ToList();
            testsTree.ItemsSource = tests.ToList();
        }
        private void delete_test_list_Click(object sender, RoutedEventArgs e)
        {
            List<Test> tests = db.Tests.Where(b => b.test_id == test_id).ToList();
            Test test = tests.Find(b => b.test_id == test_id);          

            MessageBoxResult result = MessageBox.Show($"Ви впевнені що хочете видалити тест \"{test.Name}\", без можливості повернення? ", "My App", MessageBoxButton.YesNo);
            switch (result)
            {
                case MessageBoxResult.Yes:
                    MessageBox.Show($"Tест \"{test.Name}\", видалено", "My App");
                    List<Question> questions = db.Questions.Where(b => b.test_id == test_id).ToList();
                    List<Answear> answears;

                    db.Tests.Remove(test);
                    db.Questions.RemoveRange(questions);
                    db.SaveChanges();

                    foreach(Question question in questions)
                    {
                        answears = db.Answears.Where(b => b.question_id == question.question_id).ToList();
                        db.Answears.RemoveRange(answears);
                        db.SaveChanges();
                    }

                    break;
                case MessageBoxResult.No:
                    break;
            }

            refresh_list_Click(sender, e);
        }
        private void delete_question_list_Click(object sender, RoutedEventArgs e)
        {
            List<Question> questions = db.Questions.Where(b => b.question_id == Question_id).ToList();
            Question question = questions.Find(b => b.question_id == Question_id);
            List<Answear> answears = db.Answears.Where(b => b.question_id == Question_id).ToList();

            MessageBoxResult result = MessageBox.Show($"Ви впевнені що хочете видалити питання \"{question.Question_Text}\", без можливості повернення? ", "My App", MessageBoxButton.YesNo);
            switch (result)
            {
                case MessageBoxResult.Yes:
                    MessageBox.Show($"Питання \"{question.Question_Text}\", видалено", "My App");
                    db.Questions.Remove(question);
                    db.Answears.RemoveRange(answears);
                    db.SaveChanges();
                    break;
                case MessageBoxResult.No:
                    break;
            }
            refresh_list_Click(sender, e);
        }
        private void TreeViewItem_Expanded(object sender, RoutedEventArgs e)
        {
            int temp = test_id;
            TreeViewItem tvItem = (TreeViewItem)sender;

            List<Test> tests = db.Tests.ToList();
            foreach (Test test in tests)
            {
                if (tvItem.Header.ToString() == test.Name)
                {
                    test_id = test.test_id;
                }
            }
            questions = db.Questions.Where(b => b.test_id == test_id).ToList();

            int question_id = 0;
            tvItem.Items.Clear();

            foreach (Question question in questions)
            {
                var item = new TreeViewItem();
                item.Tag = question_id;
                item.AddHandler(TreeViewItem.GotFocusEvent, new RoutedEventHandler(Question_Click));
                item.Header = question.Question_Text.ToString();
                item.ToolTip = item.Tag;

                tvItem.Items.Add(item);
                question_id += 1;
            }
            test_id = temp;
        }
        private void Question_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ClearForm();

                TreeViewItem tvItem = (TreeViewItem)sender;

                // получаю батьківський обєкт дерева
                var parent = tvItem.Parent;
                TreeViewItem parentTVI = (TreeViewItem)parent;


                // по заголовку знаходжу айді тесту
                List<Test> tests = db.Tests.ToList();
                foreach (Test test in tests)
                {
                    if (parentTVI.Header.ToString() == test.Name)
                    {
                        test_id = test.test_id;
                    }
                }
                // Знаходимо айді питання
                List<Question> questions1 = db.Questions.Where(b => b.test_id == test_id).ToList();
                foreach (Question question in questions1)
                {
                    if (tvItem.Header.ToString() == question.Question_Text)
                    {
                        Question_id = question.question_id;
                    }
                };

                questions = db.Questions.Where(b => b.test_id == test_id).ToList();
                answears = db.Answears.Where(b => b.question_id == Question_id).ToList();

                TextRange range = new TextRange(txtBox.Document.ContentStart, txtBox.Document.ContentEnd);
                TextRange answ1 = new TextRange(Ans1.Document.ContentStart, Ans1.Document.ContentEnd);
                TextRange answ2 = new TextRange(Ans2.Document.ContentStart, Ans2.Document.ContentEnd);
                TextRange answ3 = new TextRange(Ans3.Document.ContentStart, Ans3.Document.ContentEnd);

                range.Text = questions[(int)tvItem.Tag].Question_Text;
                answ1.Text = answears[0].Answeare;
                answ2.Text = answears[1].Answeare;
                answ3.Text = answears[2].Answeare;

                if (answears[0].Right == 1)
                { ch_1.IsChecked = true; }
                else { ch_1.IsChecked = false; };

                if (answears[1].Right == 1)
                { ch_2.IsChecked = true; }
                else { ch_2.IsChecked = false; };

                if (answears[2].Right == 1)
                { ch_3.IsChecked = true; }
                else { ch_3.IsChecked = false; };
            }
            catch 
            {
                MessageBox.Show("Даний тест не містить питань");
                ClearForm();

                TreeViewItem tvItem = (TreeViewItem)sender;

                // получаю батьківський обєкт дерева
                var parent = tvItem.Parent;
                TreeViewItem parentTVI = (TreeViewItem)parent;


                // по заголовку знаходжу айді тесту
                List<Test> tests = db.Tests.ToList();
                foreach (Test test in tests)
                {
                    if (parentTVI.Header.ToString() == test.Name)
                    {
                        test_id = test.test_id;
                    }
                }
                // Знаходимо айді питання
                List<Question> questions1 = db.Questions.Where(b => b.test_id == test_id).ToList();
                foreach (Question question in questions1)
                {
                    if (tvItem.Header.ToString() == question.Question_Text)
                    {
                        Question_id = question.question_id;
                    }
                };

                questions = db.Questions.Where(b => b.test_id == test_id).ToList();
                TextRange range = new TextRange(txtBox.Document.ContentStart, txtBox.Document.ContentEnd);


            }

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

        private void txtBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (bgSender != null)
            {
                TreeViewItem tvItem = (TreeViewItem)bgSender;
                tvItem.Background = Brushes.ForestGreen;
            }
        }
        #endregion

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

        private void open_Window_Click(object sender, RoutedEventArgs e)
        {
            TestSelect TS = new TestSelect();
            TS.Show();
            this.Close();
        }

    }
}




//private void Save(object sender, RoutedEventArgs e)
//{
//    try
//    {
//        FileStream fileStream = new FileStream(savePath, FileMode.Open);
//        TextRange range = new TextRange(txtBox.Document.ContentStart, txtBox.Document.ContentEnd);

//        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//        //if (range.Text.Contains("<Question>"))
//        //{

//        //    range.Text = range.Text.Remove(0, 11);
//        //    range.Text = range.Text.Remove(range.Text.IndexOf("</Question>") + 1);
//        //    //Console.WriteLine("+++++++++++++++++++++++" + now);
//        //    range.Text = $"<Question> {In_teg(range).Text} </Question>";
//        //}
//        //else
//        //{
//        //    ///////////////////////////////////////////////txtBox.AppendText(" </Question>");

//        //    range.Text = $"<Question> {In_teg(range).Text} </Question>";
//        //}
//        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

//        range.Save(fileStream, DataFormats.Rtf);
//        fileStream.Dispose();


//        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

//        //if (range.Text.Contains("</Question>"))
//        //    {

//        //    ////////////////////////////////////////////////////txtBox.Selection.Select(txtBox.Document.ContentEnd.GetPositionAtOffset(12,0), txtBox.Document.ContentEnd);
//        //    ////////////////////////////////////////////////////txtBox.Cut();

//        //        range.Text = range.Text.Remove(0, 11);
//        //        range.Text = range.Text.Remove(range.Text.IndexOf(" </Question>"));

//        //    }
//        //    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

//        fileStream.Dispose();

//    }
//    catch { MessageBox.Show("Спочатку збережіть файл як (CTRL + S)"); }

//}
