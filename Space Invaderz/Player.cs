using Raylib_CsLo;
using System.Numerics;

namespace Space_Invaderz
{
    class Player : GameObject
    {
        // Classes
        public Transform transform;
        public Collision collision;
        public SpriteRenderer spriteRenderer;

        // Player variables
        public bool shootDelay = true;
        public float accelerationSpeed = 0.3f;
        public float slowDownSpeed = 0.4f;
        Vector2 velocity;

        // Shooting variables
        double shootInterval = 0.3; // In seconds
        double lastShootTime;
        public int playerBulletLimit = 4;
        private int activeBullets = 0;

        // Invulnerability variables
        private bool playerInvulnerability = false;
        double player_InvulnerabilityTimer = 2.0;
        double lastInvulnerabilityTime;

        // Cheat variable
        bool godmode;

        public Player(Vector2 position, int size, float maxSpeed, Color color, Texture texture)
        {
            transform = new Transform(position, maxSpeed);
            collision = new Collision(transform, size);
            spriteRenderer = new SpriteRenderer(transform, color, collision.object_size, texture);
        }

        /// <summary>
        /// Players shooting function. Sends a bullet from players position upwards
        /// </summary>
        /// <param name="bullet"></param>
        public void Shoot(Bullet bullet)
        {
            // Player shoots
            double timeNow = Raylib.GetTime();
            double timeSinceLastShot = timeNow - lastShootTime;
            if (timeSinceLastShot >= shootInterval)
            {
                if (activeBullets < playerBulletLimit)
                {
                    lastShootTime = timeNow;

                    bullet.transform.position = transform.position;
                    bullet.transform.position.Y -= 10;
                    bullet.Activate("Player");
                    activeBullets++;

                    shootDelay = true;
                }
            }
        }

        public bool GetShootDelay()
        {
            return shootDelay;
        }


        public void DecreaseActiveBulletAmount()
        {
            if (activeBullets <= playerBulletLimit)
                activeBullets--;
        }

        /// <summary>
        /// Used after player gets shot. Starts a timer that stops player from losing hearts.
        /// </summary>
        public void InvulnerabilityTimer()
        {
            if (!godmode)
            {
                double timeNow = Raylib.GetTime();
                double timeSinceLastInvuln = timeNow - lastInvulnerabilityTime;
                if (timeSinceLastInvuln >= player_InvulnerabilityTimer)
                {
                    lastInvulnerabilityTime = timeNow;
                    playerInvulnerability = false;
                }
            }
            if (godmode)
            {
                playerInvulnerability = true;
            }
        }

        public bool GetPlayerInvulnerability()
        {
            return playerInvulnerability;
        }

        public void EnablePlayerInvulnerability()
        {
            playerInvulnerability = true;
        }

        public void SetTexture(Texture texture)
        {
            this.spriteRenderer.texture = texture;
        } 

        /// <summary>
        /// Player movement updates
        /// </summary>
        public void moveUpdate()
        {
            Vector2 new_direction = ReadDirectionInput();
            if (new_direction.X == 0 && new_direction.Y == 0)
            {
                // No direction given, slow down
                velocity.X -= slowDownSpeed;
                if (velocity.X < 0)
                {
                    velocity.X = 0;
                }
            }

            Vector2 velocityChange = new_direction * accelerationSpeed;
            velocity += velocityChange;
            // Check that player does not go too fast!
            if (velocity.Length() > transform.maxSpeed)
            {
                velocity = Vector2.Normalize(velocity) * transform.maxSpeed;
            }
            transform.position += velocity;
        }

        /// <summary>
        /// Player input checks
        /// </summary>
        /// <returns></returns>
        private Vector2 ReadDirectionInput()
        {
            if (Raylib.IsKeyDown(KeyboardKey.KEY_RIGHT))
            {
                return new Vector2(1, 0);
            }
            if (Raylib.IsKeyDown(KeyboardKey.KEY_LEFT))
            {
                return new Vector2(-1, 0);
            }
            return new Vector2(0, 0);
        }

        /// <summary>
        /// Stops player from escaping
        /// </summary>
        /// <param name="screen_height"></param>
        /// <param name="screen_width"></param>
        public void KeepInsidePlayArea(int screen_height, int screen_width)
        {
            if (!collision.CheckCollision(new Vector2(50, 10), screen_height, screen_width - 100))
            {
                float newX = Math.Clamp(transform.position.X, 10, screen_width - collision.object_size - 10);
                float newY = Math.Clamp(transform.position.Y, 10, screen_height - collision.object_size);

                bool xChange = newX != transform.position.X;
                bool yChange = newY != transform.position.Y;

                transform.position.X = newX;
                transform.position.Y = newY;
            }
        }

        public int GetActiveBullets()
        {
            return activeBullets;
        }

        public void ToggleGodMode()
        {
            if (godmode)
            {
                godmode = false;
            }
            else if (!godmode)
            {
                godmode = true;
            }
        }
    }
}
