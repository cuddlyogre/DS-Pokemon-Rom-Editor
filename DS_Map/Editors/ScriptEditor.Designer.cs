
namespace DSPRE.Editors
{
  partial class ScriptEditor
  {
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary> 
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Component Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      this.selectScriptFileComboBox = new System.Windows.Forms.ComboBox();
      this.label5 = new System.Windows.Forms.Label();
      this.scriptEditorTabControl = new System.Windows.Forms.TabControl();
      this.scriptsTabPage = new System.Windows.Forms.TabPage();
      this.PanelSearchScripts = new System.Windows.Forms.Panel();
      this.BtnNextSearchScript = new System.Windows.Forms.Button();
      this.BtnPrevSearchScript = new System.Windows.Forms.Button();
      this.BtnCloseSearchScript = new System.Windows.Forms.Button();
      this.panelSearchScriptTextBox = new System.Windows.Forms.TextBox();
      this.scintillaScriptsPanel = new System.Windows.Forms.Panel();
      this.functionsTabPage = new System.Windows.Forms.TabPage();
      this.PanelSearchFunctions = new System.Windows.Forms.Panel();
      this.BtnNextSearchFunc = new System.Windows.Forms.Button();
      this.BtnPrevSearchFunc = new System.Windows.Forms.Button();
      this.BtnCloseSearchFunc = new System.Windows.Forms.Button();
      this.panelSearchFunctionTextBox = new System.Windows.Forms.TextBox();
      this.scintillaFunctionsPanel = new System.Windows.Forms.Panel();
      this.actionsTabPage = new System.Windows.Forms.TabPage();
      this.PanelSearchActions = new System.Windows.Forms.Panel();
      this.BtnNextSearchActions = new System.Windows.Forms.Button();
      this.BtnPrevSearchActions = new System.Windows.Forms.Button();
      this.BtnCloseSearchActions = new System.Windows.Forms.Button();
      this.panelSearchActionTextBox = new System.Windows.Forms.TextBox();
      this.scintillaActionsPanel = new System.Windows.Forms.Panel();
      this.addScriptFileButton = new System.Windows.Forms.Button();
      this.removeScriptFileButton = new System.Windows.Forms.Button();
      this.saveScriptFileButton = new System.Windows.Forms.Button();
      this.exportScriptFileButton = new System.Windows.Forms.Button();
      this.importScriptFileButton = new System.Windows.Forms.Button();
      this.groupBox8 = new System.Windows.Forms.GroupBox();
      this.searchInScriptsButton = new System.Windows.Forms.Button();
      this.searchOnlyCurrentScriptCheckBox = new System.Windows.Forms.CheckBox();
      this.scrollToBlockStartcheckBox1 = new System.Windows.Forms.CheckBox();
      this.scriptSearchCaseSensitiveCheckBox = new System.Windows.Forms.CheckBox();
      this.searchInScriptsTextBox = new System.Windows.Forms.TextBox();
      this.label31 = new System.Windows.Forms.Label();
      this.searchProgressBar = new System.Windows.Forms.ProgressBar();
      this.label30 = new System.Windows.Forms.Label();
      this.label29 = new System.Windows.Forms.Label();
      this.searchInScriptsResultListBox = new System.Windows.Forms.ListBox();
      this.groupBox24 = new System.Windows.Forms.GroupBox();
      this.SyncNavigatorCB = new System.Windows.Forms.CheckBox();
      this.ScriptNavigatorTabControl = new System.Windows.Forms.TabControl();
      this.ScriptsNavTab = new System.Windows.Forms.TabPage();
      this.scriptsNavListbox = new System.Windows.Forms.ListBox();
      this.FunctionsNavTab = new System.Windows.Forms.TabPage();
      this.functionsNavListbox = new System.Windows.Forms.ListBox();
      this.ActionsNavTab = new System.Windows.Forms.TabPage();
      this.actionsNavListbox = new System.Windows.Forms.ListBox();
      this.openSearchScriptEditorButton = new System.Windows.Forms.Button();
      this.expandScriptTextButton = new System.Windows.Forms.Button();
      this.compressScriptTextButton = new System.Windows.Forms.Button();
      this.scriptEditorWordWrapCheckbox = new System.Windows.Forms.CheckBox();
      this.scriptEditorWhitespacesCheckbox = new System.Windows.Forms.CheckBox();
      this.groupBox26 = new System.Windows.Forms.GroupBox();
      this.scriptEditorNumberFormatNoPreference = new System.Windows.Forms.RadioButton();
      this.scriptEditorNumberFormatDecimal = new System.Windows.Forms.RadioButton();
      this.scriptEditorNumberFormatHex = new System.Windows.Forms.RadioButton();
      this.clearCurrentLevelScriptButton = new System.Windows.Forms.Button();
      this.locateCurrentScriptFile = new System.Windows.Forms.Button();
      this.scriptEditorTabControl.SuspendLayout();
      this.scriptsTabPage.SuspendLayout();
      this.PanelSearchScripts.SuspendLayout();
      this.functionsTabPage.SuspendLayout();
      this.PanelSearchFunctions.SuspendLayout();
      this.actionsTabPage.SuspendLayout();
      this.PanelSearchActions.SuspendLayout();
      this.groupBox8.SuspendLayout();
      this.groupBox24.SuspendLayout();
      this.ScriptNavigatorTabControl.SuspendLayout();
      this.ScriptsNavTab.SuspendLayout();
      this.FunctionsNavTab.SuspendLayout();
      this.ActionsNavTab.SuspendLayout();
      this.groupBox26.SuspendLayout();
      this.SuspendLayout();
      // 
      // selectScriptFileComboBox
      // 
      this.selectScriptFileComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.selectScriptFileComboBox.FormattingEnabled = true;
      this.selectScriptFileComboBox.Location = new System.Drawing.Point(7, 24);
      this.selectScriptFileComboBox.Name = "selectScriptFileComboBox";
      this.selectScriptFileComboBox.Size = new System.Drawing.Size(152, 21);
      this.selectScriptFileComboBox.TabIndex = 0;
      this.selectScriptFileComboBox.SelectedIndexChanged += new System.EventHandler(this.selectScriptFileComboBox_SelectedIndexChanged);
      // 
      // label5
      // 
      this.label5.AutoSize = true;
      this.label5.Location = new System.Drawing.Point(5, 8);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(53, 13);
      this.label5.TabIndex = 1;
      this.label5.Text = "Script File";
      // 
      // scriptEditorTabControl
      // 
      this.scriptEditorTabControl.Controls.Add(this.scriptsTabPage);
      this.scriptEditorTabControl.Controls.Add(this.functionsTabPage);
      this.scriptEditorTabControl.Controls.Add(this.actionsTabPage);
      this.scriptEditorTabControl.Location = new System.Drawing.Point(481, 22);
      this.scriptEditorTabControl.Name = "scriptEditorTabControl";
      this.scriptEditorTabControl.SelectedIndex = 0;
      this.scriptEditorTabControl.Size = new System.Drawing.Size(692, 591);
      this.scriptEditorTabControl.TabIndex = 5;
      this.scriptEditorTabControl.SelectedIndexChanged += new System.EventHandler(this.scriptEditorTabControl_TabIndexChanged);
      // 
      // scriptsTabPage
      // 
      this.scriptsTabPage.Controls.Add(this.PanelSearchScripts);
      this.scriptsTabPage.Controls.Add(this.scintillaScriptsPanel);
      this.scriptsTabPage.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.scriptsTabPage.Location = new System.Drawing.Point(4, 22);
      this.scriptsTabPage.Name = "scriptsTabPage";
      this.scriptsTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.scriptsTabPage.Size = new System.Drawing.Size(684, 565);
      this.scriptsTabPage.TabIndex = 0;
      this.scriptsTabPage.Text = "Scripts";
      this.scriptsTabPage.UseVisualStyleBackColor = true;
      // 
      // PanelSearchScripts
      // 
      this.PanelSearchScripts.BackColor = System.Drawing.Color.White;
      this.PanelSearchScripts.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.PanelSearchScripts.Controls.Add(this.BtnNextSearchScript);
      this.PanelSearchScripts.Controls.Add(this.BtnPrevSearchScript);
      this.PanelSearchScripts.Controls.Add(this.BtnCloseSearchScript);
      this.PanelSearchScripts.Controls.Add(this.panelSearchScriptTextBox);
      this.PanelSearchScripts.Location = new System.Drawing.Point(386, 3);
      this.PanelSearchScripts.Name = "PanelSearchScripts";
      this.PanelSearchScripts.Size = new System.Drawing.Size(292, 40);
      this.PanelSearchScripts.TabIndex = 14;
      this.PanelSearchScripts.Visible = false;
      // 
      // BtnNextSearchScript
      // 
      this.BtnNextSearchScript.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.BtnNextSearchScript.ForeColor = System.Drawing.Color.White;
      this.BtnNextSearchScript.Image = global::DSPRE.Properties.Resources.arrowdown;
      this.BtnNextSearchScript.Location = new System.Drawing.Point(233, 4);
      this.BtnNextSearchScript.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
      this.BtnNextSearchScript.Name = "BtnNextSearchScript";
      this.BtnNextSearchScript.Size = new System.Drawing.Size(25, 30);
      this.BtnNextSearchScript.TabIndex = 9;
      this.BtnNextSearchScript.Tag = "Find next (Enter)";
      this.BtnNextSearchScript.UseVisualStyleBackColor = true;
      this.BtnNextSearchScript.Click += new System.EventHandler(this.BtnNextSearchScript_Click);
      // 
      // BtnPrevSearchScript
      // 
      this.BtnPrevSearchScript.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.BtnPrevSearchScript.ForeColor = System.Drawing.Color.White;
      this.BtnPrevSearchScript.Image = global::DSPRE.Properties.Resources.arrowup;
      this.BtnPrevSearchScript.Location = new System.Drawing.Point(205, 4);
      this.BtnPrevSearchScript.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
      this.BtnPrevSearchScript.Name = "BtnPrevSearchScript";
      this.BtnPrevSearchScript.Size = new System.Drawing.Size(25, 30);
      this.BtnPrevSearchScript.TabIndex = 8;
      this.BtnPrevSearchScript.Tag = "Find previous (Shift+Enter)";
      this.BtnPrevSearchScript.UseVisualStyleBackColor = true;
      this.BtnPrevSearchScript.Click += new System.EventHandler(this.BtnPrevSearchScript_Click);
      // 
      // BtnCloseSearchScript
      // 
      this.BtnCloseSearchScript.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.BtnCloseSearchScript.ForeColor = System.Drawing.Color.White;
      this.BtnCloseSearchScript.Image = global::DSPRE.Properties.Resources.Cross;
      this.BtnCloseSearchScript.Location = new System.Drawing.Point(261, 4);
      this.BtnCloseSearchScript.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
      this.BtnCloseSearchScript.Name = "BtnCloseSearchScript";
      this.BtnCloseSearchScript.Size = new System.Drawing.Size(25, 30);
      this.BtnCloseSearchScript.TabIndex = 7;
      this.BtnCloseSearchScript.Tag = "Close (Esc)";
      this.BtnCloseSearchScript.UseVisualStyleBackColor = true;
      this.BtnCloseSearchScript.Click += new System.EventHandler(this.BtnCloseSearchScript_Click);
      // 
      // panelSearchScriptTextBox
      // 
      this.panelSearchScriptTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
      this.panelSearchScriptTextBox.Font = new System.Drawing.Font("Segoe UI", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.panelSearchScriptTextBox.Location = new System.Drawing.Point(10, 6);
      this.panelSearchScriptTextBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
      this.panelSearchScriptTextBox.Name = "panelSearchScriptTextBox";
      this.panelSearchScriptTextBox.Size = new System.Drawing.Size(189, 25);
      this.panelSearchScriptTextBox.TabIndex = 6;
      this.panelSearchScriptTextBox.TextChanged += new System.EventHandler(this.panelSearchScriptTextBox_TextChanged);
      this.panelSearchScriptTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.scriptTxtSearch_KeyDown);
      // 
      // scintillaScriptsPanel
      // 
      this.scintillaScriptsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
      this.scintillaScriptsPanel.Location = new System.Drawing.Point(3, 3);
      this.scintillaScriptsPanel.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
      this.scintillaScriptsPanel.Name = "scintillaScriptsPanel";
      this.scintillaScriptsPanel.Size = new System.Drawing.Size(678, 559);
      this.scintillaScriptsPanel.TabIndex = 13;
      // 
      // functionsTabPage
      // 
      this.functionsTabPage.Controls.Add(this.PanelSearchFunctions);
      this.functionsTabPage.Controls.Add(this.scintillaFunctionsPanel);
      this.functionsTabPage.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.functionsTabPage.Location = new System.Drawing.Point(4, 22);
      this.functionsTabPage.Name = "functionsTabPage";
      this.functionsTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.functionsTabPage.Size = new System.Drawing.Size(684, 565);
      this.functionsTabPage.TabIndex = 1;
      this.functionsTabPage.Text = "Functions";
      this.functionsTabPage.UseVisualStyleBackColor = true;
      // 
      // PanelSearchFunctions
      // 
      this.PanelSearchFunctions.BackColor = System.Drawing.Color.White;
      this.PanelSearchFunctions.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.PanelSearchFunctions.Controls.Add(this.BtnNextSearchFunc);
      this.PanelSearchFunctions.Controls.Add(this.BtnPrevSearchFunc);
      this.PanelSearchFunctions.Controls.Add(this.BtnCloseSearchFunc);
      this.PanelSearchFunctions.Controls.Add(this.panelSearchFunctionTextBox);
      this.PanelSearchFunctions.Location = new System.Drawing.Point(386, 3);
      this.PanelSearchFunctions.Name = "PanelSearchFunctions";
      this.PanelSearchFunctions.Size = new System.Drawing.Size(292, 40);
      this.PanelSearchFunctions.TabIndex = 16;
      this.PanelSearchFunctions.Visible = false;
      // 
      // BtnNextSearchFunc
      // 
      this.BtnNextSearchFunc.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.BtnNextSearchFunc.ForeColor = System.Drawing.Color.White;
      this.BtnNextSearchFunc.Image = global::DSPRE.Properties.Resources.arrowdown;
      this.BtnNextSearchFunc.Location = new System.Drawing.Point(233, 4);
      this.BtnNextSearchFunc.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
      this.BtnNextSearchFunc.Name = "BtnNextSearchFunc";
      this.BtnNextSearchFunc.Size = new System.Drawing.Size(25, 30);
      this.BtnNextSearchFunc.TabIndex = 9;
      this.BtnNextSearchFunc.Tag = "Find next (Enter)";
      this.BtnNextSearchFunc.UseVisualStyleBackColor = true;
      this.BtnNextSearchFunc.Click += new System.EventHandler(this.BtnNextSearchFunc_Click);
      // 
      // BtnPrevSearchFunc
      // 
      this.BtnPrevSearchFunc.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.BtnPrevSearchFunc.ForeColor = System.Drawing.Color.White;
      this.BtnPrevSearchFunc.Image = global::DSPRE.Properties.Resources.arrowup;
      this.BtnPrevSearchFunc.Location = new System.Drawing.Point(205, 4);
      this.BtnPrevSearchFunc.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
      this.BtnPrevSearchFunc.Name = "BtnPrevSearchFunc";
      this.BtnPrevSearchFunc.Size = new System.Drawing.Size(25, 30);
      this.BtnPrevSearchFunc.TabIndex = 8;
      this.BtnPrevSearchFunc.Tag = "Find previous (Shift+Enter)";
      this.BtnPrevSearchFunc.UseVisualStyleBackColor = true;
      this.BtnPrevSearchFunc.Click += new System.EventHandler(this.BtnPrevSearchFunc_Click);
      // 
      // BtnCloseSearchFunc
      // 
      this.BtnCloseSearchFunc.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.BtnCloseSearchFunc.ForeColor = System.Drawing.Color.White;
      this.BtnCloseSearchFunc.Image = global::DSPRE.Properties.Resources.Cross;
      this.BtnCloseSearchFunc.Location = new System.Drawing.Point(261, 4);
      this.BtnCloseSearchFunc.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
      this.BtnCloseSearchFunc.Name = "BtnCloseSearchFunc";
      this.BtnCloseSearchFunc.Size = new System.Drawing.Size(25, 30);
      this.BtnCloseSearchFunc.TabIndex = 7;
      this.BtnCloseSearchFunc.Tag = "Close (Esc)";
      this.BtnCloseSearchFunc.UseVisualStyleBackColor = true;
      this.BtnCloseSearchFunc.Click += new System.EventHandler(this.BtnCloseSearchFunc_Click);
      // 
      // panelSearchFunctionTextBox
      // 
      this.panelSearchFunctionTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
      this.panelSearchFunctionTextBox.Font = new System.Drawing.Font("Segoe UI", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.panelSearchFunctionTextBox.Location = new System.Drawing.Point(10, 6);
      this.panelSearchFunctionTextBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
      this.panelSearchFunctionTextBox.Name = "panelSearchFunctionTextBox";
      this.panelSearchFunctionTextBox.Size = new System.Drawing.Size(189, 25);
      this.panelSearchFunctionTextBox.TabIndex = 6;
      this.panelSearchFunctionTextBox.TextChanged += new System.EventHandler(this.panelSearchFunctionTextBox_TextChanged);
      this.panelSearchFunctionTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.functionTxtSearch_KeyDown);
      // 
      // scintillaFunctionsPanel
      // 
      this.scintillaFunctionsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
      this.scintillaFunctionsPanel.Location = new System.Drawing.Point(3, 3);
      this.scintillaFunctionsPanel.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
      this.scintillaFunctionsPanel.Name = "scintillaFunctionsPanel";
      this.scintillaFunctionsPanel.Size = new System.Drawing.Size(678, 559);
      this.scintillaFunctionsPanel.TabIndex = 15;
      // 
      // actionsTabPage
      // 
      this.actionsTabPage.Controls.Add(this.PanelSearchActions);
      this.actionsTabPage.Controls.Add(this.scintillaActionsPanel);
      this.actionsTabPage.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.actionsTabPage.Location = new System.Drawing.Point(4, 22);
      this.actionsTabPage.Name = "actionsTabPage";
      this.actionsTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.actionsTabPage.Size = new System.Drawing.Size(684, 565);
      this.actionsTabPage.TabIndex = 2;
      this.actionsTabPage.Text = "Actions";
      this.actionsTabPage.UseVisualStyleBackColor = true;
      // 
      // PanelSearchActions
      // 
      this.PanelSearchActions.BackColor = System.Drawing.Color.White;
      this.PanelSearchActions.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.PanelSearchActions.Controls.Add(this.BtnNextSearchActions);
      this.PanelSearchActions.Controls.Add(this.BtnPrevSearchActions);
      this.PanelSearchActions.Controls.Add(this.BtnCloseSearchActions);
      this.PanelSearchActions.Controls.Add(this.panelSearchActionTextBox);
      this.PanelSearchActions.Location = new System.Drawing.Point(386, 3);
      this.PanelSearchActions.Name = "PanelSearchActions";
      this.PanelSearchActions.Size = new System.Drawing.Size(292, 40);
      this.PanelSearchActions.TabIndex = 16;
      this.PanelSearchActions.Visible = false;
      // 
      // BtnNextSearchActions
      // 
      this.BtnNextSearchActions.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.BtnNextSearchActions.ForeColor = System.Drawing.Color.White;
      this.BtnNextSearchActions.Image = global::DSPRE.Properties.Resources.arrowdown;
      this.BtnNextSearchActions.Location = new System.Drawing.Point(233, 4);
      this.BtnNextSearchActions.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
      this.BtnNextSearchActions.Name = "BtnNextSearchActions";
      this.BtnNextSearchActions.Size = new System.Drawing.Size(25, 30);
      this.BtnNextSearchActions.TabIndex = 9;
      this.BtnNextSearchActions.Tag = "Find next (Enter)";
      this.BtnNextSearchActions.UseVisualStyleBackColor = true;
      this.BtnNextSearchActions.Click += new System.EventHandler(this.BtnNextSearchActions_Click);
      // 
      // BtnPrevSearchActions
      // 
      this.BtnPrevSearchActions.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.BtnPrevSearchActions.ForeColor = System.Drawing.Color.White;
      this.BtnPrevSearchActions.Image = global::DSPRE.Properties.Resources.arrowup;
      this.BtnPrevSearchActions.Location = new System.Drawing.Point(205, 4);
      this.BtnPrevSearchActions.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
      this.BtnPrevSearchActions.Name = "BtnPrevSearchActions";
      this.BtnPrevSearchActions.Size = new System.Drawing.Size(25, 30);
      this.BtnPrevSearchActions.TabIndex = 8;
      this.BtnPrevSearchActions.Tag = "Find previous (Shift+Enter)";
      this.BtnPrevSearchActions.UseVisualStyleBackColor = true;
      this.BtnPrevSearchActions.Click += new System.EventHandler(this.BtnPrevSearchActions_Click);
      // 
      // BtnCloseSearchActions
      // 
      this.BtnCloseSearchActions.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.BtnCloseSearchActions.ForeColor = System.Drawing.Color.White;
      this.BtnCloseSearchActions.Image = global::DSPRE.Properties.Resources.Cross;
      this.BtnCloseSearchActions.Location = new System.Drawing.Point(261, 4);
      this.BtnCloseSearchActions.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
      this.BtnCloseSearchActions.Name = "BtnCloseSearchActions";
      this.BtnCloseSearchActions.Size = new System.Drawing.Size(25, 30);
      this.BtnCloseSearchActions.TabIndex = 7;
      this.BtnCloseSearchActions.Tag = "Close (Esc)";
      this.BtnCloseSearchActions.UseVisualStyleBackColor = true;
      this.BtnCloseSearchActions.Click += new System.EventHandler(this.BtnCloseSearchActions_Click);
      // 
      // panelSearchActionTextBox
      // 
      this.panelSearchActionTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
      this.panelSearchActionTextBox.Font = new System.Drawing.Font("Segoe UI", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.panelSearchActionTextBox.Location = new System.Drawing.Point(10, 6);
      this.panelSearchActionTextBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
      this.panelSearchActionTextBox.Name = "panelSearchActionTextBox";
      this.panelSearchActionTextBox.Size = new System.Drawing.Size(189, 25);
      this.panelSearchActionTextBox.TabIndex = 6;
      this.panelSearchActionTextBox.TextChanged += new System.EventHandler(this.panelSearchActionTextBox_TextChanged);
      this.panelSearchActionTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.actiontTxtSearch_KeyDown);
      // 
      // scintillaActionsPanel
      // 
      this.scintillaActionsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
      this.scintillaActionsPanel.Location = new System.Drawing.Point(3, 3);
      this.scintillaActionsPanel.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
      this.scintillaActionsPanel.Name = "scintillaActionsPanel";
      this.scintillaActionsPanel.Size = new System.Drawing.Size(678, 559);
      this.scintillaActionsPanel.TabIndex = 15;
      // 
      // addScriptFileButton
      // 
      this.addScriptFileButton.Image = global::DSPRE.Properties.Resources.addIcon;
      this.addScriptFileButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.addScriptFileButton.Location = new System.Drawing.Point(314, 22);
      this.addScriptFileButton.Name = "addScriptFileButton";
      this.addScriptFileButton.Size = new System.Drawing.Size(106, 25);
      this.addScriptFileButton.TabIndex = 10;
      this.addScriptFileButton.Text = "Add Script File";
      this.addScriptFileButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.addScriptFileButton.UseVisualStyleBackColor = true;
      this.addScriptFileButton.Click += new System.EventHandler(this.addScriptFileButton_Click);
      // 
      // removeScriptFileButton
      // 
      this.removeScriptFileButton.Image = global::DSPRE.Properties.Resources.deleteIcon;
      this.removeScriptFileButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.removeScriptFileButton.Location = new System.Drawing.Point(314, 49);
      this.removeScriptFileButton.Name = "removeScriptFileButton";
      this.removeScriptFileButton.Size = new System.Drawing.Size(106, 25);
      this.removeScriptFileButton.TabIndex = 13;
      this.removeScriptFileButton.Text = "Remove Last";
      this.removeScriptFileButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.removeScriptFileButton.UseVisualStyleBackColor = true;
      this.removeScriptFileButton.Click += new System.EventHandler(this.removeScriptFileButton_Click);
      // 
      // saveScriptFileButton
      // 
      this.saveScriptFileButton.Image = global::DSPRE.Properties.Resources.saveButton;
      this.saveScriptFileButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.saveScriptFileButton.Location = new System.Drawing.Point(6, 50);
      this.saveScriptFileButton.Name = "saveScriptFileButton";
      this.saveScriptFileButton.Size = new System.Drawing.Size(154, 23);
      this.saveScriptFileButton.TabIndex = 14;
      this.saveScriptFileButton.Text = "&Save Current File";
      this.saveScriptFileButton.UseVisualStyleBackColor = true;
      this.saveScriptFileButton.Click += new System.EventHandler(this.saveScriptFileButton_Click);
      // 
      // exportScriptFileButton
      // 
      this.exportScriptFileButton.Image = global::DSPRE.Properties.Resources.exportArrow;
      this.exportScriptFileButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.exportScriptFileButton.Location = new System.Drawing.Point(239, 22);
      this.exportScriptFileButton.Name = "exportScriptFileButton";
      this.exportScriptFileButton.Size = new System.Drawing.Size(70, 52);
      this.exportScriptFileButton.TabIndex = 15;
      this.exportScriptFileButton.Text = "&Export \r\nFile";
      this.exportScriptFileButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.exportScriptFileButton.UseVisualStyleBackColor = true;
      this.exportScriptFileButton.Click += new System.EventHandler(this.exportScriptFileButton_Click);
      // 
      // importScriptFileButton
      // 
      this.importScriptFileButton.Image = global::DSPRE.Properties.Resources.importArrow;
      this.importScriptFileButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.importScriptFileButton.Location = new System.Drawing.Point(164, 22);
      this.importScriptFileButton.Name = "importScriptFileButton";
      this.importScriptFileButton.Size = new System.Drawing.Size(70, 52);
      this.importScriptFileButton.TabIndex = 16;
      this.importScriptFileButton.Text = "&Import\r\nFile";
      this.importScriptFileButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.importScriptFileButton.UseVisualStyleBackColor = true;
      this.importScriptFileButton.Click += new System.EventHandler(this.importScriptFileButton_Click);
      // 
      // groupBox8
      // 
      this.groupBox8.Controls.Add(this.searchInScriptsButton);
      this.groupBox8.Controls.Add(this.searchOnlyCurrentScriptCheckBox);
      this.groupBox8.Controls.Add(this.scrollToBlockStartcheckBox1);
      this.groupBox8.Controls.Add(this.scriptSearchCaseSensitiveCheckBox);
      this.groupBox8.Controls.Add(this.searchInScriptsTextBox);
      this.groupBox8.Controls.Add(this.label31);
      this.groupBox8.Controls.Add(this.searchProgressBar);
      this.groupBox8.Controls.Add(this.label30);
      this.groupBox8.Controls.Add(this.label29);
      this.groupBox8.Controls.Add(this.searchInScriptsResultListBox);
      this.groupBox8.Location = new System.Drawing.Point(2, 315);
      this.groupBox8.Name = "groupBox8";
      this.groupBox8.Size = new System.Drawing.Size(472, 298);
      this.groupBox8.TabIndex = 18;
      this.groupBox8.TabStop = false;
      this.groupBox8.Text = "Search for commands:";
      // 
      // searchInScriptsButton
      // 
      this.searchInScriptsButton.Image = global::DSPRE.Properties.Resources.SearchMiniIcon;
      this.searchInScriptsButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.searchInScriptsButton.Location = new System.Drawing.Point(375, 26);
      this.searchInScriptsButton.Name = "searchInScriptsButton";
      this.searchInScriptsButton.Size = new System.Drawing.Size(91, 36);
      this.searchInScriptsButton.TabIndex = 32;
      this.searchInScriptsButton.Text = "Search";
      this.searchInScriptsButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.searchInScriptsButton.UseVisualStyleBackColor = true;
      this.searchInScriptsButton.Click += new System.EventHandler(this.searchInScriptsButton_Click);
      // 
      // searchOnlyCurrentScriptCheckBox
      // 
      this.searchOnlyCurrentScriptCheckBox.AutoSize = true;
      this.searchOnlyCurrentScriptCheckBox.Location = new System.Drawing.Point(74, 61);
      this.searchOnlyCurrentScriptCheckBox.Name = "searchOnlyCurrentScriptCheckBox";
      this.searchOnlyCurrentScriptCheckBox.Size = new System.Drawing.Size(84, 17);
      this.searchOnlyCurrentScriptCheckBox.TabIndex = 41;
      this.searchOnlyCurrentScriptCheckBox.Text = "Only Current";
      this.searchOnlyCurrentScriptCheckBox.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      this.searchOnlyCurrentScriptCheckBox.UseVisualStyleBackColor = true;
      // 
      // scrollToBlockStartcheckBox1
      // 
      this.scrollToBlockStartcheckBox1.AutoSize = true;
      this.scrollToBlockStartcheckBox1.Location = new System.Drawing.Point(253, 61);
      this.scrollToBlockStartcheckBox1.Name = "scrollToBlockStartcheckBox1";
      this.scrollToBlockStartcheckBox1.Size = new System.Drawing.Size(116, 17);
      this.scrollToBlockStartcheckBox1.TabIndex = 39;
      this.scrollToBlockStartcheckBox1.Text = "Scroll to block start";
      this.scrollToBlockStartcheckBox1.UseVisualStyleBackColor = true;
      // 
      // scriptSearchCaseSensitiveCheckBox
      // 
      this.scriptSearchCaseSensitiveCheckBox.AutoSize = true;
      this.scriptSearchCaseSensitiveCheckBox.Location = new System.Drawing.Point(164, 61);
      this.scriptSearchCaseSensitiveCheckBox.Name = "scriptSearchCaseSensitiveCheckBox";
      this.scriptSearchCaseSensitiveCheckBox.Size = new System.Drawing.Size(83, 17);
      this.scriptSearchCaseSensitiveCheckBox.TabIndex = 39;
      this.scriptSearchCaseSensitiveCheckBox.Text = "Match Case";
      this.scriptSearchCaseSensitiveCheckBox.UseVisualStyleBackColor = true;
      // 
      // searchInScriptsTextBox
      // 
      this.searchInScriptsTextBox.Location = new System.Drawing.Point(11, 35);
      this.searchInScriptsTextBox.Name = "searchInScriptsTextBox";
      this.searchInScriptsTextBox.Size = new System.Drawing.Size(358, 20);
      this.searchInScriptsTextBox.TabIndex = 38;
      this.searchInScriptsTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.searchInScriptsTextBox_KeyDown);
      // 
      // label31
      // 
      this.label31.AutoSize = true;
      this.label31.Location = new System.Drawing.Point(9, 256);
      this.label31.Name = "label31";
      this.label31.Size = new System.Drawing.Size(48, 13);
      this.label31.TabIndex = 37;
      this.label31.Text = "Progress";
      // 
      // searchProgressBar
      // 
      this.searchProgressBar.Location = new System.Drawing.Point(11, 272);
      this.searchProgressBar.Name = "searchProgressBar";
      this.searchProgressBar.Size = new System.Drawing.Size(452, 19);
      this.searchProgressBar.TabIndex = 36;
      // 
      // label30
      // 
      this.label30.AutoSize = true;
      this.label30.Location = new System.Drawing.Point(11, 64);
      this.label30.Name = "label30";
      this.label30.Size = new System.Drawing.Size(42, 13);
      this.label30.TabIndex = 35;
      this.label30.Text = "Results";
      // 
      // label29
      // 
      this.label29.AutoSize = true;
      this.label29.Location = new System.Drawing.Point(8, 18);
      this.label29.Name = "label29";
      this.label29.Size = new System.Drawing.Size(77, 13);
      this.label29.TabIndex = 33;
      this.label29.Text = "Line to search:";
      // 
      // searchInScriptsResultListBox
      // 
      this.searchInScriptsResultListBox.Location = new System.Drawing.Point(11, 80);
      this.searchInScriptsResultListBox.Name = "searchInScriptsResultListBox";
      this.searchInScriptsResultListBox.Size = new System.Drawing.Size(452, 173);
      this.searchInScriptsResultListBox.TabIndex = 17;
      this.searchInScriptsResultListBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.searchInScriptsResultListBox_KeyDown);
      this.searchInScriptsResultListBox.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.searchInScripts_GoToEntryResult);
      // 
      // groupBox24
      // 
      this.groupBox24.Controls.Add(this.SyncNavigatorCB);
      this.groupBox24.Controls.Add(this.ScriptNavigatorTabControl);
      this.groupBox24.Location = new System.Drawing.Point(3, 75);
      this.groupBox24.Name = "groupBox24";
      this.groupBox24.Size = new System.Drawing.Size(472, 234);
      this.groupBox24.TabIndex = 42;
      this.groupBox24.TabStop = false;
      this.groupBox24.Text = "Navigator";
      // 
      // SyncNavigatorCB
      // 
      this.SyncNavigatorCB.Appearance = System.Windows.Forms.Appearance.Button;
      this.SyncNavigatorCB.AutoSize = true;
      this.SyncNavigatorCB.Checked = true;
      this.SyncNavigatorCB.CheckState = System.Windows.Forms.CheckState.Checked;
      this.SyncNavigatorCB.Location = new System.Drawing.Point(417, 11);
      this.SyncNavigatorCB.Name = "SyncNavigatorCB";
      this.SyncNavigatorCB.Size = new System.Drawing.Size(41, 23);
      this.SyncNavigatorCB.TabIndex = 43;
      this.SyncNavigatorCB.Text = "Sync";
      this.SyncNavigatorCB.UseVisualStyleBackColor = true;
      // 
      // ScriptNavigatorTabControl
      // 
      this.ScriptNavigatorTabControl.Controls.Add(this.ScriptsNavTab);
      this.ScriptNavigatorTabControl.Controls.Add(this.FunctionsNavTab);
      this.ScriptNavigatorTabControl.Controls.Add(this.ActionsNavTab);
      this.ScriptNavigatorTabControl.Location = new System.Drawing.Point(6, 16);
      this.ScriptNavigatorTabControl.Name = "ScriptNavigatorTabControl";
      this.ScriptNavigatorTabControl.SelectedIndex = 0;
      this.ScriptNavigatorTabControl.Size = new System.Drawing.Size(456, 209);
      this.ScriptNavigatorTabControl.TabIndex = 18;
      // 
      // ScriptsNavTab
      // 
      this.ScriptsNavTab.Controls.Add(this.scriptsNavListbox);
      this.ScriptsNavTab.Location = new System.Drawing.Point(4, 22);
      this.ScriptsNavTab.Name = "ScriptsNavTab";
      this.ScriptsNavTab.Padding = new System.Windows.Forms.Padding(3);
      this.ScriptsNavTab.Size = new System.Drawing.Size(448, 183);
      this.ScriptsNavTab.TabIndex = 0;
      this.ScriptsNavTab.Text = "Scripts";
      this.ScriptsNavTab.UseVisualStyleBackColor = true;
      // 
      // scriptsNavListbox
      // 
      this.scriptsNavListbox.Dock = System.Windows.Forms.DockStyle.Fill;
      this.scriptsNavListbox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.scriptsNavListbox.ItemHeight = 15;
      this.scriptsNavListbox.Location = new System.Drawing.Point(3, 3);
      this.scriptsNavListbox.Name = "scriptsNavListbox";
      this.scriptsNavListbox.Size = new System.Drawing.Size(442, 177);
      this.scriptsNavListbox.TabIndex = 18;
      this.scriptsNavListbox.SelectedIndexChanged += new System.EventHandler(this.scriptsNavListbox_SelectedIndexChanged);
      // 
      // FunctionsNavTab
      // 
      this.FunctionsNavTab.Controls.Add(this.functionsNavListbox);
      this.FunctionsNavTab.Location = new System.Drawing.Point(4, 22);
      this.FunctionsNavTab.Name = "FunctionsNavTab";
      this.FunctionsNavTab.Padding = new System.Windows.Forms.Padding(3);
      this.FunctionsNavTab.Size = new System.Drawing.Size(448, 183);
      this.FunctionsNavTab.TabIndex = 1;
      this.FunctionsNavTab.Text = "Functions";
      this.FunctionsNavTab.UseVisualStyleBackColor = true;
      // 
      // functionsNavListbox
      // 
      this.functionsNavListbox.Dock = System.Windows.Forms.DockStyle.Fill;
      this.functionsNavListbox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.functionsNavListbox.ItemHeight = 15;
      this.functionsNavListbox.Location = new System.Drawing.Point(3, 3);
      this.functionsNavListbox.Name = "functionsNavListbox";
      this.functionsNavListbox.Size = new System.Drawing.Size(442, 177);
      this.functionsNavListbox.TabIndex = 19;
      this.functionsNavListbox.SelectedIndexChanged += new System.EventHandler(this.functionsNavListbox_SelectedIndexChanged);
      // 
      // ActionsNavTab
      // 
      this.ActionsNavTab.Controls.Add(this.actionsNavListbox);
      this.ActionsNavTab.Location = new System.Drawing.Point(4, 22);
      this.ActionsNavTab.Name = "ActionsNavTab";
      this.ActionsNavTab.Padding = new System.Windows.Forms.Padding(3);
      this.ActionsNavTab.Size = new System.Drawing.Size(448, 183);
      this.ActionsNavTab.TabIndex = 2;
      this.ActionsNavTab.Text = "Actions";
      this.ActionsNavTab.UseVisualStyleBackColor = true;
      // 
      // actionsNavListbox
      // 
      this.actionsNavListbox.Dock = System.Windows.Forms.DockStyle.Fill;
      this.actionsNavListbox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.actionsNavListbox.ItemHeight = 15;
      this.actionsNavListbox.Location = new System.Drawing.Point(3, 3);
      this.actionsNavListbox.Name = "actionsNavListbox";
      this.actionsNavListbox.Size = new System.Drawing.Size(442, 177);
      this.actionsNavListbox.TabIndex = 19;
      this.actionsNavListbox.SelectedIndexChanged += new System.EventHandler(this.actionsNavListbox_SelectedIndexChanged);
      // 
      // openSearchScriptEditorButton
      // 
      this.openSearchScriptEditorButton.Image = global::DSPRE.Properties.Resources.SearchMiniIcon;
      this.openSearchScriptEditorButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.openSearchScriptEditorButton.Location = new System.Drawing.Point(1079, 11);
      this.openSearchScriptEditorButton.Name = "openSearchScriptEditorButton";
      this.openSearchScriptEditorButton.Size = new System.Drawing.Size(24, 24);
      this.openSearchScriptEditorButton.TabIndex = 47;
      this.openSearchScriptEditorButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.openSearchScriptEditorButton.UseVisualStyleBackColor = true;
      this.openSearchScriptEditorButton.Click += new System.EventHandler(this.openSearchScriptEditorButton_Click);
      // 
      // expandScriptTextButton
      // 
      this.expandScriptTextButton.Image = global::DSPRE.Properties.Resources.expandArrow;
      this.expandScriptTextButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.expandScriptTextButton.Location = new System.Drawing.Point(1107, 11);
      this.expandScriptTextButton.Name = "expandScriptTextButton";
      this.expandScriptTextButton.Size = new System.Drawing.Size(24, 24);
      this.expandScriptTextButton.TabIndex = 48;
      this.expandScriptTextButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.expandScriptTextButton.UseVisualStyleBackColor = true;
      this.expandScriptTextButton.Click += new System.EventHandler(this.ScriptEditorExpandButton_Click);
      // 
      // compressScriptTextButton
      // 
      this.compressScriptTextButton.Image = global::DSPRE.Properties.Resources.compressArrow;
      this.compressScriptTextButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.compressScriptTextButton.Location = new System.Drawing.Point(1135, 11);
      this.compressScriptTextButton.Name = "compressScriptTextButton";
      this.compressScriptTextButton.Size = new System.Drawing.Size(24, 24);
      this.compressScriptTextButton.TabIndex = 49;
      this.compressScriptTextButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.compressScriptTextButton.UseVisualStyleBackColor = true;
      this.compressScriptTextButton.Click += new System.EventHandler(this.ScriptEditorCollapseButton_Click);
      // 
      // scriptEditorWordWrapCheckbox
      // 
      this.scriptEditorWordWrapCheckbox.Appearance = System.Windows.Forms.Appearance.Button;
      this.scriptEditorWordWrapCheckbox.AutoSize = true;
      this.scriptEditorWordWrapCheckbox.Checked = true;
      this.scriptEditorWordWrapCheckbox.CheckState = System.Windows.Forms.CheckState.Checked;
      this.scriptEditorWordWrapCheckbox.Location = new System.Drawing.Point(907, 12);
      this.scriptEditorWordWrapCheckbox.Name = "scriptEditorWordWrapCheckbox";
      this.scriptEditorWordWrapCheckbox.Size = new System.Drawing.Size(72, 23);
      this.scriptEditorWordWrapCheckbox.TabIndex = 43;
      this.scriptEditorWordWrapCheckbox.Text = "Word Wrap";
      this.scriptEditorWordWrapCheckbox.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      this.scriptEditorWordWrapCheckbox.UseVisualStyleBackColor = true;
      this.scriptEditorWordWrapCheckbox.CheckedChanged += new System.EventHandler(this.scriptEditorWordWrapCheckbox_CheckedChanged);
      // 
      // scriptEditorWhitespacesCheckbox
      // 
      this.scriptEditorWhitespacesCheckbox.Appearance = System.Windows.Forms.Appearance.Button;
      this.scriptEditorWhitespacesCheckbox.AutoSize = true;
      this.scriptEditorWhitespacesCheckbox.Location = new System.Drawing.Point(981, 12);
      this.scriptEditorWhitespacesCheckbox.Name = "scriptEditorWhitespacesCheckbox";
      this.scriptEditorWhitespacesCheckbox.Size = new System.Drawing.Size(79, 23);
      this.scriptEditorWhitespacesCheckbox.TabIndex = 46;
      this.scriptEditorWhitespacesCheckbox.Text = "Whitespaces";
      this.scriptEditorWhitespacesCheckbox.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      this.scriptEditorWhitespacesCheckbox.UseVisualStyleBackColor = true;
      this.scriptEditorWhitespacesCheckbox.CheckedChanged += new System.EventHandler(this.viewWhiteSpacesButton_Click);
      // 
      // groupBox26
      // 
      this.groupBox26.Controls.Add(this.scriptEditorNumberFormatNoPreference);
      this.groupBox26.Controls.Add(this.scriptEditorNumberFormatDecimal);
      this.groupBox26.Controls.Add(this.scriptEditorNumberFormatHex);
      this.groupBox26.Location = new System.Drawing.Point(700, 4);
      this.groupBox26.Name = "groupBox26";
      this.groupBox26.Size = new System.Drawing.Size(190, 36);
      this.groupBox26.TabIndex = 50;
      this.groupBox26.TabStop = false;
      this.groupBox26.Text = "Number Format Preference";
      // 
      // scriptEditorNumberFormatNoPreference
      // 
      this.scriptEditorNumberFormatNoPreference.AutoSize = true;
      this.scriptEditorNumberFormatNoPreference.Checked = true;
      this.scriptEditorNumberFormatNoPreference.Location = new System.Drawing.Point(11, 14);
      this.scriptEditorNumberFormatNoPreference.Name = "scriptEditorNumberFormatNoPreference";
      this.scriptEditorNumberFormatNoPreference.Size = new System.Drawing.Size(47, 17);
      this.scriptEditorNumberFormatNoPreference.TabIndex = 36;
      this.scriptEditorNumberFormatNoPreference.TabStop = true;
      this.scriptEditorNumberFormatNoPreference.Text = "Auto";
      this.scriptEditorNumberFormatNoPreference.UseVisualStyleBackColor = true;
      this.scriptEditorNumberFormatNoPreference.CheckedChanged += new System.EventHandler(this.UpdateScriptNumberFormatNoPref);
      // 
      // scriptEditorNumberFormatDecimal
      // 
      this.scriptEditorNumberFormatDecimal.AutoSize = true;
      this.scriptEditorNumberFormatDecimal.Location = new System.Drawing.Point(121, 14);
      this.scriptEditorNumberFormatDecimal.Name = "scriptEditorNumberFormatDecimal";
      this.scriptEditorNumberFormatDecimal.Size = new System.Drawing.Size(63, 17);
      this.scriptEditorNumberFormatDecimal.TabIndex = 35;
      this.scriptEditorNumberFormatDecimal.Text = "Decimal";
      this.scriptEditorNumberFormatDecimal.UseVisualStyleBackColor = true;
      this.scriptEditorNumberFormatDecimal.CheckedChanged += new System.EventHandler(this.UpdateScriptNumberFormatDec);
      // 
      // scriptEditorNumberFormatHex
      // 
      this.scriptEditorNumberFormatHex.AutoSize = true;
      this.scriptEditorNumberFormatHex.Location = new System.Drawing.Point(68, 14);
      this.scriptEditorNumberFormatHex.Name = "scriptEditorNumberFormatHex";
      this.scriptEditorNumberFormatHex.Size = new System.Drawing.Size(44, 17);
      this.scriptEditorNumberFormatHex.TabIndex = 34;
      this.scriptEditorNumberFormatHex.Text = "Hex";
      this.scriptEditorNumberFormatHex.UseVisualStyleBackColor = true;
      this.scriptEditorNumberFormatHex.CheckedChanged += new System.EventHandler(this.UpdateScriptNumberFormatHex);
      // 
      // clearCurrentLevelScriptButton
      // 
      this.clearCurrentLevelScriptButton.Image = global::DSPRE.Properties.Resources.destroyLevelScript;
      this.clearCurrentLevelScriptButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.clearCurrentLevelScriptButton.Location = new System.Drawing.Point(314, 22);
      this.clearCurrentLevelScriptButton.Name = "clearCurrentLevelScriptButton";
      this.clearCurrentLevelScriptButton.Size = new System.Drawing.Size(106, 52);
      this.clearCurrentLevelScriptButton.TabIndex = 19;
      this.clearCurrentLevelScriptButton.Text = "&Clear This\r\nLevel Script";
      this.clearCurrentLevelScriptButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.clearCurrentLevelScriptButton.UseVisualStyleBackColor = true;
      this.clearCurrentLevelScriptButton.Visible = false;
      this.clearCurrentLevelScriptButton.Click += new System.EventHandler(this.clearCurrentLevelScriptButton_Click);
      // 
      // locateCurrentScriptFile
      // 
      this.locateCurrentScriptFile.Image = global::DSPRE.Properties.Resources.open_file;
      this.locateCurrentScriptFile.Location = new System.Drawing.Point(423, 29);
      this.locateCurrentScriptFile.Name = "locateCurrentScriptFile";
      this.locateCurrentScriptFile.Size = new System.Drawing.Size(42, 40);
      this.locateCurrentScriptFile.TabIndex = 63;
      this.locateCurrentScriptFile.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.locateCurrentScriptFile.UseVisualStyleBackColor = true;
      this.locateCurrentScriptFile.Click += new System.EventHandler(this.locateCurrentScriptFile_Click);
      // 
      // ScriptEditor
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.locateCurrentScriptFile);
      this.Controls.Add(this.groupBox24);
      this.Controls.Add(this.clearCurrentLevelScriptButton);
      this.Controls.Add(this.selectScriptFileComboBox);
      this.Controls.Add(this.groupBox26);
      this.Controls.Add(this.label5);
      this.Controls.Add(this.scriptEditorWhitespacesCheckbox);
      this.Controls.Add(this.scriptEditorWordWrapCheckbox);
      this.Controls.Add(this.addScriptFileButton);
      this.Controls.Add(this.compressScriptTextButton);
      this.Controls.Add(this.removeScriptFileButton);
      this.Controls.Add(this.expandScriptTextButton);
      this.Controls.Add(this.saveScriptFileButton);
      this.Controls.Add(this.openSearchScriptEditorButton);
      this.Controls.Add(this.exportScriptFileButton);
      this.Controls.Add(this.importScriptFileButton);
      this.Controls.Add(this.groupBox8);
      this.Controls.Add(this.scriptEditorTabControl);
      this.Name = "ScriptEditor";
      this.Size = new System.Drawing.Size(1177, 616);
      this.scriptEditorTabControl.ResumeLayout(false);
      this.scriptsTabPage.ResumeLayout(false);
      this.PanelSearchScripts.ResumeLayout(false);
      this.PanelSearchScripts.PerformLayout();
      this.functionsTabPage.ResumeLayout(false);
      this.PanelSearchFunctions.ResumeLayout(false);
      this.PanelSearchFunctions.PerformLayout();
      this.actionsTabPage.ResumeLayout(false);
      this.PanelSearchActions.ResumeLayout(false);
      this.PanelSearchActions.PerformLayout();
      this.groupBox8.ResumeLayout(false);
      this.groupBox8.PerformLayout();
      this.groupBox24.ResumeLayout(false);
      this.groupBox24.PerformLayout();
      this.ScriptNavigatorTabControl.ResumeLayout(false);
      this.ScriptsNavTab.ResumeLayout(false);
      this.FunctionsNavTab.ResumeLayout(false);
      this.ActionsNavTab.ResumeLayout(false);
      this.groupBox26.ResumeLayout(false);
      this.groupBox26.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();
    }

    private System.Windows.Forms.CheckBox scrollToBlockStartcheckBox1;

    #endregion

    public System.Windows.Forms.ComboBox selectScriptFileComboBox;
    private System.Windows.Forms.Label label5;
    public System.Windows.Forms.TabControl scriptEditorTabControl;
    private System.Windows.Forms.TabPage scriptsTabPage;
    private System.Windows.Forms.Panel PanelSearchScripts;
    private System.Windows.Forms.Button BtnNextSearchScript;
    private System.Windows.Forms.Button BtnPrevSearchScript;
    private System.Windows.Forms.Button BtnCloseSearchScript;
    private System.Windows.Forms.TextBox panelSearchScriptTextBox;
    private System.Windows.Forms.Panel scintillaScriptsPanel;
    private System.Windows.Forms.TabPage functionsTabPage;
    private System.Windows.Forms.Panel PanelSearchFunctions;
    private System.Windows.Forms.Button BtnNextSearchFunc;
    private System.Windows.Forms.Button BtnPrevSearchFunc;
    private System.Windows.Forms.Button BtnCloseSearchFunc;
    private System.Windows.Forms.TextBox panelSearchFunctionTextBox;
    private System.Windows.Forms.Panel scintillaFunctionsPanel;
    private System.Windows.Forms.TabPage actionsTabPage;
    private System.Windows.Forms.Panel PanelSearchActions;
    private System.Windows.Forms.Button BtnNextSearchActions;
    private System.Windows.Forms.Button BtnPrevSearchActions;
    private System.Windows.Forms.Button BtnCloseSearchActions;
    private System.Windows.Forms.TextBox panelSearchActionTextBox;
    private System.Windows.Forms.Panel scintillaActionsPanel;
    private System.Windows.Forms.Button addScriptFileButton;
    private System.Windows.Forms.Button removeScriptFileButton;
    private System.Windows.Forms.Button saveScriptFileButton;
    private System.Windows.Forms.Button exportScriptFileButton;
    private System.Windows.Forms.Button importScriptFileButton;
    private System.Windows.Forms.GroupBox groupBox8;
    private System.Windows.Forms.CheckBox searchOnlyCurrentScriptCheckBox;
    private System.Windows.Forms.CheckBox scriptSearchCaseSensitiveCheckBox;
    private System.Windows.Forms.TextBox searchInScriptsTextBox;
    private System.Windows.Forms.Label label31;
    private System.Windows.Forms.ProgressBar searchProgressBar;
    private System.Windows.Forms.Label label30;
    private System.Windows.Forms.Label label29;
    private System.Windows.Forms.Button searchInScriptsButton;
    private System.Windows.Forms.ListBox searchInScriptsResultListBox;
    private System.Windows.Forms.GroupBox groupBox24;
    private System.Windows.Forms.CheckBox SyncNavigatorCB;
    private System.Windows.Forms.TabControl ScriptNavigatorTabControl;
    private System.Windows.Forms.TabPage ScriptsNavTab;
    private System.Windows.Forms.ListBox scriptsNavListbox;
    private System.Windows.Forms.TabPage FunctionsNavTab;
    private System.Windows.Forms.ListBox functionsNavListbox;
    private System.Windows.Forms.TabPage ActionsNavTab;
    private System.Windows.Forms.ListBox actionsNavListbox;
    private System.Windows.Forms.Button openSearchScriptEditorButton;
    private System.Windows.Forms.Button expandScriptTextButton;
    private System.Windows.Forms.Button compressScriptTextButton;
    private System.Windows.Forms.CheckBox scriptEditorWordWrapCheckbox;
    private System.Windows.Forms.CheckBox scriptEditorWhitespacesCheckbox;
    private System.Windows.Forms.GroupBox groupBox26;
    private System.Windows.Forms.RadioButton scriptEditorNumberFormatNoPreference;
    private System.Windows.Forms.RadioButton scriptEditorNumberFormatDecimal;
    private System.Windows.Forms.RadioButton scriptEditorNumberFormatHex;
    private System.Windows.Forms.Button clearCurrentLevelScriptButton;
    private System.Windows.Forms.Button locateCurrentScriptFile;
  }
}
