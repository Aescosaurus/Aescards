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
		public enum Score
		{
			Fail = 0,
			Hard = 1,
			Good = 2,
			Easy = 3
		}

		public Card( int cardId,string front,string back,int fCount,float curScore,float daysTillNextReview )
		{
			LoadStats( cardId,front,back,fCount,curScore,daysTillNextReview );
		}

		Card( int cardId )
		{
			myId = cardId;
		}

		// return null if doesn't exist
		public static Card GenerateCard( int cardId )
		{
			var newCard = new Card( cardId );
			if( newCard.Load() ) return( newCard );
			else return( null );
		}

		public bool Load()
		{
			var cardPath = GeneratePath( myId );

			if( !File.Exists( cardPath ) ) return( false );

			var reader = new StreamReader( cardPath );
			var lines = new List<string>();
			while( !reader.EndOfStream )
			{
				string curLine = reader.ReadLine();
				lines.Add( curLine );
			}
			reader.Close();

			Debug.Assert( lines.Count == 5 );

			LoadStats( myId,lines[0],lines[1],int.Parse( lines[2] ),
				float.Parse( lines[3] ),float.Parse( lines[4] ) );
			// return ( new Card( cardId,lines[0],lines[1],int.Parse( lines[2] ),
			// 	float.Parse( lines[3] ),float.Parse( lines[4] ) ) );
			return( true );
		}

		void LoadStats( int cardId,string front,string back,int fCount,float curScore,float daysTillNextReview )
		{
			myId = cardId;
			SetFront( front );
			SetBack( back );
			// this.front = front;
			// this.back = back;
			this.fCount = fCount;
			this.curScore = curScore;
			this.daysTillNextReview = daysTillNextReview;
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

		public void SetFront( string front )
		{
			Debug.Assert( !front.Contains( "\n" ) );
			this.front = front;
		}

		public void SetBack( string back )
		{
			Debug.Assert( !back.Contains( "\n" ) );
			this.back = back;
		}

		public void UpdateScore( Score score,DeckData data )
		{
			if( curScore < 1.0f )
			{
				if( score != Score.Fail ) score = Score.Hard;
			}

			var fRepair = data.GetFRepair();
			switch( score )
			{
				case Score.Fail:
					++fCount;
					curScore = 0.0001f;
					// daysTillNextReview = 0;
					break;
				case Score.Hard:
					curScore = 1.0f / ( fCount + 1.0f );
					daysTillNextReview += data.GetHardDelay();
					// daysTillNextReview = 0.0001f;
					break;
				case Score.Good:
					fCount -= fRepair / 2;
					if( fCount < 0 ) fCount = 0;
					curScore = 1.0f + ( 1.0f / ( fCount + 1.0f ) );
					daysTillNextReview = 1;
					break;
				case Score.Easy:
					fCount -= fRepair;
					if( fCount < 0 ) fCount = 0;
					
					var easyBaseScore = data.GetEasyBaseScore();
					if( curScore < easyBaseScore ) curScore = easyBaseScore;

					curScore += 1.0f / ( fCount + 1.0f );
					curScore += data.GetEasyBuff();

					daysTillNextReview = curScore;
					break;
				default:
					Debug.Assert( false );
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

		public void ResetDaysTillNextReview()
		{
			daysTillNextReview = 0.0f;
		}

		public void ResetStats()
		{
			fCount = 0;
			curScore = 0.0f;
			ResetDaysTillNextReview();
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

		public bool IsNew()
		{
			return( curScore == 0.0f );
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

		public int repeatCount = 0; // not saved
    }
}
