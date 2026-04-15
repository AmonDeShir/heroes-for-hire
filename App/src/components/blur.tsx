import { h } from "onejs-preact";
import { useEffect, useRef, useState } from "onejs-preact/hooks";
import { Screen } from "UnityEngine";
import { getVisualElement, useRenderTexture } from "../hooks/use-render-texture";

type Props = { 
  children?: any, 
  blur?: number
  color?: string
  opacity?: number,
};

export function Blur({ children, blur = 5, color = "white", opacity = 0.1 }: Props) {
  const imageRef = useRenderTexture("CameraTexture");
  const parentRef = useRef<HTMLElement>(null);
  const [pos, setPos] = useState({w: 0, h: 0, l: 0, t: 0});

  useEffect(() => {
    const image = getVisualElement(imageRef.current);
    const parentImage = getVisualElement(parentRef.current);

    if (!image || !parentImage) {
      return;
    }

    const bound = parentImage.worldBound;

    setPos({
      w: Screen.width / image.scaledPixelsPerPoint,
      h: Screen.height / image.scaledPixelsPerPoint,
      l: -Math.round(bound.x),
      t: -Math.round(bound.y),
    });
  }, []);

  return (
    <div ref={parentRef} class={"w-full h-full overflow-hidden"}>
      <div 
        ref={imageRef} 
        style={{
          position: "absolute",
          top: pos.t, 
          left: pos.l, 
          height: pos.h, 
          width: pos.w,
          filter: `blur(${blur}px)`
        }}
      />
      
      <div class="absolute top-0 left-0 w-full h-full" style={{ backgroundColor: color, opacity }} />

      {children}
    </div>
  );
}