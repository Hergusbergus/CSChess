using System;
using System.Collections;

namespace ChessLibrary
{
    /// <summary>
    /// Interface defining the strategy for computer AI players
    /// </summary>
    public interface IAIStrategy
    {
        /// <summary>
        /// Gets the best move for the current board position
        /// </summary>
        /// <param name="rules">The current game rules</param>
        /// <param name="side">The side making the move</param>
        /// <param name="maxDepth">Maximum search depth</param>
        /// <param name="maxTime">Maximum time to think in milliseconds</param>
        /// <returns>The best move found</returns>
        Move GetBestMove(Rules rules, Side side, int maxDepth, int maxTime);

        /// <summary>
        /// Gets the name of the strategy
        /// </summary>
        string StrategyName { get; }

        /// <summary>
        /// Gets the difficulty level of the strategy (1-10)
        /// </summary>
        int DifficultyLevel { get; }
    }
} 