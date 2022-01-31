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
		public DeckPage( string deckName,string deckNamePath )
		{
			InitializeComponent();

			// load deck info
			this.deckName = deckNamePath;
			DeckName.Text = deckName;

			cardHand = new CardHandler( deckName );

			CardCount.Text = cardHand.GetCardCount().ToString() + " cards";

			// update card time till next review
			var deckDataPath = deckPath + deckName + '/' + "DeckData.txt";
			myData = new DeckData( deckDataPath );
			var daysSinceLastOpened = myData.GetDaysSinceLastOpened();
			if( daysSinceLastOpened > 1.0f ) // no use in updating partial days if spam opening and closing deck
			{
				myData.UpdateTime();
				myData.Save( deckDataPath ); // so we don't have to worry about saving on close

				cardHand.UpdateTimeTillNextReview( daysSinceLastOpened );
				cardHand.Save();
			}
		}

		public void ReloadCards()
		{
			cardHand.ReloadCards();

			CardCount.Text = cardHand.GetCardCount().ToString() + " cards";
		}

		private void BackButton_Click( object sender,RoutedEventArgs e )
		{
			MenuStack.GoBack();
		}

		private void StartReviewButton_Click( object sender,RoutedEventArgs e )
		{
			if( cardHand.GenerateReview() ) MenuStack.GoIn( new ReviewPage( cardHand ) );
		}

		private void AddCardButton_Click( object sender,RoutedEventArgs e )
		{
			MenuStack.GoIn( new AddCardPage( cardHand.GetCardCount(),this ) );
		}

		public static readonly string deckPath = "Decks/";
		DeckData myData;
		string deckName;
		CardHandler cardHand;
	}
}
