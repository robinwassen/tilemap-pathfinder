using System;

namespace Assets.Scripts.GridPathfinding
{
	[Serializable]
	public class PathFindingNode
	{
		public int x;
		public int y;
		public float distanceToEnd;
		public float distanceToStart;
		public PathFindingNode parent;

		public PathFindingNode(int x, int y)
		{
			this.x = x;
			this.y = y;
		}

		public PathFindingNode(PathFindingNode parent, int x, int y, float distanceToStart)
		{
			this.x = x;
			this.y = y;
			this.parent = parent;
			this.distanceToStart = distanceToStart;
		}

		public override bool Equals(object obj)
		{
			if (obj is PathFindingNode other)
			{
				// Simplified equal to check that the nodes
				// refer to the same position on the grid
				// and skip checking the path to that position
				//
				// Purpose is to make list operations convinient
				// rather than checking strict equality
				return this.x == other.x && this.y == other.y;
			}
			else
			{
				return base.Equals(obj);
			}
		}

		public override int GetHashCode()
		{
			unchecked
			{
				int hash = (int)2166136261;
				hash = (hash * 16777619) ^ this.x.GetHashCode();
				hash = (hash * 16777619) ^ this.y.GetHashCode();
				return hash;
			}
		}
	}
}
