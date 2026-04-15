import { useEffect, useRef, useState } from "onejs-preact/hooks";

export function useAnimateNumber(value: number) {
  const [animatedValue, setAnimatedValue] = useState(value);
  const targetRef = useRef(value);

  useEffect(() => {
    targetRef.current = value;

    let frame: number;

    const animate = () => {
      setAnimatedValue((prev) => {
        const target = targetRef.current;

        const diff = target - prev;
        const speed = Math.min(Math.abs(diff) * 0.15, 20);
        const next = prev + Math.sign(diff) * speed;

        if (Math.abs(diff) < 0.5) {
          return Math.floor(target);
        }
        
        frame = requestAnimationFrame(animate);
        
        return Math.floor(next);
      });
    };

    frame = requestAnimationFrame(animate);

    return () => cancelAnimationFrame(frame);
  }, [value]);

  return animatedValue;
}