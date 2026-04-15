import { h } from "onejs-preact";
import { Bar } from "./bar";
import { DecorativeFrame } from "./decorative-frame";

export function MiniMap() {
  return (
    <div>
      <DecorativeFrame size={133} mask="Masks/minimap">
        <img src="https://picsum.photos/2137" class="w-full h-full object-cover" />
      </DecorativeFrame>
      
      <div class="w-full h-[3px]" />

      <Bar details={1} title="Map"></Bar>
    </div>
  );
}