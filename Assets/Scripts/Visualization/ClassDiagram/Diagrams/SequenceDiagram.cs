using System.IO;
using OALProgramControl;
using UnityEngine;
using Visualization.ClassDiagram;
using Visualization.ClassDiagram.Diagrams;

namespace AnimArch.Visualization.Diagrams
{
    public class SequenceDiagram : SequenceDiagramBase
    {
        private const string OUTPUT_FORMAT = "png";
        private const string OUTPUT_DIR = "/Resources/SequenceDiagrams/";

        private string diagramNameHash;
        private string startClassName;

        public override void Init(string startClassName, string fileKeyHash)
        {
            this.startClassName = startClassName;
            this.diagramNameHash = fileKeyHash;

            ResetDiagram();
        }

        private void ResetDiagram()
        {
            if (graph != null)
            {
                Destroy(graph.gameObject);
                graph = null;
            }
        }

        public override void ToPlantUMLCommand(EXECommand command)
        {
        }

        public override void CreatePlantUMLFile()
        {
        }

        public override void LoadGeneratedDiagram()
        {
            string fullPath = Application.dataPath + OUTPUT_DIR + diagramNameHash + ".png";

            if (!File.Exists(fullPath))
            {
                Debug.LogError("[RealSequenceDiagram] PNG not found when trying to load: " + fullPath);
                return;
            }

            Debug.Log("[RealSequenceDiagram] Loading PNG from path: " + fullPath);
            var spriteChanger = GetComponent<SpriteChanger>();
            spriteChanger?.SetSprite(fullPath);

            transform.rotation = Quaternion.Euler(0, 180, 0);
            transform.localScale *= 5f;
        }
    }
}
