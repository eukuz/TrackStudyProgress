using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace TrackStudyProgress
{
    class ListEl
    {
        public Grid BaseGrid { private set; get; }

        TextBlock subjectNameTextBlock;
        TextBlock perDayTextBlock;
        StackPanel stackPanel;
        StackPanel tapPanel;
        ProgressBar progressBar;
        DateTime deadline;
        int perDay, amountOfTopics, numberOfDoneTopics; //per day should be updated daily
        string subjectName;

        public ListEl(double listBoxWidth, int amountOfTopics, DateTime deadline, string subjectName) : this(listBoxWidth, amountOfTopics, deadline, subjectName, 0) { }
        public ListEl(double listBoxWidth, int amountOfTopics, DateTime deadline, string subjectName, int numberOfDoneTopics)
        {
            this.amountOfTopics = amountOfTopics;
            this.deadline = deadline;
            this.subjectName = subjectName;
            this.numberOfDoneTopics = numberOfDoneTopics;

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
            subjectNameTextBlock.Text = subjectName;
            Grid.SetRow(subjectNameTextBlock, 0);
            Grid.SetColumn(subjectNameTextBlock, 0);
            BaseGrid.Children.Add(subjectNameTextBlock);

            perDayTextBlock = new TextBlock();
            perDayTextBlock.Text = "Per day: " + PerDay;
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
            stackPanel.Children.Add(progressBar = new ProgressBar { Value = numberOfDoneTopics, Width = listBoxWidth * 0.88, Maximum = amountOfTopics });
            stackPanel.Children.Add(increaseButton);
            Grid.SetRow(stackPanel, 1);
            Grid.SetColumn(stackPanel, 0);
            Grid.SetColumnSpan(stackPanel, 2);
            BaseGrid.Children.Add(stackPanel);
        }

        public int AmountOfTopics
        {
            get { return amountOfTopics; }
            set
            {
                amountOfTopics = value;
                UpdatePerDay();
            }
        }

        public DateTime Deadline
        {
            get { return deadline; }
            set
            {
                deadline = value;
                UpdatePerDay();
            }
        }
        public string SubjectName
        {
            get { return subjectName; }
            set
            {
                subjectName = value;
                subjectNameTextBlock.Text = value;
            }
        }
        public int PerDay
        {
            get
            {
                UpdatePerDay();
                return perDay;
            }
            set { perDay = value; }
        }
        private void UpdatePerDay()
        {
            TimeSpan time = deadline - DateTime.Now;
            perDay = (int)Math.Ceiling((decimal)(amountOfTopics - numberOfDoneTopics) / (time.Days));
            perDayTextBlock.Text = "Per day: " + perDay;
        }

        public int NumberOfDoneTopics
        {
            get { return numberOfDoneTopics; }
            set
            {
                if (value >= 0 && value <= amountOfTopics)
                {
                    progressBar.Value = value;
                    numberOfDoneTopics = value;
                    UpdatePerDay();
                }
            }
        }
        private void DecreaseButton_Click(object sender, RoutedEventArgs e) => NumberOfDoneTopics -= 1;
        private void IncreaseButton_Click(object sender, RoutedEventArgs e) => NumberOfDoneTopics += 1;
    }


}
