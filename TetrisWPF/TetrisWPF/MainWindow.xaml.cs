using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace TetrisWPF
{
    public class Position
    {
        public int Row { get; set; }
        public int Column { get; set; }

        public Position(int row, int column)
        {
            Row = row;
            Column = column;
        }
    }

    public abstract class Block
    {
        protected abstract Position[][] Tiles { get; }
        protected abstract Position StartOffset { get; }
        public abstract int Id { get; }

        private int rotationState;
        private Position offset;

        public Position Offset => offset;

        public Block()
        {
            offset = new Position(StartOffset.Row, StartOffset.Column);
        }

        public IEnumerable<Position> TilePositions()
        {
            foreach (Position p in Tiles[rotationState])
            {
                yield return new Position(p.Row + offset.Row, p.Column + offset.Column);
            }
        }

        public void RotateClockwise()
        {
            rotationState = (rotationState + 1) % Tiles.Length;
        }

        public void RotateCounterClockwise()
        {
            if (rotationState == 0)
                rotationState = Tiles.Length - 1;
            else
                rotationState--;
        }

        public void Move(int rows, int columns)
        {
            offset.Row += rows;
            offset.Column += columns;
        }

        public void Reset()
        {
            rotationState = 0;
            offset.Row = StartOffset.Row;
            offset.Column = StartOffset.Column;
        }

        public Block Clone()
        {
            Block clone = (Block)MemberwiseClone();
            clone.offset = new Position(offset.Row, offset.Column);
            return clone;
        }
    }

    public class IBlock : Block
    {
        private readonly Position[][] tiles = new Position[][]
        {
            new Position[] { new(1,0), new(1,1), new(1,2), new(1,3) },
            new Position[] { new(0,2), new(1,2), new(2,2), new(3,2) },
            new Position[] { new(2,0), new(2,1), new(2,2), new(2,3) },
            new Position[] { new(0,1), new(1,1), new(2,1), new(3,1) }
        };

        public override int Id => 1;
        protected override Position StartOffset => new Position(-1, 3);
        protected override Position[][] Tiles => tiles;
    }

    public class JBlock : Block
    {
        private readonly Position[][] tiles = new Position[][]
        {
            new Position[] { new(0,0), new(1,0), new(1,1), new(1,2) },
            new Position[] { new(0,1), new(0,2), new(1,1), new(2,1) },
            new Position[] { new(1,0), new(1,1), new(1,2), new(2,2) },
            new Position[] { new(0,1), new(1,1), new(2,0), new(2,1) }
        };

        public override int Id => 2;
        protected override Position StartOffset => new Position(0, 3);
        protected override Position[][] Tiles => tiles;
    }

    public class LBlock : Block
    {
        private readonly Position[][] tiles = new Position[][]
        {
            new Position[] { new(0,2), new(1,0), new(1,1), new(1,2) },
            new Position[] { new(0,1), new(1,1), new(2,1), new(2,2) },
            new Position[] { new(1,0), new(1,1), new(1,2), new(2,0) },
            new Position[] { new(0,0), new(0,1), new(1,1), new(2,1) }
        };

        public override int Id => 3;
        protected override Position StartOffset => new Position(0, 3);
        protected override Position[][] Tiles => tiles;
    }

    public class OBlock : Block
    {
        private readonly Position[][] tiles = new Position[][]
        {
            new Position[] { new(0,0), new(0,1), new(1,0), new(1,1) }
        };

        public override int Id => 4;
        protected override Position StartOffset => new Position(0, 4);
        protected override Position[][] Tiles => tiles;
    }

    public class SBlock : Block
    {
        private readonly Position[][] tiles = new Position[][]
        {
            new Position[] { new(0,1), new(0,2), new(1,0), new(1,1) },
            new Position[] { new(0,1), new(1,1), new(1,2), new(2,2) },
            new Position[] { new(1,1), new(1,2), new(2,0), new(2,1) },
            new Position[] { new(0,0), new(1,0), new(1,1), new(2,1) }
        };

        public override int Id => 5;
        protected override Position StartOffset => new Position(0, 3);
        protected override Position[][] Tiles => tiles;
    }

    public class TBlock : Block
    {
        private readonly Position[][] tiles = new Position[][]
        {
            new Position[] { new(0,1), new(1,0), new(1,1), new(1,2) },
            new Position[] { new(0,1), new(1,1), new(1,2), new(2,1) },
            new Position[] { new(1,0), new(1,1), new(1,2), new(2,1) },
            new Position[] { new(0,1), new(1,0), new(1,1), new(2,1) }
        };

        public override int Id => 6;
        protected override Position StartOffset => new Position(0, 3);
        protected override Position[][] Tiles => tiles;
    }

    public class ZBlock : Block
    {
        private readonly Position[][] tiles = new Position[][]
        {
            new Position[] { new(0,0), new(0,1), new(1,1), new(1,2) },
            new Position[] { new(0,2), new(1,1), new(1,2), new(2,1) },
            new Position[] { new(1,0), new(1,1), new(2,1), new(2,2) },
            new Position[] { new(0,1), new(1,0), new(1,1), new(2,0) }
        };

        public override int Id => 7;
        protected override Position StartOffset => new Position(0, 3);
        protected override Position[][] Tiles => tiles;
    }

    public class GameState
    {
        private readonly int[,] gameGrid;
        private readonly int rows;
        private readonly int cols;
        private Block currentBlock;
        private readonly Queue<Block> nextBlocks;
        private Block heldBlock;
        private bool canHold;
        private readonly Random random = new Random();

        public int Score { get; private set; }
        public int Level { get; private set; }
        public int Lines { get; private set; }
        public bool IsGameOver { get; private set; }
        public Block CurrentBlock => currentBlock;
        public Block HeldBlock => heldBlock;
        public Block[] NextBlocks => nextBlocks.ToArray();
        public int[,] GameGrid => gameGrid;

        public GameState(int rows, int cols)
        {
            this.rows = rows;
            this.cols = cols;
            gameGrid = new int[rows, cols];
            nextBlocks = new Queue<Block>();
            Reset();
        }

        private Block GetRandomBlock()
        {
            return random.Next(7) switch
            {
                0 => new IBlock(),
                1 => new JBlock(),
                2 => new LBlock(),
                3 => new OBlock(),
                4 => new SBlock(),
                5 => new TBlock(),
                _ => new ZBlock()
            };
        }

        private void AddNextBlock()
        {
            if (nextBlocks.Count <= 3)
            {
                for (int i = 0; i < 3; i++)
                {
                    nextBlocks.Enqueue(GetRandomBlock());
                }
            }
        }

        public void Reset()
        {
            for (int r = 0; r < rows; r++)
                for (int c = 0; c < cols; c++)
                    gameGrid[r, c] = 0;

            Score = 0;
            Level = 1;
            Lines = 0;
            IsGameOver = false;
            nextBlocks.Clear();
            AddNextBlock();
            heldBlock = null;
            canHold = true;
            SpawnBlock();
        }

        public void HoldBlock()
        {
            if (!canHold) return;

            if (heldBlock == null)
            {
                heldBlock = currentBlock.Clone();
                currentBlock = nextBlocks.Dequeue();
                AddNextBlock();
            }
            else
            {
                Block temp = currentBlock;
                currentBlock = heldBlock;
                heldBlock = temp;
            }

            currentBlock.Reset();
            canHold = false;
        }

        public void SpawnBlock()
        {
            currentBlock = nextBlocks.Dequeue();
            AddNextBlock();
            canHold = true;

            if (!BlockFits())
            {
                IsGameOver = true;
            }
        }

        private bool BlockFits()
        {
            foreach (Position p in currentBlock.TilePositions())
            {
                if (!IsCellEmpty(p.Row, p.Column))
                    return false;
            }
            return true;
        }

        public void RotateBlockCW()
        {
            currentBlock.RotateClockwise();
            if (!BlockFits())
            {
                currentBlock.RotateCounterClockwise();
            }
        }

        public void RotateBlockCCW()
        {
            currentBlock.RotateCounterClockwise();
            if (!BlockFits())
            {
                currentBlock.RotateClockwise();
            }
        }

        public void MoveBlockLeft()
        {
            currentBlock.Move(0, -1);
            if (!BlockFits())
            {
                currentBlock.Move(0, 1);
            }
        }

        public void MoveBlockRight()
        {
            currentBlock.Move(0, 1);
            if (!BlockFits())
            {
                currentBlock.Move(0, -1);
            }
        }

        public bool MoveBlockDown()
        {
            currentBlock.Move(1, 0);
            if (!BlockFits())
            {
                currentBlock.Move(-1, 0);
                return false;
            }
            return true;
        }

        public void DropBlock()
        {
            while (MoveBlockDown())
            {
                Score += 2;
            }
        }

        public void PlaceBlock()
        {
            foreach (Position p in currentBlock.TilePositions())
            {
                gameGrid[p.Row, p.Column] = currentBlock.Id;
            }

            Score += 10;
            if (Score > Level * 100)
            {
                Level++;
            }
        }

        private bool IsInGrid(int row, int col)
        {
            return row >= 0 && row < rows && col >= 0 && col < cols;
        }

        private bool IsCellEmpty(int row, int col)
        {
            return IsInGrid(row, col) && gameGrid[row, col] == 0;
        }

        public int ClearFullRows()
        {
            int cleared = 0;
            for (int row = rows - 1; row >= 0; row--)
            {
                if (IsRowFull(row))
                {
                    ClearRow(row);
                    cleared++;
                }
                else if (cleared > 0)
                {
                    MoveRowDown(row, cleared);
                }
            }

            if (cleared > 0)
            {
                Score += cleared * 100 * Level;
                Lines += cleared;
            }

            return cleared;
        }

        private bool IsRowFull(int row)
        {
            for (int col = 0; col < cols; col++)
            {
                if (gameGrid[row, col] == 0)
                    return false;
            }
            return true;
        }

        private void ClearRow(int row)
        {
            for (int col = 0; col < cols; col++)
            {
                gameGrid[row, col] = 0;
            }
        }

        private void MoveRowDown(int row, int numRows)
        {
            for (int col = 0; col < cols; col++)
            {
                gameGrid[row + numRows, col] = gameGrid[row, col];
                gameGrid[row, col] = 0;
            }
        }
    }

    public partial class MainWindow : Window
    {
        private readonly int rows = 20;
        private readonly int cols = 10;
        private readonly int blockSize = 46;
        private readonly Image[,] imageControls;
        private readonly GameState gameState;
        private readonly DispatcherTimer gameTimer;
        private bool gameOver = false;
        private bool isPaused = false;
        private readonly List<int> highScores = new List<int>();

        public MainWindow()
        {
            InitializeComponent();
            imageControls = SetupGameCanvas();
            gameState = new GameState(rows, cols);
            gameTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(800)
            };
            gameTimer.Tick += GameTimer_Tick;
            LoadHighScores();
            InitializeGame();
        }

        private readonly ImageSource[] BlockImages = new ImageSource[]
        {
            new BitmapImage(new Uri("Assets/Block-Empty.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Block-I.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Block-J.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Block-L.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Block-O.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Block-S.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Block-T.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Block-Z.png", UriKind.Relative))
        };

        private readonly ImageSource[] TileImages = new ImageSource[]
        {
            new BitmapImage(new Uri("Assets/TileEmpty.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TileCyan.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TileBlue.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TileOrange.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TileYellow.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TileGreen.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TilePurple.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TileRed.png", UriKind.Relative))
        };

        private Image[,] SetupGameCanvas()
        {
            Image[,] imageControls = new Image[rows, cols];
            GameCanvas.Children.Clear();

            GameCanvas.Width = cols * blockSize;
            GameCanvas.Height = rows * blockSize;

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    Image imageControl = new Image
                    {
                        Width = blockSize,
                        Height = blockSize
                    };

                    Canvas.SetTop(imageControl, r * blockSize);
                    Canvas.SetLeft(imageControl, c * blockSize);
                    GameCanvas.Children.Add(imageControl);
                    imageControls[r, c] = imageControl;
                }
            }

            return imageControls;
        }

        private void DrawGrid()
        {
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    int id = gameState.GameGrid[r, c];
                    imageControls[r, c].Source = TileImages[id];
                }
            }
        }

        private void DrawBlock(Block block)
        {
            foreach (Position p in block.TilePositions())
            {
                imageControls[p.Row, p.Column].Source = TileImages[block.Id];
            }
        }

        private void Draw()
        {
            DrawGrid();
            DrawBlock(gameState.CurrentBlock);
            DrawNextBlocks();
            DrawHeldBlock();
            ScoreText.Text = $"{gameState.Score}";
            LevelText.Text = $"{gameState.Level}";
            LinesText.Text = $"{gameState.Lines}";
        }

        private void DrawNextBlocks()
        {
            Block[] nextBlocks = gameState.NextBlocks;
            
            ClearCanvas(NextCanvas1);
            ClearCanvas(NextCanvas2);
            ClearCanvas(NextCanvas3);
            
            DrawBlockToCanvas(nextBlocks[0], NextCanvas1);
            DrawBlockToCanvas(nextBlocks[1], NextCanvas2);
            DrawBlockToCanvas(nextBlocks[2], NextCanvas3);
        }

        private void DrawHeldBlock()
        {
            ClearCanvas(HoldCanvas);
            if (gameState.HeldBlock != null)
            {
                DrawBlockToCanvas(gameState.HeldBlock, HoldCanvas);
            }
        }

        private void DrawBlockToCanvas(Block block, Canvas canvas)
        {
            canvas.Children.Clear();

            double previewBlockSize = Math.Min(
                (canvas.Width - 20) / 4,  
                (canvas.Height - 20) / 4
            );

            foreach (Position p in block.TilePositions())
            {
                Image image = new Image
                {
                    Width = previewBlockSize,
                    Height = previewBlockSize,
                    Source = TileImages[block.Id]
                };

                double offsetX = (canvas.Width - 4 * previewBlockSize) / 2;
                double offsetY = (canvas.Height - 4 * previewBlockSize) / 2;

                Canvas.SetTop(image, (p.Row - block.Offset.Row) * previewBlockSize + offsetY);
                Canvas.SetLeft(image, (p.Column - block.Offset.Column) * previewBlockSize + offsetX);
                canvas.Children.Add(image);
            }
        }

        private void ClearCanvas(Canvas canvas)
        {
            canvas.Children.Clear();
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            if (!gameState.MoveBlockDown())
            {
                gameState.PlaceBlock();
                int linesCleared = gameState.ClearFullRows();
                if (linesCleared > 0)
                {
                    UpdateScore(linesCleared);
                }

                if (gameState.IsGameOver)
                {
                    GameOver();
                }
                else
                {
                    gameState.SpawnBlock();
                    AdjustDifficulty();
                }
            }

            Draw();
        }

        private void AdjustDifficulty()
        {
            double speed = Math.Max(100, 800 - ((gameState.Level - 1) * 50));
            gameTimer.Interval = TimeSpan.FromMilliseconds(speed);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (gameOver || isPaused)
                return;

            switch (e.Key)
            {
                case Key.Left:
                    gameState.MoveBlockLeft();
                    break;
                case Key.Right:
                    gameState.MoveBlockRight();
                    break;
                case Key.Down:
                    gameState.MoveBlockDown();
                    UpdateScore(0);
                    break;
                case Key.Up:
                    gameState.RotateBlockCW();
                    break;
                case Key.Z:
                    gameState.RotateBlockCCW();
                    break;
                case Key.C:
                    gameState.HoldBlock();
                    break;
                case Key.Space:
                    gameState.DropBlock();
                    break;
                case Key.Escape:
                    PauseButton_Click(null, null);
                    break;
            }

            Draw();
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
        }

        private void GameOver()
        {
            gameTimer.Stop();
            gameOver = true;
            MessageBox.Show($"Game Over!\nScore: {gameState.Score}\nLevel: {gameState.Level}\nLines: {gameState.Lines}", 
                          "Game Over", 
                          MessageBoxButton.OK);
            CheckHighScore(gameState.Score);
        }

        private void UpdateScore(int linesCleared)
        {
            var animation = (System.Windows.Media.Animation.Storyboard)FindResource("ScoreChangeAnimation");
            animation?.Begin(ScoreText);
        }

        private void LoadHighScores()
        {
            try
            {
                if (System.IO.File.Exists("highscores.txt"))
                {
                    string[] scores = System.IO.File.ReadAllLines("highscores.txt");
                    highScores.Clear();
                    foreach (string score in scores)
                    {
                        if (int.TryParse(score, out int highScore))
                        {
                            highScores.Add(highScore);
                        }
                    }
                    highScores.Sort((a, b) => b.CompareTo(a));
                }
            }
            catch (Exception)
            {
            }
        }

        private void SaveHighScores()
        {
            try
            {
                System.IO.File.WriteAllLines("highscores.txt", 
                    highScores.ConvertAll(score => score.ToString()));
            }
            catch (Exception)
            {
            }
        }

        private void CheckHighScore(int score)
        {
            if (highScores.Count < 10 || score > highScores[highScores.Count - 1])
            {
                highScores.Add(score);
                highScores.Sort((a, b) => b.CompareTo(a));
                if (highScores.Count > 10)
                {
                    highScores.RemoveAt(10);
                }
                SaveHighScores();
                ShowLeaderboard();
            }
        }

        private void ShowLeaderboard()
        {
            string leaderboard = "High Scores:\n\n";
            for (int i = 0; i < highScores.Count; i++)
            {
                leaderboard += $"{i + 1}. {highScores[i]:N0}\n";
            }
            MessageBox.Show(leaderboard, "Leaderboard", MessageBoxButton.OK);
        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            if (gameOver)
                return;

            if (isPaused)
            {
                gameTimer.Start();
                PauseButton.Content = "PAUSE";
            }
            else
            {
                gameTimer.Stop();
                PauseButton.Content = "RESUME";
            }
            isPaused = !isPaused;
        }

        private void RestartButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to restart?", 
                              "Restart Game", 
                              MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                InitializeGame();
            }
        }

        private void OptionsButton_Click(object sender, RoutedEventArgs e)
        {
            var options = new OptionsDialog
            {
                Owner = this
            };
            options.ShowDialog();
        }

        private void LeaderboardButton_Click(object sender, RoutedEventArgs e)
        {
            ShowLeaderboard();
        }

        private void InitializeGame()
        {
            gameState.Reset();
            gameOver = false;
            isPaused = false;
            PauseButton.Content = "PAUSE";
            gameTimer.Start();
            Draw();
        }
    }

    public class OptionsDialog : Window
    {
        public OptionsDialog()
        {
            Title = "Options";
            Width = 300;
            Height = 200;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            ResizeMode = ResizeMode.NoResize;
            Background = new SolidColorBrush(Color.FromRgb(26, 26, 26)); 
            BorderBrush = Brushes.Yellow;
            BorderThickness = new Thickness(2);

            var grid = new Grid();
            Content = grid;

            var stackPanel = new StackPanel
            {
                Margin = new Thickness(10)
            };
            grid.Children.Add(stackPanel);

            var musicCheckBox = new CheckBox
            {
                Content = "Music",
                IsChecked = true,
                Margin = new Thickness(0, 5, 0, 5),
                Foreground = Brushes.White
            };
            stackPanel.Children.Add(musicCheckBox);

            var soundCheckBox = new CheckBox
            {
                Content = "Sound Effects",
                IsChecked = true,
                Margin = new Thickness(0, 5, 0, 5),
                Foreground = Brushes.White
            };
            stackPanel.Children.Add(soundCheckBox);

            var okButton = new Button
            {
                Content = "OK",
                Width = 75,
                Height = 25,
                Margin = new Thickness(0, 10, 0, 0),
                Background = new SolidColorBrush(Color.FromRgb(26, 26, 26)),
                Foreground = Brushes.White,
                BorderBrush = Brushes.Yellow,
                BorderThickness = new Thickness(2)
            };
            okButton.Click += (s, e) => Close();
            stackPanel.Children.Add(okButton);
        }
    }
}