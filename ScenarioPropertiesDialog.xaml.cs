using NorbSoftDev.SOW;
using System;
using System.Collections.Generic;
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

namespace ScenarioEditor
{
    /// <summary>
    /// Interaction logic for ScenarioSaveAsDialog.xaml
    /// </summary>
    public partial class ScenarioPropertiesDialog : AbstractDialog
    {

        Scenario.EWeather[] weathers;
        Scenario.ESandbox[] sandboxs;

        Scenario scenario;
        public ScenarioPropertiesDialog(Scenario scenario, bool showDontSave)
        {
            this.scenario = scenario;
            this.DataContext = scenario;



            InitializeComponent();
            dirText.Text = scenario.mod.directory.FullName;
            playerText.Text = scenario.playerEchelon == null ? String.Empty : scenario.playerEchelon.unit == null ? String.Empty : scenario.playerEchelon.unit.name1; 
            saveAsText.Text = scenario.name;

           
            weathers = Enum.GetValues(typeof(Scenario.EWeather)).Cast<Scenario.EWeather>().ToArray<Scenario.EWeather>();
            weatherCombo.ItemsSource = weathers;
            weatherCombo.SelectedItem = scenario.initialWeather;

            sandboxs = Enum.GetValues(typeof(Scenario.ESandbox)).Cast<Scenario.ESandbox>().ToArray<Scenario.ESandbox>();
            sandboxCombo.ItemsSource = sandboxs;
            sandboxCombo.SelectedItem = scenario.sandbox;


            if (!showDontSave) dontSaveButton.Visibility = Visibility.Hidden;

            // Populate the "Overwrite existing" combo with scenarios already in this mod's Scenarios folder
            var existingNames = new List<string>();
            var scenDir = System.IO.Path.Combine(scenario.mod.directory.FullName, "Scenarios");
            if (System.IO.Directory.Exists(scenDir))
            {
                foreach (string dir in System.IO.Directory.GetDirectories(scenDir))
                {
                    string name = System.IO.Path.GetFileName(dir);
                    if (!string.IsNullOrEmpty(name))
                        existingNames.Add(name);
                }
            }
            if (existingNames.Count > 0)
            {
                overwriteCombo.ItemsSource = existingNames;
                overwriteCombo.SelectedIndex = -1;
            }
            else
            {
                overwriteCombo.IsEnabled = false;
            }
        }


        private void weatherCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            int index = comboBox.SelectedIndex;

            Scenario.EWeather value = weathers[index];
            scenario.initialWeather = value;
        }

        private void sandboxCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            int index = comboBox.SelectedIndex;

            Scenario.ESandbox value = sandboxs[index];
            scenario.sandbox = value;
        }

        public override void Assign()
        {
            choiceWasMade = true;

            scenario.name = saveAsText.Text.Trim();
            this.DialogResult = true;
            this.Close();
        }


        protected void dontSave_Click(object sender, RoutedEventArgs e)
        {
            choiceWasMade = true;
            this.DialogResult = false;
            this.Close();
        }

        public override void PositionRelative()
        {
            //stub
        }

        public override void SetListSource(System.Collections.IEnumerable source)
        {
            //stubb
        }

        private void overwriteCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selected = overwriteCombo.SelectedItem as string;
            if (!string.IsNullOrEmpty(selected))
                saveAsText.Text = selected;
        }

        private void saveAsText_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            string text = textBox.Text;
            string cleaned = text.ToASCII(); // strip non-ASCII only; no Trim during editing
            if (cleaned != text)
            {
                int caret = textBox.CaretIndex;
                textBox.Text = cleaned;
                // Restore caret, clamping to the (possibly shorter) new length
                textBox.CaretIndex = Math.Min(caret, cleaned.Length);
            }
        }
    }
}
