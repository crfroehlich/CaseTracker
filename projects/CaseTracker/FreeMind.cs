using System;
using System.Collections.Generic;
using System.Text;
using FogBugzNet;
using System.Xml;
using System.Windows.Forms;

namespace FogBugzCaseTracker
{
    public partial class HoverWindow
    {
        private void ExportToFreeMind()
        {
            try
            {

                String tempTabSep = System.IO.Path.GetTempPath() + "cases_" + (Guid.NewGuid()).ToString() + ".mm";
                // create a writer and open the file

                Exporter ex = new Exporter(_fb, new Search(formatSearch(), _cases));
                ex.CasesToMindMap().Save(tempTabSep);

                System.Diagnostics.Process.Start("\"" + tempTabSep + "\"");
            }
            catch (System.Exception x)
            {
                MessageBox.Show("Sorry, couldn't launch Excel");
                Utils.LogError(x.ToString());
            }
        }
        private void DoImport()
        {
            XmlDocument doc = GetMindMapFromUser();
            if (doc == null)
                return;

            Importer imp = new Importer(doc, _fb);
            ImportAnalysis results = imp.Analyze();
            if (results.NothingToDo)
            {
                MessageBox.Show("No changes were detected. Nothing to import.", "Import Mind Map", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            ImportConfirmationDlg dlg = new ImportConfirmationDlg(results);

            if (dlg.ShowDialog() == DialogResult.Yes)
            {
                foreach (Case c in results.CaseToNewParent.Keys)
                {
                    try
                    {
                        _fb.SetParent(c, results.CaseToNewParent[c].ID);
                    }
                    catch (Exception x)
                    {
                        Utils.LogError(x.ToString());
                    }
                }
            }
        }


    }
}
