import React from "react";

function Output({ outputValue }) {
  return (
    <section className="output">
      <textarea id="output" readOnly wrap="off" value={outputValue} />
    </section>
  );
}

export default Output;
