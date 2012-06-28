using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Configuration;
using System.Xml;
using System.Globalization;
using Ionic.Utils.Zip;

namespace EpubCreator
{
    public partial class frmMain : Form
    {
        // Create some structs to hold chapter and illustration details
        struct Chapter
        {
            public String id;
            public String chapterName;
            public String chapterTitle;
            public String rawText;
            public String html;
            public int startLineNumber;
            public int endLineNumber;
        }

        struct Illustration
        {
            public String location;
            public String name;
            public Boolean useAsCover;
        }

        // Get the paths for the supporting and temp files
        String basePath = Application.ExecutablePath.Substring(0, Application.ExecutablePath.LastIndexOf("\\") + 1);
        String supportDir = Application.ExecutablePath.Substring(0, Application.ExecutablePath.LastIndexOf("\\") + 1) + "\\support";

        private String mainDir = "";
        private String metaDir = "";
        private String opsDir = "";
        private String cssDir = "";
        private String imagesDir = "";

        private String workingFileName = "";
        private String workingDirectory = "";
        private String OpfBody = "";

        private int runningChapterCount = 0;
        private String CleanSuffix = "";
        private bool quietOperation = false;
        private List<String> autoFileNames = new List<String>();
        char[] accentedChars = new char[48];
        String[] accentedHtml = new String[48];

		private bool treeTOC = true;

        public frmMain()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Attempt to clean some files to avoid encoding issues by
        /// setting up some easy structures to replace characters
        /// with escaped values
        /// </summary>
        private void BuildAccentedChars()
        {
            accentedChars[0] = 'Œ';
            accentedChars[1] = 'œ';
            accentedChars[2] = 'Ÿ';
            accentedChars[3] = '«';
            accentedChars[4] = '»';
            accentedChars[5] = '‹';
            accentedChars[6] = '›';
            accentedChars[7] = '€';
            accentedChars[8] = 'À';
            accentedChars[9] = 'Á';
            accentedChars[10] = 'Â';
            accentedChars[11] = 'Ä';
            accentedChars[12] = 'È';
            accentedChars[13] = 'É';
            accentedChars[14] = 'Ê';
            accentedChars[15] = 'Ë';
            accentedChars[16] = 'Ì';
            accentedChars[17] = 'Í';
            accentedChars[18] = 'Î';
            accentedChars[19] = 'Ï';
            accentedChars[20] = 'Ò';
            accentedChars[21] = 'Ó';
            accentedChars[22] = 'Ô';
            accentedChars[23] = 'Ö';
            accentedChars[24] = 'Ù';
            accentedChars[25] = 'Ú';
            accentedChars[26] = 'Û';
            accentedChars[27] = 'Ü';
            accentedChars[28] = 'à';
            accentedChars[29] = 'á';
            accentedChars[30] = 'â';
            accentedChars[31] = 'ä';
            accentedChars[32] = 'è';
            accentedChars[33] = 'é';
            accentedChars[34] = 'ê';
            accentedChars[35] = 'ë';
            accentedChars[36] = 'ì';
            accentedChars[37] = 'í';
            accentedChars[38] = 'î';
            accentedChars[39] = 'ï';
            accentedChars[40] = 'ò';
            accentedChars[41] = 'ó';
            accentedChars[42] = 'ô';
            accentedChars[43] = 'ö';
            accentedChars[44] = 'ù';
            accentedChars[45] = 'ú';
            accentedChars[46] = 'û';
            accentedChars[47] = 'ü';


            accentedHtml[0] = "&OElig;";
            accentedHtml[1] = "&oelig;";
            accentedHtml[2] = "&Yuml;";
            accentedHtml[3] = "&laquo;";
            accentedHtml[4] = "&raquo;";
            accentedHtml[5] = "&lsaquo;";
            accentedHtml[6] = "&rsaquo;";
            accentedHtml[7] = "&euro;";
            accentedHtml[8] = "&Agrave;";
            accentedHtml[9] = "&Aacute;";
            accentedHtml[10] = "&Acirc;";
            accentedHtml[11] = "&Auml;";
            accentedHtml[12] = "&Egrave;";
            accentedHtml[13] = "&Eacute;";
            accentedHtml[14] = "&Ecirc;";
            accentedHtml[15] = "&Euml;";
            accentedHtml[16] = "&Igrave;";
            accentedHtml[17] = "&Iacute;";
            accentedHtml[18] = "&Icirc;";
            accentedHtml[19] = "&Iuml;";
            accentedHtml[20] = "&Ograve;";
            accentedHtml[21] = "&Oacute;";
            accentedHtml[22] = "&Ocirc;";
            accentedHtml[23] = "&Ouml;";
            accentedHtml[24] = "&Ugrave;";
            accentedHtml[25] = "&Uacute;";
            accentedHtml[26] = "&Ucirc;";
            accentedHtml[27] = "&Uuml;";
            accentedHtml[28] = "&agrave;";
            accentedHtml[29] = "&aacute;";
            accentedHtml[30] = "&acirc;";
            accentedHtml[31] = "&auml;";
            accentedHtml[32] = "&egrave;";
            accentedHtml[33] = "&eacute;";
            accentedHtml[34] = "&ecirc;";
            accentedHtml[35] = "&euml;";
            accentedHtml[36] = "&igrave;";
            accentedHtml[37] = "&iacute;";
            accentedHtml[38] = "&icirc;";
            accentedHtml[39] = "&iuml;";
            accentedHtml[40] = "&ograve;";
            accentedHtml[41] = "&oacute;";
            accentedHtml[42] = "&ocirc;";
            accentedHtml[43] = "&ouml;";
            accentedHtml[44] = "&ugrave;";
            accentedHtml[45] = "&uacute;";
            accentedHtml[46] = "&ucirc;";
            accentedHtml[47] = "&uuml;";

        }

        /// <summary>
        /// Check to see if the app has been lauched with parameters,
        /// and react accordingly
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmMain_Load(object sender, EventArgs e)
        {
            if (Program.Args != null)
            {
                if (Program.Args.Length > 0)
                {
                    if ((Program.Args[0] == "clean") && (Program.Args.Length > 1))
                    {
                        // This will clean an HTML file
                        quietOperation = true;
                        foreach (String item in Program.Args)
                        {
                            if (File.Exists(item))
                                autoFileNames.Add(item);
                        }
                        if (autoFileNames.Count > 0)
                        {
                            CommandLineClean();
                            this.Close();
                            return;
                        }
                    }
                    else if ((Program.Args[0] == "create") && (Program.Args.Length > 1))
                    {
                        // This will auto-create an EPUB file by making several assumptions
                        // about file names and existence
                        foreach (String item in Program.Args)
                        {
                            if (File.Exists(item))
                                autoFileNames.Add(item);
                        }
                        if (autoFileNames.Count > 0)
                        {
                            CommandLineCreate();
                            this.Close();
                            return;
                        }
                    }
                    else if (File.Exists(Program.Args[0]))
                    {
                        // Otherwise, just open the file and set defaults
                        tbxFileName.Text = Program.Args[0].ToString();
                        LoadFile();
                        if (ConfigurationManager.AppSettings["WindowSize"] == "maximized")
                            this.WindowState = FormWindowState.Maximized;

                        Setup();
                    }
                }
                else
                {
                    // No args, so just open the app
                    if (ConfigurationManager.AppSettings["WindowSize"] == "maximized")
                        this.WindowState = FormWindowState.Maximized;

                    Setup();
                }
            }

            if (String.IsNullOrEmpty(tbxFileName.Text))
            {
                groupBox1.Enabled = false;
                groupBox2.Enabled = false;
                groupBox3.Enabled = false;
                groupBox4.Enabled = false;
                btnCreate.Enabled = false;
            }
        }

        /// <summary>
        /// Update some values when window is resized
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmMain_SizeChanged(object sender, EventArgs e)
        {
            System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            if (this.WindowState == FormWindowState.Maximized)
                config.AppSettings.Settings["WindowSize"].Value = "maximized";
            else
                config.AppSettings.Settings["WindowSize"].Value = "normal";

            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }

        /// <summary>
        /// Add an item to an ATOM Library
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddToLibrary_Click(object sender, EventArgs e)
        {
            frmLibrary formLibrary = new frmLibrary(this);
            formLibrary.AddToPublicLibrary = false;
            formLibrary.ShowDialog();
            formLibrary.Dispose();
        }

        /// <summary>
        /// Add an item to a public, obfuscated library
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddToPublicLibrary_Click(object sender, EventArgs e)
        {
            frmLibrary formLibrary = new frmLibrary(this);
            formLibrary.AddToPublicLibrary = true;
            formLibrary.ShowDialog();
            formLibrary.Dispose();
        }

        /// <summary>
        /// Auto-Build the EPUB file by making certain assumptions
        /// about the file names and existence of files
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBuild_Click(object sender, EventArgs e)
        {
            AutoCreateEpub();
        }

