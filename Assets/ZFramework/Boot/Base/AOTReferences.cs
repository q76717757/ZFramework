using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;
using System.Runtime.Serialization;
using System;

namespace ZFramework
{
    [Preserve]
    public class AOTReferences : MonoBehaviour
    {
        Action<object> a;
        //System.Action`1<TouchSocket.Rpc.TouchRpc.ChannelData>
        //System.Action`1<System.Byte>
        //System.Action`2<System.Object,System.Object>
        //System.Action`3<System.Object,System.Int16,System.Object>
        //System.Action`3<System.Object,System.Object,System.Object>
        //System.Action`3<System.Object,System.Byte,System.Object>
        //System.Action`4<TouchSocket.Core.Log.LogType,System.Object,System.Object,System.Object>
        //System.Action`4<System.Object,System.Int32,System.Int32,System.Byte>
        //System.Action`5<System.Object,System.Object,System.Int32,System.Int32,System.Byte>
        //System.ArraySegment`1<System.Byte>
        //System.Collections.Concurrent.ConcurrentDictionary`2<System.Object,System.Object>
        //System.Collections.Concurrent.ConcurrentDictionary`2<System.Int64,System.Object>
        //System.Collections.Concurrent.ConcurrentDictionary`2<System.Int32,System.Object>
        //System.Collections.Concurrent.ConcurrentQueue`1<TouchSocket.Sockets.UdpFrame>
        //System.Collections.Concurrent.ConcurrentQueue`1<TouchSocket.Rpc.TouchRpc.ChannelData>
        //System.Collections.Concurrent.ConcurrentQueue`1<System.Object>
        //System.Collections.Generic.Comparer`1<System.Int32>
        //System.Collections.Generic.Comparer`1<System.Object>
        //System.Collections.Generic.Dictionary`2<System.Object,TouchSocket.Core.XREF.Newtonsoft.Json.Schema.JsonSchemaType>
        //System.Collections.Generic.Dictionary`2<System.Object,System.Byte>
        //System.Collections.Generic.Dictionary`2<System.UInt16,System.Object>
        //System.Collections.Generic.Dictionary`2<System.Int32,System.Object>
        //System.Collections.Generic.Dictionary`2<System.Int64,System.Object>
        //System.Collections.Generic.Dictionary`2<System.Object,TouchSocket.Core.IO.VAction>
        //System.Collections.Generic.Dictionary`2<ZFramework.CycleType,System.Object>
        //System.Collections.Generic.Dictionary`2<TouchSocket.Core.XREF.Newtonsoft.Json.Serialization.ResolverContractKey,System.Object>
        //System.Collections.Generic.Dictionary`2<System.Object,ZFramework.ArrayMetadata>
        //System.Collections.Generic.Dictionary`2<TouchSocket.Core.XREF.Newtonsoft.Json.Utilities.TypeNameKey,System.Object>
        //System.Collections.Generic.Dictionary`2<TouchSocket.Core.XREF.Newtonsoft.Json.Utilities.ConvertUtils/TypeConvertKey,System.Object>
        //System.Collections.Generic.Dictionary`2<System.Object,System.Object>
        //System.Collections.Generic.Dictionary`2<System.Object,ZFramework.ObjectMetadata>
        //System.Collections.Generic.Dictionary`2<System.Object,ZFramework.PropertyMetadata>
        //System.Collections.Generic.Dictionary`2<System.Object,TouchSocket.Core.XREF.Newtonsoft.Json.Serialization.JsonSerializerInternalReader/PropertyPresence>
        //System.Collections.Generic.Dictionary`2<System.Object,TouchSocket.Core.XREF.Newtonsoft.Json.Utilities.PrimitiveTypeCode>
        //System.Collections.Generic.Dictionary`2<System.Object,TouchSocket.Core.XREF.Newtonsoft.Json.ReadType>
        //System.Collections.Generic.Dictionary`2<ZFramework.ChannelType,System.Object>
        //System.Collections.Generic.Dictionary`2/Enumerator<System.Object,System.Object>
        //System.Collections.Generic.Dictionary`2/Enumerator<System.Int64,System.Object>
        //System.Collections.Generic.Dictionary`2/Enumerator<System.Object,TouchSocket.Core.IO.VAction>
        //System.Collections.Generic.Dictionary`2/Enumerator<System.Object,TouchSocket.Core.XREF.Newtonsoft.Json.Serialization.JsonSerializerInternalReader/PropertyPresence>
        //System.Collections.Generic.Dictionary`2/Enumerator<ZFramework.ChannelType,System.Object>
        //System.Collections.Generic.Dictionary`2/Enumerator<System.Int32,System.Object>
        //System.Collections.Generic.Dictionary`2/KeyCollection<System.Object,System.Object>
        //System.Collections.Generic.Dictionary`2/KeyCollection/Enumerator<System.Object,System.Object>
        //System.Collections.Generic.Dictionary`2/ValueCollection<System.Int64,System.Object>
        //System.Collections.Generic.Dictionary`2/ValueCollection<System.Object,System.Object>
        //System.Collections.Generic.Dictionary`2/ValueCollection/Enumerator<System.Int64,System.Object>
        //System.Collections.Generic.Dictionary`2/ValueCollection/Enumerator<System.Object,System.Object>
        //System.Collections.Generic.EqualityComparer`1<System.Int32>
        //System.Collections.Generic.EqualityComparer`1<System.Object>
        //System.Collections.Generic.HashSet`1<System.Object>
        //System.Collections.Generic.HashSet`1/Enumerator<System.Object>
        //System.Collections.Generic.ICollection`1<System.Int32>
        //System.Collections.Generic.ICollection`1<System.Object>
        //System.Collections.Generic.ICollection`1<System.UInt16>
        //System.Collections.Generic.ICollection`1<TouchSocket.Core.XREF.Newtonsoft.Json.Schema.JsonSchemaType>
        //System.Collections.Generic.ICollection`1<ZFramework.PropertyMetadata>
        //System.Collections.Generic.ICollection`1<System.Collections.Generic.KeyValuePair`2<System.Object,System.Object>>
        //System.Collections.Generic.IComparer`1<System.Object>
        //System.Collections.Generic.IComparer`1<System.Int32>
        //System.Collections.Generic.IDictionary`2<System.Object,TouchSocket.Core.XREF.Newtonsoft.Json.Schema.JsonSchemaType>
        //System.Collections.Generic.IDictionary`2<System.Object,ZFramework.PropertyMetadata>
        //System.Collections.Generic.IDictionary`2<System.Int32,System.Object>
        //System.Collections.Generic.IDictionary`2<System.Object,ZFramework.ObjectMetadata>
        //System.Collections.Generic.IDictionary`2<System.Object,System.Object>
        //System.Collections.Generic.IDictionary`2<System.Object,ZFramework.ArrayMetadata>
        //System.Collections.Generic.IEnumerable`1<System.Collections.Generic.KeyValuePair`2<System.Object,TouchSocket.Core.XREF.Newtonsoft.Json.Schema.JsonSchemaType>>
        //System.Collections.Generic.IEnumerable`1<System.Object>
        //System.Collections.Generic.IEnumerable`1<System.Collections.Generic.KeyValuePair`2<System.Object,System.Byte>>
        //System.Collections.Generic.IEnumerable`1<TouchSocket.Core.XREF.Newtonsoft.Json.Utilities.LinqBridge.Tuple`2<System.Object,System.Int32>>
        //System.Collections.Generic.IEnumerable`1<System.UInt64>
        //System.Collections.Generic.IEnumerable`1<System.UInt16>
        //System.Collections.Generic.IEnumerable`1<System.ArraySegment`1<System.Byte>>
        //System.Collections.Generic.IEnumerable`1<TouchSocket.Core.XREF.Newtonsoft.Json.Schema.JsonSchemaType>
        //System.Collections.Generic.IEnumerable`1<System.Byte>
        //System.Collections.Generic.IEnumerable`1<System.Nullable`1<System.Int32>>
        //System.Collections.Generic.IEnumerable`1<ZFramework.PropertyMetadata>
        //System.Collections.Generic.IEnumerable`1<System.Collections.Generic.KeyValuePair`2<System.Object,System.Object>>
        //System.Collections.Generic.IEnumerable`1<System.Nullable`1<System.Decimal>>
        //System.Collections.Generic.IEnumerable`1<System.Decimal>
        //System.Collections.Generic.IEnumerable`1<System.Nullable`1<System.Double>>
        //System.Collections.Generic.IEnumerable`1<System.Int32>
        //System.Collections.Generic.IEnumerable`1<System.Double>
        //System.Collections.Generic.IEnumerable`1<System.Single>
        //System.Collections.Generic.IEnumerable`1<System.Nullable`1<System.Int64>>
        //System.Collections.Generic.IEnumerable`1<System.Int64>
        //System.Collections.Generic.IEnumerable`1<System.Nullable`1<System.Single>>
        //System.Collections.Generic.IEnumerator`1<System.Nullable`1<System.Single>>
        //System.Collections.Generic.IEnumerator`1<TouchSocket.Core.XREF.Newtonsoft.Json.Schema.JsonSchemaType>
        //System.Collections.Generic.IEnumerator`1<System.Single>
        //System.Collections.Generic.IEnumerator`1<System.UInt16>
        //System.Collections.Generic.IEnumerator`1<System.Double>
        //System.Collections.Generic.IEnumerator`1<System.Nullable`1<System.Double>>
        //System.Collections.Generic.IEnumerator`1<System.Decimal>
        //System.Collections.Generic.IEnumerator`1<System.Int64>
        //System.Collections.Generic.IEnumerator`1<System.Nullable`1<System.Decimal>>
        //System.Collections.Generic.IEnumerator`1<System.Nullable`1<System.Int32>>
        //System.Collections.Generic.IEnumerator`1<System.Nullable`1<System.Int64>>
        //System.Collections.Generic.IEnumerator`1<TouchSocket.Core.XREF.Newtonsoft.Json.Utilities.LinqBridge.Tuple`2<System.Object,System.Int32>>
        //System.Collections.Generic.IEnumerator`1<System.Collections.Generic.KeyValuePair`2<System.Object,TouchSocket.Core.XREF.Newtonsoft.Json.Schema.JsonSchemaType>>
        //System.Collections.Generic.IEnumerator`1<System.ArraySegment`1<System.Byte>>
        //System.Collections.Generic.IEnumerator`1<System.Object>
        //System.Collections.Generic.IEnumerator`1<System.Byte>
        //System.Collections.Generic.IEnumerator`1<System.UInt64>
        //System.Collections.Generic.IEnumerator`1<System.Collections.Generic.KeyValuePair`2<System.Object,System.Byte>>
        //System.Collections.Generic.IEnumerator`1<System.Int32>
        //System.Collections.Generic.IEnumerator`1<ZFramework.PropertyMetadata>
        //System.Collections.Generic.IEnumerator`1<System.Collections.Generic.KeyValuePair`2<System.Object,System.Object>>
        //System.Collections.Generic.IEqualityComparer`1<System.Object>
        //System.Collections.Generic.IList`1<System.Object>
        //System.Collections.Generic.IList`1<System.Collections.Generic.KeyValuePair`2<System.Object,System.Object>>
        //System.Collections.Generic.KeyValuePair`2<System.Object,TouchSocket.Core.IO.VAction>
        //System.Collections.Generic.KeyValuePair`2<System.Object,System.Byte>
        //System.Collections.Generic.KeyValuePair`2<System.Object,System.Object>
        //System.Collections.Generic.KeyValuePair`2<System.Int64,System.Object>
        //System.Collections.Generic.KeyValuePair`2<ZFramework.ChannelType,System.Object>
        //System.Collections.Generic.KeyValuePair`2<System.Object,TouchSocket.Core.XREF.Newtonsoft.Json.Serialization.JsonSerializerInternalReader/PropertyPresence>
        //System.Collections.Generic.KeyValuePair`2<System.Int32,System.Object>
        //System.Collections.Generic.KeyValuePair`2<System.Object,TouchSocket.Core.XREF.Newtonsoft.Json.Schema.JsonSchemaType>
        //System.Collections.Generic.List`1<System.Collections.Generic.KeyValuePair`2<System.Object,System.Object>>
        //System.Collections.Generic.List`1<ZFramework.PropertyMetadata>
        //System.Collections.Generic.List`1<System.Object>
        //System.Collections.Generic.List`1<System.Int32>
        //System.Collections.Generic.List`1<System.UInt16>
        //System.Collections.Generic.List`1<System.Byte>
        //System.Collections.Generic.List`1<TouchSocket.Core.XREF.Newtonsoft.Json.Utilities.LinqBridge.Tuple`2<System.Object,System.Int32>>
        //System.Collections.Generic.List`1<UnityEngine.Color>
        //System.Collections.Generic.List`1<System.Int64>
        //System.Collections.Generic.List`1<UnityEngine.EventSystems.RaycastResult>
        //System.Collections.Generic.List`1<TouchSocket.Core.XREF.Newtonsoft.Json.JsonPosition>
        //System.Collections.Generic.List`1<TouchSocket.Core.XREF.Newtonsoft.Json.Schema.JsonSchemaType>
        //System.Collections.Generic.List`1<ZFramework.TimerEventType>
        //System.Collections.Generic.List`1/Enumerator<System.Int32>
        //System.Collections.Generic.List`1/Enumerator<System.Collections.Generic.KeyValuePair`2<System.Object,System.Object>>
        //System.Collections.Generic.List`1/Enumerator<TouchSocket.Core.XREF.Newtonsoft.Json.JsonPosition>
        //System.Collections.Generic.List`1/Enumerator<UnityEngine.Color>
        //System.Collections.Generic.List`1/Enumerator<System.Object>
        //System.Collections.Generic.Queue`1<System.Object>
        //System.Collections.Generic.Queue`1/Enumerator<System.Object>
        //System.Collections.Generic.Stack`1<System.Object>
        //System.Collections.Generic.Stack`1<System.Int32>
        //System.Collections.Generic.Stack`1/Enumerator<System.Object>
        //System.Collections.ObjectModel.Collection`1<System.Object>
        //System.Collections.ObjectModel.KeyedCollection`2<System.Object,System.Object>
        //System.Collections.ObjectModel.ReadOnlyCollection`1<System.Object>
        //System.Comparison`1<System.Object>
        //System.Comparison`1<TouchSocket.Core.XREF.Newtonsoft.Json.Utilities.LinqBridge.Tuple`2<System.Object,System.Int32>>
        //System.EventHandler`1<System.Object>
        //System.Func`1<System.Byte>
        //System.Func`1<System.TimeSpan>
        //System.Func`1<TouchSocket.Core.Result>
        //System.Func`1<System.Object>
        //System.Func`1<TouchSocket.Sockets.ResponsedData>
        //System.Func`2<System.Int64,System.Object>
        //System.Func`2<System.Object,System.Int32>
        //System.Func`2<System.Object,System.Object>
        //System.Func`2<System.Object,System.Byte>
        //System.Func`2<TouchSocket.Core.IO.VAction,System.Int32>
        //System.Func`2<System.Collections.Generic.KeyValuePair`2<System.Object,System.Object>,System.Byte>
        //System.Func`3<System.Object,System.Object,System.Byte>
        //System.Func`3<System.Object,System.Object,System.Object>
        //System.Func`4<System.Object,System.Int32,System.Object,System.Byte>
        //System.Func`5<System.Object,System.Object,System.Object,System.Object,System.Object>
        //System.IComparable`1<System.Object>
        //System.IEquatable`1<System.Object>
        //System.IEquatable`1<TouchSocket.Core.XREF.Newtonsoft.Json.Utilities.ConvertUtils/TypeConvertKey>
        //System.IEquatable`1<TouchSocket.Core.XREF.Newtonsoft.Json.Utilities.TypeNameKey>
        //System.IEquatable`1<TouchSocket.Core.XREF.Newtonsoft.Json.Serialization.ResolverContractKey>
        //System.Linq.Expressions.Expression`1<System.Object>
        //System.Nullable`1<System.Text.RegularExpressions.RegexOptions>
        //System.Nullable`1<TouchSocket.Core.XREF.Newtonsoft.Json.Linq.JTokenType>
        //System.Nullable`1<TouchSocket.Core.XREF.Newtonsoft.Json.FloatFormatHandling>
        //System.Nullable`1<TouchSocket.Core.XREF.Newtonsoft.Json.FloatParseHandling>
        //System.Nullable`1<TouchSocket.Core.XREF.Newtonsoft.Json.DateParseHandling>
        //System.Nullable`1<TouchSocket.Core.XREF.Newtonsoft.Json.DateTimeZoneHandling>
        //System.Nullable`1<TouchSocket.Core.XREF.Newtonsoft.Json.DateFormatHandling>
        //System.Nullable`1<TouchSocket.Core.XREF.Newtonsoft.Json.Formatting>
        //System.Nullable`1<System.DateTime>
        //System.Nullable`1<System.Decimal>
        //System.Nullable`1<System.Double>
        //System.Nullable`1<TouchSocket.Core.XREF.Newtonsoft.Json.ObjectCreationHandling>
        //System.Nullable`1<TouchSocket.Core.XREF.Newtonsoft.Json.MetadataPropertyHandling>
        //System.Nullable`1<TouchSocket.Core.XREF.Newtonsoft.Json.DefaultValueHandling>
        //System.Nullable`1<TouchSocket.Core.XREF.Newtonsoft.Json.JsonPosition>
        //System.Nullable`1<TouchSocket.Core.XREF.Newtonsoft.Json.Required>
        //System.Nullable`1<TouchSocket.Core.XREF.Newtonsoft.Json.NullValueHandling>
        //System.Nullable`1<TouchSocket.Core.XREF.Newtonsoft.Json.TypeNameHandling>
        //System.Nullable`1<TouchSocket.Core.XREF.Newtonsoft.Json.ReferenceLoopHandling>
        //System.Nullable`1<System.TimeSpan>
        //System.Nullable`1<System.Byte>
        //System.Nullable`1<System.Int32>
        //System.Nullable`1<TouchSocket.Core.XREF.Newtonsoft.Json.TypeNameAssemblyFormatHandling>
        //System.Nullable`1<TouchSocket.Core.XREF.Newtonsoft.Json.StringEscapeHandling>
        //System.Nullable`1<ZFramework.ChannelType>
        //System.Nullable`1<TouchSocket.Core.XREF.Newtonsoft.Json.JsonToken>
        //System.Nullable`1<TouchSocket.Core.XREF.Newtonsoft.Json.Serialization.JsonSerializerInternalReader/PropertyPresence>
        //System.Nullable`1<System.Guid>
        //System.Nullable`1<System.SByte>
        //System.Nullable`1<System.UInt16>
        //System.Nullable`1<System.Int16>
        //System.Nullable`1<System.IO.FileAccess>
        //System.Nullable`1<System.UInt64>
        //System.Nullable`1<TouchSocket.Core.XREF.Newtonsoft.Json.PreserveReferencesHandling>
        //System.Nullable`1<System.UInt32>
        //System.Nullable`1<System.Int64>
        //System.Nullable`1<System.Single>
        //System.Nullable`1<TouchSocket.Core.XREF.Newtonsoft.Json.MissingMemberHandling>
        //System.Nullable`1<TouchSocket.Core.XREF.Newtonsoft.Json.Schema.JsonSchemaType>
        //System.Nullable`1<TouchSocket.Core.XREF.Newtonsoft.Json.ConstructorHandling>
        //System.Nullable`1<System.Runtime.Serialization.StreamingContext>
        //System.Predicate`1<System.Object>
        //System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<TouchSocket.Core.Run.WaitDataStatus>
        //System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<System.Object>
        //System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<TouchSocket.Core.Result>
        //System.Runtime.CompilerServices.TaskAwaiter`1<TouchSocket.Core.Result>
        //System.Runtime.CompilerServices.TaskAwaiter`1<System.Object>
        //System.Threading.Tasks.Task`1<TouchSocket.Core.Result>
        //System.Threading.Tasks.Task`1<System.Object>
        //System.Threading.Tasks.TaskCompletionSource`1<System.Object>
        //UnityEngine.Events.UnityAction`1<System.Single>
        //UnityEngine.Events.UnityAction`1<UnityEngine.Vector2>
        //UnityEngine.Events.UnityAction`1<UnityEngine.KeyCode>
        //UnityEngine.Events.UnityAction`1<System.Byte>
        //UnityEngine.Events.UnityEvent`1<System.Byte>
        //UnityEngine.Events.UnityEvent`1<System.Single>

