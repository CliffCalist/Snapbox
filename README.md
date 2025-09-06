# Snapbox

**Snapbox** is a flexible, metadata-driven framework for managing game state in Unity. It enables centralized snapshot saving/loading with full control over storage strategies and restoration flow.

---

# Features

- Metadata-driven architecture  
- Custom saver/loader/metadata support  
- Async/sync persistence  
- Built-in local JSON storage  
- Scene & entity state handlers  
- Post-restore logic guarantee  
- Hierarchical restoration graph  
- Dependency-aware execution

# Installing

To install via UPM, use "Install package from git URL" and add the following:

```
1. https://github.com/CliffCalist/Unity-Tools.git
2. https://github.com/CliffCalist/Snapbox.git
```

# Usage

## Initializing Snapbox

```csharp
var snapbox = new Snapbox(loader, saver);
```

- `loader`: implementation of `ISnapshotLoader`
- `saver`: implementation of `ISnapshotSaver`

Snapbox comes with a built-in logger, optimized for grouped logs and minimal spam.

## Working with Snapshot Metadata

**Snapshot metadata** defines how each snapshot is identified and tracked.

Each metadata object implements the `ISnapshotMetadata` interface, providing:
- A unique `SnapshotName`
- Change and deletion tracking via `IsChanged` and `IsDeleted` flags (managed by Snapbox itself)

Metadata must be registered with Snapbox before loading or saving snapshots.

Register metadata:

```csharp
snapbox.AddMetadata(new MySnapshotMetadata("player_inventory"));
```

Register multiple:

```csharp
snapbox.AddMetadata(new List<ISnapshotMetadata>
{
    new MySnapshotMetadata("inventory"),
    new MySnapshotMetadata("settings")
});
```

Remove metadata:

```csharp
snapbox.RemoveMetadata("player_inventory");
snapbox.RemoveMetadata(new List<string> { "settings", "inventory" });
```

## Saving Snapshots

Save all changed snapshots asynchronously:

```csharp
await snapbox.SaveAllSnapshotsAsync();
```

Or synchronously:

```csharp
snapbox.SaveAllSnapshots();
```

- Snapshots are only saved if `IsChanged` is `true`
- Setting a snapshot to `null` marks it as deleted

## Retrieving & Updating Snapshots

Read a snapshot:

```csharp
var inventory = snapbox.GetSnapshot<Inventory>("player_inventory");
```

Update a snapshot:

```csharp
snapbox.SetSnapshot("player_inventory", inventory);
```

Delete a snapshot:

```csharp
snapbox.SetSnapshot("player_inventory", null);
```

# Custom Snapbox Backends

You can integrate Snapbox with any storage backend by implementing:

- `ISnapshotSaver`
- `ISnapshotLoader`
- `ISnapshotMetadata`

This allows you to connect Snapbox to local storage, remote servers, databases, cloud services (like Firebase), or encrypted systems.

---

## Snapshot Metadata

Snapshot metadata defines how each snapshot is tracked and persisted. You are free to store any additional data inside metadata — such as file paths, database keys, encryption info, user identifiers, or the expected type of the snapshot.

> ⚠️ Snapbox uses `ISnapshotMetadata` as a generic abstraction. To work correctly, your custom loader/saver must validate and cast metadata to its expected implementation.

The `IsChanged` and `IsDeleted` properties are used internally by Snapbox — you should expose them but don't need to manage them manually.

### Example Metadata

```csharp
public class MySnapshotMetadata : ISnapshotMetadata
{
    public string SnapshotName { get; }
    public bool IsChanged { get; set; }
    public bool IsDeleted { get; set; }

    public string FilePath { get; }
    public Type SnapshotType { get; }

    public MySnapshotMetadata(string name, Type snapshotType)
    {
        SnapshotName = name;
        SnapshotType = snapshotType;
        FilePath = Path.Combine(Application.persistentDataPath, name + ".json");
    }
}
```

---

### Example Saver

