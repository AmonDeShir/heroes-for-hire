
declare namespace CS.Heroes.Presentation.UI.BuildingPanel {
  const __keep_incompatibility: unique symbol;

  class BuildingPanelPresenter {
    public get Selected(): string;
    public set Selected(value: string);
    public get Buildings(): System.Array$1<Heroes.Presentation.UI.BuildingPanel.BuildingDTO>;
    public set Buildings(value: System.Array$1<Heroes.Presentation.UI.BuildingPanel.BuildingDTO>);
    public SelectBuilding ($buildingId: string) : void
    public add_OnSelectedChanged ($value: System.Action$1<string>) : void
    public remove_OnSelectedChanged ($value: System.Action$1<string>) : void
    public add_OnBuildingsChanged ($value: System.Action$1<System.Array$1<Heroes.Presentation.UI.BuildingPanel.BuildingDTO>>) : void
    public remove_OnBuildingsChanged ($value: System.Action$1<System.Array$1<Heroes.Presentation.UI.BuildingPanel.BuildingDTO>>) : void
    public constructor ()
  }

  class BuildingDTO extends CS.System.Object {
    protected [__keep_incompatibility]: never;
    public get Id(): string;
    public get Name(): string;
    public get Description(): string;
    public get Price(): number;
    public get PopulationCost(): number;
    public get CanBuild(): boolean;
    public get LockReason(): string;
    public get Icon(): string;
    public get Category(): string;
  }
}

declare namespace CS.Heroes.Presentation.UI.ResourcesPanel {
  class KingdomResourcesPresenter {
    public get Gold(): number;
    public set Gold(value: number);
    public get Population(): number;
    public set Population(value: number);

    public add_OnGoldChanged ($value: System.Action$1<number>) : void
    public remove_OnGoldChanged ($value: System.Action$1<number>) : void
    public add_OnPopulationChanged ($value: System.Action$1<number>) : void
    public remove_OnPopulationChanged ($value: System.Action$1<number>) : void

    public constructor ()
  }
}

declare namespace CS.Heroes.Presentation.UI.SelectionPanel {
  class SelectionDTO extends CS.System.Object {
    protected [__keep_incompatibility]: never;
    public get Id(): string;
    public get Name(): string;
    public get Description(): string;
    public get Icon(): string;
  }

  class DamageableSelectionDTO extends CS.System.Object {
    protected [__keep_incompatibility]: never;
    public get CurrentHealth(): number;
    public get MaxHealth(): number;
  }

  class BuildingSelectionDTO extends CS.System.Object {
    protected [__keep_incompatibility]: never;
    public get IsAlive(): boolean;
  }

  class HeroSelectionDTO extends CS.System.Object {
    protected [__keep_incompatibility]: never;
    public get Gold(): number;
    public get GearLevel(): number;
    public get DangerLevel(): number;
    public get IsAlive(): boolean;
    public get IsInHome(): boolean;
    public get Attack(): number;
    public get Defence(): number;
    public get Speed(): number;
  }

  class HeroEquipmentItemDTO extends CS.System.Object {
    protected [__keep_incompatibility]: never;
    public get Name(): string;
    public get Icon(): string;
  }

  class HeroEquipmentSelectionDTO extends CS.System.Object {
    protected [__keep_incompatibility]: never;
    public get Weapon(): HeroEquipmentItemDTO;
    public get Armor(): HeroEquipmentItemDTO;
    public get Artifacts(): System.Array$1<HeroEquipmentItemDTO>;
    public get Consumables(): System.Array$1<HeroEquipmentItemDTO>;
    public get Backpack(): System.Array$1<HeroEquipmentItemDTO>;
  }

  class ShopItemSelectionDTO extends CS.System.Object {
    protected [__keep_incompatibility]: never;
    public get Id(): string;
    public get Name(): string;
    public get Description(): string;
    public get Icon(): string;
    public get GoldCost(): number;
    public get Attack(): number;
    public get Defense(): number;
    public get Speed(): number;
    public get HpRegeneration(): number;
    public get Slot(): string;
    public get IsSingleUse(): boolean;
    public get IsUnlocked(): boolean;
    public get LockReason(): string;
  }

