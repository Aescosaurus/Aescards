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
	/// Interaction logic for ViewCardsPage.xaml
	/// </summary>
	public partial class ViewCardsPage
		:
		Page
	{
		public ViewCardsPage( CardHandler cardHand )
		{
			InitializeComponent();

			cards = cardHand.GetAllCards();
			
			for( int i = 0; i < cards.Count; ++i )
			{
				cardSortOrder.Add( i );
			}

			foreach( var sortedItem in cardSortOrder )
			{
				AddCardListItem( cards[sortedItem].GetFront() );
			}
		}

		void AddCardListItem( string name )
		{
			ListBoxItem curItem = new ListBoxItem();
			curItem.HorizontalContentAlignment = HorizontalAlignment.Stretch;

			curItem.Content = name;

			CardList.Items.Add( curItem );
		}

		private void BackButton_Click( object sender,RoutedEventArgs e )
		{
			MenuStack.GoBack();
		}

		List<Card> cards;
		List<int> cardSortOrder = new List<int>();
	}
}
