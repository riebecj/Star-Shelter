using System;

namespace Sirenix.OdinInspector
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	public sealed class ListDrawerSettingsAttribute : Attribute
	{
		private string onTitleBarGUI;

		private int numberOfItemsPerPage;

		private bool paging;

		private bool draggable;

		private bool isReadOnly;

		private bool showItemCount;

		private bool pagingHasValue;

		private bool draggableHasValue;

		private bool isReadOnlyHasValue;

		private bool showItemCountHasValue;

		private bool expanded;

		private bool expandedHasValue;

		private bool numberOfItemsPerPageHasValue;

		private bool showIndexLabels;

		private bool showIndexLabelsHasValue;

		public bool ShowPaging
		{
			get
			{
				return paging;
			}
			set
			{
				paging = value;
				pagingHasValue = true;
			}
		}

		public bool DraggableItems
		{
			get
			{
				return draggable;
			}
			set
			{
				draggable = value;
				draggableHasValue = true;
			}
		}

		public int NumberOfItemsPerPage
		{
			get
			{
				return numberOfItemsPerPage;
			}
			set
			{
				numberOfItemsPerPage = value;
				numberOfItemsPerPageHasValue = true;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return isReadOnly;
			}
			set
			{
				isReadOnly = value;
				isReadOnlyHasValue = true;
			}
		}

		public bool ShowItemCount
		{
			get
			{
				return showItemCount;
			}
			set
			{
				showItemCount = value;
				showItemCountHasValue = true;
			}
		}

		public bool Expanded
		{
			get
			{
				return expanded;
			}
			set
			{
				expanded = value;
				expandedHasValue = true;
			}
		}

		public bool ShowIndexLabels
		{
			get
			{
				return showIndexLabels;
			}
			set
			{
				showIndexLabels = value;
				showIndexLabelsHasValue = true;
			}
		}

		public string OnTitleBarGUI
		{
			get
			{
				return onTitleBarGUI;
			}
			set
			{
				onTitleBarGUI = value;
			}
		}

		public bool AlwaysAddDefaultValue { get; set; }

		public bool HideAddButton { get; set; }

		public string ListElementLabelName { get; set; }

		public string OnBeginListElementGUI { get; set; }

		public string OnEndListElementGUI { get; set; }

		public bool PagingHasValue
		{
			get
			{
				return pagingHasValue;
			}
		}

		public bool ShowItemCountHasValue
		{
			get
			{
				return showItemCountHasValue;
			}
		}

		public bool NumberOfItemsPerPageHasValue
		{
			get
			{
				return numberOfItemsPerPageHasValue;
			}
		}

		public bool DraggableHasValue
		{
			get
			{
				return draggableHasValue;
			}
		}

		public bool IsReadOnlyHasValue
		{
			get
			{
				return isReadOnlyHasValue;
			}
		}

		public bool ExpandedHasValue
		{
			get
			{
				return expandedHasValue;
			}
		}

		public bool ShowIndexLabelsHasValue
		{
			get
			{
				return showIndexLabelsHasValue;
			}
		}
	}
}
