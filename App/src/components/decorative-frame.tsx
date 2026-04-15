import { useMemo } from 'onejs-preact/hooks';
import { h, render, ComponentChildren } from 'preact'
import { Material, Resources, Texture2D } from 'UnityEngine';
import clsx from 'clsx';

const mask = Resources.Load("FrameMask") as Material;

type Props = {
  size?: number,
  mask?: string,
  color?: "main" | "secondary" | "active" | "disabled",
  children?: ComponentChildren,
}

export function DecorativeFrame({ children, size = 78, mask = "Masks/mask", color = "main"  }: Props) {
  return (
    <div class="relative" style={{ width: size, height: size }}>
      <maskelement masksrc={mask} class="w-full h-full" style={{ scale: 0.99 }}>
        {children}
      </maskelement>

      <Corner size={size} color={color} top={0} left={0} />
      <Corner size={size} color={color} top={0} right={0} rotate="90deg" />
      <Corner size={size} color={color} bottom={0} right={0} rotate="180deg" />
      <Corner size={size} color={color} bottom={0} left={0} rotate="270deg" />
    </div>
  );
}

type CornerProps = {
  top?: number,
  left?: number,
  right?: number,
  bottom?: number,
  rotate?: string,
  size?: number,
  color: "main" | "secondary" | "active" | "disabled",
}

function Corner({top, left, right, bottom, rotate, size = 78, color}: CornerProps) {
  const freeSize = useMemo(() => size/2 - 18, [size]);
  const colorClass = clsx(
    color == "main" && "bg-main",
    color == "secondary" && "bg-secondary",
    color == "active" && "bg-active",
    color == "disabled" && "bg-disabled",
  );

  return (
    <div class="absolute w-[39px] h-[39px]" style={{ top, left, right, bottom, rotate }}>
      <div class={clsx("absolute transition-colors left-0 top-0 w-[12px] h-[2px]", colorClass)} />
      <div class={clsx("absolute transition-colors left-0 top-0 w-[2px] h-[12px]", colorClass)} />
      <div class={clsx("absolute transition-colors left-0 top-[10px] w-[12px] h-[2px]", colorClass)} />
      <div class={clsx("absolute transition-colors left-[10px] top-0 w-[2px] h-[12px]", colorClass)} />
      <div class={clsx("absolute transition-colors left-[10px] top-[12px] w-[2px] h-[4px]", colorClass)} />
      <div class={clsx("absolute transition-colors left-0 top-[16px] w-[12px] h-[2px]", colorClass)} />
      <div class={clsx("absolute transition-colors left-0 top-[18px] w-[2px] h-[21px]", colorClass)} style={{ height: freeSize }} />
      <div class={clsx("absolute transition-colors left-[18px] top-0 w-[21px] h-[2px]", colorClass)} style={{ width: freeSize }} />
      <div class={clsx("absolute transition-colors left-[12px] top-[10px] w-[4px] h-[2px]", colorClass)} />
      <div class={clsx("absolute transition-colors left-[16px] top-0 w-[2px] h-[12px]", colorClass)} />
    </div>
  );
}
