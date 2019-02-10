using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpic.Models
{
    public class ElementBounds : INotifyPropertyChanged
    {
        private Point _location;
        private Size _size;

        public ElementBounds(Point location, Size size)
        {
            Location = location;
            Size = size;
        }

        public Point Location { get => _location; set { _location = value; RaisePropertyChanged("Location"); } }
        public Size Size { get => _size; set { _size = value; RaisePropertyChanged("Size"); } }

        #region *** INotifyPropertyChanged Members and Invoker ***
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
