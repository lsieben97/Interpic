using Interpic.Models.EventArgs;
using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace Interpic.Models
{
    public class MenuItem
    {
        public MenuItem(string name)
        {
            Name = name;
        }

        public MenuItem(string name, ImageSource icon) : this(name)
        {
            Icon = icon;
        }

        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; }
        public string Description { get; set; }
        public ImageSource Icon { get; set; }
        public List<MenuItem> SubItems { get; set; } = new List<MenuItem>();

        public System.Windows.Controls.MenuItem Control { get; set; }

        public event OnMenuItemClicked Clicked;

        public void FireClickedEvent(object source, ProjectStateEventArgs eventArgs)
        {
                Clicked?.Invoke(source, eventArgs);
        }
    }
}
