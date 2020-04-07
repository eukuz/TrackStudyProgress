using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TrackStudyProgress
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<ListEl> listOfListEls = new List<ListEl>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void AddListElement(int amountOfTopics, DateTime deadline, string subjectName, int numberOfDoneTopics)
        {
            ListEl listEl = new ListEl(listBox.ActualWidth, amountOfTopics, deadline, subjectName, numberOfDoneTopics);
            listOfListEls.Add(listEl);
            listBox.Items.Add(listEl.BaseGrid);

            DBExecutor.WriteToDB(JsonConvert.SerializeObject(listEl.Data));

        }
        private void addNewElButton_Click(object sender, RoutedEventArgs e)
        {
            CreateUpdateWindow window = new CreateUpdateWindow();
            window.Title = "Create";
            window.ShowDialog();
            window.Owner = this;
            if (window.DialogResult == true)
            {
                AddListElement(amountOfTopics: Convert.ToInt32(window.amountOfTopicsTextBlock.Text),
                    deadline: window.deadlineDatePicker.SelectedDate ?? DateTime.Now, subjectName: window.subjectTextBlock.Text,
                    Convert.ToInt32(window.numberOfDoneTopicsTextBlock.Text));
            }
        }

        private void modifyElButton_Click(object sender, RoutedEventArgs e)
        {
            if (listBox.SelectedItem == null)
            {
                MessageBox.Show("Please select a subject from the list");
                return;
            }

            ListEl listEl = listOfListEls[listBox.SelectedIndex];

            CreateUpdateWindow window = new CreateUpdateWindow();

            window.Title = "Modify";
            window.subjectTextBlock.Text = listEl.SubjectName;
            window.deadlineDatePicker.SelectedDate = listEl.Deadline;
            window.amountOfTopicsTextBlock.Text = listEl.AmountOfTopics.ToString();
            window.numberOfDoneTopicsTextBlock.Text = listEl.NumberOfDoneTopics.ToString();
            window.ShowDialog();

            window.Owner = this;
            if (window.DialogResult == true)
            {
                string oldData = JsonConvert.SerializeObject(listEl.Data);
                
                listEl.SubjectName = window.subjectTextBlock.Text;
                listEl.Deadline = window.deadlineDatePicker.SelectedDate ?? DateTime.Now;
                listEl.AmountOfTopics = Convert.ToInt32(window.amountOfTopicsTextBlock.Text);
                listEl.NumberOfDoneTopics = Convert.ToInt32(window.numberOfDoneTopicsTextBlock.Text);

                DBExecutor.ModifyDBEntry(oldData, JsonConvert.SerializeObject(listEl.Data));
            }
        }
        private void deleteElButton_Click(object sender, RoutedEventArgs e)
        {
            if (listBox.SelectedItem == null)
            {
                MessageBox.Show("Please select a subject from the list");
            }
            else
            {
                int index = listBox.SelectedIndex;
                MessageBoxResult res = MessageBox.Show("Are you sure want to delete " + listOfListEls[index].SubjectName, "Sure?", MessageBoxButton.YesNo);
                if (res == MessageBoxResult.Yes)
                {
                    listOfListEls.RemoveAt(index);
                    listBox.Items.RemoveAt(index);
                    DBExecutor.DeleteDBEntry(JsonConvert.SerializeObject(listOfListEls[index].Data));
                }
            }
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DBExecutor.FillTheListFromDB(ref listBox, ref listOfListEls);
        }
        private void readElButton_Click(object sender, RoutedEventArgs e) //Remove In Final version
        {
            List<string> lines = DBExecutor.ReadFromDB();
        }
    }
}
