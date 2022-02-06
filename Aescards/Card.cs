using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace Aescards
{
    public class Card
    {
		public Card( int cardId,string front,string back,int fCount,float curScore,float daysTillNextReview )
		{
			myId = cardId;
			this.front = front;
			this.back = back;
			this.fCount = fCount;
			this.curScore = curScore;
			this.daysTillNextReview = daysTillNextReview;
		}

		// return null if doesn't exist
		public static Card GenerateCard( int cardId )
		{
			var cardPath = GeneratePath( cardId );

			if( !File.Exists( cardPath ) ) return( null );

			var reader = new StreamReader( cardPath );
			var lines = new List<string>();
			while( !reader.EndOfStream )
			{
				string curLine = reader.ReadLine();
				lines.Add( curLine );
			}
			reader.Close();

			Debug.Assert( lines.Count == 5 );

			return( new Card( cardId,lines[0],lines[1],int.Parse( lines[2] ),
				float.Parse( lines[3] ),float.Parse( lines[4] ) ) );
		}

		public void Save()
		{
			string saveStr = "";
			saveStr += front + '\n';
			saveStr += back + '\n';
			saveStr += fCount.ToString() + '\n';
			saveStr += curScore.ToString() + '\n';
			saveStr += daysTillNextReview.ToString() + '\n';

			var writer = new StreamWriter( GeneratePath( myId ) );
			writer.Write( saveStr );
			writer.Close();
		}

		public void UpdateScore( int score,int fRepair )
		{
			switch( score )
			{
				case 0: // fail
					++fCount;
					curScore = 0.0001f;
					daysTillNextReview = 0;
					break;
				case 1: // hard 0-1
					curScore = 1.0f / ( fCount + 1.0f );
					daysTillNextReview = 0.0001f;
					break;
				case 2: // good 1-2
					curScore = 1.0f + ( 1.0f / ( fCount + 1.0f ) );
					daysTillNextReview = 1;
					break;
				default: // easy, >= 3
					fCount -= fRepair;
					if( fCount < 0 ) fCount = 0;

					if( score > curScore ) curScore = score;

					curScore += 1.0f / ( fCount + 1 );

					daysTillNextReview = curScore;
					break;
			}

			// -1 since hard is 0-1
			// daysTillNextReview = curScore - 1.0f;

			modified = true;
		}

		public void UpdateDaysTillNextReview( float daysPassed )
		{
			daysTillNextReview -= daysPassed;

			modified = true;
		}

		// I don't know this card but also im sick of it so don't show me it for a few days
		public void Sick( float sickDelay )
		{
			daysTillNextReview = sickDelay;

			modified = true;
		}

		static string GeneratePath( int cardId )
		{
			return( CardHandler.curCardPath + cardId.ToString() + ".txt" );
		}

		public bool IsModified()
		{
			return( modified );
		}

		public int GetId()
		{
			return( myId );
		}

		public string GetFront()
		{
			return( front );
		}

		public string GetBack()
		{
			return( back.Replace( "\\n","\n" ) );
		}

		public int GetFCount()
		{
			return( fCount );
		}

		public float GetCurScore()
		{
			return( curScore );
		}

		public float GetDaysTillNextReview()
		{
			return( daysTillNextReview );
		}

		int myId;
		bool modified = false;

		// Stuff that goes in file
		string front; // front of the card
		string back; // back of the card
		int fCount; // how many f's we've gotten so far, every easy removes some f's.
		float curScore; // 0 = hard f, <1 = next review, <2 = tomorrow, 2-3 = keep incrementing
		float daysTillNextReview; // decrement every day, <0 = time to review

		public static readonly string folderPath = "Cards/";

		// const float sickReviewPenalty = 3.0f;
    }
}
