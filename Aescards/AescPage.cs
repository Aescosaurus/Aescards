using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Windows.Media;

namespace Aescards
{
	class AescPage
	{
		public static void SetupColors( Grid baseGrid )
		{
			ObjSearch( baseGrid );
		}

		static void ObjSearch( DependencyObject obj )
		{
			SetObjCol( obj );

			for( int i = 0; i < VisualTreeHelper.GetChildrenCount( obj ); ++i )
			{
				ObjSearch( VisualTreeHelper.GetChild( obj,i ) );
			}
		}

		static void SetObjCol( DependencyObject obj )
		{
			if( obj is TextBlock )
			{
				( obj as TextBlock ).Foreground = textCol;
			}
			else if( obj is Rectangle )
			{
				var rectObj = obj as Rectangle;
				if( rectObj.Fill == Brushes.Gray ) rectObj.Fill = bgCol;
				else rectObj.Fill = topBarCol;
			}
			else if( obj is Button )
			{
				var buttonObj = obj as Button;
				buttonObj.Background = buttonCol;
				buttonObj.Foreground = textCol;
			}
			else if( obj is TextBox )
			{
				var textBoxObj = obj as TextBox;
				textBoxObj.Foreground = textCol;
				textBoxObj.Background = listBoxCol;
			}
			else if( obj is ListBox )
			{
				var listBoxObj = obj as ListBox;
				listBoxObj.Background = listBoxCol;
				listBoxObj.Foreground = textCol;
			}
			else if( obj is ListBoxItem )
			{
				var listBoxItemObj = obj as ListBoxItem;
				listBoxItemObj.Foreground = textCol;
				listBoxItemObj.Background = listBoxCol;
			}
		}

		public static TextBlock CreateTextBlock()
		{
			var textBlock = new TextBlock();
			textBlock.FontSize = 16.0;
			textBlock.Foreground = textCol;
			return( textBlock );
		}

		public static Button CreateButton()
		{
			var button = new Button();
			button.Background = buttonCol;
			button.Foreground = textCol;
			button.FontSize = 20.0f;
			return( button );
		}

		public static TextBox CreateTextBox()
		{
			var textBox = new TextBox();
			textBox.FontSize = 30.0;
			textBox.Foreground = textCol;
			textBox.Background = listBoxCol;
			return( textBox );
		}

		public static ListBoxItem CreateListBoxItem()
		{
			var listBoxItem = new ListBoxItem();
			listBoxItem.HorizontalContentAlignment = HorizontalAlignment.Stretch;
			listBoxItem.Foreground = textCol;
			listBoxItem.Background = listBoxCol;
			return( listBoxItem );
		}

		static readonly Brush textCol = Brushes.Black;
		static readonly Brush bgCol = Brushes.DarkGray;
		static readonly Brush topBarCol = Brushes.LightGray;
		static readonly Brush buttonCol = Brushes.LightGray;
		static readonly Brush listBoxCol = Brushes.WhiteSmoke;

		// static readonly Brush textCol = Brushes.White;
		// static readonly Brush bgCol = Brushes.LightPink;
		// static readonly Brush topBarCol = Brushes.Red;
		// static readonly Brush buttonCol = Brushes.HotPink;
		// static readonly Brush listBoxCol = Brushes.DeepPink;
	}
}
