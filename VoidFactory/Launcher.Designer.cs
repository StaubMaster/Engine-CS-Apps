
namespace VoidFactory
{
    partial class Launcher
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.b_game_delete = new System.Windows.Forms.Button();
            this.ConsoleTextBox = new System.Windows.Forms.RichTextBox();
            this.gB_select = new System.Windows.Forms.GroupBox();
            this.b_Space = new System.Windows.Forms.Button();
            this.b_test = new System.Windows.Forms.Button();
            this.b_Polygon = new System.Windows.Forms.Button();
            this.b_Scene = new System.Windows.Forms.Button();
            this.b_Hedra = new System.Windows.Forms.Button();
            this.b_Plane = new System.Windows.Forms.Button();
            this.b_reset = new System.Windows.Forms.Button();
            this.b_game_create = new System.Windows.Forms.Button();
            this.lbl_Game = new System.Windows.Forms.Label();
            this.File_Load_Dialog = new System.Windows.Forms.OpenFileDialog();
            this.File_Save_Dialog = new System.Windows.Forms.SaveFileDialog();
            this.b_game_run = new System.Windows.Forms.Button();
            this.gB_select.SuspendLayout();
            this.SuspendLayout();
            // 
            // b_game_delete
            // 
            this.b_game_delete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.b_game_delete.Enabled = false;
            this.b_game_delete.Location = new System.Drawing.Point(493, 352);
            this.b_game_delete.Name = "b_game_delete";
            this.b_game_delete.Size = new System.Drawing.Size(75, 23);
            this.b_game_delete.TabIndex = 1;
            this.b_game_delete.Text = "delete";
            this.b_game_delete.UseVisualStyleBackColor = true;
            this.b_game_delete.Click += new System.EventHandler(this.b_Game_close_Click);
            // 
            // ConsoleTextBox
            // 
            this.ConsoleTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ConsoleTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(31)))), ((int)(((byte)(31)))));
            this.ConsoleTextBox.DetectUrls = false;
            this.ConsoleTextBox.Font = new System.Drawing.Font("Noto Mono", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.ConsoleTextBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(225)))), ((int)(((byte)(225)))), ((int)(((byte)(225)))));
            this.ConsoleTextBox.Location = new System.Drawing.Point(120, 12);
            this.ConsoleTextBox.Name = "ConsoleTextBox";
            this.ConsoleTextBox.ReadOnly = true;
            this.ConsoleTextBox.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedVertical;
            this.ConsoleTextBox.Size = new System.Drawing.Size(448, 334);
            this.ConsoleTextBox.TabIndex = 4;
            this.ConsoleTextBox.Text = "";
            this.ConsoleTextBox.WordWrap = false;
            // 
            // gB_select
            // 
            this.gB_select.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.gB_select.Controls.Add(this.b_Space);
            this.gB_select.Controls.Add(this.b_test);
            this.gB_select.Controls.Add(this.b_Polygon);
            this.gB_select.Controls.Add(this.b_Scene);
            this.gB_select.Controls.Add(this.b_Hedra);
            this.gB_select.Controls.Add(this.b_Plane);
            this.gB_select.Location = new System.Drawing.Point(12, 12);
            this.gB_select.Name = "gB_select";
            this.gB_select.Size = new System.Drawing.Size(102, 363);
            this.gB_select.TabIndex = 5;
            this.gB_select.TabStop = false;
            this.gB_select.Text = "Select";
            // 
            // b_Space
            // 
            this.b_Space.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.b_Space.Location = new System.Drawing.Point(6, 247);
            this.b_Space.Name = "b_Space";
            this.b_Space.Size = new System.Drawing.Size(90, 23);
            this.b_Space.TabIndex = 7;
            this.b_Space.Text = "Space";
            this.b_Space.UseVisualStyleBackColor = true;
            this.b_Space.Click += new System.EventHandler(this.b_Space_Click);
            // 
            // b_test
            // 
            this.b_test.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.b_test.Location = new System.Drawing.Point(6, 22);
            this.b_test.Name = "b_test";
            this.b_test.Size = new System.Drawing.Size(90, 23);
            this.b_test.TabIndex = 6;
            this.b_test.Text = "test";
            this.b_test.UseVisualStyleBackColor = true;
            this.b_test.Click += new System.EventHandler(this.b_test_Click);
            // 
            // b_Polygon
            // 
            this.b_Polygon.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.b_Polygon.Location = new System.Drawing.Point(6, 334);
            this.b_Polygon.Name = "b_Polygon";
            this.b_Polygon.Size = new System.Drawing.Size(90, 23);
            this.b_Polygon.TabIndex = 5;
            this.b_Polygon.Text = "PolyGon";
            this.b_Polygon.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.b_Polygon.UseVisualStyleBackColor = true;
            this.b_Polygon.Click += new System.EventHandler(this.b_Polygon_Click);
            // 
            // b_Scene
            // 
            this.b_Scene.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.b_Scene.Location = new System.Drawing.Point(6, 276);
            this.b_Scene.Name = "b_Scene";
            this.b_Scene.Size = new System.Drawing.Size(90, 23);
            this.b_Scene.TabIndex = 4;
            this.b_Scene.Text = "PolySoma";
            this.b_Scene.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.b_Scene.UseVisualStyleBackColor = true;
            this.b_Scene.Click += new System.EventHandler(this.b_Editor_Soma_Click);
            // 
            // b_Hedra
            // 
            this.b_Hedra.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.b_Hedra.Location = new System.Drawing.Point(6, 305);
            this.b_Hedra.Name = "b_Hedra";
            this.b_Hedra.Size = new System.Drawing.Size(90, 23);
            this.b_Hedra.TabIndex = 3;
            this.b_Hedra.Text = "PolyHedron";
            this.b_Hedra.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.b_Hedra.UseVisualStyleBackColor = true;
            this.b_Hedra.Click += new System.EventHandler(this.b_Editor_Hedra_Click);
            // 
            // b_Plane
            // 
            this.b_Plane.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.b_Plane.Location = new System.Drawing.Point(6, 218);
            this.b_Plane.Name = "b_Plane";
            this.b_Plane.Size = new System.Drawing.Size(90, 23);
            this.b_Plane.TabIndex = 1;
            this.b_Plane.Text = "Plane";
            this.b_Plane.UseVisualStyleBackColor = true;
            this.b_Plane.Click += new System.EventHandler(this.b_Plane_Click);
            // 
            // b_reset
            // 
            this.b_reset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.b_reset.Location = new System.Drawing.Point(120, 352);
            this.b_reset.Name = "b_reset";
            this.b_reset.Size = new System.Drawing.Size(75, 23);
            this.b_reset.TabIndex = 2;
            this.b_reset.Text = "reset Log";
            this.b_reset.UseVisualStyleBackColor = true;
            this.b_reset.Click += new System.EventHandler(this.b_Log_reset_Click);
            // 
            // b_game_create
            // 
            this.b_game_create.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.b_game_create.Enabled = false;
            this.b_game_create.Location = new System.Drawing.Point(331, 352);
            this.b_game_create.Name = "b_game_create";
            this.b_game_create.Size = new System.Drawing.Size(75, 23);
            this.b_game_create.TabIndex = 6;
            this.b_game_create.Text = "create";
            this.b_game_create.UseVisualStyleBackColor = true;
            this.b_game_create.Click += new System.EventHandler(this.b_Game_start_Click);
            // 
            // lbl_Game
            // 
            this.lbl_Game.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lbl_Game.AutoSize = true;
            this.lbl_Game.Location = new System.Drawing.Point(201, 356);
            this.lbl_Game.Name = "lbl_Game";
            this.lbl_Game.Size = new System.Drawing.Size(34, 15);
            this.lbl_Game.TabIndex = 7;
            this.lbl_Game.Text = "none";
            // 
            // File_Load_Dialog
            // 
            this.File_Load_Dialog.Filter = "*.txt|*.ymt";
            // 
            // File_Save_Dialog
            // 
            this.File_Save_Dialog.Filter = "*.txt|*.ymt";
            // 
            // b_game_run
            // 
            this.b_game_run.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.b_game_run.Enabled = false;
            this.b_game_run.Location = new System.Drawing.Point(412, 352);
            this.b_game_run.Name = "b_game_run";
            this.b_game_run.Size = new System.Drawing.Size(75, 23);
            this.b_game_run.TabIndex = 8;
            this.b_game_run.Text = "run";
            this.b_game_run.UseVisualStyleBackColor = true;
            this.b_game_run.Click += new System.EventHandler(this.b_Game_run_Click);
            // 
            // Launcher
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(580, 387);
            this.Controls.Add(this.b_game_run);
            this.Controls.Add(this.lbl_Game);
            this.Controls.Add(this.b_game_create);
            this.Controls.Add(this.gB_select);
            this.Controls.Add(this.ConsoleTextBox);
            this.Controls.Add(this.b_reset);
            this.Controls.Add(this.b_game_delete);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Launcher";
            this.Text = "Launcher";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Launcher_FormClosing);
            this.gB_select.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button b_game_delete;
        private System.Windows.Forms.RichTextBox ConsoleTextBox;
        private System.Windows.Forms.GroupBox gB_select;
        private System.Windows.Forms.Button b_Plane;
        private System.Windows.Forms.Button b_game_create;
        private System.Windows.Forms.Label lbl_Game;
        private System.Windows.Forms.Button b_reset;
        private System.Windows.Forms.Button b_Hedra;
        private System.Windows.Forms.OpenFileDialog File_Load_Dialog;
        private System.Windows.Forms.SaveFileDialog File_Save_Dialog;
        private System.Windows.Forms.Button b_game_run;
        private System.Windows.Forms.Button b_Scene;
        private System.Windows.Forms.Button b_Polygon;
        private System.Windows.Forms.Button b_test;
        private System.Windows.Forms.Button b_Space;
    }
}

