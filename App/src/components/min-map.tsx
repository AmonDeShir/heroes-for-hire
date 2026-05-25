import { h } from "onejs-preact";
import { Bar } from "./bar";
import { DecorativeFrame } from "./decorative-frame";
import { useRenderTexture } from "../hooks/use-render-texture";
import { Resources, Texture2D } from "UnityEngine";
import { ResourceBar } from "./resource-bar";
import { useEventfulState } from "onejs-preact/hooks";
import { IconButton } from "./icon-button";

const COIN_ICON = Resources.Load("Icons/coin") as Texture2D;
const PEOPLE_ICON = Resources.Load("Icons/buildings-civilian") as Texture2D;
const QUEST_ICON = (Resources.Load("Icons/all/lorc/contract") as Texture2D) || COIN_ICON;

export function MiniMap() {
  const imageRef = useRenderTexture("MiniMapTexture");
  const [gold] = useEventfulState(kingdomResourcesPanelPresenter, "Gold");
  const [population] = useEventfulState(kingdomResourcesPanelPresenter, "Population");
  const [armed] = useEventfulState(questPanelPresenter, "CombatArmed");

  return (
    <div class="relative">
      <div class="absolute w-[100px] left-[-100px] h-[133px] flex flex-col justify-center">
        <ResourceBar value={gold} icon={COIN_ICON} />
        <ResourceBar value={population} icon={PEOPLE_ICON} />
      </div>

      <DecorativeFrame size={133} mask="Masks/minimap">
        <div ref={imageRef} class="w-full h-full bg-cover" />
      </DecorativeFrame>
      
      <div class="w-full h-[3px]" />

      <Bar details={1} title="Map"></Bar>

      <div class="flex flex-row" style={{ marginTop: 6 }}>
        <IconButton
          icon={QUEST_ICON}
          active={armed}
          onClick={() => questPanelPresenter.ArmCombatQuest()}
        />
      </div>
    </div>
  );
}
