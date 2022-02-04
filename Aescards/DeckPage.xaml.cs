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

			var avgScore = cardHand.GetAvgScore();
			string scoreGrade = "";
			if( avgScore < 1.0f ) scoreGrade = "Fail";
			else if( avgScore < 2.0f ) scoreGrade = "Hard";
			else if( avgScore < 3.0f ) scoreGrade = "Good";
			else scoreGrade = "Easy";
			AvgScore.Text = "Avg score: " + avgScore.ToString() + " (" + scoreGrade + ')';

			var fCount = cardHand.GetFCount();
			FCount.Text = "Fail: " + fCount.ToString() + " (" + FormatFloat( ( ( float )fCount / nCards ) * 100.0f ) + "%)";
			var hardCount = cardHand.GetHardCount();
			HardCount.Text = "Hard: " + hardCount.ToString() + " (" + FormatFloat( ( ( float )hardCount / nCards ) * 100.0f ) + "%)";
			var goodCount = cardHand.GetGoodCount();
			GoodCount.Text = "Good: " + goodCount.ToString() + " (" + FormatFloat( ( ( float )goodCount / nCards ) * 100.0f ) + "%)";
			var easyCount = cardHand.GetEasyCount();
			EasyCount.Text = "Easy: " + easyCount.ToString() + " (" + FormatFloat( ( ( float )easyCount / nCards ) * 100.0f ) + "%)";
			var newCount = cardHand.GetNewCount();
			NewCount.Text = "New: " + newCount.ToString() + " (" + FormatFloat( ( ( float )newCount / nCards ) * 100.0f ) + "%)";

			var reviewableCount = cardHand.GetReviewCandidateCount();
			ReviewCount.Text = "Reviewable: " + reviewableCount.ToString();// + " card" + ( reviewableCount != 1 ? "s" : "" );
		}

		string FormatFloat( float val )
		{
			return( ( ( int )Math.Round( val ) ).ToString() );
		}

		private void BackButton_Click( object sender,RoutedEventArgs e )
		{
			MenuStack.GoBack();
		}

		private void StartReviewButton_Click( object sender,RoutedEventArgs e )
		{
			if( cardHand.GenerateReview() ) MenuStack.GoIn( new ReviewPage( this,cardHand ) );
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
			MenuStack.GoIn( new ViewCardsPage( cardHand ) );
		}

		public int GetMaxCard()
		{
			return( cardHand.GetCardCount() );
		}

		public DeckData GetDeckData()
		{
			return( myData );
		}

		public static readonly string deckPath = "Decks/";
		DeckData myData;
		string deckName;
		CardHandler cardHand;
		MainPage mainPage;
	}
}
