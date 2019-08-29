using Interpic.Alerts;
using Interpic.Models.Packaging;
using Interpic.Studio.Functional;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Shapes;

namespace Interpic.Studio.Windows.Developer
{
    /// <summary>
    /// Interaction logic for CreatePackage.xaml
    /// </summary>
    public partial class CreatePackage : Window
    {
        public List<PackageDefinition> Definitions { get; }
        public PackageManifest Manifest { get; set; }

        private ObservableCollection<string> contents = new ObservableCollection<string>();
        private ObservableCollection<string> dlls = new ObservableCollection<string>();

        public CreatePackage(List<PackageDefinition> definitions)
        {
            InitializeComponent();
            Definitions = definitions;
            cbbPackageType.ItemsSource = definitions;
            cbbPackageType.SelectedIndex = 0;
            lsbPackageContents.ItemsSource = contents;
            cbbMainDll.ItemsSource = dlls;
        }

        private void BtnCreate_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateManifest())
            {
                Manifest = new PackageManifest();
                Manifest.PackageName = tbName.Text;
                Manifest.Author = tbAuthor.Text;
                Manifest.Version = tbVersion.Text;
                Manifest.DllPath = cbbMainDll.SelectedValue.ToString();
                Manifest.Description = tbDescription.Text;

                PackageDefinition definition = Definitions.Find(def => def.Extension == cbbPackageType.SelectedValue.ToString());
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = $"{definition.Name} (*.{definition.Extension})";
                bool? result = saveFileDialog.ShowDialog();
                if (result.HasValue)
                {
                    if (result.Value == true)
                    {
                        if(Packages.Pack(contents.ToList(), saveFileDialog.FileName, Manifest))
                        {
                            DoneAlert.Show($"package created.\nLocation:\n{saveFileDialog.FileName}");
                        }
                    }
                }
            }
        }

        private bool ValidateManifest()
        {
            List<string> errors = new List<string>();
            if (string.IsNullOrWhiteSpace(tbName.Text))
            {
                errors.Add("Package name is required.");
            }

            if (string.IsNullOrWhiteSpace(cbbMainDll.SelectedValue.ToString()))
            {
                errors.Add("Main DLL is required, Please add a DLL to the package contents.");
            }

            if (string.IsNullOrWhiteSpace(tbVersion.Text))
            {
                errors.Add("Version is required.");
            }

            if (errors.Any())
            {
                WarningAlert.Show(string.Join("\n", errors));
                return false;
            }

            return true;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            bool? result = openFileDialog.ShowDialog();
            if(result.HasValue)
            {
                if (result.Value == true)
                {
                    contents.Add(openFileDialog.FileName);
                    if (System.IO.Path.GetExtension(openFileDialog.FileName).ToLower() == "dll")
                    {
                        dlls.Add(System.IO.Path.GetFileName(openFileDialog.FileName));
                        if (dlls.Count == 1)
                        {
                            cbbMainDll.SelectedIndex = 0;
                        }
                    }
                }
            }
        }

        private void BtnRemove_Click(object sender, RoutedEventArgs e)
        {
            string pathToRemove = lsbPackageContents.SelectedValue.ToString();
            if (System.IO.Path.GetExtension(pathToRemove).ToLower() == "dll")
            {
                dlls.Remove(System.IO.Path.GetFileName(pathToRemove));
            }
            contents.Remove(pathToRemove);
        }

        private void LsbPackageContents_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            btnRemove.IsEnabled = lsbPackageContents.SelectedItem != null;
        }
    }
}
