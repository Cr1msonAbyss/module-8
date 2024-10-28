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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ToDoList
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public class Task
        {
            public string Name { get; set; }
            public DateTime Date { get; set; }
            public bool IsCompleted { get; set; }

            public Task(string name, DateTime date)
            {
                Name = name;
                Date = date;
                IsCompleted = false;
            }

            public override string ToString()
            {
                return $"{Name} ({Date.ToString("d")}){(IsCompleted ? " - выполнено" : "")}";
            }
        }

        private List<Task> tasks = new List<Task>();

        private void AddTask_Click(object sender, RoutedEventArgs e)
        {
            if (TaskDescription.Text == "" || TaskDescription.Text == " ")
            {
                MessageBox.Show("Введите текст задачи");
            }
            else
            {
                var description = TaskDescription.Text;
                DateTime today = DateTime.Today;
                if (string.IsNullOrWhiteSpace(description)) return;
                var task = new Task(description, today);
                tasks.Add(task);
                UpdateItemList();
                TaskDescription.Clear();
            }
        }

        private void ShowTaskInfo_Click(object sender, RoutedEventArgs e)
        {
            if (TaskList.SelectedIndex >= 0)
            {
                var selectedTask = (Task)TaskList.SelectedItem;
                selectedTask.IsCompleted = true;
                UpdateItemList();
            }
            else
            {
                MessageBox.Show("Выберите задачу для пометки");
            }
        }

        private void DeleteTask_Click(object sender, RoutedEventArgs e)
        {
            if (TaskList.SelectedIndex >= 0)
            {
                var selectedTask = (Task)TaskList.SelectedItem;
                tasks.Remove(selectedTask);
                UpdateItemList();
            }
            else 
            {
                MessageBox.Show("Выберите задачу для удаления");
            }
        }

        private void UpdateItemList(List<Task> filteredItems = null)
        {
            TaskList.Items.Clear();
            var itemsToDisplay = filteredItems ?? tasks;
            foreach (var item in itemsToDisplay)
            {
                TaskList.Items.Add(item);
            }
        }

        private void FilterSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = FilterSelector.SelectedItem as ComboBoxItem;
            if (selectedItem == null) return;
            switch (selectedItem.Tag.ToString())
            {
                case "All":
                    UpdateItemList();
                    KeywordTextBox.Visibility = Visibility.Collapsed;
                    break;
                case "Nedavnie":
                    var recentItems = tasks.Where(item => item.Date >= DateTime.Now.AddDays(-2)).ToList();
                    UpdateItemList(recentItems);
                    KeywordTextBox.Visibility = Visibility.Collapsed;
                    break;
                case "KeyWord":
                    KeywordTextBox.Visibility = Visibility.Visible;
                    KeywordTextBox.TextChanged -= KeywordTextBox_TextChanged;
                    KeywordTextBox.TextChanged += KeywordTextBox_TextChanged;
                    break;
                case "Alphabet":
                    var sortedItems = tasks.OrderBy(item => item.Name).ToList();
                    UpdateItemList(sortedItems);
                    KeywordTextBox.Visibility = Visibility.Collapsed;
                    break;
            }
        }

        private void KeywordTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var keyword = KeywordTextBox.Text.ToLower();
            var filteredItems = tasks.Where(item => item.Name.ToLower().Contains(keyword)).ToList();
            UpdateItemList(filteredItems);
        }
        private void TaskDescription_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateWatermarkVisibility();
        }

        private void TaskDescription_GotFocus(object sender, RoutedEventArgs e)
        {
            UpdateWatermarkVisibility();
        }

        private void TaskDescription_LostFocus(object sender, RoutedEventArgs e)
        {
            UpdateWatermarkVisibility();
        }

        private void UpdateWatermarkVisibility()
        {
            if (string.IsNullOrWhiteSpace(TaskDescription.Text) && !TaskDescription.IsFocused)
            {
                TaskDescriptionWatermark.Visibility = Visibility.Visible;
            }
            else
            {
                TaskDescriptionWatermark.Visibility = Visibility.Collapsed;

            }
        }


     
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var filePath = "tasks.txt";
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                foreach (var task in tasks)
                {
               
                    writer.WriteLine($"{task.Name}|{task.Date.ToString("o")}|{task.IsCompleted}");
                }
            }
            MessageBox.Show("Список задач успешно сохранен.", "Сохранение", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            var filePath = "tasks.txt";
            if (!File.Exists(filePath))
            {
                MessageBox.Show("Файл со списком задач не найден.", "Загрузка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            tasks.Clear();
            using (StreamReader reader = new StreamReader(filePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
       
                    var parts = line.Split('|');
                    if (parts.Length == 3)
                    {
                        var name = parts[0];
                        var date = DateTime.Parse(parts[1]);
                        var isCompleted = bool.Parse(parts[2]);

             
                        tasks.Add(new Task(name, date) { IsCompleted = isCompleted });
                    }
                }
            }
            UpdateItemList();
            MessageBox.Show("Список задач успешно загружен.", "Загрузка", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}



