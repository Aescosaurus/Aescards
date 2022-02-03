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
			for( int i = 0; i < deckPage.GetDeckData().GetMaxDeckSize(); ++i )
			{
				var curCard = Card.GenerateCard( i );
				if( curCard == null ) break;
				else cards.Add( curCard );
			}

			// GenerateReview();
		}

		public void UpdateCurCardScore( int score )
		{
			GetCurReviewCard().UpdateScore( score,deckPage.GetDeckData().GetFRepair() );

			// repeat failed cards until they are not fail
			if( score < 1 ) reviewCards.Add( reviewCards[curReviewSpot] );
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

			foreach( var card in cards )
			{
				if( card.GetDaysTillNextReview() <= 0 )
				{
					reviewCards.Add( card.GetId() );
				}
			}

			// Sort with longest overdue cards coming first
			reviewCards.Sort( delegate( int a,int b )
			{
				return( cards[a].GetDaysTillNextReview().CompareTo( cards[b].GetDaysTillNextReview() ) );
			} );

			// cull excess cards
			var reviewSize = deckPage.GetDeckData().GetCardsPerReview();
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

		public List<Card> GetAllCards()
		{
			return( cards );
		}

		public float GetAvgScore()
		{
			float total = 0.0f;
			foreach( var card in cards ) total += card.GetCurScore();

			return( total / cards.Count );
		}

		public int GetFCount()
		{
			int fCount = 0;
			foreach( var card in cards )
			{
				if( card.GetCurScore() < 1.0f && card.GetCurScore() != 0.0f )
				{
					++fCount;
				}
			}

			return( fCount );
		}

		public int GetNewCount()
		{
			int newCount = 0;
			foreach( var card in cards )
			{
				if( card.GetCurScore() == 0.0f ) ++newCount;
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

		List<Card> cards = new List<Card>();

		// const int maxCards = 9999;
		// const int maxReviewSize = 20;

		List<int> reviewCards = new List<int>();
		int curReviewSpot = 0;

		public static string curCardPath = "";

		DeckPage deckPage;
    }
}
