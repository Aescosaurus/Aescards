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
		class ShowHideItem
		{
			public ShowHideItem( FrameworkElement item,bool showOnFront )
			{
				this.item = item;
				visOnFront = showOnFront;
			}

			public void ShowFront()
			{
				item.Visibility = visOnFront ? Visibility.Visible : Visibility.Hidden;
			}

			public void ShowBack()
			{
				item.Visibility = visOnFront ? Visibility.Hidden : Visibility.Visible;
			}

			public FrameworkElement item;
			public bool visOnFront;
		}

		public ReviewPage( string deckName )
		{
			InitializeComponent();

			showToggleItems.Add( new ShowHideItem( ButtonNext,true ) );
			showToggleItems.Add( new ShowHideItem( ButtonFail,false ) );
			showToggleItems.Add( new ShowHideItem( ButtonHard,false ) );
			showToggleItems.Add( new ShowHideItem( ButtonGood,false ) );
			showToggleItems.Add( new ShowHideItem( ButtonEasy,false ) );
			showToggleItems.Add( new ShowHideItem( ReviewFront,true ) );
			showToggleItems.Add( new ShowHideItem( ReviewAnswerFront,false ) );
			showToggleItems.Add( new ShowHideItem( ReviewAnswerSeparator,false ) );
			showToggleItems.Add( new ShowHideItem( ReviewAnswerBack,false ) );
			
			cardHand = new CardHandler( deckName );

			if( cardHand.GetCardCount() > 0 ) LoadCardFront();
		}

		public bool HasLoadedCards()
		{
			return( cardHand.GetCardCount() > 0 );
		}

		void LoadCardFront()
		{
			foreach( var item in showToggleItems ) item.ShowFront();

			var card = cardHand.GetCurReviewCard();

			ReviewFront.Text = card.GetFront();
		}

		void LoadCardBack()
		{
			foreach( var item in showToggleItems ) item.ShowBack();

			var card = cardHand.GetCurReviewCard();

			ReviewAnswerFront.Text = card.GetFront();
			ReviewAnswerBack.Text = card.GetBack();
		}

		private void BackButton_Click( object sender,RoutedEventArgs e )
		{
			FinishReview();
		}

		void FinishReview()
		{
			cardHand.Save();

			MenuStack.GoBack();
		}

		private void SickButton_Click( object sender,RoutedEventArgs e )
		{
			// set card time till next review to a few days
		}

		void ScoreButtonClick( int score )
		{
			// change card score

			if( cardHand.GotoNextReviewCard() )
			{
				FinishReview();
			}
			else
			{
				// goto next card
				LoadCardFront();
			}
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

		private void ButtonNext_Click( object sender,RoutedEventArgs e )
		{
			LoadCardBack();
		}

		CardHandler cardHand;

		List<ShowHideItem> showToggleItems = new List<ShowHideItem>();
	}
}
