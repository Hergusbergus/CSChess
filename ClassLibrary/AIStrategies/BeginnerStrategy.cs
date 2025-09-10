using System;
using System.Collections;

namespace ChessLibrary.AIStrategies
{
    /// <summary>
    /// Beginner-level AI strategy that makes simple moves with minimal lookahead
    /// </summary>
    public class BeginnerStrategy : BaseAIStrategy
    {
        public override string StrategyName => "Beginner";
        public override int DifficultyLevel => 1;

        public override Move GetBestMove(Rules rules, Side side, int maxDepth, int maxTime)
        {
            ArrayList moves = rules.GenerateAllLegalMoves(side);
            if (moves.Count == 0)
                return null;

            // For beginners, just look 1 move ahead
            Move bestMove = null;
            int bestScore = MIN_SCORE;

            foreach (Move move in moves)
            {
                int moveResult = rules.DoMove(move);
                if (moveResult == 0)
                {
                    // Simple evaluation - just material and basic position
                    int score = EvaluatePosition(rules, side);
                    
                    // Add some randomness to make it less predictable
                    Random rand = new Random();
                    score += rand.Next(-50, 50);

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
    }
} 