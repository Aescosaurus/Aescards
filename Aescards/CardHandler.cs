using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Aescards
{
    public class CardHandler
    {
		public CardHandler( string deckPath )
		{
			curCardPath = DeckPage.deckPath + deckPath + '/' + Card.folderPath;

			if( !Directory.Exists( curCardPath ) ) Directory.CreateDirectory( curCardPath );

			ReloadCards();
		}

		public void ReloadCards()
		{
			cards.Clear();
			// Load all cards
			for( int i = 0; i < maxCards; ++i )
			{
				var curCard = Card.GenerateCard( i );
				if( curCard == null ) break;
				else cards.Add( curCard );
			}

			// GenerateReview();
		}

		public void UpdateCurCardScore( int score )
		{
			GetCurReviewCard().UpdateScore( score );

			// repeat failed cards until they are not fail
			if( score < 1 ) reviewCards.Add( reviewCards[curReviewSpot] );
		}

		public void Save()
		{
			foreach( var card in cards )
			{
				if( card.IsModified() ) card.Save();
			}
		}

		public void GenerateReview()
		{
			reviewCards.Clear();
			curReviewSpot = 0;

			foreach( var card in cards )
			{
				reviewCards.Add( card.GetId() );
			}
		}

		public Card GetCurReviewCard()
		{
			return( cards[reviewCards[curReviewSpot]] );
		}

		// return true if end of review
		public bool GotoNextReviewCard()
		{
			++curReviewSpot;

			return( curReviewSpot >= reviewCards.Count );
		}

		public int GetCardCount()
		{
			return( cards.Count );
		}

		List<Card> cards = new List<Card>();

		const int maxCards = 9999;

		List<int> reviewCards = new List<int>();
		int curReviewSpot = 0;

		public static string curCardPath = "";
    }
}