  class GoapGoalSelectionDTO extends CS.System.Object {
    protected [__keep_incompatibility]: never;
    public get Name(): string;
    public get Value(): number;
    public get Icon(): string;
    public get Description(): string;
    public get IsActive(): boolean;
  }

  class GoapPlanStepSelectionDTO extends CS.System.Object {
    protected [__keep_incompatibility]: never;
    public get Name(): string;
    public get Description(): string;
    public get Value(): number;
    public get PreconditionsMet(): boolean;
  }

  class GoapSelectionDTO extends CS.System.Object {
    protected [__keep_incompatibility]: never;
    public get Goals(): System.Array$1<Heroes.Presentation.UI.SelectionPanel.GoapGoalSelectionDTO>;
    public get Steps(): System.Array$1<Heroes.Presentation.UI.SelectionPanel.GoapPlanStepSelectionDTO>;
  }

  class BuildingUpgradeSelectionDTO extends CS.System.Object {
    protected [__keep_incompatibility]: never;
    public get Id(): string;
    public get Name(): string;
    public get Description(): string;
    public get Price(): number;
    public get Icon(): string;
    public get IsQueued(): boolean;
    public get IsActive(): boolean;
    public get IsCompleted(): boolean;
    public get CanQueue(): boolean;
    public get LockReason(): string;
    public get Progress(): number;
    public get QueueIndex(): number;
  }

  class QueuedBuildingUpgradeSelectionDTO extends CS.System.Object {
    protected [__keep_incompatibility]: never;
    public get Id(): string;
    public get Name(): string;
    public get Description(): string;
    public get Icon(): string;
    public get Order(): number;
  }

  class SelectionPanelPresenter {
    public get Selected(): null | SelectionDTO;
    public get SelectedDamageable(): null | DamageableSelectionDTO;
    public get SelectedBuilding(): null | BuildingSelectionDTO;
    public get SelectedHero(): null | HeroSelectionDTO;
    public get SelectedHeroEquipment(): null | HeroEquipmentSelectionDTO;
    public get SelectedGoap(): null | GoapSelectionDTO;
    public get ShopItems(): System.Array$1<Heroes.Presentation.UI.SelectionPanel.ShopItemSelectionDTO>;
    public get BuildingUpgrades(): System.Array$1<Heroes.Presentation.UI.SelectionPanel.BuildingUpgradeSelectionDTO>;
    public get QueuedBuildingUpgrades(): System.Array$1<Heroes.Presentation.UI.SelectionPanel.QueuedBuildingUpgradeSelectionDTO>;
    public SelectUpgrade ($upgradeId: string) : void

    public add_OnSelectedChanged ($value: System.Action$1<SelectionDTO>) : void
    public remove_OnSelectedChanged ($value: System.Action$1<SelectionDTO>) : void

    public constructor ()
  }
}

declare namespace CS.System {
  interface Array$1<T> extends CS.System.Array {
    get_Item(index: number): T;
    set_Item(index: number, value: T): void;
    Length: number;
  }
}

declare const buildingPanelPresenter: CS.Heroes.Presentation.UI.BuildingPanel.BuildingPanelPresenter;
declare const kingdomResourcesPanelPresenter: CS.Heroes.Presentation.UI.ResourcesPanel.KingdomResourcesPresenter;
declare const selectionPanelPresenter: CS.Heroes.Presentation.UI.SelectionPanel.SelectionPanelPresenter;

interface MaskElement extends JSX.VisualElement {
  masksrc?: string;
}

const MaskElement: typeof OneJS.Dom.MaskElement;

declare namespace JSX {
  interface IntrinsicElements {
    maskelement: MaskElement;
  }
}
