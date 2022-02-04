using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Aescards
{
	class AescardsSettings
	{
		public AescardsSettings()
		{
			if( File.Exists( path ) )
			{
				var reader = new StreamReader( path );
				var lines = new List<string>();
				while( !reader.EndOfStream ) lines.Add( reader.ReadLine() );
				reader.Close();

				if( lines.Count > 0 ) colorScheme = int.Parse( lines[0] );
			}
		}

		public void Save()
		{
			string saveText = "";
			
			saveText += colorScheme.ToString();

			var writer = new StreamWriter( path );
			writer.Write( saveText );
			writer.Close();
		}

		public void SetColorScheme( int id )
		{
			colorScheme = id;
		}

		public int GetColorScheme()
		{
			return( colorScheme );
		}

		static readonly string path = DeckPage.deckPath + "AescardsSettings.txt";

		int colorScheme = 0;
	}
}
