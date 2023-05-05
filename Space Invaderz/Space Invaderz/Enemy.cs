using Raylib_CsLo;
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

        public bool GetShootDelay()
        {
            return shootDelay;
        }
        public void Disable()
        {
            isActive = false;
        }

        public void Activate()
        {
            isActive = true;
        }

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

        public void SetTexture(Texture texture)
        {
            this.spriteRenderer.texture = texture;
        }
    }
}