using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.IO;
using System.Linq;

class LeaderboardEntry
{
    public string PlayerName { get; set; }
    public float Time { get; set; }
}

class Game
{
    private RenderWindow window;
    private RectangleShape player;
    private List<RectangleShape> platforms;
    private List<RectangleShape> obstacles;
    private RectangleShape flag;
    private Vector2f playerVelocity;
    private bool isJumping;
    private float gravity = 0.5f;
    private float jumpStrength = -12f;
    private View gameView;
    private const int TILE_SIZE = 32;
    private const int LEVEL_WIDTH = 150;
    private const int LEVEL_HEIGHT = 20;
    private Random random;
    private bool gameWon = false;
    private Clock gameClock;
    private float elapsedTime = 0f;
    private Font font;
    private string playerName;
    private List<LeaderboardEntry> leaderboard;
    private const string LeaderboardFile = "leaderboard.json";
    private bool isEnteringName = true;

    public Game()
    {
        window = new RenderWindow(new VideoMode(800, 600), "Mario-style Platformer");
        window.SetFramerateLimit(60);

        font = new Font("Roboto-Regular.ttf");
        LoadLeaderboard();
        playerName = "";
        player = new RectangleShape(new Vector2f(TILE_SIZE, TILE_SIZE));
        player.FillColor = Color.Red;
        player.Position = new Vector2f(TILE_SIZE, LEVEL_HEIGHT * TILE_SIZE - TILE_SIZE * 2);

        platforms = new List<RectangleShape>();
        obstacles = new List<RectangleShape>();
        random = new Random();
        CreateLevel();

        playerVelocity = new Vector2f(0, 0);
        isJumping = false;

        gameView = new View(new FloatRect(0, 0, 800, 600));
        window.SetView(gameView);

        flag = new RectangleShape(new Vector2f(TILE_SIZE, TILE_SIZE * 3));
        flag.FillColor = Color.Yellow;
        flag.Position = new Vector2f((LEVEL_WIDTH - 2) * TILE_SIZE, (LEVEL_HEIGHT - 4) * TILE_SIZE);

        gameClock = new Clock();
    }

    private void CreateLevel()
    {
        // Ground
        for (int i = 0; i < LEVEL_WIDTH; i++)
        {
            CreateTile(i, LEVEL_HEIGHT - 1, Color.Green);
        }

        // Platforms and obstacles
        CreatePlatform(10, 5, 15);
        CreatePlatform(20, 8, 13);
        CreatePlatform(32, 3, 11);
        CreatePlatform(40, 10, 14);
        CreatePlatform(55, 5, 12);
        CreatePlatform(65, 8, 10);
        CreatePlatform(80, 15, 13);
        CreatePlatform(100, 20, 15);
        CreatePlatform(125, 10, 12);

        CreateObstacle(18, 1, 17);
        CreateObstacle(28, 2, 17);
        CreateObstacle(45, 1, 13);
        CreateObstacle(60, 3, 17);
        CreateObstacle(75, 1, 17);
        CreateObstacle(90, 2, 17);
        CreateObstacle(110, 3, 14);
        CreateObstacle(130, 1, 11);

        // Add some floating blocks
        for (int i = 0; i < 10; i++)
        {
            CreateTile(random.Next(LEVEL_WIDTH), random.Next(10, 15), Color.Magenta);
        }
    }

    private void CreateTile(int x, int y, Color color)
    {
        RectangleShape tile = new RectangleShape(new Vector2f(TILE_SIZE, TILE_SIZE));
        tile.Position = new Vector2f(x * TILE_SIZE, y * TILE_SIZE);
        tile.FillColor = color;
        platforms.Add(tile);
    }

    private void CreatePlatform(int x, int width, int y)
    {
        for (int i = 0; i < width; i++)
        {
            CreateTile(x + i, y, new Color(139, 69, 19)); // Dark brown color
        }
    }

