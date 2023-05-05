using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Raylib_CsLo;

namespace Space_Invaderz
{
    class GameObject
    {
        public class Transform
        {
            public Vector2 position;
            public float speed;

            public Transform(Vector2 position, float speed)
            {
                this.position = position;
                this.speed = speed;
            }

            public void MoveDown()
            {
                position.Y += speed * Raylib.GetFrameTime();
            }
            public void MoveUp()
            {
                position.Y -= speed * Raylib.GetFrameTime();
            }
            public void MoveLeft()
            {
                position.X -= speed * Raylib.GetFrameTime();
            }
            public void MoveRight()
            {
                position.X += speed * Raylib.GetFrameTime();
            }
        }

        public class Collision
        {
            public int object_size;
            public Transform object_transform;

            public Collision(Transform transform, int size)
            {
                this.object_transform = transform;
                this.object_size = size;
            }

            public bool CheckCollision(Vector2 collisionPos, float collision_Height, float collision_Width)
            {
                Rectangle collider = new Rectangle(object_transform.position.X, object_transform.position.Y, object_size, object_size);
                Rectangle collision = new Rectangle(collisionPos.X, collisionPos.Y, collision_Width, collision_Height);
                return Raylib.CheckCollisionRecs(collider, collision);
            }
        }

        public class SpriteRenderer
        {
            public Transform sprite_transform;
            public Color sprite_color;
            int sprite_size;
            public Texture texture;

            public SpriteRenderer(Transform transform, Color color, int size, Texture texture)
            {
                this.sprite_transform = transform;
                this.sprite_color = color;
                this.sprite_size = size;
                this.texture = texture;
            }

            public void RectangleRender()
            {
                Raylib.DrawRectangle((int)sprite_transform.position.X, (int)sprite_transform.position.Y, sprite_size, sprite_size, sprite_color);
            }

            public void SpriteRender(float spriteRotation, float offsetX, float offsetY)
            {
                Raylib.DrawTextureEx(texture, sprite_transform.position+new Vector2(offsetX, offsetY), spriteRotation, 0.5f,  sprite_color);
            }
        }
    }
}