```csharp
public class FileSnapshotSaver : ISnapshotSaver
{
    public async Task SaveAsync(ISnapshotMetadata metadata, object snapshot)
    {
        if (metadata is not MySnapshotMetadata typed)
            throw new InvalidOperationException($"Expected {nameof(MySnapshotMetadata)}, got {metadata.GetType()}");

        var json = JsonUtility.ToJson(snapshot);
        await File.WriteAllTextAsync(typed.FilePath, json);
    }

    public void Save(ISnapshotMetadata metadata, object snapshot)
    {
        if (metadata is not MySnapshotMetadata typed)
            throw new InvalidOperationException($"Expected {nameof(MySnapshotMetadata)}, got {metadata.GetType()}");

        File.WriteAllText(typed.FilePath, JsonUtility.ToJson(snapshot));
    }

    public async Task DeleteAsync(ISnapshotMetadata metadata)
    {
        if (metadata is not MySnapshotMetadata typed)
            throw new InvalidOperationException($"Expected {nameof(MySnapshotMetadata)}, got {metadata.GetType()}");

        if (File.Exists(typed.FilePath))
            File.Delete(typed.FilePath);

        await Task.CompletedTask;
    }

    public void Delete(ISnapshotMetadata metadata)
    {
        if (metadata is not MySnapshotMetadata typed)
            throw new InvalidOperationException($"Expected {nameof(MySnapshotMetadata)}, got {metadata.GetType()}");

        if (File.Exists(typed.FilePath))
            File.Delete(typed.FilePath);
    }
}
```

---

### Example Loader

```csharp
public class FileSnapshotLoader : ISnapshotLoader
{
    public async Task<object> LoadAsync(ISnapshotMetadata metadata)
    {
        if (metadata is not MySnapshotMetadata typed)
            throw new InvalidOperationException($"Expected {nameof(MySnapshotMetadata)}, got {metadata.GetType()}");

        if (!File.Exists(typed.FilePath))
            return null;

        var json = await File.ReadAllTextAsync(typed.FilePath);
        var result = JsonUtility.FromJson(json, typed.SnapshotType);

        if (result == null)
            throw new InvalidOperationException("Failed to deserialize snapshot.");

        return result;
    }
}
```

> ✅ You can use `SnapshotType` or another identifier (such as a registered type map) to deserialize the object safely. This approach avoids misuse and runtime casting errors.

---

---

# Scene and Entity State Handlers

Snapbox provides a powerful mechanism for **restoring and managing game state hierarchically**, using two core classes:

- `EntityStateHandler` — handles the state of a specific entity (e.g., building, unit, system).
- `SceneStateHandler` — manages the full graph of entity handlers and restores the entire scene in a dependency-aware, layered way.

These handlers enable you to decouple snapshot logic from the game logic, providing **clean, deterministic restoration**.

## SceneContext

To coordinate everything, each scene must contain a `SceneContext`. This component is simple and needs no configuration. It:

1. Tracks the current scene restoration phase (`StateRestoringPhase`).
2. Holds a reference to the active `Snapbox` database once `SceneStateHandler.RestoreState()` is called.

## EntityStateHandler

You create an `EntityStateHandler` for each entity whose state should be restored and managed.

The handler provides:
- **Three lifecycle stages:**
  - Metadata registration
  - State restoration
  - Initialization
- **Support for:**
  - Dependencies (`_dependencies`)
  - Children (`_children`)
  - Dynamic graph growth
  - Context access (`_sceneContext`, `_transform`)
  - Post-initialization logic

These handlers are automatically processed by the `SceneStateHandler`, or they **self-initialize** if created after scene restoration has completed.

> Note: If you want typed access to the target MonoBehaviour, use the generic `EntityStateHandler<T>` — this provides the `Target` property, which safely resolves the component on the same GameObject.

## Example: Full EntityStateHandler Implementation

```csharp
public class WalletHandler : EntityStateHandler<Wallet>
{
    [SerializeField] private string _snapshotKey = "wallet";

    protected override IEnumerable<EntityStateHandler> GetAdditionalDependencies()
    {
        // Dynamically resolve extra dependencies if needed
        return Enumerable.Empty<EntityStateHandler>();
    }

    protected override IEnumerable<EntityStateHandler> GetAdditionalChildren()
    {
        // Dynamically created children can be reported here
        return Enumerable.Empty<EntityStateHandler>();
    }

    protected override void RegisterSnapshotMetadataCore()
    {
        var metadata = new LocalSnapshotMetadata(
            _snapshotKey,
            typeof(WalletData),
            Application.persistentDataPath + "/Wallets"
        );
        _sceneContext.Database.AddMetadata(metadata);
    }

    protected override void RestoreStateCore()
    {
        var data = _sceneContext.Database.GetSnapshot<WalletData>(_snapshotKey);
        if (data == null)
        {
            Debug.LogWarning("No wallet data found, creating empty wallet.");
            Target.SetBalance(0);
        }
        else
        {
            Target.SetBalance(data.Balance);
        }
    }

    protected override void InitializeCore()
    {
        Target.Init();
    }
}
```

