using UnityEngine;

namespace Sirenix.Utilities
{
	public static class RectExtensions
	{
		public static Rect SetWidth(this Rect rect, float width)
		{
			rect.width = width;
			return rect;
		}

		public static Rect SetHeight(this Rect rect, float height)
		{
			rect.height = height;
			return rect;
		}

		public static Rect SetSize(this Rect rect, float width, float height)
		{
			rect.width = width;
			rect.height = height;
			return rect;
		}

		public static Rect SetSize(this Rect rect, Vector2 size)
		{
			rect.size = size;
			return rect;
		}

		public static Rect HorizontalPadding(this Rect rect, float padding)
		{
			rect.x += padding;
			rect.width -= padding * 2f;
			return rect;
		}

		public static Rect HorizontalPadding(this Rect rect, float left, float right)
		{
			rect.x += left;
			rect.width -= left + right;
			return rect;
		}

		public static Rect VerticalPadding(this Rect rect, float padding)
		{
			rect.y += padding;
			rect.height -= padding * 2f;
			return rect;
		}

		public static Rect VerticalPadding(this Rect rect, float top, float bottom)
		{
			rect.y += top;
			rect.height -= top + bottom;
			return rect;
		}

		public static Rect Padding(this Rect rect, float padding)
		{
			rect.position += new Vector2(padding, padding);
			rect.size -= new Vector2(padding, padding) * 2f;
			return rect;
		}

		public static Rect Padding(this Rect rect, float horizontal, float vertical)
		{
			rect.position += new Vector2(horizontal, vertical);
			rect.size -= new Vector2(horizontal, vertical) * 2f;
			return rect;
		}

		public static Rect Padding(this Rect rect, float left, float right, float top, float bottom)
		{
			rect.position += new Vector2(left, top);
			rect.size -= new Vector2(left + right, top + bottom);
			return rect;
		}

		public static Rect AlignLeft(this Rect rect, float width)
		{
			rect.width = width;
			return rect;
		}

		public static Rect AlignCenter(this Rect rect, float width)
		{
			rect.x = rect.x + rect.width * 0.5f - width * 0.5f;
			rect.width = width;
			return rect;
		}

		public static Rect AlignCenter(this Rect rect, float width, float height)
		{
			rect.x = rect.x + rect.width * 0.5f - width * 0.5f;
			rect.y = rect.y + rect.height * 0.5f - height * 0.5f;
			rect.width = width;
			rect.height = height;
			return rect;
		}

		public static Rect AlignRight(this Rect rect, float width)
		{
			rect.x = rect.x + rect.width - width;
			rect.width = width;
			return rect;
		}

		public static Rect AlignTop(this Rect rect, float height)
		{
			rect.height = height;
			return rect;
		}

		public static Rect AlignMiddle(this Rect rect, float height)
		{
			rect.y = rect.y + rect.height * 0.5f - height * 0.5f;
			rect.height = height;
			return rect;
		}

		public static Rect AlignBottom(this Rect rect, float height)
		{
			rect.y = rect.y + rect.height - height;
			rect.height = height;
			return rect;
		}

		public static Rect Expand(this Rect rect, float expand)
		{
			rect.position -= new Vector2(expand, expand);
			rect.size += new Vector2(expand, expand) * 2f;
			return rect;
		}

		public static Rect Expand(this Rect rect, float horizontal, float vertical)
		{
			rect.position -= new Vector2(horizontal, vertical);
			rect.size += new Vector2(horizontal, vertical) * 2f;
			return rect;
		}

		public static Rect Expand(this Rect rect, float left, float right, float top, float bottom)
		{
			rect.position -= new Vector2(left, top);
			rect.size += new Vector2(left + right, top + bottom);
			return rect;
		}

		public static Rect Split(this Rect rect, int index, int count)
		{
			int num = (int)rect.width % count;
			int num2 = (int)(rect.width / (float)count);
			rect.width = (float)(num2 + ((index < num) ? 1 : 0)) + ((index + 1 == count) ? (rect.width - (float)(int)rect.width) : 0f);
			if (index > 0)
			{
				rect.x += num2 * index + (num - (count - 1 - index));
			}
			return rect;
		}

		public static Rect SplitVertical(this Rect rect, int index, int count)
		{
			int num = (int)(rect.height / (float)count);
			rect.height = num;
			rect.y += num * index;
			return rect;
		}

		public static Rect SplitGrid(this Rect rect, float width, float height, int index)
		{
			int num = (int)(rect.width / width);
			int num2 = index / num;
			int num3 = index % num;
			rect.x += (float)num2 * width;
			rect.y += (float)num3 * height;
			rect.width = width;
			rect.height = height;
			return rect;
		}

		public static Rect SetCenterX(this Rect rect, float x)
		{
			rect.center = new Vector2(x, rect.center.y);
			return rect;
		}

