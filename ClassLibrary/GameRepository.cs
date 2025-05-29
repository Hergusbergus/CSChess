using System;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;
using System.Security.Cryptography;

namespace ChessLibrary
{
    /// <summary>
    /// Concrete implementation of IGameRepository that handles game state persistence using XML
    /// </summary>
    public class GameRepository : IGameRepository
    {
        private readonly Stack m_MovesHistory;
        private readonly Stack m_RedoMovesHistory;

        public GameRepository()
        {
            m_MovesHistory = new Stack();
            m_RedoMovesHistory = new Stack();
        }

        public void SaveGame(Game game, string filePath)
        {
            try
            {
                // Create the Game Xml 
                var gameXmlDocument = new XmlDocument();
                var gameXml = game.XmlSerialize(gameXmlDocument);

                gameXmlDocument.AppendChild(gameXmlDocument.CreateXmlDeclaration("1.0", "utf-8", null));
                gameXmlDocument.AppendChild(gameXml);

                // Save the file
                gameXmlDocument.Save(filePath);
            }
            catch (Exception ex) throw new Exception("Failed to save game: " + ex.Message, ex);
        }

        public Game LoadGame(string filePath)
        {
            try
            {
                // Create the Game Xml 
                var gameXmlDocument = new XmlDocument();
                gameXmlDocument.Load(filePath);

                var gameNode = gameXmlDocument.FirstChild;
                if (gameNode.NodeType == XmlNodeType.XmlDeclaration) gameNode = gameNode.NextSibling;

                // Create new game and deserialize
                var game = new Game();
                game.XmlDeserialize(gameNode);

                // Restore move history
                var moves = game.MoveHistory.ToArray();
                m_MovesHistory.Clear();
                for (var i = moves.Length - 1; i >= 0; i--) m_MovesHistory.Push(moves[i]);

                return game;
            }
            catch (Exception ex) throw new Exception("Failed to load game: " + ex.Message, ex);
        }

        public Stack GetMoveHistory(Game game) { return m_MovesHistory; }

        public Stack GetRedoMoveHistory(Game game) { return m_RedoMovesHistory; }

        public void AddMove(Game game, Move move)
        {
            m_MovesHistory.Push(move);
            m_RedoMovesHistory.Clear(); // Clear redo stack when new move is made
        }

        public bool UndoMove(Game game)
        {
            if (m_MovesHistory.Count > 0)
            {
                var move = (Move)m_MovesHistory.Pop();
                m_RedoMovesHistory.Push(move);
                game.Rules.UndoMove(move);
                game.NextPlayerTurn();
                return true;
            }
            return false;
        }

        public bool RedoMove(Game game)
        {
            if (m_RedoMovesHistory.Count > 0)
            {
                var move = (Move)m_RedoMovesHistory.Pop();
                m_MovesHistory.Push(move);
                game.Rules.DoMove(move);
                game.NextPlayerTurn();
                return true;
            }
            return false;
        }
    }
} 