using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp5
{
    public enum ObjectType
    {
        Apple,
        SnakePart
    }
    public enum Direction
    {
        Up,
        Right,
        Down,
        Left
    }

    public class Snake
    {
        public List<SnakePart> body;
        public SnakePart Head { get; set; }

        public Snake(SnakePart snakePart)
        {
            Head = snakePart;
            body = new List<SnakePart>() { Head };
        }
    }

    public class Feed
    {
        public Position Position { get; set; }

        public Rectangle Rectangle { get; set; }

        public Feed(int snakeSize)
        {
            Rectangle = new Rectangle();
            Rectangle.Width = snakeSize;
            Rectangle.Height = snakeSize;
            Rectangle.Fill = new SolidColorBrush(Colors.Red);
            Position = new Position(0, 0);
        }
    }

    public class SnakePart
    {
        public Rectangle Rectangle { get; set; }

        private const int _distanceBetweenShapes = 5;

        public Position Pos { get; set; }

        public Direction Direction { get; set; }

        public int Number { get; set; }

        public Rectangle SnakePartCanvas { get; set; }

        public SnakePart(int number, Position position, Direction direction)
        {
            Pos = position;
            Direction = direction;
            Number = number;
        }

        public SnakePart(int snakeSize, bool isHead)
        {
            Rectangle = new Rectangle();
            Rectangle.Width = snakeSize;
            Rectangle.Height = snakeSize;
            Rectangle.Fill = isHead ? new SolidColorBrush(Colors.LightGreen) : new SolidColorBrush(Colors.Green);
        }

        public void Move()
        {
            switch (Direction)
            {
                case Direction.Down:
                    Pos.Y += _distanceBetweenShapes;
                    break;

                case Direction.Up:
                    Pos.Y -= _distanceBetweenShapes;
                    break;

                case Direction.Right:
                    Pos.X += _distanceBetweenShapes;
                    break;

                case Direction.Left:
                    Pos.X -= _distanceBetweenShapes;
                    break;
            }
            Canvas.SetLeft(SnakePartCanvas, Pos.X);
            Canvas.SetTop(SnakePartCanvas, Pos.Y);
        }
    }


    public class Position
    {
        public double X { get; set; }
        public double Y { get; set; }

        public Position(double x, double y)
        {
            X = x;
            Y = y;
        }

    }



    public partial class MainWindow : Window
    {
        Snake snake;
        bool controlWasPressed = false;
        int gameScore = 0;
        const int appleRadius = 10;
        const int snakeSide = 10;
        private int _gameSpeed = 150;
        Direction direction = Direction.Right;
        Feed feed = new Feed(snakeSide);

        private void MainWindow_KeyDownMove(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.W:
                    direction = Direction.Up;
                    break;
                case Key.A:
                    direction = Direction.Left;
                    break;
                case Key.S:
                    direction = Direction.Down;
                    break;
                case Key.D:
                    direction = Direction.Right;
                    break;
            }
        }




        private void GenerateFeed()
        {
            
            Random random = new Random();
            feed.Position.X = random.Next(0, 38) * snakeSide;
            feed.Position.Y = random.Next(0, 48) * snakeSide;

            Canvas.SetLeft(feed.Rectangle, feed.Position.X);
            Canvas.SetTop(feed.Rectangle, feed.Position.Y);
            Field.Children.Add(feed.Rectangle);
        }

        private async void RenderGame()
        {
            Canvas.SetLeft(snake.Head.Rectangle, snake.Head.Pos.X);
            Canvas.SetTop(snake.Head.Rectangle, snake.Head.Pos.Y);
            Field.Children.Add(snake.Head.Rectangle);
            GenerateFeed();

            while (true)
            {
                for (int i = snake.body.Count - 1; i > 0; i--)
                {
                    snake.body[i].Pos.X = snake.body[i - 1].Pos.X;
                    snake.body[i].Pos.Y = snake.body[i - 1].Pos.Y;
                }

                await Task.Delay(_gameSpeed);
                if (controlWasPressed == true)
                {
                    controlWasPressed = false;
                }

                if (direction == Direction.Left)
                {
                    snake.body[0].Pos.X -= snakeSide;
                }
                else if (direction == Direction.Right)
                {
                    snake.body[0].Pos.X += snakeSide;
                }
                else if (direction == Direction.Up)
                {
                    snake.body[0].Pos.Y -= snakeSide;
                }
                else if (direction == Direction.Down)
                {
                    snake.body[0].Pos.Y += snakeSide;
                }

                for (int i = 0; i < snake.body.Count; i++)
                {
                    Field.Children.Remove(snake.body[i].Rectangle);
                    Canvas.SetLeft(snake.body[i].Rectangle, snake.body[i].Pos.X);
                    Canvas.SetTop(snake.body[i].Rectangle, snake.body[i].Pos.Y);
                    Field.Children.Add(snake.body[i].Rectangle);


                }

                if (snake.body[0].Pos.X == feed.Position.X && snake.body[0].Pos.Y == feed.Position.Y)
                {
                    SnakePart snakePart = new SnakePart(snakeSide, false)
                    {
                        Pos = new Position(snake.body.Last().Pos.X+10, snake.body.Last().Pos.Y+10)
                    };
                    snake.body.Add(snakePart);

                    Field.Children.Remove(feed.Rectangle);

                    gameScore += 1;

                    Score.Content = $"Счёт: {gameScore}";

                    _gameSpeed -= _gameSpeed / 10;

                    GenerateFeed();
                }

                if (snake.body[0].Pos.X < 0 || snake.body[0].Pos.X > 400 || snake.body[0].Pos.Y < 0 || snake.body[0].Pos.Y > 500)
                {
                    MessageBox.Show("Лох", "Snake", MessageBoxButton.OK, MessageBoxImage.Warning);
                    Close();
                    break;
                }
                else if (snake.body.Where(s => s.Pos.X == snake.body[0].Pos.X && s.Pos.Y == snake.body[0].Pos.Y).Count() > 1)
                {
                    MessageBox.Show("Лох", "Snake", MessageBoxButton.OK, MessageBoxImage.Warning);
                    Close();
                    break;
                }
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            SnakePart snakeHead = new SnakePart(snakeSide, true)
            {
                Pos = new Position(0, 0)
            };
            snake = new Snake(snakeHead);
            RenderGame();
        }

        private void Field_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (controlWasPressed == false) { 
                if (direction != Direction.Right && e.Key == Key.A)
                {
                    direction = Direction.Left;

                }
                else if (direction != Direction.Left && e.Key == Key.D)
                {
                    direction = Direction.Right;

                }
                else if (direction != Direction.Down && e.Key == Key.W)
                {
                    direction = Direction.Up;

                }
                else if (direction != Direction.Up && e.Key == Key.S)
                {
                    direction = Direction.Down;
                }
                controlWasPressed = true;
            }
        }
    }
}