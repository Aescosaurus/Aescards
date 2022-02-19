using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Diagnostics;

namespace Aescards
{
    class MenuStack
    {
		class PageItem
		{
			public PageItem( Page page,Action returnAction )
			{
				this.page = page;
				this.returnAction = returnAction;
			}

			public Page page;
			public Action returnAction;
		}

		public static void SetupContentFrame( Frame frame )
		{
			contentFrame = frame;
		}

		public static void GoIn( Page page )
		{
			Debug.Assert( contentFrame != null );
			pageStack.Push( new PageItem( page,null ) );
			contentFrame.Navigate( page );
		}

		// Go in, call onReturn when backing out to this page again
		public static void GoInAction( Page page,Action onReturn )
		{
			pageStack.Peek().returnAction = onReturn;

			GoIn( page );
		}

		public static void GoBack( bool callReturnAction = true )
		{
			Debug.Assert( contentFrame != null );

			if( pageStack.Count > 1 )
			{
				pageStack.Pop();
				var prevPageItem = pageStack.Peek();
				if( callReturnAction ) prevPageItem.returnAction?.Invoke();
				contentFrame.Navigate( prevPageItem.page );
			}
		}

		static Stack<PageItem> pageStack = new Stack<PageItem>();
		static Frame contentFrame = null;
    }
}
