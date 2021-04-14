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
        object bgSender;
        string savePath;
        string testName;
        int test_id;
        bool isExport = false;
        bool isSaveAsTest = false;


        public TestSolve()
        {
            InitializeComponent();
            db = new ApplicationContext(); //Виділення памяті і створення сcилки database
            cmbFontFamily.ItemsSource = Fonts.SystemFontFamilies.OrderBy(f => f.Source);
            cmbFontSize.ItemsSource = new List<double>() { 8, 9, 10, 11, 12, 14, 16, 18, 20, 22, 24, 26, 28, 36, 48, 72 };

            List<Test> tests = db.Tests.ToList();
            testsTree.ItemsSource = tests.ToList();


        }


        public TextRange In_teg(TextRange range)
        {
            range.Text = range.Text.Substring(0, range.Text.Length - 2);
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

                Test test = new Test(testName);
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
            if (dlg.ShowDialog() == true)
                try
                {
                    using (FileStream fileStream = new FileStream(dlg.FileName, FileMode.Create))
                    {
                        TextRange range = new TextRange(txtBox.Document.ContentStart, txtBox.Document.ContentEnd);
                        range.Save(fileStream, DataFormats.Rtf);
                        savePath = dlg.FileName;
                        fileStream.Dispose();
                    }
                }
                catch
                { MessageBox.Show("Файл виконується іншою програмою, спробуйте зберегти файл окремо"); }
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
                    TextRange range = new TextRange(txtBox.Document.ContentStart, txtBox.Document.ContentEnd);
                    Is_Save_As_Test(range);
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
                    txtBox.SelectAll();
                    txtBox.Cut();
                    Ans1.SelectAll();
                    Ans1.Cut();
                    Ans2.SelectAll();
                    Ans2.Cut();
                    Ans3.SelectAll();
                    Ans3.Cut();
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
                                txtBox.SelectAll();
                                txtBox.Cut();
                                Ans1.SelectAll();
                                Ans1.Cut();
                                Ans2.SelectAll();
                                Ans2.Cut();
                                Ans3.SelectAll();
                                Ans3.Cut();
                            }
                        }
                    }
                    catch { }
            }

        }

        private void Save_As_Test(object sender, RoutedEventArgs e)
        {
            TextRange range = new TextRange(txtBox.Document.ContentStart, txtBox.Document.ContentEnd);
            isSaveAsTest = true;
            isExport = false;
            Is_Save_As_Test(range);
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

                tvItem.Items.Add(item);
                question_id += 1;
            }
            test_id = temp;
        }
        private void Question_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                TreeViewItem tvItem = (TreeViewItem)sender;
                // получаю батьківський обєкт дерева
                var parent = tvItem.Parent;
                TreeViewItem parentTVI = (TreeViewItem)parent;

                // по заголовку знаходжу айді
                List<Test> tests = db.Tests.ToList();
                foreach (Test test in tests)
                {
                    if (parentTVI.Header.ToString() == test.Name)
                    {
                        test_id = test.test_id;
                    }
                }
                questions = db.Questions.Where(b => b.test_id == test_id).ToList();

                TextRange range = new TextRange(txtBox.Document.ContentStart, txtBox.Document.ContentEnd);

                range.Text = questions[(int)tvItem.Tag].Question_Text;
            }
            catch { }

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
