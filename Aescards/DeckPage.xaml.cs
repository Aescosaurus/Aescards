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
using System.IO;

namespace Aescards
{
    /// <summary>
    /// Interaction logic for DeckPage.xaml
    /// </summary>
    public partial class DeckPage
		:
		Page
    {
		public DeckPage( string deckNamePath,MainPage mainPage )
		{
			InitializeComponent();

			AescPage.SetupColors( BaseGrid );

			// load deck info
			this.deckName = deckNamePath;
			myData = new DeckData( deckNamePath );

			cardHand = new CardHandler( deckNamePath,this );

			this.mainPage = mainPage;

			// update card time till next review
			var daysSinceLastOpened = myData.GetDaysSinceLastOpened();
			if( daysSinceLastOpened > myData.GetTimeUpdateThresh() ) // no use in updating partial days if spam opening and closing deck
			{
				myData.UpdateTime();

				cardHand.UpdateTimeTillNextReview( daysSinceLastOpened );
				cardHand.Save();
			}

			if( myData.IsDifferentWholeDay() )
			{
				myData.UpdateWholeDay();
			}

			ReloadDeckData();

			// Save no matter what since we want to save setting data
			// also save on setting update
			myData.Save(); // so we don't have to worry about saving on close
		}

		public void ReloadCards()
		{
			cardHand.ReloadCards();

			CardCount.Text = cardHand.GetCardCount().ToString() + " cards";
		}

		public void ReloadDeckData()
		{
			DeckName.Text = myData.GetDeckName();

			var nCards = cardHand.GetCardCount();
			CardCount.Text = "Total: " + nCards.ToString();// + " card" + ( nCards != 1 ? "s" : "" );

			var newCards = myData.GetCardsAddedToday();
			NewCards.Text = "Added today: " + newCards.ToString();// + " card" + ( newCards != 1 ? "s" : "" );

			ReviewedCards.Text = "Reviewed today: " + myData.GetCardsReviewedToday().ToString();

			var avgScore = cardHand.GetAvgScore();
			string scoreGrade = "";
			if( avgScore < 1.0f ) scoreGrade = "Fail";
			else if( avgScore < 2.0f ) scoreGrade = "Hard";
			else if( avgScore < 3.0f ) scoreGrade = "Good";
			else scoreGrade = "Easy";
			AvgScore.Text = "Avg score: " + avgScore.ToString() + " (" + scoreGrade + ')';

			FCount.Text = FormatPercent( "Fail: ",cardHand.GetFCount(),nCards );
			HardCount.Text = FormatPercent( "Hard: ",cardHand.GetHardCount(),nCards );
			GoodCount.Text = FormatPercent( "Good: ",cardHand.GetGoodCount(),nCards );
			EasyCount.Text = FormatPercent( "Easy: ",cardHand.GetEasyCount(),nCards );
			NewCount.Text = FormatPercent( "New: ",cardHand.GetNewCount(),nCards );

			var reviewableCount = cardHand.GetReviewCandidateCount();
			// ReviewCount.Text = "Reviewable: " + reviewableCount.ToString();// + " card" + ( reviewableCount != 1 ? "s" : "" );
			ReviewCount.Text = FormatPercent( "Reviewable: ",reviewableCount,nCards );
		}

		string FormatFloat( float val )
		{
			return( ( ( int )Math.Round( val ) ).ToString() );
		}

		string FormatPercent( string start,int count,int maxCards )
		{
			var result = start + count.ToString();

			if( maxCards > 0 )
			{
				result += " (" + FormatFloat( ( ( float )count / maxCards ) * 100.0f ) + "%)";
			}

			return( result );
		}

		private void BackButton_Click( object sender,RoutedEventArgs e )
		{
			MenuStack.GoBack();
		}

		private void StartReviewButton_Click( object sender,RoutedEventArgs e )
		{
			if( cardHand.GenerateReview() )
			{
				MenuStack.GoIn( new ReviewPage( this,cardHand ) );

				myData.UpdateLastReviewDate();
				myData.Save();

				mainPage.ReloadDecks();
			}
		}

		private void AddCardButton_Click( object sender,RoutedEventArgs e )
		{
			MenuStack.GoIn( new AddCardPage( this ) );
		}

		private void SettingsButton_Click( object sender,RoutedEventArgs e )
		{
			MenuStack.GoIn( new SettingsPage( this,mainPage ) );
		}

		private void ViewCardsButton_Click( object sender,RoutedEventArgs e )
		{
			MenuStack.GoIn( new ViewCardsPage( cardHand,myData ) );
		}

		public int GetMaxCard()
		{
			return( cardHand.GetCardCount() );
		}

		public DeckData GetDeckData()
		{
			return( myData );
		}

		public bool CheckExisting( string front )
		{
			return( myData.GetCheckExisting() && cardHand.CheckExisting( front ) );
		}

		// return index of existing card, -1 if not existing
		public int GetExistingSpot( string front )
		{
			return( cardHand.GetExistingSpot( front ) );
		}

		public CardHandler GetCardHand()
		{
			return( cardHand );
		}

		public static readonly string deckPath = "Decks/";
		DeckData myData;
		string deckName;
		CardHandler cardHand;
		MainPage mainPage;
	}
}
