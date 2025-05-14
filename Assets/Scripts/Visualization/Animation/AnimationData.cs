using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Visualisation.Animation;
using Visualization.UI;

namespace Visualization.Animation
{
    //Holds data for created animations
    //Implemets methods to find, create and remove animations
    //has global acces: AnimationData.Instance.calledMethod()
    public class AnimationData : Singleton<AnimationData>
    {
        public Anim selectedAnim;
        public string diagramPath = @"C:/AnimArch/exportedXMI.xml";
        public int diagramId = 4;
        public float AnimSpeed { get; set; }
        private void Awake()
        {
            selectedAnim = new Anim("");
        }
        public string GetDiagramPath()
        {
            return diagramPath;
        }
        public void SetDiagramPath(string diagramPath)
        {
            this.diagramPath = diagramPath;
            this.diagramId = 0;

        }
        public void RemoveAnimation()
        {
            selectedAnim = new Anim("");
            diagramId = 0;
            
            MenuManager.SetAnimationButtonsActive(false);
        }
    }
}
