using Raylib_CsLo;
using System.Numerics;

namespace Space_Invaderz
{
    internal class Invaderz
    {
        public enum GameState
        {
            Play,
            ScoreScreen
        }
        public static GameState state;

        public static Random random = new Random();
        // Game configurations
        const int screen_width = 720;
        const int screen_height = 980;
        public static int score;
        public static int lives;

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

        static void Main(string[] args)
        {
            Run();
        }

        private static void Run()
        {
            Raylib.InitWindow(screen_width, screen_height, "Space Invaderz");
            Raylib.SetTargetFPS(60);
            Init();

            while (Raylib.WindowShouldClose() == false)
            {
                switch (state)
                {
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

        static void DrawScoreScreen()
        {
            bool restart = RayGui.GuiButton(new Rectangle(screen_width / 2 - 150, screen_height / 2, 300, 100), "New game");
            Raylib.DrawText("Game over!", screen_width / 2 - 150, 200, 60.0f, Raylib.RED);
            Raylib.DrawText($"Score: {score}", screen_width / 2 - 75, 400, 30.0f, Raylib.RED);
            if (restart)
            {
                Init();
                state = GameState.Play;
            }
        }
        static void Init()
        {
            state = GameState.Play;
            score = 0;
            lives = 3;

            player_Size = 30;
            player_Speed = 200;
            player_Color = Raylib.WHITE;
            playerImage = Raylib.LoadTexture("data/images/playerShip2_green.png");

            bullets.Clear();
            bullet_Size = 10;
            bullet_Color = Raylib.WHITE;
            enable_Bullets = false;

            enemies.Clear();
            enemy_Size = 25;
            enemy_Speed = 100;
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
        }

        static void Update()
        {
            PlayerUpdate();
            EnemyUpdate();
            BulletUpdate(); // Handles most of hit reg
            WinLossCheck();
        }

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
                {
                    continue;
                }
                bullet.spriteRenderer.RectangleRender();
            }
            // Draw enemies
            foreach (Enemy enemy in enemies)
            {
                if (!enemy.isActive)
                {
                    continue;
                }

                enemy.spriteRenderer.SpriteRender(180.0f, 40.0f, 30.0f);
                //enemy.spriteRenderer.RectangleRender();
            }

            Raylib.DrawText($"Score: {score}", 10, 10, 16, Raylib.WHITE);
            Raylib.DrawText($"Lives: {lives}", 10, 36, 16, Raylib.WHITE);

            Raylib.EndDrawing();
        }


        static void BulletUpdate()
        {
            foreach (Bullet bullet in bullets)
            {
                if (!bullet.isActive)
                {
                    continue;
                }
                if (bullet.WhoShot().ToString() == "Player")
                {
                    bullet.transform.MoveUp();
                    // Enemy collision check
                    foreach (Enemy enemy in enemies)
                    {
                        if (!enemy.isActive)
                        {
                            continue;
                        }

                        if (bullet.collision.CheckCollision(enemy.transform.position, enemy.collision.object_size, enemy.collision.object_size))
                        {
                            enemy.Disable();
                            bullet.Disable();
                            score += deadEnemies * 10;
                            deadEnemies++;
                            player.DecreasePlayerBulletCount();
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
                        enemy_ActiveBullets--;
                    }
                    if (bullet.WhoShot().ToString() == "Player")
                    {
                        enemy_ActiveBullets--;
                    }
                }
            }
        }

        static void PlayerUpdate()
        {
            player.InvulnerabilityTimer();

            // Keep player inside play area
            if (!player.collision.CheckCollision(new Vector2(50, 10), screen_height, screen_width - 100))
            {
                float newX = Math.Clamp(player.transform.position.X, 10, screen_width - player.collision.object_size - 10);
                float newY = Math.Clamp(player.transform.position.Y, 10, screen_height - player.collision.object_size);

                bool xChange = newX != player.transform.position.X;
                bool yChange = newY != player.transform.position.Y;

                player.transform.position.X = newX;
                player.transform.position.Y = newY;
            }

            // Side to side movement, also includes the input check
            player.moveUpdate();

            // Shoot check
            if (Raylib.IsKeyDown(KeyboardKey.KEY_SPACE))
            {
                foreach (Bullet bullet in bullets)
                {
                    if (bullet.isActive)
                    {
                        continue;
                    }
                    if (!player.GetShootDelay())
                    {
                        player.Shoot(bullet);
                    }
                }
                player.shootDelay = false;
            }
        }

        static void EnemyUpdate()
        {
            foreach (Enemy enemy in enemies)
            {
                enemy.transform.MoveRight();
                // Enemy wall collision check
                if (!enemy.collision.CheckCollision(new Vector2(50, 10), screen_height, screen_width - 100))
                {
                    changeDirection = true;
                }
            }
            if (changeDirection)
            {
                foreach (Enemy enemy in enemies)
                {
                    enemy.transform.speed *= -1.0f;
                    enemy.transform.position.Y += 5;
                }
                changeDirection = false;
            }

            if (enemy_ActiveBullets < enemyBulletLimit)
            {
                int chosenEnemy = random.Next(0, enemies.Count);
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

        static void WinLossCheck()
        {
            if (lives == 0)
                state = GameState.ScoreScreen;

            foreach (Enemy enemy in enemies)
            {
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
    }
}
