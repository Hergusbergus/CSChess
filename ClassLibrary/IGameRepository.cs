using System;
using System.Collections;
using ChessLibrary;

namespace ChessLibrary
{
    /// <summary>
    /// Interface defining the contract for game state persistence and management
    /// </summary>
    public interface IGameRepository
    {
        /// <summary>
        /// Saves the current game state
        /// </summary>
        /// <param name="game">The game to save</param>
        /// <param name="filePath">Path where to save the game</param>
        void SaveGame(Game game, string filePath);

        /// <summary>
        /// Loads a game state from storage
        /// </summary>
        /// <param name="filePath">Path from where to load the game</param>
        /// <returns>The loaded game</returns>
        Game LoadGame(string filePath);

        /// <summary>
        /// Gets the move history for a game
        /// </summary>
        /// <param name="game">The game to get history for</param>
        /// <returns>Stack of moves</returns>
        Stack GetMoveHistory(Game game);

        /// <summary>
        /// Gets the redo move history for a game
        /// </summary>
        /// <param name="game">The game to get redo history for</param>
        /// <returns>Stack of redo moves</returns>
        Stack GetRedoMoveHistory(Game game);

        /// <summary>
        /// Adds a move to the game history
        /// </summary>
        /// <param name="game">The game to add the move to</param>
        /// <param name="move">The move to add</param>
        void AddMove(Game game, Move move);

        /// <summary>
        /// Undoes the last move in the game
        /// </summary>
        /// <param name="game">The game to undo the move in</param>
        /// <returns>True if move was undone, false if no moves to undo</returns>
        bool UndoMove(Game game);

        /// <summary>
        /// Redoes the last undone move in the game
        /// </summary>
        /// <param name="game">The game to redo the move in</param>
        /// <returns>True if move was redone, false if no moves to redo</returns>
        bool RedoMove(Game game);
    }
} 