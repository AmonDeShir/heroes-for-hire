import { h, render } from 'preact'
import { Panel } from '../components/panel'
import { IconButton } from '../components/icon-button'
import { Resources, Texture2D } from 'UnityEngine'

const Image = Resources.Load("capybara") as Texture2D

export function InfoPanel() {
  return (
    <div class="w-[850px]">
      <Panel title='Info'>
        <div class='w-full h-full p-[2px]'>
          <IconButton active icon={Image} onClick={() => console.log("I can be activated!")}/>
          <IconButton icon={Image} onClick={() => console.log("I can be activated!")}/>
          <div>XD Test</div>
        </div>
      </Panel>
    </div>
  )
}