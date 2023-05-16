using Raylib_CsLo;
using System;
using System.Numerics;

namespace Space_Invaderz
{
    class Enemy : GameObject
    {
        public Transform transform;
        public Collision collision;
        public SpriteRenderer spriteRenderer;
        public bool shootDelay = false;
        public bool isActive;
        public bool movingRight = true;
        public bool movingLeft = false;

        // Bullet timing
        double shootInterval = 0.2; // In seconds
        double lastShootTime;

        public Enemy(Vector2 position, int size, float speed, Color color, bool isActive, Texture texture)
        {
            transform = new Transform(position, speed);
            collision = new Collision(transform, size);
            spriteRenderer = new SpriteRenderer(transform, color, collision.object_size, texture);
            this.isActive = isActive;
        }

        /// <summary>
        /// Returns shoot delay
        /// </summary>
        /// <returns></returns>
        public bool GetShootDelay()
        {
            return shootDelay;
        }

        /// <summary>
        /// Disables enemy
        /// </summary>
        public void Disable()
        {
            isActive = false;
        }

        /// <summary>
        /// Activates enemy
        /// </summary>
        public void Activate()
        {
            isActive = true;
        }

        /// <summary>
        /// Handles shooting cooldown. Is given a inactive bullet that it activates if shooting cooldown=false
        /// </summary>
        /// <param name="bullet"></param>
        public void Shoot(Bullet bullet)
        {
            // Enemy shoots
            double timeNow = Raylib.GetTime();
            double timeSinceLastShot = timeNow - lastShootTime;
            if (timeSinceLastShot >= shootInterval)
            {
                Console.WriteLine("Enemy shoots!");
                lastShootTime = timeNow;

                bullet.transform.position = spriteRenderer.sprite_transform.position;
                bullet.Activate("Enemy");

                shootDelay = true;
            }
        }

        /// <summary>
        /// Changes the enemies direction and also drops them down a bit
        /// </summary>
        public void ChangeDirection()
        {
            transform.maxSpeed *= -1.0f;
            transform.position.Y += 10;
        }

        /// <summary>
        /// Sets the enemies texture to given texture
        /// </summary>
        /// <param name="texture"
        public void SetTexture(Texture texture)
        {
            this.spriteRenderer.texture = texture;
        }

        public bool ScreenCollisionCheck(int screen_height, int screen_width)
        {
            if (collision.CheckCollision(new Vector2(40, 10), screen_height, screen_width - 100))
            {
                return false;
            }
            return true;
        }
    }
}