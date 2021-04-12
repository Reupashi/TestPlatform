using Microsoft.Win32;
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
    /// Interaction logic for TestSolve.xaml 
    /// </summary>
    public partial class TestSolve : Window
    {
        ApplicationContext db;
        string savePath;
        string question;
        bool isExport = false;
        bool isSaveAsTest = false;

        public TestSolve()
        {
            InitializeComponent();
            db = new ApplicationContext(); //Виділення памяті і створення силки database

            cmbFontFamily.ItemsSource = Fonts.SystemFontFamilies.OrderBy(f => f.Source);
            cmbFontSize.ItemsSource = new List<double>() { 8, 9, 10, 11, 12, 14, 16, 18, 20, 22, 24, 26, 28, 36, 48, 72 };
        }


        public TextRange In_teg (TextRange range) 
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
            question = range.Text;
            Test test = new Test(In_teg(range).Text);
            db.Tests.Add(test);
            db.SaveChanges();
            isSaveAsTest = true;
            isExport = false;
        }

        #region editing_buttons

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
                    {
                        using (FileStream fileStream = new FileStream(dlg.FileName, FileMode.Create))
                        {

                            TextRange range = new TextRange(txtBox.Document.ContentStart, txtBox.Document.ContentEnd);
                            range.Save(fileStream, DataFormats.Rtf);

                            savePath = dlg.FileName;
                            fileStream.Dispose();

                        }
                    }
                }
                catch
                {
                    MessageBox.Show("Файл виконується іншою програмою, спробуйте зберегти файл окремо");
                }
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            try
            {
                FileStream fileStream = new FileStream(savePath, FileMode.Open);
                TextRange range = new TextRange(txtBox.Document.ContentStart, txtBox.Document.ContentEnd);
                try
                {
                    if (isExport == true && isSaveAsTest == false)
                    {
                        Is_Export(range, fileStream);
                    }
                    else if (isExport == false && isSaveAsTest == true)
                    {
                        Is_Save_As_Test(range);
                    }
                    else
                    {
                        range.Save(fileStream, DataFormats.Rtf);
                        fileStream.Dispose();
                    }
                }
                catch { MessageBox.Show("Something goes vrong"); }

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
            catch {
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
                    catch{}
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
            try {
                txtBox.Selection.ApplyPropertyValue(Inline.FontSizeProperty, cmbFontSize.Text);
                }
            catch { }
            }

        #endregion



        private void txtBox_TextChanged(object sender, TextChangedEventArgs e)
        {
        }


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
            txtBox_Copy.Text = string.Format("Виділено : \"{0}\" ",

             txtBox.Selection.Text);

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

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
