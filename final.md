# ECS Blueprint (DOTS + OneJS)

## Założenia
- Runtime w 100% ECS (Entities 1.x, Entities Graphics).
- OneJS UI zostaje; komunikacja przez dedykowany UI Bridge.
- ScriptableObjecty służą tylko jako authoring -> Baker -> BlobAssets.
- EventBus usunięty; eventy jako ECS event entities/buffers.
- Zachowujemy: `App/**`, `Assets/_heroes/Frameworks/**`, `Assets/_thirdparty/**`
  oraz MonoBehaviours od kamery i blur.

---

## 1) Struktura projektu (propozycja)

Assets/
- Game/
  - Core/ (shared types, ids, tags)
  - Data/ (IComponentData, buffers)
  - Content/
    - Authoring/ (SO + Bakers)
    - Blobs/ (BlobAsset definitions)
  - Systems/
    - Simulation/
    - Presentation/
  - UI/
    - Bridge/
- Content/
  - Buildings/
  - Heroes/
  - Items/
  - Skills/
  - Effects/
  - Upgrades/
  - Entities/
- _heroes/Frameworks/ (zostaje)
- _thirdparty/ (zostaje)

---

## 2) SO jako authoring (wszystkie kategorie)
- Budynki
- Bohaterowie
- Itemy
- Skille
- Efekty
- Upgrady budynków
- Encje ogólne

SO służą wyłącznie do edycji; runtime korzysta z BlobAssets.

---

## 3) BlobAssets (runtime data)

BuildingCatalogBlob
- BuildingDefBlob[]: Id, Name, Category, Price, IconId, PrefabRef, Stats, Capabilities

BuildingUpgradeCatalogBlob
- UpgradeDefBlob[]: Id, BuildingId, Level, Price, DeltaStats, DeltaCaps

HeroCatalogBlob
- HeroDefBlob[]: Id, Name, BaseStats, SkillIds[], PrefabRef

ItemCatalogBlob
- ItemDefBlob[]: Id, Name, EffectIds[], EquipSlot

SkillCatalogBlob
- SkillDefBlob[]: Id, Name, Cooldown, CastTime, EffectIds[]

EffectCatalogBlob
- EffectDefBlob[]: Id, Type, Duration, StatModifiers[]

EntityCatalogBlob
- EntityDefBlob[]: Id, BaseStats, PrefabRef

---

## 4) ECS Components (runtime)

### Global / Singletons
- GameState
- KingdomResources
- SelectedBuildingType
- SelectedEntity
- UIReadModel

### Buildings
- BuildingTag
- BuildingTypeId
- BuildingLevel
- Health
- WorldTransform
- OwnerFaction
- BuildingCapabilities (mask/buffer)
- BuildingState

### Heroes
- HeroTag
- HeroTypeId
- HeroState
- Health
- WorldTransform
- OwnerFaction
- Target
- InventoryBuffer<ItemSlot>
- SkillCooldownBuffer<SkillCooldown>

### Items
- ItemTag
- ItemTypeId
- Owner
- Equipped

### Skills / Effects
- CastingState
- ActiveEffectsBuffer<EffectInstance>

---

## 5) Event Entities / Buffers
- BuildingPlacedEvent
- BuildingSelectionChangedEvent
- ResourcesChangedEvent
- EntityDamagedEvent
- EntityHealedEvent
- EntityDiedEvent
- HeroStateChangedEvent

Eventy są tworzone i kasowane w tym samym frame.

---

## 6) Systemy (docelowy zestaw)

### Simulation
- UICommandConsumeSystem
- BuildingPlacementSystem
- BuildingSpawnSystem
- BuildingUpgradeSystem
- ResourceTransactionSystem
- HealthSystem
- DamageSystem
- HealSystem
- EffectSystem
- SkillCooldownSystem
- HeroStateSystem
- SelectionSystem

### Presentation
- EntitySpawnPresentationSystem (Entities Graphics)
- UIReadbackSystem

### AI (później)
- GoapWorldStateSyncSystem
- GoapPlanExecuteSystem

---

## 7) UI Bridge (OneJS <-> ECS)

Write (UI -> ECS)
- SetSelectedBuilding(id)
- RequestPlacement(worldPos)

Read (ECS -> UI)
- UIReadModel:
  - gold
  - selectedBuildingType
  - buildingsList (id, name, price, category)
  - selectedEntityId
  - (opcjonalnie) cooldowny/heroState

---

## 8) Prefaby
- Prefaby GameObject jako authoring -> Baker -> Entity prefab
- Runtime spawn: EntityManager.Instantiate(entityPrefab)

---

## 9) Etapy migracji
1. Building placement
2. Resources
3. Selection
4. AI/GOAP (na końcu)

---

## 10) Zakres zachowania
Zostają:
- OneJS UI (`App/**`)
- Frameworks (`Assets/_heroes/Frameworks/**`)
- `_thirdparty/**` (RTSCameraController itp.)
- Content Editor (dostosowany pod nowe SO + Baking)

Usuwamy:
- obecną architekturę gameplay (Game/Systems/Domain/EventBus/Presentation/…)
