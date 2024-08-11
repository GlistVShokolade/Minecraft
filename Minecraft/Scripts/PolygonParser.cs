public static class PolygonParser
{
    public static ParsedPolygonsData Parse(Polygon[] polygons)
    {
        var vertices = new List<float>();
        var texcoords = new List<float>();
        var normals = new List<float>();

        for (int i = 0; i < polygons.Length; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                vertices.Add(polygons[i].Vertices[j].X);
                vertices.Add(polygons[i].Vertices[j].Y);
                vertices.Add(polygons[i].Vertices[j].Z);

                texcoords.Add(polygons[i].TexCoords[j].X);
                texcoords.Add(polygons[i].TexCoords[j].Y);

                normals.Add(polygons[i].Normal.X);
                normals.Add(polygons[i].Normal.Y);
                normals.Add(polygons[i].Normal.Z);
            }
        }

        return new ParsedPolygonsData(vertices, texcoords, normals);
    }
}
