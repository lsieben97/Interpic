using Interpic.Alerts;
using Interpic.AsyncTasks;
using Interpic.Extensions;
using Interpic.Models;
using Interpic.Models.EventArgs;
using Interpic.Settings;
using Interpic.Studio.Functional;
using Interpic.Studio.Tasks;
using Interpic.Studio.Windows;
using Interpic.Studio.Windows.Developer;
using Interpic.Studio.Windows.Selectors;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using ThomasJaworski.ComponentModel;

namespace Interpic.Studio
{
    /// <summary>
    /// Interaction logic for Studio.xaml
    /// </summary>
    public partial class Studio : Window, IStudioEnvironment
    {
        internal static List<IProjectTypeProvider> AvailableProjectTypes = new List<IProjectTypeProvider>();
        internal static List<IProjectBuilder> AvailableBuilders = new List<IProjectBuilder>();
        private DispatcherTimer checkTimer = new DispatcherTimer();
        private Models.Page currentPage;
        private Models.Section currentSection;
        private Models.Version currentVersion;
        private bool openingNewProject;

        #region Events
        public event OnStudioStartup StudioStartup;
        public event OnProjectLoaded ProjectLoaded;
        public event OnProjectSettingsOpening ProjectSettingsOpening;
        public event OnProjectSettingsOpened ProjectSettingsOpened;
        public event OnPageSettingsOpening PageSettingsOpening;
        public event OnPageSettingsOpened PageSettingsOpened;
        public event OnSectionSettingsOpening SectionSettingsOpening;
        public event OnSectionSettingsOpened SectionSettingsOpened;
        public event OnControlSettingsOpening ControlSettingsOpening;
        public event OnControlSettingsOpened ControlSettingsOpened;
        public event OnStudioShutdown StudioShutdown;
        public event OnProjectUnloaded ProjectUnloaded;
        public event OnProjectCreated ProjectCreated;
        public event OnGlobalSettingsSaved GlobalSettingsSaved;
        #endregion

        public Project CurrentProject { get; set; }
        internal IProjectTypeProvider ProjectTypeProvider { get; set; }

        internal IProjectBuilder ProjectBuilder { get; set; }

        private ChangeListener projectChangeListener;
        public ILogger Logger => App.ApplicationLogger;

        public Studio(Models.Project project)
        {
            openingNewProject = false;
            InitializeComponent();
            if (AvailableProjectTypes == null)
            {
                AvailableProjectTypes = new List<IProjectTypeProvider>();
                AvailableProjectTypes.Add(new Web.WebProjectTypeProvider());
            }

            CurrentProject = project;

            InitializeProjectTypeProvider(project);
            InitializeProjectBuilder(project);

            App.InitializeLogger(App.GlobalSettings.GetPathSetting("logDirectory") + "\\last.log", this);

            StudioStartup?.Invoke(this as IStudioEnvironment, new InterpicStudioEventArgs(this));

            InitializeObjectModel(project);

            CheckDeveloperMode();
            BindEvents();
        }

        private void InitializeProjectBuilder(Project project)
        {
            if (AvailableBuilders.Any((type) => type.GetBuilderId() == CurrentProject.OutputType))
            {
                ProjectBuilder = AvailableBuilders.Single((type) => type.GetBuilderId() == CurrentProject.OutputType);
            }
            else
            {
                ErrorAlert.Show("No suitable builder found for this project.\nClosing project.");
                Close();
                new Splash().Show();
            }
        }

        private void BindEvents()
        {
            GlobalSettingsSaved += (object sender, GlobalSettingsEventArgs e) => CheckDeveloperMode();
        }

        private void CheckDeveloperMode()
        {
            if (App.GlobalSettings.GetBooleanSetting("EnableDeveloperMode") == true)
            {
                miDeveloper.Visibility = Visibility.Visible;
            }
            else
            {
                miDeveloper.Visibility = Visibility.Collapsed;
            }
        }

        private void InitializeNewProject()
        {
            currentVersion = CurrentProject.Versions[0];
            CurrentProject.LastViewedVersionId = CurrentProject.Versions[0].Id;
            if (CurrentProject.HasSettingsAvailable)
            {
                if (App.GlobalSettings.GetBooleanSetting("ShowInfoForSettings"))
                {
                    InfoAlert.Show("The manual settings dialog will now be shown to further configure the project.");
                }

                SettingsEditor editor = new SettingsEditor(CurrentProject.Settings);
                editor.ShowDialog();
                if (editor.DialogResult.HasValue)
                {
                    if (editor.DialogResult.Value == true)
                    {
                        CurrentProject.Settings = editor.SettingsCollection;
                    }
                    ProjectCreated?.Invoke(this as IStudioEnvironment, new InterpicStudioEventArgs(this));
                }
            }
        }

        private void InitializeUI()
        {
            SetStatusBar("Project loaded.");
            lbLastSaved.Text = "Last saved: " + CurrentProject.LastSaved.ToShortDateString() + " " + CurrentProject.LastSaved.ToLongTimeString();

            checkTimer.Interval = new TimeSpan(0, 0, 1);
            checkTimer.Tick += CheckTimer_Tick;
            checkTimer.Start();
        }

        private void InitializeProjectTypeProvider(Project project)
        {
            LoadTypeProviderForCurrentProject();
            ProjectTypeProvider.Studio = this;
            if (ProjectTypeProvider.GetDefaultProjectSettings() != null && project.Settings == null)
            {
                project.Settings = ProjectTypeProvider.GetDefaultProjectSettings();
            }
            ProjectTypeProvider.TypeProviderConnected();
        }

        private void InitializeObjectModel(Project project)
        {
            foreach (Models.Version version in CurrentProject.Versions)
            {
                version.Parent = CurrentProject;
                foreach (Models.Page page in version.Pages)
                {
                    page.Parent = version;
                    foreach (Models.Section section in page.Sections)
                    {
                        section.Parent = page;
                        foreach (Models.Control control in section.Controls)
                        {
                            control.Parent = section;
                        }
                    }
                }
            }

            projectChangeListener = ChangeListener.Create(CurrentProject);
            projectChangeListener.PropertyChanged += ProjectChangeListener_PropertyChanged;
            projectChangeListener.CollectionChanged += ProjectChangeListener_CollectionChanged;
            ProjectLoaded?.Invoke(this as IStudioEnvironment, new ProjectLoadedEventArgs(this, CurrentProject));
        }

        private void ProjectChangeListener_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            InternalProjectChanged(sender.ToString());
        }

