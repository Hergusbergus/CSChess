/***************************************************************
 * File: Piece.cs
 * Created By: Justin Grindal		Date: 27 June, 2013
 * Description: A class for the chess piece. It stores the type of chess piece properties
 * like type, power etc. It also contains chess piece methods like
 * get next move, move, etc
 ***************************************************************/

using System;
using System.Collections.Generic;

namespace ChessLibrary 
{
	/// <summary>
	/// Defines the possible types of chess pieces
	/// </summary>
	public enum PieceType {Empty, King, Queen, Rook, Bishop, Knight, Pawn}

	/// <summary>
	/// Flyweight factory for chess pieces that manages shared piece properties
	/// </summary>
	public class PieceFactory
	{
		private static readonly Dictionary<PieceType, PieceFlyweight> _pieces = new Dictionary<PieceType, PieceFlyweight>();

		public static PieceFlyweight GetPiece(PieceType type)
		{
			if (!_pieces.ContainsKey(type))
			{
				_pieces[type] = new PieceFlyweight(type);
			}
			return _pieces[type];
		}
	}

	/// <summary>
	/// Flyweight class that contains shared piece properties
	/// </summary>
	public class PieceFlyweight
	{
		public PieceType Type { get; }
		public int BaseWeight { get; }

		public PieceFlyweight(PieceType type)
		{
			Type = type;
			BaseWeight = GetBaseWeight(type);
		}

		private int GetBaseWeight(PieceType type)
		{
			switch (type)
			{
				case PieceType.Pawn:
					return 100;
				case PieceType.Knight:
					return 320;
				case PieceType.Bishop:
					return 330;
				case PieceType.Rook:
					return 500;
				case PieceType.Queen:
					return 900;
				case PieceType.King:
					return 20000;
				default:
					return 0;
			}
		}

		public bool IsEmpty() => Type == PieceType.Empty;
		public bool IsPawn() => Type == PieceType.Pawn;
		public bool IsKnight() => Type == PieceType.Knight;
		public bool IsBishop() => Type == PieceType.Bishop;
		public bool IsRook() => Type == PieceType.Rook;
		public bool IsQueen() => Type == PieceType.Queen;
		public bool IsKing() => Type == PieceType.King;
	}

	/// <summary>
	/// Represents a chess piece with intrinsic (shared) and extrinsic (unique) properties
	/// </summary>
	[Serializable]
	public class Piece
	{
		private readonly PieceFlyweight m_flyweight;  // Shared properties
		private int m_moves;                          // Number of moves made
		private Side m_side;                          // Piece side (White/Black)

		public Piece(PieceType type, Side side)
		{
			m_flyweight = PieceFactory.GetPiece(type);
			m_side = side;
			m_moves = 0;
		}

		public Piece(PieceType type)
		{
			m_flyweight = PieceFactory.GetPiece(type);
			m_side = null;
			m_moves = 0;
		}

		// Intrinsic properties (shared)
		public PieceType Type => m_flyweight.Type;
		public int BaseWeight => m_flyweight.BaseWeight;

		// Extrinsic properties (unique to each piece)
		public int Moves
		{
			get { return m_moves; }
			set { m_moves = value; }
		}

		public Side Side
		{
			get { return m_side; }
			set { m_side = value; }
		}

		// Methods that use intrinsic properties
		public bool IsEmpty()
		{
			return m_flyweight.Type == PieceType.Empty;
		}

		public bool IsPawn()
		{
			return m_flyweight.Type == PieceType.Pawn;
		}

		public bool IsKing()
		{
			return m_flyweight.Type == PieceType.King;
		}

		public bool IsQueen()
		{
			return m_flyweight.Type == PieceType.Queen;
		}

		public bool IsRook()
		{
			return m_flyweight.Type == PieceType.Rook;
		}

		public bool IsBishop()
		{
			return m_flyweight.Type == PieceType.Bishop;
		}

		public bool IsKnight()
		{
			return m_flyweight.Type == PieceType.Knight;
		}

		public int GetWeight()
		{
			return m_flyweight.BaseWeight;
		}

		public void IncrementMoves()
		{
			m_moves++;
		}

		public void DecrementMoves()
		{
			if (m_moves > 0)
				m_moves--;
		}

		// returns the string for the piece
		public override string ToString()
		{
			switch (m_flyweight.Type)
			{
				case PieceType.King:
					return "King";
				case PieceType.Queen:
					return "Queen";
				case PieceType.Bishop:
					return "Bishop";
				case PieceType.Rook:
					return "Rook";
				case PieceType.Knight:
					return "Knight";
				case PieceType.Pawn:
					return "Pawn";
				default:
					return "E";
			}
		}
	}
}
