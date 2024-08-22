using UnityEngine;

namespace CabinetOfCuriosities
{
    public class CuriosityImage
    {
        public Texture2D Texture;
        
        public float AspectRatio => (Texture != null) ? (float) Texture.width / Texture.height : 0.0f;
        
        public CuriosityImage(Texture2D texture)
        {
            Texture = texture;
        }
        
    }
}