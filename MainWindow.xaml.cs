using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;


using NorbSoftDev.SOW;
using System.Collections.ObjectModel;
using System.ComponentModel;

using System.IO.IsolatedStorage;
using System.Diagnostics;



namespace ScenarioEditor
{
    /// <summary>
    /// Interaction logic for Window5.xaml
    /// </summary>
    public partial class SOWScenarioEditorWindow : Window, INotifyPropertyChanged
    {
        Config _config;
        internal Config config
        {
            get { return _config; }
            set
            {
                _config = value;
                OnPropertyChanged("config");
            }
        }
        #region Scenario

        //public ICommand scenario_UndoCommand { get; internal set; }
        //public ICommand scenario_RandomizeCommand { get; internal set; }
        Scenario _scenario;


        internal Scenario scenario
        {
            get { return _scenario; }
            set
            {
                if (_scenario != null)
                {
                }

                _scenario = value;
                ScenarioUndoStack = new ScenarioUndoStack(_scenario, maxUndos);
                //foreach (Graphic g in config.graphics.Values)
                //{
                //    //ONLY convert sprites and flags?
                //    g.GetBitmapSource(config);
                //    // PrintResult(g.ImageMagickExtract(config));
                //}
                //ScenarioUndoStack.active = true;
                //scenario_UndoCommand = new RelayCommand(
                //    param => scenarioUndoStack.Undo(),
                //    param => scenarioUndoStack.canUndo
                //);

                //scenario_RandomizeCommand = new BulkUndoableRelayCommand(
                //    scenarioUndoStack,
                //    param => scenario_RandomizePositions(),
                //    param => scenario_IsWritable()
                //);

            }
        }

        #endregion //Scenario


        OOBWindow _oobWindow;

        TreeViewHelper<ScenarioEchelon, ScenarioUnit> scenarioViewHelper;
        RosterDataGridHelper<ScenarioEchelon, ScenarioUnit> scenarioDataGridHelper;

        EventDataGridHelper eventDataGridHelper;
        ScenarioObjectiveDataGridHelper scenarioObjectiveDataGridHelper;
        MapObjectiveDataGridHelper mapObjectiveDataGridHelper;
        DataGridHelper<Position, PositionSelectionSet> fortsDataGridHelper;
        DataGridHelper<ScreenMessage, ScreenMessageSelectionSet> messagesDataGridHelper;
        System.Collections.ObjectModel.ObservableCollection<ScreenMessage> _screenMessagesCollection = new System.Collections.ObjectModel.ObservableCollection<ScreenMessage>();

        PythonHelper pythonHelper;

        HelpWindow helpWindow;

        string editorIniPath;

        bool _saveInDocumentsOnly = true;
        public bool saveInDocumentsOnly
        {
            get
            {
                return _saveInDocumentsOnly;
            }
            set
            {
                if (_saveInDocumentsOnly == value) return;
                _saveInDocumentsOnly = value;

                OnPropertyChanged("saveInDocumentsOnly");
            }
        }


        int maxUndos = 30;

        static ScenarioUndoStack _scenarioUndoStack;
        public ScenarioUndoStack ScenarioUndoStack
        {
            get
            {
                return _scenarioUndoStack;
            }
            set
            {
                if (_scenarioUndoStack != null)
                    _scenarioUndoStack.undoActions.PropertyChanged -= undoActions_PropertyChanged;
                _scenarioUndoStack = value;
                _scenarioUndoStack.undoActions.PropertyChanged += undoActions_PropertyChanged;

                OnPropertyChanged("ScenarioUndoStack");
            }
        }

        private void undoActions_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (maxUndos > 0 && _scenarioUndoStack.undoActions.nPushes % maxUndos == 0)
            {
                Log.Info(this, "AutoSaving to AutoSave");
                scenario.AutoSave();
            }
        }

        public static ScenarioUndoStack GetScenarioUndoStack()
        {
            return _scenarioUndoStack;
        }

        public static Scenario GetScenario()
        {
            return ((SOWScenarioEditorWindow)Application.Current.MainWindow)._scenario;
        }

        void PrintUndoStack(object sender, RoutedEventArgs e)

        {
            ScenarioUndoStack.PrintUndoStack();
        }

        void PrintScenarioCsv(object sender, RoutedEventArgs e)
        {
            Console.WriteLine(scenario.ScenarioCsv());
        }

        void PrettyPrintScenario(object sender, RoutedEventArgs e)
        {
            scenario.PrettyPrint();
        }

        void ForceException(object sender, RoutedEventArgs e)
        {
            throw new Exception();
        }

        void DumpHeaders(object sender, RoutedEventArgs e)
        {
            string dirpath = System.IO.Path.Combine(Config.USER_CONFIG_DIR, "Headers");
            Directory.CreateDirectory(dirpath);
            config.headers.WriteHeadersToDir(dirpath);
        }

        void ShowHelp(object sender, RoutedEventArgs e)
        {
            if (helpWindow == null)
            {
                helpWindow = new HelpWindow(this);
                helpWindow.Owner = this;
                helpWindow.Closed += (s, args) => helpWindow = null;
                helpWindow.Show();
            }
            helpWindow.Activate();
        }
        //internal object globalDraggedItem;

        //public formations 

        public void SetRecentlyOpenedScenario(string value)
        {
            if (value == null || value == _recentlyOpenedScenario0) return;
            recentlyOpenedScenario2 = _recentlyOpenedScenario1;
            recentlyOpenedScenario1 = _recentlyOpenedScenario0;
            recentlyOpenedScenario0 = value;
        }

        string _recentlyOpenedScenario0;
        public string recentlyOpenedScenario0
        {
            get { return _recentlyOpenedScenario0; }
            set
            {

                if (value == _recentlyOpenedScenario0) return;
                _recentlyOpenedScenario0 = value;
                OnPropertyChanged("recentlyOpenedScenario0");
            }
        }

        string _recentlyOpenedScenario1;
        public string recentlyOpenedScenario1
        {
            get { return _recentlyOpenedScenario1; }
            set
            {
                if (value == _recentlyOpenedScenario1) return;

                _recentlyOpenedScenario1 = value;
                OnPropertyChanged("recentlyOpenedScenario1");
            }
        }

        string _recentlyOpenedScenario2;
        public string recentlyOpenedScenario2
        {
            get { return _recentlyOpenedScenario2; }
            set
            {
                if (value == _recentlyOpenedScenario2) return;

                _recentlyOpenedScenario2 = value;
                OnPropertyChanged("recentlyOpenedScenario2");
            }
        }

        string _recentlyOpenedOOB;
        public string recentlyOpenedOOB
        {
            get { return _recentlyOpenedOOB; }
            set
            {
                _recentlyOpenedOOB = value;
                OnPropertyChanged("recentlyOpenedOOB");
            }
        }

        public SOWScenarioEditorWindow()
        {

            Application.Current.DispatcherUnhandledException += App_DispatcherUnhandledException;

            //if (true)
            //{
            //    try
            //    {

            //        SOWScenarioEditorWindowInit();
            //    }
            //    catch (Exception e)
            //    {
            //        Log.Exception(this, e);
            //        throw;
            //    }
            //    finally
            //    {
            //        Log.Flush();
            //    }
            //}
            //else
            //{
            //    SOWScenarioEditorWindowInit();
            //}

            try
            {
                SOWScenarioEditorWindowInit();
                Title += " " + System.Reflection.Assembly.GetExecutingAssembly()
                               .GetName()
                               .Version
                               .ToString();
            }
            finally
            {
                Log.Flush();
            }
        }

        public void SOWScenarioEditorWindowInit()
        {
            Log.SetupUserLog();

            try
            {
                Microsoft.Win32.RegistryKey installed_versions = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\NET Framework Setup\NDP");
                string[] version_names = installed_versions.GetSubKeyNames();
                for (int i = 0; i < version_names.Length; i++)
                {
                    Log.Info(this, "FrameWork:" + version_names[i] + " SP:" + installed_versions.OpenSubKey(version_names[i]).GetValue("SP", 0));
                }
                Log.Info(this, "Culture: " + System.Globalization.CultureInfo.CurrentCulture);
            }
            catch
            {
                Log.Warn(this, "Unable to list NET info");
            }

            InitializeComponent();
            InitializePanels();
            progressBar.IsIndeterminate = true;

            InitializeSOW(false);

            progressBar.IsIndeterminate = false;
            ShowGameLabel();

            pythonHelper = new PythonHelper(pythonInput, pythonOutput);
            pythonHelper.InitializeIronPython();
            pythonHelper.echelonSharedSelection = mapPanel.unitSharedSelection;
        }


        public void InitializePanels()
        {

            _oobWindow = new OOBWindow(this);
            scenarioViewHelper = new TreeViewHelper<ScenarioEchelon, ScenarioUnit>(this, scenarioTree, true);
            scenarioDataGridHelper = new RosterDataGridHelper<ScenarioEchelon, ScenarioUnit>(this, mapPanel, scenarioDataGrid);
            eventDataGridHelper = new EventDataGridHelper(this, mapPanel, eventDataGrid);
            scenarioObjectiveDataGridHelper = new ScenarioObjectiveDataGridHelper(this, mapPanel, objectiveDataGrid);
            mapObjectiveDataGridHelper = new MapObjectiveDataGridHelper(this, mapPanel, mapObjectiveDataGrid);
            fortsDataGridHelper = new DataGridHelper<Position, PositionSelectionSet>(this, mapPanel, fortsDataGrid);
            messagesDataGridHelper = new DataGridHelper<ScreenMessage, ScreenMessageSelectionSet>(this, mapPanel, screensDataGrid);
            mapPanel.window = this;
            scenarioViewHelper.sharedEchelonSelection = mapPanel.unitSharedSelection;
        }

