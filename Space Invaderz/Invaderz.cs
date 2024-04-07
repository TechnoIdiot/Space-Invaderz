using Microsoft.Toolkit.HighPerformance;
using Raylib_CsLo;
using System.Diagnostics;
using System.Numerics;
using System.Xml.Linq;

namespace Space_Invaderz
{
    internal class Invaderz
    {
        public enum GameState
        {
            Play,
            ScoreScreen,
            StartScreen,
            Options,
            Exit,
        }
        public enum LogLevel
        {
            Critical,
            Warning,
            Information,
        }
        public static GameState state;
        public static StreamWriter sw;
        public static Random random = new Random();
        private static string logPath;

        // Game configurations
        const int screen_width = 720;
        const int screen_height = 980;
        public static int score;
        public static int lives;
        public static List<Particles> particles = new List<Particles>();
        static bool pause;
        static bool options;
        static bool developer = false;
        static bool logs = false;

        // Player configurations
        public static Player player;
        public static int player_Size;
        public static int player_Speed;
        public static Color player_Color;
        public static Texture playerImage;

        // Bullet configurations
        public static List<Bullet> bullets = new List<Bullet>();
        public static int bullet_Size;
        public static Color bullet_Color;
        public static bool enable_Bullets;
        public static Texture bulletImage;

        // Enemy configurations
        public static List<Enemy> enemies = new List<Enemy>();
        public static int enemy_Size;
        public static int enemy_Speed;
        public static Color enemy_Color;
        public static bool changeDirection;
        public static int deadEnemies;
        public static int enemy_ActiveBullets;
        public static int enemyBulletLimit;
        public static Texture enemyImage;

        // PauseMenu information
        static Stopwatch timePlayedTimer = new Stopwatch();
        static TimeSpan elapsedTime;
        static int enemiesKilled;

        // Devmenu stuff
        static char[] cheatScore = new char[8];
        static int cheatNumCount = 0;
        static int framesCounter = 0;

        /// <summary>
        /// The beginning of the program
        /// </summary>
        /// <param name="args"></param>

        static void Main(string[] args)
        {
            #if DEBUG
            developer = true;
            logs = true;
            logPath = $@"{Directory.GetCurrentDirectory()}\{DateTime.Now}.txt";
#endif
            Log("File created", LogLevel.Information);
            Run();
        }

        /// <summary>
        /// Entry point of the game
        /// </summary>
        private static void Run()
        {
            Raylib.InitWindow(screen_width, screen_height, "Space Invaderz");
            Raylib.SetTargetFPS(60);
            state = GameState.StartScreen;
            Raylib.SetExitKey(KeyboardKey.KEY_TAB); // Exit key is now tab
            Log("Game loaded", LogLevel.Information);

            while (Raylib.WindowShouldClose() == false)
            {
                switch (state)
                {
                    case GameState.StartScreen:
                        Raylib.BeginDrawing();
                        Raylib.ClearBackground(Raylib.BLACK);

                        DrawStartScreen();

                        Raylib.EndDrawing();
                        break;
                    case GameState.Play:
                        Update();
                        Draw();
                        break;
                    case GameState.ScoreScreen:
                        Raylib.BeginDrawing();
                        Raylib.ClearBackground(Raylib.BLACK);
                        DrawScoreScreen();
                        Raylib.EndDrawing();
                        break;
                    case GameState.Exit:
                        Raylib.CloseWindow();
                        break;
                }
            }
        }

        /// <summary>
        /// Settings UI
        /// </summary>
        static void DrawOptions()
        {
            while (options)
            {
                Raylib.BeginDrawing();
                Raylib.DrawRectangle(95, 295, 510, 510, Raylib.WHITE);
                Raylib.DrawRectangle(100, 300, 500, 500, Raylib.BLACK);
                Raylib.DrawText($"Options", 175, 300, 100, Raylib.WHITE);
                bool back = RayGui.GuiButton(new Rectangle(190, 650, 300, 100), "Back");
                if (back)
                {
                    options = false;
                }
                Raylib.EndDrawing();
            }
        }

