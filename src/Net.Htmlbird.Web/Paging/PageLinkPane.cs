// ===============================================================================
//  产品名称：网鸟电子商务管理系统(Htmlbird ECMS)
//  产品作者：YMind Chan
//  版权所有：网鸟IT技术论坛 颜铭工作室
// ===============================================================================
//  Copyright © Htmlbird.Net. All rights reserved .
//  官方网站：http://www.htmlbird.net/
//  技术论坛：http://bbs.htmlbird.net/
// ===============================================================================
using System;
using System.Text;

namespace Net.Htmlbird.Framework.Web.Paging
{
	/// <summary>
	/// 表示以方格链接的形式提供翻页链接模块的构造器。
	/// </summary>
	public sealed class PageLinkPane
	{
		#region 私有字段

		private readonly string _url;
		private string _text;

		#endregion

		#region 构造函数

		/// <summary>
		/// 初始化 <see cref="PageLinkPane"/> 类的新实例。
		/// </summary>
		/// <param name="url">包含翻页链接按钮所关联的链接的字符串。此字符串必须包含“[#PageIndex#]”并用其来描述翻页索引数字。</param>
		/// <param name="paginationArgs">包含构造翻页链接需要的参数的 <see cref="PaginationArgs"/>。</param>
		public PageLinkPane(string url, PaginationArgs paginationArgs)
		{
			this._url = url ?? String.Empty;
			this._url = HtmlbirdECMS.SystemInfo.ApplicationSetupPath + this._url.TrimStart('/');
			this.TotalItemCount = paginationArgs.TotalItemCount <= 1 ? 1 : paginationArgs.TotalItemCount;
			this.TotalPageCount = paginationArgs.TotalPageCount <= 1 ? 1 : paginationArgs.TotalPageCount;
			this.CurrentPageIndex = paginationArgs.CurrentPageIndex > this.TotalPageCount ? this.TotalPageCount : (paginationArgs.CurrentPageIndex <= 1 ? 1 : paginationArgs.CurrentPageIndex);
			this.VisibleLinkCount = paginationArgs.VisibleLinkCount <= 2 ? 2 : paginationArgs.VisibleLinkCount;
			this.VisibleLinkIndex = this.VisibleLinkCount + 1;

			this.FirstPageIndex = 1;
			this.PreviousPageIndex = this.CurrentPageIndex - 1;
			this.PreviousPageIndex = this.PreviousPageIndex <= 1 ? 1 : this.PreviousPageIndex;
			this.NextPageIndex = this.CurrentPageIndex + 1;
			this.NextPageIndex = this.NextPageIndex > this.TotalPageCount ? this.TotalPageCount : this.NextPageIndex;
			this.LastPageIndex = this.TotalPageCount;
		}

		#endregion

		#region 私有方法

