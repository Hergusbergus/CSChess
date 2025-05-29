using System;
using System.Collections;

namespace ChessLibrary.AIStrategies
{
    /// <summary>
    /// Intermediate-level AI strategy that uses alpha-beta pruning with moderate depth
    /// </summary>
    public class IntermediateStrategy : BaseAIStrategy
    {
        public override string StrategyName => "Intermediate";
        public override int DifficultyLevel => 5;

        public override Move GetBestMove(Rules rules, Side side, int maxDepth, int maxTime)
        {
            ArrayList moves = rules.GenerateAllLegalMoves(side);
            if (moves.Count == 0)
                return null;

            // For intermediate, use alpha-beta with depth 3
            Move bestMove = null;
            int bestScore = MIN_SCORE;
            DateTime startTime = DateTime.Now;

            foreach (Move move in moves)
            {
                // Check if we've exceeded the time limit
                if ((DateTime.Now - startTime).TotalMilliseconds > maxTime)
                    break;

                int moveResult = rules.DoMove(move);
                if (moveResult == 0)
                {
                    int score = AlphaBeta(rules, new Side(side.Enemy()), 3, MIN_SCORE, MAX_SCORE, false);
                    
                    if (score > bestScore)
                    {
                        bestScore = score;
                        bestMove = move;
                    }
                    rules.UndoMove(move);
                }
            }

            return bestMove;
        }

        protected override int EvaluatePositionalFactors(Rules rules, Side side)
        {
            int score = base.EvaluatePositionalFactors(rules, side);
            ArrayList playerCells = rules.ChessBoard.GetSideCell(side.type);
            ArrayList enemyCells = rules.ChessBoard.GetSideCell(new Side(side.Enemy()).type);

            // Additional positional evaluation for intermediate level
            foreach (string cellName in playerCells)
            {
                Cell cell = rules.ChessBoard[cellName];
                Piece piece = cell.piece;

                // Bonus for castled position
                if (piece.IsKing() && (cell.col == 2 || cell.col == 6))
                {
                    score += 30;
                }

                // Bonus for connected rooks
                if (piece.IsRook())
                {
                    for (int col = 1; col <= 8; col++)
                    {
                        if (col != cell.col)
                        {
                            Cell otherCell = rules.ChessBoard[cell.row, col];
                            if (!otherCell.IsEmpty() && otherCell.piece.IsRook() && otherCell.piece.Side.type == side.type)
                            {
                                score += 15;
                            }
                        }
                    }
                }

                // Penalty for isolated pawns
                if (piece.IsPawn())
                {
                    bool hasAdjacentPawn = false;
                    if (cell.col > 1)
                    {
                        Cell leftCell = rules.ChessBoard[cell.row, cell.col - 1];
                        if (!leftCell.IsEmpty() && leftCell.piece.IsPawn() && leftCell.piece.Side.type == side.type)
                            hasAdjacentPawn = true;
                    }
                    if (cell.col < 8)
                    {
                        Cell rightCell = rules.ChessBoard[cell.row, cell.col + 1];
                        if (!rightCell.IsEmpty() && rightCell.piece.IsPawn() && rightCell.piece.Side.type == side.type)
                            hasAdjacentPawn = true;
                    }
                    if (!hasAdjacentPawn)
                        score -= 10;
                }
            }

            return score;
        }
    }
} 