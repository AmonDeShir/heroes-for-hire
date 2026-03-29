import { h } from "onejs-preact";
import { Bar } from "./bar";

export function MiniMap() {
  return (
    <div>
      <div class="w-[133px] h-[133px] border border-b-0 border-main"></div>
      <Bar details={1} title="Mapa"></Bar>
    </div>
  );
}