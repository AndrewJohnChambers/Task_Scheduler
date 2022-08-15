using static Task_Scheduler.ViewModel;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows;
using System;

namespace Task_Scheduler
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Initialize();
        }

        private MainWindow mw = (MainWindow)Application.Current.MainWindow;
        public ObservableCollection<Task> dbTasks = new ObservableCollection<Task>();
        public Task currentlySelected = new Task();

        private void TasksField_SelectionChanged(object sender, SelectionChangedEventArgs e) 
        {
            Task si = (Task)TasksField.SelectedItem;
            if (si != null) { SetSelectedTask(mw, si); } 
        }

        private void DeleteButton_Click(object sender, EventArgs e) { DeleteFromDBk(mw); }
        private void ClearButton_Click(object sender, EventArgs e) { ClearFormFields(mw); }
        private void SubmitButton_Click(object sender, EventArgs e) { SubmitToDB(mw); }
        private void TextChangedEventHandler(object sender, EventArgs e) { TextChangedEvent(mw); }
        private void DataBinding() { TasksField.ItemsSource = dbTasks; }
        private void Initialize()
        {
            mw = (MainWindow)Application.Current.MainWindow;
            DataBinding();
            PopulateStageField(mw);
            DataSynce(mw);
            ReportTaskDates(mw);
        }
    }
}
