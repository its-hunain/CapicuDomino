using System.Collections.Generic;
using UnityEngine;

public class Player3dHandTiles : MonoBehaviour
{
    [SerializeField] private List<GameObject> tiles; // 21 pre-placed tiles

    private int currentVisibleCount = 0;

    // Enable the next tile
    public void ShowNextTile()
    {
        if (currentVisibleCount < tiles.Count)
        {
            GameObject tile = tiles[currentVisibleCount];
            tile.SetActive(true);

            string themeName = PlayerPrefs.GetString("TileTheme", "TileTheme1");
            Color themeColor = TileTheme.ColorUtility.UpdateTileTheme(themeName);

            if (tile.TryGetComponent<MeshRenderer>(out var meshRenderer))
            {
                meshRenderer.material.color = themeColor;
            }
            else
            {
                Debug.LogWarning($"Tile at index {currentVisibleCount} has no MeshRenderer component.");
            }

            currentVisibleCount++;
        }
    }

    // Disable the last visible tile
    public void HideLastTile()
    {
        if (currentVisibleCount > 0)
        {
            currentVisibleCount--;
            tiles[currentVisibleCount].SetActive(false);
        }
    }

    // Enable all tiles
    public void ShowAllTiles()
    {
        foreach (var tile in tiles)
        {
            tile.SetActive(true);
        }
        currentVisibleCount = tiles.Count;
    }

    // Disable all tiles
    public void HideAllTiles()
    {
        foreach (var tile in tiles)
        {
            tile.SetActive(false);
        }
        currentVisibleCount = 0;
    }
}
