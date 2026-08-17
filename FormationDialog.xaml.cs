using NorbSoftDev.SOW;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace ScenarioEditor
{
    public partial class FormationDialog : AbstractDialog
    {
        public FormationDialog()
        {
            InitializeComponent();
        }

        public override void Assign()
        {
            DialogResult = false;
        }

        public override void PositionRelative()
        {
            PositionRelative(0, -60);
        }

        public override void SetListSource(System.Collections.IEnumerable source)
        {
            IList<ScenarioUnit> units = DataContext as IList<ScenarioUnit>;
            if (units == null) return;

            UnitClass unitClass = null;
            foreach (ScenarioUnit unit in units)
            {
                if (unitClass == null) { unitClass = unit.unitClass; continue; }
                if (unitClass != unit.unitClass) { unitClass = null; break; }
            }

            if (unitClass == null) return;

            SetButton(Line,     unitClass.lineFormation);
            SetButton(Fight,    unitClass.fightingFormation);
            SetButton(Walk,     unitClass.walkFormation);
            SetButton(Square,   unitClass.squareFormation);
            SetButton(Assault,  unitClass.assaultFormation);
            SetButton(Special,  unitClass.specialFormation);
            SetButton(Skirmish, unitClass.skirmishFormation);
            SetButton(Alt,      unitClass.alternativeSkirmishFormation);
            SetButton(Half,     unitClass.columnHalfDistanceFormation);
            SetButton(Full,     unitClass.columnFullDistanceFormation);
        }

        private void SetButton(System.Windows.Controls.Button button, Formation formation)
        {
            if (formation == null)
            {
                button.IsEnabled = false;
            }
            else
            {
                button.IsEnabled = true;
                button.Content = button.Name + " " + formation.id;
            }
        }

        protected void assign_lineFormation(object sender, RoutedEventArgs e)
        {
            foreach (IHasFormation thing in (IEnumerable<IHasFormation>)this.DataContext)
            {
                ScenarioUnit unit = thing as ScenarioUnit;
                if (unit?.unitClass?.lineFormation != null) unit.formation = unit.unitClass.lineFormation;
            }
            DialogResult = true;
        }

        protected void assign_fightingFormation(object sender, RoutedEventArgs e)
        {
            foreach (IHasFormation thing in (IEnumerable<IHasFormation>)this.DataContext)
            {
                ScenarioUnit unit = thing as ScenarioUnit;
                if (unit?.unitClass?.fightingFormation != null) unit.formation = unit.unitClass.fightingFormation;
            }
            DialogResult = true;
        }

        protected void assign_walkFormation(object sender, RoutedEventArgs e)
        {
            foreach (IHasFormation thing in (IEnumerable<IHasFormation>)this.DataContext)
            {
                ScenarioUnit unit = thing as ScenarioUnit;
                if (unit?.unitClass?.walkFormation != null) unit.formation = unit.unitClass.walkFormation;
            }
            DialogResult = true;
        }

        protected void assign_squareFormation(object sender, RoutedEventArgs e)
        {
            foreach (IHasFormation thing in (IEnumerable<IHasFormation>)this.DataContext)
            {
                ScenarioUnit unit = thing as ScenarioUnit;
                if (unit?.unitClass?.squareFormation != null) unit.formation = unit.unitClass.squareFormation;
            }
            DialogResult = true;
        }

        protected void assign_assaultFormation(object sender, RoutedEventArgs e)
        {
            foreach (IHasFormation thing in (IEnumerable<IHasFormation>)this.DataContext)
            {
                ScenarioUnit unit = thing as ScenarioUnit;
                if (unit?.unitClass?.assaultFormation != null) unit.formation = unit.unitClass.assaultFormation;
            }
            DialogResult = true;
        }

        protected void assign_specialFormation(object sender, RoutedEventArgs e)
        {
            foreach (IHasFormation thing in (IEnumerable<IHasFormation>)this.DataContext)
            {
                ScenarioUnit unit = thing as ScenarioUnit;
                if (unit?.unitClass?.specialFormation != null) unit.formation = unit.unitClass.specialFormation;
            }
            DialogResult = true;
        }

        protected void assign_skirmishFormation(object sender, RoutedEventArgs e)
        {
            foreach (IHasFormation thing in (IEnumerable<IHasFormation>)this.DataContext)
            {
                ScenarioUnit unit = thing as ScenarioUnit;
                if (unit?.unitClass?.skirmishFormation != null) unit.formation = unit.unitClass.skirmishFormation;
            }
            DialogResult = true;
        }

        protected void assign_alternativeSkirmishFormation(object sender, RoutedEventArgs e)
        {
            foreach (IHasFormation thing in (IEnumerable<IHasFormation>)this.DataContext)
            {
                ScenarioUnit unit = thing as ScenarioUnit;
                if (unit?.unitClass?.alternativeSkirmishFormation != null) unit.formation = unit.unitClass.alternativeSkirmishFormation;
            }
            DialogResult = true;
        }

        protected void assign_columnHalfDistanceFormation(object sender, RoutedEventArgs e)
        {
            foreach (IHasFormation thing in (IEnumerable<IHasFormation>)this.DataContext)
            {
                ScenarioUnit unit = thing as ScenarioUnit;
                if (unit?.unitClass?.columnHalfDistanceFormation != null) unit.formation = unit.unitClass.columnHalfDistanceFormation;
            }
            DialogResult = true;
        }

        protected void assign_columnFullDistanceFormation(object sender, RoutedEventArgs e)
        {
            foreach (IHasFormation thing in (IEnumerable<IHasFormation>)this.DataContext)
            {
                ScenarioUnit unit = thing as ScenarioUnit;
                if (unit?.unitClass?.columnFullDistanceFormation != null) unit.formation = unit.unitClass.columnFullDistanceFormation;
            }
            DialogResult = true;
        }
    }
}
