using System;
using System.Net;
using System.Collections.Generic;

namespace moonmile.ExDoc
{
	/// <summary>
	/// 仮のノードクラス
	/// </summary>
	public class EXNode
	{
	}


	/// <summary>
	/// XMLエレメント
	/// </summary>
	public class EXElement : EXNode
	{
		internal protected EXDocument _document = null;
		public Dictionary<string, EXAttr> Attributes { get; set; }
		public string Name { get; internal protected set; }
		public string Value { get; set; }
		public List<EXElement> ChildNodes;

		public EXDocument Document
		{
			get { return _document; }
		}

		protected EXElement()
		{
		}

		public EXElement(EXDocument doc)
		{
			_document = doc;
			Attributes = new Dictionary<string, EXAttr>();
			ChildNodes = new List<EXElement>();
		}
		/// <summary>
		/// 要素の値を指定して取得
		/// </summary>
		/// <param name="els"></param>
		/// <param name="tag"></param>
		/// <returns></returns>
		public static EXElements operator /(EXElement el, string tag)
		{
			EXElements items = el.selectNodes(tag);
			return items;
		}
		public static EXElements operator *(EXElement el, string tag)
		{
			EXElements items = el.selectNodes(tag, true);
			return items;
		}
		internal protected EXElements selectNodes(string tag, bool childSeek = false)
		{
			EXElements items = new EXElements();
			foreach (EXElement el in this.ChildNodes)
			{
				if (el.Name == tag)
				{
					items.Add(el);
				}
				if (childSeek == true)
				{
					items.AddRange( el.selectNodes( tag, true ));
				}
			}
			return items;
		}
		/// <summary>
		/// string 型へ暗黙のキャスト
		/// </summary>
		/// <param name="el"></param>
		/// <returns></returns>
		public static implicit operator string(EXElement el)
		{
			return el.Value ?? "";
		}
		/// <summary>
		/// 属性の取得
		/// </summary>
		/// <param name="el"></param>
		/// <param name="attrName"></param>
		/// <returns></returns>
		public static string operator %(EXElement el, string attrName)
		{
			return el[attrName];
		}
		/// <summary>
		/// 属性名を指定して値を取得
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public string this[string key]
		{
			get
			{
				if (this.Attributes.ContainsKey(key) == true)
				{
					return this.Attributes[key].Value;
				}
				else
				{
					return "";
				}
			}
			set
			{
				if (this.Attributes.ContainsKey(key) == true)
				{
					this.Attributes[key].Value = value;
				}
				else
				{
					EXAttr attr = new EXAttr(this, key);
					attr.Value = value;
					this.Attributes.Add(key, attr);
				}
			}
		}

		/// <summary>
		/// 親要素を取得する
		/// </summary>
		public EXElement Parent { get; internal protected set; }

		/// <summary>
		/// 親要素を取得する --演算子
		/// </summary>
		/// <param name="el"></param>
		/// <returns></returns>
		public static EXElement operator --(EXElement el)
		{
			return el.Parent;
		}
	}

	/// <summary>
    /// XMLエレメントクラスのコレクション
    /// </summary>
	public class EXElements : List<EXElement>
	{
		/// <summary>
		/// 要素の値を指定して取得
		/// </summary>
		/// <param name="els"></param>
		/// <param name="tag"></param>
		/// <returns></returns>
		public static EXElements operator /(EXElements els, string tag)
		{
			EXElements items = new EXElements();
			foreach ( EXElement el in els ) {
				var lst = el.selectNodes(tag);
				items.AddRange( lst );
			}
			return items;
		}
		/// <summary>
		/// 子孫の要素の値を指定して取得
		/// </summary>
		/// <param name="els"></param>
		/// <param name="tag"></param>
		/// <returns></returns>
		public static EXElements operator *(EXElements els, string tag)
		{
			EXElements items = new EXElements();
			foreach (EXElement el in els)
			{
				var lst = el.selectNodes(tag, true);
				items.AddRange(lst);
			}
			return items;
		}
		/// <summary>
		/// EXElement クラスに暗黙のキャスト
		/// </summary>
		/// <param name="els"></param>
		/// <returns></returns>
		public static implicit operator EXElement(EXElements els)
		{
			if (els.Count > 0)
				return els[0];
			else
				return EXDocument._emptyElement;
		}
		/// <summary>
		/// 比較演算子
		/// </summary>
		/// <param name="els"></param>
		/// <param name="val"></param>
		/// <returns></returns>
		public static EXElements operator ==(EXElements els, string val)
		{
			EXElements items = new EXElements();
			foreach (EXElement el in els)
			{
				if (el.Value == val)
				{
					items.Add(el);
				}
			}
			return items;
		}
		public static EXElements operator !=(EXElements els, string val)
		{
			EXElements items = new EXElements();
			foreach (EXElement el in els)
			{
				if (el.Value != val)
				{
					items.Add(el);
				}
			}
			return items;
		}
		public override bool Equals(object obj)
		{
			return base.Equals(obj);
		}
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		/// <summary>
		/// 属性を指定して取得
		/// </summary>
		/// <param name="els"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		public static EXAttrs operator %(EXElements els, string name)
		{
			EXAttrs attrs = new EXAttrs();
			foreach (EXElement el in els)
			{
				if (el.Attributes.ContainsKey(name))
				{
					attrs.Add(el.Attributes[name]);
				}
			}
			return attrs;
		}
		/// <summary>
		/// 文字列にキャスト
		/// </summary>
		/// <param name="els"></param>
		/// <returns></returns>
		public static implicit operator string(EXElements els)
		{
			return els.Count > 0 ? els[0].Value : "";
		}
		/// <summary>
		/// 文字列の配列にキャスト
		/// </summary>
		/// <param name="els"></param>
		/// <param name="tag"></param>
		/// <returns></returns>
		public static implicit operator string[](EXElements els)
		{
			List<string> items = new List<string>();
			if (els.Count > 0)
			{
				foreach (EXElement el in els)
				{
					items.Add(el.Value);
				}
			}
			return items.ToArray();
		}
	}
}
