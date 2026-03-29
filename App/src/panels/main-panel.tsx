import { h, render } from 'preact'
import { useState } from 'onejs-preact/hooks'
import { BuildingPanel } from './building-panel'
import { InfoPanel } from './info-panel'

export function MainPanel() {
  const [mode, setMode] = useState("buildings" as "buildings" | "info");

  if (mode === "buildings") {
    return (
      <BuildingPanel />
    )
  }

  if (mode === "info") {
    return (
      <InfoPanel />
    )
  }

  return (<div></div>)
}