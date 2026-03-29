import { Texture2D } from "UnityEngine";
import { h, render } from 'preact'

type IconProps = {
  icon: Texture2D;
  class?: string;
};

export function Icon({ icon, class: className }: IconProps) {
  return (
    <div
      class={`w-full h-full bg-center bg-contain bg-no-repeat ${className ?? ""}`}
      style={{ backgroundImage: icon }}
    />
  );
}