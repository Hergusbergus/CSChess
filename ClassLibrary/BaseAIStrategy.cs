using System;
using System.Collections;

namespace ChessLibrary
{
    /// <summary>
    /// Base class for AI strategies that implements common functionality
    /// </summary>
    public abstract class BaseAIStrategy : IAIStrategy
    {
        protected const int MIN_SCORE = -1000000;
        protected const int MAX_SCORE = 1000000;

        public abstract string StrategyName { get; }
        public abstract int DifficultyLevel { get; }

        public abstract Move GetBestMove(Rules rules, Side side, int maxDepth, int maxTime);

        protected int EvaluatePosition(Rules rules, Side side)
        {
            int score = 0;
            ArrayList playerCells = rules.ChessBoard.GetSideCell(side.type);
            ArrayList enemyCells = rules.ChessBoard.GetSideCell(new Side(side.Enemy()).type);

            // Evaluate material
            foreach (string cellName in playerCells)
            {
                Cell cell = rules.ChessBoard[cellName];
                score += cell.piece.GetWeight();
            }

            foreach (string cellName in enemyCells)
            {
                Cell cell = rules.ChessBoard[cellName];
                score -= cell.piece.GetWeight();
            }

            // Evaluate position (center control, piece development, etc.)
            score += EvaluatePositionalFactors(rules, side);

            return score;
        }

        protected virtual int EvaluatePositionalFactors(Rules rules, Side side)
        {
            int score = 0;
            ArrayList playerCells = rules.ChessBoard.GetSideCell(side.type);

            foreach (string cellName in playerCells)
            {
                Cell cell = rules.ChessBoard[cellName];
                Piece piece = cell.piece;

                // Bonus for controlling center
                if (cell.row >= 3 && cell.row <= 6 && cell.col >= 3 && cell.col <= 6)
                {
                    score += 10;
                }

                // Bonus for piece development
                if (piece.IsPawn() && (cell.row == 3 || cell.row == 4))
                {
                    score += 5;
                }
                else if ((piece.IsKnight() || piece.IsBishop()) && cell.row >= 3)
                {
                    score += 10;
                }
            }

            return score;
        }

        protected int AlphaBeta(Rules rules, Side side, int depth, int alpha, int beta, bool maximizingPlayer)
        {
            if (depth == 0)
            {
                return EvaluatePosition(rules, side);
            }

            ArrayList moves = rules.GenerateAllLegalMoves(side);
            if (moves.Count == 0)
            {
                if (rules.IsCheckMate(side.type))
                {
                    return maximizingPlayer ? MIN_SCORE : MAX_SCORE;
                }
                return 0; // Stalemate
            }

            if (maximizingPlayer)
            {
                int maxEval = MIN_SCORE;
                foreach (Move move in moves)
                {
                    // Make move
                    int moveResult = rules.DoMove(move);
                    if (moveResult == 0)
                    {
                        int eval = AlphaBeta(rules, new Side(side.Enemy()), depth - 1, alpha, beta, false);
                        maxEval = Math.Max(maxEval, eval);
                        alpha = Math.Max(alpha, eval);
                        rules.UndoMove(move);
                        if (beta <= alpha)
                            break;
                    }
                }
                return maxEval;
            }
            else
            {
                int minEval = MAX_SCORE;
                foreach (Move move in moves)
                {
                    // Make move
                    int moveResult = rules.DoMove(move);
                    if (moveResult == 0)
                    {
                        int eval = AlphaBeta(rules, new Side(side.Enemy()), depth - 1, alpha, beta, true);
                        minEval = Math.Min(minEval, eval);
                        beta = Math.Min(beta, eval);
                        rules.UndoMove(move);
                        if (beta <= alpha)
                            break;
                    }
                }
                return minEval;
            }
        }
    }
} 