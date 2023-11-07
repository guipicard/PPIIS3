using UnityEngine;

public class MapCreator : MonoBehaviour
{
    [Header("Cubes")] //
    [SerializeField] private GameObject Stone;
    [SerializeField] private GameObject Grass;
    [SerializeField] private GameObject Dirt;
    [SerializeField] private Texture2D HeightMap;

    [Header("Map Creation")] //
    [SerializeField] private string MapName;

    // GAIA heightmap
    public void CreateMap()
    {
        GameObject parent = new GameObject(MapName);
        for (int i = 0; i < HeightMap.width; i++)
        {
            //string debugString = "";
            for (int j = 0; j < HeightMap.height; j++)
            {
                Color color = HeightMap.GetPixel(i, j);
                float k = (color.grayscale * 20);
                k = Mathf.Floor(k);
                //debugString += k.ToString() + ", ";
                if (k < 1.0f)
                {
                    Instantiate(Grass, new Vector3(i, k, j), Quaternion.identity, parent.transform);
                }
                else if (k < 4)
                {
                    Instantiate(Grass, new Vector3(i, k, j), Quaternion.identity, parent.transform);
                    if (k > 1.0f) Instantiate(Dirt, new Vector3(i, k- 1, j), Quaternion.identity, parent.transform);
                    if (k > 1.0f) Instantiate(Dirt, new Vector3(i, k- 2, j), Quaternion.identity, parent.transform);
                }
                else
                {
                    Instantiate(Stone, new Vector3(i, k, j), Quaternion.identity, parent.transform);
                    Instantiate(Stone, new Vector3(i, k - 1, j), Quaternion.identity, parent.transform);
                    Instantiate(Stone, new Vector3(i, k - 2, j), Quaternion.identity, parent.transform);
                }
            }
            //Debug.Log(debugString);
        }
    }
}