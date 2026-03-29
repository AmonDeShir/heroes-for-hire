
declare namespace CS {
    // const __keep_incompatibility: unique symbol;
    // 
    // interface $Ref<T> {
    //     value: T
    // }
    // namespace System {
    //     interface Array$1<T> extends System.Array {
    //         get_Item(index: number):T;
    //         
    //         set_Item(index: number, value: T):void;
    //     }
    // }
    // interface $Task<T> {}
    namespace System {
        class Object
        {
            protected [__keep_incompatibility]: never;
        }
        class ValueType extends System.Object
        {
            protected [__keep_incompatibility]: never;
        }
        class Enum extends System.ValueType implements System.IFormattable, System.IComparable, System.IConvertible
        {
            protected [__keep_incompatibility]: never;
        }
        interface IFormattable
        {
        }
        interface IComparable
        {
        }
        interface IConvertible
        {
        }
        class Type extends System.Reflection.MemberInfo implements System.Runtime.InteropServices._MemberInfo, System.Runtime.InteropServices._Type, System.Reflection.ICustomAttributeProvider, System.Reflection.IReflect
        {
            protected [__keep_incompatibility]: never;
        }
        class String extends System.Object implements System.ICloneable, System.IComparable, System.IComparable$1<string>, System.IConvertible, System.Collections.Generic.IEnumerable$1<number>, System.Collections.IEnumerable, System.IEquatable$1<string>
        {
            protected [__keep_incompatibility]: never;
        }
        interface ICloneable
        {
        }
        interface IComparable$1<T>
        {
        }
        class Char extends System.ValueType implements System.IComparable, System.IComparable$1<number>, System.IConvertible, System.IEquatable$1<number>
        {
            protected [__keep_incompatibility]: never;
        }
        interface IEquatable$1<T>
        {
        }
        class Boolean extends System.ValueType implements System.IComparable, System.IComparable$1<boolean>, System.IConvertible, System.IEquatable$1<boolean>
        {
            protected [__keep_incompatibility]: never;
        }
        class Array extends System.Object implements System.Collections.IStructuralComparable, System.Collections.IStructuralEquatable, System.ICloneable, System.Collections.ICollection, System.Collections.IEnumerable, System.Collections.IList
        {
            protected [__keep_incompatibility]: never;
        }
        class Void extends System.ValueType
        {
            protected [__keep_incompatibility]: never;
        }
        class Int32 extends System.ValueType implements System.IFormattable, System.ISpanFormattable, System.IComparable, System.IComparable$1<number>, System.IConvertible, System.IEquatable$1<number>
        {
            protected [__keep_incompatibility]: never;
        }
        interface ISpanFormattable
        {
        }
        interface IDisposable
        {
        }
        class ReadOnlySpan$1<T> extends System.ValueType
        {
            protected [__keep_incompatibility]: never;
        }
        class UInt64 extends System.ValueType implements System.IFormattable, System.ISpanFormattable, System.IComparable, System.IComparable$1<bigint>, System.IConvertible, System.IEquatable$1<bigint>
        {
            protected [__keep_incompatibility]: never;
        }
        class Single extends System.ValueType implements System.IFormattable, System.ISpanFormattable, System.IComparable, System.IComparable$1<number>, System.IConvertible, System.IEquatable$1<number>
        {
            protected [__keep_incompatibility]: never;
        }
    }
    namespace UnityEngine {
        /** Base class for all objects Unity can reference.
        */
        class Object extends System.Object
        {
            protected [__keep_incompatibility]: never;
        }
        /** Base class for all objects that can exist in a scene. Add components to a GameObject to control its appearance and behavior.
        */
        class GameObject extends UnityEngine.Object
        {
            protected [__keep_incompatibility]: never;
            /** The Transform attached to the GameObject (Read Only).
            */
            public get transform(): UnityEngine.Transform;
            /** The TransformHandle of the GameObject (Read Only).
            */
            public get transformHandle(): UnityEngine.TransformHandle;
            /** Integer identifying the layer the GameObject is assigned to.
            */
            public get layer(): number;
            public set layer(value: number);
            /** The local active state of the GameObject. True if active, false if inactive. (Read Only)
            */
            public get activeSelf(): boolean;
            /** The active state of the GameObject in the Scene hierarchy. True if active, false if inactive. (Read Only)
            */
            public get activeInHierarchy(): boolean;
            /** Whether there are any Static Editor Flags set for the GameObject.
            */
            public get isStatic(): boolean;
            public set isStatic(value: boolean);
            /** The tag assigned to the GameObject.
            */
            public get tag(): string;
            public set tag(value: string);
            /** The Scene that contains the GameObject.
            */
            public get scene(): UnityEngine.SceneManagement.Scene;
            /** The Scene culling mask defined for the GameObject. (Read Only)
            */
            public get sceneCullingMask(): bigint;
            public get gameObject(): UnityEngine.GameObject;
            /** Creates a GameObject of the specified PrimtiveType with a mesh renderer and appropriate collider.
            * @param $type The type of primitive object to create, specified as a member of the PrimitiveType enum.
            */
            public static CreatePrimitive ($type: UnityEngine.PrimitiveType) : UnityEngine.GameObject
            /** Retrieves a reference to a component of specified type, by providing the component type as a method parameter.
            * @param $type The type of component to search for, specified as a Type object.
            * @returns A reference to a component of the specified type, returned as a Component type. If no component is found, returns null. 
            */
            public GetComponent ($type: System.Type) : UnityEngine.Component
            /** Retrieves a reference to a component of the specified type, by providing the name of the component type as a method parameter.
            * @param $type The name of the type of component to search for, specified as a string.
            * @returns A reference to a component of the specified type, returned as a Component type. If no component is found, returns null. 
            */
            public GetComponent ($type: string) : UnityEngine.Component
            /** This is the non-generic version of this method.
            * @param $type The type of Component to retrieve.
            * @param $includeInactive Whether to include inactive child GameObjects in the search.
            * @returns A component of the matching type, if found. 
            */
            public GetComponentInChildren ($type: System.Type, $includeInactive: boolean) : UnityEngine.Component
            /** This is the non-generic version of this method.
            * @param $type The type of Component to retrieve.
            * @param $includeInactive Whether to include inactive child GameObjects in the search.
            * @returns A component of the matching type, if found. 
            */
            public GetComponentInChildren ($type: System.Type) : UnityEngine.Component
            /** The non-generic version of this method.
            * @param $type The type of component to search for.
            * @param $includeInactive Whether to include inactive parent GameObjects in the search.
            * @returns A Component of the matching type, otherwise null if no matching Component is found. 
            */
            public GetComponentInParent ($type: System.Type, $includeInactive: boolean) : UnityEngine.Component
            /** The non-generic version of this method.
            * @param $type The type of component to search for.
            * @param $includeInactive Whether to include inactive parent GameObjects in the search.
            * @returns A Component of the matching type, otherwise null if no matching Component is found. 
            */
            public GetComponentInParent ($type: System.Type) : UnityEngine.Component
            /** The non-generic version of this method.
            * @param $type The type of component to search for.
            * @returns An array containing all matching components of type type. 
            */
            public GetComponents ($type: System.Type) : System.Array$1<UnityEngine.Component>
            public GetComponents ($type: System.Type, $results: System.Collections.Generic.List$1<UnityEngine.Component>) : void
            /** The non-generic version of this method.
            * @param $type The type of component to search for.
            * @param $includeInactive Whether to include inactive child GameObjects in the search.
            * @returns An array of all found components matching the specified type. 
            */
            public GetComponentsInChildren ($type: System.Type) : System.Array$1<UnityEngine.Component>
            /** The non-generic version of this method.
            * @param $type The type of component to search for.
            * @param $includeInactive Whether to include inactive child GameObjects in the search.
            * @returns An array of all found components matching the specified type. 
            */
            public GetComponentsInChildren ($type: System.Type, $includeInactive: boolean) : System.Array$1<UnityEngine.Component>
            public GetComponentsInParent ($type: System.Type) : System.Array$1<UnityEngine.Component>
            /** The non-generic version of this method.
            * @param $type The type of component to search for.
            * @param $includeInactive Whether to include inactive parent GameObjects in the search.
            * @returns An array of all found components matching the specified type. 
            */
            public GetComponentsInParent ($type: System.Type, $includeInactive: boolean) : System.Array$1<UnityEngine.Component>
            /** The non-generic version of this method.
            * @param $type The type of component to search for.
            * @param $component The out parameter that will contain the component or null.
            * @returns Returns true if the component is found, false otherwise. 
            */
            public TryGetComponent ($type: System.Type, $component: $Ref<UnityEngine.Component>) : boolean
            /** Retrieves the first active GameObject tagged with the specified tag. Returns null if no GameObject has the tag.
            * @param $tag The tag to search for.
            */
            public static FindWithTag ($tag: string) : UnityEngine.GameObject
            public static FindGameObjectsWithTag ($tag: string, $results: System.Collections.Generic.List$1<UnityEngine.GameObject>) : void
            public SendMessageUpwards ($methodName: string, $options: UnityEngine.SendMessageOptions) : void
            public SendMessage ($methodName: string, $options: UnityEngine.SendMessageOptions) : void
            public BroadcastMessage ($methodName: string, $options: UnityEngine.SendMessageOptions) : void
            /** Adds a component of the specified type to the GameObject.
            */
            public AddComponent ($componentType: System.Type) : UnityEngine.Component
            /** Retrieves the total number of components currently attached to the GameObject.
            * @returns The number of components on the GameObject as an Integer value. 
            */
            public GetComponentCount () : number
            /** Retrieves a reference to a component of type T at a specific index on the specified GameObject.
            * @param $index The index position in the array of components at which to find the requested object.
            * @returns A reference to a component of type T at the specified index. If no component is found at the specified index, returns null. 
            */
            public GetComponentAtIndex ($index: number) : UnityEngine.Component
            /** Retrieves the index of the specified component in the array of components attached to the GameObject.
            * @param $component The component to search for.
            * @returns The index of the specified Component if it exists. Otherwise, returns -1. 
            */
            public GetComponentIndex ($component: UnityEngine.Component) : number
            /** Activates or deactivates the GameObject locally, according to the value of the supplied parameter.
            * @param $value The active state to set, where true sets the GameObject to active and false sets it to inactive.
            */
            public SetActive ($value: boolean) : void
            /** Checks if the specified tag is attached to the GameObject.
            * @param $tag The tag to check for on the GameObject.
            * @returns true if the GameObject has the given tag, false otherwise. 
            */
            public CompareTag ($tag: string) : boolean
            /** Checks if the specified tag is attached to the GameObject.
            * @param $tag A TagHandle representing the tag to check for on the GameObject.
            * @returns true if the GameObject has the given tag, false otherwise. 
            */
            public CompareTag ($tag: UnityEngine.TagHandle) : boolean
            public static FindGameObjectWithTag ($tag: string) : UnityEngine.GameObject
            /** Retrieves an array of all active GameObjects tagged with the specified tag. Returns an empty array if no GameObjects have the tag.
            * @param $tag The name of the tag to search for GameObjects by.
            */
            public static FindGameObjectsWithTag ($tag: string) : System.Array$1<UnityEngine.GameObject>
            /** Calls the specified method on every MonoBehaviour attached to the GameObject and on every ancestor of the behaviour.
            * @param $methodName The name of the MonoBehaviour method to call.
            * @param $value An optional parameter value to pass to the called method.
            * @param $options Whether an error should be raised if the method doesn't exist on the target object.
            */
            public SendMessageUpwards ($methodName: string, $value: any, $options: UnityEngine.SendMessageOptions) : void
            /** Calls the specified method on every MonoBehaviour attached to the GameObject and on every ancestor of the behaviour.
            * @param $methodName The name of the MonoBehaviour method to call.
            * @param $value An optional parameter value to pass to the called method.
            * @param $options Whether an error should be raised if the method doesn't exist on the target object.
            */
            public SendMessageUpwards ($methodName: string, $value: any) : void
            /** Calls the specified method on every MonoBehaviour attached to the GameObject and on every ancestor of the behaviour.
            * @param $methodName The name of the MonoBehaviour method to call.
            * @param $value An optional parameter value to pass to the called method.
            * @param $options Whether an error should be raised if the method doesn't exist on the target object.
            */
            public SendMessageUpwards ($methodName: string) : void
            /** Calls the specified method on every MonoBehaviour attached to the GameObject.
            * @param $methodName The name of the MonoBehaviour method to call.
            * @param $value An optional parameter value to pass to the called method.
            * @param $options Whether an error should be raised if the method doesn't exist on the target object.
            */
            public SendMessage ($methodName: string, $value: any, $options: UnityEngine.SendMessageOptions) : void
            /** Calls the specified method on every MonoBehaviour attached to the GameObject.
            * @param $methodName The name of the MonoBehaviour method to call.
            * @param $value An optional parameter value to pass to the called method.
            * @param $options Whether an error should be raised if the method doesn't exist on the target object.
            */
            public SendMessage ($methodName: string, $value: any) : void
            /** Calls the specified method on every MonoBehaviour attached to the GameObject.
            * @param $methodName The name of the MonoBehaviour method to call.
            * @param $value An optional parameter value to pass to the called method.
            * @param $options Whether an error should be raised if the method doesn't exist on the target object.
            */
            public SendMessage ($methodName: string) : void
            /** Calls the specified method on every MonoBehaviour attached to the GameObject or any of its children.
            * @param $methodName The name of the MonoBehaviour method to call.
            * @param $parameter An optional parameter value to pass to the called method.
            * @param $options Whether an error should be raised if the method doesn't exist on the target object.
            */
            public BroadcastMessage ($methodName: string, $parameter: any, $options: UnityEngine.SendMessageOptions) : void
            /** Calls the specified method on every MonoBehaviour attached to the GameObject or any of its children.
            * @param $methodName The name of the MonoBehaviour method to call.
            * @param $parameter An optional parameter value to pass to the called method.
            * @param $options Whether an error should be raised if the method doesn't exist on the target object.
            */
            public BroadcastMessage ($methodName: string, $parameter: any) : void
            /** Calls the specified method on every MonoBehaviour attached to the GameObject or any of its children.
            * @param $methodName The name of the MonoBehaviour method to call.
            * @param $parameter An optional parameter value to pass to the called method.
            * @param $options Whether an error should be raised if the method doesn't exist on the target object.
            */
            public BroadcastMessage ($methodName: string) : void
            /** Finds and returns a GameObject with the specified name or hierarchy path.
            * @param $name The name or hierarchy path of the GameObject to find.
            */
            public static Find ($name: string) : UnityEngine.GameObject
            public static SetGameObjectsActive ($entityIds: Unity.Collections.NativeArray$1<UnityEngine.EntityId>, $active: boolean) : void
            public static InstantiateGameObjects ($sourceEntityId: UnityEngine.EntityId, $count: number, $newEntityIds: Unity.Collections.NativeArray$1<UnityEngine.EntityId>, $newTransformEntityIds: Unity.Collections.NativeArray$1<UnityEngine.EntityId>, $destinationScene?: UnityEngine.SceneManagement.Scene) : void
            public static GetScene ($entityId: UnityEngine.EntityId) : UnityEngine.SceneManagement.Scene
            public constructor ($name: string)
            public constructor ()
            public constructor ($name: string, ...components: System.Type[])
        }
        /** The various primitives that can be created using the GameObject.CreatePrimitive function.
        */
        enum PrimitiveType
        { Sphere = 0, Capsule = 1, Cylinder = 2, Cube = 3, Plane = 4, Quad = 5 }
        /** Base class for everything attached to a GameObject.
        */
        class Component extends UnityEngine.Object
        {
            protected [__keep_incompatibility]: never;
        }
        /** Options for how to send a message.
        */
        enum SendMessageOptions
        { RequireReceiver = 0, DontRequireReceiver = 1 }
        /** Position, rotation and scale of an object.
        */
        class Transform extends UnityEngine.Component implements System.Collections.IEnumerable
        {
            protected [__keep_incompatibility]: never;
        }
        /** Position, rotation and scale of an object.
        */
        class TransformHandle extends System.ValueType implements System.IComparable$1<UnityEngine.TransformHandle>, System.IEquatable$1<UnityEngine.TransformHandle>
        {
            protected [__keep_incompatibility]: never;
        }
        /** A handle to one of the tag values that can be applied to a GameObject.
        */
        class TagHandle extends System.ValueType
        {
            protected [__keep_incompatibility]: never;
        }
        class EntityId extends System.ValueType implements System.IComparable$1<UnityEngine.EntityId>, System.IEquatable$1<UnityEngine.EntityId>
        {
            protected [__keep_incompatibility]: never;
        }
    }
    namespace System.Reflection {
        class MemberInfo extends System.Object implements System.Runtime.InteropServices._MemberInfo, System.Reflection.ICustomAttributeProvider
        {
            protected [__keep_incompatibility]: never;
        }
        interface ICustomAttributeProvider
        {
        }
        interface IReflect
        {
        }
    }
    namespace System.Runtime.InteropServices {
        interface _MemberInfo
        {
        }
        interface _Type
        {
        }
    }
    namespace System.Collections.Generic {
        interface IEnumerable$1<T> extends System.Collections.IEnumerable
        {
        }
        interface IReadOnlyList$1<T> extends System.Collections.Generic.IEnumerable$1<T>, System.Collections.IEnumerable, System.Collections.Generic.IReadOnlyCollection$1<T>
        {
        }
        interface IReadOnlyCollection$1<T> extends System.Collections.Generic.IEnumerable$1<T>, System.Collections.IEnumerable
        {
        }
        interface IList$1<T> extends System.Collections.Generic.IEnumerable$1<T>, System.Collections.IEnumerable, System.Collections.Generic.ICollection$1<T>
        {
        }
        interface ICollection$1<T> extends System.Collections.Generic.IEnumerable$1<T>, System.Collections.IEnumerable
        {
        }
        class List$1<T> extends System.Object implements System.Collections.Generic.IReadOnlyList$1<T>, System.Collections.ICollection, System.Collections.Generic.IEnumerable$1<T>, System.Collections.IEnumerable, System.Collections.Generic.IList$1<T>, System.Collections.Generic.IReadOnlyCollection$1<T>, System.Collections.IList, System.Collections.Generic.ICollection$1<T>
        {
            protected [__keep_incompatibility]: never;
            public [Symbol.iterator]() : IterableIterator<T>
        }
    }
    namespace System.Collections {
        interface IEnumerable
        {
        }
        interface IStructuralComparable
        {
        }
        interface IStructuralEquatable
        {
        }
        interface ICollection extends System.Collections.IEnumerable
        {
        }
        interface IList extends System.Collections.ICollection, System.Collections.IEnumerable
        {
        }
    }
    namespace Unity.Collections {
        class NativeArray$1<T> extends System.ValueType implements System.Collections.Generic.IEnumerable$1<T>, System.Collections.IEnumerable, System.IDisposable, System.IEquatable$1<Unity.Collections.NativeArray$1<T>>
        {
            protected [__keep_incompatibility]: never;
            public [Symbol.iterator]() : IterableIterator<T>
        }
    }
    namespace UnityEngine.SceneManagement {
        /** The runtime data structure for a scene.
        */
        class Scene extends System.ValueType
        {
            protected [__keep_incompatibility]: never;
        }
    }
}