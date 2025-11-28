using Microsoft.Office.Tools.Ribbon;
using System;
using System.Threading;
using Word = Microsoft.Office.Interop.Word;

namespace DemoWordAddIn
{
    public partial class ThisAddIn
    {
        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
            //PrankYourFriends();

            Application.DocumentBeforeSave += Application_DocumentBeforeSave;
            
        }

        private void Application_DocumentBeforeSave(Word.Document Doc, ref bool SaveAsUI, ref bool Cancel)
        {
           

            var lastParaGraph = Doc.Paragraphs.Last;
            if (lastParaGraph.Range.Text.StartsWith("Dokument wurde zuletzt gespeichert am"))
            {
                lastParaGraph.Range.Text = $"Dokument wurde zuletzt gespeichert am {DateTime.Now}";
            }
            else
            {
                lastParaGraph.Range.InsertParagraphAfter();
                lastParaGraph = Doc.Paragraphs.Last;
                lastParaGraph.Range.Text = $"Dokument wurde zuletzt gespeichert am {DateTime.Now}";
            }          

        }

        protected override IRibbonExtension[] CreateRibbonObjects()
        {
            return new MyRibbon[] { new MyRibbon() };
        }


        public void PrankYourFriends()
        {
            for (int i = 0; i < 10; i++)
            {
                var doc = Application.Documents.Add();
                var para = doc.Content.Paragraphs.Add();
                para.Range.Text = "Leider haben Sie sich einen Virus eingefangen";
                para.Range.Font.Color = Word.WdColor.wdColorRed;
                if (i == 9)
                {
                    para.Range.Text = "War nur ein Spaß. Ich habe nur einen Test für VSTO gemacht";
                }
                Thread.Sleep(2000);

            }
        }

        private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
        {
        }

        #region Von VSTO generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InternalStartup()
        {
            this.Startup += new System.EventHandler(ThisAddIn_Startup);
            this.Shutdown += new System.EventHandler(ThisAddIn_Shutdown);
        }
        
        #endregion
    }
}
