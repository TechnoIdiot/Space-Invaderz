using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Raylib_CsLo;

namespace Space_Invaderz
{
    class Bullet : GameObject
    {
        public enum ShotBy
        {
            Player,
            Enemy
        }

        public ShotBy entity;

        public Transform transform;
        public Collision collision;
        public SpriteRenderer spriteRenderer;
        public bool isActive;

        public Bullet(Vector2 position, int size, Color color, bool isActive, Texture texture)
        {
            this.transform = new Transform(position, 350f);
            collision = new Collision(transform, size);
            spriteRenderer = new SpriteRenderer(transform, color, collision.object_size, texture);
            this.isActive = isActive;
        }

        /// <summary>
        /// Disables bullet
        /// </summary>
        public void Disable()
        {
            isActive = false;
        }

        /// <summary>
        /// Activates bullet. Also saves who was it shot by
        /// </summary>
        public void Activate(string shotBy)
        {
            entity = Enum.Parse<ShotBy>(shotBy);
            isActive = true;
        }

        /// <summary>
        /// Returns who shot the bullet
        /// </summary>
        public ShotBy WhoShot()
        {
            return entity;
        }
    }
}
