// @ts-ignore
console.log("[index.tsx]: OneJS is good to go")

import { h, render } from 'preact'
import { Heroes } from './src/panels/heroes'
import { MiniMap } from './src/components/min-map'
import { MainPanel } from './src/panels/main-panel'


function App() {
  return (
    <div class="w-full h-full flex justify-between p-1 pb-2">
      <div class="w-full flex flex-row justify-between">
        <Heroes />
        <MiniMap />
      </div>

      <div class="w-full h-[142px] flex flex-row justify-center">
        <MainPanel />
      </div>
    </div>
  )
}
 
render(<App />, document.body)