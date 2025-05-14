using System.IO;
using OALProgramControl;
using UnityEngine;
using Visualization.ClassDiagram;
using Visualization.ClassDiagram.Diagrams;

namespace AnimArch.Visualization.Diagrams
{
    public class ProxySequenceDiagram : SequenceDiagramBase
    {
        private SequenceDiagramBase realDiagram;
        private bool generationStarted = false;

        public ProxySequenceDiagram()
        {}

        public void TryInitialize(string startClassName, string fileKeyHash)
        {
            string path = Application.dataPath + "/Resources/SequenceDiagrams/" + fileKeyHash + ".png";
            if (File.Exists(path))
            {
                // okamžite použijeme reálny diagram
                SequenceDiagram sequenceDiagram = GameObject.Find("SequenceDiagram").GetComponent<SequenceDiagram>();
                realDiagram = sequenceDiagram;
                realDiagram = GameObject.Find("SequenceDiagram").GetComponent<SequenceDiagram>();
                realDiagram.Init(startClassName, fileKeyHash);
                realDiagram.LoadGeneratedDiagram();
            }
            else
            {
                Debug.Log("[Proxy] PNG not found — PlantUML generation will be started.");
                realDiagram = new PlantUmlGeneratingDiagram(startClassName, fileKeyHash);
                generationStarted = true;
            }
        }

        public override void Init(string startClassName, string fileKeyHash)
        {
            realDiagram.Init(startClassName, fileKeyHash);
        }

        public override void ToPlantUMLCommand(EXECommand command)
        {
            realDiagram.ToPlantUMLCommand(command);
        }

        public override void CreatePlantUMLFile()
        {
            realDiagram.CreatePlantUMLFile();
        }

        public override void LoadGeneratedDiagram()
        {
            realDiagram.LoadGeneratedDiagram();
        }

        public bool IsGenerationPending() => generationStarted;
    }
}