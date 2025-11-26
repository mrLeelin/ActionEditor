using System;
using System.Collections.Generic;
using FullSerializer;
using UnityEngine;

namespace NBC.ActionEditor
{
    public class fsInterfaceConverter : fsConverter
    {
        public override bool CanProcess(Type type)
        {
            return type.IsInterface || type.IsAbstract;
        }

        public override fsResult TrySerialize(object instance, out fsData serialized, Type storageType)
        {
            serialized = fsData.CreateDictionary();
            var result = fsResult.Success;

            if (instance == null)
            {
                return result;
            }

            var actualType = instance.GetType();
            
            var dict = serialized.AsDictionary;
            dict["$type"] = new fsData(actualType.AssemblyQualifiedName);

            fsData objectData;
            result += Serializer.TrySerialize(actualType, instance, out objectData);
            dict["$data"] = objectData;

            return result;
        }

        public override fsResult TryDeserialize(fsData data, ref object instance, Type storageType)
        {
            var result = fsResult.Success;

            if (data.IsNull)
            {
                instance = null;
                return result;
            }

            if (!data.IsDictionary)
            {
                return fsResult.Fail("Expected dictionary for interface deserialization");
            }

            var dict = data.AsDictionary;
            
            if (!dict.ContainsKey("$type"))
            {
                return fsResult.Fail("Missing $type key in interface serialization");
            }

            string typeString = dict["$type"].AsString;
            Type actualType = Type.GetType(typeString);

            if (actualType == null)
            {
                return fsResult.Fail($"Cannot find type {typeString}");
            }

            if (!dict.ContainsKey("$data"))
            {
                return fsResult.Fail("Missing $data key in interface serialization");
            }

            fsData objectData = dict["$data"];
            result += Serializer.TryDeserialize(objectData, actualType, ref instance);

            return result;
        }

        public override object CreateInstance(fsData data, Type storageType)
        {
            return null;
        }
    }
}
