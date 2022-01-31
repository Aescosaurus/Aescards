using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

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

		public void UpdateScore( int score )
		{
			if( score < 1 ) // fail
			{
				++fCount;
				curScore = 0;
			}
			else if( score > 2 ) // easy
			{
				if( fCount > 0 ) --fCount;
				if( score > curScore ) curScore = score;

				curScore += 1.0f / fCount;
			}
			else curScore = score; // hard/good

			daysTillNextReview = curScore;
			modified = true;
		}

		// I don't know this card but also im sick of it so don't show me it for a few days
		public void Sick()
		{
			daysTillNextReview = sickReviewPenalty;
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

		const float sickReviewPenalty = 3.0f;
    }
}
