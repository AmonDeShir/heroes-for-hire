import { h, render } from 'preact'
import { useEffect, useEventfulState, useState } from 'onejs-preact/hooks'
import { BuildingPanel } from './building-panel'
import { InfoPanel } from './info-panel'
import { Debug } from 'UnityEngine';

export function MainPanel() {
  const [mode, setMode] = useState("buildings" as "buildings" | "info");

  const [selected] = useEventfulState(selectionPanelPresenter, "Selected");

  useEffect(() => {
    console.log("Selected changed:", selected);

    if (selected != null) {
        setMode("info");
    }
    else {
      setMode("buildings");
    }

  }, [selected]);

  if (mode === "buildings") {
    Debug.Log("Rendering building panel");

    return (
      <BuildingPanel />
    )
  }

  if (mode === "info") {
    Debug.Log("Rendering info panel");

    return (
      <InfoPanel />
    )
  }

  return (<div></div>)
}