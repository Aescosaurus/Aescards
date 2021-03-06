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

			var curCard = GetCurReviewCard();
				
			curCard.UpdateScore( score,deckData );

			// repeat failed cards until they are not fail
			if( score == Card.Score.Fail )
			{
				if( curReviewSpot < reviewCards.Count - 1 &&
				// curReviewSpot < deckData.GetCardsPerReview() + deckData.GetRepeatCardCount() &&
				reviewCards.Count < deckData.GetCardsPerReview() + deckData.GetRepeatCardCount() &&
				curCard.repeatCount < deckData.GetRepeatTries() &&
				!CheckCardInReviewAfterSpot( GetCurReviewCard(),curReviewSpot ) )
				{
					var cardsPerReview = deckData.GetCardsPerReview();
					var repeatCount = deckData.GetRepeatCardCount();


					reviewCards.Add( reviewCards[curReviewSpot] );

					++curCard.repeatCount;
				}
			}
			else
			{
				// if you update score to not fail remove copy at end
				RemoveNextReviewCardCopy();
			}
		}

		public void SickCurCard()
		{
			GetCurReviewCard().Sick( deckPage.GetDeckData().GetSickDelay() );

			RemoveNextReviewCardCopy();
		}

		// return true if card match exists after spot
		bool CheckCardInReviewAfterSpot( Card card,int spot )
		{
			return( CheckCardInReviewAfterSpotGetIndex( card,spot ) != -1 );
		}

		// return index of next copy of card in review after spot, -1 = not found
		int CheckCardInReviewAfterSpotGetIndex( Card card,int spot )
		{
			for( int i = spot + 1; i < reviewCards.Count; ++i )
			{
				if( cards[reviewCards[i]].GetFront() == card.GetFront() )
				{
					return( i );
				}
			}

			return( -1 );
		}

		void RemoveNextReviewCardCopy()
		{
			var nextCopySpot = CheckCardInReviewAfterSpotGetIndex( GetCurReviewCard(),curReviewSpot );
			if( nextCopySpot > -1 )
			{
				reviewCards.RemoveAt( nextCopySpot );
			}
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

		public void ResetCardReviewTries()
		{
			foreach( var card in cards )
			{
				card.repeatCount = 0;
			}
		}

		// return true if there is >0 cards in review list
		public bool GenerateReview()
		{
			reviewCards.Clear();
			curReviewSpot = 0;

			var reviewDateSorted = new List<int>();
			foreach( var card in cards ) reviewDateSorted.Add( card.GetId() );
			reviewDateSorted.Sort( delegate( int a,int b )
			{
				return( cards[a].GetDaysTillNextReview().CompareTo( cards[b].GetDaysTillNextReview() ) );
			} );

			var deckData = deckPage.GetDeckData();

			var allowInReviewThresh = deckData.GetAllowReviewThresh();
			var targetNewPerReview = deckData.GetNewCardsPerReview();
			var reviewSize = deckData.GetCardsPerReview();
			var targetRandPerReview = deckData.GetTargetRandPerReview();

			// add new
			int newAdded = 0;
			foreach( var card in reviewDateSorted )
			{
				var curCard = cards[card];
				if( curCard.IsNew() && newAdded < targetNewPerReview )
				{
					reviewCards.Add( curCard.GetId() );
					++newAdded;
				}
			}

			// add rand
			int randTries = 9999;
			var addRand = new Random();
			for( int i = 0; i < targetRandPerReview; )
			{
				var curCard = cards[addRand.Next( 0,cards.Count - 1 )];
				if( !curCard.IsNew() && curCard.GetDaysTillNextReview() <= allowInReviewThresh && !reviewCards.Contains( curCard.GetId() ) )
				{
					reviewCards.Add( curCard.GetId() );
					++i;
					if( --randTries < 0 ) break; // no infinite loops here
				}
			}

			// fill out leftover
			foreach( var card in reviewDateSorted )
			{
				var curCard = cards[card];
				if( !curCard.IsNew() && curCard.GetDaysTillNextReview() <= allowInReviewThresh && !reviewCards.Contains( curCard.GetId() ) )
				{
					if( reviewCards.Count < reviewSize ) reviewCards.Add( curCard.GetId() );
					else break;
				}
			}

			// int curNew = 0;
			// foreach( var card in cards )
			// {
			// 	if( card.GetDaysTillNextReview() <= allowInReviewThresh )
			// 	{
			// 		if( !card.IsNew() || curNew++ < targetNewPerReview )
			// 		{
			// 			reviewCards.Add( card.GetId() );
			// 		}
			// 	}
			// }
			// 
			// // // Sort with longest overdue cards coming first
			// // reviewCards.Sort( delegate( int a,int b )
			// // {
			// // 	return( cards[a].GetDaysTillNextReview().CompareTo( cards[b].GetDaysTillNextReview() ) );
			// // } );
			// // 
			// // // i dont think this wrecks the previous sorting that much but maybe a little so we should find a better solution
			// // reviewCards.Sort( delegate( int a,int b )
			// // {
			// // 	return( cards[b].IsNew().CompareTo( cards[a].IsNew() ) );
			// // } );
			// reviewCards.Sort( delegate( int a,int b )
			// {
			// 	if( cards[a].IsNew() != cards[b].IsNew() ) // new & not new, sort new first
			// 	{
			// 		return( cards[b].IsNew().CompareTo( cards[a].IsNew() ) );
			// 	}
			// 	else // both new or both not new, sort with longest overdue cards coming first
			// 	{
			// 		return( cards[a].GetDaysTillNextReview().CompareTo( cards[b].GetDaysTillNextReview() ) );
			// 	}
			// } );
			// 
			// // // Try to get all new cards in reviewCards.
			// // if( deckData.GetPrioritizeNew() )
			// // {
			// // 	// new cards at the front, they're all new so we don't care about days till next review
			// // 	reviewCards.Sort( delegate( int a,int b )
			// // 	{
			// // 		return( cards[b].IsNew().CompareTo( cards[a].IsNew() ) );
			// // 	} );
			// // }
			// // else
			// // {
			// // 	// Sort with longest overdue cards coming first
			// // 	reviewCards.Sort( delegate( int a,int b )
			// // 	{
			// // 		return( cards[a].GetDaysTillNextReview().CompareTo( cards[b].GetDaysTillNextReview() ) );
			// // 	} );
			// // }

			// cull excess cards
			if( reviewCards.Count > reviewSize )
			{
				int amountOver = reviewCards.Count - reviewSize;
				reviewCards.RemoveRange( reviewCards.Count - amountOver,amountOver );
			}

			// shuffle
			var shuffleRand = new Random();
			for( int shuffle = 0; shuffle < 3; ++shuffle )
			{
				for( int i = 0; i < reviewCards.Count; ++i )
				{
					int randSpot = shuffleRand.Next( 0,reviewCards.Count - 1 );
			
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

		public void GotoPrevReviewCard()
		{
			--curReviewSpot;
		}

		public bool CanGoPrev()
		{
			return( curReviewSpot > 0 );
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
				if( card.GetDaysTillNextReview() <= deckPage.GetDeckData().GetAllowReviewThresh() ) ++candidateCount;
			}
			return( candidateCount );
		}

		public bool CheckExisting( string front )
		{
			return( GetExistingSpot( front ) != -1 );
		}

		public int GetExistingSpot( string front )
		{
			for( int i = 0; i < cards.Count; ++i )
			{
				if( cards[i].GetFront() == front ) return( i );
			}

			return( -1 );
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
