using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Diagnostics;

namespace Aescards
{
	class AescPage
	{
		public static void SetupElements( params FrameworkElement[] elements )
		{
			foreach( var el in elements )
			{
				ProcessElement( el );
			}
		}

		static void ProcessElement( FrameworkElement el )
		{
			foreach( char c in el.Tag.ToString() )
			{
				if( c == tags[0] )
				{
					// translation?
				}
				else
				{
					double fontSize = 0.0;
					if( c == tags[1] ) fontSize = largeTextSize;
					else if( c == tags[2] ) fontSize = medTextSize;
					else if( c == tags[3] ) fontSize = smallTextSize;
					else if( c == tags[4] ) fontSize = buttonTextSize;

					if( el is Button )
					{
						( ( Button )el ).FontSize = fontSize;
					}
					else if( el is TextBlock )
					{
						( ( TextBlock )el ).FontSize = fontSize;
					}
					else
					{
						Debug.Assert( false );
					}
				}
			}
		}

		public static double GetLargeTextSize()
		{
			return( largeTextSize );
		}

		public static double GetMedTextSize()
		{
			return( medTextSize );
		}

		public static double GetSmallTextSize()
		{
			return( smallTextSize );
		}

		public static double GetButtonTextSize()
		{
			return( buttonTextSize );
		}

		static readonly char[] tags =
		{
			't', // translateable
			'l', // large text (60)
			'm', // med text (40)
			's', // small text (20)
			'b' // button text (16)
		};

		const double largeTextSize = 60.0;
		const double medTextSize = 40.0;
		const double smallTextSize = 20.0;
		const double buttonTextSize = 16.0;
	}
}