    private void CreateObstacle(int x, int width, int y)
    {
        for (int i = 0; i < width; i++)
        {
            RectangleShape obstacle = new RectangleShape(new Vector2f(TILE_SIZE, TILE_SIZE));
            obstacle.Position = new Vector2f((x + i) * TILE_SIZE, y * TILE_SIZE);
            obstacle.FillColor = Color.Yellow;
            obstacles.Add(obstacle);
        }
    }

    public void Run()
    {
        while (window.IsOpen)
        {
            window.DispatchEvents();
            if (window.HasFocus())  // Only process input when window is focused
            {
                if (isEnteringName)
                {
                    HandleNameInput();
                }
                else if (!gameWon)
                {
                    HandleInput();
                    Update();
                    elapsedTime = gameClock.ElapsedTime.AsSeconds();
                }
            }
            Render();
        }
    }

    private void HandleNameInput()
    {
        window.KeyPressed -= OnKeyPressed;  // Remove previous event handler if exists
        window.KeyPressed += OnKeyPressed;  // Add the event handler
    }

    private void OnKeyPressed(object sender, KeyEventArgs e)
    {
        if (e.Code == Keyboard.Key.Enter && !string.IsNullOrWhiteSpace(playerName))
        {
            isEnteringName = false;
            gameClock.Restart();
            window.KeyPressed -= OnKeyPressed;  // Remove the event handler
        }
        else if (e.Code == Keyboard.Key.Backspace && playerName.Length > 0)
        {
            playerName = playerName.Substring(0, playerName.Length - 1);
        }
        else if (playerName.Length < 20 && ((e.Code >= Keyboard.Key.A && e.Code <= Keyboard.Key.Z) || e.Code == Keyboard.Key.Space))
        {
            playerName += e.Code == Keyboard.Key.Space ? " " : e.Code.ToString();
        }
    }

    private void HandleInput()
    {
        if (Keyboard.IsKeyPressed(Keyboard.Key.Left))
            playerVelocity.X = -5;
        else if (Keyboard.IsKeyPressed(Keyboard.Key.Right))
            playerVelocity.X = 5;
        else
            playerVelocity.X = 0;

        if (Keyboard.IsKeyPressed(Keyboard.Key.Space) && !isJumping)
        {
            playerVelocity.Y = jumpStrength;
            isJumping = true;
        }
    }

    private void Update()
    {
        playerVelocity.Y += gravity;
        player.Position += playerVelocity;

        // Collision detection
        isJumping = true;
        foreach (var platform in platforms)
        {
            if (player.GetGlobalBounds().Intersects(platform.GetGlobalBounds()))
            {
                if (playerVelocity.Y > 0 && player.Position.Y < platform.Position.Y)
                {
                    player.Position = new Vector2f(player.Position.X, platform.Position.Y - player.Size.Y);
                    playerVelocity.Y = 0;
                    isJumping = false;
                }
                else if (playerVelocity.Y < 0 && player.Position.Y > platform.Position.Y)
                {
                    player.Position = new Vector2f(player.Position.X, platform.Position.Y + platform.Size.Y);
                    playerVelocity.Y = 0;
                }
                else if (playerVelocity.X > 0)
                {
                    player.Position = new Vector2f(platform.Position.X - player.Size.X, player.Position.Y);
                }
                else if (playerVelocity.X < 0)
                {
                    player.Position = new Vector2f(platform.Position.X + platform.Size.X, player.Position.Y);
                }
            }
        }

        // Obstacle collision
        foreach (var obstacle in obstacles)
        {
            if (player.GetGlobalBounds().Intersects(obstacle.GetGlobalBounds()))
            {
                player.Position = new Vector2f(TILE_SIZE, LEVEL_HEIGHT * TILE_SIZE - TILE_SIZE * 2);
            }
        }

        // Flag collision (win condition)
        if (player.GetGlobalBounds().Intersects(flag.GetGlobalBounds()))
        {
            gameWon = true;
            AddToLeaderboard();
        }

        // Update camera view to center on player
        gameView.Center = player.Position + new Vector2f(0, -TILE_SIZE * 2);
        window.SetView(gameView);

        // Limit player to level bounds
        player.Position = new Vector2f(
            Math.Max(0, Math.Min(player.Position.X, LEVEL_WIDTH * TILE_SIZE - player.Size.X)),
            Math.Min(player.Position.Y, LEVEL_HEIGHT * TILE_SIZE - player.Size.Y)
        );
    }

