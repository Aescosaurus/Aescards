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

namespace Aescards
{
    /// <summary>
    /// Interaction logic for CreateDeckPage.xaml
    /// </summary>
    public partial class CreateDeckPage
		:
		Page
    {
        public CreateDeckPage( MainPage mainPageRef )
        {
            InitializeComponent();

			this.mainPageRef = mainPageRef;
        }

		private void CreateButton_Click( object sender,RoutedEventArgs e )
		{
			// var newDeckPath = DeckPage.deckPath + DeckName.Text;
			var curDeckNum = mainPageRef.GetDeckCount().ToString();
			var newDeckPath = DeckPage.deckPath + curDeckNum;
			if( DeckName.Text.Length > 0 && !Directory.Exists( newDeckPath ) )
			{
				Directory.CreateDirectory( newDeckPath );
				var curDeckData = new DeckData( curDeckNum );
				curDeckData.UpdateTime();
				curDeckData.SetDeckName( DeckName.Text );
				curDeckData.Save( curDeckNum );
				mainPageRef.ReloadDecks();
				MenuStack.GoBack();
			}
		}

		private void BackButton_Click( object sender,RoutedEventArgs e )
		{
			MenuStack.GoBack();
		}

		MainPage mainPageRef;
	}
}
