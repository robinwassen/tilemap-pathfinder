using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.Scripts.GridPathfinding.Unity
{
	/// <summary>
	/// Example component to show how movement along
	/// a path could work in Unity.
	/// 
	/// Not great, not terrible.
	/// </summary>
	public class PathWalker : MonoBehaviour
	{
		public float speed = 1f;
		public int pathIndex;
		public List<PathFindingNode> path;
		public bool moving;
		public Tilemap groundTilemap;

		public void SetPath(List<PathFindingNode> path)
		{
			this.path = path;
			this.moving = path != null;
			this.pathIndex = 0;
		}

		private void Update()
		{
			if (!moving)
			{
				return;
			}

			if (this.pathIndex >= this.path.Count)
			{
				this.moving = false;
				return;
			}

			float updateMovementRemaining = Time.deltaTime * speed;

			// Perform multiple movement operations if needed
			// to complete the movement to the current waypoint
			// before moving towards the next one
			while (updateMovementRemaining > 0 && this.pathIndex < this.path.Count)
			{
				PathFindingNode nextNode = this.path[this.pathIndex];
				Vector3 nextNodePosition = groundTilemap.GetCellCenterWorld(new Vector3Int(nextNode.x, nextNode.y, 0));
				Vector3 currentPosition = this.gameObject.transform.position;

				Vector3 moveVector = nextNodePosition - currentPosition;

				float maxMoveDistance = moveVector.magnitude;
				float moveDistance = Math.Min(maxMoveDistance, updateMovementRemaining);

				if (updateMovementRemaining > maxMoveDistance)
				{
					this.pathIndex++;
				}

				moveVector.Normalize();
				moveVector = moveVector * moveDistance;

				gameObject.transform.localPosition = currentPosition + moveVector;
				updateMovementRemaining -= moveDistance;
			}
		}
	}
}
