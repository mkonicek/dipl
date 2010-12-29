using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace ClassDiagrams
{
	public class VirtualizingCollection<T> : IList<T>
	{
		private Dictionary<int, T> itemCache = new Dictionary<int, T>();

		public int Count
		{
			get { return this.ValueProvider.GetCount(); }
		}

		public T this[int index]
		{
			get
			{
				T cachedItem;
				if (!itemCache.TryGetValue(index, out cachedItem))
				{
					cachedItem = this.ValueProvider.GetItemAt(index);
					this.itemCache[index] = cachedItem;
				}
				return cachedItem;
			}
			set { }
		}

		public IListValuesProvider<T> ValueProvider
		{
			get
			{
				throw new System.NotImplementedException();
			}
			set
			{
			}
		}

		#region IList<T> Members


		#endregion

		#region ICollection<T> Members


		#endregion

		#region IEnumerable<T> Members


		#endregion

		#region IEnumerable Members


		#endregion
	}

	public interface IListValuesProvider<T>
	{
		int GetCount();
		T GetItemAt(int index);
	}

	public interface IEnumerableValuesProvider<T>
	{
		T GetNextItem();
	}

	public class ItemsControl
	{



		public VirtualizingCollection<object> ItemsSource
		{
			get
			{
				throw new System.NotImplementedException();
			}
			set
			{
			}
		}
	}

	namespace AA
	{
		public class ListView
		{
		}
	}

	public class VirtualizingIEnumerable
	{
		public void AddNextItems(int count)
		{
		}

		public IEnumerableValuesProvider<object> ValueProvider
		{
			get
			{
				throw new System.NotImplementedException();
			}
			set
			{
			}
		}
	}

	public class LazyListView : ClassDiagrams.AA.ListView
	{
		public int InitialCount { get; set; }

		public VirtualizingIEnumerable VirtualizingSource
		{
			get
			{
				throw new System.NotImplementedException();
			}
			set
			{
			}
		}
	}

	public class EnumerableValuesProvider : IEnumerableValuesProvider<ObjectValue>
	{
		public Debugger Debugger
		{
			get
			{
				throw new System.NotImplementedException();
			}
			set
			{
			}
		}
		#region ValueProvider<ObjectValue> Members

		public ObjectValue GetNextItem()
		{
			throw new NotImplementedException();
		}

		#endregion
	}

	public class ListValuesProvider : IListValuesProvider<ObjectValue>
	{
		public Debugger Debugger
		{
			get
			{
				throw new System.NotImplementedException();
			}
			set
			{
			}
		}

		public int GetCount()
		{
			return 0;
		}

		public ObjectValue GetItemAt(int index)
		{
			return null;
		}

		#region IListValuesProvider<object> Members


		#endregion
	}

	public class ObjectValue
	{
		/// <summary> Index of this item in the collection. </summary>
		public int Index
		{
			get { return 0; }
		}

		/// <summary>
		/// Returns property with given name.
		/// </summary>
		public string this[string propertyName]
		{
			get { return null; }
		}
	}

	public class Debugger
	{
	}
}
