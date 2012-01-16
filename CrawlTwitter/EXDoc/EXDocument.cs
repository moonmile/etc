using System;
using System.Net;
using System.Xml;
using System.IO;
using System.Collections.Generic;

namespace moonmile.ExDoc
{
	public class EXDocument : EXNode
	{
		public EXElement DocumentElement { get; protected set; }
		internal protected static EXElement _emptyElement;
		internal protected static EXAttr _emptyAttr;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public EXDocument()
		{
			_emptyElement = new EXElement(this);
			_emptyAttr = new EXAttr(_emptyElement, "");
			_emptyAttr.Value = "";
		}

        /// <summary>
        /// ファイルからドキュメントを作成する
        /// </summary>
        /// <param name="path"></param>
		public void Load(string path)
		{
			StreamReader sr = new StreamReader(new FileStream(path, FileMode.Open));
			string xml = sr.ReadToEnd();
			sr.Close();
			LoadXML(xml);
		}

		/// <summary>
		/// 文字列からドキュメントを作成する
		/// </summary>
		/// <param name="xml"></param>
		public void LoadXML(string xml)
		{
			using (XmlReader reader = XmlReader.Create(new StringReader(xml)))
			{
				// ルート要素までジャンプ
				while (reader.Read())
				{
					if (reader.NodeType == XmlNodeType.Element)
					{
						EXElement root = new EXElement(this);
						this.DocumentElement = root;
						root.Name = reader.Name;
						root.Value = "";
						root.Parent = EXDocument._emptyElement;
						LoadXML(reader, root);
					}
				}
			}
		}
		/// <summary>
		/// 補助関数
		/// </summary>
		/// <param name="reader"></param>
		/// <param name="pa"></param>
		protected void LoadXML(XmlReader reader, EXElement pa)
		{
			EXElement el = null;
			while (reader.Read())
			{
				switch (reader.NodeType)
				{
					case XmlNodeType.Element:
						el = new EXElement(pa.Document);
						el.Name = reader.Name;
						pa.ChildNodes.Add(el);
						el.Parent = pa;
						if (reader.HasAttributes)
						{
							if (reader.HasAttributes)
							{
								for (int i = 0; i < reader.AttributeCount; i++)
								{
									reader.MoveToAttribute(i);
									EXAttr attr = new EXAttr(el, reader.Name);
									attr.Value = reader.Value;
									el.Attributes.Add(attr.Name, attr);
								}
							}
							reader.MoveToElement();
						}
						LoadXML(reader, el);
						break;
					case XmlNodeType.EndElement:
						return;
					case XmlNodeType.Text:
						pa.Value = reader.Value;
						break;
				}
			}
		}

		/// <summary>
		/// ルート要素を取得する
		/// </summary>
		public EXElement Root
		{
			get { return this.DocumentElement; }
			set { this.DocumentElement = value; }
		}

		/// <summary>
		/// 要素を作成する
		/// </summary>
		/// <returns></returns>
		public EXElement CreateElement()
		{
			EXElement el = new EXElement(this);
			return el;
		}

		/// <summary>
		/// 子要素を探索する
		/// </summary>
		/// <param name="doc"></param>
		/// <param name="s"></param>
		/// <returns></returns>
		public static EXElements operator /(EXDocument doc, string tag)
		{
			EXElements items = new EXElements();
			if (doc.DocumentElement != null)
			{
				if (doc.DocumentElement.Name == tag)
				{
					items.Add(doc.DocumentElement);
				}
			}
			return items;
		}

		/// <summary>
		/// 子孫要素を探索する
		/// </summary>
		/// <param name="doc"></param>
		/// <param name="tag"></param>
		/// <returns></returns>
		public static EXElements operator *(EXDocument doc, string tag)
		{
			EXElements items = new EXElements();
			if (doc.DocumentElement != null)
			{
				if (doc.DocumentElement.Name == tag)
				{
					items.Add(doc.DocumentElement);
				}
				items.AddRange( doc.DocumentElement.selectNodes(tag, true));
			}
			return items ;
		}
	}
}
