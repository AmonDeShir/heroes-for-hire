import { h } from "onejs-preact";
import { Bar } from "./bar";
import { DecorativeFrame } from "./decorative-frame";
import { useRenderTexture } from "../hooks/use-render-texture";

export function MiniMap() {
  const imageRef = useRenderTexture("MiniMapTexture");

  return (
    <div>
      <DecorativeFrame size={133} mask="Masks/minimap">
        <div ref={imageRef} class="w-full h-full bg-cover" />
      </DecorativeFrame>
      
      <div class="w-full h-[3px]" />

      <Bar details={1} title="Map"></Bar>
    </div>
  );
}