using Microsoft.Office.Tools.Ribbon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Word = Microsoft.Office.Interop.Word;
using Office = Microsoft.Office.Core;
using Microsoft.Office.Tools.Word;

namespace DemoWordAddIn
{
    public partial class MyRibbon
    {
        private void MyRibbon_Load(object sender, RibbonUIEventArgs e)
        {
            try
            {
                // Initialisierungscode hier
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Laden des Ribbons: {ex.Message}");
            }
        }

        private void btnTest_Click(object sender, RibbonControlEventArgs e)
        {
            Word.Application word = Globals.ThisAddIn.Application;
            word.ActiveDocument.Content.Text = "Das ist ein Testtext, der vom Ribbon-Button eingefügt wurde.";

        }
    }
}
