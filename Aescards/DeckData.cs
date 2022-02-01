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
			var path = GeneratePath( deckName );
			if( File.Exists( path ) )
			{
				var reader = new StreamReader( path );
				var lines = new List<string>();
				while( !reader.EndOfStream ) lines.Add( reader.ReadLine() );
				reader.Close();

				if( lines.Count > 0 ) lastSave = DateTime.Parse( lines[0] );
				if( lines.Count > 1 ) this.deckName = lines[1];
				if( lines.Count > 2 ) fRepair = int.Parse( lines[2] );
				if( lines.Count > 3 ) nCardsPerReview = int.Parse( lines[3] );
				if( lines.Count > 4 ) timeUpdateThresh = float.Parse( lines[4] );
				if( lines.Count > 5 ) maxDeckSize = int.Parse( lines[5] );
			}
		}

		public void Save( string deckName )
		{
			Debug.Assert( deckName.Length > 0 );

			var path = GeneratePath( deckName );

			string saveData = "";

			saveData += lastSave.ToString() + '\n';
			saveData += this.deckName + '\n';
			saveData += fRepair.ToString() + '\n';
			saveData += nCardsPerReview.ToString() + '\n';
			saveData += timeUpdateThresh.ToString() + '\n';
			saveData += maxDeckSize.ToString() + '\n';

			var writer = new StreamWriter( path );
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

		public void SetDeckName( string name )
		{
			deckName = name;
		}

		public float GetDaysSinceLastOpened()
		{
			return( ( float )( ( DateTime.Now - lastSave ).TotalDays ) );
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

		DateTime lastSave;
		string deckName = "";
		int fRepair = 1;
		int nCardsPerReview = 20;
		float timeUpdateThresh = 0.2f;
		int maxDeckSize = 10000;
	}
}
