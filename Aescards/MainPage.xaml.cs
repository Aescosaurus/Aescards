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
			public DeckListItem( string name,string path )
			{
				this.name = name;
				this.path = path;
			}

			public string name;
			public string path;
		}

        public MainPage()
        {
            InitializeComponent();

			LoadDecks();
        }

		void LoadDecks()
		{
			if( !Directory.Exists( DeckPage.deckPath ) ) Directory.CreateDirectory( DeckPage.deckPath );

			var deckDataList = new List<DeckListItem>();

			foreach( var specificDeckPath in Directory.EnumerateDirectories( DeckPage.deckPath ) )
			{
				string deckName = "";
				for( int i = 0; i < specificDeckPath.Length; ++i )
				{
					if( specificDeckPath[i] == '/' ) deckName = specificDeckPath.Substring( i + 1 );
				}

				deckDataList.Add( new DeckListItem( deckName,specificDeckPath ) );
				deckDataDict.Add( deckName,specificDeckPath );
			}

			foreach( var deckData in deckDataList )
			{
				ListBoxItem curItem = new ListBoxItem();
				curItem.HorizontalContentAlignment = HorizontalAlignment.Stretch;
				
				var curButton = new Button();
				curButton.Content = deckData.name;
				curButton.FontSize = 16.0;
				// curButton.Name = deckData.path;
				curButton.Click += new RoutedEventHandler( OnDeckClick );

				curItem.Content = curButton;

				DeckList.Items.Add( curItem );
			}
		}

		void OnDeckClick( object sender,RoutedEventArgs args )
		{
			var selectedDeck = sender as Button;
			
			Debug.Assert( selectedDeck != null );
			
			MenuStack.GoIn( new DeckPage( deckDataDict[selectedDeck.Content as string] ) );
		}

		Dictionary<string,string> deckDataDict = new Dictionary<string,string>();
    }
}
