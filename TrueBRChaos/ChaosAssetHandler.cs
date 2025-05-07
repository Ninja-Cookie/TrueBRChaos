using Reptile;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;

namespace TrueBRChaos
{
    internal static class ChaosAssetHandler
    {
        private     static Dictionary<string, Bundle> _assetBundle  = new Dictionary<string, Bundle>();
        internal    static Dictionary<string, Bundle> AssetBundle   => _assetBundle;

        private static Dictionary<GameObject, Core.OnUpdateHandler> activeJunk = new Dictionary<GameObject, Core.OnUpdateHandler>();
        private static Dictionary<int, UnityEngine.Object> LoadedAssets = new Dictionary<int, UnityEngine.Object>();

        /// <summary>
        /// Loads Asset Bundle.
        /// </summary>
        /// <exception cref="System.NullReferenceException"/>
        internal static void LoadAssetBundle()
        {
            if (_assetBundle == null || _assetBundle.Count == 0)
            {
                try
                {
                    _assetBundle = Commons.Assets.GetValue<Dictionary<string, Bundle>>("availableBundles");
                }
                catch (System.Exception ex)
                {
                    throw new System.NullReferenceException("Asset has not been created by Core.", ex);
                }
                finally
                {
                    if (_assetBundle == null)
                        _assetBundle = new Dictionary<string, Bundle>();
                }
            }
        }

        internal struct BundleInfo
        {
            internal string AssetBundle { get; private set; }
            internal string AssetName   { get; private set; }
            internal int    Index       { get; private set; }

            internal BundleInfo(string assetBundle, string assetName, int index = 0)
            {
                this.AssetBundle    = assetBundle;
                this.AssetName      = assetName;
                this.Index          = index;
            }
        }

        internal static bool TryGetGameAsset<T>(BundleInfo bundleInfo, out T value, bool strict = false) where T : UnityEngine.Object
        {
            int hash = (typeof(T), bundleInfo).GetHashCode();
            if (LoadedAssets.TryGetValue(hash, out Object asset))
            {
                if (asset != null)
                {
                    value = asset as T;
                    return true;
                }
                LoadedAssets.Remove(hash);
            }

            T obj = GetGameAsset<T>(bundleInfo, strict);
            if (obj == null)
            {
                value = default;
                return false;
            }

            LoadedAssets.Add(hash, obj);
            value = obj;
            return true;
        }

        private static T GetGameAsset<T>(BundleInfo bundleInfo, bool strict = false) where T : UnityEngine.Object
        {
            if (AssetBundle.TryGetValue(bundleInfo.AssetBundle, out Bundle value) && value?.AssetBundle != null && value.AssetBundle.Contains(bundleInfo.AssetName))
            {
                try
                {
                    if (strict)
                    {
                        T strictObj = value.AssetBundle.LoadAssetWithSubAssets<T>(bundleInfo.AssetName).Where(x => x.name == bundleInfo.AssetName).ToArray()[bundleInfo.Index];
                        return strictObj;
                    }

                    T obj = value.AssetBundle.LoadAsset<T>(bundleInfo.AssetName);
                    return obj;
                }
                catch
                {
                    return null;
                }
            }
            return null;
        }

        internal static GameObject CreateGameObject(BundleInfo objectBundle, Vector3 position, Quaternion rotation, BundleInfo? materialBundle = null, BundleInfo? customMeshBundle = null, bool strict = false)
        {
            GameObject obj = GetGameObject(objectBundle, materialBundle, customMeshBundle, strict);
            return obj != null ? GameObject.Instantiate<GameObject>(obj, position, rotation) : null;
        }

        internal static GameObject CreateGameObject(BundleInfo objectBundle, BundleInfo? materialBundle = null, BundleInfo? customMeshBundle = null, bool strict = false)
        {
            GameObject obj = GetGameObject(objectBundle, materialBundle, customMeshBundle, strict);
            return obj != null ? GameObject.Instantiate<GameObject>(obj, null, true) : null;
        }

        internal static GameObject GetGameObject(BundleInfo objectBundle, BundleInfo? materialBundle = null, BundleInfo? customMeshBundle = null, bool strict = false)
        {
            if (!TryGetGameAsset<GameObject>(objectBundle, out GameObject gameObject, strict))
                return null;

            BundleInfo meshInfo = customMeshBundle ?? objectBundle;
            if (TryGetGameAsset<Mesh>(meshInfo, out Mesh mesh, strict))
                gameObject.AddComponentIfMissing<MeshCollider>().sharedMesh = mesh;

            if (materialBundle != null && gameObject.TryGetComponent<MeshRenderer>(out MeshRenderer renderer) && TryGetGameAsset<Material>((BundleInfo)materialBundle, out Material material, strict))
            {
                renderer.material           = material;
                renderer.sharedMaterial     = material;

                Material[] matArray         = new Material[1] { material };
                renderer.materials          = matArray;
                renderer.sharedMaterials    = matArray;
            }

            return gameObject;
        }

