using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using moonmile.ExDoc;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace ModifyEdmx
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			dataGridView1.AutoGenerateColumns = false;
		}

		string _filename = @"Model1.edmx";
		EXDocument _doc = null;
		List<TableMapping> _tables;
		List<ScalarProperty> _columns;
		string _modelName;

		/// <summary>
		/// 読込ボタン
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void button1_Click(object sender, EventArgs e)
		{
			OpenFileDialog dlg = new OpenFileDialog();
			dlg.Filter = "edmx files (*.edmx)|*.edmx|All files (*.*)|*.*";
			dlg.FilterIndex = 0;
			if (dlg.ShowDialog() != DialogResult.OK)
				return;

			_filename = dlg.FileName;
			_doc = new EXDocument();
			_doc.Load(_filename);

			_tables = new List<TableMapping>();
			var els = _doc * "edmx:Mappings" * "EntitySetMapping";
			foreach (var el in els)
			{
				TableMapping tm = new TableMapping();
				_tables.Add(tm);
				tm.EntityName = el / "EntityTypeMapping" % "TypeName";
				tm.EntityName = tm.EntityName.Substring(tm.EntityName.IndexOf('.') + 1);
				tm.EntitySetName = el % "Name";
				tm.StoreName = el / "EntityTypeMapping" / "MappingFragment" % "StoreEntitySet";
			}
			listBox1.DataSource = _tables;

			EXElement el2 = _doc * "EntitySet";
			string name = el2 % "EntityType";
			_modelName = name.Substring(0, name.IndexOf('.') );
		}

		/// <summary>
		/// リスト選択
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (listBox1.SelectedIndex == -1) return;
			TableMapping tm = (TableMapping)listBox1.SelectedItem;
			string name = tm.EntityName;
			EXElement el = _doc * "MappingFragment" % "StoreEntitySet" == tm.StoreName;
			var items = from t in el / "ScalarProperty"
						select new ScalarProperty
						{
							Name = t % "Name",
							ColumnName = t % "ColumnName"
						};
			_columns = items.ToList();
			dataGridView1.DataSource = _columns;

			textTableName.Text = name;
		}

		private void button2_Click(object sender, EventArgs e)
		{
			SaveFileDialog dlg = new SaveFileDialog();
			dlg.DefaultExt = "emdx";
			dlg.Filter = "edmx files (*.edmx)|*.edmx|All files (*.*)|*.*";
			dlg.FilterIndex = 0;
			if (dlg.ShowDialog() != DialogResult.OK)
				return;

			string filename = dlg.FileName;
			_doc.Save( filename);
			MessageBox.Show("保存しました");
		}

		/// <summary>
		/// テーブル名を更新
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void buttonTableUpdate_Click(object sender, EventArgs e)
		{
			int idx = listBox1.SelectedIndex;
			string entityNameOld = _tables[idx].EntityName;
			string entityTypeOld = _modelName + "." + entityNameOld;
			string entityNameNew = textTableName.Text;
			string entityTypeNew = _modelName + "." + entityNameNew;
			EXElement el = _doc * "edmx:ConceptualModels" * "EntitySet" % "EntityType" == entityTypeOld;
			el["Name"] = entityNameNew;
			el["EntityType"] = entityTypeNew;


			el = _doc * "edmx:ConceptualModels" * "EntitySet" % "EntityType" == entityTypeOld;
			el["Name"] = entityNameNew;
			el["EntityType"] = entityTypeNew;
			el = _doc * "edmx:ConceptualModels" * "EntityType" % "Name" == entityNameOld;
			el["Name"] = entityNameNew;

			el = _doc * "edmx:Mappings" * "EntityTypeMapping" % "TypeName" == entityTypeOld;
			el["TypeName"] = entityTypeNew;
			el.Parent["Name"] = entityNameNew;



			el = _doc * "Diagrams" * "EntityTypeShape" % "EntityType" == entityTypeOld;
			el["EntityType"] = entityTypeNew;

			_tables[idx].EntityName = entityNameNew;
			_tables[idx].EntitySetName = entityNameNew;

			listBox1.DataSource = null;
			listBox1.DataSource = _tables;
			listBox1.SelectedIndex = idx;

		}

		private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
		{
			int column = e.ColumnIndex;
			int row = e.RowIndex;
			if (column < 0 || row < 0)
				return;

			var cell = dataGridView1.Rows[row].Cells[column];
			string val = (string)cell.Value;
			Debug.Print("{0},{1} {2} {3}", row, column, _oldProperty.ColumnName, val);

			// 該当箇所を変更
			ChangeScalarProp(_oldProperty.Name, val);
		}

		/// <summary>
		/// カラム名を変更
		/// </summary>
		/// <param name="oldName"></param>
		/// <param name="newName"></param>
		private void ChangeScalarProp( string oldName, string newName)
		{
			string entityName = _tables[listBox1.SelectedIndex].EntityName;
			EXElement el = _doc * "edmx:ConceptualModels" * "EntityType" % "Name" == entityName;

			EXElement elref = el * "PropertyRef" % "Name" == oldName;
			if (EXDocument.IsEmpty(elref) == false)
			{
				elref["Name"] = newName;
			}
			EXElement elprop = el * "Property" % "Name" == oldName;
			elprop["Name"] = newName;

			string storeName = _tables[listBox1.SelectedIndex].StoreName;
			el = _doc * "edmx:Mappings" * "MappingFragment" % "StoreEntitySet" == storeName;
			elprop = el / "ScalarProperty" % "Name" == oldName;
			elprop["Name"] = newName;
		}

		/// <summary>
		/// 編集前の情報を保持
		/// </summary>
		ScalarProperty _oldProperty;
		/// <summary>
		/// 編集前の情報を保持する
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void dataGridView1_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
		{
			int column = e.ColumnIndex;
			int row = e.RowIndex;
			if (column < 0 || row < 0)
				return;

			_oldProperty = new ScalarProperty()
			{
				Name = _columns[row].Name,
				ColumnName = _columns[row].ColumnName
			};
		}

		/// <summary>
		/// 列名を一律で変更
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void button4_Click(object sender, EventArgs e)
		{
			foreach (ScalarProperty prop in _columns)
			{
				string oldName = prop.Name;
				string newName = oldName;
				newName = newName.Substring(0, 1).ToUpper() + newName.Substring(1);
				while (true)
				{
					int i = newName.IndexOf('_');
					if (i < 0) break;
					newName = newName.Substring(0, i ) +
						newName.Substring(i + 1, 1).ToUpper() +
						newName.Substring(i + 2);
				}
				ChangeScalarProp(oldName, newName);
				prop.Name = newName;
			}
			dataGridView1.DataSource = null;
			dataGridView1.DataSource = _columns;
		}
	}


	/// <summary>
	/// テーブルマッピング
	/// </summary>
	public class TableMapping
	{
		public string StoreName { get; set; }		// 元テーブルの名前
		public string EntityName { get; set; }		// エンティティの名前
		public string EntitySetName { get; set; }	// エンティティセットの名前
		// 表示用
		public override string ToString()
		{
			return string.Format("{0}({1})", EntityName, StoreName);
		}
	}


	/// <summary>
	/// カラム名マッピングクラス
	/// </summary>
	public class ScalarProperty
	{
		public string Name { get; set; }
		public string ColumnName { get; set; }
	}
}