        private void ProjectChangeListener_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            InternalProjectChanged(e.PropertyName);
        }

        private void InternalProjectChanged(string property)
        {
            CurrentProject.Changed = true;
            imgChangeIndicator.Visibility = Visibility.Visible;
            RedrawTreeView();
        }

        private void LoadTypeProviderForCurrentProject()
        {
            if (AvailableProjectTypes.Any((type) => type.GetProjectTypeId() == CurrentProject.TypeProviderId))
            {
                ProjectTypeProvider = AvailableProjectTypes.Single((type) => type.GetProjectTypeId() == CurrentProject.TypeProviderId);
            }
            else
            {
                ErrorAlert.Show("No suitable type provider found for this project.\nClosing project.");
                Close();
                new Splash().Show();
            }
        }

        private void CheckTimer_Tick(object sender, EventArgs e)
        {
            if (CurrentProject.Changed == true && CurrentProject.LastSaved.AddMinutes(App.GlobalSettings.GetNumeralSetting("SaveWarningOffset")) < DateTime.Now)
            {
                lbLastSaved.Foreground = new SolidColorBrush(Color.FromRgb(176, 0, 32));
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (CurrentProject.IsNew)
            {
                InitializeNewProject();
            }
            currentVersion = CurrentProject.Versions.Single(version => version.Id == CurrentProject.LastViewedVersionId);
            currentVersion.IsCurrent = true;
            spVersions.ItemsSource = CurrentProject.Versions;
            RedrawTreeView();
            (tvManualTree.Items[0] as TreeViewItem).IsSelected = true;
            InitializeUI();
        }

        public void RedrawTreeView()
        {
            string previousPath = tvManualTree.SelectedValuePath;
            if (tvManualTree.Items.Count > 0)
            {
                tvManualTree.Items.RemoveAt(0);
            }
            tvManualTree.Items.Add(Projects.GetTreeViewForProject(CurrentProject, currentVersion));
            foreach (object item in tvManualTree.Items)
            {
                TreeViewItem treeItem = tvManualTree.ItemContainerGenerator.ContainerFromItem(item) as TreeViewItem;
                if (treeItem != null)
                    ExpandAll(treeItem, true);
                treeItem.IsExpanded = true;
            }
            tvManualTree.SelectedValuePath = previousPath;

        }

        internal void ReselectPage(Models.Page page)
        {
            page.TreeViewItem.IsSelected = false;
            page.TreeViewItem.IsSelected = true;
        }

        private void miExtensions_Click(object sender, RoutedEventArgs e)
        {
            Project project = CurrentProject;
            new ExtensionManager(ref project).ShowDialog();
            CurrentProject = project;
        }

        private void miAddPage_Click(object sender, RoutedEventArgs e)
        {
            AddPage();
        }

        private void AddPage()
        {
            NewPage dialog = new NewPage();
            dialog.ShowDialog();
            if (dialog.Page != null)
            {
                if (ProjectTypeProvider.GetDefaultPageSettings() != null)
                {
                    dialog.Page.Settings = ProjectTypeProvider.GetDefaultPageSettings();
                    if (App.GlobalSettings.GetBooleanSetting("ShowInfoForSettings"))
                    {
                        InfoAlert.Show("The page settings dialog will now be shown to further configure the page.");
                    }
                    SettingsEditor editor = new SettingsEditor(dialog.Page.Settings);
                    editor.ShowDialog();
                    if (editor.DialogResult.HasValue)
                    {
                        if (editor.DialogResult.Value == true)
                        {
                            dialog.Page.Settings = editor.SettingsCollection;
                        }
                    }
                }
                // TODO: this is ugly, find another way. before V1.0
                Project currentProject = CurrentProject;
                Models.Page currentPage = dialog.Page;
                string document = ProjectTypeProvider.GetSourceProvider().GetSource(ref currentProject, ref currentPage);
                CurrentProject = currentProject;
                dialog.Page = currentPage;
                if (document != null)
                {
                    dialog.Page.Source = document;
                    dialog.Page.Parent = currentVersion;

                    (Models.Page page, bool succes) result = ProjectTypeProvider.RefreshPage(dialog.Page, currentVersion, CurrentProject);
                    if (result.succes)
                    {
                        if (result.page.Screenshot != null)
                        {
                            currentVersion.Pages.Add(dialog.Page);
                            RedrawTreeView();
                            dialog.Page.TreeViewItem.IsSelected = true;
                        }
                        else
                        {
                            SetStatusBar("page not added because refreshing failed.");
                        }
                    }
                }

            }
        }

        private void ExpandAll(ItemsControl items, bool expand)
        {
            foreach (object obj in items.Items)
            {
                ItemsControl childControl = items.ItemContainerGenerator.ContainerFromItem(obj) as ItemsControl;
                if (childControl != null)
                {
                    ExpandAll(childControl, expand);
                }
                TreeViewItem item = childControl as TreeViewItem;
                if (item != null)
                    item.IsExpanded = true;
            }
        }

        private void tvManualTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (tvManualTree.SelectedItem != null)
            {
                object selection = (tvManualTree.SelectedItem as TreeViewItem).Tag;


                if (selection.GetType() == typeof(Models.Page))
                {
                    Models.Page page = selection as Models.Page;
                    currentPage = page;
                    if (page.Type == "text")
                    {
                        RenderTextPage(page);
                    }
                    else if (page.Type == "reference")
                    {
                        RenderTOCForPage(page);
                    }
                }
                else if (selection.GetType() == typeof(Section))
                {
                    currentSection = selection as Section;
                    currentPage = ((Section)selection).Parent;
                    RenderSection(selection as Section);
                }
                else if (selection.GetType() == typeof(Models.Control))
                {
                    currentSection = ((Models.Control)selection).Parent;
                    RenderControlPage(selection as Models.Control);
                }
                else if (selection.GetType() == typeof(Project))
                {
                    RenderTOC(CurrentProject);
                }
            }
            else
            {
                spWorkspace.Children.Clear();
            }
        }

        private void RenderSection(Models.Section section)
        {
            spWorkspace.Children.Clear();
            // stack panel for title and icon
            StackPanel titlePanel = new StackPanel();
            titlePanel.HorizontalAlignment = HorizontalAlignment.Stretch;
            titlePanel.Margin = new Thickness(0, 0, 0, 0);
            titlePanel.Orientation = Orientation.Horizontal;
            titlePanel.Width = double.NaN;
            titlePanel.Height = double.NaN;

            // Icon
            Image image = new Image();
            image.Source = new BitmapImage(new Uri("/Interpic.UI;component/Icons/SectionWhite.png", UriKind.RelativeOrAbsolute));
            image.Width = 48;
            image.Height = 48;
            image.Margin = new Thickness(12, -1, 0, 0);
            titlePanel.Children.Add(image);

            // title
            TextBlock titleTextBlock = new TextBlock();
            titleTextBlock.FontFamily = new System.Windows.Media.FontFamily("Verdana");
            titleTextBlock.FontSize = 24;
            titleTextBlock.Width = double.NaN;
            titleTextBlock.Margin = new Thickness(5, 10, 0, 0);
            titleTextBlock.HorizontalAlignment = HorizontalAlignment.Left;
            titleTextBlock.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            titleTextBlock.Text = section.Name;
            titlePanel.Children.Add(titleTextBlock);

            spWorkspace.Children.Add(titlePanel);

            TextBlock descriptionTextBlock = new TextBlock();
            descriptionTextBlock.FontFamily = new System.Windows.Media.FontFamily("Verdana");
            descriptionTextBlock.FontSize = 14;
            descriptionTextBlock.Width = double.NaN;
            descriptionTextBlock.Margin = new Thickness(12, 5, 0, 0);
            descriptionTextBlock.HorizontalAlignment = HorizontalAlignment.Left;
            descriptionTextBlock.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            descriptionTextBlock.Text = "Page description";
            spWorkspace.Children.Add(descriptionTextBlock);

            // textbox for description 
            TextBox descriptionTextBox = new TextBox();
            descriptionTextBox.Margin = new Thickness(12, 5, 12, 12);
            descriptionTextBox.HorizontalAlignment = HorizontalAlignment.Stretch;
            descriptionTextBox.VerticalAlignment = VerticalAlignment.Stretch;
            descriptionTextBox.Height = 75;
            descriptionTextBox.AcceptsReturn = true;
            descriptionTextBox.AcceptsTab = true;
            descriptionTextBox.Tag = section;
            descriptionTextBox.Style = Application.Current.Resources["TextBoxStyle"] as Style;
            descriptionTextBox.FontFamily = new System.Windows.Media.FontFamily("Verdana");
            descriptionTextBox.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            descriptionTextBox.Text = section.Description;
            descriptionTextBox.TextChanged += PageDescriptionTextAreaChanged;
            spWorkspace.Children.Add(descriptionTextBox);

            Button saveButton = new Button();
            saveButton.Margin = new Thickness(12, 5, 12, 12);
            saveButton.Tag = (section, descriptionTextBox);
            saveButton.Style = Application.Current.Resources["ButtonStyle"] as Style;
            saveButton.Content = "Save";
            saveButton.Click += SectionSaveButton_Click;
            spWorkspace.Children.Add(saveButton);

            foreach (Models.Control control in section.Controls)
            {
                // stack panel for edit and remove buttons
                StackPanel pagePanel = new StackPanel();
                pagePanel.HorizontalAlignment = HorizontalAlignment.Stretch;
                pagePanel.Margin = new Thickness(0, 0, 0, 0);
                pagePanel.Orientation = Orientation.Vertical;
                pagePanel.Width = double.NaN;
                pagePanel.Height = double.NaN;
                spWorkspace.Children.Add(pagePanel);

                // line between sections
                Separator seperator2 = new Separator();
                seperator2.HorizontalAlignment = HorizontalAlignment.Stretch;
                seperator2.Width = double.NaN;
                seperator2.Height = double.NaN;
                seperator2.Margin = new Thickness(6, 6, 6, 3);
                pagePanel.Children.Add(seperator2);

                // title
                TextBlock pageTitleTextBlock = new TextBlock();
                pageTitleTextBlock.FontFamily = new System.Windows.Media.FontFamily("Verdana");
                pageTitleTextBlock.FontSize = 20;
                pageTitleTextBlock.Width = double.NaN;
                pageTitleTextBlock.Margin = new Thickness(12, 12, 0, 0);
                pageTitleTextBlock.HorizontalAlignment = HorizontalAlignment.Left;
                VerticalAlignment = VerticalAlignment.Center;
                pageTitleTextBlock.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                pageTitleTextBlock.Text = control.Name;
                pagePanel.Children.Add(pageTitleTextBlock);

                // stack panel for edit and remove buttons
                StackPanel panel = new StackPanel();
                panel.HorizontalAlignment = HorizontalAlignment.Right;
                panel.VerticalAlignment = VerticalAlignment.Top;
                panel.Margin = new Thickness(0, -12, 6, 0);
                panel.Orientation = Orientation.Horizontal;
                panel.Width = double.NaN;
                panel.Height = double.NaN;
                pagePanel.Children.Add(panel);

                // remove button
                Button removeButton = new Button();
                removeButton.Style = Application.Current.Resources["ButtonStyle"] as Style;
                removeButton.Content = "Remove";
                removeButton.FontFamily = new System.Windows.Media.FontFamily("Verdana");
                removeButton.HorizontalAlignment = HorizontalAlignment.Right;
                removeButton.VerticalAlignment = VerticalAlignment.Top;
                removeButton.Margin = new Thickness(0, 0, 6, 0);
                removeButton.Tag = control;
                removeButton.Click += TOCRemoveControlButton_Click;
                panel.Children.Add(removeButton);

                // edit button
                Button editButton = new Button();
                editButton.Style = Application.Current.Resources["ButtonStyle"] as Style;
                editButton.Content = "Edit";
                editButton.FontFamily = new System.Windows.Media.FontFamily("Verdana");
                editButton.HorizontalAlignment = HorizontalAlignment.Right;
                editButton.VerticalAlignment = VerticalAlignment.Top;
                editButton.Margin = new Thickness(0, 0, 6, 0);
                editButton.Tag = control;
                editButton.Click += TOCEditControlButton_Click;
                panel.Children.Add(editButton);

                // edit button
                //Button viewButton = new Button();
                //viewButton.Style = Application.Current.Resources["ButtonStyle"] as Style;
                //viewButton.Content = "View";
                //viewButton.FontFamily = new System.Windows.Media.FontFamily("Verdana");
                //viewButton.HorizontalAlignment = HorizontalAlignment.Right;
                //viewButton.VerticalAlignment = VerticalAlignment.Top;
                //viewButton.Margin = new Thickness(0, 0, 12, 0);
                //viewButton.Tag = control;
                //viewButton.Click += TOCViewControlButton_Click;
                //panel.Children.Add(viewButton);
            }
        }

        private void TOCEditControlButton_Click(object sender, RoutedEventArgs e)
        {
            Models.Control control = (e.Source as Button).Tag as Models.Control;
            new AddControl(control, ProjectTypeProvider.GetControlSelector());
        }

        private void TOCRemoveControlButton_Click(object sender, RoutedEventArgs e)
        {
            Models.Control control = (e.Source as Button).Tag as Models.Control;
            ConfirmAlert confirm = ConfirmAlert.Show("'" + control.Name + "' will be removed.");
            if (confirm.Result == true)
            {
                control.Parent.TreeViewItem.IsSelected = true;
                control.Parent.TreeViewItem.BringIntoView();
                control.Parent.Controls.Remove(control);
            }
        }

        internal void RemovePage(Models.Page page)
        {
            currentVersion.Pages.Remove(page);
            page.TreeViewItem.IsSelected = false;
            tvManualTree.Items.Remove(page.TreeViewItem);
            RedrawTreeView();
            (tvManualTree.Items.GetItemAt(0) as TreeViewItem).IsSelected = true;
        }

        private void RenderTOC(Project project)
        {
            spWorkspace.Children.Clear();

            // stack panel for title and icon
            StackPanel titlePanel = new StackPanel();
            titlePanel.HorizontalAlignment = HorizontalAlignment.Stretch;
            titlePanel.Margin = new Thickness(0, 0, 0, 0);
            titlePanel.Orientation = Orientation.Horizontal;
            titlePanel.Width = double.NaN;
            titlePanel.Height = double.NaN;

            // Icon
            Image image = new Image();
            image.Source = new BitmapImage(new Uri("/Interpic.UI;component/Icons/ProjectWhite.png", UriKind.RelativeOrAbsolute));
            image.Width = 48;
            image.Height = 48;
            image.Margin = new Thickness(12, -1, 0, 0);
            titlePanel.Children.Add(image);

            // title
            TextBlock titleTextBlock = new TextBlock();
            titleTextBlock.FontFamily = new System.Windows.Media.FontFamily("Verdana");
            titleTextBlock.FontSize = 24;
            titleTextBlock.Width = double.NaN;
            titleTextBlock.Margin = new Thickness(5, 10, 0, 0);
            titleTextBlock.HorizontalAlignment = HorizontalAlignment.Left;
            titleTextBlock.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            titleTextBlock.Text = project.Name;
            titlePanel.Children.Add(titleTextBlock);

            spWorkspace.Children.Add(titlePanel);
            foreach (Models.Page page in currentVersion.Pages)
            {


                // stack panel for edit and remove buttons
                StackPanel pagePanel = new StackPanel();
                pagePanel.HorizontalAlignment = HorizontalAlignment.Stretch;
                pagePanel.Margin = new Thickness(0, 0, 0, 0);
                pagePanel.Orientation = Orientation.Vertical;
                pagePanel.Width = double.NaN;
                pagePanel.Height = double.NaN;
                spWorkspace.Children.Add(pagePanel);

                // line between pages
                Separator seperator = new Separator();
                seperator.HorizontalAlignment = HorizontalAlignment.Stretch;
                seperator.Width = double.NaN;
                seperator.Height = double.NaN;
                seperator.Margin = new Thickness(6, 6, 6, 3);
                pagePanel.Children.Add(seperator);

                // title
                TextBlock pageTitleTextBlock = new TextBlock();
                pageTitleTextBlock.FontFamily = new System.Windows.Media.FontFamily("Verdana");
                pageTitleTextBlock.FontSize = 20;
                pageTitleTextBlock.Width = double.NaN;
                pageTitleTextBlock.Margin = new Thickness(12, 12, 0, 0);
                pageTitleTextBlock.HorizontalAlignment = HorizontalAlignment.Left;
                VerticalAlignment = VerticalAlignment.Center;
                pageTitleTextBlock.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                pageTitleTextBlock.Text = page.Name;
                pagePanel.Children.Add(pageTitleTextBlock);

                // stack panel for edit and remove buttons
                StackPanel panel = new StackPanel();
                panel.HorizontalAlignment = HorizontalAlignment.Right;
                panel.VerticalAlignment = VerticalAlignment.Top;
                panel.Margin = new Thickness(0, -12, 6, 0);
                panel.Orientation = Orientation.Horizontal;
                panel.Width = double.NaN;
                panel.Height = double.NaN;
                pagePanel.Children.Add(panel);

                // remove button
                Button removeButton = new Button();
                removeButton.Style = Application.Current.Resources["ButtonStyle"] as Style;
                removeButton.Content = "Remove";
                removeButton.FontFamily = new System.Windows.Media.FontFamily("Verdana");
                removeButton.HorizontalAlignment = HorizontalAlignment.Right;
                removeButton.VerticalAlignment = VerticalAlignment.Top;
                removeButton.Margin = new Thickness(0, 0, 6, 0);
                removeButton.Tag = page;
                removeButton.Click += TOCRemoveButton_Click;
                panel.Children.Add(removeButton);

                // edit button
                Button editButton = new Button();
                editButton.Style = Application.Current.Resources["ButtonStyle"] as Style;
                editButton.Content = "Edit";
                editButton.FontFamily = new System.Windows.Media.FontFamily("Verdana");
                editButton.HorizontalAlignment = HorizontalAlignment.Right;
                editButton.VerticalAlignment = VerticalAlignment.Top;
                editButton.Margin = new Thickness(0, 0, 6, 0);
                editButton.Tag = page;
                editButton.Click += TOCEditButton_Click;
                panel.Children.Add(editButton);

                // edit button
                Button viewButton = new Button();
                viewButton.Style = Application.Current.Resources["ButtonStyle"] as Style;
                viewButton.Content = "View";
                viewButton.FontFamily = new System.Windows.Media.FontFamily("Verdana");
                viewButton.HorizontalAlignment = HorizontalAlignment.Right;
                viewButton.VerticalAlignment = VerticalAlignment.Top;
                viewButton.Margin = new Thickness(0, 0, 12, 0);
                viewButton.Tag = page;
                viewButton.Click += TOCViewButton_Click;
                panel.Children.Add(viewButton);
            }
        }

        private void RenderTOCForPage(Models.Page page)
        {
            spWorkspace.Children.Clear();
            // stack panel for title and icon
            StackPanel titlePanel = new StackPanel();
            titlePanel.HorizontalAlignment = HorizontalAlignment.Stretch;
            titlePanel.Margin = new Thickness(0, 0, 0, 0);
            titlePanel.Orientation = Orientation.Horizontal;
            titlePanel.Width = double.NaN;
            titlePanel.Height = double.NaN;

            // Icon
            Image image = new Image();
            image.Source = new BitmapImage(new Uri("/Interpic.UI;component/Icons/PageWhite.png", UriKind.RelativeOrAbsolute));
            image.Width = 48;
            image.Height = 48;
            image.Margin = new Thickness(12, -1, 0, 0);
            titlePanel.Children.Add(image);

            // title
            TextBlock titleTextBlock = new TextBlock();
            titleTextBlock.FontFamily = new System.Windows.Media.FontFamily("Verdana");
            titleTextBlock.FontSize = 24;
            titleTextBlock.Width = double.NaN;
            titleTextBlock.Margin = new Thickness(5, 10, 0, 0);
            titleTextBlock.HorizontalAlignment = HorizontalAlignment.Left;
            titleTextBlock.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            titleTextBlock.Text = page.Name;
            titlePanel.Children.Add(titleTextBlock);

            spWorkspace.Children.Add(titlePanel);

            TextBlock descriptionTextBlock = new TextBlock();
            descriptionTextBlock.FontFamily = new System.Windows.Media.FontFamily("Verdana");
            descriptionTextBlock.FontSize = 14;
            descriptionTextBlock.Width = double.NaN;
            descriptionTextBlock.Margin = new Thickness(12, 5, 0, 0);
            descriptionTextBlock.HorizontalAlignment = HorizontalAlignment.Left;
            descriptionTextBlock.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            descriptionTextBlock.Text = "Page description";
            spWorkspace.Children.Add(descriptionTextBlock);

            // textbox for description 
            TextBox descriptionTextBox = new TextBox();
            descriptionTextBox.Margin = new Thickness(12, 5, 12, 12);
            descriptionTextBox.HorizontalAlignment = HorizontalAlignment.Stretch;
            descriptionTextBox.VerticalAlignment = VerticalAlignment.Stretch;
            descriptionTextBox.Height = 75;
            descriptionTextBox.AcceptsReturn = true;
            descriptionTextBox.AcceptsTab = true;
            descriptionTextBox.Tag = page;
            descriptionTextBox.Style = Application.Current.Resources["TextBoxStyle"] as Style;
            descriptionTextBox.FontFamily = new System.Windows.Media.FontFamily("Verdana");
            descriptionTextBox.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            descriptionTextBox.TextChanged += PageDescriptionTextAreaChanged;
            descriptionTextBox.Text = page.Description;
            spWorkspace.Children.Add(descriptionTextBox);

            Button saveButton = new Button();
            saveButton.Margin = new Thickness(12, 5, 12, 12);
            saveButton.Tag = (page, descriptionTextBox);
            saveButton.Style = Application.Current.Resources["ButtonStyle"] as Style;
            saveButton.Content = "Save";
            saveButton.Click += PageSaveButton_Click;
            spWorkspace.Children.Add(saveButton);

            // line between sections
            Separator seperator = new Separator();
            seperator.HorizontalAlignment = HorizontalAlignment.Stretch;
            seperator.Width = double.NaN;
            seperator.Height = double.NaN;
            seperator.Margin = new Thickness(6, 6, 6, 3);
            spWorkspace.Children.Add(seperator);

            foreach (Models.Section section in page.Sections)
            {
                // stack panel for edit and remove buttons
                StackPanel pagePanel = new StackPanel();
                pagePanel.HorizontalAlignment = HorizontalAlignment.Stretch;
                pagePanel.Margin = new Thickness(0, 0, 0, 0);
                pagePanel.Orientation = Orientation.Vertical;
                pagePanel.Width = double.NaN;
                pagePanel.Height = double.NaN;
                spWorkspace.Children.Add(pagePanel);

                // line between sections
                Separator seperator2 = new Separator();
                seperator2.HorizontalAlignment = HorizontalAlignment.Stretch;
                seperator2.Width = double.NaN;
                seperator2.Height = double.NaN;
                seperator2.Margin = new Thickness(6, 6, 6, 3);
                pagePanel.Children.Add(seperator2);

                // title
                TextBlock pageTitleTextBlock = new TextBlock();
                pageTitleTextBlock.FontFamily = new System.Windows.Media.FontFamily("Verdana");
                pageTitleTextBlock.FontSize = 20;
                pageTitleTextBlock.Width = double.NaN;
                pageTitleTextBlock.Margin = new Thickness(12, 12, 0, 0);
                pageTitleTextBlock.HorizontalAlignment = HorizontalAlignment.Left;
                VerticalAlignment = VerticalAlignment.Center;
                pageTitleTextBlock.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                pageTitleTextBlock.Text = section.Name;
                pagePanel.Children.Add(pageTitleTextBlock);

                // stack panel for edit and remove buttons
                StackPanel panel = new StackPanel();
                panel.HorizontalAlignment = HorizontalAlignment.Right;
                panel.VerticalAlignment = VerticalAlignment.Top;
                panel.Margin = new Thickness(0, -12, 6, 0);
                panel.Orientation = Orientation.Horizontal;
                panel.Width = double.NaN;
                panel.Height = double.NaN;
                pagePanel.Children.Add(panel);

                // remove button
                Button removeButton = new Button();
                removeButton.Style = Application.Current.Resources["ButtonStyle"] as Style;
                removeButton.Content = "Remove";
                removeButton.FontFamily = new System.Windows.Media.FontFamily("Verdana");
                removeButton.HorizontalAlignment = HorizontalAlignment.Right;
                removeButton.VerticalAlignment = VerticalAlignment.Top;
                removeButton.Margin = new Thickness(0, 0, 6, 0);
                removeButton.Tag = section;
                removeButton.Click += TOCRemoveSectionButton_Click;
                panel.Children.Add(removeButton);

                // edit button
                Button editButton = new Button();
                editButton.Style = Application.Current.Resources["ButtonStyle"] as Style;
                editButton.Content = "Edit";
                editButton.FontFamily = new System.Windows.Media.FontFamily("Verdana");
                editButton.HorizontalAlignment = HorizontalAlignment.Right;
                editButton.VerticalAlignment = VerticalAlignment.Top;
                editButton.Margin = new Thickness(0, 0, 6, 0);
                editButton.Tag = section;
                editButton.Click += TOCEditSectionButton_Click;
                panel.Children.Add(editButton);

                // edit button
                Button viewButton = new Button();
                viewButton.Style = Application.Current.Resources["ButtonStyle"] as Style;
                viewButton.Content = "View";
                viewButton.FontFamily = new System.Windows.Media.FontFamily("Verdana");
                viewButton.HorizontalAlignment = HorizontalAlignment.Right;
                viewButton.VerticalAlignment = VerticalAlignment.Top;
                viewButton.Margin = new Thickness(0, 0, 12, 0);
                viewButton.Tag = section;
                viewButton.Click += TOCViewSectionButton_Click;
                panel.Children.Add(viewButton);



            }
        }

        private void RenderControlPage(Models.Control control)
        {
            spWorkspace.Children.Clear();
            // stack panel for title and icon
            StackPanel titlePanel = new StackPanel();
            titlePanel.HorizontalAlignment = HorizontalAlignment.Stretch;
            titlePanel.Margin = new Thickness(0, 0, 0, 0);
            titlePanel.Orientation = Orientation.Horizontal;
            titlePanel.Width = double.NaN;
            titlePanel.Height = double.NaN;

            // Icon
            Image image = new Image();
            image.Source = new BitmapImage(new Uri("/Interpic.UI;component/Icons/ControlWhite.png", UriKind.RelativeOrAbsolute));
            image.Width = 48;
            image.Height = 48;
            image.Margin = new Thickness(12, -1, 0, 0);
            titlePanel.Children.Add(image);

            // title
            TextBlock titleTextBlock = new TextBlock();
            titleTextBlock.FontFamily = new System.Windows.Media.FontFamily("Verdana");
            titleTextBlock.FontSize = 24;
            titleTextBlock.Width = double.NaN;
            titleTextBlock.Margin = new Thickness(5, 10, 0, 0);
            titleTextBlock.HorizontalAlignment = HorizontalAlignment.Left;
            titleTextBlock.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            titleTextBlock.Text = control.Name;
            titlePanel.Children.Add(titleTextBlock);

            spWorkspace.Children.Add(titlePanel);

            TextBlock descriptionTextBlock = new TextBlock();
            descriptionTextBlock.FontFamily = new System.Windows.Media.FontFamily("Verdana");
            descriptionTextBlock.FontSize = 14;
            descriptionTextBlock.Width = double.NaN;
            descriptionTextBlock.Margin = new Thickness(12, 5, 0, 0);
            descriptionTextBlock.HorizontalAlignment = HorizontalAlignment.Left;
            descriptionTextBlock.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            descriptionTextBlock.Text = "Page description";
            spWorkspace.Children.Add(descriptionTextBlock);

            // textbox for description 
            TextBox descriptionTextBox = new TextBox();
            descriptionTextBox.Margin = new Thickness(12, 5, 12, 12);
            descriptionTextBox.HorizontalAlignment = HorizontalAlignment.Stretch;
            descriptionTextBox.VerticalAlignment = VerticalAlignment.Stretch;
            descriptionTextBox.Height = 75;
            descriptionTextBox.AcceptsReturn = true;
            descriptionTextBox.AcceptsTab = true;
            descriptionTextBox.Tag = control;
            descriptionTextBox.Style = Application.Current.Resources["TextBoxStyle"] as Style;
            descriptionTextBox.FontFamily = new System.Windows.Media.FontFamily("Verdana");
            descriptionTextBox.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            descriptionTextBox.TextChanged += PageDescriptionTextAreaChanged;
            descriptionTextBox.Text = control.Description;
            spWorkspace.Children.Add(descriptionTextBox);

            Button saveButton = new Button();
            saveButton.Margin = new Thickness(12, 5, 12, 12);
            saveButton.Tag = (control, descriptionTextBox);
            saveButton.Style = Application.Current.Resources["ButtonStyle"] as Style;
            saveButton.Content = "Save";
            saveButton.Click += ControlSaveButton_Click;
            spWorkspace.Children.Add(saveButton);

            // line between sections
            Separator seperator = new Separator();
            seperator.HorizontalAlignment = HorizontalAlignment.Stretch;
            seperator.Width = double.NaN;
            seperator.Height = double.NaN;
            seperator.Margin = new Thickness(6, 6, 6, 3);
            spWorkspace.Children.Add(seperator);
        }

        private void PageSaveButton_Click(object sender, RoutedEventArgs e)
        {
            (Models.Page page, TextBox textbox) controls = ((Models.Page page, TextBox textbox))(e.Source as Button).Tag;
            currentVersion.Pages[currentVersion.Pages.IndexOf(controls.page)].Description = controls.textbox.Text;
            RenderTOCForPage(controls.page);
            SetStatusBar("Page description saved.");
        }

        private void SectionSaveButton_Click(object sender, RoutedEventArgs e)
        {
            (Models.Section section, TextBox textbox) controls = ((Models.Section section, TextBox textbox))(e.Source as Button).Tag;
            currentPage.Sections[currentPage.Sections.IndexOf(controls.section)].Description = controls.textbox.Text;
            RenderSection(controls.section);
            SetStatusBar("Section description saved.");
        }

        private void ControlSaveButton_Click(object sender, RoutedEventArgs e)
        {
            (Models.Control control, TextBox textbox) controls = ((Models.Control section, TextBox textbox))(e.Source as Button).Tag;
            currentSection.Controls[currentSection.Controls.IndexOf(controls.control)].Description = controls.textbox.Text;
            RenderControlPage(controls.control);
            SetStatusBar("Control description saved.");
        }

        private void PageDescriptionTextAreaChanged(object sender, TextChangedEventArgs e)
        {
            //Models.Page page = (e.Source as TextBox).Tag as Models.Page;
            //page.Description = (e.Source as TextBox).Text;
        }

        private void TOCRemoveSectionButton_Click(object sender, RoutedEventArgs e)
        {
            Models.Section section = (e.Source as Button).Tag as Models.Section;
            ConfirmAlert confirm = ConfirmAlert.Show("'" + section.Name + "' will be removed.");
            if (confirm.Result == true)
            {
                section.Parent.Sections.Remove(section);
            }
        }

        private void TOCEditSectionButton_Click(object sender, RoutedEventArgs e)
        {
            Models.Section section = (e.Source as Button).Tag as Models.Section;
            NewSection newSection = new NewSection(section);
            newSection.sectionIdentifierSelector = ProjectTypeProvider.GetSectionSelector();
            newSection.ShowDialog();
        }

        private void TOCViewSectionButton_Click(object sender, RoutedEventArgs e)
        {
            Models.Section section = (e.Source as Button).Tag as Models.Section;
            section.TreeViewItem.IsSelected = true;
            section.TreeViewItem.BringIntoView();
        }

        private void TOCViewButton_Click(object sender, RoutedEventArgs e)
        {
            Models.Page page = (e.Source as Button).Tag as Models.Page;
            page.TreeViewItem.IsSelected = true;
        }

        private void TOCEditButton_Click(object sender, RoutedEventArgs e)
        {
            Models.Page page = (e.Source as Button).Tag as Models.Page;
            NewPage editPage = new NewPage(page);
            editPage.ShowDialog();
            RedrawTreeView();
            (tvManualTree.Items[0] as TreeViewItem).IsSelected = true;
        }

        private void TOCRemoveButton_Click(object sender, RoutedEventArgs e)
        {
            Models.Page page = (e.Source as Button).Tag as Models.Page;
            ConfirmAlert alert = ConfirmAlert.Show("'" + page.Name + "' will be removed.");
            if (alert.Result == true)
            {
                RemovePage(page);
            }
        }

        private void RenderTextPage(Models.Page page)
        {
            spWorkspace.Children.Clear();
            spWorkspace.Children.Add(Pages.RenderTextPage(page));
        }

        private void miSave_Click(object sender, RoutedEventArgs e)
        {
            SaveProjectTask task = new SaveProjectTask(CurrentProject);
            ProcessTaskDialog dialog = new ProcessTaskDialog(task, "Saving...");
            dialog.ShowDialog();
            if (!dialog.TaskToExecute.IsCanceled)
            {
                SetStatusBar("Project saved.");
                lbLastSaved.Text = "Last saved: " + CurrentProject.LastSaved.ToShortDateString() + " " + CurrentProject.LastSaved.ToLongTimeString();
                lbLastSaved.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                imgChangeIndicator.Visibility = Visibility.Hidden;
            }

        }

        public void SetStatusBar(string message)
        {
            lbStatusBar.Text = message;
        }

        private void miGlobalSettings_Click(object sender, RoutedEventArgs e)
        {
            SettingsEditor editor = new SettingsEditor(App.GlobalSettings);
            editor.ShowDialog();
            if (editor.DialogResult.Value == true)
            {
                App.GlobalSettings = editor.SettingsCollection;
                App.SaveGlobalSettings();
                SetStatusBar("Global settings saved.");
                GlobalSettingsSaved?.Invoke(this, new GlobalSettingsEventArgs(this, App.GlobalSettings));
            }
            else
            {
                SetStatusBar("Changes in global settings canceled.");
            }

        }

        public List<string> GetLoadedExtensions()
        {
            List<string> extensionNames = new List<string>();
            foreach (Extensions.Extension extension in Functional.Extensions.LoadedExtensions)
            {
                extensionNames.Add(extension.GetName());
            }
            return extensionNames;
        }

        public string GetDataFolderForExtension(string extension)
        {
            string directory = App.EXECUTABLE_DIRECTORY + "\\" + "Extensions\\" + extension;
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            return directory;
        }

        public string GetStudioDirectory()
        {
            return App.EXECUTABLE_DIRECTORY;
        }

        public SaveResult PromptProjectSave(string extensionName)
        {
            SaveResult result = new SaveResult();
            ConfirmAlert alert = new ConfirmAlert(extensionName + " Wants you to save your project.");
            alert.ShowDialog();
            if (alert.Result)
            {
                result.PromptAccepted = true;
                result.Saved = Projects.SaveProject(CurrentProject);
            }
            return result;
        }

        public string GetStudioVersion()
        {
            return App.VERSION;
        }

        private void miOpen_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentProject.Changed)
            {
                if (ConfirmProjectClose())
                {
                    LoadNewProject();
                }
            }
            else
            {
                LoadNewProject();
            }
        }

        private void LoadNewProject()
        {
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.Title = "Select a location";
            dialog.Filter = "Interpic Project files (.ipp)|*.ipp";

            bool? result = dialog.ShowDialog();

            if (result.Value == true)
            {

                LoadProjectTask loadTask = new LoadProjectTask(dialog.FileName);
                ProcessTaskDialog taskDialog = new ProcessTaskDialog(loadTask, "Loading...");
                taskDialog.ShowDialog();
                if (!taskDialog.TaskToExecute.IsCanceled)
                {
                    ProjectUnloaded?.Invoke(this as IStudioEnvironment, new InterpicStudioEventArgs(this));
                    UnloadCurrentProject();
                    openingNewProject = true;
                    checkTimer.Stop();
                    new Studio(loadTask.Project).Show();
                    Close();
                }
            }
        }

        private void miProjectSettings_Click(object sender, RoutedEventArgs e)
        {
            ProjectSettingsOpening?.Invoke(this, new ProjectSettingsEventArgs(this, CurrentProject, CurrentProject.Settings));
            SettingsEditor editor = new SettingsEditor(CurrentProject.Settings);
            editor.ShowDialog();
            if (editor.DialogResult.Value == true)
            {
                CurrentProject.Settings = editor.SettingsCollection;
                SetStatusBar("Project settings saved.");
            }
            else
            {
                SetStatusBar("Changes in project settings canceled.");
            }
            ProjectSettingsOpened?.Invoke(this, new ProjectSettingsEventArgs(this, CurrentProject, CurrentProject.Settings));
        }

        private void DisplayControlHint(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (((FrameworkElement)sender).Tag != null)
            {
                SetStatusBar(((FrameworkElement)sender).Tag.ToString());
            }
        }

        private void miAbout_Click(object sender, RoutedEventArgs e)
        {
            new About().ShowDialog();
        }

        private void miNewProject_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentProject.Changed)
            {
                if (ConfirmProjectClose())
                {
                    NewProject dialog = new NewProject();
                    dialog.ShowDialog();
                    if (dialog.Project != null)
                    {
                        ProjectUnloaded?.Invoke(this as IStudioEnvironment, new InterpicStudioEventArgs(this));
                        UnloadCurrentProject();
                        openingNewProject = true;
                        checkTimer.Stop();
                        new Studio(dialog.Project).Show();
                        Close();

                    }
                }
            }
            else
            {
                NewProject dialog = new NewProject();
                dialog.ShowDialog();
                if (dialog.Project != null)
                {
                    ProjectUnloaded?.Invoke(this as IStudioEnvironment, new InterpicStudioEventArgs(this));
                    UnloadCurrentProject();
                    openingNewProject = true;
                    checkTimer.Stop();
                    new Studio(dialog.Project).Show();
                    Close();
                }
            }
        }

        private void UnloadCurrentProject()
        {
            CurrentProject = null;
            currentPage = null;
            currentSection = null;
        }

        private bool ConfirmProjectClose()
        {
            WarningAlert alert = new WarningAlert(CurrentProject.Name + " has unsaved changes.\nThe project will now be closed.");
            alert.ShowDialog();
            return alert.Result;
        }

        private void miNewPage_Click(object sender, RoutedEventArgs e)
        {
            AddPage();
        }

        private void miNewSection_Click(object sender, RoutedEventArgs e)
        {
            SelectPage selector = new SelectPage(currentVersion);
            if (selector.ShowDialog().Value)
            {
                Models.Page selectedpage = currentVersion.Pages.Single(Page => Page.Id == selector.SelectedPageId);
                AddSection(ref selectedpage);
            }
        }

        private void miSaveAs_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();
            dialog.Description = "Select a new project folder.";

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                SaveAsNewProjectTask task = new SaveAsNewProjectTask(CurrentProject, dialog.SelectedPath);
                ProcessTaskDialog saveDialog = new ProcessTaskDialog(task, "Saving...");
                saveDialog.ShowDialog();
                if (!saveDialog.TaskToExecute.IsCanceled)
                {
                    SetStatusBar("Project saved.");
                }
            }
        }

        private void miExit_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentProject.Changed)
            {
                if (ConfirmProjectClose())
                {
                    ProjectUnloaded?.Invoke(this as IStudioEnvironment, new InterpicStudioEventArgs(this));
                    UnloadCurrentProject();
                    // todo: unload global extensions.
                    StudioShutdown?.Invoke(this as IStudioEnvironment, new InterpicStudioEventArgs(this));
                    Close();
                }
            }

        }

        private void miBuildProject_Click(object sender, RoutedEventArgs e)
        {
            new Build(ProjectBuilder, CurrentProject).ShowDialog();
        }

        private void miBuildPage_Click(object sender, RoutedEventArgs e)
        {
            new Build(ProjectBuilder, CurrentProject, currentVersion).ShowDialog();
        }

        private void miAddSection_Click(object sender, RoutedEventArgs e)
        {
            if (currentPage != null)
            {
                AddSection(ref currentPage);
            }
            else
            {
                ErrorAlert.Show("No page selected.\nSelect a page from the manual tree on the left.");
            }
        }

        private void AddSection(ref Models.Page page)
        {
            NewSection dialog = new NewSection(page);
            dialog.sectionIdentifierSelector = ProjectTypeProvider.GetSectionSelector();
            dialog.ShowDialog();
            if (dialog.Section != null)
            {
                dialog.Section.Parent = page;


                if (ProjectTypeProvider.GetControlFinder() != null)
                {
                    ObservableCollection<DiscoveredControl> foundControls = ProjectTypeProvider.GetControlFinder().FindControls(dialog.Section);
                    if (foundControls == null)
                    {
                        foundControls = new ObservableCollection<DiscoveredControl>();
                    }

                    dialog.Section.DiscoveredControls = foundControls;
                    SetStatusBar("Found " + foundControls.Count.ToString() + " controls.");
                    (Section section, bool succes) result = ProjectTypeProvider.RefreshSection(dialog.Section, dialog.Section.Parent, currentVersion, CurrentProject);
                    if (result.succes)
                    {
                        if (result.section.ElementBounds != null)
                        {
                            page.Sections.Add(dialog.Section);
                            RedrawTreeView();
                            dialog.Section.TreeViewItem.IsSelected = true;
                            dialog.Section.TreeViewItem.BringIntoView();
                        }
                        else
                        {
                            SetStatusBar("Section not added because refreshing failed.");
                        }
                    }
                }
                else
                {
                    SetStatusBar("No control finder found for this project type, skipping control discovery.");
                    (Section section, bool succes) result = ProjectTypeProvider.RefreshSection(dialog.Section, dialog.Section.Parent, currentVersion, CurrentProject);
                    if (result.succes)
                    {
                        page.Sections.Add(dialog.Section);
                    }
                    else
                    {
                        SetStatusBar("Section not added because refreshing failed.");
                    }
                }
            }
        }

        private void miAddControl_Click(object sender, RoutedEventArgs e)
        {
            if (currentSection != null)
            {
                AddControl(ref currentSection);
            }
            else
            {
                ErrorAlert.Show("No page selected.\nSelect a page from the manual tree on the left.");
            }
        }

        private void AddControl(ref Models.Section section)
        {
            IControlIdentifierSelector selector = ProjectTypeProvider.GetControlSelector();
            selector.Section = section;
            AddControl dialog = new AddControl(section.DiscoveredControls, selector);
            dialog.ShowDialog();
            if (dialog.Control != null)
            {
                dialog.Control.Parent = section;

                (Models.Control control, bool succes) result = ProjectTypeProvider.RefreshControl(dialog.Control, section, section.Parent, currentVersion, CurrentProject);
                if (result.succes)
                {
                    if (result.control.ElementBounds != null)
                    {
                        section.Controls.Add(dialog.Control);
                        RedrawTreeView();
                        dialog.Control.TreeViewItem.IsSelected = true;
                    }
                    else
                    {
                        SetStatusBar("Control not added because refreshing failed.");
                    }
                }

            }
        }

        private void MiOpenObjectModelViewer_Click(object sender, RoutedEventArgs e)
        {
            new ObjectModelViewer(CurrentProject).ShowDialog();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (!openingNewProject)
            {
                Application.Current.Shutdown();
            }
        }

        private void MiNewControl_Click(object sender, RoutedEventArgs e)
        {
            SelectPage selector = new SelectPage(currentVersion);
            if (selector.ShowDialog().Value)
            {
                Models.Page selectedpage = currentVersion.Pages.Single(Page => Page.Id == selector.SelectedPageId);
                SelectSection sectionSelector = new SelectSection(selectedpage);
                if (sectionSelector.ShowDialog().Value)
                {
                    Models.Section selectedSection = selectedpage.Sections.Single(section => section.Id == sectionSelector.SelectedSectionId);
                    AddControl(ref selectedSection);
                }
            }
        }

        private void MiExportJson_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dialog = new Microsoft.Win32.SaveFileDialog();
            dialog.Title = "Select a location";
            dialog.Filter = "JSON files (.json)|*.json";

            bool? result = dialog.ShowDialog();

            if (result.Value == true)
            {
                SaveProjectAsJsonTask task = new SaveProjectAsJsonTask(CurrentProject, dialog.FileName);
                ProcessTaskDialog taskDialog = new ProcessTaskDialog(task, "Exporting...");
                if (!taskDialog.TaskToExecute.IsCanceled)
                {
                    SetStatusBar("Manual exported.");
                }
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            InfoAlert.Show("No user manual available now.");
        }

        private void BtnVersion_Click(object sender, RoutedEventArgs e)
        {
            currentVersion = CurrentProject.Versions.Single(version => version.Id == (e.Source as Button).Tag.ToString());
            SetStatusBar("Switched to version '" + currentVersion.Name + "'.");
            CurrentProject.LastViewedVersionId = currentVersion.Id;
            RedrawTreeView();
            CurrentProject.TreeViewItem.IsSelected = true;
            Title = CurrentProject.Name + " - " + currentVersion.Name + " - Interpic Studio";
        }
    }
}
