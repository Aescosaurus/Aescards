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
    /// Interaction logic for AddCardPage.xaml
    /// </summary>
    public partial class AddCardPage
		:
		Page
    {
        public AddCardPage( int cardCount,DeckPage deckPage )
        {
            InitializeComponent();

			maxCard = cardCount;
			this.deckPage = deckPage;
        }

		private void BackButton_Click( object sender,RoutedEventArgs e )
		{
			MenuStack.GoBack();
		}

		private void SaveButton_Click( object sender,RoutedEventArgs e )
		{
			if( InputFront.Text.Length > 0 && InputBack.Text.Length > 0 )
			{
				// var savePath = DeckPage.deckPath + Card.folderPath + maxCard.ToString() + ".txt";

				// replace \r\n with "\n" (2 chars, diff than '\n') so reader doesn't glitch
				var frontText = InputFront.Text.Replace( "\r\n","\\n" );
				var backText = InputBack.Text.Replace( "\r\n","\\n" );

				var newCard = new Card( maxCard,frontText,backText,0,0.0f,0.0f );
				newCard.Save();

				deckPage.ReloadCards();

				MenuStack.GoBack();
			}
		}

		int maxCard;
		DeckPage deckPage;
	}
}