        public void RefMethods()
        {
            _ = typeof(System.Runtime.Serialization.DataContractSerializer).Name;

            Activator.CreateInstance<object>();
            Array.BinarySearch<long>(null, 0L);
            Array.Empty<UInt16>();
            Array.Empty<object>();
            Array.IndexOf<object>(null, null);
            Array.IndexOf<UInt16>(null, 0);
            Array.IndexOf<UInt16>(null, 0, 0);
            var o = new object[0];
            Array.Resize<object>(ref o, 0);
            System.Linq.Enumerable.Contains<object>(null, null);
            System.Linq.Enumerable.Count<object>(null);
            System.Linq.Enumerable.First<object>(null);
            System.Linq.Enumerable.FirstOrDefault<object>(null, null);

            //System.Int32 System.Array.IndexOf<TouchSocket.Core.XREF.Newtonsoft.Json.Linq.JTokenType>(TouchSocket.Core.XREF.Newtonsoft.Json.Linq.JTokenType[], TouchSocket.Core.XREF.Newtonsoft.Json.Linq.JTokenType)
            //System.Int32 System.Linq.Enumerable::Max<TouchSocket.Core.IO.VAction>(System.Collections.Generic.IEnumerable`1 < TouchSocket.Core.IO.VAction >, System.Func`2 < TouchSocket.Core.IO.VAction, System.Int32 >)
            //System.Linq.IOrderedEnumerable`1 < System.Object > System.Linq.Enumerable::OrderByDescending<System.Object, System.Int32>(System.Collections.Generic.IEnumerable`1 < System.Object >, System.Func`2 < System.Object, System.Int32 >)
            //System.Collections.Generic.IEnumerable`1 < System.Object > System.Linq.Enumerable::Select<System.Object, System.Object>(System.Collections.Generic.IEnumerable`1 < System.Object >, System.Func`2 < System.Object, System.Object >)
            //System.Object[] System.Linq.Enumerable::ToArray<System.Object>(System.Collections.Generic.IEnumerable`1 < System.Object >)
            //System.Int64[] System.Linq.Enumerable::ToArray<System.Int64>(System.Collections.Generic.IEnumerable`1 < System.Int64 >)
            //System.Collections.Generic.Dictionary`2 < System.Object,System.Object > System.Linq.Enumerable::ToDictionary<System.Object, System.Object>(System.Collections.Generic.IEnumerable`1 < System.Object >, System.Func`2 < System.Object, System.Object >)
            //System.Collections.Generic.List`1 < System.Object > System.Linq.Enumerable::ToList<System.Object>(System.Collections.Generic.IEnumerable`1 < System.Object >)
            //System.Collections.Generic.IEnumerable`1 < System.Object > System.Linq.Enumerable::Where<System.Object>(System.Collections.Generic.IEnumerable`1 < System.Object >, System.Func`2 < System.Object, System.Boolean >)
            //System.Linq.Expressions.Expression`1 < System.Object > System.Linq.Expressions.Expression::Lambda<System.Object>(System.Linq.Expressions.Expression, System.Linq.Expressions.ParameterExpression[])
            //System.Object System.Reflection.CustomAttributeExtensions::GetCustomAttribute<System.Object>(System.Reflection.ParameterInfo)
            //System.Object System.Reflection.CustomAttributeExtensions::GetCustomAttribute<System.Object>(System.Reflection.MemberInfo)
            //System.Collections.Generic.IEnumerable`1 < System.Object > System.Reflection.CustomAttributeExtensions::GetCustomAttributes<System.Object>(System.Reflection.MemberInfo, System.Boolean)
            //System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1 < TouchSocket.Core.Result >::AwaitUnsafeOnCompleted < System.Runtime.CompilerServices.TaskAwaiter`1 < TouchSocket.Core.Result >,TouchSocket.Rpc.TouchRpc.RpcActor /< PullFileAsync > d__22 > (System.Runtime.CompilerServices.TaskAwaiter`1 < TouchSocket.Core.Result > &,TouchSocket.Rpc.TouchRpc.RpcActor /< PullFileAsync > d__22 &)
            //System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1 < System.Object >::AwaitUnsafeOnCompleted < System.Runtime.CompilerServices.TaskAwaiter,TouchSocket.Core.Reflection.Method /< InvokeObjectAsync > d__28 > (System.Runtime.CompilerServices.TaskAwaiter &, TouchSocket.Core.Reflection.Method /< InvokeObjectAsync > d__28 &)
            //System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1 < TouchSocket.Core.Result >::AwaitUnsafeOnCompleted < System.Runtime.CompilerServices.TaskAwaiter`1 < TouchSocket.Core.Result >,TouchSocket.Rpc.TouchRpc.RpcActor /< PullFileAsync > d__21 > (System.Runtime.CompilerServices.TaskAwaiter`1 < TouchSocket.Core.Result > &,TouchSocket.Rpc.TouchRpc.RpcActor /< PullFileAsync > d__21 &)
            //System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1 < System.Object >::Start < TouchSocket.Core.Reflection.Method /< InvokeObjectAsync > d__28 > (TouchSocket.Core.Reflection.Method /< InvokeObjectAsync > d__28 &)
            //System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1 < TouchSocket.Core.Result >::Start < TouchSocket.Rpc.TouchRpc.RpcActor /< PullFileAsync > d__22 > (TouchSocket.Rpc.TouchRpc.RpcActor /< PullFileAsync > d__22 &)
            //System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1 < TouchSocket.Core.Result >::Start < TouchSocket.Rpc.TouchRpc.RpcActor /< PullFileAsync > d__21 > (TouchSocket.Rpc.TouchRpc.RpcActor /< PullFileAsync > d__21 &)
            //System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1 < TouchSocket.Core.Run.WaitDataStatus >::Start < TouchSocket.Core.Run.WaitData`1 /< WaitAsync > d__19 < System.Object >> (TouchSocket.Core.Run.WaitData`1 /< WaitAsync > d__19<System.Object> &)
            //System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1 < TouchSocket.Core.Run.WaitDataStatus >::Start < TouchSocket.Core.Run.WaitData`1 /< WaitAsync > d__19 < TouchSocket.Sockets.ResponsedData >> (TouchSocket.Core.Run.WaitData`1 /< WaitAsync > d__19<TouchSocket.Sockets.ResponsedData> &)
            //System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder::AwaitUnsafeOnCompleted < System.Object,LoginPanel /< OnSendCode > d__8 > (System.Object &, LoginPanel /< OnSendCode > d__8 &)
            //System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder::AwaitUnsafeOnCompleted < System.Object,ZFramework.WebRequestComponent /< Awake > d__0 > (System.Object &, ZFramework.WebRequestComponent /< Awake > d__0 &)
            //System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder::Start < LoginPanel /< Btn_LR > d__13 > (LoginPanel /< Btn_LR > d__13 &)
            //System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder::Start < LoginPanel /< OnSendCode > d__8 > (LoginPanel /< OnSendCode > d__8 &)
            //System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder::Start < ZFramework.WebRequestComponent /< Awake > d__0 > (ZFramework.WebRequestComponent /< Awake > d__0 &)
            //System.Int32 System.Runtime.InteropServices.Marshal::SizeOf<System.Object>(System.Object)
            //System.Int32 System.Runtime.InteropServices.Marshal::SizeOf<System.UInt32>(System.UInt32)
            //System.Object System.Threading.Interlocked::CompareExchange<System.Object>(System.Object &, System.Object, System.Object)
            //System.Threading.Tasks.Task`1 < System.Object > System.Threading.Tasks.Task::FromResult<System.Object>(System.Object)
            //System.Threading.Tasks.Task`1 < System.Int32 > System.Threading.Tasks.Task::FromResult<System.Int32>(System.Int32)
            //System.Threading.Tasks.Task`1 < System.Byte > System.Threading.Tasks.Task::Run<System.Byte>(System.Func`1 < System.Byte >)
            //System.Threading.Tasks.Task`1 < System.Object > System.Threading.Tasks.Task::Run<System.Object>(System.Func`1 < System.Object >)
            //System.Threading.Tasks.Task`1 < TouchSocket.Core.Result > System.Threading.Tasks.Task::Run<TouchSocket.Core.Result>(System.Func`1 < TouchSocket.Core.Result >)
            //System.Threading.Tasks.Task`1 < System.TimeSpan > System.Threading.Tasks.Task::Run<System.TimeSpan>(System.Func`1 < System.TimeSpan >)
            //System.Threading.Tasks.Task`1 < TouchSocket.Sockets.ResponsedData > System.Threading.Tasks.Task::Run<TouchSocket.Sockets.ResponsedData>(System.Func`1 < TouchSocket.Sockets.ResponsedData >)

            UnityEngine.GameObject a = null;
            a.GetComponent<UnityEngine.Object>();
            a.AddComponent<UnityEngine.Component>();
            a.GetComponentInChildren<UnityEngine.Object>();
            a.GetComponentInChildren<UnityEngine.Object>();
            UnityEngine.Object.FindObjectOfType<UnityEngine.Object>();
            UnityEngine.Object.Instantiate<UnityEngine.Object>(null);
            UnityEngine.Object.Instantiate<UnityEngine.Object>(null, null);
            Resources.Load<UnityEngine.Object>("");
        }
    }
}
