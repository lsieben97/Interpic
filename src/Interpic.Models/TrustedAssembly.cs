using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpic.Models
{
    public class TrustedAssembly : INotifyPropertyChanged
    {
        private string _path;
        private string _checksum;
        private bool inPackage;
        private string packagePath;

        public string Path { get => _path; set { _path = value; RaisePropertyChanged("Path"); } }
        public string Checksum { get => _checksum; set { _checksum = value; RaisePropertyChanged("Checksum"); } }
        public bool InPackage { get => inPackage; set { inPackage = value; RaisePropertyChanged("InPackage"); } }
        public string PackagePath { get => packagePath; set { packagePath = value; RaisePropertyChanged("PackagePath"); } }

        #region *** INotifyPropertyChanged Members and Invoker ***
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
