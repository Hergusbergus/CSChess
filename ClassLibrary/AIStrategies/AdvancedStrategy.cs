using System;
using System.Collections;

namespace ChessLibrary.AIStrategies
{
    /// <summary>
    /// Advanced-level AI strategy that uses deeper search and more sophisticated evaluation
    /// </summary>
    public class AdvancedStrategy : BaseAIStrategy
    {
        private const int MAX_DEPTH = 4;
        private const int QUIESCENCE_DEPTH = 2;

        public override string StrategyName => "Advanced";
        public override int DifficultyLevel => 8;

        public override Move GetBestMove(Rules rules, Side side, int maxDepth, int maxTime)
        {
            ArrayList moves = rules.GenerateAllLegalMoves(side);
            if (moves.Count == 0)
                return null;

            Move bestMove = null;
            int bestScore = MIN_SCORE;
            DateTime startTime = DateTime.Now;
            int currentDepth = 1;

            // Iterative deepening
            while (currentDepth <= MAX_DEPTH)
            {
                Move currentBestMove = null;
                int currentBestScore = MIN_SCORE;

                foreach (Move move in moves)
                {
                    // Check if we've exceeded the time limit
                    if ((DateTime.Now - startTime).TotalMilliseconds > maxTime)
                        break;

                    int moveResult = rules.DoMove(move);
                    if (moveResult == 0)
                    {
                        int score = AlphaBeta(rules, new Side(side.Enemy()), currentDepth, MIN_SCORE, MAX_SCORE, false);
                        
                        // Add quiescence search for more stable evaluation
                        if (currentDepth == MAX_DEPTH)
                        {
                            score = QuiescenceSearch(rules, side, score, MIN_SCORE, MAX_SCORE, QUIESCENCE_DEPTH);
                        }

                        if (score > currentBestScore)
                        {
                            currentBestScore = score;
                            currentBestMove = move;
                        }
                        rules.UndoMove(move);
                    }
                }

                // Update best move if we completed the depth
                if ((DateTime.Now - startTime).TotalMilliseconds <= maxTime)
                {
                    bestMove = currentBestMove;
                    bestScore = currentBestScore;
                }
                else
                {
                    break;
                }

                currentDepth++;
            }

            return bestMove;
        }

        private int QuiescenceSearch(Rules rules, Side side, int alpha, int beta, int depth, int maxDepth)
        {
            int standPat = EvaluatePosition(rules, side);
            if (depth == 0)
                return standPat;

            if (standPat >= beta)
                return beta;
            if (alpha < standPat)
                alpha = standPat;

            ArrayList captures = GetCaptures(rules, side);
            foreach (Move move in captures)
            {
                int moveResult = rules.DoMove(move);
                if (moveResult == 0)
                {
                    int score = -QuiescenceSearch(rules, new Side(side.Enemy()), -beta, -alpha, depth - 1, maxDepth);
                    rules.UndoMove(move);

                    if (score >= beta)
                        return beta;
                    if (score > alpha)
                        alpha = score;
                }
            }

            return alpha;
        }

        private ArrayList GetCaptures(Rules rules, Side side)
        {
            ArrayList moves = rules.GenerateAllLegalMoves(side);
            ArrayList captures = new ArrayList();

            foreach (Move move in moves)
            {
                if (!move.EndCell.IsEmpty())
                {
                    captures.Add(move);
                }
            }

            return captures;
        }

