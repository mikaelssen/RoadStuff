using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raylib;

namespace RoadGenerator2000
{
	class Road
	{
	}

	class Node
	{
		public Node() { }
		public Node(Vector3 pos) {
			position = pos;
		}
		public Node(Vector3 pos, List<Node> neigbors) {
			position = pos;
			neigbors = Neigbors;
		}
		public List<Node> Neigbors = new List<Node>();
		public Vector3 position = new Vector3(0, 0, 0);
	}
}
