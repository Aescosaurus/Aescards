using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Aescards
{
	/// <summary>
	/// Interaction logic for ReviewPage.xaml
	/// </summary>
	public partial class ReviewPage
		:
		Page
	{
		public ReviewPage( Frame reviewFrame )
		{
			InitializeComponent();

			this.reviewFrame = reviewFrame;

			KeyDown += new KeyEventHandler( OnKeyDown );

			ButtonFail.Focus();
		}

		private void BackButton_Click( object sender,RoutedEventArgs e )
		{
			reviewFrame.Navigate( null );
		}

		private void SickButton_Click( object sender,RoutedEventArgs e )
		{
			
		}

		void OnKeyDown( object sender, KeyEventArgs keyArgs )
		{
			switch( keyArgs.Key )
			{
				case Key.D1:
					ScoreButtonClick( 0 );
					break;
				case Key.D2:
					ScoreButtonClick( 1 );
					break;
				case Key.D3:
					ScoreButtonClick( 2 );
					break;
				case Key.D4:
					ScoreButtonClick( 3 );
					break;
				default:
					break;
			}
		}

		void ScoreButtonClick( int score )
		{
			// change card score
			// goto next card
		}

		private void ButtonFail_Click( object sender,RoutedEventArgs e )
		{
			ScoreButtonClick( 0 );
		}

		private void ButtonHard_Click( object sender,RoutedEventArgs e )
		{
			ScoreButtonClick( 1 );
		}

		private void ButtonGood_Click( object sender,RoutedEventArgs e )
		{
			ScoreButtonClick( 2 );
		}

		private void ButtonEasy_Click( object sender,RoutedEventArgs e )
		{
			ScoreButtonClick( 3 );
		}

		Frame reviewFrame;
	}
}