        /// <summary>
        /// Startscreen UI
        /// </summary>
        static void DrawStartScreen()
        {
            Raylib.DrawText("Space Invaderz!", screen_width / 2 - 250, 200, 60.0f, Raylib.WHITE);
            Raylib.DrawText("Right arrow key = Move right", screen_width / 2 - 250, 270, 20.0f, Raylib.WHITE);
            Raylib.DrawText("Down arrow key = Move down", screen_width / 2 - 250, 290, 20.0f, Raylib.WHITE);
            Raylib.DrawText("Left arrow key = Move left", screen_width / 2 - 250, 310, 20.0f, Raylib.WHITE);
            Raylib.DrawText("Up arrow key = Move up", screen_width / 2 - 250, 330, 20.0f, Raylib.WHITE);
            Raylib.DrawText("Spacebar = Shoot", screen_width / 2 - 250, 350, 20.0f, Raylib.WHITE);
            Raylib.DrawText("Exit key = Tab", screen_width / 2 - 250, 370, 20.0f, Raylib.WHITE);
            bool newGame = RayGui.GuiButton(new Rectangle(screen_width / 2 - 150, screen_height / 2 - 100, 300, 100), "New game");
            bool option = RayGui.GuiButton(new Rectangle(screen_width / 2 - 150, screen_height / 2 + 25, 300, 100), "Options");
            bool exitGame = RayGui.GuiButton(new Rectangle(screen_width / 2 - 150, screen_height / 2 + 150, 300, 100), "Exit Game");
            if (newGame)
            {
                Log("Started new game", LogLevel.Information);
                state = GameState.Play;
                Init();
            }
            if (option)
            {
                Log("Options opened", LogLevel.Information);
                options = true;
                DrawOptions();
            }
            if (exitGame)
            {
                Log("Exited game", LogLevel.Information);
                state = GameState.Exit;
            }
        }

        /// <summary>
        /// Scorescreen UI
        /// </summary>
        static void DrawScoreScreen()
        {
            bool newGame = RayGui.GuiButton(new Rectangle(screen_width / 2 - 150, screen_height / 2, 300, 100), "New game");
            bool backToMenu = RayGui.GuiButton(new Rectangle(screen_width / 2 - 150, screen_height / 2 + 150, 300, 100), "Back to main menu");
            Raylib.DrawText("Game over!", screen_width / 2 - 150, 200, 60.0f, Raylib.RED);
            Raylib.DrawText($"Score: {score}", screen_width / 2 - 75, 400, 30.0f, Raylib.RED);
            if (newGame)
            {
                Log("Started new game", LogLevel.Information);
                Init();
                state = GameState.Play;
            }
            if (backToMenu)
            {
                Log("Returned to main menu", LogLevel.Information);
                state = GameState.StartScreen;
            }
        }

