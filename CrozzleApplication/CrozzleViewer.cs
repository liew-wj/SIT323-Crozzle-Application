using System;
using System.Windows.Forms;
using System.IO;

using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace CrozzleApplication
{
    public partial class CrozzleViewerForm : Form
    {
        #region properties
        private Crozzle SIT323Crozzle { get; set; }
        private ErrorsViewer ErrorListViewer { get; set; }
        private AboutBox ApplicationAboutBox { get; set; }

        private const String FileDialogFilter = "Crozzle File|*.czl"; /// ASS1 - Set the file dialog to only open crozzle (.czl) files.
        #endregion

        #region constructors
        public CrozzleViewerForm()
        {
            InitializeComponent();

            ApplicationAboutBox = new AboutBox();
            ErrorListViewer = new ErrorsViewer();
            ErrorListViewer.Text = ApplicationAboutBox.AssemblyTitle + " - " + ErrorListViewer.Text;
        }
        #endregion

        #region File menu event handlers
        private void openCrozzleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openCrozzleFile();
        }

        private void openCrozzleFile()
        {
            DialogResult result;
            
            // As we are opening a crozzle file,
            // indicate crozzle file, and crozzle are not valid, and clear GUI.
            crozzleToolStripMenuItem.Enabled = false;
            crozzleWebBrowser.DocumentText = "";
            ErrorListViewer.WebBrowser.DocumentText = "";

            // Process crozzle file.
            openFileDialog1.Filter = FileDialogFilter;
            result = openFileDialog1.ShowDialog();

            // Assessment Task 1 - Check
            if (result == DialogResult.OK)
            {
                // Get configuration filename.
                String configurationFileName = GetConfigurationFileName(openFileDialog1.FileName);
                if (configurationFileName == null)
                {
                    MessageBox.Show("configuration filename is missing from the crozzle file", ApplicationAboutBox.AssemblyTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                else
                {
                    String filename = configurationFileName.Trim();
                    if (Validator.IsDelimited(filename, Crozzle.StringDelimiters))
                        filename = filename.Trim(Crozzle.StringDelimiters);
                    configurationFileName = filename;

                    if (!Path.IsPathRooted(configurationFileName))
                        configurationFileName = Path.GetDirectoryName(openFileDialog1.FileName) + @"\" + configurationFileName;
                }

                // Parse configuration file.
                Configuration aConfiguration;
                Configuration.TryParse(configurationFileName, out aConfiguration);

                // Get wordlist filename.
                String wordListFileName = GetWordlistFileName(openFileDialog1.FileName);
                if (wordListFileName == null)
                {
                    MessageBox.Show("wordlist filename is missing from the crozzle file", ApplicationAboutBox.AssemblyTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                else
                {
                    String filename = wordListFileName.Trim();
                    if (Validator.IsDelimited(filename, Crozzle.StringDelimiters))
                        filename = filename.Trim(Crozzle.StringDelimiters);
                    wordListFileName = filename;

                    if (!Path.IsPathRooted(wordListFileName))
                        wordListFileName = Path.GetDirectoryName(openFileDialog1.FileName) + @"\" + wordListFileName;
                }

                // Parse wordlist file.
                WordList wordList = null;
                WordList.TryParse(wordListFileName, aConfiguration, out wordList);

                // Parse crozzle file.
                Crozzle aCrozzle;
                Crozzle.TryParse(openFileDialog1.FileName, aConfiguration, wordList, out aCrozzle);
                SIT323Crozzle = aCrozzle;

                // Update GUI - menu enabled, display crozzle data (whether valid or invalid), and crozzle file errors.
                if (SIT323Crozzle.FileValid && SIT323Crozzle.Configuration.Valid && SIT323Crozzle.WordList.Valid)
                    crozzleToolStripMenuItem.Enabled = true;

                crozzleWebBrowser.DocumentText = SIT323Crozzle.ToStringHTML();
                ErrorListViewer.WebBrowser.DocumentText =
                    SIT323Crozzle.FileErrorsHTML +
                    SIT323Crozzle.Configuration.FileErrorsHTML +
                    SIT323Crozzle.WordList.FileErrorsHTML;

                // Log errors.
                SIT323Crozzle.LogFileErrors(SIT323Crozzle.FileErrorsTXT);
                SIT323Crozzle.LogFileErrors(SIT323Crozzle.Configuration.FileErrorsTXT);
                SIT323Crozzle.LogFileErrors(SIT323Crozzle.WordList.FileErrors);
            }
        }
        private String GetConfigurationFileName(String path)
        {
            CrozzleFileItem aCrozzleFileItem = null;
            StreamReader fileIn = new StreamReader(path);

            // Search for file name.
            while (!fileIn.EndOfStream)
            {
                FileFragment<CrozzleFileItem> aFragment;
                List<String> fragment = new List<String>();
                do
                {
                    fragment.Add(fileIn.ReadLine());
                } while (!String.IsNullOrEmpty(fragment.Last()));
                if (CrozzleFileItem.TryParse(fragment, out aFragment))
                    if ((aCrozzleFileItem = aFragment.Items.Where(item => item.Name == CrozzleFileKeys.DEPENDENCIES_CONFIGDATA).First()) != null)
                        break;
            }

            // Close files.
            fileIn.Close();

            // Return file name.
            return (aCrozzleFileItem == null) ? null : aCrozzleFileItem.KeyValue.Value;
        }

        private String GetWordlistFileName(String path)
        {
            CrozzleFileItem aCrozzleFileItem = null;
            StreamReader fileIn = new StreamReader(path);

            // Search for file name.
            while (!fileIn.EndOfStream)
            {
                FileFragment<CrozzleFileItem> aFragment;
                List<String> fragment = new List<String>();
                do
                {
                    fragment.Add(fileIn.ReadLine());
                } while (!String.IsNullOrEmpty(fragment.Last()));
                if (CrozzleFileItem.TryParse(fragment, out aFragment))
                    if ((aCrozzleFileItem = aFragment.Items.Where(item => item.Name == CrozzleFileKeys.DEPENDENCIES_SEQDATA).First()) != null)
                        break;
            }

            // Close files.
            fileIn.Close();

            // Return file name.
            return (aCrozzleFileItem == null) ? null : aCrozzleFileItem.KeyValue.Value;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion

        #region Validate menu event handlers
        private void crozzleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Check if the crozzle is valid.
            SIT323Crozzle.Validate();

            // Update GUI - display crozzle data (whether valid or invalid), 
            // crozle file errors, config file errors, word list file errors and crozzle errors.
            crozzleWebBrowser.DocumentText = SIT323Crozzle.ToStringHTML();
            ErrorListViewer.WebBrowser.DocumentText =
                SIT323Crozzle.FileErrorsHTML +
                SIT323Crozzle.Configuration.FileErrorsHTML +
                SIT323Crozzle.WordList.FileErrorsHTML +
                SIT323Crozzle.ErrorsHTML;

            // Log crozzle errors.
            SIT323Crozzle.LogFileErrors(SIT323Crozzle.ErrorsTXT);
        }
        #endregion
        
        #region Debug menu event handlers
        /// Implement unit test designs for the required validators
        private void debugBoolean_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog debugDialog = new OpenFileDialog())
            {
                if (debugDialog.ShowDialog() == DialogResult.OK)
                {
                    StreamReader debugStream = new StreamReader(debugDialog.FileName);
                    String ext = Path.GetExtension(debugDialog.FileName);

                    if (new String[] { ".czl", ".seq" }.Contains(ext))
                    {
                        String line = debugStream.ReadToEnd();
                        Debug.Assert(line.ToLower().Contains(Boolean.TrueString.ToLower()) == false, 
                            String.Format(DebugWarnings.SubStringExistsWarning, debugDialog.FileName, Boolean.TrueString));

                        Debug.Assert(line.ToLower().Contains(Boolean.FalseString.ToLower()) == false, 
                            String.Format(DebugWarnings.SubStringExistsWarning, debugDialog.FileName, Boolean.FalseString));
                    }
                    else
                    {
                        String debugLine;
                        int count = 0;
                        while (!debugStream.EndOfStream)
                        {
                            debugLine = debugStream.ReadLine();
                            if (!String.IsNullOrEmpty(debugLine))
                                if (debugLine.ToLower().Contains(Boolean.TrueString.ToLower()) || debugLine.ToLower().Contains(Boolean.FalseString.ToLower()))
                                {
                                    Boolean funcResult, paramResult;
                                    funcResult = Validator.IsBoolean(debugLine.Split(new char[] { '=' }, 2).Last(), out paramResult);

                                    Debug.Assert(funcResult == true, String.Format(DebugWarnings.UnparseableBooleanWarning, debugLine));
                                    Debug.Assert(paramResult == funcResult, String.Format(DebugWarnings.ConflictingBooleanWarning, funcResult, paramResult));

                                    count++;
                                }
                        }
                        Debug.Assert(count == 1, DebugWarnings.ExcessBooleanWarning, debugDialog.FileName);
                    }
                }
            }

            MessageBox.Show("Debug completed.");
        }

        private void debugHexCode_Click(object sender, EventArgs e)
        {
            // ...
        }
        #endregion

        #region View menu event handlers
        private void errorListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ErrorListViewer.WindowState = FormWindowState.Normal;
            ErrorListViewer.Show();
            ErrorListViewer.Activate();
        }
        #endregion

        #region Help menu event handlers
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ApplicationAboutBox.ShowDialog();
        }
        #endregion
    }
}
