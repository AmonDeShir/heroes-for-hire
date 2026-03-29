declare namespace CS {
    namespace Heroes {
        export interface BuildingPanelViewModel {
          public add_OnSelectedChanged(handler: Function): void
          public remove_OnSelectedChanged(handler: Function): void

          public add_OnBuildingsChanged(handler: Function): void
          public remove_OnBuildingsChanged(handler: Function): void

          public Selected: string
          public Buildings: BuildingDTO[]
        }

        export interface BuildingDTO {
          public Id: string
          public Name: string
          public Description: string
          public Price: number
          public Icon: string
          public Category: string
        }
    }
}
 
declare const buildingPanelVM: CS.Heroes.BuildingPanelViewModel;