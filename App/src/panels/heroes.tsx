import { h } from "onejs-preact";
import { Bar } from "../components/bar";

export function Heroes() {
  return (
    <div style={{ width: 200 }}>
      <Bar details={1} title="Heroes">
      </Bar>  
    </div>
    
  );
}