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
using System.Globalization;
using System.Windows.Input;

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
				var textBlockObj = obj as TextBlock;
				textBlockObj.Foreground = GetCol( textCol );
			}
			else if( obj is Rectangle )
			{
				var rectObj = obj as Rectangle;
				if( rectObj.Name == "bg" ) rectObj.Fill = GetCol( bgCol );
				else rectObj.Fill = GetCol( buttonCol ); // top bar & button same color
			}
			else if( obj is Button )
			{
				var buttonObj = obj as Button;
				buttonObj.Background = GetCol( buttonCol );
				buttonObj.Foreground = GetCol( textCol );
				buttonObj.FontSize = 20.0;
			}
			else if( obj is TextBox )
			{
				var textBoxObj = obj as TextBox;
				textBoxObj.Foreground = GetCol( textCol );
				textBoxObj.Background = GetCol( listBoxCol );
				textBoxObj.FontSize = 30.0;

				var cultureInfo = obj.Dispatcher.Thread.CurrentCulture.Name.ToString();
				InputLanguageManager.SetInputLanguage( textBoxObj,new CultureInfo( cultureInfo ) );
			}
			else if( obj is ListBox )
			{
				var listBoxObj = obj as ListBox;
				listBoxObj.Background = GetCol( listBoxCol );
				listBoxObj.Foreground = GetCol( textCol );
				listBoxObj.FontSize = 20.0;
			}
			else if( obj is ListBoxItem )
			{
				var listBoxItemObj = obj as ListBoxItem;
				listBoxItemObj.Foreground = GetCol( textCol );
				listBoxItemObj.Background = GetCol( listBoxCol );
				listBoxItemObj.FontSize = 20.0;
				listBoxItemObj.HorizontalContentAlignment = HorizontalAlignment.Stretch;
			}
			else if( obj is CheckBox )
			{
				var checkBoxObj = obj as CheckBox;
				checkBoxObj.LayoutTransform = new ScaleTransform( 1.5,1.5 );
			}
		}

		public static TextBlock CreateTextBlock()
		{
			var textBlock = new TextBlock();
			SetObjCol( textBlock );
			textBlock.FontSize = 16.0;
			return ( textBlock );
		}

		public static Button CreateButton()
		{
			var button = new Button();
			SetObjCol( button );
			return( button );
		}

		public static TextBox CreateTextBox()
		{
			var textBox = new TextBox();
			SetObjCol( textBox );
			return( textBox );
		}

		public static ListBoxItem CreateListBoxItem()
		{
			var listBoxItem = new ListBoxItem();
			SetObjCol( listBoxItem );
			return( listBoxItem );
		}

		public static CheckBox CreateCheckBox()
		{
			var checkBox = new CheckBox();
			SetObjCol( checkBox );
			return( checkBox );
		}

		public static void SetColorScheme( int index )
		{
			curColorScheme = index;
		}

		static Brush GetCol( int index )
		{
			return( colors[curColorScheme][index] );
		}

		const int textCol = 0;
		const int bgCol = 1;
		const int buttonCol = 2;
		const int listBoxCol = 3;

		static int curColorScheme = 0;

		static readonly Brush[][] colors =
		{
			new Brush[] // default gray
			{
				Brushes.Black,
				Brushes.DarkGray,
				Brushes.LightGray,
				Brushes.WhiteSmoke
			},
			new Brush[] // pink
			{
				Brushes.White,
				Brushes.LightPink,
				Brushes.DeepPink,
				Brushes.HotPink
			},
			new Brush[] // mellow blue
			{
				Brushes.DarkBlue,
				Brushes.LightBlue,
				Brushes.CornflowerBlue,
				Brushes.AliceBlue
			},
			new Brush[] // deep sea
			{
				Brushes.Turquoise,
				Brushes.DarkSlateGray,
				Brushes.DarkCyan,
				Brushes.SteelBlue
			},
			new Brush[] // sandy desert
			{
				Brushes.DarkGoldenrod,
				Brushes.PaleGoldenrod,
				Brushes.LightYellow,
				Brushes.LightGoldenrodYellow
			}

			// deep sky blue
			// dark turquoise
			// dodger blue
		};
	}
}
