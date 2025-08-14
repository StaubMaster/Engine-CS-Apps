
namespace VoidFactory.Editor
{
    partial class EditorPolyHedra
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
            this.rTB_Content = new System.Windows.Forms.RichTextBox();
            this.b_Content_load = new System.Windows.Forms.Button();
            this.glC_Display = new OpenTK.GLControl.GLControl();
            this.cB_Content_update_auto = new System.Windows.Forms.CheckBox();
            this.b_Content_update = new System.Windows.Forms.Button();
            this.b_Browse_change = new System.Windows.Forms.Button();
            this.fD_Browse = new System.Windows.Forms.FolderBrowserDialog();
            this.b_Browse_refresh = new System.Windows.Forms.Button();
            this.tV_Browse = new System.Windows.Forms.TreeView();
            this.tB_Browse_Dir = new System.Windows.Forms.TextBox();
            this.b_Content_save = new System.Windows.Forms.Button();
            this.gB_Content_Cursor = new System.Windows.Forms.GroupBox();
            this.label_Content_AOR = new System.Windows.Forms.Label();
            this.label_Content_data_AOR = new System.Windows.Forms.Label();
            this.label_Content_data_Cursor = new System.Windows.Forms.Label();
            this.label_Content_name_AOR = new System.Windows.Forms.Label();
            this.label_Content_name_Cursor = new System.Windows.Forms.Label();
            this.gB_Content = new System.Windows.Forms.GroupBox();
            this.label_Content_name_Count = new System.Windows.Forms.Label();
            this.label_Content_name_Size = new System.Windows.Forms.Label();
            this.label_Content_data_Count = new System.Windows.Forms.Label();
            this.label_Content_data_Size = new System.Windows.Forms.Label();
            this.cB_Display_color = new System.Windows.Forms.CheckBox();
            this.cB_Display_light = new System.Windows.Forms.CheckBox();
            this.cB_Display_Light_Follow = new System.Windows.Forms.CheckBox();
            this.tB_Browse_File = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.rTB_Error = new System.Windows.Forms.RichTextBox();
            this.b_view_reset = new System.Windows.Forms.Button();
            this.gB_Content_Cursor.SuspendLayout();
            this.gB_Content.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            this.SuspendLayout();
            // 
            // rTB_Content
            // 
            this.rTB_Content.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rTB_Content.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(31)))), ((int)(((byte)(31)))));
            this.rTB_Content.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(225)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.rTB_Content.Location = new System.Drawing.Point(3, 33);
            this.rTB_Content.Name = "rTB_Content";
            this.rTB_Content.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedVertical;
            this.rTB_Content.Size = new System.Drawing.Size(311, 140);
            this.rTB_Content.TabIndex = 0;
            this.rTB_Content.Text = "";
            this.rTB_Content.WordWrap = false;
            this.rTB_Content.SelectionChanged += new System.EventHandler(this.rTB_Content_SelectionChanged);
            this.rTB_Content.TextChanged += new System.EventHandler(this.rTB_Content_TextChanged);
            // 
            // b_Content_load
            // 
            this.b_Content_load.Location = new System.Drawing.Point(3, 3);
            this.b_Content_load.Name = "b_Content_load";
            this.b_Content_load.Size = new System.Drawing.Size(75, 24);
            this.b_Content_load.TabIndex = 1;
            this.b_Content_load.Text = "Load";
            this.b_Content_load.UseVisualStyleBackColor = true;
            this.b_Content_load.Click += new System.EventHandler(this.b_Content_load_Click);
            // 
            // glC_Display
            // 
            this.glC_Display.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.glC_Display.API = OpenTK.Windowing.Common.ContextAPI.OpenGL;
            this.glC_Display.APIVersion = new System.Version(4, 0, 0, 0);
            this.glC_Display.Cursor = System.Windows.Forms.Cursors.Cross;
            this.glC_Display.Flags = OpenTK.Windowing.Common.ContextFlags.Default;
            this.glC_Display.IsEventDriven = true;
            this.glC_Display.Location = new System.Drawing.Point(3, 3);
            this.glC_Display.Name = "glC_Display";
            this.glC_Display.Profile = OpenTK.Windowing.Common.ContextProfile.Core;
            this.glC_Display.SharedContext = null;
            this.glC_Display.Size = new System.Drawing.Size(377, 229);
            this.glC_Display.TabIndex = 0;
            this.glC_Display.MouseClick += new System.Windows.Forms.MouseEventHandler(this.glC_Display_MouseClick);
            this.glC_Display.MouseMove += new System.Windows.Forms.MouseEventHandler(this.glC_Display_MouseMove);
            this.glC_Display.Resize += new System.EventHandler(this.glC_Display_Resize);
            // 
            // cB_Content_update_auto
            // 
            this.cB_Content_update_auto.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cB_Content_update_auto.AutoSize = true;
            this.cB_Content_update_auto.Checked = true;
            this.cB_Content_update_auto.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cB_Content_update_auto.Location = new System.Drawing.Point(130, 183);
            this.cB_Content_update_auto.Name = "cB_Content_update_auto";
            this.cB_Content_update_auto.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.cB_Content_update_auto.Size = new System.Drawing.Size(103, 18);
            this.cB_Content_update_auto.TabIndex = 26;
            this.cB_Content_update_auto.Text = "auto update";
            this.cB_Content_update_auto.UseVisualStyleBackColor = true;
            // 
            // b_Content_update
            // 
            this.b_Content_update.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.b_Content_update.Location = new System.Drawing.Point(239, 179);
            this.b_Content_update.Name = "b_Content_update";
            this.b_Content_update.Size = new System.Drawing.Size(75, 24);
            this.b_Content_update.TabIndex = 27;
            this.b_Content_update.Text = "update";
            this.b_Content_update.UseVisualStyleBackColor = true;
            this.b_Content_update.Click += new System.EventHandler(this.b_Content_Update_Click);
            // 
            // b_Browse_change
            // 
            this.b_Browse_change.Location = new System.Drawing.Point(3, 3);
            this.b_Browse_change.Name = "b_Browse_change";
            this.b_Browse_change.Size = new System.Drawing.Size(75, 24);
            this.b_Browse_change.TabIndex = 31;
            this.b_Browse_change.Text = "Change";
            this.b_Browse_change.UseVisualStyleBackColor = true;
            this.b_Browse_change.Click += new System.EventHandler(this.b_Browse_change_Click);
            // 
            // b_Browse_refresh
            // 
            this.b_Browse_refresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.b_Browse_refresh.Location = new System.Drawing.Point(239, 3);
            this.b_Browse_refresh.Name = "b_Browse_refresh";
            this.b_Browse_refresh.Size = new System.Drawing.Size(75, 24);
            this.b_Browse_refresh.TabIndex = 35;
            this.b_Browse_refresh.Text = "Refresh";
            this.b_Browse_refresh.UseVisualStyleBackColor = true;
            this.b_Browse_refresh.Click += new System.EventHandler(this.b_Browse_refresh_Click);
            // 
            // tV_Browse
            // 
            this.tV_Browse.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tV_Browse.Location = new System.Drawing.Point(3, 33);
            this.tV_Browse.Name = "tV_Browse";
            this.tV_Browse.Size = new System.Drawing.Size(311, 124);
            this.tV_Browse.TabIndex = 42;
            this.tV_Browse.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tV_Browse_AfterSelect);
            // 
            // tB_Browse_Dir
            // 
            this.tB_Browse_Dir.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tB_Browse_Dir.Location = new System.Drawing.Point(84, 4);
            this.tB_Browse_Dir.Name = "tB_Browse_Dir";
            this.tB_Browse_Dir.ReadOnly = true;
            this.tB_Browse_Dir.Size = new System.Drawing.Size(149, 22);
            this.tB_Browse_Dir.TabIndex = 32;
            // 
            // b_Content_save
            // 
            this.b_Content_save.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.b_Content_save.Location = new System.Drawing.Point(239, 3);
            this.b_Content_save.Name = "b_Content_save";
            this.b_Content_save.Size = new System.Drawing.Size(75, 24);
            this.b_Content_save.TabIndex = 38;
            this.b_Content_save.Text = "Save";
            this.b_Content_save.UseVisualStyleBackColor = true;
            this.b_Content_save.Click += new System.EventHandler(this.b_Content_save_Click);
            // 
            // gB_Content_Cursor
            // 
            this.gB_Content_Cursor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.gB_Content_Cursor.Controls.Add(this.label_Content_AOR);
            this.gB_Content_Cursor.Controls.Add(this.label_Content_data_AOR);
            this.gB_Content_Cursor.Controls.Add(this.label_Content_data_Cursor);
            this.gB_Content_Cursor.Controls.Add(this.label_Content_name_AOR);
            this.gB_Content_Cursor.Controls.Add(this.label_Content_name_Cursor);
            this.gB_Content_Cursor.Location = new System.Drawing.Point(12, 395);
            this.gB_Content_Cursor.Name = "gB_Content_Cursor";
            this.gB_Content_Cursor.Size = new System.Drawing.Size(273, 105);
            this.gB_Content_Cursor.TabIndex = 33;
            this.gB_Content_Cursor.TabStop = false;
            this.gB_Content_Cursor.Text = "Cursor";
            // 
            // label_Content_AOR
            // 
            this.label_Content_AOR.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label_Content_AOR.AutoSize = true;
            this.label_Content_AOR.Location = new System.Drawing.Point(6, 46);
            this.label_Content_AOR.Name = "label_Content_AOR";
            this.label_Content_AOR.Size = new System.Drawing.Size(112, 14);
            this.label_Content_AOR.TabIndex = 42;
            this.label_Content_AOR.Text = "abs - off = rel";
            // 
            // label_Content_data_AOR
            // 
            this.label_Content_data_AOR.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label_Content_data_AOR.AutoSize = true;
            this.label_Content_data_AOR.Location = new System.Drawing.Point(68, 60);
            this.label_Content_data_AOR.Name = "label_Content_data_AOR";
            this.label_Content_data_AOR.Size = new System.Drawing.Size(133, 28);
            this.label_Content_data_AOR.TabIndex = 41;
            this.label_Content_data_AOR.Text = "#### - #### = ####\r\n#### - #### = ####";
            // 
            // label_Content_data_Cursor
            // 
            this.label_Content_data_Cursor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label_Content_data_Cursor.AutoSize = true;
            this.label_Content_data_Cursor.Location = new System.Drawing.Point(103, 18);
            this.label_Content_data_Cursor.Name = "label_Content_data_Cursor";
            this.label_Content_data_Cursor.Size = new System.Drawing.Size(35, 28);
            this.label_Content_data_Cursor.TabIndex = 37;
            this.label_Content_data_Cursor.Text = "####\r\n####";
            // 
            // label_Content_name_AOR
            // 
            this.label_Content_name_AOR.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label_Content_name_AOR.AutoSize = true;
            this.label_Content_name_AOR.Location = new System.Drawing.Point(6, 60);
            this.label_Content_name_AOR.Name = "label_Content_name_AOR";
            this.label_Content_name_AOR.Size = new System.Drawing.Size(56, 28);
            this.label_Content_name_AOR.TabIndex = 40;
            this.label_Content_name_AOR.Text = "vertex:\r\nface:";
            // 
            // label_Content_name_Cursor
            // 
            this.label_Content_name_Cursor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label_Content_name_Cursor.AutoSize = true;
            this.label_Content_name_Cursor.Location = new System.Drawing.Point(6, 18);
            this.label_Content_name_Cursor.Name = "label_Content_name_Cursor";
            this.label_Content_name_Cursor.Size = new System.Drawing.Size(91, 28);
            this.label_Content_name_Cursor.TabIndex = 36;
            this.label_Content_name_Cursor.Text = "cursor line:\r\ncursor col:";
            // 
            // gB_Content
            // 
            this.gB_Content.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.gB_Content.Controls.Add(this.label_Content_name_Count);
            this.gB_Content.Controls.Add(this.label_Content_name_Size);
            this.gB_Content.Controls.Add(this.label_Content_data_Count);
            this.gB_Content.Controls.Add(this.label_Content_data_Size);
            this.gB_Content.Location = new System.Drawing.Point(291, 395);
            this.gB_Content.Name = "gB_Content";
            this.gB_Content.Size = new System.Drawing.Size(274, 105);
            this.gB_Content.TabIndex = 34;
            this.gB_Content.TabStop = false;
            this.gB_Content.Text = "Content";
            // 
            // label_Content_name_Count
            // 
            this.label_Content_name_Count.AutoSize = true;
            this.label_Content_name_Count.Location = new System.Drawing.Point(6, 18);
            this.label_Content_name_Count.Name = "label_Content_name_Count";
            this.label_Content_name_Count.Size = new System.Drawing.Size(98, 28);
            this.label_Content_name_Count.TabIndex = 38;
            this.label_Content_name_Count.Text = "vertex count:\r\nface count:";
            // 
            // label_Content_name_Size
            // 
            this.label_Content_name_Size.AutoSize = true;
            this.label_Content_name_Size.Location = new System.Drawing.Point(6, 46);
            this.label_Content_name_Size.Name = "label_Content_name_Size";
            this.label_Content_name_Size.Size = new System.Drawing.Size(49, 42);
            this.label_Content_name_Size.TabIndex = 36;
            this.label_Content_name_Size.Text = "min:\r\nmax:\r\nrange:";
            // 
            // label_Content_data_Count
            // 
            this.label_Content_data_Count.AutoSize = true;
            this.label_Content_data_Count.Location = new System.Drawing.Point(110, 18);
            this.label_Content_data_Count.Name = "label_Content_data_Count";
            this.label_Content_data_Count.Size = new System.Drawing.Size(35, 28);
            this.label_Content_data_Count.TabIndex = 39;
            this.label_Content_data_Count.Text = "####\r\n####";
            // 
            // label_Content_data_Size
            // 
            this.label_Content_data_Size.AutoSize = true;
            this.label_Content_data_Size.Location = new System.Drawing.Point(61, 46);
            this.label_Content_data_Size.Name = "label_Content_data_Size";
            this.label_Content_data_Size.Size = new System.Drawing.Size(175, 42);
            this.label_Content_data_Size.TabIndex = 37;
            this.label_Content_data_Size.Text = "~##.## , ~##.## , ~##.##\r\n~##.## , ~##.## , ~##.##\r\n~##.## , ~##.## , ~##.##\r\n";
            // 
            // cB_Display_color
            // 
            this.cB_Display_color.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cB_Display_color.AutoSize = true;
            this.cB_Display_color.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cB_Display_color.Checked = true;
            this.cB_Display_color.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cB_Display_color.Location = new System.Drawing.Point(663, 434);
            this.cB_Display_color.Name = "cB_Display_color";
            this.cB_Display_color.Size = new System.Drawing.Size(61, 18);
            this.cB_Display_color.TabIndex = 39;
            this.cB_Display_color.Text = "Color";
            this.cB_Display_color.UseVisualStyleBackColor = true;
            this.cB_Display_color.CheckedChanged += new System.EventHandler(this.cB_Display_light_CheckedChanged);
            // 
            // cB_Display_light
            // 
            this.cB_Display_light.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cB_Display_light.AutoSize = true;
            this.cB_Display_light.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cB_Display_light.Checked = true;
            this.cB_Display_light.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cB_Display_light.Location = new System.Drawing.Point(663, 458);
            this.cB_Display_light.Name = "cB_Display_light";
            this.cB_Display_light.Size = new System.Drawing.Size(61, 18);
            this.cB_Display_light.TabIndex = 40;
            this.cB_Display_light.Text = "Light";
            this.cB_Display_light.UseVisualStyleBackColor = true;
            this.cB_Display_light.CheckedChanged += new System.EventHandler(this.cB_Display_color_CheckedChanged);
            // 
            // cB_Display_Light_Follow
            // 
            this.cB_Display_Light_Follow.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cB_Display_Light_Follow.AutoSize = true;
            this.cB_Display_Light_Follow.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cB_Display_Light_Follow.Location = new System.Drawing.Point(579, 482);
            this.cB_Display_Light_Follow.Name = "cB_Display_Light_Follow";
            this.cB_Display_Light_Follow.Size = new System.Drawing.Size(145, 18);
            this.cB_Display_Light_Follow.TabIndex = 41;
            this.cB_Display_Light_Follow.Text = "Light follow View";
            this.cB_Display_Light_Follow.UseVisualStyleBackColor = true;
            this.cB_Display_Light_Follow.CheckedChanged += new System.EventHandler(this.cB_Display_Light_Follow_CheckedChanged);
            // 
            // tB_Browse_File
            // 
            this.tB_Browse_File.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tB_Browse_File.Location = new System.Drawing.Point(84, 4);
            this.tB_Browse_File.Name = "tB_Browse_File";
            this.tB_Browse_File.ReadOnly = true;
            this.tB_Browse_File.Size = new System.Drawing.Size(149, 22);
            this.tB_Browse_File.TabIndex = 43;
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.splitContainer1);
            this.panel1.Location = new System.Drawing.Point(12, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(712, 377);
            this.panel1.TabIndex = 44;
            // 
            // splitContainer1
            // 
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitContainer1.Cursor = System.Windows.Forms.Cursors.VSplit;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.splitContainer2);
            this.splitContainer1.Panel1.Cursor = System.Windows.Forms.Cursors.Default;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer3);
            this.splitContainer1.Panel2.Cursor = System.Windows.Forms.Cursors.Default;
            this.splitContainer1.Size = new System.Drawing.Size(712, 377);
            this.splitContainer1.SplitterDistance = 321;
            this.splitContainer1.TabIndex = 0;
            // 
            // splitContainer2
            // 
            this.splitContainer2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitContainer2.Cursor = System.Windows.Forms.Cursors.HSplit;
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.tV_Browse);
            this.splitContainer2.Panel1.Controls.Add(this.tB_Browse_Dir);
            this.splitContainer2.Panel1.Controls.Add(this.b_Browse_refresh);
            this.splitContainer2.Panel1.Controls.Add(this.b_Browse_change);
            this.splitContainer2.Panel1.Cursor = System.Windows.Forms.Cursors.Default;
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.rTB_Content);
            this.splitContainer2.Panel2.Controls.Add(this.tB_Browse_File);
            this.splitContainer2.Panel2.Controls.Add(this.b_Content_load);
            this.splitContainer2.Panel2.Controls.Add(this.b_Content_save);
            this.splitContainer2.Panel2.Controls.Add(this.cB_Content_update_auto);
            this.splitContainer2.Panel2.Controls.Add(this.b_Content_update);
            this.splitContainer2.Panel2.Cursor = System.Windows.Forms.Cursors.Default;
            this.splitContainer2.Size = new System.Drawing.Size(321, 377);
            this.splitContainer2.SplitterDistance = 164;
            this.splitContainer2.TabIndex = 0;
            // 
            // splitContainer3
            // 
            this.splitContainer3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitContainer3.Cursor = System.Windows.Forms.Cursors.HSplit;
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer3.Location = new System.Drawing.Point(0, 0);
            this.splitContainer3.Name = "splitContainer3";
            this.splitContainer3.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.glC_Display);
            this.splitContainer3.Panel1.Cursor = System.Windows.Forms.Cursors.Default;
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.rTB_Error);
            this.splitContainer3.Panel2.Cursor = System.Windows.Forms.Cursors.Default;
            this.splitContainer3.Size = new System.Drawing.Size(387, 377);
            this.splitContainer3.SplitterDistance = 239;
            this.splitContainer3.TabIndex = 0;
            // 
            // rTB_Error
            // 
            this.rTB_Error.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rTB_Error.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(31)))), ((int)(((byte)(31)))));
            this.rTB_Error.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(63)))), ((int)(((byte)(63)))));
            this.rTB_Error.Location = new System.Drawing.Point(3, 3);
            this.rTB_Error.Name = "rTB_Error";
            this.rTB_Error.ReadOnly = true;
            this.rTB_Error.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedVertical;
            this.rTB_Error.Size = new System.Drawing.Size(377, 124);
            this.rTB_Error.TabIndex = 0;
            this.rTB_Error.Text = "";
            this.rTB_Error.WordWrap = false;
            // 
            // b_view_reset
            // 
            this.b_view_reset.Location = new System.Drawing.Point(622, 395);
            this.b_view_reset.Name = "b_view_reset";
            this.b_view_reset.Size = new System.Drawing.Size(102, 23);
            this.b_view_reset.TabIndex = 45;
            this.b_view_reset.Text = "reset view";
            this.b_view_reset.UseVisualStyleBackColor = true;
            this.b_view_reset.Click += new System.EventHandler(this.b_view_reset_Click);
            // 
            // EditorPolyHedra
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(736, 512);
            this.Controls.Add(this.b_view_reset);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.cB_Display_Light_Follow);
            this.Controls.Add(this.cB_Display_light);
            this.Controls.Add(this.cB_Display_color);
            this.Controls.Add(this.gB_Content);
            this.Controls.Add(this.gB_Content_Cursor);
            this.Font = new System.Drawing.Font("Noto Mono", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.KeyPreview = true;
            this.Name = "EditorPolyHedra";
            this.Text = "Polyhedra";
            this.Load += new System.EventHandler(this.EditorPolyhedra_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.EditorPolyHedra_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.EditorPolyHedra_KeyUp);
            this.gB_Content_Cursor.ResumeLayout(false);
            this.gB_Content_Cursor.PerformLayout();
            this.gB_Content.ResumeLayout(false);
            this.gB_Content.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel1.PerformLayout();
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
            this.splitContainer3.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox rTB_Content;
        private System.Windows.Forms.Button b_Content_load;
        private System.Windows.Forms.CheckBox cB_Content_update_auto;
        private System.Windows.Forms.Button b_Content_update;
        private OpenTK.GLControl.GLControl glC_Display;
        private System.Windows.Forms.Button b_Browse_change;
        private System.Windows.Forms.FolderBrowserDialog fD_Browse;
        private System.Windows.Forms.TextBox tB_Browse_Dir;
        private System.Windows.Forms.GroupBox gB_Content_Cursor;
        private System.Windows.Forms.GroupBox gB_Content;
        private System.Windows.Forms.Label label_Content_name_Cursor;
        private System.Windows.Forms.Label label_Content_data_Cursor;
        private System.Windows.Forms.Label label_Content_name_Size;
        private System.Windows.Forms.Label label_Content_data_Size;
        private System.Windows.Forms.Label label_Content_name_Count;
        private System.Windows.Forms.Label label_Content_data_Count;
        private System.Windows.Forms.Label label_Content_name_AOR;
        private System.Windows.Forms.Label label_Content_data_AOR;
        private System.Windows.Forms.Label label_Content_AOR;
        private System.Windows.Forms.Button b_Content_save;
        private System.Windows.Forms.CheckBox cB_Display_color;
        private System.Windows.Forms.CheckBox cB_Display_light;
        private System.Windows.Forms.CheckBox cB_Display_Light_Follow;
        private System.Windows.Forms.TreeView tV_Browse;
        private System.Windows.Forms.TextBox tB_Browse_File;
        private System.Windows.Forms.Button b_Browse_refresh;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.SplitContainer splitContainer3;
        private System.Windows.Forms.RichTextBox rTB_Error;
        private System.Windows.Forms.Button b_view_reset;
    }
}