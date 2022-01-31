using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace Aescards
{
    class DeckData
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

				lastSave = DateTime.Parse( lines[0] );
				this.deckName = lines[1];
			}
		}

		public void Save( string deckName )
		{
			Debug.Assert( deckName.Length > 0 );

			var path = GeneratePath( deckName );
			string saveData = "";
			saveData += lastSave.ToString() + '\n';
			saveData += this.deckName + '\n';

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

		DateTime lastSave;
		string deckName = "";
	}
}
