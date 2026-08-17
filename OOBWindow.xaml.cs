using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using NorbSoftDev.SOW;

namespace ScenarioEditor
{
    public partial class OOBWindow : Window
    {
        private readonly SOWScenarioEditorWindow _mainWindow;
        internal TreeViewHelper<OOBEchelon, OOBUnit> oobViewHelper;
        internal RosterDataGridHelper<OOBEchelon, OOBUnit> oobDataGridHelper;
        private bool _helpersInitialized = false;

        public OOBWindow(SOWScenarioEditorWindow mainWindow)
        {
            _mainWindow = mainWindow;
            InitializeComponent();
        }

        private void OOBWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (_helpersInitialized) return;
            _helpersInitialized = true;
            oobViewHelper = new TreeViewHelper<OOBEchelon, OOBUnit>(_mainWindow, orderOfBattleTree, false);
            oobDataGridHelper = new RosterDataGridHelper<OOBEchelon, OOBUnit>(_mainWindow, _mainWindow.mapPanel, orderOfBattleDataGrid);
            // Collapse to division level now that helpers are ready
            if (DataContext != null)
                oobViewHelper.CollapseBelowRank(ERank.Division);
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            // Hide instead of close so the window can be reopened
            e.Cancel = true;
            Hide();
        }

        internal void AssignItemSources(Config config)
        {
            unitClass.ItemsSource = config.unitClasses.Values;
        }

        internal void BindAttributes(Scenario scenario, Config config)
        {
            // Clear any previously added dynamic columns (keep the fixed ones)
            int fixedColumns = 13; // number of static columns defined in XAML
            while (orderOfBattleDataGrid.Columns.Count > fixedColumns)
                orderOfBattleDataGrid.Columns.RemoveAt(fixedColumns);

            foreach (NorbSoftDev.SOW.Attribute attribute in scenario.orderOfBattle.attributeNames)
            {
                NorbSoftDev.SOW.Attribute levels = config.attributes[attribute.name];
                Log.Info(this, "[BindAttributes] OOB \"" + attribute.name + "\" has " + (levels != null ? levels.Count.ToString() : "null") + " levels");
                DataGridComboBoxColumn col = new DataGridComboBoxColumn();
                col.ItemsSource = levels;
                col.Header = attribute.name;
                col.SelectedValueBinding = new Binding("attributes[" + attribute.name + "]");
                orderOfBattleDataGrid.Columns.Add(col);
            }
        }

        internal void SelectId(string id, OrderOfBattle oob)
        {
            OOBUnit ou;
            if (oob.TryGetUnitByIdOrName(id, out ou))
            {
                orderOfBattleDataGrid.SelectedValue = id;
                orderOfBattleDataGrid.ScrollIntoView(ou);
            }
        }

        #region Collapse level buttons
        private void otv_CollapseToArmy(object sender, RoutedEventArgs e)     { if (oobViewHelper != null) oobViewHelper.CollapseBelowRank(ERank.Army); }
        private void otv_CollapseToCorps(object sender, RoutedEventArgs e)    { if (oobViewHelper != null) oobViewHelper.CollapseBelowRank(ERank.Corps); }
        private void otv_CollapseToDivision(object sender, RoutedEventArgs e) { if (oobViewHelper != null) oobViewHelper.CollapseBelowRank(ERank.Division); }
        private void otv_CollapseToBrigade(object sender, RoutedEventArgs e)  { if (oobViewHelper != null) oobViewHelper.CollapseBelowRank(ERank.Brigade); }
        private void otv_CollapseToRegiment(object sender, RoutedEventArgs e) { if (oobViewHelper != null) oobViewHelper.CollapseBelowRank(ERank.Regiment); }
        #endregion

        #region OOB TreeView
        private bool _contextMenuIsOpen = false;

        private void otv_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            _contextMenuIsOpen = true;
        }

        private void otv_ContextMenuClosing(object sender, ContextMenuEventArgs e)
        {
            _contextMenuIsOpen = false;
        }

        private void otv_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            oobViewHelper?.SelectedItemChanged(sender, e);
        }
        private void otv_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // When the context menu is open, a left-click is just dismissing it — don't treat it as a unit selection/drag.
            if (_contextMenuIsOpen) return;
            oobViewHelper?.MouseLeftButtonDown(sender, e);
        }
        private void otv_MouseMove(object sender, MouseEventArgs e)
        {
            oobViewHelper?.MouseMove(sender, e);
        }
        private void otv_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            oobViewHelper?.MouseRightButtonDown(sender, e);
        }
        private void otv_DragOver(object sender, DragEventArgs e)
        {
            oobViewHelper?.DragOver(sender, e);
        }
        private void otv_Drop(object sender, DragEventArgs e)
        {
            oobViewHelper?.Drop(sender, e);
        }
        private void otv_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Scenario scenario = SOWScenarioEditorWindow.GetScenario();
            if (scenario == null) return;
            OOBEchelon echelon = (sender as TreeViewItem)?.DataContext as OOBEchelon;
            if (echelon?.unit == null) return;
            e.Handled = true;
            try { scenario.InsertUnit(echelon.unit); }
            catch (Exception ex) { Log.Error(this, ex.Message); }
        }
        #endregion

        #region OOB DataGrid
        private void oobdg_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Delete) return;
            oobDataGridHelper?.PreviewKeyDown(sender, e);
            e.Handled = true;
        }
        private void oobdg_WeaponButtonClick(object sender, RoutedEventArgs e)
        {
            oobDataGridHelper?.ShowWeaponDialog(sender, e);
        }
        private void oobdg_Flag1ButtonClick(object sender, RoutedEventArgs e)
        {
            oobDataGridHelper?.ShowFlagDialog(sender, e, 1);
        }
        private void oobdg_Flag2ButtonClick(object sender, RoutedEventArgs e)
        {
            oobDataGridHelper?.ShowFlagDialog(sender, e, 2);
        }
        private void dataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            _mainWindow.dataGrid_CellEditEnding(sender, e);
        }
        #endregion

        #region Formation
        private void HasFormationButtonClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            IHasFormation iHasFormation = button?.DataContext as IHasFormation;
            if (iHasFormation == null) return;
            FormationDialog dialog = new FormationDialog();
            var list = new List<IHasFormation> { iHasFormation };
            dialog.DataContext = list;
            dialog.SetListSource(_mainWindow.config.formations.Values);
            dialog.PositionRelative();
            dialog.ShowDialog();
        }
        #endregion

        private void loadOrderOfBattleClick(object sender, RoutedEventArgs e)
        {
            _mainWindow.loadOrderOfBattle();
        }
    }
}
