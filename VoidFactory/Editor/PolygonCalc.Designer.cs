
namespace VoidFactory
{
    partial class PolygonCalc
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
            this.num_Poly_Num = new System.Windows.Forms.NumericUpDown();
            this.num_Poly_Size = new System.Windows.Forms.NumericUpDown();
            this.Offset_Y = new System.Windows.Forms.NumericUpDown();
            this.Offset_X = new System.Windows.Forms.NumericUpDown();
            this.Offset_C = new System.Windows.Forms.NumericUpDown();
            this.Points_Flat = new System.Windows.Forms.RichTextBox();
            this.Points_Body = new System.Windows.Forms.RichTextBox();
            this.Manual = new System.Windows.Forms.Button();
            this.label_Y = new System.Windows.Forms.Label();
            this.label_X = new System.Windows.Forms.Label();
            this.label_C = new System.Windows.Forms.Label();
            this.label_Corner_Num = new System.Windows.Forms.Label();
            this.label_Corner_Size = new System.Windows.Forms.Label();
            this.Offset_reset = new System.Windows.Forms.Button();
            this.num_Layer_Num = new System.Windows.Forms.NumericUpDown();
            this.label_Layer_Num = new System.Windows.Forms.Label();
            this.label_Wnk = new System.Windows.Forms.Label();
            this.Wnk_Select = new System.Windows.Forms.ListBox();
            this.Dir_Select = new System.Windows.Forms.ListBox();
            this.label_Dir = new System.Windows.Forms.Label();
            this.num_Layer_Size = new System.Windows.Forms.NumericUpDown();
            this.label_Layer_Size = new System.Windows.Forms.Label();
            this.Point_Lead = new System.Windows.Forms.NumericUpDown();
            this.Index_Lead = new System.Windows.Forms.NumericUpDown();
            this.label_Point_0 = new System.Windows.Forms.Label();
            this.label_Index_0 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.num_Poly_Num)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_Poly_Size)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Offset_Y)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Offset_X)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Offset_C)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_Layer_Num)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_Layer_Size)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Point_Lead)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Index_Lead)).BeginInit();
            this.SuspendLayout();
            // 
            // num_Poly_Num
            // 
            this.num_Poly_Num.Location = new System.Drawing.Point(12, 11);
            this.num_Poly_Num.Maximum = new decimal(new int[] {
            32,
            0,
            0,
            0});
            this.num_Poly_Num.Name = "num_Poly_Num";
            this.num_Poly_Num.Size = new System.Drawing.Size(75, 22);
            this.num_Poly_Num.TabIndex = 0;
            this.num_Poly_Num.ValueChanged += new System.EventHandler(this.Poly_Num_ValueChanged);
            // 
            // num_Poly_Size
            // 
            this.num_Poly_Size.DecimalPlaces = 2;
            this.num_Poly_Size.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.num_Poly_Size.Location = new System.Drawing.Point(12, 38);
            this.num_Poly_Size.Name = "num_Poly_Size";
            this.num_Poly_Size.Size = new System.Drawing.Size(75, 22);
            this.num_Poly_Size.TabIndex = 1;
            this.num_Poly_Size.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.num_Poly_Size.ValueChanged += new System.EventHandler(this.Poly_Size_ValueChanged);
            // 
            // Offset_Y
            // 
            this.Offset_Y.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Offset_Y.DecimalPlaces = 2;
            this.Offset_Y.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.Offset_Y.Location = new System.Drawing.Point(555, 12);
            this.Offset_Y.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.Offset_Y.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            -2147483648});
            this.Offset_Y.Name = "Offset_Y";
            this.Offset_Y.Size = new System.Drawing.Size(75, 22);
            this.Offset_Y.TabIndex = 2;
            this.Offset_Y.ValueChanged += new System.EventHandler(this.Offset_Y_ValueChanged);
            // 
            // Offset_X
            // 
            this.Offset_X.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Offset_X.DecimalPlaces = 2;
            this.Offset_X.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.Offset_X.Location = new System.Drawing.Point(555, 39);
            this.Offset_X.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.Offset_X.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            -2147483648});
            this.Offset_X.Name = "Offset_X";
            this.Offset_X.Size = new System.Drawing.Size(75, 22);
            this.Offset_X.TabIndex = 3;
            this.Offset_X.ValueChanged += new System.EventHandler(this.Offset_X_ValueChanged);
            // 
            // Offset_C
            // 
            this.Offset_C.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Offset_C.DecimalPlaces = 2;
            this.Offset_C.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.Offset_C.Location = new System.Drawing.Point(555, 66);
            this.Offset_C.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.Offset_C.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            -2147483648});
            this.Offset_C.Name = "Offset_C";
            this.Offset_C.Size = new System.Drawing.Size(75, 22);
            this.Offset_C.TabIndex = 4;
            this.Offset_C.ValueChanged += new System.EventHandler(this.Offset_C_ValueChanged);
            // 
            // Points_Flat
            // 
            this.Points_Flat.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.Points_Flat.Font = new System.Drawing.Font("Noto Mono", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.Points_Flat.Location = new System.Drawing.Point(12, 121);
            this.Points_Flat.Name = "Points_Flat";
            this.Points_Flat.ReadOnly = true;
            this.Points_Flat.Size = new System.Drawing.Size(229, 261);
            this.Points_Flat.TabIndex = 5;
            this.Points_Flat.Text = "";
            // 
            // Points_Body
            // 
            this.Points_Body.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Points_Body.Font = new System.Drawing.Font("Noto Mono", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.Points_Body.Location = new System.Drawing.Point(247, 12);
            this.Points_Body.Name = "Points_Body";
            this.Points_Body.ReadOnly = true;
            this.Points_Body.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedVertical;
            this.Points_Body.Size = new System.Drawing.Size(240, 396);
            this.Points_Body.TabIndex = 6;
            this.Points_Body.Text = "";
            // 
            // Manual
            // 
            this.Manual.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.Manual.Location = new System.Drawing.Point(12, 387);
            this.Manual.Name = "Manual";
            this.Manual.Size = new System.Drawing.Size(75, 21);
            this.Manual.TabIndex = 7;
            this.Manual.Text = "calculate";
            this.Manual.UseVisualStyleBackColor = true;
            this.Manual.Click += new System.EventHandler(this.Manual_Click);
            // 
            // label_Y
            // 
            this.label_Y.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label_Y.AutoSize = true;
            this.label_Y.Location = new System.Drawing.Point(528, 14);
            this.label_Y.Name = "label_Y";
            this.label_Y.Size = new System.Drawing.Size(21, 14);
            this.label_Y.TabIndex = 8;
            this.label_Y.Text = "Y:";
            // 
            // label_X
            // 
            this.label_X.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label_X.AutoSize = true;
            this.label_X.Location = new System.Drawing.Point(528, 40);
            this.label_X.Name = "label_X";
            this.label_X.Size = new System.Drawing.Size(21, 14);
            this.label_X.TabIndex = 9;
            this.label_X.Text = "X:";
            // 
            // label_C
            // 
            this.label_C.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label_C.AutoSize = true;
            this.label_C.Location = new System.Drawing.Point(528, 67);
            this.label_C.Name = "label_C";
            this.label_C.Size = new System.Drawing.Size(21, 14);
            this.label_C.TabIndex = 10;
            this.label_C.Text = "C:";
            // 
            // label_Corner_Num
            // 
            this.label_Corner_Num.AutoSize = true;
            this.label_Corner_Num.Location = new System.Drawing.Point(93, 13);
            this.label_Corner_Num.Name = "label_Corner_Num";
            this.label_Corner_Num.Size = new System.Drawing.Size(77, 14);
            this.label_Corner_Num.TabIndex = 11;
            this.label_Corner_Num.Text = ": Poly Num";
            // 
            // label_Corner_Size
            // 
            this.label_Corner_Size.AutoSize = true;
            this.label_Corner_Size.Location = new System.Drawing.Point(93, 40);
            this.label_Corner_Size.Name = "label_Corner_Size";
            this.label_Corner_Size.Size = new System.Drawing.Size(84, 14);
            this.label_Corner_Size.TabIndex = 12;
            this.label_Corner_Size.Text = ": Poly Size";
            // 
            // Offset_reset
            // 
            this.Offset_reset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Offset_reset.Location = new System.Drawing.Point(555, 92);
            this.Offset_reset.Name = "Offset_reset";
            this.Offset_reset.Size = new System.Drawing.Size(75, 21);
            this.Offset_reset.TabIndex = 13;
            this.Offset_reset.Text = "reset";
            this.Offset_reset.UseVisualStyleBackColor = true;
            this.Offset_reset.Click += new System.EventHandler(this.Offset_reset_Click);
            // 
            // num_Layer_Num
            // 
            this.num_Layer_Num.Location = new System.Drawing.Point(12, 65);
            this.num_Layer_Num.Maximum = new decimal(new int[] {
            8,
            0,
            0,
            0});
            this.num_Layer_Num.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.num_Layer_Num.Name = "num_Layer_Num";
            this.num_Layer_Num.Size = new System.Drawing.Size(75, 22);
            this.num_Layer_Num.TabIndex = 16;
            this.num_Layer_Num.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.num_Layer_Num.ValueChanged += new System.EventHandler(this.Layer_Num_ValueChanged);
            // 
            // label_Layer_Num
            // 
            this.label_Layer_Num.AutoSize = true;
            this.label_Layer_Num.Location = new System.Drawing.Point(93, 67);
            this.label_Layer_Num.Name = "label_Layer_Num";
            this.label_Layer_Num.Size = new System.Drawing.Size(84, 14);
            this.label_Layer_Num.TabIndex = 17;
            this.label_Layer_Num.Text = ": Layer Num";
            // 
            // label_Wnk
            // 
            this.label_Wnk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label_Wnk.AutoSize = true;
            this.label_Wnk.Location = new System.Drawing.Point(507, 119);
            this.label_Wnk.Name = "label_Wnk";
            this.label_Wnk.Size = new System.Drawing.Size(42, 14);
            this.label_Wnk.TabIndex = 18;
            this.label_Wnk.Text = "Axis:";
            // 
            // Wnk_Select
            // 
            this.Wnk_Select.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Wnk_Select.FormattingEnabled = true;
            this.Wnk_Select.ItemHeight = 14;
            this.Wnk_Select.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6"});
            this.Wnk_Select.Location = new System.Drawing.Point(555, 119);
            this.Wnk_Select.Name = "Wnk_Select";
            this.Wnk_Select.Size = new System.Drawing.Size(75, 88);
            this.Wnk_Select.TabIndex = 21;
            this.Wnk_Select.SelectedIndexChanged += new System.EventHandler(this.Axis_Select_SelectedIndexChanged);
            // 
            // Dir_Select
            // 
            this.Dir_Select.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Dir_Select.FormattingEnabled = true;
            this.Dir_Select.ItemHeight = 14;
            this.Dir_Select.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6"});
            this.Dir_Select.Location = new System.Drawing.Point(555, 213);
            this.Dir_Select.Name = "Dir_Select";
            this.Dir_Select.Size = new System.Drawing.Size(75, 88);
            this.Dir_Select.TabIndex = 22;
            this.Dir_Select.SelectedIndexChanged += new System.EventHandler(this.Orig_Select_SelectedIndexChanged);
            // 
            // label_Dir
            // 
            this.label_Dir.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label_Dir.AutoSize = true;
            this.label_Dir.Location = new System.Drawing.Point(493, 213);
            this.label_Dir.Name = "label_Dir";
            this.label_Dir.Size = new System.Drawing.Size(56, 14);
            this.label_Dir.TabIndex = 19;
            this.label_Dir.Text = "Origin:";
            // 
            // num_Layer_Size
            // 
            this.num_Layer_Size.DecimalPlaces = 2;
            this.num_Layer_Size.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.num_Layer_Size.Location = new System.Drawing.Point(12, 93);
            this.num_Layer_Size.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.num_Layer_Size.Name = "num_Layer_Size";
            this.num_Layer_Size.Size = new System.Drawing.Size(75, 22);
            this.num_Layer_Size.TabIndex = 23;
            this.num_Layer_Size.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.num_Layer_Size.ValueChanged += new System.EventHandler(this.Layer_Size_ValueChanged);
            // 
            // label_Layer_Size
            // 
            this.label_Layer_Size.AutoSize = true;
            this.label_Layer_Size.Location = new System.Drawing.Point(93, 95);
            this.label_Layer_Size.Name = "label_Layer_Size";
            this.label_Layer_Size.Size = new System.Drawing.Size(91, 14);
            this.label_Layer_Size.TabIndex = 24;
            this.label_Layer_Size.Text = ": Layer Size";
            // 
            // Point_Lead
            // 
            this.Point_Lead.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Point_Lead.Location = new System.Drawing.Point(493, 307);
            this.Point_Lead.Maximum = new decimal(new int[] {
            4,
            0,
            0,
            0});
            this.Point_Lead.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.Point_Lead.Name = "Point_Lead";
            this.Point_Lead.Size = new System.Drawing.Size(32, 22);
            this.Point_Lead.TabIndex = 25;
            this.Point_Lead.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.Point_Lead.ValueChanged += new System.EventHandler(this.Point_Lead_ValueChanged);
            // 
            // Index_Lead
            // 
            this.Index_Lead.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Index_Lead.Location = new System.Drawing.Point(493, 335);
            this.Index_Lead.Maximum = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.Index_Lead.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.Index_Lead.Name = "Index_Lead";
            this.Index_Lead.Size = new System.Drawing.Size(32, 22);
            this.Index_Lead.TabIndex = 26;
            this.Index_Lead.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.Index_Lead.ValueChanged += new System.EventHandler(this.Index_Lead_ValueChanged);
            // 
            // label_Point_0
            // 
            this.label_Point_0.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label_Point_0.AutoSize = true;
            this.label_Point_0.Location = new System.Drawing.Point(531, 309);
            this.label_Point_0.Name = "label_Point_0";
            this.label_Point_0.Size = new System.Drawing.Size(84, 14);
            this.label_Point_0.TabIndex = 27;
            this.label_Point_0.Text = "Point: 0,00";
            // 
            // label_Index_0
            // 
            this.label_Index_0.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label_Index_0.AutoSize = true;
            this.label_Index_0.Location = new System.Drawing.Point(531, 337);
            this.label_Index_0.Name = "label_Index_0";
            this.label_Index_0.Size = new System.Drawing.Size(63, 14);
            this.label_Index_0.TabIndex = 28;
            this.label_Index_0.Text = "Index: 0";
            // 
            // PolygonCalc
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(642, 420);
            this.Controls.Add(this.label_Index_0);
            this.Controls.Add(this.label_Point_0);
            this.Controls.Add(this.Index_Lead);
            this.Controls.Add(this.Point_Lead);
            this.Controls.Add(this.label_Layer_Size);
            this.Controls.Add(this.num_Layer_Size);
            this.Controls.Add(this.Dir_Select);
            this.Controls.Add(this.Wnk_Select);
            this.Controls.Add(this.label_Dir);
            this.Controls.Add(this.label_Wnk);
            this.Controls.Add(this.label_Layer_Num);
            this.Controls.Add(this.num_Layer_Num);
            this.Controls.Add(this.Offset_reset);
            this.Controls.Add(this.label_Corner_Size);
            this.Controls.Add(this.label_Corner_Num);
            this.Controls.Add(this.label_C);
            this.Controls.Add(this.label_X);
            this.Controls.Add(this.label_Y);
            this.Controls.Add(this.Manual);
            this.Controls.Add(this.Points_Body);
            this.Controls.Add(this.Points_Flat);
            this.Controls.Add(this.Offset_C);
            this.Controls.Add(this.Offset_X);
            this.Controls.Add(this.Offset_Y);
            this.Controls.Add(this.num_Poly_Size);
            this.Controls.Add(this.num_Poly_Num);
            this.Font = new System.Drawing.Font("Noto Mono", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.Name = "PolygonCalc";
            this.Text = "Polygon";
            ((System.ComponentModel.ISupportInitialize)(this.num_Poly_Num)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_Poly_Size)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Offset_Y)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Offset_X)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Offset_C)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_Layer_Num)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_Layer_Size)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Point_Lead)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Index_Lead)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NumericUpDown num_Poly_Num;
        private System.Windows.Forms.NumericUpDown num_Poly_Size;
        private System.Windows.Forms.NumericUpDown Offset_Y;
        private System.Windows.Forms.NumericUpDown Offset_X;
        private System.Windows.Forms.NumericUpDown Offset_C;
        private System.Windows.Forms.RichTextBox Points_Flat;
        private System.Windows.Forms.RichTextBox Points_Body;
        private System.Windows.Forms.Button Manual;
        private System.Windows.Forms.Label label_Y;
        private System.Windows.Forms.Label label_X;
        private System.Windows.Forms.Label label_C;
        private System.Windows.Forms.Label label_Corner_Num;
        private System.Windows.Forms.Label label_Corner_Size;
        private System.Windows.Forms.Button Offset_reset;
        private System.Windows.Forms.NumericUpDown num_Layer_Num;
        private System.Windows.Forms.Label label_Layer_Num;
        private System.Windows.Forms.Label label_Wnk;
        private System.Windows.Forms.ListBox Wnk_Select;
        private System.Windows.Forms.ListBox Dir_Select;
        private System.Windows.Forms.Label label_Dir;
        private System.Windows.Forms.NumericUpDown num_Layer_Size;
        private System.Windows.Forms.Label label_Layer_Size;
        private System.Windows.Forms.NumericUpDown Point_Lead;
        private System.Windows.Forms.NumericUpDown Index_Lead;
        private System.Windows.Forms.Label label_Point_0;
        private System.Windows.Forms.Label label_Index_0;
    }
}