const http = require("http");

const requestData = {
  x: 2,
  y: 3,
  s: "sample",
  m: ["a", "b", "c"],
  o: { surname: "Ugorenko", name: "Violetta" }
};

const data = JSON.stringify(requestData);

const req = http.request(
  "http://localhost:5000/task04",
  {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
      "Content-Length": Buffer.byteLength(data)
    }
  },
  (res) => {
    const chunks = [];
    res.on("data", (chunk) => chunks.push(chunk));
    res.on("end", () => {
      const bodyText = Buffer.concat(chunks).toString("utf8");
      const bodyJson = JSON.parse(bodyText);
      console.log("Status:", res.statusCode, res.statusMessage);
      console.log("Body:", bodyJson);
    });
  }
);

req.on("error", (err) => {
  console.error("Request error:", err.message);
});

req.write(data);
req.end();
