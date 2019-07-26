using System;
using System.Collections.Generic;

namespace Assets.Scripts.GridPathfinding
{
	[Serializable]
	public class PathFinder
	{
		private const int MAX_SEEKS = 1000;
		private readonly float SQRT_OF_2 = (float)Math.Sqrt(2);

		public List<PathFindingNode> foundNodes = new List<PathFindingNode>();
		public List<PathFindingNode> unexploredNodes = new List<PathFindingNode>();
		public PathFindingNode currentNode;
		public IPathFindingGrid grid;
		public int destinationX;
		public int destinationY;
		public bool allowDiagonalMovement = true;
		public bool allowCuttingCorners = false;

		/// <summary>
		/// Setup the pathfinder for a new seek
		/// </summary>
		/// <param name="grid">The grid to search</param>
		/// <param name="destinationX">Destination x-coord</param>
		/// <param name="destinationY">Destination y-coord</param>
		/// <param name="startX">Start x-coord</param>
		/// <param name="startY">Start y-coord</param>
		public void Set(IPathFindingGrid grid, int destinationX, int destinationY, int startX, int startY)
		{
			this.grid = grid;
			this.destinationX = destinationX;
			this.destinationY = destinationY;
			this.currentNode = new PathFindingNode(startX, startY);
			this.foundNodes.Clear();
			this.unexploredNodes.Clear();
		}

		/// <summary>
		/// Seek a path from the start to the destination
		/// </summary>
		/// <returns>Path if possible, otherwise null</returns>
		public List<PathFindingNode> Seek()
		{
			List<PathFindingNode> path = new List<PathFindingNode>();

			if (currentNode == null || grid == null)
			{
				return path;
			}

			if (!grid.IsWalkable(destinationX, destinationY))
			{
				return path;
			}

			foundNodes.Add(currentNode);

			for (int i = 0; i < MAX_SEEKS; i++)
			{
				if (currentNode.x == destinationX && currentNode.y == destinationY)
				{
					path.Add(currentNode);
					PathFindingNode n = currentNode.parent;

					while (n != null)
					{
						path.Add(n);
						n = n.parent;
					}

					// Since we iterate from the end
					// to the start we need to reverse the
					// path to get a logical order
					path.Reverse();
					break;
				}

				SeekNext();
			}

			return path;
		}
		
		/// <summary>
		/// Seeks the the next highest priority node
		/// </summary>
		private void SeekNext()
		{
			var neighbours = GetNeighbours(currentNode);

			foreach (PathFindingNode neighbour in neighbours)
			{
				// Not walkable, not interesting
				if (!grid.IsWalkable(neighbour.x, neighbour.y))
				{
					continue;
				}

				int deltaX = Math.Abs(neighbour.x - destinationX);
				int deltaY = Math.Abs(neighbour.y - destinationY);

				if (this.allowDiagonalMovement && !this.allowCuttingCorners)
				{
					if (!grid.IsWalkable(neighbour.x, currentNode.y))
					{
						continue;
					}

					if (!grid.IsWalkable(currentNode.x, neighbour.y))
					{
						continue;
					}
				}

				// We have already found the neighbour
				if (foundNodes.Contains(neighbour))
				{
					int foundIndex = foundNodes.IndexOf(neighbour);

					// Check if the current path is shorter to the 
					// neighbour than the path previously found
					// if so, update the already found
					if (neighbour.distanceToStart < foundNodes[foundIndex].distanceToStart)
					{
						foundNodes[foundIndex].parent = neighbour.parent;
						foundNodes[foundIndex].distanceToStart = neighbour.distanceToStart;
					}

					continue;
				}

				
				if (this.allowDiagonalMovement)
				{
					// Euclidean distance to end
					neighbour.distanceToEnd = (float)Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
				}
				else
				{
					// Manhattan distance to end
					neighbour.distanceToEnd = deltaX + deltaY;
				}
				
				foundNodes.Add(neighbour);
				unexploredNodes.Add(neighbour);
			}

			// Sort the unexplored nodes to prioritize exploring 
			// nodes that have shortest distance to start + end
			unexploredNodes.Sort((x, y) => x.distanceToEnd + x.distanceToStart < y.distanceToEnd + y.distanceToStart ? -1 : 1);

			// Pop the next node to explore
			currentNode = unexploredNodes[0];
			unexploredNodes.RemoveAt(0);
		}

		/// <summary>
		/// Returns the neighbours to the node
		/// </summary>
		/// <param name="node">Node to return neighbours for</param>
		/// <returns>A list of neighbours to the node</returns>
		private List<PathFindingNode> GetNeighbours(PathFindingNode node)
		{
			int x = node.x;
			int y = node.y;
			float movementCost = node.distanceToStart + 1f;

			List<PathFindingNode> n = new List<PathFindingNode>
			{
				new PathFindingNode(node, x, y + 1, movementCost), // top
				new PathFindingNode(node, x + 1, y, movementCost), // right
				new PathFindingNode(node, x, y - 1, movementCost), // bottom
				new PathFindingNode(node, x - 1, y, movementCost)  // left
			};

			if (this.allowDiagonalMovement)
			{
				float diagonalMovementCost = node.distanceToStart + SQRT_OF_2;

				n.Add(new PathFindingNode(node, x + 1, y + 1, diagonalMovementCost)); // top right
				n.Add(new PathFindingNode(node, x + 1, y - 1, diagonalMovementCost)); // bottom right
				n.Add(new PathFindingNode(node, x - 1, y - 1, diagonalMovementCost)); // bottom left
				n.Add(new PathFindingNode(node, x - 1, y + 1, diagonalMovementCost)); // top left
			}

			return n;
		}

		/// <summary>
		/// Compress a path to only contain significant nodes
		/// </summary>
		/// <param name="path">The path to compress</param>
		/// <returns>The compressed path</returns>
		public static List<PathFindingNode> CompressPath(List<PathFindingNode> path)
		{
			// Less than three parts in the path
			// Nothing to compress
			if (path.Count < 3)
			{
				return path;
			}

			List<PathFindingNode> compressedPath = new List<PathFindingNode>();

			int prevDirectionX = int.MaxValue;
			int prevDirectionY = int.MaxValue;
			PathFindingNode prevNode = null;

			foreach (PathFindingNode n in path)
			{
				// Skip the first
				// It will automatically be added next iteration
				if (prevNode == null)
				{
					prevNode = n;
					continue;
				}

				int directionX = (n.x - prevNode.x);
				int directionY = (n.y - prevNode.y);

				int directionDeltaX = prevDirectionX - directionX;
				int directionDeltaY = prevDirectionY - directionY;

				// Direction has changed, previous node is significant
				if (directionDeltaX != 0 || directionDeltaY != 0)
				{
					compressedPath.Add(prevNode);
				}

				prevDirectionX = directionX;
				prevDirectionY = directionY;
				prevNode = n;
			}

			// Include the last node
			compressedPath.Add(path[path.Count - 1]);

			return compressedPath;
		}
	}
}
