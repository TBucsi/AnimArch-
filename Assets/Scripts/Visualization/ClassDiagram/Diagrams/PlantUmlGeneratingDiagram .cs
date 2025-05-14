using System.IO;
using OALProgramControl;
using UnityEngine;
using Visualization.ClassDiagram;
using Visualization.ClassDiagram.Diagrams;

namespace AnimArch.Visualization.Diagrams
{

    public class PlantUmlGeneratingDiagram : SequenceDiagramBase
    {
        private VisitorCommandToPlantUML visitor = new VisitorCommandToPlantUML();
        private readonly string _startClassName;
        private readonly string _fileKeyHash;

        public PlantUmlGeneratingDiagram(string startClassName, string fileKeyHash)
        {
            _startClassName = startClassName;
            _fileKeyHash = fileKeyHash;
            StartPlantUMLCreation();
        }

        private void StartPlantUMLCreation()
        {
            visitor.classNames.Push(_startClassName);
            visitor.AddPlantUmlHeader();
            visitor.AddTransparentBackground();
            visitor.SetArrowColor("white");
        }

        public override void Init(string startClassName, string fileKeyHash) { }

        public override void ToPlantUMLCommand(EXECommand command)
        {
            command.Accept(visitor);
        }

        public override void CreatePlantUMLFile()
        {
            visitor.AddPlantUmlFutter();
            PlantUmlExecutor executor = new PlantUmlExecutor();
            executor.Execute(visitor.GetCommandString(),
                            Application.dataPath + "/Resources/SequenceDiagrams/",
                            _fileKeyHash,
                            "png");
        }

        public override void LoadGeneratedDiagram() { /* nothing for now */ }
    }
}