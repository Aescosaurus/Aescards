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
        public AddCardPage( DeckPage deckPage )
        {
            InitializeComponent();

			AescPage.SetupColors( BaseGrid );

			// maxCard = cardCount;
			this.deckPage = deckPage;

			SetInputBoxLanguage( InputFront );
			SetInputBoxLanguage( InputBack );

			// InputFront.PreviewTextInput += new TextCompositionEventHandler( OnTextInput );
			// InputBack.PreviewTextInput += new TextCompositionEventHandler( OnTextInput );
			InputFront.TextChanged += new TextChangedEventHandler( OnTextInput );
			InputBack.TextChanged += new TextChangedEventHandler( OnTextInput );
		}

		private void BackButton_Click( object sender,RoutedEventArgs e )
		{
			bool canLeave = true;

			if( InputFront.Text.Length > 0 || InputBack.Text.Length > 0 )
			{
				var result = MessageBox.Show( "Error: Unsaved card data.  Leave without saving?","Unsaved Data",MessageBoxButton.YesNoCancel );
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

				bool allowAdd = true;
				var existingCardSpot = deckPage.GetExistingSpot( frontText );
				if( existingCardSpot > -1 && deckPage.GetDeckData().GetCheckExisting() )
				{
					var result = MessageBox.Show( "Error: Card already exists in deck!  Would you like to edit the existing card?","Card Already Exists",MessageBoxButton.YesNoCancel );
					// allowAdd = ( result == MessageBoxResult.Yes );
					allowAdd = false;
					if( result == MessageBoxResult.Yes )
					{
						// MenuStack.GoIn( new EditCardPage( deckPage.GetCardHand(),existingCardSpot ) );
						MenuStack.GoInAction( new EditCardPage( deckPage.GetCardHand(),existingCardSpot ),EditExistingReturnAction );
					}
				}
				if( deckPage.GetMaxCard() >= deckPage.GetDeckData().GetMaxDeckSize() )
				{
					var result = MessageBox.Show( "Error: Exceeding max card limit!  Would you like to add anyway?","Too Many Cards",MessageBoxButton.YesNoCancel );
					allowAdd = ( result == MessageBoxResult.Yes );
				}

				if( allowAdd )
				{
					var newCard = new Card( deckPage.GetMaxCard(),frontText,backText,0,0.0f,0.0f );
					newCard.Save();

					deckPage.GetDeckData().AddCard();
					deckPage.GetDeckData().Save();

					deckPage.ReloadCards();
					deckPage.ReloadDeckData();

					InputFront.Text = "";
					InputBack.Text = "";

					// MenuStack.GoBack();
				}
			}
		}

		// called by default, if leave without saving then not called
		void EditExistingReturnAction()
		{
			InputFront.Text = "";
			InputBack.Text = "";
		}

		void SetInputBoxLanguage( TextBox box )
		{
			var cultureInfo = Dispatcher.Thread.CurrentCulture.Name.ToString();
			InputLanguageManager.SetInputLanguage( box,new System.Globalization.CultureInfo( cultureInfo ) );
		}

		void OnTextInput( object sender,TextChangedEventArgs args )
		{
			SaveButton.IsEnabled = ( InputFront.Text.Length > 0 && InputBack.Text.Length > 0 );
		}

		DeckPage deckPage;
	}
}
