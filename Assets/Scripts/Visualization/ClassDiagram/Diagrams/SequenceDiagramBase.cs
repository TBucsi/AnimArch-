using System.IO;
using OALProgramControl;
using UnityEngine;
using Visualization.ClassDiagram;
using Visualization.ClassDiagram.Diagrams;

namespace AnimArch.Visualization.Diagrams
{
    public abstract  class SequenceDiagramBase: Diagram
    {
        public abstract void Init(string startClassName, string fileKeyHash);
        public abstract void ToPlantUMLCommand(EXECommand command);
        public abstract void CreatePlantUMLFile();
        public abstract void LoadGeneratedDiagram();
    }
}