        protected override int EvaluatePositionalFactors(Rules rules, Side side)
        {
            int score = base.EvaluatePositionalFactors(rules, side);
            ArrayList playerCells = rules.ChessBoard.GetSideCell(side.type);
            ArrayList enemyCells = rules.ChessBoard.GetSideCell(new Side(side.Enemy()).type);

            // Advanced positional evaluation
            foreach (string cellName in playerCells)
            {
                Cell cell = rules.ChessBoard[cellName];
                Piece piece = cell.piece;

                // Piece-specific positional bonuses
                if (piece.IsPawn())
                {
                    // Doubled pawns penalty
                    bool isDoubled = false;
                    for (int row = 1; row <= 8; row++)
                    {
                        if (row != cell.row)
                        {
                            Cell otherCell = rules.ChessBoard[row, cell.col];
                            if (!otherCell.IsEmpty() && otherCell.piece.IsPawn() && otherCell.piece.Side.type == side.type)
                            {
                                isDoubled = true;
                                break;
                            }
                        }
                    }
                    if (isDoubled)
                        score -= 20;

                    // Passed pawn bonus
                    bool isPassed = true;
                    int enemyPawnRow = side.isWhite() ? cell.row - 1 : cell.row + 1;
                    for (int col = Math.Max(1, cell.col - 1); col <= Math.Min(8, cell.col + 1); col++)
                    {
                        if (enemyPawnRow >= 1 && enemyPawnRow <= 8)
                        {
                            Cell enemyPawnCell = rules.ChessBoard[enemyPawnRow, col];
                            if (!enemyPawnCell.IsEmpty() && enemyPawnCell.piece.IsPawn())
                            {
                                isPassed = false;
                                break;
                            }
                        }
                    }
                    if (isPassed)
                        score += 30;
                }
                else if (piece.IsKnight())
                {
                    // Knights are better in the center
                    double centerDistance = Math.Abs(4.5 - cell.row) + Math.Abs(4.5 - cell.col);
                    score += (int)(20 - centerDistance * 2);
                }
                else if (piece.IsBishop())
                {
                    // Bishops are better with open diagonals
                    int openDiagonals = CountOpenDiagonals(rules, cell);
                    score += openDiagonals * 10;
                }
                else if (piece.IsRook())
                {
                    // Rooks are better on open files
                    int openFile = IsOpenFile(rules, cell.col) ? 1 : 0;
                    score += openFile * 15;
                }
                else if (piece.IsQueen())
                {
                    // Queens are better in the center in the middlegame
                    if (IsMiddlegame(rules))
                    {
                        double centerDistance = Math.Abs(4.5 - cell.row) + Math.Abs(4.5 - cell.col);
                        score += (int)(10 - centerDistance);
                    }
                }
                else if (piece.IsKing())
                {
                    // King safety evaluation
                    if (IsEndgame(rules))
                    {
                        // In endgame, king should be active
                        double centerDistance = Math.Abs(4.5 - cell.row) + Math.Abs(4.5 - cell.col);
                        score += (int)(centerDistance * -5);
                    }
                    else
                    {
                        // In middlegame, king should be safe
                        if (IsKingSafe(rules, cell))
                            score += 30;
                    }
                }
            }

            return score;
        }

        private bool IsMiddlegame(Rules rules)
        {
            int totalPieces = 0;
            for (int row = 1; row <= 8; row++)
            {
                for (int col = 1; col <= 8; col++)
                {
                    if (!rules.ChessBoard[row, col].IsEmpty())
                        totalPieces++;
                }
            }
            return totalPieces > 10 && totalPieces < 20;
        }

        private bool IsEndgame(Rules rules)
        {
            int totalPieces = 0;
            for (int row = 1; row <= 8; row++)
            {
                for (int col = 1; col <= 8; col++)
                {
                    if (!rules.ChessBoard[row, col].IsEmpty())
                        totalPieces++;
                }
            }
            return totalPieces <= 10;
        }

        private int CountOpenDiagonals(Rules rules, Cell cell)
        {
            int openDiagonals = 0;
            int[][] directions = new int[][] { 
                new int[] { -1, -1 }, new int[] { -1, 1 },
                new int[] { 1, -1 }, new int[] { 1, 1 }
            };

            foreach (int[] dir in directions)
            {
                bool isOpen = true;
                int row = cell.row + dir[0];
                int col = cell.col + dir[1];

                while (row >= 1 && row <= 8 && col >= 1 && col <= 8)
                {
                    if (!rules.ChessBoard[row, col].IsEmpty())
                    {
                        isOpen = false;
                        break;
                    }
                    row += dir[0];
                    col += dir[1];
                }

                if (isOpen)
                    openDiagonals++;
            }

            return openDiagonals;
        }

        private bool IsOpenFile(Rules rules, int col)
        {
            for (int row = 1; row <= 8; row++)
            {
                if (!rules.ChessBoard[row, col].IsEmpty() && rules.ChessBoard[row, col].piece.IsPawn())
                    return false;
            }
            return true;
        }

        private bool IsKingSafe(Rules rules, Cell kingCell)
        {
            // Check if king is castled
            if (kingCell.col == 2 || kingCell.col == 6)
                return true;

            // Check if king has pawn shield
            int pawnRow = kingCell.piece.Side.isWhite() ? kingCell.row - 1 : kingCell.row + 1;
            int pawnCount = 0;

            for (int col = Math.Max(1, kingCell.col - 1); col <= Math.Min(8, kingCell.col + 1); col++)
            {
                if (pawnRow >= 1 && pawnRow <= 8)
                {
                    Cell pawnCell = rules.ChessBoard[pawnRow, col];
                    if (!pawnCell.IsEmpty() && pawnCell.piece.IsPawn() && pawnCell.piece.Side.type == kingCell.piece.Side.type)
                        pawnCount++;
                }
            }

            return pawnCount >= 2;
        }
    }
} 