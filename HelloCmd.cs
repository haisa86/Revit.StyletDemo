using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Revit.StyletDemo.View;
using System;
using System.Diagnostics;

namespace Revit.StyletDemo
{
    [Transaction(TransactionMode.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    [Regeneration(RegenerationOption.Manual)]
    public class HelloCmd : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var win = new MainWindow();

            IntPtr rvtIntPtr =Process.GetCurrentProcess().MainWindowHandle;
            System.Windows.Interop.WindowInteropHelper WindowHelper = new System.Windows.Interop.WindowInteropHelper(win);
            WindowHelper.Owner = rvtIntPtr;
            win.ShowDialog();
            return Result.Failed;
        }
    }
}