		public static Rect SetCenterY(this Rect rect, float y)
		{
			rect.center = new Vector2(rect.center.x, y);
			return rect;
		}

		public static Rect SetCenter(this Rect rect, float x, float y)
		{
			rect.center = new Vector2(x, y);
			return rect;
		}

		public static Rect SetCenter(this Rect rect, Vector2 center)
		{
			rect.center = center;
			return rect;
		}

		public static Rect SetPosition(this Rect rect, Vector2 position)
		{
			rect.position = position;
			return rect;
		}

		public static Rect ResetPosition(this Rect rect)
		{
			rect.position = Vector2.zero;
			return rect;
		}

		public static Rect AddPosition(this Rect rect, Vector2 move)
		{
			rect.position += move;
			return rect;
		}

		public static Rect SetX(this Rect rect, float x)
		{
			rect.x = x;
			return rect;
		}

		public static Rect AddX(this Rect rect, float x)
		{
			rect.x += x;
			return rect;
		}

		public static Rect SubX(this Rect rect, float x)
		{
			rect.x -= x;
			return rect;
		}

		public static Rect SetY(this Rect rect, float y)
		{
			rect.y = y;
			return rect;
		}

		public static Rect AddY(this Rect rect, float y)
		{
			rect.y += y;
			return rect;
		}

		public static Rect SubY(this Rect rect, float y)
		{
			rect.y -= y;
			return rect;
		}

		public static Rect SetMin(this Rect rect, Vector2 min)
		{
			rect.min = min;
			return rect;
		}

		public static Rect AddMin(this Rect rect, Vector2 value)
		{
			rect.min += value;
			return rect;
		}

		public static Rect SubMin(this Rect rect, Vector2 value)
		{
			rect.min -= value;
			return rect;
		}

		public static Rect SetMax(this Rect rect, Vector2 max)
		{
			rect.max = max;
			return rect;
		}

		public static Rect AddMax(this Rect rect, Vector2 value)
		{
			rect.max += value;
			return rect;
		}

		public static Rect SubMax(this Rect rect, Vector2 value)
		{
			rect.max -= value;
			return rect;
		}

		public static Rect SetXMin(this Rect rect, float xMin)
		{
			rect.xMin = xMin;
			return rect;
		}

		public static Rect AddXMin(this Rect rect, float value)
		{
			rect.xMin += value;
			return rect;
		}

		public static Rect SubXMin(this Rect rect, float value)
		{
			rect.xMin -= value;
			return rect;
		}

		public static Rect SetXMax(this Rect rect, float xMax)
		{
			rect.xMax = xMax;
			return rect;
		}

		public static Rect AddXMax(this Rect rect, float value)
		{
			rect.xMax += value;
			return rect;
		}

		public static Rect SubXMax(this Rect rect, float value)
		{
			rect.xMax -= value;
			return rect;
		}

		public static Rect SetYMin(this Rect rect, float yMin)
		{
			rect.yMin = yMin;
			return rect;
		}

		public static Rect AddYMin(this Rect rect, float value)
		{
			rect.yMin += value;
			return rect;
		}

		public static Rect SubYMin(this Rect rect, float value)
		{
			rect.yMin -= value;
			return rect;
		}

		public static Rect SetYMax(this Rect rect, float yMax)
		{
			rect.yMax = yMax;
			return rect;
		}

		public static Rect AddYMax(this Rect rect, float value)
		{
			rect.yMax += value;
			return rect;
		}

		public static Rect SubYMax(this Rect rect, float value)
		{
			rect.yMax -= value;
			return rect;
		}

		public static Rect MinWidth(this Rect rect, float minWidth)
		{
			rect.width = Mathf.Max(rect.width, minWidth);
			return rect;
		}

		public static Rect MaxWidth(this Rect rect, float maxWidth)
		{
			rect.width = Mathf.Min(rect.width, maxWidth);
			return rect;
		}

		public static Rect MinHeight(this Rect rect, float minHeight)
		{
			rect.height = Mathf.Max(rect.height, minHeight);
			return rect;
		}

		public static Rect MaxHeight(this Rect rect, float maxHeight)
		{
			rect.height = Mathf.Min(rect.height, maxHeight);
			return rect;
		}

		public static Rect ExpandTo(this Rect rect, Vector2 pos)
		{
			if (pos.x < rect.xMin)
			{
				rect.xMin = pos.x;
			}
			else if (pos.x > rect.xMax)
			{
				rect.xMax = pos.x;
			}
			if (pos.y < rect.yMin)
			{
				rect.yMin = pos.y;
			}
			else if (pos.y > rect.yMax)
			{
				rect.yMax = pos.y;
			}
			return rect;
		}
	}
}
