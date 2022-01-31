using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Aescards
{
    class DeckData
    {
		public DeckData( string path )
		{
			if( File.Exists( path ) )
			{
				var reader = new StreamReader( path );
				var lines = new List<string>();
				while( !reader.EndOfStream ) lines.Add( reader.ReadLine() );
				reader.Close();

				lastSave = DateTime.Parse( lines[0] );
			}
		}

		public void Save( string path )
		{
			string saveData = "";
			saveData += lastSave.ToString();

			var writer = new StreamWriter( path );
			writer.Write( saveData );
			writer.Close();
		}

		public void UpdateTime()
		{
			lastSave = DateTime.Now;
		}

		public float GetDaysSinceLastOpened()
		{
			return( ( float )( ( DateTime.Now - lastSave ).TotalDays ) );
		}

		DateTime lastSave;
    }
}
