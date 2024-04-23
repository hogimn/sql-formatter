import Input from "./Input";
import React, { useState } from "react";
import Output from "./Output";

function Body() {
  const [outputValue, setOutputValue] = useState("");

  const handleInputChange = (inputValue) => {
    if (!!inputValue.trim()) {
      fetch("http://localhost:5073/format_sql", {
        method: "POST",
        mode: "cors",
        body: inputValue,
      })
        .then((response) => response.text())
        .then((result) => setOutputValue(result))
        .catch((error) => {
          alert(error);
        });
    } else {
      setOutputValue("");
    }
  };

  return (
    <>
      <main>
        <Input onInputChange={handleInputChange} />
        <Output outputValue={outputValue} />
      </main>
    </>
  );
}

export default Body;
