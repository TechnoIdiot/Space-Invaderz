using Raylib_CsLo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Space_Invaderz
{
    internal class Particles : GameObject
    {
        float size;
        float max_size;
        float lifetime_left;
        float max_lifetime;
        public bool isActive;
        Transform transform;
        Random randomGenerator;
       
        public Particles(float max_size, float max_lifetime, bool isActive, Transform transform, Random randomGenerator)
        {
            this.max_size = max_size;
            this.max_lifetime = max_lifetime;
            this.isActive = isActive;
            this.transform = transform;
            this.randomGenerator = randomGenerator;
        }

        /// <summary>
        /// Updates particle and renders it
        /// </summary>
        public void Update()
        {
            lifetime_left -= Raylib.GetFrameTime();
            float progress = lifetime_left / max_lifetime;
            size = Math.Max(progress * max_size, 0.0f); // size is never negative7
            ParticleRender();
            if (lifetime_left < 0.0f)
            {
                isActive = false;
            }
        }

        public void ParticleRender()
        {
            Raylib.DrawCircle((int)transform.position.X, (int)transform.position.Y, size, Raylib.ORANGE);
        }

        /// <summary>
        /// Gets enemy and uses it to position the particle at the enemys position
        /// </summary>
        /// <param name="enemy"></param>
        public void EnemyInit(Enemy enemy)
        {
            size = max_size;
            Vector2 direction = new Vector2(randomGenerator.Next(-1, 1), randomGenerator.Next(-1, 1));
            Vector2.Normalize(direction);
            transform.position = enemy.transform.position += direction;
            isActive = true;
            lifetime_left = max_lifetime;
        }

        public void PlayerInit(Player player)
        {
            size = max_size;
            Vector2 direction = new Vector2(randomGenerator.Next(-1, 1), randomGenerator.Next(-1, 1));
            Vector2.Normalize(direction);
            transform.position = player.transform.position += direction;
            isActive = true;
            lifetime_left = max_lifetime;
        }
    }
}