        /// <summary>
        /// Required everytime the game starts/restarts. Used to reset the game.
        /// </summary>
        static void Init()
        {
            Log("Loading game", LogLevel.Information);
            // Game variables
            score = 0;
            lives = 3;
            pause = false;

            // Player variables
            player_Size = 30;
            player_Speed = 5;
            player_Color = Raylib.WHITE;
            playerImage = Raylib.LoadTexture("data/images/playerShip2_green.png");

            // Bullet variables
            bullets.Clear();
            bullet_Size = 10;
            bullet_Color = Raylib.WHITE;
            enable_Bullets = false;

            // Enemy variables
            enemies.Clear();
            enemy_Size = 25;
            enemy_Speed = 50;
            enemy_Color = Raylib.RED;
            changeDirection = false;
            enemy_ActiveBullets = 0;
            enemyBulletLimit = 4;
            enemyImage = Raylib.LoadTexture("data/images/playerShip2_green.png");

            // Create player
            player = new(new Vector2(screen_width / 2, screen_height - 200), player_Size, player_Speed, player_Color, playerImage);

            // Related to creating enemies
            int maxRow = 5;
            int maxColumn = 11;
            int enemy_posX = 20;
            int enemy_posY = 100;

            // Initialize bullets
            for (int i = 0; bullets.Count < player.playerBulletLimit + enemyBulletLimit; i++)
            {
                Bullet bullet = new(player.transform.position, bullet_Size, bullet_Color, enable_Bullets, bulletImage);
                bullets.Add(bullet);
            }

            // Spawn the enemies when game begins
            for (int i = 0; i < maxRow; i++)
            {
                for (int j = 0; j < maxColumn; j++)
                {
                    enemy_posX += 50;
                    Enemy enemy = new Enemy(new Vector2(enemy_posX, enemy_posY), enemy_Size, enemy_Speed, Raylib.GRAY, true, enemyImage);
                    enemies.Add(enemy);
                }
                enemy_posY += 70;
                enemy_posX = 20;
            }
            particles.Clear();
            for (int i = 0; i < 10; i++)
            {
                Particles particle = new Particles(15, 0.5f, false, new GameObject.Transform(new Vector2(0, 0), 10), random);
                particles.Add(particle);
            }

            enemiesKilled = 0;
            timePlayedTimer.Reset();
            timePlayedTimer.Start();
            Log("Game loaded", LogLevel.Information);
        }

        /// <summary>
        /// Super secret developer cheat menu, current only function is killing all enemies.
        /// </summary>
        static void DeveloperMenu()
        {
            bool developMenu = true;
            while (developMenu)
            {
                int maxNums = 7;
                string numbers = "";
                bool mouseOnText = false;
                Rectangle textbox = new Rectangle(1, 190, 200, 50);
                Raylib.BeginDrawing();

                // Set score stuff
                Raylib.DrawText("Set player score, max 7 nums", 1, 160, 18, Raylib.WHITE);
                Raylib.DrawRectangle((int)textbox.x, (int)textbox.y, (int)textbox.width, (int)textbox.height, Raylib.LIGHTGRAY); // Input box
                Raylib.DrawText("Place mouse over input box to type", 1, 255, 15, Raylib.WHITE);

                if (Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), textbox))
                    mouseOnText = true;

                if (mouseOnText)
                {
                    framesCounter++;
                    Raylib.SetMouseCursor(MouseCursor.MOUSE_CURSOR_IBEAM);

                    int key = Raylib.GetCharPressed();

                    while (key > 0)
                    {
                        if (key >= 48 && key <= 57 && cheatNumCount < maxNums)
                        {
                            cheatScore[cheatNumCount] = (char)key;
                            cheatNumCount++;
                        }
                        key = Raylib.GetCharPressed();
                    }
                    if (Raylib.IsKeyPressed(KeyboardKey.KEY_BACKSPACE))
                    {
                        cheatNumCount--;
                        if (cheatNumCount < 0) cheatNumCount = 0;
                        cheatScore[cheatNumCount] = '\0';
                    }
                }
                else
                {
                    framesCounter = 0;
                    Raylib.SetMouseCursor(MouseCursor.MOUSE_CURSOR_DEFAULT);
                }

                if (mouseOnText) 
                    Raylib.DrawRectangleLines((int)textbox.x, (int)textbox.y, (int)textbox.width, (int)textbox.height, Raylib.RED);
                else 
                    Raylib.DrawRectangleLines((int)textbox.x, (int)textbox.y, (int)textbox.width, (int)textbox.height, Raylib.DARKGRAY);


