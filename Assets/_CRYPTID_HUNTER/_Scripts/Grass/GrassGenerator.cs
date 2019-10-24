using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;

[ExecuteInEditMode]
public class GrassGenerator : MonoBehaviour
{

    [SerializeField] private MeshFilter meshFilter;
    [OnValueChanged("GenerateGrass")]
    [SerializeField] private int seed;
    [Range(1, 60000)]
    [SerializeField] private int numberOfGrassTufts;
    [SerializeField] private Vector2 areaToGrow;
    [SerializeField] private float startHeight;
    [SerializeField] private float grassOffset;
    [SerializeField] private LayerMask hitLayer;

    private Transform cachedTransform;
    private Mesh mesh;

    // Start is called before the first frame update
    void Start()
    {
        cachedTransform = transform;
        //GenerateGrass();
    }

    [Button("GenerateGrass")]
    private void GenerateGrass()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.SetActive(false);
        Random.InitState(seed);
        List<Vector3> positions = new List<Vector3>(numberOfGrassTufts);
        List<int> indicies = new List<int>();//(numberOfGrassTufts);
        List<Color> colors = new List<Color>(numberOfGrassTufts);
        List<Vector3> normals = new List<Vector3>(numberOfGrassTufts);

        int finalGrassNumb = 0;
        for (int i = 0; i < numberOfGrassTufts; ++i)
        {
            Vector3 origin = this.transform.position;
            origin.y = startHeight;
            origin.x += areaToGrow.x * Random.Range(-0.5f, 0.5f);
            origin.z += areaToGrow.y * Random.Range(-0.5f, 0.5f);
            Ray ray = new Ray(origin, Vector3.down);
            RaycastHit hit;
            if (Physics.SphereCast(ray, .5f, out hit))//.Raycast(ray, out hit))
            {
                if(((1 << hit.collider.gameObject.layer) & hitLayer.value) == 0)
                {
                    continue;
                }
                Vector3 grassPos = hit.point;
                grassPos.y += grassOffset;
                grassPos -= this.transform.position;

                positions.Add(grassPos);
                indicies.Add(finalGrassNumb++);
                colors.Add(new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), 1));
                normals.Add(hit.normal);
            }
        }

        mesh = new Mesh();
        mesh.SetVertices(positions);
        mesh.SetIndices(indicies.ToArray(), MeshTopology.Points, 0);
        mesh.SetColors(colors);
        mesh.SetNormals(normals);
        meshFilter.mesh = mesh;

        player.SetActive(true);
    }
}
