using Ellie.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CarGame.Saving
{
    public class JsonResolver : DefaultSerializationBinder
    {
        private static JsonSerializerSettings Settings => new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto,
            SerializationBinder = new JsonResolver(),

            Converters = new List<JsonConverter>
            {
                new Vector3Converter(),
                new QuaternionConverter(),
                new ColorConverter(),
            },

            Formatting = Formatting.Indented
        };


        private static readonly Dictionary<string, Type> allowedTypes = new()
        {
            { "PlayerData", typeof(PlayerSaveData) },
            { "WorldData", typeof(WorldSaveData) },
            /*
            // Inventory
            { "ConsumableItem", typeof(ConsumeableItemData) },*/
        };

        private static readonly Dictionary<Type, string> typeNames = 
            allowedTypes.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);

        public override Type BindToType(string assemblyName, string typeName)
        {
            if (allowedTypes.TryGetValue(typeName, out Type type)) 
            { 
                return type;
            }

            throw new InvalidOperationException($"Type {typeName} is not allowed");
        }

        public override void BindToName(Type serializedType, out string assemblyName, out string typeName)
        {
            assemblyName = null;

            if (typeNames.TryGetValue(serializedType, out string name))
            {
                typeName = name;
            }
            else
            {
                throw new InvalidOperationException($"Type {serializedType.Name} is not registered");
            }
        }
    }
}
