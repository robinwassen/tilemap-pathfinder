using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.Scripts.GridPathfinding.Unity
{
	/// <summary>
	/// Component that use the PathFinder to determine
	/// how to move from the current position to the
	/// click position on a tile map
	/// </summary>
	public class PathClickToWalk : MonoBehaviour
	{
		public List<Tilemap> obstacleTilemaps;
		public List<PathFindingNode> path;

		[SerializeField]
		private PathFinder pathFinder = new PathFinder();
		private UnityTilemapGrid grid;
		private PathWalker walker;

		private void Start()
		{
			walker = this.GetComponent<PathWalker>();

			if (walker == null)
			{
				throw new Exception("Game object with PathClickToWalk component must also have a PathWalker component");
			}

			grid = new UnityTilemapGrid { obstacleMaps = this.obstacleTilemaps };
		}

		void Update()
		{
			if (Input.GetButton("Fire1"))
			{
				walker.SetPath(null);

				Tilemap tilemap = walker.groundTilemap;

				Vector3Int currentCell = tilemap.WorldToCell(transform.position);
				Vector3 clickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				Vector3Int destinationCell = tilemap.WorldToCell(clickPosition);

				pathFinder.Set(this.grid, destinationCell.x, destinationCell.y, currentCell.x, currentCell.y);

				this.path = pathFinder.Seek();
				this.path = PathFinder.CompressPath(this.path);
			}

			if (Input.GetButtonUp("Fire1"))
			{
				walker.SetPath(path);
			}
		}
	}
}