        public void InitializeSOW(bool loadModsFromGame)
        {

            editorIniPath = System.IO.Path.GetFullPath(System.IO.Path.Combine(
                Config.USER_SOW_DOCUMENTS_DIR,
                "ScenarioEditor.ini"
            ));

            config = null;
            string gameDir = null;

            if (File.Exists(editorIniPath))
            {
                IniReader iniReader = new IniReader(editorIniPath);
                recentlyOpenedScenario0 = iniReader.GetValue("LastScenario0", "init", "");
                recentlyOpenedScenario1 = iniReader.GetValue("LastScenario1", "init", "");
                recentlyOpenedScenario2 = iniReader.GetValue("LastScenario2", "init", "");
                recentlyOpenedOOB = iniReader.GetValue("LastOOB0", "init", "");

                gameDir = iniReader.GetValue("GameDir", "init", null);

                bool.TryParse(iniReader.GetValue("saveInDocumentsOnly", "init", "true"), out _saveInDocumentsOnly);

                //no mods defined in editor ini, so check game

                //no mods defined in editor ini, so check game
                int nMods;
                int.TryParse(iniReader.GetValue("count", "mods", "0"),
                    out nMods);

                if (nMods < 1)
                {
                    loadModsFromGame = true;
                }


                int.TryParse(
                    iniReader.GetValue("MaxUndos", "init", maxUndos.ToString()),
                    out maxUndos);

            }
            else
            {
                loadModsFromGame = true;
            }


            if (gameDir == null)
            {
                // no GameDir saved — try to auto-find, otherwise ask the user
                try
                {
                    config = Config.AutoFind();
                }
                catch
                {
                    string browsed = BrowseForGameDirectory();
                    if (browsed == null)
                    {
                        MessageBox.Show("A game directory is required to use the editor.", "Game Directory Not Set", MessageBoxButton.OK, MessageBoxImage.Error);
                        Environment.Exit(1);
                        return;
                    }
                    config = new Config(browsed);
                    SaveEditorIni();
                }
            }
            else
            {
                if (!Directory.Exists(gameDir))
                {
                    string messageBoxText = "GameDir \"" + gameDir + "\" no longer exists.\nPlease select the game installation folder.";
                    Log.Error(this, messageBoxText);
                    MessageBox.Show(messageBoxText, "SOW ScenarioEditor Could Not Find Game Dir", MessageBoxButton.OK, MessageBoxImage.Warning);

                    string browsed = BrowseForGameDirectory();
                    if (browsed == null)
                    {
                        Environment.Exit(1);
                        return;
                    }
                    gameDir = browsed;
                }

                config = new Config(gameDir);
            }

            ApplyModActiveStates(loadModsFromGame);


            this.scenario_ModsMenu.Items.Clear();
            // scenario_SetModMenu lives in the "_Scenario" map panel menu, which is currently
            // commented out in MainWindow.xaml. Uncomment there and below together to restore.
            //this.scenario_SetModMenu.Items.Clear();

            //scenario_SetModMenu.IsEnabled = !saveInDocumentsOnly;

            foreach (Mod mod in config.mods)
            {
                MenuItem modMenu = new MenuItem();
                modMenu.Header = mod.name;
                modMenu.IsEnabled = mod.active;
                this.scenario_ModsMenu.Items.Add(modMenu);


                if (mod.active)
                {
                    //if (!saveInDocumentsOnly && mod != config.sdkMod & mod != config.baseMod)
                    //{
                    //    MenuItem setScenMenu = new MenuItem();
                    //    setScenMenu.Header = mod;
                    //    setScenMenu.Click += ModSetScenarioClicked;
                    //    scenario_SetModMenu.Items.Add(setScenMenu);
                    //}

                    foreach (string filepath in mod.GetScenarios())
                    {
                        MenuItem scenMenu = new MenuItem();
                        scenMenu.Header = filepath;
                        scenMenu.Click += ModScenarioClicked;
                        modMenu.Items.Add(scenMenu);
                    }
                }
            }

            this.scenario_DLCMenu.Items.Clear();
            this.scenario_DLCMenu.IsEnabled = config.dlcMods.Count > 0;
            foreach (Mod dlcMod in config.dlcMods)
            {
                MenuItem dlcModMenu = new MenuItem();
                dlcModMenu.Header = dlcMod.name;
                this.scenario_DLCMenu.Items.Add(dlcModMenu);

                foreach (string filepath in dlcMod.GetScenarios())
                {
                    MenuItem scenMenu = new MenuItem();
                    scenMenu.Header = filepath;
                    scenMenu.Click += ModScenarioClicked;
                    dlcModMenu.Items.Add(scenMenu);
                }
            }

            this.map_ModsMenu.Items.Clear();
            foreach (Mod mod in config.mods)
            {
                MenuItem modMenu = new MenuItem();
                modMenu.Header = mod.name;
                modMenu.IsEnabled = mod.active;
                this.map_ModsMenu.Items.Add(modMenu);

                if (mod.active)
                    foreach (string filepath in mod.GetMaps())
                    {
                        MenuItem scenMenu = new MenuItem();
                        scenMenu.Header = filepath;
                        scenMenu.Click += ModMapClicked;
                        modMenu.Items.Add(scenMenu);
                    }
            }

            this.oob_ModsMenu.Items.Clear();
            foreach (Mod mod in config.mods)
            {
                MenuItem modMenu = new MenuItem();
                modMenu.Header = mod.name;
                modMenu.IsEnabled = mod.active;
                this.oob_ModsMenu.Items.Add(modMenu);

                if (mod.active)
                    foreach (string filepath in mod.GetOOBs())
                    {
                        MenuItem scenMenu = new MenuItem();
                        scenMenu.Header = filepath;
                        scenMenu.Click += ModOOBClicked;
                        modMenu.Items.Add(scenMenu);
                    }
            }

            this.file_ModsMenu.Items.Clear();
            foreach (Mod mod in config.mods)
            {
                UserMod userMod = mod as UserMod;
                if (userMod == null) continue;

                MenuItem modMenu = new MenuItem();
                modMenu.Header = "[" + userMod.index + " ] " + userMod.name + " " + userMod.directory;
                modMenu.DataContext = userMod;
                modMenu.IsCheckable = true;
                modMenu.IsChecked = userMod.active;
                this.file_ModsMenu.Items.Add(modMenu);
                modMenu.PreviewMouseLeftButtonUp += ModMenuItemPreviewClick;
            }

            // Rebuild once when the user closes the menu, not on every click
            file_ModsMenu.SubmenuClosed -= OnModMenuClosed;
            file_ModsMenu.SubmenuClosed += OnModMenuClosed;
        }

        private void OnModMenuClosed(object sender, RoutedEventArgs e)
        {
            SaveEditorIni();
            InitializeSOW(false);
        }



        private void python_KeyUp(object sender, KeyEventArgs e)
        {
            pythonHelper.KeyUp(sender, e);
        }

