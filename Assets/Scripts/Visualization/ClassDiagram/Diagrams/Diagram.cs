using UMSAGL.Scripts;
using UnityEngine;

namespace Visualization.ClassDiagram.Diagrams
{
    public class Diagram : MonoBehaviour
    {
        protected float offsetZ = 800;
        public Graph graph;

        public void setZOffset(float newOffset)
        {
            offsetZ = newOffset;
        }

        public GameObject CreateInterGraphLine(GameObject start, GameObject end)
        {
            GameObject Line = Instantiate(DiagramPool.Instance.interGraphLinePrefab);

            Line.GetComponent<LineRenderer>().widthMultiplier = 4f;
            //Line.transform.SetParent(graph.units);

            return Line;
        }
    }
}
