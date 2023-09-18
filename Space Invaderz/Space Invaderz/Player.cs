using Raylib_CsLo;
using System.Numerics;

namespace Space_Invaderz
{
    class Player : GameObject
    {
        public Transform transform;
        public Collision collision;
        public SpriteRenderer spriteRenderer;
        public bool shootDelay = true;
        public float accelerationSpeed = 0.3f;
        public float slowDownSpeed = 0.4f;
        Vector2 velocity;

        // Bullet timing
        double shootInterval = 0.3; // In seconds
        double lastShootTime;
        public int playerBulletLimit = 4;
        private int activeBullets = 0;
        private bool playerInvulnerability = false;
        double player_InvulnerabilityTimer = 2.0;
        double lastInvulnerabilityTime;

        public Player(Vector2 position, int size, float maxSpeed, Color color, Texture texture)
        {
            transform = new Transform(position, maxSpeed);
            collision = new Collision(transform, size);
            spriteRenderer = new SpriteRenderer(transform, color, collision.object_size, texture);
        }

        public void Shoot(Bullet bullet)
        {
            // Player shoots
            double timeNow = Raylib.GetTime();
            double timeSinceLastShot = timeNow - lastShootTime;
            if (timeSinceLastShot >= shootInterval)
            {
                if (activeBullets < playerBulletLimit)
                {
                    Console.WriteLine("Player shoots!");
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

        public void DecreasePlayerBulletCount()
        {
            if (activeBullets < 8)
                activeBullets--;
        }

        public void InvulnerabilityTimer()
        {
            double timeNow = Raylib.GetTime();
            double timeSinceLastInvuln = timeNow - lastInvulnerabilityTime;
            if (timeSinceLastInvuln >= player_InvulnerabilityTimer)
            {
                lastInvulnerabilityTime = timeNow;
                playerInvulnerability = false;
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
            // Check that does not go too fast!
            if (velocity.Length() > transform.maxSpeed)
            {
                velocity = Vector2.Normalize(velocity) * transform.maxSpeed;
            }
            transform.position += velocity;
        }

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
    }
}