        /// <summary>
        /// Create the EPUB file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCreate_Click(object sender, EventArgs e)
        {
            CreateEpub();
            MessageBox.Show("ePub Successfully Created.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Open an HTML or TXT file -- HTML is much prefered and a much better
        /// option for creating an EPUB.  Much better to hand-convert the TXT
        /// to HTML to make sure everything is done well and correctly.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOpenFile_Click(object sender, EventArgs e)
        {
            dlgFile.RestoreDirectory = true;
            dlgFile.Multiselect = false;
            dlgFile.Filter = "Text files (*.txt;*.htm;*.html;*.xhtml)|*.txt;*.htm;*.html;*.xhtml";
            dlgFile.InitialDirectory = ConfigurationManager.AppSettings["BaseDir"];

            // If working on several files within the same directory (say an Author)
            // try to start in the same folder
            if (!String.IsNullOrEmpty(tbxFileName.Text))
            {
                if (File.Exists(tbxFileName.Text))
                {
                    dlgFile.InitialDirectory = Path.GetFullPath(tbxFileName.Text);
                }
            }

            if (dlgFile.ShowDialog() == DialogResult.OK)
            {
                tbxFileName.Text = dlgFile.FileName;
                LoadFile();
            }
        }

        /// <summary>
        /// Create a new GUID via an SHA-1 hash as the EPUB UUID
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGUID_Click(object sender, EventArgs e)
        {
            tbxIdentifier.Text = Program.GetHash(tbxAuthorSort.Text + tbxTitle.Text); //System.Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Open image files
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnImage_Click(object sender, EventArgs e)
        {
            dlgFile.RestoreDirectory = true;
            dlgFile.Multiselect = true;
            dlgFile.Filter = "Image files (*.jpg;*.jpeg;*.gif;*.png)|*.jpg;*.jpeg;*.gif;*.png";

            if (dlgFile.ShowDialog() == DialogResult.OK)
            {
                if (dlgFile.FileNames.Length > 1)
                {
                    foreach (String image in dlgFile.FileNames)
                    {
                        AddImage(image, Path.GetFileName(image));
                    }
                }
                else
                {
                    tbxImageFileName.Text = dlgFile.FileName;
                    tbxImageName.Text = Path.GetFileName(dlgFile.FileName);
                }
            }
        }

        /// <summary>
        /// Based on the Chapter rules, guess each chapter
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGuessChapters_Click(object sender, EventArgs e)
        {
            GuessChapters();
        }

        /// <summary>
        /// Process the chapters based on the line starts and ends found
        /// during the GuessChapters() function
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnProcess_Click(object sender, EventArgs e)
        {
            ProcessChapters();
        }

        /// <summary>
        /// Clean the HTML file using HTML Tidy and a few other rules
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCleanHtml_Click(object sender, EventArgs e)
        {
            String CleanFilename = "";
            CleanFilename = Path.GetFileName(tbxFileName.Text);
            CleanFilename = CleanFilename.Substring(0, CleanFilename.LastIndexOf(".")) + CleanSuffix + CleanFilename.Substring(CleanFilename.LastIndexOf("."));
            CleanFilename = Path.Combine(workingDirectory, CleanFilename);

            HtmlTidy(tbxFileName.Text, CleanFilename);

            Process edit = new Process();
            edit.StartInfo.FileName = ConfigurationManager.AppSettings["TextEditor"];
            edit.StartInfo.Arguments = String.Format("\"{0}\"", CleanFilename);
            edit.Start();
            edit.Dispose();

            tbxCleanFileName.Text = CleanFilename;
            workingFileName = CleanFilename;
        }

        /// <summary>
        /// Move a tree item up a level
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnMoveUp_Click(object sender, EventArgs e)
        {
            int index = -1;

            if (treeChapters.SelectedNode != null)
            {
                index = treeChapters.SelectedNode.Index;

                if (index > 0)
                {
                    TreeNode oldNode = treeChapters.SelectedNode;
                    TreeNode node = (TreeNode)treeChapters.SelectedNode.Clone();
                    treeChapters.Nodes.Insert(treeChapters.SelectedNode.Index - 1, node);
                    treeChapters.Nodes.Remove(oldNode);
                    node = treeChapters.Nodes[index - 1];
                    treeChapters.Focus();
                    treeChapters.SelectedNode = treeChapters.Nodes.Find(node.Name, false)[0];
                }
            }
        }

        /// <summary>
        /// Move a tree item down a level
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnMoveDown_Click(object sender, EventArgs e)
        {
            int index = -1;

            if (treeChapters.SelectedNode != null)
            {
                index = treeChapters.SelectedNode.Index;

                if (index < treeChapters.Nodes.Count - 1)
                {
                    TreeNode oldNode = treeChapters.SelectedNode;
                    TreeNode node = (TreeNode)treeChapters.SelectedNode.Clone();

                    treeChapters.Nodes.Insert(treeChapters.SelectedNode.Index + 2, node);
                    treeChapters.Nodes.Remove(oldNode);

                    node = treeChapters.Nodes[index + 1];
                    treeChapters.Focus();
                    treeChapters.SelectedNode = treeChapters.Nodes.Find(node.Name, false)[0];
                }
            }
        }

        /// <summary>
        /// Add a title page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddTitlePage_Click(object sender, EventArgs e)
        {
            AddTitlePage();
        }

        /// <summary>
        /// Add a new Chapter to the EPUB
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddChapter_Click(object sender, EventArgs e)
        {
            int newIndex = -1;
            foreach (TreeNode node in treeChapters.Nodes)
            {
                if (int.Parse(node.Name) > newIndex)
                    newIndex = int.Parse(node.Name);
            }

            newIndex++;

            TreeNode newNode = new TreeNode(ddlChapterKeyword.Text + " " + newIndex.ToString());
            newNode.Name = newIndex.ToString();
            treeChapters.Nodes.Add(newNode);
        }

        /// <summary>
        /// Delete an item from the tree
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (treeChapters.Nodes.Count > 0)
                treeChapters.Nodes.Remove(treeChapters.SelectedNode);
        }

        /// <summary>
        /// Save any edits to the Chapter
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, EventArgs e)
        {
            TreeNode curNode = null;
            
            // Find the selected tree node
            foreach (TreeNode node in treeChapters.Nodes)
            {
                if (node.BackColor == System.Drawing.SystemColors.Highlight)
                {
                    curNode = node;
                    break;
                }
            }

            // Found it, so save/update the data
            if (curNode != null)
            {
                Chapter chapter = (Chapter)curNode.Tag;

                chapter.chapterName = tbxChapterName.Text;
                chapter.chapterTitle = tbxChapterTitle.Text;
                chapter.rawText = tbxText.Text;
                chapter.html = tbxHTML.Text;
                chapter.id = tbxChapterID.Text;
                chapter.startLineNumber = int.Parse(tbxChapStartLineNumber.Text);
                chapter.endLineNumber = int.Parse(tbxChapEndLineNumber.Text);

                if (chapter.chapterName != curNode.Text)
                    curNode.Text = chapter.chapterName;

                curNode.Tag = chapter;
            }
        }

        /// <summary>
        /// Make a tree node a child of the node above it
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnMakeChild_Click(object sender, EventArgs e)
        {
            TreeNode curNode = treeChapters.SelectedNode;
            TreeNode parentNode = curNode.PrevNode;
            parentNode.Nodes.Add((TreeNode)curNode.Clone());

            if (treeChapters.Nodes.Count > 0)
                treeChapters.Nodes.Remove(curNode);

            parentNode.ExpandAll();
        }

        /// <summary>
        /// Make a tree node a parent
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnMakeParent_Click(object sender, EventArgs e)
        {
            TreeNode curNode = treeChapters.SelectedNode;
            TreeNode parentNode = curNode.Parent;
            TreeNode grandParentNode = parentNode.Parent;

            if (grandParentNode == null)
                treeChapters.Nodes.Insert(parentNode.Index + 1, (TreeNode)curNode.Clone());
            else
                grandParentNode.Nodes.Add((TreeNode)curNode.Clone());

            parentNode.Nodes.Remove(curNode);
        }

        /// <summary>
        /// Perform actions after a tree node is selected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeChapters_AfterSelect(object sender, TreeViewEventArgs e)
        {
            // Remove the highlighted color
            ResetNodeColors(treeChapters.Nodes);
            TreeNode curNode = treeChapters.SelectedNode;

            if (curNode != null)
            {
                // Set the back color to keep the item highlighted when
                // losing focus on editing the text -- nice for the user
                // and will be used by other functions to determine the 
                // selected tree node.
                curNode.BackColor = System.Drawing.SystemColors.Highlight;
                curNode.ForeColor = System.Drawing.SystemColors.HighlightText;
                
                Chapter chapter = (Chapter)curNode.Tag;
                tbxChapterName.Text = chapter.chapterName;
                tbxChapterTitle.Text = chapter.chapterTitle;
                tbxText.Text = chapter.rawText;
                tbxHTML.Text = chapter.html;
                tbxChapterID.Text = chapter.id;
                tbxChapStartLineNumber.Text = chapter.startLineNumber.ToString();
                tbxChapEndLineNumber.Text = chapter.endLineNumber.ToString();
            }
        }

        /// <summary>
        /// Add an image
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddImage_Click(object sender, EventArgs e)
        {
            AddImage(tbxImageFileName.Text, tbxImageName.Text);
        }

        /// <summary>
        /// Add an image and update the UI
        /// </summary>
        /// <param name="ImageFileName"></param>
        /// <param name="ImageName"></param>
        private void AddImage(String ImageFileName, String ImageName)
        {
            // Get a new tree node and create an Illustration to attach
            TreeNode node = new TreeNode();
            Illustration image = new Illustration();
            image.location = ImageFileName;
            image.name = ImageName;
            image.useAsCover = false;
            node.Tag = image;
            node.Text = ImageName;
            node.Name = treeImages.Nodes.Count.ToString();
            treeImages.Nodes.Add(node);
            treeImages.SelectedNode = treeImages.Nodes[0];
        }

        /// <summary>
        /// Delete an image from the tree
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDeleteImage_Click(object sender, EventArgs e)
        {
            if (treeImages.Nodes.Count > 0)
                treeImages.Nodes.Remove(treeImages.SelectedNode);
        }

        /// <summary>
        /// Save the image details
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSaveImage_Click(object sender, EventArgs e)
        {
            TreeNode curNode = null;

            foreach (TreeNode node in treeImages.Nodes)
            {
                if (node.BackColor == System.Drawing.SystemColors.Highlight)
                {
                    curNode = node;
                    break;
                }
            }

            if (curNode != null)
            {
                Illustration image = (Illustration)curNode.Tag;

                image.location = tbxImageFileName.Text;
                image.name = tbxImageName.Text;
                image.useAsCover = chkUseAsCover.Checked;
                curNode.Tag = image;
            }
        }

        /// <summary>
        /// Update the Image tree
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeImages_AfterSelect(object sender, TreeViewEventArgs e)
        {
            foreach (TreeNode node in treeImages.Nodes)
            {
                node.BackColor = System.Drawing.SystemColors.Window;
                node.ForeColor = System.Drawing.SystemColors.WindowText;
            }

            TreeNode curNode = treeImages.SelectedNode;

            if (curNode != null)
            {
                curNode.BackColor = System.Drawing.SystemColors.Highlight;
                curNode.ForeColor = System.Drawing.SystemColors.HighlightText;
                Illustration image = (Illustration)curNode.Tag;

                tbxImageFileName.Text = image.location;
                tbxImageName.Text = image.name;
                chkUseAsCover.Checked = image.useAsCover;
            }
        }

        /// <summary>
        /// Determine file type and update UI
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ddlType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlType.SelectedIndex == 0)
            {
                gboxText.Visible = true;
                gboxHtml.Visible = false;
            }
            else if (ddlType.SelectedIndex == 1)
            {
                gboxText.Visible = false;
                gboxHtml.Visible = true;
            }
        }