        internal static GameObject CreateRigidbodyObject(BundleInfo objectBundle, BundleInfo? materialBundle, Vector3 position, Quaternion rotation)
        {
            GameObject gameObject = CreateGameObject(objectBundle, position, rotation, materialBundle, null);
            if (gameObject == null)
                return null;

            gameObject.layer = 21;

            if (gameObject.TryGetComponent<MeshCollider>(out MeshCollider collider))
                GameObject.Destroy(collider);

            gameObject.AddComponentsIfMissing(typeof(BoxCollider), typeof(Rigidbody), typeof(LODGroup));

            Junk junk           = gameObject.AddComponentIfMissing<Junk>();
            junk.destroyTimer   = 0f;
            junk.interactOn     = Junk.Interact.ON_AWAKE;

            JunkBehaviour junkBehaviour = new JunkBehaviour();
            Core.OnUpdateHandler junkUpdate = null;
            junkUpdate = delegate
            {
                if (junk != null)
                {
                    junkBehaviour.InvokeMethod("JunkUpdate", junkBehaviour, Time.deltaTime);
                }
                else
                {
                    Core.OnUpdate -= junkUpdate;
                    activeJunk.Remove(gameObject);
                }
            };

            activeJunk.Add(gameObject, junkUpdate);

            junkBehaviour.InvokeMethod("Init", new object[1] { new Junk[1] { junk } });
            Core.OnUpdate += junkUpdate;

            return gameObject;
        }

        internal static void RemoveAllRigidbodyObjects()
        {
            RemoveRigidbodyObjects(activeJunk.Keys.ToArray());
        }

        internal static void RemoveRigidbodyObjects(params GameObject[] junkToRemove)
        {
            foreach (var junk in junkToRemove)
            {
                if (junk == null)
                    continue;

                if (activeJunk.TryGetValue(junk, out var value))
                {
                    Core.OnUpdate -= value;
                    activeJunk.Remove(junk);
                }

                GameObject.Destroy(junk);
            }
        }

        internal static void DebugGetGameObjects()
        {
            GameObject[] gameObjects = Physics.OverlapSphere(Commons.Player.transform.position, 0.5f).Select(collider => collider.gameObject).ToArray();

            foreach (GameObject gameObject in gameObjects)
            {
                if (gameObject == null)
                    continue;

                string assetName = string.Empty;
                foreach (var value in AssetBundle.Keys)
                {
                    try
                    {
                        string objectName = gameObject.name.Contains(' ') ? gameObject.name.Substring(0, gameObject.name.IndexOf(' ')) : gameObject.name;
                        if (TryGetGameAsset<UnityEngine.Object>(new BundleInfo(value, objectName), out _))
                        {
                            assetName = value;
                            break;
                        }
                    }
                    catch
                    {
                        continue;
                    }
                }
                
                Debug.LogError($"[{assetName}] {gameObject}:");
                if (gameObject.TryGetComponent<Renderer>(out Renderer renderer))
                {
                    Debug.LogWarning($"\tMaterial: {renderer.material}");
                    Debug.LogWarning($"\tMaterial: {renderer.material.mainTextureOffset}");
                    Debug.LogWarning($"\tMaterial: {renderer.material.mainTextureScale}");
                }
                if (gameObject.TryGetComponent<MeshCollider>(out MeshCollider collider))
                    Debug.LogWarning($"\tMesh: {collider.sharedMesh}");

                SkinnedMeshRenderer SMR = gameObject.GetComponentInChildren<SkinnedMeshRenderer>();
                if (SMR != null)
                {
                    foreach (var mat in SMR.materials)
                    Debug.LogWarning($"\tMaterial:          {mat}");
                    Debug.LogWarning($"\tCurrent Material:  {SMR.material}");
                    Debug.LogWarning($"\tMaterial Mesh:     {SMR.sharedMesh}");
                }

                foreach (var comp in gameObject.GetComponentsInChildren<Component>())
                    Debug.Log($"\tComponent: {comp}");
                Debug.Log("\n\n");
            }
        }
    }
}
