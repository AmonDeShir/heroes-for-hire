import { h, render } from 'preact'
import { Panel } from '../components/panel'
import { IconButton } from '../components/icon-button'
import { Resources, Texture2D } from 'UnityEngine'
import { useEventfulState, useMemo } from 'onejs-preact/hooks'
import { DecorativeFrame } from '../components/decorative-frame'
import { Icon } from '../components/icon'
import { ProgressBar } from '../components/progress-bar'

const INFO_ICON = Resources.Load("Icons/info") as Texture2D
const HEART_ICON = Resources.Load("Icons/hearts") as Texture2D

export function InfoPanel() {
  const [selected] = useEventfulState(selectionPanelPresenter, "Selected");
  const [damageable] = useEventfulState(selectionPanelPresenter, "SelectedDamageable");
  const [building] = useEventfulState(selectionPanelPresenter, "SelectedBuilding");

  if (selected == null) {
    return <div></div>
  }

  const icon = useMemo(() => Resources.Load(selected.Icon) as Texture2D, [selected]);

  return (
    <div class="w-[850px]">
      <Panel title={selected.Name}>
        <div class='w-full h-full p-[2px] flex flex-row '>
          <div class='flex w-7 h-full flex-col justify-start items-start'>
            <IconButton active icon={INFO_ICON} onClick={() => console.log("I can be activated!")}/>
          </div>
          
          <div class='flex w-full h-full flex-row justify-center items-start'>
            <div class='flex-shrink-0 flex flex-col w-[78px] h-full justify-center items-center ml-2'>
              <DecorativeFrame>
                <div class='w-full h-full p-2 bg-tertiary'>
                  <Icon icon={icon} />
                </div>
              </DecorativeFrame>
            </div>
            
            <div class='w-0.5 h-[70px] bg-main ml-4 mr-4 my-auto' />
          
            <div class='flex h-full items-start justify-evenly'>
              {damageable && (
                <div>
                  <ProgressBar 
                    icon={HEART_ICON} 
                    max={damageable.MaxHealth} 
                    value={damageable.CurrentHealth} 
                    text='Health' 
                    displayValue 
                  />
                </div>
                )
              }

              <div class='font-[10px]'>
                {selected.Description}
              </div>
            </div>
          </div>
        </div>
      </Panel>
    </div>
  )
}