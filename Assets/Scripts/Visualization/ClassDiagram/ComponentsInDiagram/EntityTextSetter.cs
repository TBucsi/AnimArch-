using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using Visualization.ClassDiagram;
using Visualization.UI;

namespace Visualization.ClassDiagram.ComponentsInDiagram
{
    public class EntityTextSetter : MonoBehaviour
    {
        public GameObject Entity;
        public GameObject Name;

        public float charWidthMultiplier = 0.6f;
        public void setText(string text){
            var name = Name.GetComponent<TextMeshProUGUI>();
            name.text = text;
            int length = text.Length;
            float totalWidth = length * name.fontSize * charWidthMultiplier + 50;
            Entity.GetComponent<RectTransform>().sizeDelta = new Vector2(totalWidth, 80);
        }

        public void Awake(){}
    }
}
