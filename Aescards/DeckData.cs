using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace Aescards
{
	public class DeckData
	{
		public DeckData( string deckName )
		{
			mySavePath = GeneratePath( deckName );
			if( File.Exists( mySavePath ) )
			{
				ReloadData();
			}
		}

		public void ReloadData()
		{
			var reader = new StreamReader( mySavePath );
			var lines = new List<string>();
			while( !reader.EndOfStream ) lines.Add( reader.ReadLine() );
			reader.Close();

			if( lines.Count > 0 ) lastSave = DateTime.Parse( lines[0] );
			if( lines.Count > 1 ) this.deckName = lines[1];
			if( lines.Count > 2 ) fRepair = int.Parse( lines[2] );
			if( lines.Count > 3 ) nCardsPerReview = int.Parse( lines[3] );
			if( lines.Count > 4 ) timeUpdateThresh = float.Parse( lines[4] );
			if( lines.Count > 5 ) maxDeckSize = int.Parse( lines[5] );
			if( lines.Count > 6 ) sickDelay = int.Parse( lines[6] );
			if( lines.Count > 7 ) wholeDay = DateTime.Parse( lines[7] );
			if( lines.Count > 8 ) cardsAddedToday = int.Parse( lines[8] );
			if( lines.Count > 9 ) checkExisting = bool.Parse( lines[9] );
			if( lines.Count > 10 ) prioritizeNew = bool.Parse( lines[10] );
			if( lines.Count > 11 ) cardsReviewedToday = int.Parse( lines[11] );
			if( lines.Count > 12 ) targetNewPerReview = int.Parse( lines[12] );
			if( lines.Count > 13 ) lastReviewDate = DateTime.Parse( lines[13] );
			if( lines.Count > 14 ) allowReviewThresh = float.Parse( lines[14] );
		}

		public void Save()
		{
			Debug.Assert( deckName.Length > 0 );

			// var path = GeneratePath( deckName );

			string saveData = "";

			saveData += lastSave.ToString() + '\n';
			saveData += this.deckName + '\n';
			saveData += fRepair.ToString() + '\n';
			saveData += nCardsPerReview.ToString() + '\n';
			saveData += timeUpdateThresh.ToString() + '\n';
			saveData += maxDeckSize.ToString() + '\n';
			saveData += sickDelay.ToString() + '\n';
			saveData += wholeDay.ToString() + '\n';
			saveData += cardsAddedToday.ToString() + '\n';
			saveData += checkExisting.ToString() + '\n';
			saveData += prioritizeNew.ToString() + '\n';
			saveData += cardsReviewedToday.ToString() + '\n';
			saveData += targetNewPerReview.ToString() + '\n';
			saveData += lastReviewDate.ToString() + '\n';
			saveData += allowReviewThresh.ToString() + '\n';

			var writer = new StreamWriter( mySavePath );
			writer.Write( saveData );
			writer.Close();
		}

		public static string GeneratePath( string deckName )
		{
			return( DeckPage.deckPath + deckName + '/' + "DeckData.txt" );
		}

		public void UpdateTime()
		{
			lastSave = DateTime.Now;
		}

		public void UpdateWholeDay()
		{
			wholeDay = DateTime.Now;

			cardsAddedToday = 0;
			cardsReviewedToday = 0;
		}

		public void SetDeckName( string name )
		{
			deckName = name;
		}

		public void SetFRepair( int fRepair )
		{
			this.fRepair = fRepair;
		}

		public void SetCardsPerReview( int nCards )
		{
			nCardsPerReview = nCards;
		}

		public void SetTimeUpdateThresh( float thresh )
		{
			timeUpdateThresh = thresh;
		}

		public void SetMaxDeckSize( int size )
		{
			maxDeckSize = size;
		}

		public void SetSickDelay( float delay )
		{
			sickDelay = delay;
		}

		public void AddCard()
		{
			++cardsAddedToday;
		}

		public void SetCheckExisting( bool check )
		{
			checkExisting = check;
		}

		public void SetPrioritizeNew( bool prioritize )
		{
			prioritizeNew = prioritize;
		}

		public void ReviewCard()
		{
			++cardsReviewedToday;
		}

		public void SetNewCardsPerReview( int count )
		{
			targetNewPerReview = count;
		}

		public void UpdateLastReviewDate()
		{
			lastReviewDate = DateTime.Now;
		}

		public void SetAllowReviewThresh( float thresh )
		{
			allowReviewThresh = thresh;
		}

		public float GetDaysSinceLastOpened()
		{
			return( ( float )( ( DateTime.Now - lastSave ).TotalDays ) );
		}

		public int GetWholeDayDiff()
		{
			return( ( DateTime.Now - wholeDay ).Days );
		}

		public bool IsDifferentWholeDay()
		{
			return( DateTime.Now.DayOfYear != wholeDay.DayOfYear );
		}

		public string GetDeckName()
		{
			return( deckName );
		}

		public int GetFRepair()
		{
			return( fRepair );
		}

		public int GetCardsPerReview()
		{
			return( nCardsPerReview );
		}

		public float GetTimeUpdateThresh()
		{
			return( timeUpdateThresh );
		}

		public int GetMaxDeckSize()
		{
			return( maxDeckSize );
		}

		public float GetSickDelay()
		{
			return( sickDelay );
		}

		public int GetCardsAddedToday()
		{
			return( cardsAddedToday );
		}

		public bool GetCheckExisting()
		{
			return( checkExisting );
		}

		public bool GetPrioritizeNew()
		{
			return( prioritizeNew );
		}

		public int GetCardsReviewedToday()
		{
			return( cardsReviewedToday );
		}

		public int GetNewCardsPerReview()
		{
			return( targetNewPerReview );
		}

		public DateTime GetLastReviewDate()
		{
			return( lastReviewDate );
		}

		public float GetAllowReviewThresh()
		{
			return( allowReviewThresh );
		}

		string mySavePath;

		DateTime lastSave;
		string deckName = "";
		int fRepair = 1;
		int nCardsPerReview = 20;
		float timeUpdateThresh = 0.2f;
		int maxDeckSize = 10000;
		float sickDelay = 3.0f;
		bool checkExisting = false;

		DateTime wholeDay;
		int cardsAddedToday = 0;

		bool prioritizeNew = false;
		int cardsReviewedToday = 0;

		int targetNewPerReview = 5;

		DateTime lastReviewDate;

		float allowReviewThresh = 0.0f;
	}
}
