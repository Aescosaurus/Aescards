﻿using System;
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
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace Aescards
{
	/// <summary>
	/// Interaction logic for SettingsPage.xaml
	/// </summary>
	public partial class SettingsPage
		:
		Page
	{
		enum InputType
		{
			String,
			Int,
			Float,
			Bool
		}

		public SettingsPage( DeckPage deckPage,MainPage mainPage )
		{
			InitializeComponent();

			AescPage.SetupColors( BaseGrid );

			this.deckPage = deckPage;
			this.mainPage = mainPage;

			var deckData = deckPage.GetDeckData();

			AddSettingsListItem( deckNameStr,deckData.GetDeckName(),InputType.String );
			AddSettingsListItem( fRepairStr,deckData.GetFRepair().ToString(),InputType.Int );
			AddSettingsListItem( cardsPerReviewStr,deckData.GetCardsPerReview().ToString(),InputType.Int );
			AddSettingsListItem( timeUpdateThreshStr,deckData.GetTimeUpdateThresh().ToString(),InputType.Float );
			AddSettingsListItem( maxDeckSizeStr,deckData.GetMaxDeckSize().ToString(),InputType.Int );
			AddSettingsListItem( sickDelayStr,deckData.GetSickDelay().ToString(),InputType.Float );
			AddSettingsListItem( checkExistingStr,deckData.GetCheckExisting().ToString(),InputType.Bool );
			// AddSettingsListItem( prioritizeNewStr,deckData.GetPrioritizeNew().ToString(),InputType.Bool );
			AddSettingsListItem( targetNewPerReviewStr,deckData.GetNewCardsPerReview().ToString(),InputType.Int );
		}

		void AddSettingsListItem( string name,string value,InputType inputType )
		{
			var curItem = AescPage.CreateListBoxItem();
			// curItem.HorizontalContentAlignment = HorizontalAlignment.Stretch;

			FrameworkElement curSetting = null;
			if( inputType != InputType.Bool )
			{
				curSetting = AescPage.CreateTextBox();
				( curSetting as TextBox ).Text = value;
			}
			else
			{
				curSetting = AescPage.CreateCheckBox();
				( curSetting as CheckBox ).IsChecked = bool.Parse( value );
			}
			switch( inputType )
			{
				case InputType.String:
					// no verification
					break;
				case InputType.Int:
					curSetting.PreviewTextInput += new TextCompositionEventHandler( TextIntMatch );
					break;
				case InputType.Float:
					curSetting.PreviewTextInput += new TextCompositionEventHandler( TextFloatMatch );
					break;
				case InputType.Bool:
					// no veri
					break;
				default:
					Debug.Assert( false );
					break;
			}
			settingBoxes.Add( name,curSetting );

			// curItem.Content = curSetting;
			var stackPanel = new StackPanel();
			var textBlock = AescPage.CreateTextBlock();
			textBlock.Text = name;
			stackPanel.Children.Add( textBlock );
			stackPanel.Children.Add( curSetting );

			curItem.Content = stackPanel;

			SettingsList.Items.Add( curItem );
		}

		private void SaveButton_Click( object sender,RoutedEventArgs e )
		{
			var deckData = deckPage.GetDeckData();

			var deckNameVal = GetSettingStr( deckNameStr );
			var fRepairVal = GetSettingInt( fRepairStr );
			var cardsPerReviewVal = GetSettingInt( cardsPerReviewStr );
			var timeUpdateThreshVal = GetSettingFloat( timeUpdateThreshStr );
			var maxDeckSizeVal = GetSettingInt( maxDeckSizeStr );
			var sickDelayVal = GetSettingFloat( sickDelayStr );
			var checkExistingVal = GetSettingBool( checkExistingStr );
			// var prioritizeNewVal = GetSettingBool( prioritizeNewStr );
			var targetNewVal = GetSettingInt( targetNewPerReviewStr );

			if( !parseError )
			{
				deckData.SetDeckName( deckNameVal );
				deckData.SetFRepair( fRepairVal );
				deckData.SetCardsPerReview( cardsPerReviewVal );
				deckData.SetTimeUpdateThresh( timeUpdateThreshVal );
				deckData.SetMaxDeckSize( maxDeckSizeVal );
				deckData.SetSickDelay( sickDelayVal );
				deckData.SetCheckExisting( checkExistingVal );
				// deckData.SetPrioritizeNew( prioritizeNewVal );
				deckData.SetNewCardsPerReview( targetNewVal );

				deckData.Save();

				deckPage.ReloadDeckData();
				mainPage.ReloadDecks();

				MenuStack.GoBack();
			}
		}

		string GetSettingStr( string index )
		{
			Debug.Assert( settingBoxes[index] is TextBox );
			return( ( settingBoxes[index] as TextBox ).Text );
		}

		int GetSettingInt( string index )
		{
			int val = -1;
			bool result = int.TryParse( GetSettingStr( index ),out val );
			if( !result )
			{
				parseError = true;
				ShowErrorBox( index );
			}
			return( val );
		}

		float GetSettingFloat( string index )
		{
			float val = -1.0f;
			bool result = float.TryParse( GetSettingStr( index ),out val );
			if( !result )
			{
				parseError = true;
				ShowErrorBox( index );
			}
			return( val );
		}

		bool GetSettingBool( string index )
		{
			Debug.Assert( settingBoxes[index] is CheckBox );
			return ( ( settingBoxes[index] as CheckBox ).IsChecked ?? false );
		}

		void ShowErrorBox( string paramName )
		{
			MessageBox.Show( "Error: Invalid formatting for " + paramName + "!","Invalid Formatting",MessageBoxButton.OK );
		}

		private void CancelButton_Click( object sender,RoutedEventArgs e )
		{
			if( ShowConfirmBox() ) MenuStack.GoBack();
		}

		private void BackButton_Click( object sender,RoutedEventArgs e )
		{
			if( ShowConfirmBox() ) MenuStack.GoBack();
		}

		// return true if want to leave
		bool ShowConfirmBox()
		{
			if( SettingsActuallyChanged() )
			{
				var result = MessageBox.Show( "Some settings are modified.  Leave without saving?","Unsaved Changed",MessageBoxButton.YesNoCancel );
				return( result == MessageBoxResult.Yes );
			}
			return( true );
		}

		// True if diff from DeckData
		bool SettingsActuallyChanged()
		{
			var deckData = deckPage.GetDeckData();
			return( GetSettingStr( deckNameStr ) != deckData.GetDeckName() ||
				GetSettingInt( fRepairStr ) != deckData.GetFRepair() ||
				GetSettingInt( cardsPerReviewStr ) != deckData.GetCardsPerReview() ||
				GetSettingFloat( timeUpdateThreshStr ) != deckData.GetTimeUpdateThresh() ||
				GetSettingInt( maxDeckSizeStr ) != deckData.GetMaxDeckSize() ||
				GetSettingFloat( sickDelayStr ) != deckData.GetSickDelay() ||
				GetSettingBool( checkExistingStr ) != deckData.GetCheckExisting() ||
				// GetSettingBool( prioritizeNewStr ) != deckData.GetPrioritizeNew() ||
				GetSettingInt( targetNewPerReviewStr ) != deckData.GetNewCardsPerReview() );
		}

		private void TextIntMatch( object sender,TextCompositionEventArgs args )
		{
			args.Handled = floatRegex.IsMatch( args.Text );
		}

		private void TextFloatMatch( object sender,TextCompositionEventArgs args )
		{
			args.Handled = intRegex.IsMatch( args.Text );
		}

		DeckPage deckPage;
		MainPage mainPage;

		Dictionary<string,FrameworkElement> settingBoxes = new Dictionary<string,FrameworkElement>();
		Regex intRegex = new Regex( "[^0-9\\.]+" );
		Regex floatRegex = new Regex( "[^0-9]+" );

		bool parseError = false;

		static readonly string deckNameStr = "Deck Name";
		static readonly string fRepairStr = "F Repair";
		static readonly string cardsPerReviewStr = "Cards Per Review";
		static readonly string timeUpdateThreshStr = "Time Update Threshhold";
		static readonly string maxDeckSizeStr = "Max Deck Size";
		static readonly string sickDelayStr = "Sick Delay";
		static readonly string checkExistingStr = "Check Existing";
		// static readonly string prioritizeNewStr = "Prioritize New";
		static readonly string targetNewPerReviewStr = "Target New Cards Per Review";
	}
}
