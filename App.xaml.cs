using NorbSoftDev.SOW;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ScenarioEditor
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            DispatcherUnhandledException += (s, ex) =>
            {
                Log.Error(this, "Unhandled exception: " + ex.Exception.Message);
                Log.Error(this, ex.Exception.StackTrace);
                ex.Handled = true;
                MessageBox.Show(
                    ex.Exception.Message + "\n\n" + ex.Exception.StackTrace,
                    "Unhandled Exception",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            };
        }
    }
}
