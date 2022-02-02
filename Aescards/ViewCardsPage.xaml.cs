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
using System.Diagnostics;

namespace Aescards
{
	/// <summary>
	/// Interaction logic for ViewCardsPage.xaml
	/// </summary>
	public partial class ViewCardsPage
		:
		Page
	{
		enum SortType
		{
			Created,
			FCount,
			Score,
			Review // days till next review
		}

		enum SortOrder
		{
			Ascending,
			Descending
		}

		public ViewCardsPage( CardHandler cardHand )
		{
			InitializeComponent();

			cards = cardHand.GetAllCards();

			for( int i = 0; i < cards.Count; ++i )
			{
				cardSortOrder.Add( i );
			}

			ResetCardListItems();
		}

		void Resort( SortType type )
		{
			switch( type )
			{
				case SortType.Created:
					cardSortOrder.Sort( delegate( int a,int b )
					{
						return( a - b );
					} );
					break;
				case SortType.FCount:
					cardSortOrder.Sort( delegate( int a,int b )
					{
						return( cards[a].GetFCount() - cards[b].GetFCount() );
					} );
					break;
				case SortType.Score:
					cardSortOrder.Sort( delegate ( int a,int b )
					{
						return( ( int )( cards[a].GetCurScore() - cards[b].GetCurScore() ) );
					} );
					break;
				case SortType.Review:
					cardSortOrder.Sort( delegate ( int a,int b )
					{
						return ( ( int )( cards[a].GetDaysTillNextReview() - cards[b].GetDaysTillNextReview() ) );
					} );
					break;
				default:
					Debug.Assert( false );
					break;
			}
		}

		void AddCardListItem( string name )
		{
			ListBoxItem curItem = new ListBoxItem();
			curItem.HorizontalContentAlignment = HorizontalAlignment.Stretch;

			curItem.Content = name;

			CardList.Items.Add( curItem );
		}

		void ResetCardListItems()
		{
			CardList.Items.Clear();

			if( sortOrder == SortOrder.Ascending )
			{
				for( int i = 0; i < cardSortOrder.Count; ++i )
				{
					AddCardListItem( cards[cardSortOrder[i]].GetFront() );
				}
			}
			else if( sortOrder == SortOrder.Descending )
			{
				for( int i = cardSortOrder.Count - 1; i >= 0; --i )
				{
					AddCardListItem( cards[cardSortOrder[i]].GetFront() );
				}
			}
		}

		private void BackButton_Click( object sender,RoutedEventArgs e )
		{
			MenuStack.GoBack();
		}

		private void SortFCount_Click( object sender,RoutedEventArgs e )
		{
			Resort( SortType.FCount );

			ResetCardListItems();
		}

		private void SortScore_Click( object sender,RoutedEventArgs e )
		{
			Resort( SortType.Score );

			ResetCardListItems();
		}

		private void SortReview_Click( object sender,RoutedEventArgs e )
		{
			Resort( SortType.Review );

			ResetCardListItems();
		}

		private void Created_Click( object sender,RoutedEventArgs e )
		{
			Resort( SortType.Created );

			ResetCardListItems();
		}

		private void SortAsc_Click( object sender,RoutedEventArgs e )
		{
			sortOrder = SortOrder.Ascending;

			ResetCardListItems();
		}

		private void SortDesc_Click( object sender,RoutedEventArgs e )
		{
			sortOrder = SortOrder.Descending;

			ResetCardListItems();
		}

		List<Card> cards;
		List<int> cardSortOrder = new List<int>();

		SortOrder sortOrder = SortOrder.Descending;
	}
}
