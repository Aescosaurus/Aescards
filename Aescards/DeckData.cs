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

			int curLine = 0;

			if( lines.Count > curLine ) lastSave = DateTime.Parse( lines[curLine++] );
			if( lines.Count > curLine ) this.deckName = lines[curLine++];
			if( lines.Count > curLine ) fRepair = int.Parse( lines[curLine++] );
			if( lines.Count > curLine ) nCardsPerReview = int.Parse( lines[curLine++] );
			if( lines.Count > curLine ) timeUpdateThresh = float.Parse( lines[curLine++] );
			if( lines.Count > curLine ) maxDeckSize = int.Parse( lines[curLine++] );
			if( lines.Count > curLine ) sickDelay = int.Parse( lines[curLine++] );
			if( lines.Count > curLine ) wholeDay = DateTime.Parse( lines[curLine++] );
			if( lines.Count > curLine ) cardsAddedToday = int.Parse( lines[curLine++] );
			if( lines.Count > curLine ) checkExisting = bool.Parse( lines[curLine++] );
			if( lines.Count > curLine ) prioritizeNew = bool.Parse( lines[curLine++] );
			if( lines.Count > curLine ) cardsReviewedToday = int.Parse( lines[curLine++] );
			if( lines.Count > curLine ) targetNewPerReview = int.Parse( lines[curLine++] );
			if( lines.Count > curLine ) lastReviewDate = DateTime.Parse( lines[curLine++] );
			if( lines.Count > curLine ) allowReviewThresh = float.Parse( lines[curLine++] );
			if( lines.Count > curLine ) maxRepeatCardCount = int.Parse( lines[curLine++] );
			if( lines.Count > curLine ) maxRepeatTries = int.Parse( lines[curLine++] );
			if( lines.Count > curLine ) hardDelay = float.Parse( lines[curLine++] );
			if( lines.Count > curLine ) easyBuff = float.Parse( lines[curLine++] );
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
			saveData += maxRepeatCardCount.ToString() + '\n';
			saveData += maxRepeatTries.ToString() + '\n';
			saveData += hardDelay.ToString() + '\n';
			saveData += easyBuff.ToString() + '\n';

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

		public void UnReviewCard()
		{
			--cardsReviewedToday;
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

		public void SetRepeatCardCount( int count )
		{
			maxRepeatCardCount = count;
		}

		public void SetRepeatTries( int tries )
		{
			maxRepeatTries = tries;
		}

		public void SetHardDelay( float delay )
		{
			hardDelay = delay;
		}

		public void SetEasyBuff( float buff )
		{
			easyBuff = buff;
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

		public int GetRepeatCardCount()
		{
			return( maxRepeatCardCount );
		}

		public int GetRepeatTries()
		{
			return( maxRepeatTries );
		}

		public float GetHardDelay()
		{
			return( hardDelay );
		}

		public float GetEasyBuff()
		{
			return( easyBuff );
		}

		string mySavePath;

		DateTime lastSave;
		string deckName = "";
		int fRepair = 1;
		int nCardsPerReview = 20;
		float timeUpdateThresh = 0.2f;
		int maxDeckSize = 10000;
		float sickDelay = 3.0f;
		bool checkExisting = true; // true by default

		DateTime wholeDay;
		int cardsAddedToday = 0;

		bool prioritizeNew = false;
		int cardsReviewedToday = 0;

		int targetNewPerReview = 5;

		DateTime lastReviewDate;

		float allowReviewThresh = 0.0f;

		int maxRepeatCardCount = 5;
		int maxRepeatTries = 5;

		float hardDelay = 1.0f;
		float easyBuff = 2.0f;
	}
}
