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
		}

		public void ReloadCards()
		{
			cardHand.ReloadCards();
		}

		private void BackButton_Click( object sender,RoutedEventArgs e )
		{
			MenuStack.GoBack();
		}

		private void StartReviewButton_Click( object sender,RoutedEventArgs e )
		{
			if( cardHand.GetCardCount() > 0 )
			{
				cardHand.GenerateReview();

				MenuStack.GoIn( new ReviewPage( cardHand ) );
			}
		}

		private void AddCardButton_Click( object sender,RoutedEventArgs e )
		{
			MenuStack.GoIn( new AddCardPage( cardHand.GetCardCount(),this ) );
		}

		public static readonly string deckPath = "Decks/";
		string deckName;
		CardHandler cardHand;
	}
}
