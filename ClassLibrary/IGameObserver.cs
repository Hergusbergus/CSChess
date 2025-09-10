using System;

namespace ChessLibrary
{
    /// <summary>
    /// Interface for objects that want to observe game state changes
    /// </summary>
    public interface IGameObserver
    {
        /// <summary>
        /// Called when a move is made in the game
        /// </summary>
        /// <param name="move">The move that was made</param>
        void OnMoveMade(Move move);

        /// <summary>
        /// Called when the game state changes (e.g., check, checkmate, stalemate)
        /// </summary>
        /// <param name="gameState">The new game state</param>
        void OnGameStateChanged(GameState gameState);

        /// <summary>
        /// Called when a piece is captured
        /// </summary>
        /// <param name="piece">The piece that was captured</param>
        void OnPieceCaptured(Piece piece);
    }

    /// <summary>
    /// Represents the possible states of the game
    /// </summary>
    public enum GameState
    {
        Normal,
        Check,
        Checkmate,
        Stalemate,
        Draw
    }
} 