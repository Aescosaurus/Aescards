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

		public ViewCardsPage( CardHandler cardHand,DeckData deckData )
		{
			InitializeComponent();

			AescPage.SetupColors( BaseGrid );

			DeckName.Text = deckData.GetDeckName() + " Cards";

			this.cardHand = cardHand;

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
						return( a.CompareTo( b ) );
					} );
					break;
				case SortType.FCount:
					cardSortOrder.Sort( delegate( int a,int b )
					{
						var result = cards[a].GetFCount().CompareTo( cards[b].GetFCount() );
						if( result == 0 ) result = a.CompareTo( b );
						return( result );
					} );
					break;
				case SortType.Score:
					cardSortOrder.Sort( delegate ( int a,int b )
					{
						var result = cards[a].GetCurScore().CompareTo( cards[b].GetCurScore() );
						if( result == 0 ) result = a.CompareTo( b );
						return ( result );
					} );
					break;
				case SortType.Review:
					cardSortOrder.Sort( delegate ( int a,int b )
					{
						var result = cards[a].GetDaysTillNextReview().CompareTo( cards[b].GetDaysTillNextReview() );
						if( result == 0 ) result = a.CompareTo( b );
						return ( result );
					} );
					break;
				default:
					Debug.Assert( false );
					break;
			}
		}

		void ResetCardListItems()
		{
			CardList.Items.Clear();

			if( sortOrder == SortOrder.Ascending )
			{
				for( int i = 0; i < cardSortOrder.Count; ++i )
				{
					// AddCardListItem( cards[cardSortOrder[i]].GetFront(),cardSortOrder[i] );
					TryAddCardListItem( cards[cardSortOrder[i]],cardSortOrder[i] );
				}
			}
			else if( sortOrder == SortOrder.Descending )
			{
				for( int i = cardSortOrder.Count - 1; i >= 0; --i )
				{
					// AddCardListItem( cards[cardSortOrder[i]].GetFront(),cardSortOrder[i] );
					TryAddCardListItem( cards[cardSortOrder[i]],cardSortOrder[i] );
				}
			}
		}

		void TryAddCardListItem( Card card,int id )
		{
			if( card.GetFront().Contains( filterStr ) || card.GetBack().Contains( filterStr ) )
			{
				AddCardListItem( card.GetFront(),id );
			}
		}

		void AddCardListItem( string name,int id )
		{
			var curItem = AescPage.CreateListBoxItem();
			// curItem.HorizontalContentAlignment = HorizontalAlignment.Stretch;

			curItem.Content = name;
			curItem.Name = "card" + id.ToString();

			curItem.Selected += new RoutedEventHandler( OnCardSelected );

			CardList.Items.Add( curItem );
		}

		void OnCardSelected( object sender,RoutedEventArgs args )
		{
			Debug.Assert( sender is ListBoxItem );

			var cardListItem = sender as ListBoxItem;

			string cardNum = "";
			foreach( char c in cardListItem.Name )
			{
				if( char.IsNumber( c ) ) cardNum += c;
			}

			highlightedCard = int.Parse( cardNum );

			DisplayHighlightedCardInfo();
		}

		void DisplayHighlightedCardInfo()
		{
			var selectedCard = cards[highlightedCard];

			CardFront.Content = "Front: " + selectedCard.GetFront();
			CardBack.Content = "Back:\n" + selectedCard.GetBack();
			CardFCount.Content = "F Count: " + selectedCard.GetFCount().ToString();
			CardScore.Content = "Score: " + selectedCard.GetCurScore().ToString();
			CardReview.Content = "Days Till Next Review: " + selectedCard.GetDaysTillNextReview().ToString();
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

		private void CardEdit_Click( object sender,RoutedEventArgs e )
		{
			if( highlightedCard > -1 )
			{
				MenuStack.GoInAction( new EditCardPage( cardHand,highlightedCard ),ReloadHighlightedCard );
			}
		}

		void ReloadHighlightedCard()
		{
			cards[highlightedCard].Load();

			DisplayHighlightedCardInfo();
		}

		private void SearchButton_Click( object sender,RoutedEventArgs e )
		{
			filterStr = SearchBox.Text;

			ResetCardListItems();
		}

		private void ClearSearchButton_Click( object sender,RoutedEventArgs e )
		{
			SearchBox.Text = "";
			filterStr = "";

			ResetCardListItems();
		}

		CardHandler cardHand;
		List<Card> cards;
		List<int> cardSortOrder = new List<int>();

		SortOrder sortOrder = SortOrder.Descending;

		int highlightedCard = -1;

		string filterStr = "";
	}
}