        /// <summary>
        /// Set an illustration as the cover
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkUseAsCover_CheckedChanged(object sender, EventArgs e)
        {
            foreach (TreeNode node in treeImages.Nodes)
            {
                if ((!node.Equals(treeImages.SelectedNode)) && (chkUseAsCover.Checked == true))
                {
                    Illustration image = (Illustration)node.Tag;
                    image.useAsCover = false;
                    node.Tag = image;
                }
            }
        }

        /// <summary>
        /// Allow a quick way to open cleaned file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbxCleanFileName_DoubleClick(object sender, EventArgs e)
        {
            Process edit = new Process();
            edit.StartInfo.FileName = ConfigurationManager.AppSettings["TextEditor"];
            edit.StartInfo.Arguments = tbxCleanFileName.Text;
            edit.Start();
            edit.Dispose();
        }

        /// <summary>
        /// Update UI based on file name
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbxFileName_TextChanged(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(tbxFileName.Text))
            {
                groupBox1.Enabled = false;
                groupBox2.Enabled = false;
                groupBox3.Enabled = false;
                groupBox4.Enabled = false;
                btnCreate.Enabled = false;
            }
            else
            {
                groupBox1.Enabled = true;
                groupBox2.Enabled = true;
                groupBox3.Enabled = true;
                groupBox4.Enabled = true;
                btnCreate.Enabled = true;
            }
        }

        /// <summary>
        /// Do a simple setup of the application
        /// </summary>
        private void Setup()
        {
            tbxIdentifier.Text = Program.GetHash(tbxAuthorSort.Text + tbxTitle.Text); //System.Guid.NewGuid().ToString();
            tbxDateCreated.Text = DateTime.Now.ToString("yyyy-MM-dd"); //DateTime.Now.ToString("ddd MMM dd HH:mm:ss zzz yyyy");
            ddlType.SelectedIndex = 1;
            ddlChapterKeyword.SelectedIndex = 0;
            chkToTitleCase.Checked = bool.Parse(ConfigurationManager.AppSettings["ToTitleCase"]);
            CleanSuffix = ConfigurationManager.AppSettings["CleanSuffix"];
        }

        /// <summary>
        /// Load a file and determine what to do
        /// </summary>
        private void LoadFile()
        {
            workingFileName = tbxFileName.Text;
            workingDirectory = Path.GetDirectoryName(workingFileName);

            switch (Path.GetExtension(workingFileName))
            {
                case ".txt":
                    ddlType.SelectedIndex = 0;
                    break;
                case ".htm":
                case ".html":
                case ".xhtml":
                    ddlType.SelectedIndex = 1;
                    break;
            }

            // Clear any existing trees, etc...
            treeChapters.Nodes.Clear();
            treeImages.Nodes.Clear();

            // Set some defaults
            tbxCleanFileName.Text = tbxFileName.Text;
            tbxImageFileName.Text = "";
            tbxImageName.Text = "";
            chkUseAsCover.Checked = false;

            tbxDateCreated.Text = DateTime.Now.ToString("yyyy-MM-dd");
            tbxEpubCreator.Text = "EPUB Creator";
            tbxLanguage.Text = "en";

            // Get some details based on the file name
            // Assuming, best case scenario:
            // For example: {Title} - {Author(s)} - {Publisher} - {Year(s)} - {Subject(s)}.html
            String[] args = Path.GetFileNameWithoutExtension(tbxFileName.Text).Split(new String[] { " - " }, StringSplitOptions.None);
            if (args.Length >= 4)
            {
                tbxTitle.Text = args[0].Trim().Replace("=", ":");
                tbxAuthorSort.Text = args[1].Trim();
                tbxAuthor.Text = (args[1].Split(new Char[] { ',' })[1] + " " + args[1].Split(new Char[] { ',' })[0]).Trim();
                tbxPublisher.Text = args[2].Trim();
                tbxOrigPublishDate.Text = args[3].Trim();
                if (args.Length >= 5)
                    tbxSubject.Text = args[4].Trim();
                tbxIdentifier.Text = Program.GetHash(tbxAuthorSort.Text + tbxTitle.Text); //System.Guid.NewGuid().ToString();
            }
            else
            {
                // Something's wrong, so just reset all values
                tbxAuthor.Text = "";
                tbxAuthorSort.Text = "";
                tbxCoverage.Text = "";
                tbxDescription.Text = "";
                tbxIdentifier.Text = "";
                tbxOrigPublishDate.Text = "";
                tbxPublisher.Text = "";
                tbxRights.Text = "";
                tbxSource.Text = "";
                tbxSubject.Text = "";
                tbxTitle.Text = "";
            }
        }

        /// <summary>
        /// Remove the tree node highlight color
        /// </summary>
        /// <param name="Nodes"></param>
        private void ResetNodeColors(TreeNodeCollection Nodes)
        {
            foreach (TreeNode node in Nodes)
            {
                node.BackColor = System.Drawing.SystemColors.Window;
                node.ForeColor = System.Drawing.SystemColors.WindowText;
                if (node.Nodes.Count > 0)
                    ResetNodeColors(node.Nodes);
            }
        }

        /// <summary>
        /// Simple check for a numeric value
        /// </summary>
        /// <param name="Expression"></param>
        /// <returns></returns>
        public bool IsNumeric(object Expression)
        {
            double retNum;

            try
            {
                return Double.TryParse(Convert.ToString(Expression), System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out retNum);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Convert (simplified) something like "TWO" to 2
        /// </summary>
        /// <param name="Number"></param>
        /// <returns></returns>
        private int ConvertAlphaToInt(String Number)
        {
            String[] alpha = { "ZERO","ONE","TWO","THREE","FOUR","FIVE","SIX","SEVEN","EIGHT","NINE",
								"TEN","ELEVEN","TWELVE","THIRTEEN","FOURTEEN","FIFTEEN","SIXTEEN","SEVENTEEN","EIGHTEEN","NINETEEN",
								"TWENTY","TWENTYONE","TWENTYTWO","TWENTYTHREE","TWENTYFOUR","TWENTYFIVE","TWENTYSIX","TWENTYSEVEN","TWENTYEIGHT","TWENTYNINE",
								"THIRTY","THIRTYONE","THIRTYTWO","THIRTYTHREE","THIRTYFOUR","THIRTYFIVE","THIRTYSIX","THIRTYSEVEN","THIRTYEIGHT","THIRTYNINE",
								"FORTY","FORTYONE","FORTYTWO","FORTYTHREE","FORTYFOUR","FORTYFIVE","FORTYSIX","FORTYSEVEN","FORTYEIGHT","FORTYNINE",
								"FIFTY","FIFTYONE","FIFTYTWO","FIFTYTHREE","FIFTYFOUR","FIFTYFIVE","FIFTYSIX","FIFTYSEVEN","FIFTYEIGHT","FIFTYNINE",
								"SIXTY","SIXTYONE","SIXTYTWO","SIXTYTHREE","SIXTYFOUR","SIXTYFIVE","SIXTYSIX","SIXTYSEVEN","SIXTYEIGHT","SIXTYNINE",
								"SEVENTY","SEVENTYONE","SEVENTYTWO","SEVENTYTHREE","SEVENTYFOUR","SEVENTYFIVE","SEVENTYSIX","SEVENTYSEVEN","SEVENTYEIGHT","SEVENTYNINE",
								"EIGHTY","EIGHTYONE","EIGHTYTWO","EIGHTYTHREE","EIGHTYFOUR","EIGHTYFIVE","EIGHTYSIX","EIGHTYSEVEN","EIGHTYEIGHT","EIGHTYNINE",
								"NINETY","NINETYONE","NINETYTWO","NINETYTHREE","NINETYFOUR","NINETYFIVE","NINETYSIX","NINETYSEVEN","NINETYEIGHT","NINETYNINE",
								"ONEHUNDRED"
			};

            if (!String.IsNullOrEmpty(Number))
            {
                List<String> AlphaNumerals = new List<String>(alpha);

                Number = Number.Replace(" ", "").Replace("-", "").ToUpper();
                int pos = AlphaNumerals.IndexOf(Number);

                AlphaNumerals = null;

                return pos;
            }
            else
                return -1;
        }

        /// <summary>
        /// Convert (simplified) Roman Numerals
        /// </summary>
        /// <param name="Number"></param>
        /// <returns></returns>
        private int ConvertRomanToInt(String Number)
        {
            String[] roman = {"","I","II","III","IV","V","VI","VII","VIII","IX",
								"X","XI","XII","XIII","XIV","XV","XVI","XVII","XVIII","XIX",
								"XX","XXI","XXII","XXIII","XXIV","XXV","XXVI","XXVII","XXVIII","XXIX",
								"XXX","XXXI","XXXII","XXXIII","XXXIV","XXXV","XXXVI","XXXVII","XXXVIII","XXXIX",
								"XL","XLI","XLII","XLIII","XLIV","XLV","XLVI","XLVII","XLVIII","XLIX",
								"L","LI","LII","LIII","LIV","LV","LVI","LVII","LVIII","LIX",
								"LX","LXI","LXII","LXIII","LXIV","LXV","LXVI","LXVII","LXVIII","LXIX",
								"LXX","LXXI","LXXII","LXXIII","LXXIV","LXXV","LXXVI","LXXVII","LXXVIII","LXXIX",
								"LXXX","LXXXI","LXXXII","LXXXIII","LXXXIV","LXXXV","LXXXVI","LXXXVII","LXXXVIII","LXXXIX",
								"XC","XCI","XCII","XCIII","XCIV","XCV","XCVI","XCVII","XCVIII","XCIX",
								"C"
			};

            if (!String.IsNullOrEmpty(Number))
            {

                List<String> RomanNumerals = new List<String>(roman);

                Number = Number.Replace(" ", "").Replace("-", "").ToUpper();
                int pos = RomanNumerals.IndexOf(Number);

                RomanNumerals = null;

                return pos;
            }
            else
                return -1;
        }

        /// <summary>
        /// Clean up the HTML
        /// </summary>
        /// <param name="OrigFileName"></param>
        /// <param name="CleanFilename"></param>
        private void HtmlTidy(String OrigFileName, String CleanFilename)
        {
            // First, perform some cleaning such as replacing certain
            // characters
            BuildAccentedChars();

            // Get the HtmlTidy path and if it is not defined, then assume
            // the base assembly folder
            String TidyPath = ConfigurationManager.AppSettings["HtmlTidy"];
            if (String.IsNullOrEmpty(TidyPath))
                TidyPath = basePath;

            // Read all contents
            String contents = File.ReadAllText(OrigFileName, Encoding.Default);

            // Replace some values
            contents = contents.Replace("&#160;", " ");
            contents = contents.Replace("`", "'");
            contents = contents.Replace("<br>", "<br />");
            // Now strip nearly all tags from the file
            contents = CleanHtml(contents);

            // Write out the cleaned file for use
            File.WriteAllText(CleanFilename, contents);

            // Now execute HtmlTidy on the cleaned file
            Process tidy = new Process();
            tidy.StartInfo.FileName = Path.Combine(TidyPath, @"tidy.exe");
            tidy.StartInfo.Arguments = String.Format("-config \"{0}\" \"{1}\"", Path.Combine(TidyPath, "config.txt"), CleanFilename);

            // Show/Hide the tidy window
            if (quietOperation)
                tidy.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            
            tidy.Start();
            tidy.WaitForExit();
            tidy.Dispose();

            // Read the newly cleaned file and replace some values set by HtmlTidy
            contents = File.ReadAllText(CleanFilename);
            contents = contents.Replace("<br>", "<br />");
            contents = contents.Replace("<br />\r\n", "<br />").Replace("<br />\n", "<br />").Replace("<br />\r", "<br />");

            // Write out the cleaned file for use
            File.WriteAllText(CleanFilename, contents);
        }

        /// <summary>
        /// Strip HTML tags from a file
        /// </summary>
        /// <param name="Html"></param>
        /// <returns></returns>
        private String CleanHtml(String Html)
        {
            // Start by completely removing all unwanted tags     
            // Then run another pass over the html (twice seems to be safest), removing unwanted attributes
            Html = Regex.Replace(Html, @"<[/]?(" + ConfigurationManager.AppSettings["TagsToRemove"] + @":\w+)[^>]*?>", "", RegexOptions.IgnoreCase);
            Html = Regex.Replace(Html, @"<([^>]*)(?:" + ConfigurationManager.AppSettings["TagsToRemove"] + @":\w+)=(?:'[^']*'|""[^""]*""|[^\s>]+)([^>]*)>", "<$1$2>", RegexOptions.IgnoreCase);
            Html = Regex.Replace(Html, @"<([^>]*)(?:" + ConfigurationManager.AppSettings["TagsToRemove"] + @":\w+)=(?:'[^']*'|""[^""]*""|[^\s>]+)([^>]*)>", "<$1$2>", RegexOptions.IgnoreCase);
            
            return Html;
        }

        /// <summary>
        /// Strip an HTML tag and all attributes
        /// </summary>
        /// <param name="Html"></param>
        /// <param name="Tag"></param>
        /// <returns></returns>
        private String StripSingleTag(String Html, String Tag)
        {
            Html = Regex.Replace(Html, @"<[/]?(" + Tag + @"|[" + Tag + @"]:\w+)[^>]*?>", "", RegexOptions.IgnoreCase);
            return Html;
        }

        /// <summary>
        /// Strip an HTML tag and all attributes
        /// </summary>
        /// <param name="Html"></param>
        /// <param name="Tag">Pass empty String to remove all tags</param>
        /// <returns></returns>
        private String StripTags(String Html, String Tag)
        {
            int startTag = -1, endTag = -1;

            while (Html.IndexOf("<" + Tag) >= 0)
            {
                startTag = Html.IndexOf("<" + Tag);
                if (startTag >= 0)
                    endTag = Html.IndexOf(">", startTag);

                if (endTag > startTag)
                    Html = Html.Remove(startTag, endTag - startTag + 1);

                if (endTag == -1)
                {
                    Html = Html.Remove(startTag);
                    break;
                }
            }

            return Html;
        }

        /// <summary>
        /// Replace certain char values with the escaped HTML value
        /// </summary>
        /// <param name="InputText"></param>
        /// <returns></returns>
        private String HtmlEncode(String InputText)
        {
            int i;
            for (i = 161; i <= 255; i++)
                InputText = InputText.Replace(Convert.ToString(Convert.ToChar(i)), "&#" + i.ToString() + ";");

            InputText = InputText.Replace(Convert.ToString(Convert.ToChar(8211)), "&#8211;");
            InputText = InputText.Replace(Convert.ToString(Convert.ToChar(8212)), "&#8212;");
            InputText = InputText.Replace(Convert.ToString(Convert.ToChar(8213)), "&#8213;");
            InputText = InputText.Replace(Convert.ToString(Convert.ToChar(8216)), "&#8216;");
            InputText = InputText.Replace(Convert.ToString(Convert.ToChar(8217)), "&#8217;");
            InputText = InputText.Replace(Convert.ToString(Convert.ToChar(8220)), "&#8220;");
            InputText = InputText.Replace(Convert.ToString(Convert.ToChar(8221)), "&#8221;");
            InputText = InputText.Replace(Convert.ToString(Convert.ToChar(8224)), "&#8224;");
            InputText = InputText.Replace(Convert.ToString(Convert.ToChar(8225)), "&#8225;");
            InputText = InputText.Replace(Convert.ToString(Convert.ToChar(8226)), "&#8226;");
            InputText = InputText.Replace(Convert.ToString(Convert.ToChar(8230)), "&#8230;");
            InputText = InputText.Replace(Convert.ToString(Convert.ToChar(8242)), "&#8242;");
            InputText = InputText.Replace(Convert.ToString(Convert.ToChar(8592)), "&#8592;");
            InputText = InputText.Replace(Convert.ToString(Convert.ToChar(8594)), "&#8594;");
            InputText = InputText.Replace(Convert.ToString(Convert.ToChar(8722)), "&#8722;");

            return InputText;
        }

        /// <summary>
        /// Simple ToTitleCase
        /// </summary>
        /// <param name="Title"></param>
        /// <returns></returns>
        private String TitleCase(String Title)
        {
            if (chkToTitleCase.Checked)
            {
                String title = Title.ToLower();
                title = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(title);
                return title;
            }
            else
                return Title;
        }

        /// <summary>
        /// Check if a chapter starts with a heading
        /// </summary>
        /// <param name="Chapter"></param>
        /// <param name="Spacer"></param>
        /// <returns></returns>
        private bool StartsWithChapterHeading(String Chapter, String Spacer)
        {
            bool result = false;
            String[] chapters = ConfigurationManager.AppSettings["ChapterNames"].Split(',');

            foreach (String item in chapters)
            {
                if (Chapter.ToLowerInvariant().StartsWith(item.ToLowerInvariant() + Spacer))
                {
                    result = true;
                    break;
                }
            }

            return result;
        }

        /// <summary>
        /// Check to see if chapter is an Alternate chapter
        /// </summary>
        /// <param name="Chapter"></param>
        /// <returns></returns>
        private bool IsAlternateChapter(String Chapter)
        {
            String[] alternates = ConfigurationManager.AppSettings["AlternateChapters"].Split(new char[] { ',' });

            foreach (String alternate in alternates)
            {
                if (alternate.Equals(Chapter, StringComparison.InvariantCultureIgnoreCase))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Guess chapters based on the document type and chapter heading info
        /// </summary>
        private void GuessChapters()
        {
            runningChapterCount = 0;

            if (ddlType.Text == "Text")
                GuessChaptersText();
            else
            {
                if (String.IsNullOrEmpty(tbxChapterBeginTag.Text.Trim()))
                {
                    MessageBox.Show("Enter a begin tag.");
                    tbxChapterBeginTag.Focus();
                }
                else
                {
                    GuessChaptersHtml();
                }
            }
        }

        /// <summary>
        /// Guess chapters in a TEXT document
        /// Bit rough, and would suggest HTML at all times
        /// </summary>
        private void GuessChaptersText()
        {
            String line;
            int chapterNumber = 0;
            int lineNumber = 0;
            String keyword = "";
            String method = "";
            String alternateChapterNames = ConfigurationManager.AppSettings["AlternateChapters"];

            if (ddlChapterKeyword.Text.StartsWith("Alpha"))
            {
                keyword = "";
                method = "alpha";
            }
            else if (ddlChapterKeyword.Text.StartsWith("Roman"))
            {
                keyword = "";
                method = "roman";
            }
            else
            {
                keyword = ddlChapterKeyword.Text;
                method = "";
            }

            using (StreamReader sr = File.OpenText(tbxFileName.Text))
            {
                line = sr.ReadLine();
                lineNumber++;
                while (line != null)
                {
                    line = line.Trim();
                    if ((line.Trim().StartsWith(keyword) && (!String.IsNullOrEmpty(keyword)) && (String.IsNullOrEmpty(method)) && (line.Length > 0) && (line.Length < 80))
                        || ((alternateChapterNames.IndexOf(line.Trim().ToUpper()) >= 0) && (chkIntro.Checked) && (line.Length > 0))
                        )
                    {
                        AddChapter(line, lineNumber, chapterNumber, false, false);

                        chapterNumber++;
                    }
                    else if ((method == "alpha") && (line.Length >= 3) && (line.Length < 80))
                    {
                        String chapterName = "";
                        String chapterTitle = "";

                        SplitChapterLine(line, tbxChapterSeparator.Text, ref chapterName, ref chapterTitle);
                        if (ConvertAlphaToInt(chapterName) >= 0)
                            AddChapter(line, lineNumber, chapterNumber, false, false);

                        chapterNumber++;
                    }
                    else if ((method == "roman") && (line.Length > 0) && (line.Length < 80))
                    {
                        String chapterName = "";
                        String chapterTitle = "";

                        SplitChapterLine(line, tbxChapterSeparator.Text, ref chapterName, ref chapterTitle);
                        if (ConvertRomanToInt(chapterName) >= 0)
                            AddChapter(line, lineNumber, chapterNumber, false, false);

                        chapterNumber++;
                    }

                    line = sr.ReadLine();
                    lineNumber++;
                }
            }

            // Guess the ending line numbers
            GuessEndLineNumbers();
        }

        /// <summary>
        /// Guess chapters based on an H1, H2, H3 (normally) structure
        /// </summary>
        private void GuessChaptersHtml()
        {
            String line;
            int chapterNumber = 0;
            int lineNumber = 0;
            String beginTag = "";
            String childBeginTag = "";
            String grandchildBeginTag = "";

            // Setup the details to parse the file
            workingFileName = tbxCleanFileName.Text;
            beginTag = tbxChapterBeginTag.Text;
            childBeginTag = tbxChildChapterBeginTag.Text;
            grandchildBeginTag = tbxGrandchildChapterBeginTag.Text;

            // Begin reading the file and step through each line
            using (StreamReader sr = File.OpenText(workingFileName))
            {
                line = sr.ReadLine();
                lineNumber++;
                while (line != null)
                {
                    // Check to see what level the chapter belongs
                    // and then add the chapter
                    line = line.Trim();
                    if (line.Trim().StartsWith(beginTag) && (line.Length > 0))
                    {
                        String newLine = StripTags(line, "");
                        AddChapter(line, lineNumber, chapterNumber, false, false);
                        chapterNumber++;
                    }
                    else if (line.Trim().StartsWith(childBeginTag) && (line.Length > 0))
                    {
                        String newLine = StripTags(line, "");
                        AddChapter(line, lineNumber, chapterNumber, true, false);
                        chapterNumber++;
                    }
                    else if (line.Trim().StartsWith(grandchildBeginTag) && (line.Length > 0))
                    {
                        String newLine = StripTags(line, "");
                        AddChapter(line, lineNumber, chapterNumber, false, true);
                        chapterNumber++;
                    }

                    line = sr.ReadLine();
                    lineNumber++;
                }
            }

            // Now guess the ending line numbers for each chapter
            GuessEndLineNumbers();
        }

        /// <summary>
        /// Process chapters and set first node as selected
        /// </summary>
        private void ProcessChapters()
        {
            ProcessFile();
            treeChapters.SelectedNode = treeChapters.Nodes[0];
        }

        /// <summary>
        /// Get the file line count and process each chapter
        /// </summary>
        private void ProcessFile()
        {
            int lineCount = 1;
            String line;

            using (StreamReader sr = File.OpenText(workingFileName))
            {
                line = sr.ReadLine();
                while (line != null)
                {
                    line = sr.ReadLine();
                    lineCount++;
                }
                lineCount++;
            }
            line = null;

            // Extract the text for each chapter based on start/end
            ProcessEachChapter(treeChapters.Nodes);
        }

        /// <summary>
        /// Process each chapter that has been guessed
        /// </summary>
        /// <param name="Nodes"></param>
        private void ProcessEachChapter(TreeNodeCollection Nodes)
        {
            String strippedLine;
            String line;

            int blankLineCount = 0;
            int chapterStart = 0, chapterEnd = 0;
            int i = 1;
            bool skipFirstBlankLine = true;

            // Loop through each tree node and extract the text info
            foreach (TreeNode node in Nodes)
            {
                TreeNode curNode = node;
                Chapter chapter = (Chapter)curNode.Tag;
                chapterStart = chapter.startLineNumber;
                chapterEnd = chapter.endLineNumber;

                // Probably not the best way to do this, but open the
                // file for each chapter and read until its start,
                // extract until its end
                using (StreamReader sr = File.OpenText(workingFileName))
                {
                    // Go until the start line if found
                    skipFirstBlankLine = true;
                    line = sr.ReadLine();
                    i = 1;
                    while ((line != null) && (i <= chapterStart))
                    {
                        i++;
                        line = sr.ReadLine();
                    }

                    // Found the start, now go to the end and process each line
                    while ((line != null) && (i <= chapterEnd))
                    {
                        strippedLine = line.Trim();
                        if (strippedLine.Length > 0)
                        {
                            // Blank line, so add empty paragraph
                            String paragraphPadding = "";
                            if (blankLineCount == 2)
                                paragraphPadding = "<p></p>";

                            if (ddlType.Text == "Text")
                            {
                                // If this is a TEXT file, then add HTML code
                                line = line.Trim();
                                chapter.rawText += line + "\r\n\r\n";
                                chapter.html += paragraphPadding + "<p>" + HtmlEncode(line) + "</p>\r\n";
                            }
                            else
                            {
                                //This is HTML, so clean the line
                                char[] charsToTrim = { ' ' };
                                line = line.TrimEnd(charsToTrim);
                                chapter.rawText += StripTags(line, "") + "\r\n\r\n";
                                chapter.html += line + "\r\n";
                            }

                            // Check for the end of the HTML document
                            if (chapter.html.IndexOf("</body") >= 0)
                            {
                                chapter.html = chapter.html.Replace("</body>" + Environment.NewLine, "").Replace("</body>", "");
                                chapter.endLineNumber--;
                            }
                            if (chapter.html.IndexOf("</html") >= 0)
                            {
                                chapter.html = chapter.html.Replace("</html>" + Environment.NewLine, "").Replace("</html>", "");
                                chapter.endLineNumber--;
                            }

                            curNode.Tag = chapter;
                            blankLineCount = 0;
                        }
                        else if ((skipFirstBlankLine) && (strippedLine.Length == 0))
                        {
                            skipFirstBlankLine = false;
                        }
                        else if ((!skipFirstBlankLine) && (strippedLine.Length == 0))
                        {
                            blankLineCount++;
                        }

                        line = sr.ReadLine();
                        i++;
                    }
                }

                // Now process the children, if any, of this node
                if (node.Nodes.Count > 0)
                    ProcessEachChapter(node.Nodes);
            }
        }

        /// <summary>
        /// Split a chapter line based on the provided separator
        /// </summary>
        /// <param name="line"></param>
        /// <param name="ChapterSeparator"></param>
        /// <param name="chapterName"></param>
        /// <param name="chapterTitle"></param>
        private void SplitChapterLine(String line, String ChapterSeparator, ref String chapterName, ref String chapterTitle)
        {
            chapterName = "";
            chapterTitle = "";
            String[] items;
            String[] parms = new String[] { ChapterSeparator };

            // Check to see if the separator is even present
            if (ChapterSeparator.Length > 0)
                items = line.Split(parms, StringSplitOptions.None);
            else
                items = null;

            if (items != null)
            {
                // We have items, so assign their values
                switch (items.Length)
                {
                    case 0:
                    case 1:
                        chapterName = line;
                        chapterTitle = "";
                        break;
                    case 2:
                        chapterName = items[0];
                        chapterTitle = items[1];
                        break;
                }
            }
            else
            {
                // No items
                chapterName = line;
                chapterTitle = "";
            }
        }

        /// <summary>
        /// Add a chapter, set its level
        /// </summary>
        /// <param name="line"></param>
        /// <param name="lineNumber"></param>
        /// <param name="chapterNumber"></param>
        /// <param name="asChild"></param>
        /// <param name="asGrandchild"></param>
        private void AddChapter(String line, int lineNumber, int chapterNumber, bool asChild, bool asGrandchild)
        {
            // Set some base info
            int number;
            String chapterPrefix = (asChild | asGrandchild) == true ? "Part " : "Chapter ";
            String chapterName = "";
            String chapterTitle = "";
            String alternateChapterNames = ConfigurationManager.AppSettings["AlternateChapters"];
            String separator = tbxChapterSeparatorHtml.Text;

            // Split the chapter line and clean it of any HTML as
            // it may cause issues with the Table of Contents of eReaders
            SplitChapterLine(StripTags(line, ""), separator, ref chapterName, ref chapterTitle);
            if (!StartsWithChapterHeading(chapterName, " "))
            {
                // The chapter doesn't start with a Chapter heading
                // so process the names (ONE --> 1, I --> 1)
                // Also, could be an Alternate title
                number = -1;
                number = ConvertAlphaToInt(chapterName);
                if (number == -1)
                    number = ConvertRomanToInt(chapterName);

                if (number > -1)
                {
                    chapterName = chapterPrefix + number.ToString();
                    if (!(asChild | asGrandchild))
                        runningChapterCount = number;
                }
                else if (IsAlternateChapter(chapterName))
                {
                    // Do nothing
                }
                else if (!IsAlternateChapter(chapterName))
                {
                    if (separator.Length > 0)
                    {
                        if (!(asChild | asGrandchild))
                            runningChapterCount++;

                        chapterTitle = TitleCase(chapterName);
                        chapterName = chapterPrefix + runningChapterCount.ToString();
                    }
                    else
                    {
                        chapterTitle = "";
                        chapterName = TitleCase(chapterName);
                    }
                }
                else
                    chapterName = chapterTitle;
            }
            else //if (StartsWithChapterHeading(chapterName, " "))
            {
                String[] chapterBits = chapterName.Split(' ');

                if (StartsWithChapterHeading(chapterBits[0], ""))
                    chapterPrefix = TitleCase(chapterBits[0]) + " ";

                if (IsNumeric(chapterBits[1]))
                {
                    chapterName = chapterPrefix + chapterBits[1];
                    if (!(asChild | asGrandchild))
                        runningChapterCount = int.Parse(chapterBits[1]);
                }
                else if (ConvertAlphaToInt(chapterBits[1]) > 0)
                {
                    chapterName = chapterPrefix + ConvertAlphaToInt(chapterBits[1]).ToString();
                    if (!(asChild | asGrandchild))
                        runningChapterCount = ConvertAlphaToInt(chapterBits[1]);
                }
                else if (ConvertRomanToInt(chapterBits[1]) > 0)
                {
                    chapterName = chapterPrefix + ConvertRomanToInt(chapterBits[1]).ToString();
                    if (!(asChild | asGrandchild))
                        runningChapterCount = ConvertRomanToInt(chapterBits[1]);
                }
                chapterTitle = TitleCase(chapterTitle);
            }
            //else
            //    chapterTitle = TitleCase(chapterTitle);

            if (chapterTitle.EndsWith("."))
                chapterTitle = chapterTitle.Substring(0, chapterTitle.Length - 1);

            // Start setting up the chapter
            Chapter chapter = new Chapter();
            if (!String.IsNullOrEmpty(chapterName))
            {
                if (alternateChapterNames.IndexOf(chapterTitle, StringComparison.InvariantCultureIgnoreCase) < 0)
                    chapter.chapterName = chapterName + (chapterTitle == "" ? "" : " - " + chapterTitle);
                else
                    chapter.chapterName = chapterName;
            }
            else
            {
                chapterName = chapterTitle;
                chapter.chapterName = chapterTitle;
            }

            // Setup the chapter title
            chapter.chapterTitle = chapterTitle;
            if (asChild)
            {
                String parent = ((Chapter)treeChapters.Nodes[treeChapters.Nodes.Count - 1].Tag).id;
                chapter.id = parent + TitleCase(chapterName).Replace(" ", "");
            }
            else if (asGrandchild)
            {
                String parent = ((Chapter)treeChapters.Nodes[treeChapters.Nodes.Count - 1].Nodes[treeChapters.Nodes[treeChapters.Nodes.Count - 1].Nodes.Count - 1].Tag).id;
                chapter.id = parent + TitleCase(chapterName).Replace(" ", "");
            }
            else
                chapter.id = TitleCase(chapterName).Replace(" ", "");

            chapter.id = CleanString(chapter.id);
            chapter.startLineNumber = lineNumber;

            // Update the chapter tree nodes
            TreeNode newNode = new TreeNode(!String.IsNullOrEmpty(chapterName) ? chapterName : chapterTitle);
            newNode.Name = chapterNumber.ToString();
            newNode.Tag = chapter;

            // Add the chapter based on depth
            if (!(asChild | asGrandchild))
                treeChapters.Nodes.Add(newNode);
            else if (asChild)
                treeChapters.Nodes[treeChapters.Nodes.Count - 1].Nodes.Add(newNode);
            else if (asGrandchild)
                treeChapters.Nodes[treeChapters.Nodes.Count - 1].Nodes[treeChapters.Nodes[treeChapters.Nodes.Count - 1].Nodes.Count - 1].Nodes.Add(newNode);
            else
                treeChapters.Nodes.Add(newNode);

            newNode = null;
        }

        /// <summary>
        /// Remove all punctuation from a String
        /// </summary>
        /// <param name="Input"></param>
        /// <returns></returns>
        private String CleanString(String Input)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < Input.Length; i++)
                if (char.IsLetterOrDigit(Input[i]))
                    sb.Append(Input[i]);

            return sb.ToString();
        }

        /// <summary>
        /// Guess the ending line numbers
        /// </summary>
        private void GuessEndLineNumbers()
        {
            int lineCount = 1;
            String line = "";
            using (StreamReader sr = File.OpenText(workingFileName))
            {
                // Get the total line count -- probably a better way...
                line = sr.ReadLine();
                while (line != null)
                {
                    line = sr.ReadLine();
                    lineCount++;
                }
                lineCount++;
            }
            line = null;

            // Now, process the chapters to get the end lines
            ProcessChapterEndLineNumbers(treeChapters.Nodes, lineCount);
        }

        /// <summary>
        /// Get the end line numbers for a chapter
        /// </summary>
        /// <param name="Nodes"></param>
        /// <param name="lineCount"></param>
        private void ProcessChapterEndLineNumbers(TreeNodeCollection Nodes, int lineCount)
        {
            // Basic idea, go through each node and get the next node's start
            // line number and simply subtract one
            // However, if there are children, then process the end lines
            // for each child, too.
            //
            // Grab the chapter and the next chapter, update the end line,
            // resave the chapter, rinse-repeat.
            int i;
            TreeNode node;
            for (i = 0; i < Nodes.Count; i++)
            {
                node = null;
                node = Nodes[i];
                if (i < Nodes.Count - 1)
                {
                    if (node.Nodes.Count > 0)
                    {
                        Chapter chapter = (Chapter)node.Tag;
                        Chapter chapterNext = (Chapter)node.Nodes[0].Tag;

                        chapter.endLineNumber = chapterNext.startLineNumber - 1;
                        node.Tag = chapter;

                        ProcessChapterEndLineNumbers(node.Nodes, lineCount);
                    }
                    else
                    {
                        Chapter chapter = (Chapter)node.Tag;
                        Chapter chapterNext = (Chapter)Nodes[i + 1].Tag;

                        chapter.endLineNumber = chapterNext.startLineNumber - 1;
                        node.Tag = chapter;
                    }
                }
                else
                {
                    // Final tree node, so process according to if it is a top-level
                    // or has one or more parents
                    if (node.Nodes.Count > 0)
                    {
                        Chapter chapter = (Chapter)node.Tag;
                        Chapter chapterNext = (Chapter)node.Nodes[0].Tag;

                        chapter.endLineNumber = chapterNext.startLineNumber - 1;
                        node.Tag = chapter;

                        ProcessChapterEndLineNumbers(node.Nodes, lineCount);
                    }
                    else if (node.Parent != null)
                    {
                        Chapter chapter = (Chapter)node.Tag;

                        if (node.Parent.NextNode != null)
                        {
                            Chapter chapterNext = (Chapter)node.Parent.NextNode.Tag;
                            chapter.endLineNumber = chapterNext.startLineNumber - 1;
                        }
                        else
                        {
                            chapter.endLineNumber = lineCount;
                        }
                        node.Tag = chapter;
                    }
                    else
                    {
                        if (node.Nodes.Count > 0)
                        {
                            Chapter chapter = (Chapter)node.Tag;
                            Chapter chapterNext = (Chapter)node.Nodes[0].Tag;

                            chapter.endLineNumber = chapterNext.startLineNumber - 1;
                            node.Tag = chapter;

                            ProcessChapterEndLineNumbers(node.Nodes, lineCount);
                        }
                        else
                        {
                            Chapter chapter = (Chapter)node.Tag;
                            chapter.endLineNumber = lineCount;
                            node.Tag = chapter;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Create and EPUB file
        /// </summary>
        /// <returns></returns>
        private String CreateEpub()
        {
            String fileName = null;
            try
            {
                CreateDirectories();
                CreateMimeType();
                CreateMetaINF();
                CreateContentOpf();
                CreateTOC();
                CreateCSS();
                CreateChapters();
                fileName = CreateEpubArchive();
            }
            catch (Exception ex)
            {
                if (Directory.Exists(mainDir))
                    Directory.Delete(mainDir, true);

                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return fileName;
        }

        /// <summary>
        /// Auto-create an EPUB file by making certain assumptions
        /// based on file names and locations, etc...
        /// </summary>
        /// <returns></returns>
        private String AutoCreateEpub()
        {
            String fileName = null;
            String imageLocation = Path.GetDirectoryName(tbxFileName.Text);
            String imageName = String.Format("{0}_{1}.{2}",
                                tbxAuthorSort.Text.Substring(0, tbxAuthorSort.Text.IndexOf(",")).Replace(" ", ""),
                                tbxTitle.Text.Replace(" ", "").Replace(",", ""),
                                tbxImageExtension.Text);

            if (!File.Exists(Path.Combine(imageLocation, imageName)))
            {
                MessageBox.Show(String.Format("Cannot find Cover: {0}", Path.Combine(imageLocation, imageName)));
                return null;
            }

            GuessChapters();
            ProcessChapters();
            AddTitlePage(Path.Combine(imageLocation, imageName), imageName);
            fileName = CreateEpub();

            return fileName;
        }

        /// <summary>
        /// Clean one or more HTML files called from the command-line
        /// </summary>
        private void CommandLineClean()
        {
            int i = 1;

            frmBlindSave blind = new frmBlindSave();
            blind.Show();
            Application.DoEvents();

            quietOperation = true;
            foreach (String item in autoFileNames)
            {
                blind.NumFiles = autoFileNames.Count;
                blind.Info = String.Format("({0} of {1}) - Processing {2}", i, autoFileNames.Count, Path.GetFileName(item));
                blind.UpdateInfo();

                HtmlTidy(item, item);

                blind.UpdateProgress();
            }

            blind.Close();
            blind.Dispose();
        }

        /// <summary>
        /// Auto-create one or more EPUBs from the command-line
        /// </summary>
        private void CommandLineCreate()
        {
            int i = 1;
            List<String> fileNames = new List<String>();

            frmBlindSave blind = new frmBlindSave();
            blind.Show();
            Application.DoEvents();

            quietOperation = true;
            foreach (String item in autoFileNames)
            {
                blind.NumFiles = autoFileNames.Count;
                blind.Info = String.Format("({0} of {1}) - Processing {2}", i, autoFileNames.Count, Path.GetFileName(item));
                blind.UpdateInfo();

                tbxFileName.Text = item;
                LoadFile();
                Setup();
                fileNames.Add(AutoCreateEpub());
                i++;
                blind.UpdateProgress();
            }

            // Ask to add the EPUB to the ATOM library file
            if (MessageBox.Show("Add all items to Library?", "Add items?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                ListBox lstImages = new ListBox();

                foreach (String item in fileNames)
                {
                    Program.ReadEpub(item, lstImages);
                    String subFolder = Path.GetDirectoryName(item);
                    subFolder = subFolder.Substring(ConfigurationManager.AppSettings["BaseDir"].Length);
                    Program.AddToLibrary(item, lstImages, subFolder, false);
                }

                MessageBox.Show("ePub(s) added to Library.");
            }

            blind.Close();
            blind.Dispose();
        }

        /// <summary>
        /// Create the EPUB folder structure
        /// </summary>
        private void CreateDirectories()
        {
            if (!String.IsNullOrEmpty(tbxTitle.Text))
            {
                mainDir = tbxTitle.Text;
                String newMainDir = mainDir;
                int i = 1;

                while (Directory.Exists(newMainDir))
                {
                    newMainDir = String.Format("{0} ({1})", mainDir, i.ToString());
                    i++;
                }

                // Based on the EPUB spec
                // OPS can also be OEBPS
                mainDir = newMainDir;
                metaDir = mainDir + @"\META-INF";
                opsDir = mainDir + @"\OPS";
                cssDir = mainDir + @"\OPS\css";
                imagesDir = mainDir + @"\OPS\images";

                Directory.CreateDirectory(mainDir);
                Directory.CreateDirectory(metaDir);
                Directory.CreateDirectory(opsDir);
                Directory.CreateDirectory(cssDir);
                Directory.CreateDirectory(imagesDir);
            }
        }

        /// <summary>
        /// Copy the MIMETYPE file
        /// </summary>
        private void CreateMimeType()
        {
            File.Copy(Path.Combine(supportDir, "mimetype"), Path.Combine(mainDir, "mimetype"), true);
        }

        /// <summary>
        /// Copy the default container.xml file
        /// </summary>
        private void CreateMetaINF()
        {
            File.Copy(Path.Combine(supportDir, "container.xml"), Path.Combine(metaDir, "container.xml"), true);
        }

        /// <summary>
        /// Create the OPF file and save it
        /// TODO: consider not using global for the OpfBody!
        /// </summary>
        private void CreateContentOpf()
        {
            OpfBody = "<?xml version=\"1.0\" encoding=\"UTF-8\" ?>\n";
            OpfBody += "<package version=\"2.0\" unique-identifier=\"PrimaryID\" xmlns=\"http://www.idpf.org/2007/opf\">\n";

            // Add each section to the OPF file
            CreateMetaData();
            CreateManifest();
            CreateSpine();
            CreateGuide();

            OpfBody += "</package>";

            File.WriteAllText(opsDir + @"\content.opf", OpfBody);
        }

        /// <summary>
        /// Copy the main CSS file
        /// </summary>
        private void CreateCSS()
        {
            File.Copy(Path.Combine(supportDir, "main.css"), Path.Combine(cssDir, "main.css"), true);
        }

        /// <summary>
        /// Create the MetaData for the OPF file
        /// </summary>
        private void CreateMetaData()
        {
            String indent = "\t";
            String child = "\t\t";

            OpfBody += indent + "<metadata xmlns:dc=\"http://purl.org/dc/elements/1.1/\" xmlns:opf=\"http://www.idpf.org/2007/opf\">\n";
            OpfBody += String.Format("{0}<dc:title>{1}</dc:title>\n", child, tbxTitle.Text);
            OpfBody += String.Format("{0}<dc:identifier id=\"PrimaryID\" opf:scheme=\"URN\">urn:uuid:{1}</dc:identifier>\n", child, tbxIdentifier.Text);
            OpfBody += String.Format("{0}<dc:language>{1}</dc:language>\n", child, tbxLanguage.Text);
            OpfBody += String.Format("{0}<dc:creator opf:role=\"aut\" opf:file-as=\"{1}\">{1}</dc:creator>\n", child, tbxAuthorSort.Text, tbxAuthor.Text);
            OpfBody += String.Format("{0}<dc:publisher>{1}</dc:publisher>\n", child, tbxPublisher.Text);
            OpfBody += String.Format("{0}<dc:description>{1}</dc:description>\n", child, tbxDescription.Text);
            OpfBody += String.Format("{0}<dc:coverage>{1}</dc:coverage>\n", child, tbxCoverage.Text);
            OpfBody += String.Format("{0}<dc:source>{1}</dc:source>\n", child, tbxSource.Text);
            OpfBody += String.Format("{0}<dc:date opf:event=\"original-publication\">{1}</dc:date>\n", child, tbxOrigPublishDate.Text);
            OpfBody += String.Format("{0}<dc:date opf:event=\"ops-publication\">{1}</dc:date>\n", child, tbxDateCreated.Text);
			OpfBody += String.Format("{0}<dc:rights>{1}</dc:rights>\n", child, tbxRights.Text);

            foreach (String subject in tbxSubject.Text.Split(new Char[] { ',' }))
            {
                OpfBody += String.Format("{0}<dc:subject>{1}</dc:subject>\n", child, subject.Trim());
            }
			OpfBody += child + "<meta name=\"cover\" content=\"cover\" />\n";
			OpfBody += indent + "</metadata>\n";
        }

        /// <summary>
        /// Create the Manifest section
        /// </summary>
        private void CreateManifest()
        {
            String indent = "\t";
            String child = "\t\t";
            int i = 0;

            OpfBody += indent + "<manifest>\n";

            // Add each chapter tag
            OpfBody += child + "<!-- Content Documents -->\n";
            ProcessManifestItems(treeChapters.Nodes);

            OpfBody += child + "\n<!-- CSS Style Sheets -->\n";
            OpfBody += child + "<item id=\"main-css\" href=\"css/main.css\" media-type=\"text/css\"/>\n";
            
            // Add each image tag
            OpfBody += child + "\n<!-- Images -->\n";
            foreach (TreeNode node in treeImages.Nodes)
            {
                String id = "image-" + i.ToString();
                String type = "jpeg";

                Illustration image = (Illustration)node.Tag;
                switch (Path.GetExtension(image.name).ToLower())
                {
                    case ".jpg":
                    case ".jpeg":
                        type = "jpeg";
                        break;
                    case ".gif":
                        type = "gif";
                        break;
                    case ".png":
                        type = "png";
                        break;
                }
				
                // Set the cover image meta data so that eReaders like the Nook can find
                // the image to use as a thumbnail
				if (image.useAsCover)
					OpfBody += child + String.Format("{0}<item id=\"cover\" href=\"images/{1}\" media-type=\"image/{2}\"/>\n", child, image.name, type);
				else
					OpfBody += child + String.Format("{0}<item id=\"{1}\" href=\"images/{2}\" media-type=\"image/{3}\"/>\n", child, id, image.name, type);

                File.Copy(image.location, Path.Combine(imagesDir, image.name), true);
                i++;
            }

            OpfBody += child + "\n<!-- NCX -->\n";
            OpfBody += child + "<item id=\"ncx\" href=\"toc.ncx\" media-type=\"application/x-dtbncx+xml\"/>\n";
            OpfBody += indent + "</manifest>\n";
        }

        /// <summary>
        /// Create the chapter tags for the Manifest section
        /// </summary>
        /// <param name="Nodes"></param>
        private void ProcessManifestItems(TreeNodeCollection Nodes)
        {
            String child = "\t\t";

            foreach (TreeNode node in Nodes)
            {
                Chapter chapter = (Chapter)node.Tag;
                OpfBody += String.Format("{0}<item id=\"{1}\" href=\"{1}.xml\" media-type=\"application/xhtml+xml\"/>\n", child, chapter.id);

                if (node.Nodes.Count > 0)
                    ProcessManifestItems(node.Nodes);
            }
        }

        /// <summary>
        /// Create the Spine items for the OPF
        /// </summary>
        private void CreateSpine()
        {
            String indent = "\t";

            OpfBody += indent + "<spine toc=\"ncx\">\n";
            ProcessSpineItems(treeChapters.Nodes);
            OpfBody += indent + "</spine>\n";
        }

        /// <summary>
        /// Create the Spine items
        /// </summary>
        /// <param name="Nodes"></param>
        private void ProcessSpineItems(TreeNodeCollection Nodes)
        {
            String child = "\t\t";

            foreach (TreeNode node in Nodes)
            {
                Chapter chapter = (Chapter)node.Tag;
				if ((chapter.chapterName.Equals("Footnotes", StringComparison.InvariantCultureIgnoreCase))
					|| (chapter.chapterTitle.Equals("Footnotes", StringComparison.InvariantCultureIgnoreCase))
					)
					OpfBody += String.Format("{0}<itemref idref=\"{1}\" linear=\"no\"/>\n", child, chapter.id);
				else
                    OpfBody += String.Format("{0}<itemref idref=\"{1}\" linear=\"yes\"/>\n", child, chapter.id);

                if (node.Nodes.Count > 0)
                    ProcessSpineItems(node.Nodes);
            }
        }

        private void CreateGuide()
        {
            //TODO: ?
        }

        /// <summary>
        /// Create the table of contents
        /// </summary>
        private void CreateTOC()
        {
            using (StreamWriter sw = File.CreateText(opsDir + @"\toc.ncx"))
            {
                sw.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
                sw.WriteLine("<!DOCTYPE ncx PUBLIC \"-//NISO//DTD ncx 2005-1//EN\" \"http://www.daisy.org/z3986/2005/ncx-2005-1.dtd\">");
                sw.WriteLine("<ncx xmlns=\"http://www.daisy.org/z3986/2005/ncx/\" version=\"2005-1\">");
                sw.WriteLine("\t<head>");
                sw.WriteLine("\t\t<!--The following four metadata items are required for all");
                sw.WriteLine("\t\t\tNCX documents, including those conforming to the relaxed");
                sw.WriteLine("\t\t\tconstraints of OPS 2.0-->");
                sw.WriteLine(String.Format("\t\t<meta name=\"dtb:uid\" content=\"{0}\"/>", tbxIdentifier.Text));
                sw.WriteLine(String.Format("\t\t<meta name=\"epub-creator\" content=\"{0}\"/>", tbxEpubCreator.Text));
                sw.WriteLine("\t\t<meta name=\"dtb:depth\" content=\"1\"/>");
                sw.WriteLine("\t\t<meta name=\"dtb:totalPageCount\" content=\"0\"/>");
                sw.WriteLine("\t\t<meta name=\"dtb:maxPageNumber\" content=\"0\"/>");
                sw.WriteLine("\t</head>");
                sw.WriteLine("\t<docTitle>");
                sw.WriteLine(String.Format("\t\t<text>{0}</text>", tbxTitle.Text));
                sw.WriteLine("\t</docTitle>");
                sw.WriteLine("\t<docAuthor>");
                sw.WriteLine(String.Format("\t\t<text>{0}</text>", tbxAuthor.Text));
                sw.WriteLine("\t</docAuthor>");

                sw.WriteLine("\t<navMap>");
                int playOrder = 1;
                WriteTOCChapters(sw, treeChapters.Nodes, ref playOrder);
                sw.WriteLine("\t</navMap>");

                sw.WriteLine("</ncx>");
            }
        }

        /// <summary>
        /// Write the Table of Contents Navigation
        /// </summary>
        /// <param name="sw"></param>
        /// <param name="Nodes"></param>
        /// <param name="playOrder"></param>
        private void WriteTOCChapters(StreamWriter sw, TreeNodeCollection Nodes, ref int playOrder)
        {
            // Write out each chapter
            Chapter chapter;
            foreach (TreeNode node in Nodes)
            {
                //If the chapter has a parent, then add to the parent nested
                chapter = (Chapter)node.Tag;
				if (node.Parent != null & node.Index >= 0)
				{
					String spacer = "";
                    // To support eReaders that cannot nest the TOC, use .... to indent
					if (!treeTOC)
						spacer = "...." + node.Parent.Parent != null ? "...." : "";
					
                    // Write out the chapter name and title
					Chapter parentChapter = (Chapter)node.Parent.Tag;
					sw.WriteLine(String.Format("\t\t<navPoint id=\"navpoint-{0}\" playOrder=\"{0}\">", playOrder.ToString()));
					sw.WriteLine("\t\t\t<navLabel>");
					sw.WriteLine(String.Format("\t\t\t\t<text>{0}{1}</text>", spacer, chapter.chapterName.Replace(" & ", " &amp; ")));
					sw.WriteLine("\t\t\t</navLabel>");
					sw.WriteLine(String.Format("\t\t\t<content src=\"{0}.xml\"/>", chapter.id));

                    // Add nested chapters in the eReader supports it
					if (treeTOC)
						if (node.Nodes.Count > 0)
							WriteTOCChapters(sw, node.Nodes, ref playOrder);
		
					sw.WriteLine("\t\t</navPoint>");

					playOrder++;
				}
				else
				{
                    // This is a top-level chapter, so start a new navPoint
					sw.WriteLine(String.Format("\t\t<navPoint id=\"navpoint-{0}\" playOrder=\"{0}\">", playOrder.ToString()));
					sw.WriteLine("\t\t\t<navLabel>");
					sw.WriteLine(String.Format("\t\t\t\t<text>{0}</text>", chapter.chapterName.Replace(" & ", " &amp; ")));
					sw.WriteLine("\t\t\t</navLabel>");
					sw.WriteLine(String.Format("\t\t\t<content src=\"{0}.xml\"/>", chapter.id));

                    // Add nested chapters in the eReader supports it
                    if (treeTOC)
						if (node.Nodes.Count > 0)
							WriteTOCChapters(sw, node.Nodes, ref playOrder);

					sw.WriteLine("\t\t</navPoint>");

					playOrder++;
				}
				
                // No tree-TOC supported, so process next node
				if (!treeTOC)
					if (node.Nodes.Count > 0)
						WriteTOCChapters(sw, node.Nodes, ref playOrder);
            }
        }

        /// <summary>
        /// Add a title page and set the image to use
        /// </summary>
        private void AddTitlePage()
        {
            // Ask the user for the image location
            frmImages formImages = new frmImages(this);
            formImages.ShowDialog();
            String imageLocation = formImages.location;
            String imageName = formImages.name;
            formImages.Dispose();

            AddTitlePage(imageLocation, imageName);
        }

        /// <summary>
        /// Add a title page to the EPUB
        /// </summary>
        /// <param name="imageLocation"></param>
        /// <param name="imageName"></param>
        private void AddTitlePage(String imageLocation, String imageName)
        {
            String body = File.ReadAllText(Path.Combine(supportDir, "title.xml"));

            // Replace the placeholders in the template with the EPUB values
            body = body.Replace("{TITLE}", tbxTitle.Text);
            body = body.Replace("{AUTHOR}", tbxAuthor.Text);
            body = body.Replace("{PUBLISHDATE}", tbxOrigPublishDate.Text);
            body = body.Replace("{SUBJECT}", tbxSubject.Text);
            body = body.Replace("{SOURCE}", tbxSource.Text);

            if (treeImages.Nodes.Count > 0)
                body = body.Replace("{COVERIMAGE}", ((Illustration)treeImages.Nodes[0].Tag).name);
            else
                body = body.Replace("{COVERIMAGE}", imageName);

            // Add the chapter to the chapter tree
            TreeNode node = new TreeNode();
            Chapter chapter = new Chapter();
            chapter.chapterName = "Title Page";
            chapter.chapterTitle = "Title Page";
            chapter.id = "titlepage";
            chapter.html = body.Replace("\n", "\r\n");
            chapter.rawText = "N/A";
            node.Tag = chapter;
            node.Text = "Title Page";
            node.Name = "t0";
            treeChapters.Nodes.Insert(0, node);

            // Add the image to the image tree
            node = new TreeNode();
            Illustration image = new Illustration();
            image.location = imageLocation;
            image.name = imageName;
            image.useAsCover = true;
            node.Tag = image;
            node.Text = "Cover Image";
            node.Name = "t0";
            treeImages.Nodes.Insert(0, node);
            treeImages.SelectedNode = treeImages.Nodes[0];
            treeChapters.SelectedNode = treeChapters.Nodes[0];

            foreach (TreeNode tnode in treeImages.Nodes)
            {
                if (!tnode.Equals(treeImages.Nodes[0]))
                {
                    image = (Illustration)tnode.Tag;
                    image.useAsCover = false;
                    tnode.Tag = image;
                }
            }
        }

        /// <summary>
        /// Write the chapter files
        /// </summary>
        private void CreateChapters()
        {
            WriteChapters(treeChapters.Nodes);
        }

        /// <summary>
        /// Write the chapter files
        /// </summary>
        /// <param name="Nodes"></param>
        private void WriteChapters(TreeNodeCollection Nodes)
        {
            String chapterName = "";

            foreach (TreeNode node in Nodes)
            {
                // Grab each chapter, its details, and writeout the file
                Chapter chapter = (Chapter)node.Tag;
                using (StreamWriter sw = File.CreateText(opsDir + String.Format("\\{0}.xml", chapter.id)))
                {
					if ((chapter.html == null) || (!chapter.html.StartsWith("<?xml")))
					{
                        // Write out the chapter header
						sw.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\" ?>");
						sw.WriteLine("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.1//EN\" \"http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd\">");
						sw.WriteLine("<html xmlns=\"http://www.w3.org/1999/xhtml\" xml:lang=\"en\">");
						sw.WriteLine("<head>");
						sw.WriteLine(String.Format("<title>{0}</title>", tbxTitle.Text));
						sw.WriteLine("<link rel=\"stylesheet\" href=\"css/main.css\" type=\"text/css\" />");
						sw.WriteLine("<meta http-equiv=\"Content-Type\" content=\"application/xhtml+xml; charset=utf-8\" />");
						sw.WriteLine("</head>");
						sw.WriteLine("<body>");

                        // Determine the chapter name and title
						if (chapter.chapterName.IndexOf(" - ") >= 0)
							chapterName = chapter.chapterName.Substring(0, chapter.chapterName.IndexOf(" - "));
						else
							chapterName = chapter.chapterName;

						if ((!StartsWithChapterHeading(chapterName, "")) && (!IsAlternateChapter(chapterName)))
						{
							chapter.chapterTitle = chapterName;
							chapterName = "";
						}

						if (node.Parent != null)
						{
                            // This is a child chapter, so grab some of the parent details to give
                            // the user some reference as to its position/depth
							String parent = ((Chapter)node.Parent.Tag).chapterName;

							if (parent.IndexOf(" - ") >= 0)
								chapterName = String.Format("{0}{1}", parent.Substring(0, parent.IndexOf(" - ")), String.IsNullOrEmpty(chapterName) ? "" : ": " + chapterName);
							else
								chapterName = String.Format("{0}{1}", parent, String.IsNullOrEmpty(chapterName) ? "" : ": " + chapterName);

							if (node.Parent.Parent != null)
							{
								parent = ((Chapter)node.Parent.Parent.Tag).chapterName;

								if (parent.IndexOf(" - ") >= 0)
									chapterName = String.Format("{0}{1}", parent.Substring(0, parent.IndexOf(" - ")), String.IsNullOrEmpty(chapterName) ? "" : ": " + chapterName);
								else
									chapterName = String.Format("{0}{1}", parent, String.IsNullOrEmpty(chapterName) ? "" : ": " + chapterName);
							}
						}

                        // Write out the default chapter name & title block
						sw.WriteLine(String.Format("<div class=\"title_box\"><div class=\"chapnum\">{0}</div><div class=\"chaptitle\">{1}</div><hr/></div>", chapterName.Replace("&", "&amp;"), chapter.chapterTitle.Replace("&", "&amp;")));

                        // If the chapter doesn't have text, and it has children,
                        // then write out links to the child chapters
						if ((chapter.html == null) & (node.Nodes.Count > 0))
						{
							sw.WriteLine("<ul>");
							foreach (TreeNode childNode in node.Nodes)
							{
								Chapter childChapter = (Chapter)childNode.Tag;
								sw.WriteLine(String.Format("<li><a href=\"{0}\">{1}</a></li>", childChapter.id + ".xml", childChapter.chapterName.Replace("&", "&amp;")));
							}
							sw.WriteLine("</ul>");
						}
						else
						{
							sw.WriteLine(chapter.html ?? "");
						}
						sw.WriteLine("</body></html>");
					}
					else if (chapter.html.StartsWith("<?xml"))
					{
						sw.Write(chapter.html);
					}
				}

                // No do the children
                if (node.Nodes.Count > 0)
                    WriteChapters(node.Nodes);
            }
        }

        /// <summary>
        /// Begin creating the actual EPUB file
        /// </summary>
        /// <returns></returns>
        private String CreateEpubArchive()
        {
            String EpubFileName = "";

            // Open the save dialog and set the name
            dlgSave.RestoreDirectory = true;
            dlgSave.AddExtension = true;
            dlgSave.OverwritePrompt = true;
            dlgSave.Filter = "ePub files (*.epub)|*.epub";

            String Title = tbxTitle.Text;
            String Author = tbxAuthorSort.Text;
            CultureInfo cultureInfo = System.Threading.Thread.CurrentThread.CurrentCulture;
            TextInfo textInfo = cultureInfo.TextInfo;

            Author = Author.Substring(0, Author.IndexOf(",") < 0 ? Author.Length : Author.IndexOf(",")).Replace(" ", "");
            Title = textInfo.ToTitleCase(Title);
            Title = Author + "_" + Title.Replace(" ", "").Replace(",", "");

            dlgSave.FileName = Title;

            // Is this an autocreate scenario?
            if (!quietOperation)
            {
                if (dlgSave.ShowDialog() == DialogResult.OK)
                {
                    EpubFileName = dlgSave.FileName;
                    SaveEpubFile(EpubFileName);
                }
            }
            else
            {
                String epubLocation = Path.GetDirectoryName(tbxFileName.Text);
                EpubFileName = Path.Combine(epubLocation, Title + ".epub");
                SaveEpubFile(EpubFileName);
            }

            return EpubFileName;
        }

        /// <summary>
        /// Save the EPUB using Ionic zip library
        /// </summary>
        /// <param name="EpubFileName"></param>
        private void SaveEpubFile(String EpubFileName)
        {
            if (File.Exists(EpubFileName))
                File.Delete(EpubFileName);

            using (ZipFile zip = new ZipFile(EpubFileName))
            {
                // MUST save the 'mimetype' file as the first file and non-compressed
                zip.ForceNoCompression = true;
                zip.AddFile(Path.Combine(mainDir, "mimetype"), "");

                // Can compress all other files
                zip.ForceNoCompression = false;
                zip.AddDirectory(metaDir, "META-INF");
                zip.AddDirectory(opsDir, "OPS");

                zip.Save();
                Directory.Delete(mainDir, true);
                if (ddlType.Text == "Html")
                    if (workingFileName.ToLower() != tbxFileName.Text.ToLower())
                        File.Delete(workingFileName);
            }
        }

        /// <summary>
        /// Obfuscate a Library
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnObfuscate_Click(object sender, EventArgs e)
        {
            Program.Obfuscate();
        }

        /// <summary>
        /// Fix accented characters
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnFixAccents_Click(object sender, EventArgs e)
        {
            BuildAccentedChars();
            String contents = File.ReadAllText(tbxFileName.Text, Encoding.Default);

            for (int i = 0; i < accentedChars.Length; i++)
            {
                if (contents.IndexOf(accentedChars[i]) > 0)
                {
                    contents = contents.Replace(accentedChars[i].ToString(), accentedHtml[i]);
                }
            }

            File.WriteAllText(tbxFileName.Text, contents);
        }
    }
}