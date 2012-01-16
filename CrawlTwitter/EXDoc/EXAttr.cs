using System;
using System.Net;
using System.Collections.Generic;

namespace moonmile.ExDoc
{
	public class EXAttr : EXNode
	{
		internal protected EXElement _exElement = null;

		public string Name { get; protected set; }
		public string Value { get; set; }

		public EXAttr(EXElement el, string name)
		{
			if (el != EXDocument._emptyElement)
			{
				this._exElement = el;
				this.Name = name;
			}
			else
			{
				this._exElement = EXDocument._emptyElement;
				this.Name = "";
				this.Value = "";
			}
		}
		/// <summary>
		/// string 型へ暗黙のキャスト
		/// </summary>
		/// <param name="at"></param>
		/// <returns></returns>
		public static implicit operator string(EXAttr at)
		{
			return at.Value;
		}
	}
	public class EXAttrs : List<EXAttr>
	{
		public static EXElements operator ==(EXAttrs ats, string val)
		{
			EXElements items = new EXElements();
			foreach (EXAttr at in ats)
			{
				if (at.Value == val)
				{
					items.Add(at._exElement);
				}
			}
			return items;
		}
		public static EXElements operator !=(EXAttrs ats, string val)
		{
			EXElements items = new EXElements();
			foreach (EXAttr at in ats)
			{
				if (at.Value != val)
				{
					items.Add(at._exElement);
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
		/// 文字列へキャスト
		/// </summary>
		/// <param name="ats"></param>
		/// <returns></returns>
		public static implicit operator string(EXAttrs ats)
		{
			return (ats.Count > 0) ? ats[0].Value : "";
		}

		public static implicit operator string[](EXAttrs ats)
		{
			List<string> items = new List<string>();
			if (ats.Count > 0)
			{
				foreach (EXAttr at in ats)
				{
					items.Add(at.Value);
				}
			}
			return items.ToArray();
		}

		/// <summary>
		/// EXAttr クラスへキャスト
		/// </summary>
		/// <param name="ats"></param>
		/// <returns></returns>
		public static implicit operator EXAttr(EXAttrs ats)
		{
			if (ats.Count > 0)
			{
				return ats[0];
			}
			else
			{
				return EXDocument._emptyAttr;
			}
		}
	}
}
