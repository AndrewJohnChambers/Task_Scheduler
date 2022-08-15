using System.Data.SqlClient;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System;
using System.Windows.Controls;

namespace Task_Scheduler
{
    internal class ViewModel : Model
    {
        public static void SetSelectedTask(MainWindow mw, Task si)
        {
            mw.currentlySelected = new Task
            {
                Id = si.Id,
                Name = si.Name,
                Description = si.Description,
                Due = si.Due,
                Stage = si.Stage
            };

            PopulateFieldsBySelectedTask(mw, si);
        }
        public static void SubmitToDB(MainWindow mw)
        {
            if (String.IsNullOrEmpty(mw.IdField.Text)) { InsertToDB(mw); return; }
            EditInDB(mw);
        }
        public static void InsertToDB(MainWindow mw)
        {
            SqlConnection con = new SqlConnection(sqlDbConnectionString);
            SqlDataAdapter adapter = new SqlDataAdapter();
            string sql = "insert into Tasks (Name,Description,Due,Stage) values('" + @GetNameFieldText(mw) + "','" + 
                                                                            @GetDescriptionFieldText(mw) + "','" + 
                                                                            @GetDueFieldText(mw) + "','" + 
                                                                            @GetStageFieldSelectedText(mw) + "')";
            con.Open();
            adapter.InsertCommand = new SqlCommand(sql, con);
            adapter.InsertCommand.ExecuteNonQuery();
            
            con.Close(); adapter.Dispose(); con.Dispose();
            DataSynce(mw); ClearFormFields(mw);
        }
        public static void EditInDB(MainWindow mw)
        {
            SqlConnection con = new SqlConnection(sqlDbConnectionString);
            SqlDataAdapter adapter = new SqlDataAdapter();
            string sql = $"update Tasks set Name='" + @GetNameFieldText(mw) + 
                                    "',Description='" + @GetDescriptionFieldText(mw) + 
                                    "',Due='" + @GetDueFieldText(mw) + 
                                    "',Stage='" + @GetStageFieldSelectedText(mw) + 
                                    "' where Id='" + @GetIdFieldText(mw) + "'";
            con.Open();
            adapter.UpdateCommand = new SqlCommand(sql, con);
            adapter.UpdateCommand.ExecuteNonQuery();
            
            con.Close(); adapter.Dispose(); con.Dispose();
            DataSynce(mw); ClearFormFields(mw);
        }
        public static void DeleteFromDBk(MainWindow mw)
        {
            SqlConnection con = new SqlConnection(sqlDbConnectionString);
            SqlDataAdapter adapter = new SqlDataAdapter();
            string sql = $"Delete Tasks where Id='" + @GetIdFieldText(mw) + "'";

            if (!String.IsNullOrEmpty(mw.IdField.Text))
            {
                con.Open();
                adapter.DeleteCommand = new SqlCommand(sql, con);
                adapter.DeleteCommand.ExecuteNonQuery();
                
                con.Close(); adapter.Dispose(); con.Dispose();
                DataSynce(mw); ClearFormFields(mw);
            }
        }
        public static void DataSynce(MainWindow mw)
        {
            SqlConnection con = new SqlConnection(sqlDbConnectionString);
            string sql = "Select Id,Name,Description,Due,Stage from Tasks";
            
            con.Open();
            SqlCommand command = new SqlCommand(sql, con);
            SqlDataReader dataReader = command.ExecuteReader();

            if (mw.dbTasks != null) { mw.dbTasks.Clear(); }

            while (dataReader.Read())
            {
                Task curTask = new Task
                {
                    Id = (int)dataReader.GetValue(0),
                    Name = dataReader.GetValue(1).ToString(),
                    Description = dataReader.GetValue(2).ToString(),
                    Due = dataReader.GetValue(3).ToString(),
                    Stage = dataReader.GetValue(4).ToString()
                };
                
                mw.dbTasks.Add(curTask);
            }
            
            con.Close(); con.Dispose(); ReportTaskDates(mw);
        }
        public static void PopulateStageField(MainWindow mw)
        {
            List<string> stages = GetTaskStageNames();
            
            foreach (string stage in stages)
            {
                mw.StageField.Items.Add(stage);
            }
            
            mw.StageField.SelectedIndex = 0;
        }
        public static void ReportTaskDates(MainWindow mw)
        {
            ObservableCollection<Task> obsColl = mw.dbTasks;
            mw.OverDueTasks.Items.Clear();
            mw.DueTasks.Items.Clear();

            foreach (Task task in obsColl)
            {
                if (!string.IsNullOrEmpty(task.Due.ToString()) 
                    && task.Stage.ToString() != TaskStates.OnHold.ToString()
                    && task.Stage.ToString() != TaskStates.Canceled.ToString()
                    && task.Stage.ToString() != TaskStates.Completed.ToString())
                {
                    string itemDate = task.Due.ToString();
                    DateTime parsedDate = DateTime.Parse(itemDate);
                    int days = (parsedDate.Date - DateTime.Now.Date).Days;

                    string reportText = task.Due.ToString() + " | " + task.Name + " | " + task.Description;

                    if (days == 0) { mw.DueTasks.Items.Add(reportText); }
                    else if (days < 0) { mw.OverDueTasks.Items.Add(reportText); }
                }
            }
        }
        public static void TextChangedEvent(MainWindow mw)
        {
            string curSearchPattern = mw.SearchField.Text.ToLower();

            if (string.IsNullOrEmpty(mw.SearchField.Text))
            {
                foreach (var item in mw.TasksField.Items)
                {
                    var row = mw.TasksField.ItemContainerGenerator.ContainerFromItem(item) as DataGridRow;
                    row.Visibility = Visibility.Visible;
                }
            }
            else
            {
                foreach (Task item in mw.TasksField.Items)
                {
                    string text = (item.Id + "|" + item.Name + "|" + item.Description + "|" + item.Due + "|" + item.Stage).ToLower();

                    var row = mw.TasksField.ItemContainerGenerator.ContainerFromItem(item) as DataGridRow;
                    if (text.Contains(curSearchPattern)) { row.Visibility = Visibility.Visible; }
                    else { row.Visibility = Visibility.Hidden; }
                }
            }
        }
        private static void PopulateFieldsBySelectedTask(MainWindow mw, Task task)
        {
            mw.IdField.Text = task.Id.ToString();
            mw.NameField.Text = task.Name.ToString();
            mw.DescriptionField.Text = task.Description.ToString();
            mw.DueField.Text = task.Due.ToString();
            mw.StageField.SelectedValue = task.Stage.ToString();
        }
        public static void ClearFormFields(MainWindow mw)
        {
            mw.IdField.Text = null;
            mw.NameField.Text = null;
            mw.DescriptionField.Text = null;
            mw.DueField.Text = null;
            mw.StageField.SelectedIndex = 0;
            mw.SearchField.Text = null;
            mw.OverDueTasks.Items.Clear();
            mw.DueTasks.Items.Clear();
            mw.currentlySelected = new Task();
            ReportTaskDates(mw);
        }
        public static string GetStageFieldSelectedText(MainWindow mw) 
        {
            if (mw.StageField.SelectedValue == null) 
            { 
                mw.StageField.SelectedValue = TaskStates.ToDo.ToString(); 
                mw.StageField.SelectedIndex = 0; 
            }
            return mw.StageField.SelectedValue.ToString(); 
        }
        public static string GetIdFieldText(MainWindow mw) { return mw.IdField.Text; }
        public static string GetNameFieldText(MainWindow mw) { return mw.NameField.Text; }
        public static string GetDescriptionFieldText(MainWindow mw) { return mw.DescriptionField.Text; }
        public static string GetDueFieldText(MainWindow mw) { return mw.DueField.Text; }
    }
}
