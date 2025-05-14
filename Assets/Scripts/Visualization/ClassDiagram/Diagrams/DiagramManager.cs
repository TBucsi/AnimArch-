using System;
using System.Collections.Generic;
using AnimArch.Visualization.Diagrams;
using UnityEngine;

namespace Visualization.ClassDiagram.Diagrams
{
    public class DiagramManager : Singleton<DiagramManager>
    {
        [SerializeField] private float Offset;

        [SerializeField] public ClassDiagram classDiagram;
        [SerializeField] public ObjectDiagram objectDiagram;
        [SerializeField] public ActivityDiagram activityDiagram;
        [SerializeField] public SequenceDiagramBase sequenceDiagram;

        private List<Diagram> diagramList;
        private AbstractLayout layoutState = new GridLayout();

        private void Awake()
        {
            diagramList = new List<Diagram>()
            {
                classDiagram,
                activityDiagram,
                objectDiagram,
                sequenceDiagram
            };
        }

        public void ChangeLayout()
        {
            layoutState = layoutState.changeLayout(diagramList, Offset);
            PinCamToDiagramLayout();
        }

        private void PinCamToDiagramLayout()
        {
            var camera = Camera.main;
            if (camera == null) return;

            var bounds = new Bounds(Vector3.zero, Vector3.zero);
            foreach (var diagram in diagramList)
            {
                if (!diagram || !diagram.graph)
                    return;
                
                bounds.Encapsulate(diagram.graph.transform.position);
                
            }
            
            // TODO: eatch call of this method the camera is moved to back of couple of px, with cause that eventually
            // it will be moved to far from the diagram (it is no longer visible)
            const float cameraPadding = 1f;
            camera.transform.position = 
                new Vector3(
                    bounds.center.x, bounds.center.y, camera.transform.position.z) - 
                    camera.transform.forward * (Mathf.Max(bounds.size.x, bounds.size.y) / cameraPadding
                );
            camera.orthographicSize = Mathf.Max(bounds.size.x, bounds.size.y) / 2;
        }
    }
}