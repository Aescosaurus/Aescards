using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Aescards
{
    class CardHandler
    {
		public CardHandler()
		{
			if( !Directory.Exists( Card.folderPath ) ) Directory.CreateDirectory( Card.folderPath );

			// Load all cards
			for( int i = 0; i < maxCards; ++i )
			{
				var curCard = Card.GenerateCard( i );
				if( curCard == null ) break;
				else cards.Add( curCard );
			}

			GenerateReview();
		}

		public void Save()
		{
			foreach( var card in cards )
			{
				if( card.IsModified() ) card.Save();
			}
		}

		void GenerateReview()
		{
			reviewCards.Clear();

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

		List<Card> cards = new List<Card>();

		const int maxCards = 9999;

		List<int> reviewCards = new List<int>();
		int curReviewSpot = 0;
    }
}
