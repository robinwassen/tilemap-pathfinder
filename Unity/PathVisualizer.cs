using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.Scripts.GridPathfinding.Unity
{
	/// <summary>
	/// Helper component to visualize the path used 
	/// by the PathClickToWalk component
	/// </summary>
	public class PathVisualizer : MonoBehaviour
	{
		private PathClickToWalk pathClickToWalk;

		public void Start()
		{
			pathClickToWalk = this.GetComponent<PathClickToWalk>();

			if (pathClickToWalk == null)
			{
				throw new Exception("Game object with PathVisualizer component must also have a PathClickToWalk component");
			}
		}

		private void OnDrawGizmos()
		{
			if (pathClickToWalk == null || pathClickToWalk.path == null)
			{
				return;
			}

			// Just grab the first best tilemap in order to be able
			// to resolve position for the gizmos
			Tilemap tilemap = pathClickToWalk.obstacleTilemaps[0];

			Vector3 previousWaypoint = Vector3.zero;

			foreach (PathFindingNode n in pathClickToWalk.path)
			{
				Gizmos.color = Color.yellow;
				Vector3 nextWaypoint = tilemap.GetCellCenterWorld(new Vector3Int(n.x, n.y, 0));
				Gizmos.DrawWireSphere(nextWaypoint, 0.05f);

				if (previousWaypoint != Vector3.zero)
				{
					Gizmos.DrawLine(previousWaypoint, nextWaypoint);
				}

				previousWaypoint = nextWaypoint;
			}
		}
	}
}
