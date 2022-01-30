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

			for( int i = 0; i < maxCards; ++i )
			{
				var curCard = Card.GenerateCard( i );
				if( curCard == null ) break;
				else cards.Add( curCard );
			}
		}

		public void Save()
		{
			foreach( var card in cards )
			{
				if( card.IsModified() ) card.Save();
			}
		}

		List<Card> cards = new List<Card>();

		const int maxCards = 9999;
    }
}
