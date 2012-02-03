namespace ModifyEdmx
{
	partial class Form1
	{
		/// <summary>
		/// 必要なデザイナー変数です。
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// 使用中のリソースをすべてクリーンアップします。
		/// </summary>
		/// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows フォーム デザイナーで生成されたコード

		/// <summary>
		/// デザイナー サポートに必要なメソッドです。このメソッドの内容を
		/// コード エディターで変更しないでください。
		/// </summary>
		private void InitializeComponent()
		{
			this.button1 = new System.Windows.Forms.Button();
			this.listBox1 = new System.Windows.Forms.ListBox();
			this.dataGridView1 = new System.Windows.Forms.DataGridView();
			this.button2 = new System.Windows.Forms.Button();
			this.textTableName = new System.Windows.Forms.TextBox();
			this.buttonTableUpdate = new System.Windows.Forms.Button();
			this.button4 = new System.Windows.Forms.Button();
			this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
			this.SuspendLayout();
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(13, 13);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(99, 42);
			this.button1.TabIndex = 0;
			this.button1.Text = "読込";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// listBox1
			// 
			this.listBox1.FormattingEnabled = true;
			this.listBox1.ItemHeight = 12;
			this.listBox1.Location = new System.Drawing.Point(14, 62);
			this.listBox1.Name = "listBox1";
			this.listBox1.Size = new System.Drawing.Size(141, 268);
			this.listBox1.TabIndex = 1;
			this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
			// 
			// dataGridView1
			// 
			this.dataGridView1.AllowUserToAddRows = false;
			this.dataGridView1.AllowUserToDeleteRows = false;
			this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2});
			this.dataGridView1.Location = new System.Drawing.Point(161, 62);
			this.dataGridView1.Name = "dataGridView1";
			this.dataGridView1.RowTemplate.Height = 21;
			this.dataGridView1.Size = new System.Drawing.Size(364, 318);
			this.dataGridView1.TabIndex = 2;
			this.dataGridView1.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.dataGridView1_CellBeginEdit);
			this.dataGridView1.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellValueChanged);
			// 
			// button2
			// 
			this.button2.Location = new System.Drawing.Point(118, 13);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(99, 42);
			this.button2.TabIndex = 3;
			this.button2.Text = "保存";
			this.button2.UseVisualStyleBackColor = true;
			this.button2.Click += new System.EventHandler(this.button2_Click);
			// 
			// textTableName
			// 
			this.textTableName.Location = new System.Drawing.Point(14, 337);
			this.textTableName.Name = "textTableName";
			this.textTableName.Size = new System.Drawing.Size(100, 19);
			this.textTableName.TabIndex = 6;
			// 
			// buttonTableUpdate
			// 
			this.buttonTableUpdate.Location = new System.Drawing.Point(121, 335);
			this.buttonTableUpdate.Name = "buttonTableUpdate";
			this.buttonTableUpdate.Size = new System.Drawing.Size(34, 23);
			this.buttonTableUpdate.TabIndex = 7;
			this.buttonTableUpdate.Text = "更";
			this.buttonTableUpdate.UseVisualStyleBackColor = true;
			this.buttonTableUpdate.Click += new System.EventHandler(this.buttonTableUpdate_Click);
			// 
			// button4
			// 
			this.button4.Location = new System.Drawing.Point(427, 14);
			this.button4.Name = "button4";
			this.button4.Size = new System.Drawing.Size(99, 42);
			this.button4.TabIndex = 8;
			this.button4.Text = "一律変更";
			this.button4.UseVisualStyleBackColor = true;
			this.button4.Click += new System.EventHandler(this.button4_Click);
			// 
			// Column1
			// 
			this.Column1.DataPropertyName = "ColumnName";
			this.Column1.HeaderText = "DB名";
			this.Column1.Name = "Column1";
			this.Column1.ReadOnly = true;
			// 
			// Column2
			// 
			this.Column2.DataPropertyName = "Name";
			this.Column2.HeaderText = "マッピング名";
			this.Column2.Name = "Column2";
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(538, 393);
			this.Controls.Add(this.button4);
			this.Controls.Add(this.buttonTableUpdate);
			this.Controls.Add(this.textTableName);
			this.Controls.Add(this.button2);
			this.Controls.Add(this.dataGridView1);
			this.Controls.Add(this.listBox1);
			this.Controls.Add(this.button1);
			this.Name = "Form1";
			this.Text = "Modify Edmx ver.0.1";
			this.Load += new System.EventHandler(this.Form1_Load);
			((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.ListBox listBox1;
		private System.Windows.Forms.DataGridView dataGridView1;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.TextBox textTableName;
		private System.Windows.Forms.Button buttonTableUpdate;
		private System.Windows.Forms.Button button4;
		private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
		private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
	}
}

