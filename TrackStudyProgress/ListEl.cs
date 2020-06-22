using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace TrackStudyProgress
{
    class ListEl
    {
        static readonly string[] congrats = { "Brilliant job!", "Outstanding work!", "This is truly above and beyond.", "This is superb!", "You set a high bar with this one.", "Good work, as always.", "Thanks for getting this done.", "Perfect!", "Wonderful, this is more than I expected.", "Well done!", "You consistently bring your all and I truly appreciate that.", "I am so proud of you!" };
        public Grid BaseGrid { private set; get; }

        TextBlock subjectNameTextBlock;
        TextBlock perDayTextBlock;
        TextBlock progressTextBlock;
        StackPanel stackPanel;
        ProgressBar progressBar;

        //MainWindow parent;

        public ListElData Data { get; private set; }

        public ListEl(/*MainWindow mainWindow,*/double listBoxWidth, int amountOfTopics, DateTime deadline, string subjectName, int numberOfDoneTopics)
        {
            //parent = mainWindow;
            Data = new ListElData(deadline, amountOfTopics, numberOfDoneTopics, subjectName)
            {
                deadline = deadline,
                subjectName = subjectName,
                amountOfTopics = amountOfTopics,
                numberOfDoneTopics = numberOfDoneTopics
            };
            Initialize(listBoxWidth);
        }
        public ListEl(/*MainWindow mainWindow,*/ double listBoxWidth, ListElData data)
        {
            //parent = mainWindow;
            Data = data;
            Initialize(listBoxWidth);
        }

        private void Initialize(double listBoxWidth)
        {
            BaseGrid = new Grid();

            Border border = new Border { BorderThickness = new Thickness(0, 0, 0, 0) };
            border.SetValue(Grid.ColumnProperty, 0);
            border.SetValue(Grid.RowProperty, 1);
            BaseGrid.Children.Add(border);

            BaseGrid.ColumnDefinitions.Add(new ColumnDefinition());
            BaseGrid.ColumnDefinitions.Add(new ColumnDefinition());
            BaseGrid.RowDefinitions.Add(new RowDefinition());
            BaseGrid.RowDefinitions.Add(new RowDefinition());

            subjectNameTextBlock = new TextBlock();
            subjectNameTextBlock.Text = Data.subjectName;
            Grid.SetRow(subjectNameTextBlock, 0);
            Grid.SetColumn(subjectNameTextBlock, 0);
            BaseGrid.Children.Add(subjectNameTextBlock);

            perDayTextBlock = new TextBlock();
            UpdatePerDay();
            Grid.SetRow(perDayTextBlock, 0);
            Grid.SetColumn(perDayTextBlock, 1);
            BaseGrid.Children.Add(perDayTextBlock);

            stackPanel = new StackPanel();
            stackPanel.Orientation = Orientation.Horizontal;

            Button increaseButton = new Button();
            increaseButton.Content = "+";
            increaseButton.Click += IncreaseButton_Click;
            increaseButton.Width = listBoxWidth * 0.035;
            increaseButton.Margin = new Thickness(listBoxWidth * 0.005, 0, listBoxWidth * 0.01, 0);

            Button decreaseButton = new Button();
            decreaseButton.Content = "-";
            decreaseButton.Click += DecreaseButton_Click;
            decreaseButton.Width = listBoxWidth * 0.035;
            decreaseButton.Margin = new Thickness(0, 0, listBoxWidth * 0.005, 0);

            stackPanel.Children.Add(decreaseButton);
            Grid progressGrid = new Grid();
            progressBar = new ProgressBar { Value = Data.numberOfDoneTopics, Width = listBoxWidth * 0.88, Maximum = Data.amountOfTopics };
            Grid.SetColumn(progressBar,0);
            Grid.SetRow(progressBar,0);
            progressTextBlock = new TextBlock();
            progressTextBlock.Text = $"{progressBar.Value}/{progressBar.Maximum}";
            progressTextBlock.HorizontalAlignment = HorizontalAlignment.Center;
            progressTextBlock.VerticalAlignment = VerticalAlignment.Center;
            progressTextBlock.Foreground = Brushes.Black;
            Grid.SetColumn(progressTextBlock, 0);
            Grid.SetRow(progressTextBlock, 0);
            progressGrid.Children.Add(progressBar);
            progressGrid.Children.Add(progressTextBlock);

            stackPanel.Children.Add(progressGrid);
            stackPanel.Children.Add(increaseButton);
            Grid.SetRow(stackPanel, 1);
            Grid.SetColumn(stackPanel, 0);
            Grid.SetColumnSpan(stackPanel, 2);
            BaseGrid.Children.Add(stackPanel);
        }


        public int AmountOfTopics
        {
            get { return Data.amountOfTopics; }
            set
            {
                Data.amountOfTopics = value;
                UpdatePerDay();
            }
        }

        public DateTime Deadline
        {
            get { return Data.deadline; }
            set
            {
                Data.deadline = value;
                UpdatePerDay();
            }
        }
        public string SubjectName
        {
            get { return Data.subjectName; }
            set
            {
                Data.subjectName = value;
                subjectNameTextBlock.Text = value;
            }
        }
        private void UpdatePerDay()
        {
            TimeSpan time = Data.deadline - DateTime.Now;
            if (time.Days > 0)
            {
                perDayTextBlock.Text = $"Per day: {(int)Math.Ceiling((decimal)(Data.amountOfTopics - Data.numberOfDoneTopics) / (time.Days))}";
            }
            else
            {
                perDayTextBlock.Text = "Deadline was missed!";
            }

        }

        public int NumberOfDoneTopics
        {
            get { return Data.numberOfDoneTopics; }
            set
            {
                if (value >= 0 && value <= Data.amountOfTopics)
                {
                    string oldData = Newtonsoft.Json.JsonConvert.SerializeObject(Data);
                    progressBar.Value = value;
                    progressTextBlock.Text = $"{progressBar.Value}/{progressBar.Maximum}";
                    Data.numberOfDoneTopics = value;

                    UpdatePerDay();
                    DBExecutor.ModifyDBEntry(oldData, Newtonsoft.Json.JsonConvert.SerializeObject(Data));
                }
            }
        }


        private void DecreaseButton_Click(object sender, RoutedEventArgs e) => NumberOfDoneTopics -= 1;
        private void IncreaseButton_Click(object sender, RoutedEventArgs e)
        {
            NumberOfDoneTopics += 1;
            if (NumberOfDoneTopics == AmountOfTopics)
            {
                Random random = new Random();
                MessageBoxResult res = MessageBox.Show(congrats[random.Next(congrats.Length)] + "\r\nDo you want to finish this subject?", "Congrats!", MessageBoxButton.YesNo);
                if (res == MessageBoxResult.Yes)
                {
                    ListBox listBox = BaseGrid.Parent as ListBox;
                    (((listBox.Parent as Grid).Parent as Grid).Parent as MainWindow).DeleteEl(listBox.Items.IndexOf((BaseGrid)));
                }

            }
        }
    }

    partial class ListElData
    {
        public DateTime deadline;
        public int amountOfTopics, numberOfDoneTopics; //per day should be updated daily
        public string subjectName;

        public ListElData(DateTime deadline, int amountOfTopics, int numberOfDoneTopics, string subjectName)
        {
            this.deadline = deadline;
            this.amountOfTopics = amountOfTopics;
            this.numberOfDoneTopics = numberOfDoneTopics;
            this.subjectName = subjectName;
        }


    }
}
