namespace Interpic.Settings
{
    public class PathSetting : Setting<string>
    {
        private PathType _type;
        private PathOperation _operation;
        private string _startPath;
        private string _extension;
        private string _dialogTitle;
        private string _startFileName;
        private string _filter;

        public PathType Type { get => _type; set { _type = value; RaisePropertyChanged("Type"); } }
        public PathOperation Operation { get => _operation; set { _operation = value; RaisePropertyChanged("Operation"); } }
        public string StartPath { get => _startPath; set { _startPath = value; RaisePropertyChanged("StartPath"); } }
        public string Extension { get => _extension; set { _extension = value; RaisePropertyChanged("Extension"); } }
        public string DialogTitle { get => _dialogTitle; set { _dialogTitle = value; RaisePropertyChanged("DialogTitle"); } }
        public string StartFileName { get => _startFileName; set { _startFileName = value; RaisePropertyChanged("StartFileName"); } }
        public string Filter { get => _filter; set { _filter = value; RaisePropertyChanged("Filter"); } }

        public enum PathType
        {
            File,
            Folder
        }

        public enum PathOperation
        {
            Save,
            Load
        }
    }
}