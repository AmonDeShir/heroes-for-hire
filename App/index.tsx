// @ts-ignore
console.log("[index.tsx]: OneJS is good to go")

import { h, render } from 'preact'
import { useState, useEffect, useMemo, useRef } from 'preact/hooks'
import { Bar } from './src/components/bar'
 
function App() {
  return (
    <div class="w-full h-full justify-center items-center">

      <div class="absolute w-full h-[142px] flex flex-row justify-center bottom-2">
        <div class="w-[850px]">
          <Bar details={6} title='Budynki'/>
        </div>
      </div>
    </div>
  )
}
 
render(<App />, document.body)