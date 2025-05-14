using System;
using System.Collections.Generic;
using UnityEngine;

namespace Visualization.ClassDiagram.Diagrams
{
    public class GridLayout : AbstractLayout
    {
        private static AbstractLayout otherLayout = new QueueLayout();

        public override AbstractLayout changeLayout(List<Diagram> diagramList, float Offset)
        {
            var diagramIndex = 0;
            var rows = (int)Math.Ceiling(Math.Sqrt(diagramList.Count));
            var cols = (int)Math.Ceiling((double)diagramList.Count / rows);

            for (var j = 0; j < rows; j++)
            {
                for (var i = 0; i < cols; i++)
                {
                    if (diagramIndex >= diagramList.Count)
                        return otherLayout;
                    var diagram = diagramList[diagramIndex++];
                    if (!diagram || !diagram.graph)
                        return otherLayout;

                    // set grid position for diagrams in the list.
                    diagram.graph.transform.position = new Vector3(i * Offset, j * Offset, 0);
                }
            }
            return otherLayout;
        }
    }
}