## SceneStateHandler

This is the entry point to **restore and manage all state handlers** in the scene.

**Setup:**

- Assign the `SceneContext` to `_context`
- Assign root entity handlers to `_rootHandlers`

**Runtime API:**

```csharp
public void RestoreState(Snapbox database, Action onComplete = null);
public void CaptureState();
```

`CaptureState` triggers all `EntityStateHandler`s to write their snapshots.  
`RestoreState` builds a layered graph of handlers and executes:

1. Metadata registration
2. Database load
3. State restoration
4. Dependency resolution
5. Child discovery
6. Initialization — from leaves to root

## Example: SceneStateHandler Runtime Setup

```csharp
[SerializeField] private SceneStateHandler _sceneStateHandler;
[SerializeField] private SceneContext _sceneContext;

private void Start()
{
    var snapbox = new Snapbox(
        new LocalSnapshotLoader(),
        new LocalSnapshotSaver()
    );

    _sceneStateHandler.RestoreState(snapbox, () =>
    {
        Debug.Log("Scene state restored and initialized.");
    });
}
```

> You can access the Snapbox database during handler methods via `_sceneContext.Database`.

## Summary

- Scene restoration is **layered**, with child handlers initialized before parents.
- Dependencies only apply within the same layer.
- New child handlers discovered during restoration are processed immediately.
- Handlers created **after** full restoration self-initialize and are included in subsequent saves.

# Utilities and Helpers for EntityStateHandler

Snapbox includes a set of helper components and utilities to simplify working with `EntityStateHandler` in more complex or large-scale projects.

---

## SimpleEntityInitializer & ISimpleInitializable

When you only need to **initialize an entity after scene restoration**, and don’t need to implement custom restore or metadata logic, you can use:

- `SimpleEntityInitializer` — a ready-to-use `EntityStateHandler` subclass that only calls an `Init()` method on a target component.
- `ISimpleInitializable` — an interface with a single method: `void Init();`

This reduces boilerplate and is useful for one-shot components or systems that must be initialized but have no persistent state.

```csharp
public class MyComponent : MonoBehaviour, ISimpleInitializable
{
    public void Init()
    {
        Debug.Log("Initialized!");
    }
}
```

Add `SimpleEntityInitializer` on the same GameObject — no code needed.

## DependencyDecorator

When multiple sibling handlers share the same dependencies, you can use a `DependencyDecorator` to avoid duplicating configuration.

- Place `DependencyDecorator` on a parent GameObject.
- It will apply its `_dependencies` to all `EntityStateHandler` components below it in the scene hierarchy.
- SceneStateHandler will resolve this correctly in the restoration graph.

This greatly simplifies hierarchy management and reduces setup overhead.

## ContextPathUtilities

A static utility class to simplify path formatting:

- `IsStringNotEmpty(string input)`  
  Returns `true` if the string is not null, not empty, and doesn't contain only whitespace.
  
- `PathToName(IEnumerable<string> pathParts)`  
  Joins the provided segments using underscores (`_`), e.g. `["Zone1", "Room2"]` → `"Zone1_Room2"`.

Useful for dynamic snapshot key generation or organizing hierarchy-based storage.

## IContextPathDecorator & ContextPathDecorator

To create **virtual folder structures for snapshots**, Snapbox supports contextual path decorators.

- `ContextPathDecorator` can be added to any GameObject in the scene hierarchy.
- It defines a `_selfContextPath` string, such as `"Enemies"` or `"Zone_1"`.
- All child `EntityStateHandler` components will automatically resolve their **context path** using this hierarchy when building metadata.

Use the `GetContextPath()` method in your `EntityStateHandler` when registering snapshot metadata:

```csharp
var path = GetContextPath(); // e.g., "Enemies_Skeleton1"
```

This enables:

- Clean organization of snapshot files (especially for file-based databases).
- Logical grouping of runtime data.
- Visual matching of scene hierarchy to snapshot storage structure.

---

These helpers are **optional**, but greatly enhance maintainability and clarity in complex projects.

# Roadmap

- [ ] Safer handling of DB types with single-mode (sync-only or async-only) access
- [ ] Snapshot buffer system for async DBs  
- [ ] Versioning and migration support  
- [ ] Parallel async DB operations
- [ ] Editor visualization of scene graph & dependency tree
- [ ] (Needs validation) Decouple `EntityStateHandler` from fixed metadata implementation  
- [ ] (Needs validation) Extract state handler algorithm into standalone framework  
