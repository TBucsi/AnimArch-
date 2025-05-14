using System.IO;
using UnityEngine;

namespace Visualization.ClassDiagram.Diagrams
{
    public class SpriteChanger : MonoBehaviour
    {
        public void SetSprite(string pngPath)
        {
            Sprite sprite = ConvertPngToSprite(pngPath);
            if (sprite != null)
            {
                gameObject.GetComponent<SpriteRenderer>().sprite = sprite;
            }
            else
            {
                Debug.LogError("[PLANTUML] PNGpath could not be converted to Sprite.");
            }
        }

        private Sprite ConvertPngToSprite(string pngPath)
        {
            if (!File.Exists(pngPath))
            {
                Debug.LogError("[PLANTUML] File not found at path: " + pngPath);
                return null;
            }

            byte[] pngBytes = File.ReadAllBytes(pngPath);
            Texture2D texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);

            if (texture.LoadImage(pngBytes))
            {
                return Sprite.Create(
                    texture,
                    new Rect(0, 0, texture.width, texture.height),
                    new Vector2(0.5f, 0.5f),
                    5f
                );
            }

            Debug.LogError("Failed to load image from path: " + pngPath);
            return null;
        }
    }
}