    private void Render()
    {
        window.Clear(Color.Cyan);

        if (isEnteringName)
        {
            RenderNameInput();
        }
        else
        {
            foreach (var platform in platforms)
                window.Draw(platform);

            foreach (var obstacle in obstacles)
                window.Draw(obstacle);

            window.Draw(flag);
            window.Draw(player);

            // Draw timer
            Text timerText = new Text(FormatTime(elapsedTime), font, 20);
            timerText.Position = new Vector2f(gameView.Center.X - 380, gameView.Center.Y - 280);
            timerText.FillColor = Color.Black;
            window.Draw(timerText);

            if (gameWon)
            {
                RenderLeaderboard();
            }
        }

        window.Display();
    }

    private void RenderNameInput()
    {
        Text namePrompt = new Text("Enter your name:", font, 30);
        namePrompt.Position = new Vector2f(400 - namePrompt.GetGlobalBounds().Width / 2, 250);
        window.Draw(namePrompt);

        Text nameText = new Text(playerName, font, 30);
        nameText.Position = new Vector2f(400 - nameText.GetGlobalBounds().Width / 2, 300);
        window.Draw(nameText);
    }

    private void RenderLeaderboard()
    {
        RectangleShape background = new RectangleShape(new Vector2f(400, 400));
        background.FillColor = new Color(0, 0, 0, 200);
        background.Position = new Vector2f(gameView.Center.X - 200, gameView.Center.Y - 200);
        window.Draw(background);

        Text leaderboardTitle = new Text("Leaderboard", font, 30);
        leaderboardTitle.Position = new Vector2f(gameView.Center.X - leaderboardTitle.GetGlobalBounds().Width / 2, gameView.Center.Y - 180);
        leaderboardTitle.FillColor = Color.White;
        window.Draw(leaderboardTitle);

        for (int i = 0; i < Math.Min(leaderboard.Count, 10); i++)
        {
            Text entry = new Text($"{i + 1}. {leaderboard[i].PlayerName}: {FormatTime(leaderboard[i].Time)}", font, 20);
            entry.Position = new Vector2f(gameView.Center.X - 180, gameView.Center.Y - 130 + i * 30);
            entry.FillColor = Color.White;
            window.Draw(entry);
        }
    }

    private string FormatTime(float seconds)
    {
        int minutes = (int)(seconds / 60);
        int remainingSeconds = (int)(seconds % 60);
        int milliseconds = (int)((seconds - (int)seconds) * 100);
        return $"{minutes:00}:{remainingSeconds:00}.{milliseconds:00}";
    }

    private void LoadLeaderboard()
    {
        if (File.Exists(LeaderboardFile))
        {
            string json = File.ReadAllText(LeaderboardFile);
            leaderboard = JsonSerializer.Deserialize<List<LeaderboardEntry>>(json);
        }
        else
        {
            leaderboard = new List<LeaderboardEntry>();
        }
    }

    private void SaveLeaderboard()
    {
        string json = JsonSerializer.Serialize(leaderboard);
        File.WriteAllText(LeaderboardFile, json);
    }

    private void AddToLeaderboard()
    {
        leaderboard.Add(new LeaderboardEntry { PlayerName = playerName, Time = elapsedTime });
        leaderboard = leaderboard.OrderBy(e => e.Time).ToList();
        if (leaderboard.Count > 10)
        {
            leaderboard.RemoveAt(10);
        }
        SaveLeaderboard();
    }
}

class Program
{
    static void Main(string[] args)
    {
        Game game = new Game();
        game.Run();
    }
}
