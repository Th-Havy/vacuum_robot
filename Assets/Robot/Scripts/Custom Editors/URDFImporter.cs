// using UnityEngine;
// using UnityEditor;
// using UnityEditor.AssetImporters;
// using System.Linq;
// using System.Xml;
// using System.Xml.Linq;

// [ScriptedImporter(1, "urdf")]
// public class URDFImporter : ScriptedImporter
// {
//     /// <summary>
//     /// RosSharp.ImportSettings is not marked as serializable, so we need to make a serializable version of it
//     /// </summary>
//     [System.Serializable]
//     public class SerializableImportSettings
//     {
//         public RosSharp.ImportSettings.axisType choosenAxis;
//         public RosSharp.ImportSettings.convexDecomposer convexMethod;

//         public static implicit operator SerializableImportSettings(RosSharp.ImportSettings rhs)
//         {
//             return new SerializableImportSettings() {choosenAxis = rhs.choosenAxis, convexMethod = rhs.convexMethod};
//         }

//         public static implicit operator RosSharp.ImportSettings(SerializableImportSettings rhs)
//         {
//             return new RosSharp.ImportSettings() {choosenAxis = rhs.choosenAxis, convexMethod = rhs.convexMethod};
//         }
//     }

//     [SerializeField]
//     private string _xmlData = "";
//     [SerializeField]
//     private string _filepath = "";
//     [SerializeField]
//     private Material[] _remappedMaterials;
//     [SerializeField]
//     private SerializableImportSettings _settings = RosSharp.ImportSettings.DefaultSettings();

//     // Parsed URDF data
//     [SerializeField]
//     private string _robotName = "";
//     [SerializeField]
//     private string[] _materials;

//     /// <summary>
//     /// This function is executed upon importing a .urdf file
//     /// </summary>
//     public override void OnImportAsset(AssetImportContext ctx)
//     {
//         _xmlData = System.IO.File.ReadAllText(ctx.assetPath);
//         _filepath = ctx.assetPath;

//         ParseXML(_filepath);
//         CreateURDFGameObjectAsset(ctx);
//     }

//     /// <summary>
//     /// Extract relevant information from the urdf file (e.g. name of robot, list of materials)
//     /// </summary>
//     private void ParseXML(string filename)
//     {
//         XDocument xdoc = XDocument.Load(filename);
//         XElement robotNode = xdoc.Element("robot");
//         _robotName = robotNode.Attribute("name").Value;

//         // Find materials
//         _materials = (from n in robotNode.Elements("material").Attributes("name") select (string)n).ToArray();

//         // Update remap materials if needed
//         if (_remappedMaterials.Length != _materials.Length)
//         {
//             Material[] newRemap = new Material[_materials.Length];

//             for (int i = 0; i < newRemap.Length && i < _remappedMaterials.Length; i++)
//             {
//                 newRemap[i] = _remappedMaterials[i];
//             }

//             _remappedMaterials = newRemap;
//         }
//     }

//     /// <summary>
//     /// Use the URDF importer to create the asset associated to the .urdf file.
//     /// </summary>
//     private void CreateURDFGameObjectAsset(AssetImportContext ctx)
//     {
//         var enumerator = RosSharp.Urdf.Editor.UrdfRobotExtensions.Create(RosSharp.Urdf.Editor.UrdfAssetPathHandler.GetFullAssetPath(_filepath), _settings);
//         enumerator.MoveNext();
//         GameObject robot = enumerator.Current;
        
//         RemapMaterials(robot);
        
//         RegisterCustomMeshCollidersInAsset(ctx, robot);
//         ctx.AddObjectToAsset("robot", robot);
//         ctx.SetMainObject(robot);
//     }

//     /// <summary>
//     /// Remap the materials of the imported URDF.
//     /// </summary>
//     private void RemapMaterials(GameObject robot)
//     {
//         if (robot.name != _robotName)
//         {
//             Debug.LogWarning("Materials could not be remapped. The provided object for material remapping does not seem to be the imported URDF robot.");
//         }

//         MeshRenderer[] renderers = robot.GetComponentsInChildren<MeshRenderer>(true);
        
//         foreach (var rend in renderers)
//         {
//             int index = System.Array.IndexOf(_materials, rend.sharedMaterial.name);
            
//             if (index != -1)
//             {
//                 rend.sharedMaterial = _remappedMaterials[index];
//             }
//         }
//     }

//     /// <summary>
//     /// Generated meshes (e.g. URDF cylinder collider) are not serialized when the object is turned into a prefab and must thus be
//     /// registered separately in the importer.
//     /// </summary>
//     private void RegisterCustomMeshCollidersInAsset(AssetImportContext ctx, GameObject robot)
//     {
//         MeshCollider[] meshColliders = robot.GetComponentsInChildren<MeshCollider>(true);

//         int i =0;
//         foreach (var col in meshColliders)
//         {
//             if (IsCustomMesh(col.sharedMesh))
//             {
//                 col.sharedMesh.name = "mesh" + i.ToString();
//                 ctx.AddObjectToAsset(col.sharedMesh.name, col.sharedMesh);
//                 i++;
//             }
//         }
//     }

//     private bool IsCustomMesh(Mesh mesh)
//     {
//         string assetPath = AssetDatabase.GetAssetPath(mesh.GetInstanceID());
//         return (assetPath == null || assetPath == "");
//     }
// }
