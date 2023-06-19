using UnityEngine;
using Unity.Entities;

namespace CellexalVR
{
    /// <summary>
    /// Struct for holding prefab entities as components (updated for Entities 1.0.10)
    /// </summary>
    public struct PrefabEntitiesComponent : IComponentData
    {
        public Entity prefabEntity;

    }

    /// <summary>
    /// Spawns entities, in our case points, and converts to an entity.
    /// </summary>
    public class PrefabEntities : MonoBehaviour//, IConvertGameObjectToEntity
    {
        public static Entity prefabEntity;
        public GameObject prefab;

        /*public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            using (BlobAssetStore blobAssetStore = new BlobAssetStore())
            {
                Entity prefabEntity = GameObjectConversionUtility.ConvertGameObjectHierarchy(prefab,
                    GameObjectConversionSettings.FromWorld(dstManager.World, blobAssetStore));
                PrefabEntities.prefabEntity = prefabEntity;
            }
        }*/
    }

    public class PrefabEntitiesBaker : Baker<PrefabEntities>
    {
        public override void Bake(PrefabEntities authoring)
        {
            AddComponent(new PrefabEntitiesComponent { prefabEntity = GetEntity(authoring.prefab) });
            //throw new System.NotImplementedException();
        }
    }
}