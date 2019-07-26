using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.Scripts.GridPathfinding.Unity
{
	/// <summary>
	/// Implementation of a grid that use Unity's 
	/// Tilemaps to determine if a tile is walkable or not
	/// </summary>
	public class UnityTilemapGrid : IPathFindingGrid
	{
		public List<Tilemap> obstacleMaps;

		public bool IsWalkable(int x, int y)
		{
			foreach (Tilemap om in obstacleMaps)
			{
				if (om.GetTile(new Vector3Int(x, y, 0)))
				{
					return false;
				}
			}

			return true;
		}
	}
}
