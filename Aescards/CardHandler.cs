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

		public void UpdateTimeTillNextReview( float daysPassed )
		{
			foreach( var card in cards )
			{
				card.UpdateDaysTillNextReview( daysPassed );
			}
		}

		// return true if there is >0 cards in review list
		public bool GenerateReview()
		{
			reviewCards.Clear();
			curReviewSpot = 0;

			foreach( var card in cards )
			{
				// reviewCards.Add( card.GetId() );
				if( card.GetDaysTillNextReview() <= 0 )
				{
					reviewCards.Add( card.GetId() );
				}
			}

			// Sort with longest overdue cards coming first
			reviewCards.Sort( delegate( int a,int b )
			{
				return( ( int )( cards[b].GetDaysTillNextReview() - cards[a].GetDaysTillNextReview() ) );
			} );

			// cull excess cards
			if( reviewCards.Count > maxReviewSize )
			{
				int amountOver = reviewCards.Count - maxReviewSize;
				reviewCards.RemoveRange( reviewCards.Count - amountOver,amountOver );
			}

			// shuffle
			var rand = new Random();
			for( int i = 0; i < reviewCards.Count; ++i )
			{
				int randSpot = rand.Next( 0,reviewCards.Count - 1 );

				var temp = reviewCards[i];
				reviewCards[i] = reviewCards[randSpot];
				reviewCards[randSpot] = temp;
			}

			return( reviewCards.Count > 0 );
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
		const int maxReviewSize = 100;

		List<int> reviewCards = new List<int>();
		int curReviewSpot = 0;

		public static string curCardPath = "";
    }
}
