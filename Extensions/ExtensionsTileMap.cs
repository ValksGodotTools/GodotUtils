namespace GodotUtils;

public static class ExtensionsTileMap 
{
    // enable a layer with Mathf.Pow(2, x - 1) where x is the layer you want enabled
    // if you wanted to enable multiple then add the sum of the powers
    // e.g. Mathf.Pow(2, 1) + Mathf.Pow(2, 3) to enable layers 0 and 2
    public static void EnableLayers(this TileMap tileMap, params uint[] layers)
    {
        uint result = 0;

        foreach (var layer in layers)
            result += GUMath.UIntPow(2, layer - 1);

        tileMap.TileSet.SetPhysicsLayerCollisionLayer(0, result);
        tileMap.TileSet.SetPhysicsLayerCollisionMask(0, result);
    }

    public static string GetTileName(this TileMap tilemap, Vector2 pos, int layer = 0)
    {
        if (!tilemap.TileExists(pos))
            return "";

        var tileData = tilemap.GetCellTileData(layer, tilemap.LocalToMap(pos));

        if (tileData == null)
            return "";

        var data = tileData.GetCustomData("Name");

        return data.AsString();
    }

    public static bool TileExists(this TileMap tilemap, Vector2 pos, int layer = 0) => tilemap.GetCellSourceId(layer, tilemap.LocalToMap(pos)) != -1;

    private static int GetCurrentTileId(this TileMap tilemap, Vector2 pos)
    {
        var cellPos = tilemap.LocalToMap(pos);
        return 0;
        //return tilemap.GetCellv(cellPos); // TODO: Godot 4 conversion
    }
}
