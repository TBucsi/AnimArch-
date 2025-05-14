using System;
using System.Collections.Generic;

namespace Visualization.ClassDiagram.Diagrams
{
    public abstract class AbstractLayout
    {
        public virtual AbstractLayout changeLayout(List<Diagram> diagramList, float Offset)
        {
            throw new NotImplementedException();
        }
    }
}