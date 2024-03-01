namespace WebSocketExample.Dialogs
{
    partial class EditPropertiesDlg
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.txtJson = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnMoveDown = new System.Windows.Forms.Button();
            this.btnMoveUp = new System.Windows.Forms.Button();
            this.splitButton1 = new SplitButtonDemo.SplitButton();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.installOSToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.uploadFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.telnetCommandsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.registryIngestToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rebootToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnRemoveAction = new System.Windows.Forms.Button();
            this.lvwSteps = new System.Windows.Forms.ListView();
            this.btnUpdateReference = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtJson
            // 
            this.txtJson.BackColor = System.Drawing.Color.White;
            this.txtJson.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtJson.Location = new System.Drawing.Point(430, 0);
            this.txtJson.Multiline = true;
            this.txtJson.Name = "txtJson";
            this.txtJson.ReadOnly = true;
            this.txtJson.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtJson.Size = new System.Drawing.Size(480, 476);
            this.txtJson.TabIndex = 0;
            this.txtJson.WordWrap = false;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.lvwSteps);
            this.panel1.Controls.Add(this.propertyGrid1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(430, 476);
            this.panel1.TabIndex = 3;
            // 
            // propertyGrid1
            // 
            this.propertyGrid1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.propertyGrid1.LineColor = System.Drawing.SystemColors.ControlDark;
            this.propertyGrid1.Location = new System.Drawing.Point(0, 180);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.Size = new System.Drawing.Size(430, 296);
            this.propertyGrid1.TabIndex = 3;
            // 
            // splitter1
            // 
            this.splitter1.Location = new System.Drawing.Point(430, 0);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(3, 476);
            this.splitter1.TabIndex = 6;
            this.splitter1.TabStop = false;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.btnUpdateReference);
            this.panel2.Controls.Add(this.btnMoveDown);
            this.panel2.Controls.Add(this.btnMoveUp);
            this.panel2.Controls.Add(this.splitButton1);
            this.panel2.Controls.Add(this.btnCancel);
            this.panel2.Controls.Add(this.btnSave);
            this.panel2.Controls.Add(this.btnRemoveAction);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 476);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(910, 51);
            this.panel2.TabIndex = 7;
            // 
            // btnMoveDown
            // 
            this.btnMoveDown.Enabled = false;
            this.btnMoveDown.Location = new System.Drawing.Point(344, 16);
            this.btnMoveDown.Name = "btnMoveDown";
            this.btnMoveDown.Size = new System.Drawing.Size(75, 23);
            this.btnMoveDown.TabIndex = 9;
            this.btnMoveDown.Text = "Move Down";
            this.btnMoveDown.UseVisualStyleBackColor = true;
            this.btnMoveDown.Click += new System.EventHandler(this.btnMoveDown_Click);
            // 
            // btnMoveUp
            // 
            this.btnMoveUp.Enabled = false;
            this.btnMoveUp.Location = new System.Drawing.Point(263, 16);
            this.btnMoveUp.Name = "btnMoveUp";
            this.btnMoveUp.Size = new System.Drawing.Size(75, 23);
            this.btnMoveUp.TabIndex = 10;
            this.btnMoveUp.Text = "Move Up";
            this.btnMoveUp.UseVisualStyleBackColor = true;
            this.btnMoveUp.Click += new System.EventHandler(this.btnMoveUp_Click);
            // 
            // splitButton1
            // 
            this.splitButton1.ClickedImage = "Clicked";
            this.splitButton1.ContextMenuStrip = this.contextMenuStrip1;
            this.splitButton1.DisabledImage = "Disabled";
            this.splitButton1.DoubleClickedEnabled = true;
            this.splitButton1.FocusedImage = "Focused";
            this.splitButton1.HoverImage = "Hover";
            this.splitButton1.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.splitButton1.ImageKey = "Normal";
            this.splitButton1.Location = new System.Drawing.Point(12, 16);
            this.splitButton1.Name = "splitButton1";
            this.splitButton1.NormalImage = "Normal";
            this.splitButton1.Size = new System.Drawing.Size(90, 23);
            this.splitButton1.TabIndex = 8;
            this.splitButton1.Text = "Add Action";
            this.splitButton1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.splitButton1.UseVisualStyleBackColor = true;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.installOSToolStripMenuItem,
            this.uploadFileToolStripMenuItem,
            this.telnetCommandsToolStripMenuItem,
            this.registryIngestToolStripMenuItem,
            this.rebootToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(173, 114);
            // 
            // installOSToolStripMenuItem
            // 
            this.installOSToolStripMenuItem.Name = "installOSToolStripMenuItem";
            this.installOSToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.installOSToolStripMenuItem.Text = "Install OS";
            this.installOSToolStripMenuItem.Click += new System.EventHandler(this.installOSToolStripMenuItem_Click);
            // 
            // uploadFileToolStripMenuItem
            // 
            this.uploadFileToolStripMenuItem.Name = "uploadFileToolStripMenuItem";
            this.uploadFileToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.uploadFileToolStripMenuItem.Text = "Upload File";
            this.uploadFileToolStripMenuItem.Click += new System.EventHandler(this.uploadFileToolStripMenuItem_Click);
            // 
            // telnetCommandsToolStripMenuItem
            // 
            this.telnetCommandsToolStripMenuItem.Name = "telnetCommandsToolStripMenuItem";
            this.telnetCommandsToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.telnetCommandsToolStripMenuItem.Text = "Telnet Commands";
            this.telnetCommandsToolStripMenuItem.Click += new System.EventHandler(this.telnetCommandsToolStripMenuItem_Click);
            // 
            // registryIngestToolStripMenuItem
            // 
            this.registryIngestToolStripMenuItem.Name = "registryIngestToolStripMenuItem";
            this.registryIngestToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.registryIngestToolStripMenuItem.Text = "Registry Ingest";
            this.registryIngestToolStripMenuItem.Click += new System.EventHandler(this.registryIngestToolStripMenuItem_Click);
            // 
            // rebootToolStripMenuItem
            // 
            this.rebootToolStripMenuItem.Name = "rebootToolStripMenuItem";
            this.rebootToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.rebootToolStripMenuItem.Text = "Reboot";
            this.rebootToolStripMenuItem.Click += new System.EventHandler(this.rebootToolStripMenuItem_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(823, 16);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(742, 16);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 2;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnRemoveAction
            // 
            this.btnRemoveAction.Location = new System.Drawing.Point(108, 16);
            this.btnRemoveAction.Name = "btnRemoveAction";
            this.btnRemoveAction.Size = new System.Drawing.Size(91, 23);
            this.btnRemoveAction.TabIndex = 1;
            this.btnRemoveAction.Text = "Remove Action";
            this.btnRemoveAction.UseVisualStyleBackColor = true;
            this.btnRemoveAction.Click += new System.EventHandler(this.btnRemoveAction_Click);
            // 
            // lvwSteps
            // 
            this.lvwSteps.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvwSteps.Location = new System.Drawing.Point(0, 0);
            this.lvwSteps.Name = "lvwSteps";
            this.lvwSteps.Size = new System.Drawing.Size(430, 180);
            this.lvwSteps.TabIndex = 4;
            this.lvwSteps.UseCompatibleStateImageBehavior = false;
            this.lvwSteps.View = System.Windows.Forms.View.List;
            this.lvwSteps.SelectedIndexChanged += new System.EventHandler(this.lvwSteps_SelectedIndexChanged);
            // 
            // btnUpdateReference
            // 
            this.btnUpdateReference.Location = new System.Drawing.Point(538, 16);
            this.btnUpdateReference.Name = "btnUpdateReference";
            this.btnUpdateReference.Size = new System.Drawing.Size(103, 23);
            this.btnUpdateReference.TabIndex = 5;
            this.btnUpdateReference.Text = "Update Reference";
            this.btnUpdateReference.UseVisualStyleBackColor = true;
            this.btnUpdateReference.Visible = false;
            this.btnUpdateReference.Click += new System.EventHandler(this.btnUpdateReference_Click);
            // 
            // EditPropertiesDlg
            // 
            this.AcceptButton = this.btnSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(910, 527);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.txtJson);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel2);
            this.Name = "EditPropertiesDlg";
            this.Text = "EditPropertiesDlg";
            this.Shown += new System.EventHandler(this.EditPropertiesDlg_Shown);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtJson;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PropertyGrid propertyGrid1;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnRemoveAction;
        private SplitButtonDemo.SplitButton splitButton1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem installOSToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem telnetCommandsToolStripMenuItem;
        private System.Windows.Forms.Button btnMoveDown;
        private System.Windows.Forms.Button btnMoveUp;
        private System.Windows.Forms.ToolStripMenuItem rebootToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem uploadFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem registryIngestToolStripMenuItem;
        private System.Windows.Forms.ListView lvwSteps;
        private System.Windows.Forms.Button btnUpdateReference;
    }
}