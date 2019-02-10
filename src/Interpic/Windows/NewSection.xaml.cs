using Interpic.Alerts;
using Interpic.Extensions;
using Interpic.Models;
using Interpic.Utils;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace Interpic.Studio.Windows
{
    /// <summary>
    /// Interaction logic for NewPage.xaml
    /// </summary>
    public partial class NewSection : Window
    {
        public Models.Section Section { get; set; }
        private SectionIdentifier identifier; 
        private bool edit = false;
        private Models.Page page;
        public ISectionIdentifierSelector sectionIdentifierSelector { get; set; }

        public NewSection(Models.Page page)
        {
            this.page = page;
            InitializeComponent();
        }

        public NewSection(Models.Section section)
        {
            InitializeComponent();
            Section = section;
            lbTitle.Text = "Edit Section";
            btnCreate.Content = "Save";
            edit = true;
            tbName.Text = Section.Name;
            sectionIdentifierSelector.SectionIdentifier = Section.SectionIdentifier;
            tbBaseNode.Text = section.SectionIdentifier.Name;
        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            bool valid = true;
            if (tbName.Text == string.Empty)
            {
                valid = false;
                lbNameError.Text = "Enter name";
            }
            else
            {
                lbNameError.Text = string.Empty;
            }

            if (valid == true)
            {
                if (!edit)
                {
                    Section = new Models.Section();
                }
                Section.Name = tbName.Text;
                Section.SectionIdentifier = sectionIdentifierSelector.SectionIdentifier;
                Section.SectionIdentifier = identifier;
                Close();
            }
        }

        private void btnSelectNode_Click(object sender, RoutedEventArgs e)
        {
            sectionIdentifierSelector.Page = page;
            sectionIdentifierSelector.ShowSelector();
            if (sectionIdentifierSelector.SectionIdentifier != null)
            {
                identifier = sectionIdentifierSelector.SectionIdentifier;
                tbBaseNode.Text = sectionIdentifierSelector.SectionIdentifier.Name;
            }
        }
    }
}
