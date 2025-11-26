using System;
using System.IO;
using FullSerializer;
using UnityEngine;

namespace NBC.ActionEditor
{
    public class Json
    {
        private static fsSerializer CreateSerializer()
        {
            var serializer = new fsSerializer();
            serializer.Config.SerializeAttributes = new[] { typeof(SerializeField), typeof(fsPropertyAttribute) };
            
            serializer.AddConverter(new fsInterfaceConverter());
            
            return serializer;
        }

        public static string Serialize(object value, bool isCompressed = false)
        {
            try
            {
                CreateSerializer().TrySerialize(value, out var data).AssertSuccessWithoutWarnings();
                if (isCompressed)
                {
                    return fsJsonPrinter.CompressedJson(data);
                }

                return fsJsonPrinter.PrettyJson(data);
            }
            catch (Exception e)
            {
                Debug.LogError($"Serialization error: {e}");
                return string.Empty;
            }
        }

        public static object Deserialize(Type type, string serializedState)
        {
            try
            {
                fsData data = fsJsonParser.Parse(serializedState);
                object deserialized = null;
                var ser = CreateSerializer();
                ser.TryDeserialize(data, type, ref deserialized).AssertSuccessWithoutWarnings();

                return deserialized;
            }
            catch (Exception e)
            {
#if UNITY_EDITOR
                File.WriteAllText($"{Application.dataPath}/../error_json.json", serializedState);
#endif
                Debug.LogError($"Deserialization error for type {type.Name}: {e}");
                return null;
            }
        }
    }
}