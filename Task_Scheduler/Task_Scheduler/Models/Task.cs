using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Task_Scheduler
{
    public class Task : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private int _id;
        private string _name;
        private string _description;
        private string _due;
        private string _stage;

        public int Id { get { return _id; } set { _id = value; OnPropertyChanged(); } }
        public string Name { get { return _name; } set { _name = value; OnPropertyChanged(); } }
        public string Description { get {return _description; } set { _description = value;  OnPropertyChanged(); } }
        public string Due { get { return _due; } set { _due = value; OnPropertyChanged(); } }
        public string Stage { get { return _stage; } set { _stage = value; OnPropertyChanged(); } }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name)); }
    }
}
