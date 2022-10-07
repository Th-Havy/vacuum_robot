using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;

[RequireComponent(typeof(Lidar))]
public class LidarVisualizer : MonoBehaviour
{
    [Tooltip("Material used to render the scanned points.")]
    public Material pointMaterial;
    public float pointRadius = 0.01f;
    private Lidar lidar;
    private GameObject visualizer;
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    // Will be used to store the points measured by the LIDAR
    private Mesh lidarPoints;
    List<Vector3> vertices;
    List<Vector3> normals;
    List<Vector2> uvs;
    List<Color> colors;
    List<int> triangles;

    [Header("ROS settings")]
    private ROSConnection _ros;
    [SerializeField]
    private string topicName = "lidar";

    void Start()
    {
        // Create sibling GameObject to display the lidar measurements
        visualizer = new GameObject("LIDAR Visualizer");
        visualizer.transform.parent = transform.parent;
        visualizer.transform.position = transform.position;
        meshFilter = visualizer.AddComponent<MeshFilter>();
        meshRenderer = visualizer.AddComponent<MeshRenderer>();
        meshRenderer.material = pointMaterial;

        lidar = GetComponent<Lidar>();
        _ros = ROSConnection.GetOrCreateInstance();
        _ros.Subscribe<RosMessageTypes.Sensor.LaserScanMsg>(topicName, BuildScanMesh);
    }

    void BuildScanMesh(RosMessageTypes.Sensor.LaserScanMsg message)
    {
        ResetMesh(message.ranges.Length);

        var ranges = message.ranges;

        for (uint i = 0; i < ranges.Length; i++)
        {
            if (ranges[i] < Mathf.Infinity)
            {
                // TODO: should not depend on lidar, but should use the `frame` field of the message to find the corresponding orientation
                Vector3 point = Quaternion.AngleAxis(-lidar.ComputeScanAngle(i) * Mathf.Rad2Deg, Vector3.up) * (Vector3.forward * ranges[i]);
                Vector3 normal = Quaternion.AngleAxis(-lidar.ComputeScanAngle(i) * Mathf.Rad2Deg, Vector3.up) * Vector3.back;
                AddQuadToMesh(point, normal, pointRadius);
            }            
        }

        UpdateMesh();
    }

    private void ResetMesh(int numSamples)
    {
        lidarPoints = new Mesh();
        vertices = new List<Vector3>(4 * numSamples);
        normals = new List<Vector3>(4 * vertices.Capacity);
        uvs = new List<Vector2>(4 * vertices.Capacity);
        triangles = new List<int>(6 * vertices.Capacity);
        colors = new List<Color>(4 * vertices.Capacity);
    }

    void UpdateMesh()
    {
        lidarPoints.SetVertices(vertices);
        lidarPoints.SetNormals(normals);
        lidarPoints.SetUVs(0, uvs);
        lidarPoints.SetTriangles(triangles, 0);
        lidarPoints.SetColors(colors);
        meshFilter.mesh = lidarPoints;
    }

    private void AddQuadToMesh(Vector3 position, Vector3 normal, float radius)
	{
        // Vertex indices
        int v0 = vertices.Count;
        int v1 = v0 + 1;
        int v2 = v0 + 2;
        int v3 = v0 + 3;

        // Vertices
        Vector3 offsetV0 = new Vector3 (-radius, -radius, 0);
        Vector3 offsetV1 = new Vector3 (-radius, radius, 0);
        Vector3 offsetV2 = new Vector3 (radius, -radius, 0);
        Vector3 offsetV3 = new Vector3 (radius, radius, 0);

        Quaternion normalRotation = Quaternion.FromToRotation(Vector3.back, normal);
        offsetV0 = normalRotation * offsetV0;
        offsetV1 = normalRotation * offsetV1;
        offsetV2 = normalRotation * offsetV2;
        offsetV3 = normalRotation * offsetV3;

        vertices.Add(position + offsetV0);
        vertices.Add(position + offsetV1);
        vertices.Add(position + offsetV2);
        vertices.Add(position + offsetV3);

        // Normals
        normals.Add(normal);
        normals.Add(normal);
        normals.Add(normal);
        normals.Add(normal);

        // UVs
        uvs.Add(new Vector2 (0, 0));
        uvs.Add(new Vector2 (0, 1));
        uvs.Add(new Vector2 (1, 0));
        uvs.Add(new Vector2 (1, 1));

        // Triangles
        triangles.Add(v0);
        triangles.Add(v1);
        triangles.Add(v2);
        triangles.Add(v3);
        triangles.Add(v2);
        triangles.Add(v1);

        // Colors
        // TODO: color could be related to intensity (e.g. 0->black, 1->white)
        colors.Add(Color.black);
        colors.Add(Color.black);
        colors.Add(Color.black);
        colors.Add(Color.black);
	}
}
