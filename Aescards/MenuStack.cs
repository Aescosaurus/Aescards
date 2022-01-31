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
		public static void SetupContentFrame( Frame frame )
		{
			contentFrame = frame;
		}

		public static void GoIn( Page page )
		{
			Debug.Assert( contentFrame != null );
			pageStack.Push( page );
			contentFrame.Navigate( page );
		}

		public static void GoBack()
		{
			Debug.Assert( contentFrame != null );

			if( pageStack.Count > 1 )
			{
				pageStack.Pop();
				Page prevPage = pageStack.Peek();
				contentFrame.Navigate( prevPage );
			}
		}

		static Stack<Page> pageStack = new Stack<Page>();
		static Frame contentFrame = null;
    }
}
