import { cva } from 'class-variance-authority';
import { h, render } from 'preact'
import { Texture2D } from 'UnityEngine';
import { Icon } from './icon';

type Props = {
  variant?: "default" | "active";
  icon: Texture2D;
};

const iconFrameStyles = cva(
  "w-full h-full flex items-center justify-center",
  {
    variants: {
      variant: {
        default: "bg-tertiary border-2 border-main",
        active: "bg-tertiaryLight border-2 border-active",
      }
    }
  }
);

export function IconFrame({ variant = "default", icon}: Props) {
  return (
    <div class={iconFrameStyles({ variant })}>
      <Icon icon={icon} />
    </div>
  );
}