                numbers = new string(cheatScore);
                Raylib.DrawText(numbers, (int)textbox.x + 5, (int)textbox.y + 12, 20, Raylib.MAROON);
                if (cheatNumCount < maxNums)
                {
                    // Draw blinking underscore char
                    if (((framesCounter / 20) % 2) == 0) Raylib.DrawText("_", (int)textbox.x + 8 + Raylib.MeasureText(numbers, 20), (int)textbox.y + 12, 40, Raylib.MAROON);
                }

                // Buttons and functions
                bool killAllEnemies = RayGui.GuiButton(new Rectangle(151, 1, 150, 150), "Kill all enemies");
                bool godMode = RayGui.GuiButton(new Rectangle(301, 1, 150, 150), "God mode");
                bool setScore = RayGui.GuiButton(new Rectangle(301, 151, 150, 150), "Set score");
                bool exitDevMenu = RayGui.GuiButton(new Rectangle(1, 1, 150, 150), "Exit dev menu");
                if (godMode)
                {
                    player.ToggleGodMode();
                }
                if (killAllEnemies)
                {
                    foreach (Enemy enemy in enemies)
                    {
                        if (enemy.isActive)
                        {
                            enemy.Disable();
                            deadEnemies++;
                        }
                    }
                    Log("Enemies killed succesfully", LogLevel.Information);
                }
                if (setScore)
                {
                    int result;
                    int.TryParse(numbers, out result);
                    score = result;
                    cheatScore = new char[8];
                }
                if (exitDevMenu)
                {
                    Log("Devcheat menu closed", LogLevel.Information);
                    developMenu = false;
                }
                Raylib.EndDrawing();
            }
        }

        /// <summary>
        /// Main game loop
        /// </summary>
        static void Update()
        {
            if (developer)
            {
                if (Raylib.IsKeyPressed(KeyboardKey.KEY_F1))
                {
                    Log("Dev menu opened", LogLevel.Information);
                    DeveloperMenu();
                }
            }

            if (Raylib.IsKeyPressed(KeyboardKey.KEY_ESCAPE))
            {
                if (!pause)
                {
                    Log("Paused", LogLevel.Information);
                    timePlayedTimer.Stop();
                    elapsedTime = timePlayedTimer.Elapsed;
                    pause = true;
                }
                else if (pause)
                {
                    Log("Unpaused", LogLevel.Information);
                    timePlayedTimer.Start();
                    pause = false;
                }
            }

            if (!pause)
            {
                PlayerUpdate();
                EnemyUpdate();
                BulletUpdate();
                WinLossCheck();
            }
        }
        /// <summary>
        /// Updates the screen
        /// </summary>
        static void Draw()
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Raylib.BLACK);

            // Draw Player
            player.spriteRenderer.SpriteRender(0.0f, -15.0f, 0.0f);
            
            // Draw Bullets
            foreach (Bullet bullet in bullets)
            {
                if (!bullet.isActive)
                    continue;

                bullet.spriteRenderer.RectangleRender();
            }
            // Draw enemies
            foreach (Enemy enemy in enemies)
            {
                if (!enemy.isActive)
                    continue;

                enemy.spriteRenderer.SpriteRender(180.0f, 40.0f, 30.0f);
                //enemy.spriteRenderer.RectangleRender();
            }

            foreach (Particles particle in particles)
            {
                if (!particle.isActive)
                    continue;

                particle.Update();
            }

            Raylib.DrawText($"Score: {score}", 10, 10, 16, Raylib.WHITE);
            Raylib.DrawText($"Lives: {lives}", 10, 36, 16, Raylib.WHITE);

            if (pause)
            {
                Raylib.DrawRectangle(95, 295, 510, 590, Raylib.WHITE);
                Raylib.DrawRectangle(100, 300, 500, 580, Raylib.BLACK);
                Raylib.DrawText("Paused", 175, 300, 100, Raylib.RED);
                string elapsedTimeText = String.Format("{0:0}:{1:00}:{2:00}", elapsedTime.Hours, elapsedTime.Minutes, elapsedTime.Seconds);
                Raylib.DrawText("Time played:", 100, 240, 25, Raylib.WHITE);
                Raylib.DrawText("Enemies killed:", 400, 240, 25, Raylib.WHITE);
                Raylib.DrawText(elapsedTimeText, 100, 260, 25, Raylib.WHITE);
                Raylib.DrawText(enemiesKilled.ToString(), 400, 260, 25, Raylib.WHITE);
                bool continuee = RayGui.GuiButton(new Rectangle(190, 400, 300, 100), "Continue");
                bool newGame = RayGui.GuiButton(new Rectangle(190, 525, 300, 100), "New game");
                bool option = RayGui.GuiButton(new Rectangle(190, 650, 300, 100), "Options");
                bool back = RayGui.GuiButton(new Rectangle(190, 775, 300, 100), "Return to main menu");
                if (continuee)
                {
                    Log("Pause menu closed", LogLevel.Information);
                    timePlayedTimer.Start();
                    pause = false;
                }
                if (newGame)
                {
                    Log("Player started new game", LogLevel.Information);
                    Init();
                }
                if (option)
                {
                    Log("Options opened", LogLevel.Information);
                    options = true;
                    DrawOptions();
                }
                if (back)
                {
                    Log("Player returned to main menu", LogLevel.Information);
                    state = GameState.StartScreen;
                }
            }

            Raylib.EndDrawing();
        }

        /// <summary>
        /// Updates every bullets status. Also handles hit reg.
        /// </summary>
        static void BulletUpdate()
        {
            foreach (Bullet bullet in bullets)
            {
                if (!bullet.isActive)
                    continue;

                if (bullet.WhoShot().ToString() == "Player")
                {
                    bullet.transform.MoveUp();
                    // Enemy collision check
                    foreach (Enemy enemy in enemies)
                    {
                        if (!enemy.isActive)
                            continue;

                        // If bullet hits enemy?
                        if (bullet.collision.CheckCollision(enemy.transform.position, enemy.collision.object_size, enemy.collision.object_size))
                        {
                            enemy.Disable();
                            bullet.Disable();
                            score += deadEnemies * 10;
                            deadEnemies++;
                            player.DecreaseActiveBulletAmount();
                            EnemyDeath(enemy);
                        }
                    }
                }

                if (bullet.WhoShot().ToString() == "Enemy")
                {
                    bullet.transform.MoveDown();

                    // Player collision check
                    if (bullet.collision.CheckCollision(player.transform.position, player.collision.object_size, player.collision.object_size))
                    {
                        bullet.Disable();
                        if (!player.GetPlayerInvulnerability())
                        {
                            player.EnablePlayerInvulnerability();
                            lives--;
                        }
                        enemy_ActiveBullets--;
                    }
                }

                // Screen collision check
                if (!bullet.collision.CheckCollision(new Vector2(0, 10), screen_height, screen_width))
                {
                    bullet.Disable();
                    if (bullet.WhoShot().ToString() == "Enemy")
                    {
                        player.DecreaseActiveBulletAmount();
                    }
                    if (bullet.WhoShot().ToString() == "Player")
                    {
                        enemy_ActiveBullets--;
                    }
                }
            }
        }

        /// <summary>
        /// Keeps player up to date
        /// </summary>
        static void PlayerUpdate()
        {
            player.InvulnerabilityTimer();
            player.KeepInsidePlayArea(screen_height, screen_width);

            // Side to side movement, also includes the input check
            player.moveUpdate();

            // Shoot check
            if (Raylib.IsKeyDown(KeyboardKey.KEY_SPACE))
            {
                foreach (Bullet bullet in bullets)
                {
                    if (bullet.isActive)
                        continue;

                    if (!player.GetShootDelay())
                    {
                        Log("Player shot bullet", LogLevel.Information);
                        player.Shoot(bullet);
                    }
                }
                player.shootDelay = false;
            }
        }

        /// <summary>
        /// Keeps enemy up to date
        /// </summary>
        static void EnemyUpdate()
        {
            int chosenEnemy = random.Next(0, enemies.Count);
            foreach (Enemy enemy in enemies)
            {
                // For some reason moving ScreenCollisionCheck inside enemy class allowed every enemy except for the last row to go past the left wall and this fixes that
                if (changeDirection)
                    continue;
                enemy.transform.MoveRight();
                // Enemy wall collision check
                changeDirection = enemy.ScreenCollisionCheck(screen_height, screen_width);
            }
            if (changeDirection)
            {
                foreach (Enemy enemy in enemies)
                {
                    enemy.ChangeDirection();
                }
                // When the enemies bounce off the side of the screen, reset their ammo.
                enemy_ActiveBullets = 0;
                changeDirection = false;
            }

            if (enemy_ActiveBullets < enemyBulletLimit)
            {
                foreach (Bullet bullet in bullets)
                {
                    if (bullet.isActive)
                        continue;

                    if (enemies[chosenEnemy].isActive & !enemies[chosenEnemy].GetShootDelay())
                    {
                        Log("Enemy shot bullet", LogLevel.Information);
                        enemies[chosenEnemy].Shoot(bullet);
                    }
                }
                if (enemies[chosenEnemy].GetShootDelay())
                    enemy_ActiveBullets++;

                enemies[chosenEnemy].shootDelay = false;
            }
        }

        /// <summary>
        /// Checks player lives and enemy amount. Handles enemy wave respawns
        /// </summary>
        static void WinLossCheck()
        {
            if (lives == 0)
            {
                Log("Player died", LogLevel.Information);
                state = GameState.ScoreScreen;
            }

            foreach (Enemy enemy in enemies)
            {
                if (!enemy.isActive)
                    continue;

                if (enemy.transform.position.Y == player.transform.position.Y - 10)
                {
                    Log("Enemies reached player", LogLevel.Information);
                    state = GameState.ScoreScreen;
                }
            }

            // Enemy amount check, if all dead then respawn them, might as well give a score bonus
            if (deadEnemies == enemies.Count)
            {
                enemies.Clear();
                // Related to creating enemies
                int maxRow = 5;
                int maxColumn = 11;
                int enemy_posX = 20;
                int enemy_posY = 100;

                score += lives * 10000;
                // Spawn the enemies when game begins or in this case we are respawning them
                for (int i = 0; i < maxRow; i++)
                {
                    for (int j = 0; j < maxColumn; j++)
                    {
                        enemy_posX += 50;
                        Enemy enemy = new Enemy(new Vector2(enemy_posX, enemy_posY), enemy_Size, enemy_Speed, Raylib.GRAY, true, enemyImage);
                        enemies.Add(enemy);
                        deadEnemies--;
                    }
                    enemy_posY += 70;
                    enemy_posX = 20;
                }
            }
        }

        /// <summary>
        /// Initializes a particle on the enemy death position
        /// </summary>
        /// <param name="enemy"></param>
        static void EnemyDeath(Enemy enemy)
        {
            bool particleonlyoncepls = false;
            foreach (Particles particle in particles)
            {
                if (particle.isActive)
                    continue;

                if (!particleonlyoncepls)
                {
                    particle.Init(enemy);
                    particleonlyoncepls = true;
                }
            }
            particleonlyoncepls = false;
            enemiesKilled++;
        }

        /// <summary>
        /// Used to log information into a text file
        /// </summary>
        /// <param name="text"></param>
        /// <param name="level"></param>
        static void Log(string text, LogLevel level)
        {
            if (logs)
            {
                if (!File.Exists(logPath))
                {
                    File.Create(logPath).Close();
                }
                if (File.Exists(logPath))
                {
                    StreamWriter sw = new StreamWriter(logPath, true);
                    sw.WriteLine($"{level} {text}");
                    sw.Close();
                }

            }
        }
    }

}
