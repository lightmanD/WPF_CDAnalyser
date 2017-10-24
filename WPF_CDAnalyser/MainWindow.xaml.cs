using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.ComponentModel;
using System;
using System.Diagnostics;

namespace WPF_CDAnalyser
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string[] _filesPaths;
        MainBody MainBody;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
           
            OpenFileDialog openFileDialog2 = new OpenFileDialog();
            openFileDialog2.Filter = "msr files (*.msr)|*.msr|All files (*.*)|*.*";

            openFileDialog2.Multiselect = true;
            openFileDialog2.FilterIndex = 2;
            openFileDialog2.RestoreDirectory = true;

            if (openFileDialog2.ShowDialog() == true)       
            {
                try
                {
                    _filesPaths = openFileDialog2.FileNames;
                    
                    int counter = 0;
                    foreach (var elem in _filesPaths)
                    {
                        string[] split = elem.Split('\\');
                        string temp = counter.ToString() + " | " + split[split.Length - 1].ToString() + "\n";

                        rTextBoxOutput.AppendText(temp); 
                       
                        counter++;
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message.ToString());
                }
            }

            //Основное рабочее тело
            MainBody = new MainBody(_filesPaths);
            MainBody.mainAnalysis();

            //
        }



        private void RichTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {//exit button
            this.Close();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {//config open button

            Process.Start(Directory.GetCurrentDirectory());
        }
    }
}