		private void _Process()
		{
			this.FirstPageUrlText = this._url.Replace("[#PageIndex#]", this.FirstPageIndex.ToString());
			this.PreviousPageUrlText = this._url.Replace("[#PageIndex#]", this.PreviousPageIndex.ToString());
			this.CurrentPageUrlText = this._url.Replace("[#PageIndex#]", this.CurrentPageIndex.ToString());
			this.NextPageUrlText = this._url.Replace("[#PageIndex#]", this.NextPageIndex.ToString());
			this.LastPageUrlText = this._url.Replace("[#PageIndex#]", this.LastPageIndex.ToString());

			StringBuilder htmlBuilder = new StringBuilder();
			int index;

			htmlBuilder.Append("<div class=\"pc\"><ul>");

			// 总数信息
			htmlBuilder.AppendFormat("<li>共 {0} 项&nbsp;</li>", this.TotalItemCount);

			if (this.TotalPageCount > this.VisibleLinkCount)
			{
				// 上一页
				if (this.PreviousPageIndex > this.FirstPageIndex && this.PreviousPageIndex < this.CurrentPageIndex) htmlBuilder.AppendFormat("<li><a href=\"{0}\">&#171; 上一页</a></li>", this.PreviousPageUrlText);

				// 首页
				if (this.CurrentPageIndex > this.FirstPageIndex && this.CurrentPageIndex > this.VisibleLinkCount + 1)
				{
					htmlBuilder.AppendFormat("<li><a href=\"{0}\">{1}</a></li>", this.FirstPageUrlText, this.FirstPageIndex);

					if (this.CurrentPageIndex > this.FirstPageIndex + this.VisibleLinkCount + 1) htmlBuilder.Append("<li>...</li>");
				}
			}

			// 前置按钮
			for (int i = 0; i < this.VisibleLinkCount; i++)
			{
				index = this.CurrentPageIndex - this.VisibleLinkCount + i;

				if (index < this.FirstPageIndex) continue;
				if (index == this.CurrentPageIndex) break;

				htmlBuilder.AppendFormat("<li><a href=\"{0}\">{1}</a></li>", this._url.Replace("[#PageIndex#]", index.ToString()), index);
			}

			// 当前按钮
			if (this.TotalPageCount > 1) htmlBuilder.AppendFormat("<li><a href=\"{0}\" class=\"focus\">{1}</a></li>", this.CurrentPageUrlText, this.CurrentPageIndex);

			// 后续按钮
			for (int i = 0; i <= this.VisibleLinkCount; i++)
			{
				index = this.CurrentPageIndex + i;

				if (index > this.LastPageIndex) break;
				if (index == this.CurrentPageIndex) continue;

				htmlBuilder.AppendFormat("<li><a href=\"{0}\">{1}</a></li>", this._url.Replace("[#PageIndex#]", index.ToString()), index);
			}

			if (this.TotalPageCount > this.VisibleLinkCount)
			{
				// 尾页
				if (this.CurrentPageIndex < this.LastPageIndex && this.CurrentPageIndex < this.LastPageIndex - this.VisibleLinkCount)
				{
					if (this.CurrentPageIndex < this.LastPageIndex - this.VisibleLinkCount - 1) htmlBuilder.Append("<li>...</li>");

					htmlBuilder.AppendFormat("<li><a href=\"{0}\">{1}</a></li>", this.LastPageUrlText, this.LastPageIndex);
				}

				// 下一页
				if (this.NextPageIndex < this.LastPageIndex && this.LastPageIndex > this.CurrentPageIndex) htmlBuilder.AppendFormat("<li><a href=\"{0}\">下一页 &#187;</a></li>", this.NextPageUrlText);
			}

			htmlBuilder.Append("</ul></div>");

			this._text = htmlBuilder.ToString();
		}

		#endregion

		#region 公有方法

		/// <summary>
		/// 返回当前 <see cref="PageLinkPane"/> 对象的字符串表示形式。
		/// </summary>
		/// <returns>返回表示当前 <see cref="PageLinkPane"/> 对象的字符串。</returns>
		public override string ToString()
		{
			this._Process();

			return this._text;
		}

		#endregion

		#region 公有属性

		/// <summary>
		/// 获取翻页链接描述的页面数据的所有项数。
		/// </summary>
		public int TotalItemCount { get; private set; }

		/// <summary>
		/// 获取翻页链接描述的页面数据的所有页数。
		/// </summary>
		public int TotalPageCount { get; private set; }

		/// <summary>
		/// 获取翻页链接第一页的索引数字。
		/// </summary>
		public int FirstPageIndex { get; private set; }

		/// <summary>
		/// 获取翻页链接上一页的索引数字。
		/// </summary>
		public int PreviousPageIndex { get; private set; }

		/// <summary>
		/// 获取翻页链接下一页的索引数字。
		/// </summary>
		public int NextPageIndex { get; private set; }

		/// <summary>
		/// 获取翻页链接最后一页的索引数字。
		/// </summary>
		public int LastPageIndex { get; private set; }

		/// <summary>
		/// 获取翻页链接当前页面的索引数字。
		/// </summary>
		public int CurrentPageIndex { get; private set; }

		/// <summary>
		/// 获取翻页链接当前高亮显示的可见按钮的前后按钮的最大项数。
		/// </summary>
		public int VisibleLinkCount { get; private set; }

		/// <summary>
		/// 获取翻页链接当前可见按钮中高亮显示的索引数字（第 N 个）。
		/// </summary>
		public int VisibleLinkIndex { get; private set; }

		/// <summary>
		/// 获取翻页链接中表示第一页的链接的相对路径的字符串。
		/// </summary>
		public string FirstPageUrlText { get; private set; }

		/// <summary>
		/// 获取翻页链接中表示上一页的链接的相对路径的字符串。
		/// </summary>
		public string PreviousPageUrlText { get; private set; }

		/// <summary>
		/// 获取翻页链接中表示下一页的链接的相对路径的字符串。
		/// </summary>
		public string NextPageUrlText { get; private set; }

		/// <summary>
		/// 获取翻页链接中表示最后一页的链接的相对路径的字符串。
		/// </summary>
		public string LastPageUrlText { get; private set; }

		/// <summary>
		/// 获取翻页链接中表示当前页面的链接的相对路径的字符串。
		/// </summary>
		public string CurrentPageUrlText { get; private set; }

		#endregion
	}
}