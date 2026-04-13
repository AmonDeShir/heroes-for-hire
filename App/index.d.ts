
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
    public get Icon(): string;
    public get Category(): string;
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
