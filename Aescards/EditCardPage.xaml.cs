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
	/// Interaction logic for EditCardPage.xaml
	/// </summary>
	public partial class EditCardPage
		:
		Page
	{
		public EditCardPage( CardHandler cardHand,int cardIndex )
		{
			InitializeComponent();

			AescPage.SetupColors( BaseGrid );

			card = cardHand.GetCardAt( cardIndex );

			SetInputBoxLanguage( InputFront );
			SetInputBoxLanguage( InputBack );

			InputFront.Text = card.GetFront();
			InputBack.Text = card.GetBack();

			origFront = InputFront.Text;
			origBack = InputBack.Text;
		}

		private void BackButton_Click( object sender,RoutedEventArgs e )
		{
			bool canLeave = true;

			if( InputFront.Text != origFront || InputBack.Text != origBack )
			{
				var result = MessageBox.Show( "Error: Unsaved changes.  Leave without saving?","Unsaved Changes",MessageBoxButton.YesNoCancel );
				canLeave = ( result == MessageBoxResult.Yes );
			}

			if( canLeave ) MenuStack.GoBack();
		}

		private void SaveButton_Click( object sender,RoutedEventArgs e )
		{
			if( InputFront.Text.Length > 0 && InputBack.Text.Length > 0 )
			{
				// var savePath = DeckPage.deckPath + Card.folderPath + maxCard.ToString() + ".txt";

				// replace \r\n with "\n" (2 chars, diff than '\n') so reader doesn't glitch
				var frontText = InputFront.Text.Replace( "\r\n","\\n" );
				var backText = InputBack.Text.Replace( "\r\n","\\n" );

				// bool allowAdd = true;
				// if( deckPage.CheckExisting( frontText ) )
				// {
				// 	var result = MessageBox.Show( "Error: Card already exists in deck!  Would you like to add anyway?","Card Already Exists",MessageBoxButton.YesNoCancel );
				// 	allowAdd = ( result == MessageBoxResult.Yes );
				// }
				// if( deckPage.GetMaxCard() >= deckPage.GetDeckData().GetMaxDeckSize() )
				// {
				// 	var result = MessageBox.Show( "Error: Exceeding max card limit!  Would you like to add anyway?","Too Many Cards",MessageBoxButton.YesNoCancel );
				// 	allowAdd = ( result == MessageBoxResult.Yes );
				// }

				// if( allowAdd )
				{
					// var newCard = new Card( deckPage.GetMaxCard(),frontText,backText,0,0.0f,0.0f );
					// newCard.Save();
					// 
					// deckPage.GetDeckData().AddCard();
					// deckPage.GetDeckData().Save();
					// 
					// deckPage.ReloadCards();
					// deckPage.ReloadDeckData();
					card.SetFront( frontText );
					card.SetBack( backText );
					card.Save();

					// we're editing the card ref so no need to reload cards

					InputFront.Text = "";
					InputBack.Text = "";

					MenuStack.GoBack();
				}
			}
		}

		void SetInputBoxLanguage( TextBox box )
		{
			var cultureInfo = Dispatcher.Thread.CurrentCulture.Name.ToString();
			InputLanguageManager.SetInputLanguage( box,new System.Globalization.CultureInfo( cultureInfo ) );
		}

		Card card;

		string origFront;
		string origBack;
	}
}
