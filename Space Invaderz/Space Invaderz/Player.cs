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

        // Bullet timing
        double shootInterval = 0.3; // In seconds
        double lastShootTime;
        public int playerBulletLimit = 4;
        private int activeBullets = 0;
        private bool playerInvulnerability = false;
        double player_InvulnerabilityTimer = 3.0;
        double lastInvulnerabilityTime;

        public Player(Vector2 position, int size, float speed, Color color, Texture texture)
        {
            transform = new Transform(position, speed);
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
            if (Raylib.IsKeyDown(KeyboardKey.KEY_RIGHT))
            {
                transform.MoveRight();
            }
            if (Raylib.IsKeyDown(KeyboardKey.KEY_LEFT))
            {
                transform.MoveLeft();
            }
        }
    }
}
