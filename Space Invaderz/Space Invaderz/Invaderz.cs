using Raylib_CsLo;
using System.Numerics;

namespace Space_Invaderz
{
    internal class Invaderz
    {
        public enum GameState
        {
            Play,
            ScoreScreen,
            StartScreen,
            Options
        }
        public static GameState state;

        public static Random random = new Random();
        // Game configurations
        const int screen_width = 720;
        const int screen_height = 980;
        public static int score;
        public static int lives;
        public static List<Particles> particles = new List<Particles>();
        static bool pause;
        static bool options;
        static bool developer = false;

        // Player configurations
        public static Player? player;
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

        /// <summary>
        /// The beginning of the program
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            #if DEBUG
                developer = true;
            #endif
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
            Raylib.SetExitKey(259); // Exit key is now backspace

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
                }
            }
            Raylib.CloseWindow();
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
            bool restart = RayGui.GuiButton(new Rectangle(screen_width / 2 - 150, screen_height / 2 - 100, 300, 100), "New game");
            bool option = RayGui.GuiButton(new Rectangle(screen_width / 2 - 150, screen_height / 2 + 25, 300, 100), "Options");
            bool exit = RayGui.GuiButton(new Rectangle(screen_width / 2 - 150, screen_height / 2 + 150, 300, 100), "Exit Game");
            if (restart)
            {
                state = GameState.Play;
                Init();
            }
            if (option)
            {
                options = true;
                DrawOptions();
            }
            if (exit)
            {
                Raylib.CloseWindow();
            }
        }

        /// <summary>
        /// Scorescreen UI
        /// </summary>
        static void DrawScoreScreen()
        {
            bool restart = RayGui.GuiButton(new Rectangle(screen_width / 2 - 150, screen_height / 2, 300, 100), "New game");
            bool exit = RayGui.GuiButton(new Rectangle(screen_width / 2 - 150, screen_height / 2 + 150, 300, 100), "Exit Game");
            Raylib.DrawText("Game over!", screen_width / 2 - 150, 200, 60.0f, Raylib.RED);
            Raylib.DrawText($"Score: {score}", screen_width / 2 - 75, 400, 30.0f, Raylib.RED);
            if (restart)
            {
                Init();
                state = GameState.Play;
            }
            if (exit)
            {
                Raylib.CloseWindow();
            }
        }

        /// <summary>
        /// Required everytime the game starts/restarts.
        /// </summary>
        static void Init()
        {
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
            Console.WriteLine(bullets.Count);

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
        }

        static void DeveloperMenu()
        {
            bool developMenu = true;
            while (developMenu)
            {
                Raylib.BeginDrawing();
                bool killAllEnemies = RayGui.GuiButton(new Rectangle(250, 1, 250, 500), "Kill all enemies");
                bool exit = RayGui.GuiButton(new Rectangle(1, 1, 250, 500), "Exit dev menu");
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
                    Console.WriteLine("Enemies killed succesfully");
                }
                if (exit)
                {
                    developMenu = false;
                }
                Raylib.EndDrawing();
            }
        }

        /// <summary>
        /// higher fps = more updates!!!111 // Tulevaisuuden minä. Mitä on tämä?
        /// </summary>
        static void Update()
        {
            if (developer)
            {
                if (Raylib.IsKeyPressed(KeyboardKey.KEY_F1))
                {
                    DeveloperMenu();
                }
            }

            if (Raylib.IsKeyPressed(KeyboardKey.KEY_ESCAPE))
            {
                if (!pause)
                    pause = true;
                else if (pause)
                    pause = false;
            }

            if (!pause)
            {
                PlayerUpdate();
                EnemyUpdate();
                BulletUpdate(); // Handles most of hit reg
                WinLossCheck();
            }
        }
        /// <summary>
        /// draw.
        /// </summary>
        static void Draw()
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Raylib.BLACK);

            //Raylib.DrawTexture(playerImage, 0, 0, Raylib.WHITE);

            // Draw Player
            player.spriteRenderer.SpriteRender(0.0f, -15.0f, 0.0f);
            //player.spriteRenderer.RectangleRender();
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

            foreach(Particles particle in particles)
            {
                if (!particle.isActive)
                    continue;

                particle.Update();
            }

            Raylib.DrawText($"Score: {score}", 10, 10, 16, Raylib.WHITE);
            Raylib.DrawText($"Lives: {lives}", 10, 36, 16, Raylib.WHITE);

            if (pause)
            {
                Raylib.DrawRectangle(95, 295, 510, 510, Raylib.WHITE);
                Raylib.DrawRectangle(100, 300, 500, 500, Raylib.BLACK);
                Raylib.DrawText($"Paused", 175, 300, 100, Raylib.RED);
                bool continuee = RayGui.GuiButton(new Rectangle(190, 400, 300, 100), "Continue");
                bool option = RayGui.GuiButton(new Rectangle(190, 525, 300, 100), "Options");
                bool back = RayGui.GuiButton(new Rectangle(190, 650, 300, 100), "Return to main menu");
                if (continuee)
                {
                    pause = false;
                }
                if (option)
                {
                    options = true;
                    DrawOptions();
                }
                if (back)
                {
                    state = GameState.StartScreen;
                }
            }

            Raylib.EndDrawing();
        }

        /// <summary>
        /// Updates every bullets status. Also handles hit reg. Feel like this is a bit unclean
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
                        
                        // Jos ammus osuu viholliseen?
                        if (bullet.collision.CheckCollision(enemy.transform.position, enemy.collision.object_size, enemy.collision.object_size))
                        {
                            enemy.Disable();
                            bullet.Disable();
                            score += deadEnemies * 10;
                            deadEnemies++;
                            player.DecreasePlayerBulletCount();
                            EnemyDeadge(enemy);
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
                        player.DecreasePlayerBulletCount();
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
                // When the enemies bounce off the side of the screen, reset their ammo or like just let them shoot again ykyk
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
                state = GameState.ScoreScreen;

            foreach (Enemy enemy in enemies)
            {
                if (!enemy.isActive)
                    continue;

                if (enemy.transform.position.Y == player.transform.position.Y - 10)
                {
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

        static void EnemyDeadge(Enemy enemy)
        {
            bool particleonlyoncepls = false;
            foreach(Particles particle in particles)
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
        }
    }
}
