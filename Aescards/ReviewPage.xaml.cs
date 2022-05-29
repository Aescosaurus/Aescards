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

		public ReviewPage( DeckPage deckPage,CardHandler cardHand )
		{
			InitializeComponent();

			AescPage.SetupColors( BaseGrid );

			showToggleItems.Add( new ShowHideItem( ButtonNext,true ) );
			showToggleItems.Add( new ShowHideItem( ButtonFail,false ) );
			showToggleItems.Add( new ShowHideItem( ButtonHard,false ) );
			showToggleItems.Add( new ShowHideItem( ButtonGood,false ) );
			showToggleItems.Add( new ShowHideItem( ButtonEasy,false ) );
			showToggleItems.Add( new ShowHideItem( ReviewFront,true ) );
			showToggleItems.Add( new ShowHideItem( ReviewAnswerFront,false ) );
			showToggleItems.Add( new ShowHideItem( ReviewAnswerSeparator,false ) );
			showToggleItems.Add( new ShowHideItem( ReviewAnswerBack,false ) );
			
			// cardHand = new CardHandler( deckName );
			this.deckPage = deckPage;
			this.cardHand = cardHand;

			// if( cardHand.GetCardCount() > 0 ) LoadCardFront();
			LoadCardFront();
		}

		// public bool HasLoadedCards()
		// {
		// 	return( cardHand.GetCardCount() > 0 );
		// }

		void LoadCardFront()
		{
			foreach( var item in showToggleItems ) item.ShowFront();

			var card = cardHand.GetCurReviewCard();

			ReviewFront.Text = card.GetFront();

			// PrevButton.IsEnabled = cardHand.CanGoPrev();
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
			cardHand.ResetCardReviewTries();
			cardHand.Save();

			deckPage.GetDeckData().Save();
			deckPage.ReloadDeckData();

			MenuStack.GoBack();
		}

		private void SickButton_Click( object sender,RoutedEventArgs e )
		{
			// set card time till next review to a few days
			// cardHand.GetCurReviewCard().Sick();
			cardHand.SickCurCard();

			GotoNextCard();
		}

		void ScoreButtonClick( Card.Score score )
		{
			// change card score
			cardHand.UpdateCurCardScore( score );

			GotoNextCard();
		}

		void GotoNextCard()
		{
			if( !cardHand.ReviewingOldCards() ) deckPage.GetDeckData().ReviewCard();

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
			ScoreButtonClick( Card.Score.Fail );
		}

		private void ButtonHard_Click( object sender,RoutedEventArgs e )
		{
			ScoreButtonClick( Card.Score.Hard );
		}

		private void ButtonGood_Click( object sender,RoutedEventArgs e )
		{
			ScoreButtonClick( Card.Score.Good );
		}

		private void ButtonEasy_Click( object sender,RoutedEventArgs e )
		{
			ScoreButtonClick( Card.Score.Easy );
		}

		private void ButtonNext_Click( object sender,RoutedEventArgs e )
		{
			LoadCardBack();
		}

		private void EditButton_Click( object sender,RoutedEventArgs e )
		{
			// MenuStack.GoIn( new EditCardPage( cardHand,cardHand.GetCurReviewCardIndex() ) );
			MenuStack.GoInAction( new EditCardPage( cardHand,cardHand.GetCurReviewCardIndex() ),LoadCardFront );
		}

		private void PrevButton_Click( object sender,RoutedEventArgs e )
		{
			if( cardHand.CanGoPrev() )
			{
				cardHand.GotoPrevReviewCard();
				deckPage.GetDeckData().UnReviewCard();
			}

			LoadCardFront();

			// PrevButton.IsEnabled = false; // only allow go 1 prev
		}

		private void SkipButton_Click( object sender,RoutedEventArgs e )
		{
			GotoNextCard();
		}

		DeckPage deckPage;
		CardHandler cardHand;

		List<ShowHideItem> showToggleItems = new List<ShowHideItem>();
	}
}
