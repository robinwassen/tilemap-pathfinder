namespace Assets.Scripts.GridPathfinding
{
	/// <summary>
	/// Interface for a grid
	/// 
	/// Implement this interface if you wish
	/// to use a custom grid type
	/// </summary>
	public interface IPathFindingGrid
	{
		bool IsWalkable(int x, int y);
	}
}
