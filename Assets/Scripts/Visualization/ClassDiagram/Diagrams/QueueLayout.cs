using System.Collections.Generic;
using UnityEngine;

namespace Visualization.ClassDiagram.Diagrams
{
    public class QueueLayout : AbstractLayout
    {
        private static AbstractLayout otherLayout = new GridLayout();

        public override AbstractLayout changeLayout(List<Diagram> diagramList, float Offset)
        {
            for (var i = 0; i < diagramList.Count; i++)
            {
                var diagram = diagramList[i];
                if (diagram)
                {
                    if (diagram.graph)
                    {
                        diagram.graph.transform.position = new Vector3(0, 0, Offset * i);
                    }
                }
            }
            return otherLayout;
        }
    }
}