        private void python_ImportScript(object sender, RoutedEventArgs e)
        {

            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
            ofd.InitialDirectory = System.IO.Path.Combine(System.IO.Path.Combine(config.resourcesDir, "python"));
            ofd.Filter = "py|*.py;";

            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                pythonHelper.ImportScript(ofd.FileName);
            }

        }

        private void python_ExecuteScript(object sender, RoutedEventArgs e)
        {

            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
            ofd.InitialDirectory = System.IO.Path.Combine(System.IO.Path.Combine(config.resourcesDir, "python"));
            ofd.Filter = "py|*.py;";

            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                pythonHelper.ExecuteScript(ofd.FileName);
            }

        }

        private void python_Execute(object sender, RoutedEventArgs e)
        {
            pythonHelper.Execute();
        }

        private void python_ExecuteAndClear(object sender, RoutedEventArgs e)
        {
            pythonHelper.ExecuteAndClear();
        }


        bool saveUserScenario(bool showDontSave)
        {
            return saveAsScenario(config.userMod, showDontSave);
        }

        private void saveClick(object sender, RoutedEventArgs e)
        {
            if (_scenario == null) return;
            if (!saveScenario_CanExecute()) return;
            saveScenario();
        }

        private void saveToUserFolderClick(object sender, RoutedEventArgs e)
        {
            if (_scenario == null) return;
            saveAsScenario(config.userMod, false);
        }

        private void saveAsClick(object sender, RoutedEventArgs e)
        {
            if (_scenario == null) return;

            // Build list of available mod folders from <gameBase>\Mods\
            string gameModsDir = System.IO.Path.Combine(config.SOWDir, config.gameBase, "Mods");
            var modFolders = new List<string>();
            if (System.IO.Directory.Exists(gameModsDir))
            {
                foreach (string dir in System.IO.Directory.GetDirectories(gameModsDir))
                    modFolders.Add(System.IO.Path.GetFileName(dir));
            }

            // Show picker dialog
            var dialog = new Window
            {
                Title = "Save As",
                Width = 380,
                Height = 180,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = this,
                ResizeMode = ResizeMode.NoResize,
                ShowInTaskbar = false
            };

            var grid = new System.Windows.Controls.Grid { Margin = new Thickness(12) };
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            var label = new System.Windows.Controls.Label { Content = "Choose or type a mod folder name in the game's Mods directory:" };
            System.Windows.Controls.Grid.SetRow(label, 0);

            var combo = new System.Windows.Controls.ComboBox
            {
                IsEditable = true,
                Margin = new Thickness(0, 4, 0, 12),
                ItemsSource = modFolders,
                Text = modFolders.Count > 0 ? modFolders[0] : string.Empty
            };
            System.Windows.Controls.Grid.SetRow(combo, 1);

            var buttonPanel = new System.Windows.Controls.StackPanel
            {
                Orientation = System.Windows.Controls.Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Right
            };
            var okBtn = new System.Windows.Controls.Button { Content = "OK", Width = 75, Margin = new Thickness(0, 0, 8, 0), IsDefault = true };
            var cancelBtn = new System.Windows.Controls.Button { Content = "Cancel", Width = 75, IsCancel = true };
            buttonPanel.Children.Add(okBtn);
            buttonPanel.Children.Add(cancelBtn);
            System.Windows.Controls.Grid.SetRow(buttonPanel, 2);

            grid.Children.Add(label);
            grid.Children.Add(combo);
            grid.Children.Add(buttonPanel);
            dialog.Content = grid;

            bool confirmed = false;
            okBtn.Click += (s, args) => { confirmed = true; dialog.Close(); };
            cancelBtn.Click += (s, args) => { dialog.Close(); };
            dialog.ShowDialog();

            if (!confirmed) return;

            string folderName = combo.Text?.Trim();
            if (string.IsNullOrEmpty(folderName)) return;

            // Resolve or create the mod directory
            string targetModDir = System.IO.Path.Combine(gameModsDir, folderName);
            if (!System.IO.Directory.Exists(targetModDir))
                System.IO.Directory.CreateDirectory(targetModDir);

            // Find an existing Mod object for this folder, or create a transient one
            Mod targetMod = config.mods.FirstOrDefault(m =>
                m.directory != null &&
                string.Equals(m.directory.FullName, System.IO.Path.GetFullPath(targetModDir), StringComparison.OrdinalIgnoreCase));

            if (targetMod == null)
                targetMod = new NorbSoftDev.SOW.UserMod(new System.IO.DirectoryInfo(targetModDir));

            saveAsScenario(targetMod, false);
        }
        /// <summary>
        /// bool indicates false if canceled
        /// </summary>
        /// <param name="mod"></param>
        /// <param name="showDontSave"></param>
        /// <returns></returns>
        bool saveAsScenario(Mod mod, bool showDontSave)
        {

            if (_scenario == null) return true;
            Mod oldMod = _scenario.mod;

            if (mod != null)
            {
                _scenario.mod = mod;
            }

            ScenarioPropertiesDialog window = new ScenarioPropertiesDialog(scenario, showDontSave);
            window.Owner = this;
            window.ShowDialog();
            if (window.DialogResult != true) return window.choiceWasMade;

            saveScenario();
            return true;
        }


        void saveScenario()
        {

            if (_scenario == null) return;
            if (SaveISOWFile(_scenario))
            {
                SetRecentlyOpenedScenario(_scenario.scenarioCsvFile);
            }
        }


        public bool saveScenario_CanExecute()
        {
            if (_scenario == null) return false;
            if (_scenario.mod == config.sdkMod) return false;
            if (saveInDocumentsOnly && _scenario.mod != config.userMod) return false;
            return true;
        }



        private void ModSetScenarioClicked(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;
            if (sender == null) return;
            if (scenario != null) scenario.mod = menuItem.Header as Mod;

        }

        private void ModScenarioClicked(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;
            if (sender == null) return;
            loadScenario(menuItem.Header as string);

        }
        public void ModMapClicked(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;
            if (sender == null) return;
            if (scenario == null)
            {
                Scenario.defaultMapName = menuItem.Header as string;
            }
        }

        // ── Map ComboBox ──────────────────────────────────────────────────────

        private class MapEntry
        {
            public string Name        { get; }
            public string DisplayName { get; }
            public MapEntry(string name, string displayName) { Name = name; DisplayName = string.IsNullOrEmpty(displayName) ? name : displayName; }
            public override string ToString() => DisplayName;
        }

        bool _suppressMapComboChange = false;

        private void PopulateMapComboBox()
        {
            if (config == null) return;
            _suppressMapComboChange = true;
            try
            {
                var entries = new System.Collections.Generic.List<MapEntry>();
                var seen = new System.Collections.Generic.HashSet<string>(System.StringComparer.OrdinalIgnoreCase);
                foreach (Mod mod in config.mods)
                {
                    if (!mod.active) continue;
                    foreach (string filepath in mod.GetMaps())
                    {
                        string name = System.IO.Path.GetFileNameWithoutExtension(filepath);
                        if (!seen.Add(name)) continue;
                        Map m = config.GetMap(name);
                        entries.Add(new MapEntry(name, m?.niceName));
                    }
                }
                mapComboBox.ItemsSource = entries;
                if (scenario?.map != null)
                    mapComboBox.SelectedItem = entries.Find(me => string.Equals(me.Name, scenario.map.name, System.StringComparison.OrdinalIgnoreCase));
            }
            finally
            {
                _suppressMapComboChange = false;
            }
        }

        private void mapComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (_suppressMapComboChange) return;
            if (scenario == null || config == null) return;
            MapEntry entry = mapComboBox.SelectedItem as MapEntry;
            if (entry == null) return;
            if (string.Equals(entry.Name, scenario.map?.name, System.StringComparison.OrdinalIgnoreCase)) return;
            Map newMap = config.GetMap(entry.Name);
            if (newMap == null) return;
            mapPanel.ChangeMap(newMap);
        }

        public void ModOOBClicked(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;
            if (sender == null) return;
            loadOrderOfBattle(menuItem.Header as string);

        }

        private void ModMenuItemPreviewClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;
            if (menuItem == null) return;
            Mod mod = menuItem.DataContext as Mod;
            if (mod == null) return;
            mod.active = !mod.active;
            menuItem.IsChecked = mod.active;
            e.Handled = true; // Prevents WPF from closing the menu
        }

        public void ModClicked(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;
            if (menuItem == null) return;
            Mod mod = menuItem.DataContext as Mod;
            if (mod == null) return;
            mod.active = !mod.active;
            menuItem.IsChecked = mod.active;
        }


        private void modsSyncClick(object sender, RoutedEventArgs e)
        {
            SaveEditorIni();
            InitializeSOW(true);
        }

        string BrowseForGameDirectory()
        {
            using (var fbd = new System.Windows.Forms.FolderBrowserDialog())
            {
                fbd.Description = "Select the SOW Gettysburg (SOWGBx64) or SOW Waterloo (SOWWLx64) game installation folder";
                fbd.ShowNewFolderButton = false;
                if (config != null && Directory.Exists(config.SOWDir))
                    fbd.SelectedPath = config.SOWDir;
                if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string selected = fbd.SelectedPath;
                    if (!Config.IsValidGameDir(selected))
                    {
                        MessageBox.Show(
                            "The selected folder does not appear to be a valid SOW Gettysburg or Waterloo installation.\nCould not find '" + Config.GAME_BASE + "' or '" + Config.GAME_BASE_WL + "' subfolder in:\n" + selected,
                            "Invalid Game Directory",
                            MessageBoxButton.OK,
                            MessageBoxImage.Warning);
                        return null;
                    }
                    return selected;
                }
            }
            return null;
        }

        private void SetGameDirectoryClick(object sender, RoutedEventArgs e)
        {
            string gameDir = BrowseForGameDirectory();
            if (gameDir == null) return;

            if (_scenario != null &&
                !string.Equals(gameDir, config?.SOWDir, StringComparison.OrdinalIgnoreCase))
            {
                string newGame = Config.DetectGameBase(gameDir) == Config.GAME_BASE_WL
                    ? "Scourge of War: Waterloo" : "Scourge of War: Gettysburg";
                var result = MessageBox.Show(
                    $"Switching to {newGame} will close the current scenario.\n\nAny unsaved changes will be lost. Continue?",
                    "Switch Game",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);
                if (result != MessageBoxResult.Yes) return;
            }

            // Clear any open scenario before switching games
            _scenario = null;
            this.DataContext = null;
            mapPanel.Scenario = null;
            if (_oobWindow != null) _oobWindow.DataContext = null;
            ResetUIForGameChange();

            Log.Warnings.Clear();
            Log.Errors.Clear();

            config = new Config(gameDir);
            config.ApplyGameIniActiveStates(); // correct mod states before saving
            SaveEditorIni();
            InitializeSOW(false);
            ShowGameLabel();
        }

        void ApplyModActiveStates(bool fromGameIni)
        {
            if (fromGameIni)
            {
                config.ApplyGameIniActiveStates();
                return;
            }

            // Read mod active states saved by this editor.
            if (!File.Exists(editorIniPath))
            {
                config.ApplyGameIniActiveStates();
                return;
            }

            IniReader iniReader = new IniReader(editorIniPath);
            int nMods = 0;
            int.TryParse(iniReader.GetValue("count", "mods", "0"), out nMods);
            if (nMods < 1)
            {
                config.ApplyGameIniActiveStates();
                return;
            }

            var editorModStates = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);
            for (int i = 0; i < nMods; i++)
            {
                string modRelPath = iniReader.GetValue("modname" + i, "mods", null);
                if (string.IsNullOrWhiteSpace(modRelPath)) continue;
                bool isActive = iniReader.GetValue("modactive" + i, "mods", "0").Trim() == "1";
                modRelPath = modRelPath.Trim().Replace('\\', System.IO.Path.DirectorySeparatorChar);
                // Use config.gameBase (not the hardcoded constant) so Waterloo paths resolve correctly.
                string fullPath = System.IO.Path.GetFullPath(
                    System.IO.Path.Combine(config.SOWDir, config.gameBase, modRelPath));
                editorModStates[fullPath] = isActive;
            }

            int matched = 0;
            foreach (Mod mod in config.mods)
            {
                UserMod userMod = mod as UserMod;
                if (userMod == null) continue;
                bool isActive;
                if (editorModStates.TryGetValue(userMod.directory.FullName, out isActive))
                {
                    userMod.active = isActive;
                    matched++;
                }
                else
                {
                    // Not in editor INI — default inactive so nothing is silently enabled.
                    userMod.active = false;
                }
            }

            // If nothing in the editor INI matched the current game's mods (e.g. after a
            // game switch), fall back to the game's own INI for a sensible starting state.
            if (matched == 0)
            {
                Log.Info(this, "No editor INI mod entries matched current game — falling back to game INI");
                config.ApplyGameIniActiveStates();
                return;
            }

            Log.Info(this, "Mod active states loaded from editor INI (" + matched + " matched)");
        }

        public void SaveEditorIni()
        {
            IniReader ini = new IniReader(editorIniPath);


            int modIndex = 0;
            if (config != null && config.mods != null)
            {
                foreach (Mod mod in config.mods)
                {
                    UserMod userMod = mod as UserMod;
                    if (userMod == null) continue;
                    ini.SetValue("modname" + modIndex, "mods", "Mods\\" + userMod.name);
                    ini.SetValue("modactive" + modIndex, "mods", userMod.active ? "1" : "0");
                    modIndex++;
                }
            }

            ini.SetValue("count", "mods", modIndex);
            if (recentlyOpenedScenario0 != null) ini.SetValue("LastScenario0", "init", recentlyOpenedScenario0);
            if (recentlyOpenedScenario1 != null) ini.SetValue("LastScenario1", "init", recentlyOpenedScenario1);
            if (recentlyOpenedScenario2 != null) ini.SetValue("LastScenario2", "init", recentlyOpenedScenario2);
            if (recentlyOpenedOOB != null) ini.SetValue("LastOOB0", "init", recentlyOpenedOOB);

            ini.SetValue("saveInDocumentsOnly", "init", saveInDocumentsOnly);

            if (config != null)
                ini.SetValue("GameDir", "init", config.SOWDir);

            WriteTextFile(ini.Pretty(), editorIniPath);
        }


        void WriteTextFile(string text, string filepath)
        {

            if (File.Exists(filepath))
            {
                Log.Info(this, "Overwriting existing: \"" + filepath + "\"");
            }
            else
            {
                Log.Info(this, "Writing: \"" + filepath + "\"");
            }

            StreamWriter writer = null;
            try
            {
                Directory.CreateDirectory(System.IO.Path.GetDirectoryName(filepath));
                writer = new StreamWriter(filepath, false, Config.TextFileEncoding);

                writer.Write(text);
            }
            catch (Exception ex)
            {
                FileInfo info = new FileInfo(filepath);
                Log.Error(this, "Unable to write to file " + filepath + ". " + info.Attributes);
                Log.Exception(this, ex);
                MessageBox.Show(
                    "Failed to write file \"" + filepath + "\".\n\n" + ex.Message,
                    "File Write Failed",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }

            finally
            {
                if (writer != null) writer.Close();
            }

        }

        void randomizeScenario()
        {
            ScenarioGeneratorWindow window = new ScenarioGeneratorWindow(this.scenario);
            window.Owner = this;
            window.ShowDialog();
            if (window.DialogResult == true)
            {
                SetScenario(window.scenario);
            }
        }




        void newScenario()
        {
            NewScenarioWindow window = new NewScenarioWindow(this);
            window.Owner = this;
            window.ShowDialog();
        }

        /// <summary>
        /// 
        /// </summary>
        void loadScenario()
        {
            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();

            string lastDir = null;

            try
            {
                lastDir = System.IO.Path.GetDirectoryName(
                //System.IO.Path.GetDirectoryName(Properties.Settings.Default.LastScenario)
                recentlyOpenedScenario0
                    );
            }
            catch
            {

            }
            if (lastDir == null || lastDir == String.Empty)
            {
                lastDir = System.IO.Path.Combine(config.baseMod.directory.FullName, "Scenarios");
            }

            try
            {
                ofd.InitialDirectory = lastDir;
                ofd.CustomPlaces.Add(System.IO.Path.Combine(config.userMod.directory.FullName));
                ofd.CustomPlaces.Add(System.IO.Path.Combine(config.baseMod.directory.FullName));
            }
            catch { }


            foreach (Mod mod in config.mods)
            {
                if (!mod.active) continue;
                ofd.CustomPlaces.Add(System.IO.Path.Combine(mod.directory.FullName));
            }

            ofd.Filter = "Csv|scenario.csv;";

            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                loadScenario(ofd.FileName);
            }
        }

        void loadScenario(string filename)
        {
            if (filename == null || filename == String.Empty) return;

            System.IO.FileInfo fileInfo = new System.IO.FileInfo(filename);
            if (!TestFile(fileInfo)) return;

            Mod mod = config.GetModFromFile(fileInfo);
            if (mod == null)
            {
                string messageBoxText = "Unable to determine the Mod for " + filename + ".\n Scenarios must be in from your Users Documents\\SowGB\\Scenarios dir or a Mod Scenarios dir";
                string caption = "Scenario Loaded Failed";
                MessageBoxButton button = MessageBoxButton.OK;
                MessageBoxImage icon = MessageBoxImage.Error;
                MessageBox.Show(messageBoxText, caption, button, icon);
                return;
            }

            if (!mod.active)
            {
                string messageBoxText = "Mod " + mod.name + " for " + filename + " is not active.\n Scenarios must be loaded from active mods.\n Please activate the Mod in the game and restart this editor";
                string caption = "Mod Not Active";
                MessageBoxButton button = MessageBoxButton.OK;
                MessageBoxImage icon = MessageBoxImage.Error;
                MessageBox.Show(messageBoxText, caption, button, icon);
                return;
            }


            Log.Warnings.Clear();
            Log.Errors.Clear();

            Scenario _tmp;

            _tmp = new Scenario(config, mod, fileInfo.Directory.Name);
            try
            {
                _tmp.Load();
            }
            catch (Exception ex)
            {
                Log.Exception(this, ex);
                MessageBox.Show(
                    "Failed to load scenario \"" + fileInfo.Name + "\".\n\n" + ex.Message + "\n\nThe previous scenario (if any) has been kept open.",
                    "Scenario Load Failed",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }

            SetRecentlyOpenedScenario(filename);

            SetScenario(_tmp);



            //scenario = _tmp;
            //ScenarioUndoStack.Initialize();
            //ScenarioUndoStack.active = true;

            //this.DataContext = scenario;
            //AssignItemSources(config);
            //BindAttributes(scenario);
            //mapPanel.Scenario = scenario;
            //pythonHelper.scenario = scenario;

            //scenarioViewHelper.CollapseBelowRank(ERank.Division);
            //oobViewHelper.CollapseBelowRank(ERank.Division);

        }

        public void SetScenario(Scenario tmp)
        {
            GraphicsManager.ClearCache();
            scenario = tmp;
            ScenarioUndoStack.Initialize();
            ScenarioUndoStack.active = true;

            this.DataContext = scenario;
            AssignItemSources(config);
            BindAttributes(scenario);
            mapPanel.Scenario = scenario;
            pythonHelper.scenario = scenario;
            PopulateMapComboBox();

            scenarioViewHelper.CollapseBelowRank(ERank.Division);
            SetActiveEchelonButton(stvBtnDivision);
            if (_oobWindow != null) _oobWindow.DataContext = scenario;
            RefreshScreenMessagesCollection();
        }

        // Returns the game-specific template path if it exists, otherwise the generic one.
        // E.g. "EnglishScenScreen.txt" → looks for "EnglishScenScreen_WL.txt" / "_GB.txt" first.
        string ResolveScenarioTemplate(string filename)
        {
            string suffix = (config?.gameBase == NorbSoftDev.SOW.Config.GAME_BASE_WL) ? "_WL" : "_GB";
            string nameOnly = System.IO.Path.GetFileNameWithoutExtension(filename);
            string ext      = System.IO.Path.GetExtension(filename);
            string specific = System.IO.Path.Combine(config.resourcesDir, "Scenario", nameOnly + suffix + ext);
            if (System.IO.File.Exists(specific)) return specific;
            string generic  = System.IO.Path.Combine(config.resourcesDir, "Scenario", filename);
            return System.IO.File.Exists(generic) ? generic : null;
        }

        void RefreshScreenMessagesCollection()
        {
            _screenMessagesCollection.Clear();
            if (scenario?.screenMessages == null) return;
            foreach (ScreenMessage msg in scenario.screenMessages.Values)
                _screenMessagesCollection.Add(msg);

            // For a brand-new unsaved scenario, screenMessages will be empty and no file exists yet.
            // Pre-populate from the game-specific template so the tab isn't blank.
            if (_screenMessagesCollection.Count == 0)
            {
                string screenFile = System.IO.Path.Combine(scenario.scenarioDir, "EnglishScenScreen.txt");
                if (!System.IO.File.Exists(screenFile))
                {
                    string templatePath = ResolveScenarioTemplate("EnglishScenScreen.txt");
                    if (templatePath != null)
                    {
                        foreach (var msg in ParseScreenTemplate(templatePath))
                        {
                            scenario.screenMessages[msg.id] = msg;
                            _screenMessagesCollection.Add(msg);
                        }
                    }
                }
            }

            screensDataGrid.ItemsSource = _screenMessagesCollection;

            // Load intro text — fall back to game-specific template for new unsaved scenarios
            string introPath = System.IO.Path.Combine(scenario.scenarioDir, "EnglishScenIntro.txt");
            if (System.IO.File.Exists(introPath))
            {
                introTextBox.Text = System.IO.File.ReadAllText(introPath, NorbSoftDev.SOW.Config.TextFileEncoding);
            }
            else
            {
                string templatePath = ResolveScenarioTemplate("EnglishScenIntro.txt");
                introTextBox.Text = templatePath != null
                    ? System.IO.File.ReadAllText(templatePath, NorbSoftDev.SOW.Config.TextFileEncoding)
                    : string.Empty;
            }
        }


        void loadStartLocsDirect(IList<ScenarioUnit> unitFilter)
        {

            if (unitFilter == null)
            {
                unitFilter = scenario;
            }

            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
            ofd.InitialDirectory = System.IO.Path.Combine(config.userMod.directory.FullName);
            ofd.Filter = "Csv|*.csv;";

            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                System.IO.FileInfo fileInfo = new System.IO.FileInfo(ofd.FileName);
                if (!TestFile(fileInfo)) return;

                ScenarioUndoStack.active = false;
                scenario.ReadStartLocsCsv(fileInfo.FullName, unitFilter);


            }
        }

        void loadStartLocsWithModifiers(IList<ScenarioUnit> unitFilter)
        {
            if (unitFilter == null)
            {
                unitFilter = scenario;
            }
            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
            ofd.InitialDirectory = System.IO.Path.Combine(config.userMod.directory.FullName);
            ofd.Filter = "Csv|*.csv;";

            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                System.IO.FileInfo fileInfo = new System.IO.FileInfo(ofd.FileName);
                if (!TestFile(fileInfo)) return;

                ScenarioUndoStack.active = false;
                //scenario.ReadGameDBCsv(fileInfo.FullName, unitFilter);

                ApplyUnitLocsWindow window = new ApplyUnitLocsWindow(fileInfo.FullName, config, unitFilter);
                window.Owner = this;
                window.ShowDialog();
                ScenarioUndoStack.Initialize();
                ScenarioUndoStack.active = true;

            }
        }

        void loadGameDB(IList<ScenarioUnit> unitFilter)
        {

            if (unitFilter == null)
            {
                unitFilter = scenario;
            }
            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
            ofd.InitialDirectory = System.IO.Path.Combine(config.userMod.directory.FullName);
            ofd.Filter = "Csv|*.csv;";

            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                System.IO.FileInfo fileInfo = new System.IO.FileInfo(ofd.FileName);
                if (!TestFile(fileInfo)) return;

                ScenarioUndoStack.active = false;
                //scenario.ReadGameDBCsv(fileInfo.FullName, unitFilter);

                ApplyGameDBWindow window = new ApplyGameDBWindow(fileInfo.FullName, config, unitFilter);
                window.Owner = this;
                window.ShowDialog();
                ScenarioUndoStack.Initialize();
                ScenarioUndoStack.active = true;

            }
        }

        void saveOrderOfBattle()
        {

            if (_scenario == null) throw new Exception("No ISOWFile");
            OrderOfBattle oob = _scenario.orderOfBattle;

            Mod oldMod = oob.mod;
            oob.mod = config.userMod;
            if (SaveISOWFile(oob))
            {
                //Properties.Settings.Default.LastOOB = oob.orderOfBattleFile;
                recentlyOpenedOOB = oob.orderOfBattleFile;
            }
            else
            {
                oob.mod = config.userMod;
            }
        }


        public bool saveOrderOfBattle_CanExecute()
        {
            if (_scenario == null) return false;
            if (_scenario.orderOfBattle == null) return false;
            if (_scenario.isSandbox) return false;
            if (_scenario.orderOfBattle.mod == config.sdkMod) return false;

            if (saveInDocumentsOnly && _scenario.orderOfBattle.mod != config.userMod) return false;

            //if (_scenario.orderOfBattle.isDirty) return true;
            return true;
        }

        void exportOrderOfBattle()
        {

            if (_scenario == null) throw new Exception("No ISOWFile");

            // Configure save file dialog box
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.InitialDirectory = System.IO.Path.Combine(config.userMod.directory.FullName, "OOBs");
            dlg.FileName = "ExportedOOB"; // Default file name
            dlg.DefaultExt = ".csv"; // Default file extension
            dlg.Filter = "Csv documents (.csv)|*.csv"; // Filter files by extension 
            Nullable<bool> result = dlg.ShowDialog();

            // Process save file dialog box results 
            if (result == true)
            {
                // Save document 
                string filename = dlg.FileName;
                _scenario.ExportAsOOB(filename);
            }
        }


        public bool exportOrderOfBattle_CanExecute()
        {
            if (_scenario == null) return false;
            if (_scenario.orderOfBattle == null) return false;
            if (_scenario.isSandbox) return true;
            return true;
        }


        internal void loadOrderOfBattle()
        {

            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
            string lastDir = null;
            try
            {
                System.IO.Path.GetDirectoryName(
                    recentlyOpenedOOB
                    );
            }
            catch
            {

            }
            if (lastDir == null || lastDir == String.Empty)
            {
                lastDir = System.IO.Path.Combine(config.userMod.directory.FullName, "OOBs");
            }

            ofd.InitialDirectory = lastDir;
            ofd.CustomPlaces.Add(System.IO.Path.Combine(config.userMod.directory.FullName));
            ofd.CustomPlaces.Add(System.IO.Path.Combine(config.baseMod.directory.FullName));
            foreach (Mod mod in config.mods)
            {
                ofd.CustomPlaces.Add(System.IO.Path.Combine(mod.directory.FullName));
            }

            ofd.Filter = "Csv|*.csv;";

            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                loadOrderOfBattle(ofd.FileName);
            }
        }

        internal void loadOrderOfBattle(string filename)
        {
            if (filename == null || filename == String.Empty) return;
            System.IO.FileInfo fileInfo = new System.IO.FileInfo(filename);

            if (!TestFile(fileInfo)) return;

            Mod mod = config.GetModFromFile(fileInfo);
            if (mod == null)
            {
                string messageBoxText = "Unable to determine the Mod for " + filename + ". OOB must be in from your Users Documents\\SowGB\\OOBs dir or a Mod OOBs dir";
                string caption = "OOB Loaded Failed";
                MessageBoxButton button = MessageBoxButton.OK;
                MessageBoxImage icon = MessageBoxImage.Error;
                MessageBox.Show(messageBoxText, caption, button, icon);
                return;
            }

            if (!mod.active)
            {
                string messageBoxText = "Mod " + mod.name + " for " + filename + " is not active. OOBs must be loaded from active mods. Please activate the Mod in the game and restart this editor";
                string caption = "Mod Not Active";
                MessageBoxButton button = MessageBoxButton.OK;
                MessageBoxImage icon = MessageBoxImage.Error;
                MessageBox.Show(messageBoxText, caption, button, icon);
                return;
            }


            Log.Warnings.Clear();
            Log.Errors.Clear();

            progressBar.IsIndeterminate = true;
            progressBarTextBlock.Text = "Loading";
            //Would need worker thread
            OrderOfBattle orderOfBattle = new OrderOfBattle(config, mod, System.IO.Path.GetFileNameWithoutExtension(fileInfo.Name));
            try
            {
                orderOfBattle.Load();
            }
            catch (Exception ex)
            {
                progressBar.IsIndeterminate = false;
                ShowGameLabel();
                Log.Exception(this, ex);
                MessageBox.Show(
                    "Failed to load OOB \"" + fileInfo.Name + "\".\n\n" + ex.Message,
                    "OOB Load Failed",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }
            recentlyOpenedOOB = orderOfBattle.orderOfBattleFile;

            scenario = new Scenario(orderOfBattle, false);
            ScenarioUndoStack.Initialize();
            ScenarioUndoStack.active = true;

            this.DataContext = scenario;
            AssignItemSources(config);
            BindAttributes(scenario);
            mapPanel.Scenario = scenario;
            pythonHelper.scenario = scenario;
            PopulateMapComboBox();
            RefreshScreenMessagesCollection();

            progressBar.IsIndeterminate = false;
            ShowGameLabel();

            _oobWindow?.oobViewHelper?.CollapseBelowRank(ERank.Division);

        }

        internal void NewScenario(string selectedOobPath, string selectedMapPath)
        {

            if (selectedMapPath != null && selectedMapPath != string.Empty)
            {
                Scenario.defaultMapName = selectedMapPath;
            }
            loadOrderOfBattle(selectedOobPath);

            string screen_txt = config.GetResourceFile("Scenario/EnglishScenScreen.txt");

            scenario.ReadScreen(screen_txt);
        }
        bool TestFile(FileInfo fileInfo)
        {
            if (!fileInfo.Exists)
            {

                string messageBoxText = fileInfo.FullName + " does not exist";
                string caption = "TestFile";
                MessageBoxButton button = MessageBoxButton.OK;
                MessageBoxImage icon = MessageBoxImage.Warning;
                MessageBox.Show(messageBoxText, caption, button, icon);
                return false;
            }
            return true;
        }

        public bool SaveISOWFile(ISOWFile sowFile)
        {

            if (sowFile == null) throw new Exception("No ISOWFile");

            // Configure the message box to be displayed 
            if (!sowFile.CanSafelySave())
            {
                string messageBoxText = "Do you want to save over  \"" + sowFile.name + "\" in \"" + sowFile.mod.directory + "\"?";
                string caption = "SOW Save File";
                MessageBoxButton button = MessageBoxButton.YesNoCancel;
                MessageBoxImage icon = MessageBoxImage.Warning;
                MessageBoxResult result = MessageBox.Show(messageBoxText, caption, button, icon);

                switch (result)
                {
                    case MessageBoxResult.Yes:
                        break;
                    default:
                        return false;
                }
            }

            try
            {
                sowFile.Save();
            }
            catch (Exception ex)
            {
                Log.Exception(this, ex);
                MessageBox.Show(
                    "Failed to save \"" + sowFile.name + "\".\n\n" + ex.Message,
                    "Save Failed",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return false;
            }
            Log.Info(this, "Saved " + sowFile);
            return true;

        }


        void BindAttributes(Scenario scenario)
        {
            // Dynamic attribute columns (OOB side now lives in OOBWindow)
            _oobWindow?.BindAttributes(scenario, config);

            // Remove previously added attribute columns before re-adding (prevents duplicates on reload)
            var attributeHeaders = new HashSet<string>(scenario.attributeNames.Select(a => a.name));
            foreach (var existing in scenarioDataGrid.Columns
                .Where(c => c is DataGridComboBoxColumn && attributeHeaders.Contains(c.Header?.ToString()))
                .ToList())
            {
                scenarioDataGrid.Columns.Remove(existing);
            }

            foreach (NorbSoftDev.SOW.Attribute attribute in scenario.attributeNames)
            {
                DataGridComboBoxColumn col = new DataGridComboBoxColumn();
                col.ItemsSource = config.attributes[attribute.name];
                col.Header = attribute.name;
                col.SelectedValueBinding = new Binding("attributes[" + attribute.name + "]");
                scenarioDataGrid.Columns.Add(col);
            }



        }


        /// <summary>
        /// Assign lookups for known tables
        /// </summary>
        /// <param name="config"></param>
        void AssignItemSources(Config config)
        {
            //Scenario
            //startTimeControl.DataContext = scenario.startTime;
            //foreach (Mod mod in config.mods)
            //{
            //    if (!mod.active) continue;
            //    MenuItem submenu = new MenuItem();
            //    submenu.Header = mod.ToString();
            //    submenu.DataContext = mod;
            //    submenu.Click += ModMenuClick;
            //    scenarioModMenu.Items.Add(mod);

            //}

            objectiveDataGrid.ItemsSource = new ObservableCollection<ScenarioObjective>(scenario.objectives.Values);

            //directly assigning values will cause a  'EditItem' is not allowed for this view.
            //It's ok to wrap them in the collection as they will not dynamically change
            mapObjectiveDataGrid.ItemsSource = new ObservableCollection<MapObjective>(scenario.map.objectives.Values);
            fortsDataGrid.ItemsSource = new ObservableCollection<Fort>(scenario.map.forts.Values);

            //odgPriority.ItemsSource = new string[] { "Major", "Minor" };

            //CollectionViewSource cvs = (CollectionViewSource)(eventDataGrid.ItemsSource);
            //foreach (var i in eventDataGrid.ItemsSource)
            //{
            //    Console.WriteLine(i);
            //}
            scenario.battleScript.events.ItemPropertyChanged -= edg_CollectionViewSource_OnSourcePropertyChanged;
            scenario.battleScript.events.ItemPropertyChanged += edg_CollectionViewSource_OnSourcePropertyChanged;
            //dingus
            //eventFormationCombo.ItemsSource = config.formations.Values;
            //eventCommandCombo.ItemsSource = config.commandTemplates.Values;
            //eventUnitCommandCombo.ItemsSource = scenario;

            //

            //OOB
            _oobWindow?.AssignItemSources(config);
            //weapon.ItemsSource = config.weapons.Values;
            //flag.ItemsSource = config.graphics.Values;
            //flag2.ItemsSource = config.graphics.Values;

        }

        private void ModMenuClick(object sender, RoutedEventArgs e)
        {
            scenario.mod = config.userMod;
            //MenuItem menuItem = (MenuItem)sender;
            //Mod mod = (Mod)menuItem.DataContext;
            //scenario.mod = mod;
        }


        public void SelectedUnitChanged(string id)
        {
            // xaml needs to have SelectedValuePath="id"

            ScenarioUnit su;
            if (scenario.TryGetUnitByIdOrName(id, out su))
            {
                scenarioDataGrid.SelectedValue = id;

                scenarioDataGrid.ScrollIntoView(su);

            }

            _oobWindow?.SelectId(id, scenario.orderOfBattle);
        }

        private void causeErrorClick(object sender, RoutedEventArgs e)
        {
            throw new Exception("Test of the Logging System");
        }

        private void map_GoToPositionClick(object sender, RoutedEventArgs e)
        {
            IHasPosition ihp = (IHasPosition)((Button)sender).DataContext;
            mapPanel.Center(ihp);
        }

        private void map_GoToObjectiveClick(object sender, RoutedEventArgs e)
        {
            IHasObjective iho = (IHasObjective)((Button)sender).DataContext;
            mapPanel.Center((Position)iho.objective);
        }

        private void map_XSliderChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            mapPanel.SlideToXPercent(e.NewValue);
        }

        private void map_YSliderChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            mapPanel.SlideToYPercent(e.NewValue);
        }

        private void map_ModeChanged(object sender, SelectionChangedEventArgs e)
        {
            mapPanel.mode = (MapMode)(sender as ComboBox).SelectedItem;
        }

        private void map_ZoomIn(object sender, RoutedEventArgs e)
        {
            mapPanel.Zoom(2);
        }

        private void map_ZoomOut(object sender, RoutedEventArgs e)
        {
            mapPanel.Zoom(0.5);
        }

        private void map_ZoomReset(object sender, RoutedEventArgs e)
        {
            mapPanel.ZoomOff();
        }

        private void map_ClearDrawings(object sender, RoutedEventArgs e)
        {
            mapPanel.ClearVisuals();
        }



        private void newScenarioClick(object sender, RoutedEventArgs e)
        {
            newScenario();
        }

        private void loadScenarioClick(object sender, RoutedEventArgs e)
        {
            loadScenario();
        }

        private void randomizeScenarioClick(object sender, RoutedEventArgs e)
        {
            randomizeScenario();
        }

        private void loadRecentlyOpenedScenario0Click(object sender, RoutedEventArgs e)
        {
            //loadScenario(Properties.Settings.Default.LastScenario);
            loadScenario(recentlyOpenedScenario0);
        }

        private void loadRecentlyOpenedScenario1Click(object sender, RoutedEventArgs e)
        {
            //loadScenario(Properties.Settings.Default.LastScenario);
            loadScenario(recentlyOpenedScenario1);
        }

        private void loadRecentlyOpenedScenario2Click(object sender, RoutedEventArgs e)
        {
            //loadScenario(Properties.Settings.Default.LastScenario);
            loadScenario(recentlyOpenedScenario2);
        }

        private void loadLastOOBClick(object sender, RoutedEventArgs e)
        {
            //loadOrderOfBattle(Properties.Settings.Default.LastOOB);
            loadOrderOfBattle(recentlyOpenedOOB);
        }

        private void loadStartLocsDirectClick(object sender, RoutedEventArgs e)
        {
            loadStartLocsDirect(null);
        }

        private void loadStartLocsDirectSelectedClick(object sender, RoutedEventArgs e)
        {
            loadStartLocsDirect(scenarioViewHelper.sharedEchelonSelection.units.ToList<ScenarioUnit>());
        }

        private void loadStartLocsClick(object sender, RoutedEventArgs e)
        {
            loadStartLocsWithModifiers(null);
        }

        private void loadStartLocsSelectedClick(object sender, RoutedEventArgs e)
        {
            loadStartLocsWithModifiers(scenarioViewHelper.sharedEchelonSelection.units.ToList<ScenarioUnit>());
        }

        private void loadGameDBClick(object sender, RoutedEventArgs e)
        {
            loadGameDB(null);
        }

        private void loadGameDBSelectedClick(object sender, RoutedEventArgs e)
        {
            loadGameDB(scenarioViewHelper.sharedEchelonSelection.units.ToList<ScenarioUnit>());
        }

        private void loadUnitLocsClick(object sender, RoutedEventArgs e)
        {
            string messageBoxText = "Function Not Yet Implemented";
            string caption = "SOW Data Editor";
            MessageBoxButton button = MessageBoxButton.OK;
            MessageBoxImage icon = MessageBoxImage.Warning;
            MessageBox.Show(messageBoxText, caption, button, icon);
        }

        private void loadOrderOfBattleClick(object sender, RoutedEventArgs e)
        {
            loadOrderOfBattle();
        }



        /// <summary>
        /// Mimics MultiCell Editing
        /// a giant PITA
        /// http://www.scottlogic.com/blog/2008/12/02/wpf-datagrid-detecting-clicked-cell-and-row.html
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal void dataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            //Console.WriteLine(sender + " Edited " + e.EditingElement);
            DataGrid dataGrid = (DataGrid)sender;

            // Get out if not multi-editing
            if (dataGrid.SelectedCells.Count < 2) return;

            if (e.EditingElement is ComboBox)
            {
                dataGrid_ComboCellEditEnding(sender, e);
                return;
            }

            //Only handles cases where the cell contains a TextBox

            string value = null;
            string propertyName = null;
            DataGridBoundColumn dgc = dataGrid.CurrentCell.Column as DataGridBoundColumn;

            if (dgc != null && e.EditingElement is TextBox)
            {
                TextBox editedTextbox = e.EditingElement as TextBox;
                value = editedTextbox.Text;

                Binding binding = dgc.Binding as Binding;
                propertyName = binding.Path.Path;
            }
            else
            {
                // this seems to happen when borders get clicked
                //Log.Error(this,"Cannot multi edit " + e.EditingElement);
                return;

            }
            //Console.WriteLine("propertyName:" + propertyName);

            // When SelectionUnit is FullRow, then all cells in row are considered selected
            // so only apply to those cells that have the same column

            foreach (DataGridCellInfo cellInfo in dataGrid.SelectedCells)
            {
                if (cellInfo.Item == e.EditingElement) continue;
                DataGridColumn column = cellInfo.Column;

                //skip not in same column
                if (e.Column != column) continue;

                //DataGridRow row = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromItem(cellInfo.Item);
                try
                {
                    cellInfo.Item.SetPropertyValue(propertyName, value);
                }
                catch
                {
                    e.Cancel = true;
                    // this freezes
                    // dataGrid.CancelEdit(DataGridEditingUnit.Cell);
                    MessageBox.Show("Cannot set " + propertyName + " to " + value + " on " + cellInfo.Item);
                }
            }
        }

        /// <summary>
        /// Multi Editing for ComboBoxes - super hacky, depends on header name to ge that of property
        /// as I cannot figure out how to get the binding path from a ComboBoxColumn
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGrid_ComboCellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            ComboBox editedComboBox = e.EditingElement as ComboBox;
            DataGrid dataGrid = (DataGrid)sender;

            object value = editedComboBox.SelectedItem;
            //binding = ((DataGridBoundColumn)dataGrid.CurrentCell.Column).Binding as Binding;
            //binding = BindingOperations.GetBinding(editedComboBox, ComboBox.ItemsSourceProperty);
            //binding = editedComboBox.BindingGroup.Items[0].; 
            //OMG THIS IS SO FUGLY!
            DataGridComboBoxColumn col = (DataGridComboBoxColumn)e.Column; // (DataGridComboBoxColumn)dataGrid.CurrentCell.Column;
            Binding binding = col.SelectedValueBinding as Binding;

            //string propertyName = ((DataGridColumn)dataGrid.CurrentCell.Column).Header.ToString();
            string propertyName = binding.Path.Path; ;

            //Console.WriteLine("propertyName:" + propertyName);

            //these are a special case
            if (propertyName.StartsWith("attributes"))
            {
                int rtrim = "attributes[".Length;

                string key = propertyName.Substring(rtrim, propertyName.Length - rtrim - 1);
                //Console.WriteLine("attributeKey:" + key);

                foreach (DataGridCellInfo cellInfo in dataGrid.SelectedCells)
                {
                    if (cellInfo.Item == e.EditingElement) continue;
                    //skip not in same column
                    if (e.Column != cellInfo.Column) continue;

                    DataGridRow row = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromItem(cellInfo.Item);

                    IUnit unit = (IUnit)row.Item;
                    unit.attributes[key] = (AttributeLevel)value;
                }

            }


            foreach (DataGridCellInfo cellInfo in dataGrid.SelectedCells)
            {
                if (cellInfo.Item == e.EditingElement) continue;
                DataGridColumn column = cellInfo.Column;
                DataGridRow row = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromItem(cellInfo.Item);

                //try
                //{
                System.Reflection.PropertyInfo pi = cellInfo.Item.GetType().GetProperty(propertyName);

                if (pi != null && pi.CanWrite)
                {
                    pi.SetValue(cellInfo.Item, value, null);
                }
                //}
                //catch
                //{
                //    e.Cancel = true;
                //    // this freezes
                //    //dataGrid.CancelEdit(DataGridEditingUnit.Cell);
                //    MessageBox.Show("Cannot set combo " + propertyName + " to " + value);
                //}

            }
        }


        /// <summary>
        /// Handle Delete Key on DataGrids
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void sdg_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Delete)
                return;

            //var res = MessageBox.Show("Proceed?", "Delete Records", MessageBoxButton.YesNoCancel, MessageBoxImage.Question, MessageBoxResult.No);
            //if (res == MessageBoxResult.Cancel || res == MessageBoxResult.No)
            scenarioDataGridHelper.PreviewKeyDown(sender, e);
            e.Handled = true;
        }

        #region ScenarioDataGrid Drag
        private void sdg_MouseDown(object sender, MouseButtonEventArgs e)
        {
            scenarioDataGridHelper.MouseDown(sender, e);
            return;
        }

        private void sdg_MouseMove(object sender, MouseEventArgs e)
        {
            scenarioDataGridHelper.MouseMove(sender, e);
            return;
        }

        private void sdg_DragOver(object sender, DragEventArgs e)
        {
            scenarioDataGridHelper.DragOver(sender, e);
        }

        private void sdg_Drop(object sender, DragEventArgs e)
        {
            scenarioDataGridHelper.Drop(sender, e);
        }

        private void sdg_FormationButtonClick(object sender, RoutedEventArgs e)
        {

            scenarioDataGridHelper.ShowFormationDialog(sender, e);
            return;
        }

        #endregion
        /// <summary>
        /// fix for yet another half completed feature of wpf
        /// updating group when group key changes
        /// http://social.msdn.microsoft.com/Forums/vstudio/en-US/418c81c3-db02-41a2-b1d5-48b3545645e2/wpf-datagrid-grouping-not-updating-after-item-edit-update?forum=wpf
        /// </summary>
        bool _pendingCsvRefresh = false;
        private void edg_CollectionViewSource_OnSourcePropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "block") return;
            if (_pendingCsvRefresh) return;
            _pendingCsvRefresh = true;
            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, new Action(() =>
            {
                _pendingCsvRefresh = false;
                ListCollectionView csv = eventDataGrid.ItemsSource as ListCollectionView;
                if (csv == null) return;
                try { csv.Refresh(); }
                catch { }
            }));
        }


        private void edg_addBattleScriptEvent(object sender, RoutedEventArgs e)
        {

            eventDataGridHelper.CreateEvent(sender, e);
        }


        private void edg_CollectionViewSource_Filter(object sender, FilterEventArgs e)
        {
            BattleScriptEvent t = e.Item as BattleScriptEvent;
            if (t != null)
            // If filter is turned on, filter completed items.
            {
                //if (this.cbCompleteFilter.IsChecked == true && t.Complete == true)
                //    e.Accepted = false;
                //else
                e.Accepted = true;
            }
        }

        private void edg_IconMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                Image image = sender as Image;
                ContextMenu contextMenu = image.ContextMenu;
                contextMenu.PlacementTarget = image;
                contextMenu.IsOpen = true;
                e.Handled = true;
            }
        }

        private void mapPanel_ToggleAllUnitsClick(object sender, RoutedEventArgs e)
        {
            if (((System.Windows.Controls.Primitives.ToggleButton)sender).IsChecked == true)
                mapPanel.ShowAllUnits();
            else
                mapPanel.HideAllUnits();
        }

        private void mapPanel_ToggleBlueUnitsClick(object sender, RoutedEventArgs e)
        {
            if (((System.Windows.Controls.Primitives.ToggleButton)sender).IsChecked == true)
                mapPanel.ShowAllUnitsOfSide(1);
            else
                mapPanel.HideAllUnitsOfSide(1);
        }

        private void mapPanel_ToggleRedUnitsClick(object sender, RoutedEventArgs e)
        {
            if (((System.Windows.Controls.Primitives.ToggleButton)sender).IsChecked == true)
                mapPanel.ShowAllUnitsOfSide(2);
            else
                mapPanel.HideAllUnitsOfSide(2);
        }

        private void mapPanel_ToggleGreenUnitsClick(object sender, RoutedEventArgs e)
        {
            if (((System.Windows.Controls.Primitives.ToggleButton)sender).IsChecked == true)
                mapPanel.ShowAllUnitsOfSide(3);
            else
                mapPanel.HideAllUnitsOfSide(3);
        }

        private void mapPanel_ToggleAllCommandsClick(object sender, RoutedEventArgs e)
        {
            if (((System.Windows.Controls.Primitives.ToggleButton)sender).IsChecked == true)
                mapPanel.ShowAllCommands();
            else
                mapPanel.HideAllCommands();
        }

        private void mapPanel_ToggleObjectivesClick(object sender, RoutedEventArgs e)
        {
            if (toggleObjectivesButton.IsChecked == true) mapPanel.ShowAllObjectivesCommands();
            else                                          mapPanel.HideAllObjectivesCommands();
        }

        private void edg_SortClick(object sender, RoutedEventArgs e)
        {
            eventDataGridHelper.Sort();
        }
        private void edg_GroupVisibiltyClick(object sender, RoutedEventArgs e)
        {

            CollectionViewGroup grp = ((CollectionViewGroup)((FrameworkElement)sender).DataContext);

            mapPanel.ToggleVisibility(grp.Items);
            //foreach (object item in grp.Items)
            //{
            //    Event bEvent = (Event)item;
            //    Console.WriteLine("edg_VisibiltyClick " + bEvent);
            //}
        }

        private void edg_VisibiltyClick(object sender, RoutedEventArgs e)
        {

            BattleScriptEvent bEvent = ((BattleScriptEvent)((FrameworkElement)sender).DataContext);

            mapPanel.ToggleVisibility(bEvent);
            //foreach (object item in grp.Items)
            //{
            //    Event bEvent = (Event)item;
            //    Console.WriteLine("edg_VisibiltyClick " + bEvent);
            //}
        }

        private void edg_SetTriggerClick(object sender, RoutedEventArgs e)
        {

            TimeEvent tEvent = ((Button)sender).DataContext as TimeEvent;

            if (tEvent != null)
            {
                eventDataGridHelper.ShowTimeDialog(sender, e, tEvent);
                return;
            }

            // -		base	{MS.Internal.Data.CollectionViewGroupInternal}	System.Windows.Data.CollectionViewGroup {MS.Internal.Data.CollectionViewGroupInternal}
            System.Windows.Data.CollectionViewGroup group = ((Button)sender).Content as System.Windows.Data.CollectionViewGroup;
            if (group != null)
            {
                if (group.ItemCount < 1) return;
                tEvent = group.Items[0] as TimeEvent;
                if (tEvent != null)
                {
                    eventDataGridHelper.ShowTimeDialog(sender, e, group);
                }
                return;

            }


            //mapPanel.ToggleVisibility(bEvent);
            //foreach (object item in grp.Items)
            //{
            //    Event bEvent = (Event)item;
            //    Console.WriteLine("edg_VisibiltyClick " + bEvent);
            //}
        }

        private void edg_DuplicateBeforeClick(object sender, RoutedEventArgs e)
        {
            if (sender == null) return;
            eventDataGridHelper.DuplicateBefore(sender, e);
        }

        private void edg_DuplicateAfterClick(object sender, RoutedEventArgs e)
        {
            if (sender == null) return;
            eventDataGridHelper.DuplicateAfter(sender, e);
        }

        private void edg_CreateEvtContClick(object sender, RoutedEventArgs e)
        {
            if (sender == null) return;
            eventDataGridHelper.CreateEvtContClick(sender, e);
        }

        private void edg_CreateAfterClick(object sender, RoutedEventArgs e)
        {
            if (sender == null) return;
            eventDataGridHelper.CreateEventAfter(sender, e);
        }

        private void edg_DeleteClick(object sender, RoutedEventArgs e)
        {
            if (sender == null) return;
            eventDataGridHelper.Delete(sender, e);
        }

        private void edg_TriggerButtonClick(object sender, RoutedEventArgs e)
        {

            eventDataGridHelper.ShowEventDialog(sender);
            return;
        }

        private void edg_RandomEventTriggerButtonClick(object sender, RoutedEventArgs e)
        {

            eventDataGridHelper.ShowRandomEventDialog();
            return;
        }

        private void edg_RandomEventCommandButtonClick(object sender, RoutedEventArgs e)
        {

            eventDataGridHelper.ShowRandomEventDialog();
            return;
        }

        private void edg_UnitButtonClick(object sender, RoutedEventArgs e)
        {

            eventDataGridHelper.UnitButtonClick(sender, e);
            return;
        }

        private void edg_CommandButtonClick(object sender, RoutedEventArgs e)
        {

            eventDataGridHelper.ShowCommandDialog(sender);
            return;
        }

        private void edg_FormTypeArgButtonClick(object sender, RoutedEventArgs e)
        {

            eventDataGridHelper.ShowFormTypeDialog(sender);
            return;
        }

        //private void edg_MoveSpecClick(object sender, RoutedEventArgs e)
        //{

        //    eventDataGridHelper.ShowMoveSpecDialog(sender);
        //    return;
        //}

        //private void edg_DayWheel(object sender, MouseWheelEventArgs e)
        //{
        //    //TODO Make suer we have keyboard focus
        //    e.Handled = eventDataGridHelper.IntWheel(sender, e);
        //}

        //private void edg_HourWheel(object sender, MouseWheelEventArgs e)
        //{
        //    //TODO Make suer we have keyboard focus
        //    e.Handled = eventDataGridHelper.IntWheel(sender, e);
        //}

        //private void edg_MinuteWheel(object sender, MouseWheelEventArgs e)
        //{
        //    e.Handled = eventDataGridHelper.IntWheel(sender, e);

        //}

        //private void edg_SecondWheel(object sender, MouseWheelEventArgs e)
        //{
        //    e.Handled = eventDataGridHelper.IntWheel(sender, e);

        //}

        //private void edg_DayChanged(object sender, TextChangedEventArgs e)
        //{
        //    eventDataGridHelper.DayChanged(sender);
        //}

        //private void edg_HourChanged(object sender, TextChangedEventArgs e)
        //{
        //    eventDataGridHelper.HourChanged(sender);
        //}

        //private void edg_MinuteChanged(object sender, TextChangedEventArgs e)
        //{
        //    eventDataGridHelper.MinuteChanged(sender);
        //}

        //private void edg_SecondChanged(object sender, TextChangedEventArgs e)
        //{
        //    eventDataGridHelper.SecondChanged(sender);
        //}

        //private void edg_DayLostFocus(object sender, RoutedEventArgs e)
        //{
        //    eventDataGridHelper.DayChanged(sender);
        //}
        //private void edg_HourLostFocus(object sender, RoutedEventArgs e)
        //{
        //    eventDataGridHelper.HourChanged(sender);
        //}
        //private void edg_MinuteLostFocus(object sender, RoutedEventArgs e)
        //{
        //    eventDataGridHelper.MinuteChanged(sender);
        //}
        //private void edg_SecondLostFocus(object sender, RoutedEventArgs e)
        //{
        //    eventDataGridHelper.SecondChanged(sender);
        //}
        #region EventDataGrid DragDrop
        private void edg_MouseDown(object sender, MouseButtonEventArgs e)
        {
            eventDataGridHelper.MouseDown(sender, e);
            return;
        }

        private void edg_MouseMove(object sender, MouseEventArgs e)
        {

            eventDataGridHelper.MouseMove(sender, e);
            return;
        }

        /// <summary>
        /// Captures mouse movement where we don't want dragdrop
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void edg_ValueMouseMove(object sender, MouseEventArgs e)
        {
            e.Handled = true;

        }

        //private void edg_DragOver(object sender, DragEventArgs e)
        //{
        //    eventDataGridHelper.DragOver(sender, e);
        //    return;
        //}
        private void edg_Drop(object sender, DragEventArgs e)
        {
            eventDataGridHelper.Drop(sender, e);
            return;
        }

        private void edg_CommandDrop(object sender, DragEventArgs e)
        {
            eventDataGridHelper.CommandDrop(sender, e);
            return;
        }

        #endregion

        private void odg_AddClick(object sender, RoutedEventArgs e)
        {
            if (scenario == null) return;
            string id = scenario.objectives.GetUniqueId("NEW");
            ScenarioObjective obj = new ScenarioObjective(id);
            obj.beg = scenario.startTime;
            obj.end = scenario.startTime.Add(new TimeSpan(4, 0, 0));
            scenario.objectives.Add(obj);
            objectiveDataGrid.ItemsSource = new ObservableCollection<ScenarioObjective>(scenario.objectives.Values);

        }

        private void odg_BeginButtonClick(object sender, RoutedEventArgs e)
        {
            scenarioObjectiveDataGridHelper.ShowBeginTimeDialog(sender, e);
        }

        private void odg_EndButtonClick(object sender, RoutedEventArgs e)
        {
            scenarioObjectiveDataGridHelper.ShowEndTimeDialog(sender, e);
        }

        private void odg_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                var grid = (DataGrid)sender;

                if (grid.SelectedItems.Count > 0)
                {
                    string checkMessage = "The following will be removed: ";

                    foreach (var row in grid.SelectedItems)
                    {
                        ScenarioObjective customer = row as ScenarioObjective;
                        checkMessage += customer.id + ",";
                    }
                    //checkMessage = Regex.Replace(checkMessage, ",$", "");

                    var result = MessageBox.Show(checkMessage, "Delete", MessageBoxButton.OKCancel);
                    if (result == MessageBoxResult.OK)
                    {
                        foreach (var row in grid.SelectedItems)
                        {
                            ScenarioObjective customer = row as ScenarioObjective;
                            scenario.objectives.Remove(customer.id);
                        }

                    }

                }
            }
        }

        #region ObjectiveDataGrid DragDrop
        private void odg_MouseDown(object sender, MouseButtonEventArgs e)
        {
            scenarioObjectiveDataGridHelper.MouseDown(sender, e);
            return;
        }

        private void odg_MouseMove(object sender, MouseEventArgs e)
        {

            scenarioObjectiveDataGridHelper.MouseMove(sender, e);
            return;
        }

        private void odg_Drop(object sender, DragEventArgs e)
        {
            scenarioObjectiveDataGridHelper.Drop(sender, e);
            return;
        }
        #endregion

        #region MapObjectiveDataGrid DragDrop
        private void modg_MouseDown(object sender, MouseButtonEventArgs e)
        {
            mapObjectiveDataGridHelper.MouseDown(sender, e);
            return;
        }

        private void modg_MouseMove(object sender, MouseEventArgs e)
        {

            mapObjectiveDataGridHelper.MouseMove(sender, e);
            return;
        }

        #endregion

        #region Messages Tab

        // Replicates the internal ScreenReader parsing logic so the editor can read the template
        // without requiring a DLL rebuild to make ScreenReader public.
        IEnumerable<ScreenMessage> ParseScreenTemplate(string filepath)
        {
            var messages = new List<ScreenMessage>();
            ScreenMessage current = null;
            foreach (string line in System.IO.File.ReadLines(filepath, NorbSoftDev.SOW.Config.TextFileEncoding))
            {
                if (line.StartsWith("$"))
                {
                    var parts = line.Split(new char[]{' ', '\t'}, 2);
                    string id = parts[0].Substring(1);
                    string firstLine = parts.Length > 1 ? parts[1] : string.Empty;
                    current = new ScreenMessage(id, new List<string> { firstLine });
                    messages.Add(current);
                }
                else if (current != null)
                {
                    current.AddLine(line);
                }
            }
            return messages;
        }

        private void msg_AddClick(object sender, RoutedEventArgs e)
        {
            if (scenario == null) return;
            int n = scenario.screenMessages.Count + 1;
            string newId = "MSG_" + n;
            while (scenario.screenMessages.ContainsKey(newId))
                newId = "MSG_" + (++n);
            var msg = new ScreenMessage(newId, new System.Collections.Generic.List<string>());
            _screenMessagesCollection.Add(msg);
            screensDataGrid.SelectedItem = msg;
            screensDataGrid.ScrollIntoView(msg);
        }

        private void msg_DeleteClick(object sender, RoutedEventArgs e)
        {
            if (scenario == null) return;
            var selected = new System.Collections.Generic.List<ScreenMessage>();
            foreach (var item in screensDataGrid.SelectedItems)
            {
                if (item is ScreenMessage sm) selected.Add(sm);
            }
            foreach (var msg in selected)
                _screenMessagesCollection.Remove(msg);
        }

        private void msg_SaveClick(object sender, RoutedEventArgs e)
        {
            if (scenario == null) return;
            // Commit any in-progress cell edit first
            screensDataGrid.CommitEdit(DataGridEditingUnit.Row, true);
            // Rebuild dictionary from collection (handles id renames and deletions)
            scenario.screenMessages.Clear();
            foreach (ScreenMessage msg in _screenMessagesCollection)
                if (!string.IsNullOrEmpty(msg.id))
                    scenario.screenMessages[msg.id] = msg;
            // Write EnglishScenScreen.txt directly
            string filepath = System.IO.Path.Combine(scenario.scenarioDir, "EnglishScenScreen.txt");
            using (var writer = new System.IO.StreamWriter(filepath, false, NorbSoftDev.SOW.Config.TextFileEncoding))
            {
                foreach (ScreenMessage msg in scenario.screenMessages.Values)
                    writer.Write(string.Format("${0} {1}{2}", msg.id, msg.contents, NorbSoftDev.SOW.Config.NewLine));
            }
            Log.Info(this, "[Messages] Saved " + scenario.screenMessages.Count + " messages to EnglishScenScreen.txt");
        }

        private void intro_SaveClick(object sender, RoutedEventArgs e)
        {
            if (scenario == null) return;
            string filepath = System.IO.Path.Combine(scenario.scenarioDir, "EnglishScenIntro.txt");
            using (var writer = new System.IO.StreamWriter(filepath, false, NorbSoftDev.SOW.Config.TextFileEncoding))
                writer.Write(introTextBox.Text);
            Log.Info(this, "[Intro] Saved EnglishScenIntro.txt");
        }

        #region MessageScreensDataGrid DragDrop
        private void mdg_MouseDown(object sender, MouseButtonEventArgs e)
        {
            messagesDataGridHelper.MouseDown(sender, e);
            return;
        }

        private void mdg_MouseMove(object sender, MouseEventArgs e)
        {

            messagesDataGridHelper.MouseMove(sender, e);
            return;
        }
        #endregion

        #endregion

        #region FortsDataGrid DragDrop
        private void fdg_MouseDown(object sender, MouseButtonEventArgs e)
        {
            fortsDataGridHelper.MouseDown(sender, e);
            return;
        }

        private void fdg_MouseMove(object sender, MouseEventArgs e)
        {

            fortsDataGridHelper.MouseMove(sender, e);
            return;
        }

        #endregion

        #region ScenarioTree DragDrop
        private void stv_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            scenarioViewHelper.SelectedItemChanged(sender, e);
        }

        private void stv_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            scenarioViewHelper.MouseLeftButtonDown(sender, e);
            return;
        }

        private void stv_MouseMove(object sender, MouseEventArgs e)
        {
            scenarioViewHelper.MouseMove(sender, e);
            return;
        }

        private void stv_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            scenarioViewHelper.MouseRightButtonDown(sender, e);
        }


        private void SetActiveEchelonButton(System.Windows.Controls.Primitives.ToggleButton active)
        {
            foreach (var btn in new System.Windows.Controls.Primitives.ToggleButton[] { stvBtnArmy, stvBtnCorps, stvBtnDivision, stvBtnBrigade, stvBtnRegiment })
                btn.IsChecked = (btn == active);
        }

        private void stv_CollapseToArmy(object sender, RoutedEventArgs e)
        {
            SetActiveEchelonButton(stvBtnArmy);
            scenarioViewHelper.CollapseBelowRank(ERank.Army);
        }

        private void stv_CollapseToCorps(object sender, RoutedEventArgs e)
        {
            SetActiveEchelonButton(stvBtnCorps);
            scenarioViewHelper.CollapseBelowRank(ERank.Corps);
        }

        private void stv_CollapseToDivision(object sender, RoutedEventArgs e)
        {
            SetActiveEchelonButton(stvBtnDivision);
            scenarioViewHelper.CollapseBelowRank(ERank.Division);
        }

        private void stv_CollapseToBrigade(object sender, RoutedEventArgs e)
        {
            SetActiveEchelonButton(stvBtnBrigade);
            scenarioViewHelper.CollapseBelowRank(ERank.Brigade);
        }

        private void stv_CollapseToRegiment(object sender, RoutedEventArgs e)
        {
            SetActiveEchelonButton(stvBtnRegiment);
            scenarioViewHelper.CollapseBelowRank(ERank.Regiment);
        }

        /// <summary>
        /// http://stackoverflow.com/questions/6037883/how-to-disable-double-click-behaviour-in-a-wpf-treeview
        /// apparnelty it has to be handled in preview here, if I handle it in doubleclick event,
        /// it trickles upwards even if handled
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void stv_PreviewMouseDoubleClick(object sender, MouseEventArgs e)
        {
            //this will suppress the event that is causing the nodes to expand/contract 
            if (e.Handled) return;
            //Console.WriteLine("stv_PreviewMouseDoubleClick: sender:" + sender + " e:" + e.OriginalSource);
            TreeViewItem tvi = sender as TreeViewItem;
            if (tvi == null) return;

            ScenarioEchelon echelon = tvi.Header as ScenarioEchelon;
            if (echelon == null) return;

            mapPanel.Center(echelon);
            e.Handled = true;
            return;
        }

        private void stv_DragOver(object sender, DragEventArgs e)
        {
            scenarioViewHelper.DragOver(sender, e);
            return;
        }
        private void stv_Drop(object sender, DragEventArgs e)
        {
            scenarioViewHelper.Drop(sender, e);
            return;
        }

        private void pe_DragOver(object sender, DragEventArgs e)
        {
            if (scenario == null)
            {
                e.Effects = DragDropEffects.None;
            }
            else
            {
                e.Effects = DragDropEffects.Move;
            }

            e.Handled = true;
        }

        private void pe_Drop(object sender, DragEventArgs e)
        {
            if (scenario == null) return;

            EchelonSelectionSet<ScenarioEchelon, ScenarioUnit> scenarioSelection = EchelonSelectionSet<ScenarioEchelon, ScenarioUnit>.ExtractFromDataObject(e.Data as DataObject);
            if (scenarioSelection == null || scenarioSelection.Count < 1) return;
            scenario.playerEchelon = scenarioSelection[0];
        }

        private void setPlayerCharacterClick(Object sender, RoutedEventArgs e)
        {
            if (scenario == null) return;
            MenuItem mi = sender as MenuItem;
            if (mi == null) return;
            ScenarioEchelon scenarioEchelon = mi.CommandParameter as ScenarioEchelon;
            if (scenarioEchelon == null || scenarioEchelon.unit == null) return;

            scenario.playerEchelon = scenarioEchelon;
        }

        private void populateEchelonsFromOrderOfBattleClick(Object sender, RoutedEventArgs e)
        {
            if (scenario == null) return;
            scenario.PopulateEchelonsFromOrderOfBattle();
        }
        #endregion

        #region CreateDialogButtons
        private void HasFormationButtonClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            IHasFormation iHasFormation = ((IHasFormation)button.DataContext);
            FormationDialog dialog = new FormationDialog();
            List<IHasFormation> list = new List<IHasFormation>();
            list.Add(iHasFormation);

            dialog.DataContext = list;
            dialog.SetListSource(config.formations.Values);
            dialog.PositionRelative();
            dialog.ShowDialog();
        }
        #endregion



        #region RelayCommands

        //private RelayCommand _setPlayerCharacterCommand;
        //public RelayCommand SetPlayerCharacterCommand
        //{
        //    get
        //    {
        //        return _setPlayerCharacterCommand
        //          ?? (_setPlayerCharacterCommand = new RelayCommand(
        //            execute =>
        //            {
        //                setPlayerCharacter();
        //            },

        //            canexecute =>
        //            {
        //                return saveScenario_CanExecute();
        //            }
        //            ));
        //    }
        //}
        //private void setPlayerCharacter()
        //{
        //    if (mapPanel.unitSharedSelection == null || mapPanel.unitSharedSelection.Count < 1) return;
        //    scenario.playerEchelon = mapPanel.unitSharedSelection[0];

        //    //if (scenario == null) return;
        //    //ScenarioEchelon scenarioEchelon = obj as ScenarioEchelon;
        //    //if (scenarioEchelon == null || scenarioEchelon.unit == null) return;
        //    //scenario.playerEchelon = scenarioEchelon;
        //}

        private RelayCommand _saveScenarioCommand;
        /// <summary>
        ///  relay commands wrap Execute and CanExecute functions to work with dumb WPF abstraction
        ///  see http://msdn.microsoft.com/en-us/magazine/dn237302.aspx
        /// </summary>
        public RelayCommand SaveScenarioCommand
        {
            get
            {
                return _saveScenarioCommand
                  ?? (_saveScenarioCommand = new RelayCommand(
                    execute =>
                    {
                        saveScenario();
                    },
                    canexecute =>
                    {
                        return saveScenario_CanExecute();
                    }
                    ));
            }
        }

        private RelayCommand _saveUserScenarioCommand;
        /// <summary>
        ///  relay commands wrap Execute and CanExecute functions to work with dumb WPF abstraction
        ///  see http://msdn.microsoft.com/en-us/magazine/dn237302.aspx
        /// </summary>
        public RelayCommand SaveUserScenarioCommand
        {
            get
            {
                return _saveUserScenarioCommand
                  ?? (_saveUserScenarioCommand = new RelayCommand(
                    execute =>
                    {
                        saveUserScenario(false);
                    },
                    canexecute =>
                    {
                        return true;
                    }
                    ));
            }
        }

        private RelayCommand _saveOrderOfBattleCommand;
        /// <summary>
        ///  relay commands wrap Execute and CanExecute functions to work with dumb WPF abstraction
        ///  see http://msdn.microsoft.com/en-us/magazine/dn237302.aspx
        /// </summary>
        public RelayCommand SaveOrderOfBattleCommand
        {
            get
            {
                return _saveOrderOfBattleCommand
                  ?? (_saveOrderOfBattleCommand = new RelayCommand(
                    execute =>
                    {
                        saveOrderOfBattle();
                    },
                    canexecute =>
                    {
                        return saveOrderOfBattle_CanExecute();
                    }
                    ));
            }
        }

        private RelayCommand _exportOrderOfBattleCommand;
        /// <summary>
        ///  relay commands wrap Execute and CanExecute functions to work with dumb WPF abstraction
        ///  see http://msdn.microsoft.com/en-us/magazine/dn237302.aspx
        /// </summary>
        public RelayCommand ExportOrderOfBattleCommand
        {
            get
            {
                return _exportOrderOfBattleCommand
                  ?? (_exportOrderOfBattleCommand = new RelayCommand(
                    execute =>
                    {
                        exportOrderOfBattle();
                    },
                    canexecute =>
                    {
                        return exportOrderOfBattle_CanExecute();
                    }
                    ));
            }
        }

        #endregion

        private RelayCommand _removeScenarioEchelon;

        public RelayCommand RemoveScenarioEchelon
        {
            get
            {
                return _removeScenarioEchelon
                  ?? (_removeScenarioEchelon = new RelayCommand(
                    execute =>
                    {
                        removeScenarioEchelon();
                    },
                    canexecute =>
                    {
                        return removeScenarioEchelon_CanExecute();
                    }
                    ));
            }
        }

        void removeScenarioEchelon()
        {
            Console.WriteLine("foo");
        }

        bool removeScenarioEchelon_CanExecute()
        {
            return true;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SaveEditorIni();


            if (scenario != null && scenario.isDirty && scenario.Count > 0)
            {
                bool proceed = true;

                if (saveScenario_CanExecute())
                {
                    proceed = saveAsScenario(null, true);
                }
                else
                {
                    proceed = saveUserScenario(true);
                }

                if (!proceed)
                {
                    //user aborted
                    e.Cancel = true;
                    return;
                }
            }

            try
            {
                if (scenario != null)
                {
                    Log.Info(this, "AutoSaving to AutoSave");
                    scenario.AutoSave();
                }
            }
            catch (Exception ex)
            {
                Log.Warn(this, "AutoSave failed: " + ex.Message);
            }


            Log.CloseUserLog();
            if (helpWindow != null) helpWindow.Close();
            if (_oobWindow != null) _oobWindow.Close();

            Application.Current.Shutdown();
        }

        static string _resourcesDir;
        public static Uri GetResourceUri(string name)
        {
            if (_resourcesDir == null)
            {

                DirectoryInfo parentDir = (new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory));
                //Path.GetDirectoryName( System.Reflection.Assembly.GetExecutingAssembly().Location ),

                while (parentDir.FullName != parentDir.Root.FullName)
                {
                    DirectoryInfo[] subdirs = parentDir.GetDirectories("Resources");
                    if (subdirs.Length > 0)
                    {
                        _resourcesDir = subdirs[0].FullName;
                        break;
                    }

                    parentDir = parentDir.Parent;
                }
            }
            string filepath = System.IO.Path.Combine(_resourcesDir, name);

            return new Uri(filepath);
        }


        void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {

            try
            {
                if (scenario != null)
                {
                    Log.Info(this, "AutoSaving to AutoSave");
                    scenario.AutoSave();
                }
            }
            catch (Exception autoSaveEx)
            {
                Log.Warn(this, "AutoSave failed during crash handler: " + autoSaveEx.Message);
            }

            Log.Error(this, "Please attach this log your bug report");
            // Prevent default unhandled exception processing

            string msg = e.Exception.GetType() + ".\n+" + e.Exception.Message;
            Log.Exception(sender, e.Exception);

            if (e.Exception.InnerException != null)
            {
                try
                {
                    msg = e.Exception.InnerException.GetType() + "\n+" + e.Exception.InnerException.Message;
                    if (e.Exception.InnerException.Data.Contains("UserMessage"))
                    {
                        msg += "\n" + e.Exception.InnerException.Data["UserMessage"].ToString();
                    }
                    Log.Exception(e.Exception.InnerException, e.Exception.InnerException);
                }
                catch { }
            }

            try
            {
                if (e.Exception.Data.Contains("UserMessage"))
                {
                    msg += "\n" + e.Exception.Data["UserMessage"].ToString();
                }
            }
            catch { }



            string messageBoxText = "SOW ScenarioEditor Failed with a \n\n" + msg + "+\n\nPlease send Documents\\SowGB\\ScenarioEditor.log with a short description of what you were doing.";
            string caption = "SOW ScenarioEditor Exception";
            MessageBoxButton button = MessageBoxButton.OK;
            MessageBoxImage icon = MessageBoxImage.Error;
            MessageBox.Show(messageBoxText, caption, button, icon);
            e.Handled = true;
            Environment.Exit(1);

        }


        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        // Create the OnPropertyChanged method to raise the event 
        protected void OnPropertyChanged(string name)
        {
            // PropertyChangedEventHandler handler = PropertyChanged;
            // if (handler != null)
            // {
            //     handler(this, new PropertyChangedEventArgs(name));
            // }

            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }

        }

        protected void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }
        #endregion

        private void infoTab_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void ResetUIForGameChange()
        {
            // Clear all tabs that have explicitly-assigned ItemSources so stale
            // data from the previous game doesn't remain after a game switch.
            objectiveDataGrid.ItemsSource    = null;
            mapObjectiveDataGrid.ItemsSource = null;
            fortsDataGrid.ItemsSource        = null;
            _screenMessagesCollection.Clear();
            screensDataGrid.ItemsSource      = null;
            introTextBox.Text                = string.Empty;
            scenarioDataGrid.ItemsSource     = null;
            eventDataGrid.ItemsSource        = null;
        }

        private void ShowGameLabel()
        {
            if (config == null) { progressBarTextBlock.Text = ""; return; }
            progressBarTextBlock.Text = config.gameBase == Config.GAME_BASE_WL
                ? "Scourge of War: Waterloo"
                : "Scourge of War: Gettysburg";
        }

        private void warningsListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            NorbSoftDev.SOW.LogEntry entry = warningsListBox.SelectedItem as NorbSoftDev.SOW.LogEntry;
            if (entry?.Target == null) return;

            ScenarioUnit unit = entry.Target as ScenarioUnit;
            if (unit != null && scenario != null)
            {
                // Switch to Units tab (index 0) and select the unit
                mainTabControl.SelectedIndex = 0;
                scenarioDataGrid.SelectedValue = unit.id;
                scenarioDataGrid.ScrollIntoView(unit);
                return;
            }

            BattleScriptEvent bEvent = entry.Target as BattleScriptEvent;
            if (bEvent != null)
            {
                // Switch to BattleScript tab (index 1) and select the event
                mainTabControl.SelectedIndex = 1;
                eventDataGrid.SelectedItem = bEvent;
                eventDataGrid.ScrollIntoView(bEvent);
            }
        }

        private void undo_Off(object sender, RoutedEventArgs e)
        {
            ScenarioUndoStack.active = false;
        }

        private void undo_On(object sender, RoutedEventArgs e)
        {
            ScenarioUndoStack.active = true;
        }

        private void SelectButtonClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button == null) return;
            ScenarioEchelon data = button.CommandParameter as ScenarioEchelon;


            List<ScenarioEchelon> newSelection = new List<ScenarioEchelon>();

            newSelection.Add(data);

            if (!((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control))
            {
                scenarioViewHelper.sharedEchelonSelection.Clear();
            }

            scenarioViewHelper.sharedEchelonSelection.AddRange(newSelection);
        }

        private void PositionButtonClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button == null) return;
            Position p = button.CommandParameter as Position;
            if (p == null) return;

            mapPanel.Center(p);
        }

        private void IHasTransformPositionButtonClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button == null) return;
            IHasPosition p = button.CommandParameter as IHasPosition;
            if (p == null) return;

            mapPanel.Center(p.position);
        }

        private void ShowMasterOOBClick(object sender, RoutedEventArgs e)
        {
            if (_oobWindow == null) _oobWindow = new OOBWindow(this);
            _oobWindow.Owner = this;
            if (scenario != null) _oobWindow.DataContext = scenario;
            _oobWindow.Show();
            _oobWindow.Activate();
        }

        private void assignMenuItemPlayerClick(object sender, RoutedEventArgs e)
        {
            if (mapPanel.unitSharedSelection == null || mapPanel.unitSharedSelection.Count < 1) return;
            scenario.playerEchelon = mapPanel.unitSharedSelection[0];

            //if (scenario == null) return;
            //ScenarioEchelon scenarioEchelon = obj as ScenarioEchelon;
            //if (scenarioEchelon == null || scenarioEchelon.unit == null) return;
            //scenario.playerEchelon = scenarioEchelon;
        }

    }



    static class ObjectExtensions
    {
        public static void SetPropertyValue<T>(this object obj, string propertyName, T propertyValue)
            where T : IConvertible
        {
            System.Reflection.PropertyInfo pi = obj.GetType().GetProperty(propertyName);

            if (pi != null && pi.CanWrite)
            {
                pi.SetValue
                (
                    obj,
                    Convert.ChangeType(propertyValue, pi.PropertyType),
                    null
                );
            }
        }
    }

}