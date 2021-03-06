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
using System.IO;
using System.Diagnostics;

namespace Aescards
{
    /// <summary>
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class MainPage
		:
		Page
    {
		class DeckListItem
		{
			public DeckListItem( string name,string path,DeckData deckData )
			{
				this.name = name;
				this.path = path;
				this.deckData = deckData;
			}

			public string name;
			public string path;
			public DeckData deckData;
		}

        public MainPage()
        {
            InitializeComponent();

			if( !Directory.Exists( DeckPage.deckPath ) ) Directory.CreateDirectory( DeckPage.deckPath );

			settings = new AescardsSettings();

			var selectedColorScheme = settings.GetColorScheme();
			ColorSchemeBox.SelectedIndex = selectedColorScheme;
			AescPage.SetColorScheme( selectedColorScheme );

			LoadDecks();

			AescPage.SetupColors( BaseGrid );
		}

		void LoadDecks()
		{
			var deckDataList = new List<DeckListItem>();

			for( int i = 0; i < 9999; ++i )
			{
				var deckPath = i.ToString();
				if( File.Exists( DeckData.GeneratePath( deckPath ) ) )
				{
					var deckData = new DeckData( deckPath );

					var deckName = deckData.GetDeckName();

					deckDataList.Add( new DeckListItem( deckName,deckPath,deckData ) );
					deckDataDict.Add( deckName,deckPath );
				}
				else break;
			}

			// Sort with most recently reviewed decks first.
			deckDataList.Sort( delegate( DeckListItem a,DeckListItem b )
			{
				return( b.deckData.GetLastReviewDate().CompareTo( a.deckData.GetLastReviewDate() ) );
			} );

			foreach( var deckData in deckDataList )
			{
				AddDeckListItem( deckData.name,new RoutedEventHandler( OnDeckClick ) );
			}

			AddDeckListItem( "(+) New deck",new RoutedEventHandler( OnCreateDeck ) );
		}

		public void ReloadDecks()
		{
			DeckList.Items.Clear();
			deckDataDict.Clear();

			LoadDecks();
		}
		
		void AddDeckListItem( string name,RoutedEventHandler clickHandler )
		{
			var curItem = AescPage.CreateListBoxItem();
			// curItem.HorizontalContentAlignment = HorizontalAlignment.Stretch;

			var curButton = AescPage.CreateButton();
			curButton.Content = name;
			curButton.Click += clickHandler;

			curItem.Content = curButton;

			DeckList.Items.Add( curItem );
		}

		void OnDeckClick( object sender,RoutedEventArgs args )
		{
			var selectedDeck = sender as Button;
			
			Debug.Assert( selectedDeck != null );
			
			var deckName = selectedDeck.Content as string;
			MenuStack.GoIn( new DeckPage( deckDataDict[deckName],this ) );
		}

		void OnCreateDeck( object sender,RoutedEventArgs args )
		{
			MenuStack.GoIn( new CreateDeckPage( this ) );
		}

		public int GetDeckCount()
		{
			return( deckDataDict.Count );
		}

		private void ColorSchemeBox_SelectionChanged( object sender,SelectionChangedEventArgs e )
		{
			var comboBox = sender as ComboBox;

			settings.SetColorScheme( comboBox.SelectedIndex );
			settings.Save();
			AescPage.SetColorScheme( comboBox.SelectedIndex );


			AescPage.SetupColors( BaseGrid );
		}

		Dictionary<string,string> deckDataDict = new Dictionary<string,string>();

		AescardsSettings settings;

		// const int maxDecks = 9999;
	}
}
