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
		public CardHandler( string deckPath,DeckPage deckPage )
		{
			curCardPath = DeckPage.deckPath + deckPath + '/' + Card.folderPath;
			
			this.deckPage = deckPage;

			if( !Directory.Exists( curCardPath ) ) Directory.CreateDirectory( curCardPath );

			ReloadCards();
		}

		public void ReloadCards()
		{
			cards.Clear();
			// Load all cards
			// for( int i = 0; i < deckPage.GetDeckData().GetMaxDeckSize(); ++i )
			for( int i = 0; i < int.MaxValue; ++i )
			{
				var curCard = Card.GenerateCard( i );
				if( curCard == null ) break;
				else cards.Add( curCard );
			}

			// GenerateReview();
		}

		public void UpdateCurCardScore( Card.Score score )
		{
			var deckData = deckPage.GetDeckData();

			// Can't say good/easy if you've already failed it once this review
			if( curReviewSpot >= deckData.GetCardsPerReview() && score > Card.Score.Hard )
			{
				score = Card.Score.Hard;
			}

			GetCurReviewCard().UpdateScore( score,deckData.GetFRepair() );

			// repeat failed cards until they are not fail
			if( score == Card.Score.Fail && curReviewSpot < reviewCards.Count - 1 ) reviewCards.Add( reviewCards[curReviewSpot] );
		}

		public void SickCurCard()
		{
			GetCurReviewCard().Sick( deckPage.GetDeckData().GetSickDelay() );
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

			var deckData = deckPage.GetDeckData();

			var targetNewPerReview = deckData.GetNewCardsPerReview();
			int curNew = 0;
			foreach( var card in cards )
			{
				if( card.GetDaysTillNextReview() <= 0 )
				{
					if( !card.IsNew() || curNew++ < targetNewPerReview )
					{
						reviewCards.Add( card.GetId() );
					}
				}
			}

			// // Sort with longest overdue cards coming first
			// reviewCards.Sort( delegate( int a,int b )
			// {
			// 	return( cards[a].GetDaysTillNextReview().CompareTo( cards[b].GetDaysTillNextReview() ) );
			// } );
			// 
			// // i dont think this wrecks the previous sorting that much but maybe a little so we should find a better solution
			// reviewCards.Sort( delegate( int a,int b )
			// {
			// 	return( cards[b].IsNew().CompareTo( cards[a].IsNew() ) );
			// } );
			reviewCards.Sort( delegate( int a,int b )
			{
				if( cards[a].IsNew() != cards[b].IsNew() ) // new & not new, sort new first
				{
					return( cards[b].IsNew().CompareTo( cards[a].IsNew() ) );
				}
				else // both new or both not new, sort with longest overdue cards coming first
				{
					return( cards[a].GetDaysTillNextReview().CompareTo( cards[b].GetDaysTillNextReview() ) );
				}
			} );

			// // Try to get all new cards in reviewCards.
			// if( deckData.GetPrioritizeNew() )
			// {
			// 	// new cards at the front, they're all new so we don't care about days till next review
			// 	reviewCards.Sort( delegate( int a,int b )
			// 	{
			// 		return( cards[b].IsNew().CompareTo( cards[a].IsNew() ) );
			// 	} );
			// }
			// else
			// {
			// 	// Sort with longest overdue cards coming first
			// 	reviewCards.Sort( delegate( int a,int b )
			// 	{
			// 		return( cards[a].GetDaysTillNextReview().CompareTo( cards[b].GetDaysTillNextReview() ) );
			// 	} );
			// }

			// cull excess cards
			var reviewSize = deckData.GetCardsPerReview();
			if( reviewCards.Count > reviewSize )
			{
				int amountOver = reviewCards.Count - reviewSize;
				reviewCards.RemoveRange( reviewCards.Count - amountOver,amountOver );
			}

			// shuffle
			var rand = new Random();
			for( int shuffle = 0; shuffle < 3; ++shuffle )
			{
				for( int i = 0; i < reviewCards.Count; ++i )
				{
					int randSpot = rand.Next( 0,reviewCards.Count - 1 );
			
					var temp = reviewCards[i];
					reviewCards[i] = reviewCards[randSpot];
					reviewCards[randSpot] = temp;
				}
			}

			return( reviewCards.Count > 0 );
		}

		public Card GetCurReviewCard()
		{
			return( cards[GetCurReviewCardIndex()] );
		}

		public int GetCurReviewCardIndex()
		{
			return( reviewCards[curReviewSpot] );
		}

		// return true if end of review
		public bool GotoNextReviewCard()
		{
			++curReviewSpot;

			return( curReviewSpot >= reviewCards.Count );
		}

		// returns true if we're just reviewing cards that we've seen in the same review but marked as f so they're coming back
		public bool ReviewingOldCards()
		{
			return( curReviewSpot >= deckPage.GetDeckData().GetCardsPerReview() );
		}

		public int GetCardCount()
		{
			return( cards.Count );
		}

		public List<Card> GetAllCards()
		{
			return( cards );
		}

		public float GetAvgScore()
		{
			float total = 0.0f;
			foreach( var card in cards ) total += card.GetCurScore();

			if( cards.Count > 0 ) return( total / cards.Count );
			else return( 0.0f );
		}

		public int GetFCount()
		{
			int fCount = 0;
			foreach( var card in cards )
			{
				if( card.GetCurScore() < 1.0f && card.GetCurScore() != 0.0f ) ++fCount;
			}
			return( fCount );
		}

		public int GetHardCount()
		{
			int hardCount = 0;
			foreach( var card in cards )
			{
				if( card.GetCurScore() >= 1.0f && card.GetCurScore() < 2.0f ) ++hardCount;
			}
			return( hardCount );
		}

		public int GetGoodCount()
		{
			int goodCount = 0;
			foreach( var card in cards )
			{
				if( card.GetCurScore() >= 2.0f && card.GetCurScore() < 3.0f ) ++goodCount;
			}
			return( goodCount );
		}

		public int GetEasyCount()
		{
			int easyCount = 0;
			foreach( var card in cards )
			{
				if( card.GetCurScore() >= 3.0f ) ++easyCount;
			}
			return( easyCount );
		}

		public int GetNewCount()
		{
			int newCount = 0;
			foreach( var card in cards )
			{
				if( card.IsNew() ) ++newCount;
			}
			return( newCount );
		}

		public int GetReviewCandidateCount()
		{
			int candidateCount = 0;
			foreach( var card in cards )
			{
				if( card.GetDaysTillNextReview() <= 0.0f ) ++candidateCount;
			}
			return( candidateCount );
		}

		public bool CheckExisting( string front )
		{
			foreach( var card in cards )
			{
				if( card.GetFront() == front ) return( true );
			}

			return( false );
		}

		public Card GetCardAt( int i )
		{
			return( cards[i] );
		}

		List<Card> cards = new List<Card>();

		// const int maxCards = 9999;
		// const int maxReviewSize = 20;

		List<int> reviewCards = new List<int>();
		int curReviewSpot = 0;

		public static string curCardPath = "";

		DeckPage deckPage;
    }
}
